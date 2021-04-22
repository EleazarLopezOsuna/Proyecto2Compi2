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
        private int temporalSwitch;
        private int temporalSalidaSwitch;
        public SegundaPasada(int contadorEtiqueta)
        {
            posicionRelativa = 0;
            declaracion = new Declaracion(0);
            contadorTemporal = 0;
            temporalSwitch = 0;
            temporalSalidaSwitch = 0;
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
            int etiquetaVerdadera;
            int etiquetaFalsa;
            int etiquetaInicial;
            int etiquetaSalida;
            int tmp;
            string nombreVariable;
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
                        etiquetaSalida = contadorEtiqueta;
                        contadorEtiqueta++;
                        etiquetaFalsa = contadorEtiqueta;
                        traduccion += "goto L" + etiquetaFalsa + "; //Movimiento a etiqueta falsa" + Environment.NewLine;
                        traduccion += "L" + etiquetaVerdadera + ":" + Environment.NewLine;
                        recorrer(root.ChildNodes[1], entorno);
                        traduccion += "goto L" + etiquetaSalida + "; //Movimiento a etiqueta de escape del if" + Environment.NewLine;
                        traduccion += "L" + etiquetaFalsa + ":" + Environment.NewLine;
                        recorrer(root.ChildNodes[2], entorno);
                        //Codigo para salir del if cuando fue verdadero
                        traduccion += "L" + etiquetaSalida + ": //Salio del if" + Environment.NewLine;
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
                        etiquetaSalida = contadorEtiqueta;
                        traduccion += "goto L" + etiquetaSalida + "; //Movimiento a etiqueta falsa" + Environment.NewLine;
                        traduccion += "L" + etiquetaVerdadera + ":" + Environment.NewLine;
                        recorrer(root.ChildNodes[1], entorno);
                        //Codigo para salir del if sin importar si fue verdadero o falso
                        traduccion += "L" + etiquetaSalida + ": //Salio del if" + Environment.NewLine;
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
                    if (declaracion.contadorTemporal > contadorTemporal)
                        contadorTemporal = declaracion.contadorTemporal;
                    contadorEtiqueta++;
                    etiquetaVerdadera = contadorEtiqueta;
                    traduccion += "if (T" + declaracion.contadorTemporal + ") goto L" + etiquetaVerdadera + "; //Movimiento hacia etiqueta de ejecucion del WHILE" + Environment.NewLine;
                    contadorEtiqueta++;
                    etiquetaSalida = contadorEtiqueta;
                    traduccion += "goto L" + etiquetaSalida + ";" + Environment.NewLine;
                    traduccion += "L" + etiquetaVerdadera + ":" + Environment.NewLine;
                    declaracion.contadorTemporal = 0;
                    recorrer(root.ChildNodes[1], entorno);
                    traduccion += "goto L" + etiquetaInicial + ";" + Environment.NewLine;
                    traduccion += "L" + etiquetaSalida + ":" + Environment.NewLine;
                    break;
                case "REPEAT":
                    // repeat instrucciones instrucciones until condicion
                    //              0               1               2
                    contadorEtiqueta++;
                    etiquetaInicial = contadorEtiqueta;
                    traduccion += "L" + etiquetaInicial + ": //Etiqueta que permite generar el ciclo" + Environment.NewLine;
                    declaracion.contadorTemporal = 0;
                    traduccion += "//Inicio de instrucciones pertenecientes al bloque REPEAT" + Environment.NewLine;
                    recorrer(root.ChildNodes[0], entorno);
                    recorrer(root.ChildNodes[1], entorno);
                    traduccion += "//Fin de instrucciones pertenecientes al bloque REPEAT" + Environment.NewLine;
                    traduccion += "//Inicio de calculo de la expresion condicional del REPEAT" + Environment.NewLine;
                    declaracion.contadorTemporal = 0;
                    str = declaracion.ResolverExpresion(root.ChildNodes[2], entorno) + Environment.NewLine;
                    traduccion += str;
                    traduccion += "//Fin de calculo de la expresion condicional del REPEAT" + Environment.NewLine;
                    if (declaracion.contadorTemporal > contadorTemporal)
                        contadorTemporal = declaracion.contadorTemporal;
                    traduccion += "if (!T" + declaracion.contadorTemporal + ") goto L" + etiquetaInicial + "; //Movimiento hacia el ciclo REPEAT" + Environment.NewLine;
                    contadorEtiqueta++;
                    etiquetaSalida = contadorEtiqueta;
                    traduccion += "goto L" + etiquetaSalida + ";" + Environment.NewLine;
                    traduccion += "L" + etiquetaSalida + ":" + Environment.NewLine;
                    break;
                case "FOR":
                    // for asignacion TO-DOWNTO instrucciones
                    //         0           1         2
                    contadorEtiqueta++;
                    traduccion += "//Inicializacion de variable" + Environment.NewLine;
                    etiquetaInicial = contadorEtiqueta;
                    tmp = contadorTemporal;
                    recorrer(root.ChildNodes[0], entorno);
                    traduccion += "L" + etiquetaInicial + ": //Etiqueta que permite generar el ciclo FOR" + Environment.NewLine;
                    nombreVariable = removerExtras(root.ChildNodes[0].ChildNodes[0].ChildNodes[0].ToString());
                    simbolo = entorno.buscar(removerExtras(nombreVariable));
                    traduccion += "T" + tmp + " = STACK[" + simbolo.direccionRelativa + "];" + Environment.NewLine;
                    declaracion.contadorTemporal = 0;
                    traduccion += "//Inicio de calculo de la expresion condicional del FOR" + Environment.NewLine;
                    declaracion.contadorTemporal = 0;
                    str = declaracion.ResolverExpresion(root.ChildNodes[1].ChildNodes[0], entorno);
                    //tmp = contadorTemporal;
                    if (!str.StartsWith("T"))
                    {
                        contadorTemporal++;
                        str = "T" + contadorTemporal + " = " + str;
                    }
                    traduccion += str + Environment.NewLine;
                    traduccion += "//Fin de calculo de la expresion condicional del FOR" + Environment.NewLine;
                    contadorEtiqueta++;
                    etiquetaVerdadera = contadorEtiqueta;
                    contadorEtiqueta++;
                    etiquetaSalida = contadorEtiqueta;
                    if (root.ChildNodes[1].ToString().Equals("ARRIBA"))
                    {
                        //TO
                        traduccion += "if (T" + tmp + " <= T" + contadorTemporal + ") goto L" + etiquetaVerdadera + ";" + Environment.NewLine;
                    }
                    else
                    {
                        //DOWN TO
                        traduccion += "if (T" + tmp + " >= T" + contadorTemporal + ") goto L" + etiquetaVerdadera + ";" + Environment.NewLine;
                    }
                    if (declaracion.contadorTemporal > contadorTemporal)
                        contadorTemporal = declaracion.contadorTemporal;
                    traduccion += "goto L" + etiquetaSalida + ";" + Environment.NewLine;
                    traduccion += "//Inicio de instrucciones pertenecientes al bloque FOR" + Environment.NewLine;
                    traduccion += "L" + etiquetaVerdadera + ": //Etiqueta verdadera, se ejecutan las instrucciones" + Environment.NewLine;
                    recorrer(root.ChildNodes[2], entorno);

                    contadorTemporal++;
                    traduccion += "T" + contadorTemporal + " = STACK[" + simbolo.direccionAbsoluta + "] + 1 ;" + Environment.NewLine;
                    traduccion += "STACK[" + simbolo.direccionAbsoluta + "] = T" + contadorTemporal + ";" + Environment.NewLine;

                    traduccion += "goto L" + etiquetaInicial + ";" + Environment.NewLine;
                    traduccion += "//Fin de instrucciones pertenecientes al bloque FOR" + Environment.NewLine;
                    traduccion += "L" + etiquetaSalida + ": //Salida del FOR" + Environment.NewLine; 
                    break;
                case "CASE":
                    //Case
                    // expresion case listaCase
                    //     0       1      2
                    traduccion += "//Inicio de calculo de la expresion del SWITCH" + Environment.NewLine;
                    declaracion.contadorTemporal = 0;
                    str = declaracion.ResolverExpresion(root.ChildNodes[0], entorno);
                    //tmp = contadorTemporal;
                    if (!str.StartsWith("T"))
                    {
                        contadorTemporal++;
                        str = "T" + contadorTemporal + " = " + str;
                    }
                    temporalSwitch = contadorTemporal;
                    contadorTemporal++;
                    contadorEtiqueta++;
                    temporalSalidaSwitch = contadorEtiqueta;
                    traduccion += str + Environment.NewLine;
                    traduccion += "//Fin de calculo de la expresion del SWITCH" + Environment.NewLine;
                    recorrer(root.ChildNodes[1], entorno);
                    recorrer(root.ChildNodes[2], entorno);
                    if (root.ChildNodes.Count == 4)
                    {
                        //Case-else
                        // expresion case listaCase sentencia(else)
                        //     0       1      2            3
                        recorrer(root.ChildNodes[3], entorno);
                    }
                    traduccion += "L" + temporalSalidaSwitch + ": //Salida del SWITCH" + Environment.NewLine;
                    break;
                case "OPCION_CASE":
                    if (root.ChildNodes.Count > 0)
                    {
                        if (!root.ChildNodes[0].ToString().Equals("OPCION_CASE"))
                        {
                            traduccion += "//Inicio de calculo de expresion del CASE" + Environment.NewLine;
                            declaracion.contadorTemporal = 0;
                            str = declaracion.ResolverExpresion(root.ChildNodes[0], entorno);
                            if (declaracion.contadorTemporal > contadorTemporal)
                                contadorTemporal = declaracion.contadorTemporal;
                            //tmp = contadorTemporal;
                            if (!str.StartsWith("T"))
                            {
                                contadorTemporal++;
                                str = "T" + contadorTemporal + " = " + str;
                            }
                            traduccion += str + Environment.NewLine;
                            traduccion += "//Fin de calculo de expresion del CASE" + Environment.NewLine;
                            contadorEtiqueta++;
                            etiquetaVerdadera = contadorEtiqueta;
                            contadorEtiqueta++;
                            etiquetaSalida = contadorEtiqueta;
                            contadorEtiqueta++;
                            etiquetaFalsa = contadorEtiqueta;
                            traduccion += "if (T" + contadorTemporal + " == T" + temporalSwitch + ") goto L" + etiquetaVerdadera + "; //Entra al case en caso que se cumpla la condicion" + Environment.NewLine;
                            declaracion.contadorTemporal = 0;
                            traduccion += "goto L" + etiquetaFalsa + "; //Movimiento a la etiqueta falsa" + Environment.NewLine;
                            traduccion += "L" + etiquetaVerdadera + ":" + Environment.NewLine;
                            contadorTemporal++;
                            recorrer(root.ChildNodes[1], entorno);
                            traduccion += "goto L" + temporalSalidaSwitch + "; //Movimiento a etiqueta de escape del CASE" + Environment.NewLine;
                            traduccion += "L" + etiquetaFalsa + ":" + Environment.NewLine;
                            //Codigo para salir del if cuando fue verdadero
                            traduccion += "L" + etiquetaSalida + ": //Salio del if" + Environment.NewLine;
                        }
                        else
                        {
                            recorrer(root.ChildNodes[0], entorno);
                            recorrer(root.ChildNodes[1], entorno);
                        }
                    }
                    break;
                case "LLAMADA":
                    if (root.ChildNodes[0].ToString().Contains("writeln (id)"))
                    {
                        traduccion += "printf(\"" + removerExtras(root.ChildNodes[1].ChildNodes[0].ToString()) + "%c\", 10);" + Environment.NewLine; //Eliminar luego, es solo para control
                    }
                    break;
                case "ASIGNACION":
                    if (root.ChildNodes.Count == 2)
                    {
                        if (root.ChildNodes[0].ToString().Equals("VARIABLE"))
                        {
                            nombreVariable = removerExtras(root.ChildNodes[0].ChildNodes[0].ToString());
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



                                if (declaracion.contadorTemporal > contadorTemporal)
                                    contadorTemporal = declaracion.contadorTemporal;


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
