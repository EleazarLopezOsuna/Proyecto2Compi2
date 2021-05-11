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
        private int contadorEtiqueta;
        private string cuerpo;
        private string subProgramas;
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
                    Declaracion tradurcirDeclaracion = new Declaracion(contadorTemporal, contadorEtiqueta, corridaUno.entornoGlobal);
                    if (corridaUno.errores.Count == 0)
                    {
                        //No existieron errores al momento de crear las variables
                        string encabezado = "#include <stdio.h>" + Environment.NewLine +
                            "float HEAP[100000000];" + Environment.NewLine +
                            "float STACK[100000000];" + Environment.NewLine +
                            "float SP;" + Environment.NewLine +
                            "float HP;" + Environment.NewLine +
                            "float T_SP, T_HP, S_SP, S_HP;" + Environment.NewLine +
                            "float ";
                        subProgramas = "";
                        cuerpo = "void main(){" + Environment.NewLine;
                        traducirVariables(corridaUno.entornoGlobal, tradurcirDeclaracion);
                        SegundaPasada segundaPasada = new SegundaPasada(contadorEtiqueta, corridaUno.entornoGlobal);
                        segundaPasada.iniciarSegundaPasada(resultadoAnalisis.Root, 0, corridaUno.entornoGlobal);
                        if (segundaPasada.contadorTemporal > contadorTemporal)
                            contadorTemporal = segundaPasada.contadorTemporal;
                        cuerpo += segundaPasada.traduccion;
                        cuerpo += "return;" + Environment.NewLine +
                            "}";
                        for (int i = 0; i <= contadorTemporal; i++)
                        {
                            if (i == 0)
                                encabezado += "T" + i;
                            else
                                encabezado += ", T" + i;
                        }
                        encabezado += ";";
                        console_textbox.Text = encabezado + Environment.NewLine + subProgramas + cuerpo;
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

        private void traducirVariables(Entorno ent, Declaracion tradurcirDeclaracion)
        {
            Simbolo variable;
            Object[] traduccionVariable;
            cuerpo += "SP = 0;" + Environment.NewLine;
            foreach (KeyValuePair<string, Simbolo> llave in ent.tabla)
            {
                variable = llave.Value;
                if (variable.direccionHeap == -1)
                {
                    symbol_table.Rows.Add(llave.Key, variable.tipo, ent.nombreEntorno, variable.rol, "NA", variable.size, variable.direccionAbsoluta, variable.direccionRelativa, variable.fila + 1, variable.columna + 1);
                }
                else
                {
                    symbol_table.Rows.Add(llave.Key, variable.tipo, ent.nombreEntorno, variable.rol, variable.direccionHeap, variable.size, variable.direccionAbsoluta, variable.direccionRelativa, variable.fila + 1, variable.columna + 1);
                }
                traduccionVariable = tradurcirDeclaracion.Traducir(variable, ent, llave.Key);
                if (contadorTemporal < int.Parse(traduccionVariable[0].ToString()))
                {
                    contadorTemporal = int.Parse(traduccionVariable[0].ToString());
                }
                if (contadorEtiqueta < int.Parse(traduccionVariable[2].ToString()))
                {
                    contadorEtiqueta = int.Parse(traduccionVariable[2].ToString());
                }
                tradurcirDeclaracion.contadorTemporal = 0;
                if (variable.tipo != Simbolo.EnumTipo.procedimiento && variable.tipo != Simbolo.EnumTipo.funcion)
                {
                    if (traduccionVariable[1].ToString() != "")
                    {
                        cuerpo += traduccionVariable[1] + Environment.NewLine;
                    }
                }
                else
                {
                    if (traduccionVariable[1].ToString() != "")
                    {
                        subProgramas += traduccionVariable[1] + Environment.NewLine;
                    }
                }
                if (variable.tipo == Simbolo.EnumTipo.objeto || variable.tipo == Simbolo.EnumTipo.funcion || variable.tipo == Simbolo.EnumTipo.procedimiento)
                {
                    soloAgregar(variable.atributos, tradurcirDeclaracion);
                }
            }
        }

        private void soloAgregar(Entorno ent, Declaracion tradurcirDeclaracion)
        {
            Simbolo variable;
            foreach (KeyValuePair<string, Simbolo> llave in ent.tabla)
            {
                variable = llave.Value;
                if (variable.direccionHeap == -1)
                {
                    symbol_table.Rows.Add(llave.Key, variable.tipo, ent.nombreEntorno, variable.rol, "NA", variable.size, variable.direccionAbsoluta, variable.direccionRelativa, variable.fila + 1, variable.columna + 1);
                }
                else
                {
                    symbol_table.Rows.Add(llave.Key, variable.tipo, ent.nombreEntorno, variable.rol, variable.direccionHeap, variable.size, variable.direccionAbsoluta, variable.direccionRelativa, variable.fila + 1, variable.columna + 1);
                }
                if (variable.tipo == Simbolo.EnumTipo.objeto)
                {
                    soloAgregar(variable.atributos, tradurcirDeclaracion);
                }
            }
        }

        private void symbol_table_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
