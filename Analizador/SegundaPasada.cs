using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Irony.Parsing;
using Proyecto2_Compiladores2.Modelos;
using Proyecto2_Compiladores2.Traduccion;

namespace Proyecto2_Compiladores2.Analizador
{
    class SegundaPasada
    {
        public Entorno entorno;
        int posicionAbsoluta;
        public int posicionRelativa;
        public ArrayList errores;
        public string traduccion;
        private Declaracion declaracion;
        public int contadorTemporal;
        public int contadorEtiqueta;
        public SegundaPasada(int contadorEtiqueta)
        {
            posicionRelativa = 0;
            declaracion = new Declaracion(0);
            contadorTemporal = 0;
            this.contadorEtiqueta = contadorEtiqueta;
        }
        public void iniciarSegundaPasada(ParseTreeNode root, int posicionAbsoluta, Entorno entorno)
        {
            this.entorno = entorno;
            traduccion = "";
            this.posicionAbsoluta = posicionAbsoluta;
            errores = new ArrayList();
            recorrer(root, entorno);
        }
        private Expresion buscarVariable(ParseTreeNode root, Entorno entorno)
        {
            Simbolo resultadoBusqueda = entorno.buscar(removerExtras(root.ToString()));
            if (resultadoBusqueda is null)
            {
                return new Expresion(Simbolo.EnumTipo.error);
            }
            else
            {
                return new Expresion(resultadoBusqueda.tipo);
            }
        }
        private void recorrer(ParseTreeNode root, Entorno entorno)
        {
            Simbolo simbolo = null;
            string str;
            int etiquetaSalidaIf;
            int etiquetaVerdadera;
            int etiquetaFalsa;
            int etiquetaInicial;
            switch (root.ToString())
            {
                case "PROGRAMA":
                case "BEGIN_END":
                case "SENTENCIA":
                    if (root.ChildNodes.Count > 0)
                    {
                        foreach (ParseTreeNode hijo in root.ChildNodes)
                        {
                            recorrer(hijo, entorno);
                        }
                    }
                    break;
                case "IF":
                    if (root.ChildNodes.Count == 3)
                    {
                        //if expresion then instrucciones else instrucciones
                        //      0                1                 2
                        traduccion += "//Inicio de calculo de la expresion condicional de IF" + Environment.NewLine;
                        declaracion.contadorTemporal = 0;
                        str = declaracion.ResolverExpresion(root.ChildNodes[0], entorno) + Environment.NewLine;
                        traduccion += str;
                        traduccion += "//Fin de calculo de la expresion condicional de IF" + Environment.NewLine;
                        if (declaracion.contadorTemporal > contadorTemporal)
                            contadorTemporal = declaracion.contadorTemporal;
                        contadorEtiqueta++;
                        etiquetaVerdadera = contadorEtiqueta;
                        traduccion += "if (T" + declaracion.contadorTemporal + ") goto L" + etiquetaVerdadera + "; //Movimiento a etiqueta verdadera" + Environment.NewLine;
                        declaracion.contadorTemporal = 0;
                        contadorEtiqueta++;
                        etiquetaSalidaIf = contadorEtiqueta;
                        contadorEtiqueta++;
                        etiquetaFalsa = contadorEtiqueta;
                        traduccion += "goto L" + etiquetaFalsa + "; //Movimiento a etiqueta falsa" + Environment.NewLine;
                        traduccion += "L" + etiquetaVerdadera + ":" + Environment.NewLine;
                        recorrer(root.ChildNodes[1], entorno);
                        traduccion += "goto L" + etiquetaSalidaIf + "; //Movimiento a etiqueta de escape del if" + Environment.NewLine;
                        traduccion += "L" + etiquetaFalsa + ":" + Environment.NewLine;
                        recorrer(root.ChildNodes[2], entorno);
                        //Codigo para salir del if cuando fue verdadero
                        traduccion += "L" + etiquetaSalidaIf + ": //Salio del if" + Environment.NewLine;
                    }
                    else if (root.ChildNodes.Count == 2)
                    {
                        //if expresion then instrucciones
                        //      0                1
                        traduccion += "//Inicio de calculo de la expresion condicional de IF" + Environment.NewLine;
                        declaracion.contadorTemporal = 0;
                        str = declaracion.ResolverExpresion(root.ChildNodes[0], entorno) + Environment.NewLine;
                        traduccion += str;
                        traduccion += "//Fin de calculo de la expresion condicional de IF" + Environment.NewLine;
                        if (declaracion.contadorTemporal > contadorTemporal)
                            contadorTemporal = declaracion.contadorTemporal;
                        contadorEtiqueta++;
                        etiquetaVerdadera = contadorEtiqueta;
                        traduccion += "if (T" + declaracion.contadorTemporal + ") goto L" + etiquetaVerdadera + "; //Movimiento a etiqueta verdadera" + Environment.NewLine;
                        declaracion.contadorTemporal = 0;
                        contadorEtiqueta++;
                        etiquetaFalsa = contadorEtiqueta;
                        traduccion += "goto L" + etiquetaFalsa + "; //Movimiento a etiqueta falsa" + Environment.NewLine;
                        traduccion += "L" + etiquetaVerdadera + ":" + Environment.NewLine;
                        recorrer(root.ChildNodes[1], entorno);
                        //Codigo para salir del if sin importar si fue verdadero o falso
                        traduccion += "L" + etiquetaFalsa + ": //Salio del if" + Environment.NewLine;
                    }
                    break;
                case "WHILE":
                    //while expresion do instrucciones
                    //         0              1
                    contadorEtiqueta++;
                    etiquetaInicial = contadorEtiqueta;
                    traduccion += "L" + etiquetaInicial + ":" + Environment.NewLine;
                    traduccion += "//Inicio de calculo de la expresion condicional del WHILE" + Environment.NewLine;
                    declaracion.contadorTemporal = 0;
                    str = declaracion.ResolverExpresion(root.ChildNodes[0], entorno) + Environment.NewLine;
                    traduccion += str;
                    traduccion += "//Fin de calculo de la expresion condicional del WHILE" + Environment.NewLine;
                    MessageBox.Show(declaracion.contadorTemporal.ToString());
                    if (declaracion.contadorTemporal > contadorTemporal)
                        contadorTemporal = declaracion.contadorTemporal;
                    contadorEtiqueta++;
                    etiquetaVerdadera = contadorEtiqueta;
                    traduccion += "if (T" + declaracion.contadorTemporal + ") goto L" + etiquetaVerdadera + "; //Movimiento hacia etiqueta de ejecucion del WHILE" + Environment.NewLine;
                    contadorEtiqueta++;
                    etiquetaFalsa = contadorEtiqueta;
                    traduccion += "goto L" + etiquetaFalsa + ";" + Environment.NewLine;
                    traduccion += "L" + etiquetaVerdadera + ":" + Environment.NewLine;
                    declaracion.contadorTemporal = 0;
                    recorrer(root.ChildNodes[1], entorno);
                    traduccion += "goto L" + etiquetaInicial + ";" + Environment.NewLine;
                    traduccion += "L" + etiquetaFalsa + ":" + Environment.NewLine;
                    break;
                case "LLAMADA":
                    if (root.ChildNodes[0].ToString().Equals("writeln (id)"))
                    {
                        traduccion += "printf(\"" + removerExtras(root.ChildNodes[1].ChildNodes[0].ToString()) + "%c\", 10);" + Environment.NewLine; //Eliminar luego, es solo para control
                    }
                    break;
                case "ASIGNACION":
                    if (root.ChildNodes.Count == 2)
                    {
                        if (root.ChildNodes[0].ToString().Equals("VARIABLE"))
                        {
                            string nombreVariable = removerExtras(root.ChildNodes[0].ChildNodes[0].ToString());
                            simbolo = entorno.buscar(removerExtras(nombreVariable));
                            if (simbolo is null)
                            {
                                //AGREGAR ERROR La variable no existe
                            }
                            else
                            {
                                //identificador := expresion
                                //     0               1
                                str = declaracion.ResolverExpresion(root.ChildNodes[1], entorno) + Environment.NewLine;
                                if (!str.StartsWith("T"))
                                {
                                    contadorTemporal++;
                                    str = "//Inicio de modificacion de identificador " + nombreVariable + Environment.NewLine +
                                        "T" + contadorTemporal + " = " + str;
                                }
                                else
                                {
                                    str = "//Inicio de modificacion de identificador " + nombreVariable + Environment.NewLine + str;
                                }
                                traduccion += str;
                                if (declaracion.contadorTemporal > contadorTemporal)
                                {
                                    contadorTemporal = declaracion.contadorTemporal;
                                    traduccion += "STACK[" + simbolo.direccionAbsoluta + "] = T" + contadorTemporal + ";" + Environment.NewLine;
                                }
                                else
                                {
                                    if (declaracion.contadorTemporal == 0)
                                        declaracion.contadorTemporal = contadorTemporal;
                                    traduccion += "STACK[" + simbolo.direccionAbsoluta + "] = T" + declaracion.contadorTemporal + ";" + Environment.NewLine;
                                }
                                declaracion.contadorTemporal = 0;
                            }
                        }
                    }
                    break;
            }
        }
        private string removerExtras(string token)
        {
            token = token.Replace(" (id)", "");
            token = token.Replace(" (Keyword)", "");
            token = token.Replace(" (Key symbol)", "");
            token = token.Replace(" (entero)", "");
            token = token.Replace(" (cadena)", "");
            token = token.Replace(" (real)", "");
            token = token.Replace(" (boleano)", "");

            return token;
        }
    }
}
