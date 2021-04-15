using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Irony.Parsing;
using Proyecto2_Compiladores2.Analizador;
using Proyecto2_Compiladores2.Modelos;
using Proyecto2_Compiladores2.Traduccion;

namespace Proyecto2_Compiladores2
{
    public partial class Form1 : Form
    {
        private string fileName;
        private ParseTree resultadoAnalisis = null;
        private int posicionAbsoluta;
        private int contadorTemporal;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //dataGridView1.Rows.Add("Nombre", "Tipo", "Ambito", "Fila", "Columna", "Valor");
            console_textbox.Text = "";
            code_textbox.Text = "";
            fileName = "";

            //Parametros para el File Dialog
            openFileDialog1.InitialDirectory = @"C:\compiladores2\";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Pascal files (*.pas)|*.pas";


            openFileDialog1.ShowDialog();
            fileName = openFileDialog1.FileName;
            if (!fileName.Equals(""))
            {
                string textFromFile = File.ReadAllText(fileName);
                code_textbox.Text = textFromFile;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            resultadoAnalisis = null;
            resultadoAnalisis = Sintactico.Analizar(code_textbox.Text);
            error_table.Rows.Clear();
            symbol_table.Rows.Clear();
            console_textbox.Text = "";
            posicionAbsoluta = 0;
            contadorTemporal = 0;

            if (resultadoAnalisis != null)
            {
                if (resultadoAnalisis.ParserMessages.Count == 0)
                {
                    PrimeraPasada corridaUno = new PrimeraPasada();
                    corridaUno.iniciarPrimeraPasada(resultadoAnalisis.Root, posicionAbsoluta);
                    Declaracion tradurcirDeclaracion = new Declaracion(contadorTemporal);
                    if (corridaUno.errores.Count == 0)
                    {
                        //No existieron errores al momento de crear las variables
                        Simbolo variable;
                        string tipo;
                        Object[] traduccionVariable;
                        foreach (KeyValuePair<string, Simbolo> llave in corridaUno.entornoGlobal.tabla)
                        {
                            tipo = "Variable";
                            variable = llave.Value;
                            if (variable.constante)
                                tipo = "Constante";
                            symbol_table.Rows.Add(llave.Key, variable.tipo, "Global", tipo, variable.direccionAbsoluta, variable.direccionRelativa, variable.fila + 1, variable.columna + 1);
                            traduccionVariable = tradurcirDeclaracion.Traducir(variable, corridaUno.entornoGlobal);
                            contadorTemporal = int.Parse(traduccionVariable[0].ToString());
                            console_textbox.Text += traduccionVariable[1] + Environment.NewLine;
                        }
                        symbol_table.Visible = true;
                    }

                    //Graficar Arbol Irony
                    Sintactico.crearImagen(resultadoAnalisis.Root);

                    Thread.Sleep(1000);
                    var p = new Process();
                    //Abrir imagen Irony
                    p.StartInfo = new ProcessStartInfo(@"C:\compiladores2\ArbolIrony.png")
                    {
                        UseShellExecute = true
                    };
                    //p.Start();
                }
                else
                {
                    table_label.Text = "Error Table";
                    symbol_table.Visible = false;
                    error_table.Visible = true;

                    string mensajeTraducido = "";

                    foreach (Irony.LogMessage error in resultadoAnalisis.ParserMessages)
                    {
                        if (error.Message.Contains("expected"))
                            mensajeTraducido = error.Message.Replace("Syntax error, expected: ", "Se esperaba el token: ");
                        else
                            mensajeTraducido = "No se encontro simbolo para finalizar la cadena";
                        error_table.Rows.Add("Sintactico", mensajeTraducido, error.Location.Line + 1, error.Location.Column + 1);
                    }
                }
            }
            else
            {

            }
        }
    }
}
