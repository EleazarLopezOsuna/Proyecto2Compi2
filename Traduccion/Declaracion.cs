using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Irony.Parsing;
using Proyecto2_Compiladores2.Analizador;
using Proyecto2_Compiladores2.Modelos;

namespace Proyecto2_Compiladores2.Traduccion
{
    class Declaracion
    {
        public int contadorTemporal;
        public int contadorEtiqueta;
        private Declaracion declaracionTemp;
        private string acumulado;
        private Entorno entornoGlobal;
        public Declaracion(int contadorTemporal, int contadorEtiqueta, Entorno entornoGlobal)
        {
            this.contadorTemporal = contadorTemporal;
            this.contadorEtiqueta = contadorEtiqueta;
            this.entornoGlobal = entornoGlobal;
            acumulado = "";
        }
        public Object[] Traducir(Simbolo variable, Entorno entorno, string nombreVariable)
        {
            Object[] retorno = new Object[3];
            string resultadoTraduccion = "";
            int verdadero;
            int falso;
            if (variable.tipo == Simbolo.EnumTipo.arreglo)
            {
                if (variable.direccionHeap != -1)
                {
                    contadorEtiqueta++;
                    int direccionHeap = variable.direccionHeap;
                    int size = variable.size;
                    string dato = "0";
                    if (variable.contenido.tipo == Simbolo.EnumTipo.arreglo)
                    {
                        int tmpSize = variable.size;

                        int posicion = variable.direccionHeap;

                        dato = "0";
                        contadorTemporal++;

                        resultadoTraduccion += "T" + contadorTemporal + " = HP;//Guardamos la posicion de HP anterior" + Environment.NewLine;
                        int posicionAnterior = contadorTemporal;
                        contadorTemporal++;
                        resultadoTraduccion += "HP = " + posicion + ";//Actualizacion de HP" + Environment.NewLine;
                        resultadoTraduccion += "T" + contadorTemporal + " = HP; //Iniciacion del for" + Environment.NewLine;
                        //////resultadoTraduccion += "T" + contadorTemporal + " = " + posicion + "; //Iniciacion del for" + Environment.NewLine;

                        resultadoTraduccion += "L" + contadorEtiqueta + ": //Etiqueta para generar el loop" + Environment.NewLine;
                        contadorEtiqueta++;
                        verdadero = contadorEtiqueta;

                        resultadoTraduccion += "T_HP = HP + " + tmpSize + ";" + Environment.NewLine;
                        resultadoTraduccion += "if (T" + contadorTemporal + " < T_HP) goto L" + contadorEtiqueta + ";" + Environment.NewLine;
                        //////resultadoTraduccion += "if (T" + contadorTemporal + " < " + (tmpSize + posicion) + ") goto L" + contadorEtiqueta + ";" + Environment.NewLine;

                        contadorEtiqueta++;
                        falso = contadorEtiqueta;
                        resultadoTraduccion += "goto L" + contadorEtiqueta + ";" + Environment.NewLine;
                        resultadoTraduccion += "L" + verdadero + ": //Llenamos el arreglo" + Environment.NewLine;
                        resultadoTraduccion += "HEAP[(int)T" + contadorTemporal + "] = " + dato + ";" + Environment.NewLine;
                        resultadoTraduccion += "T" + contadorTemporal + " = T" + contadorTemporal + " + 1; //Incremento del for" + Environment.NewLine;
                        resultadoTraduccion += "goto L" + (verdadero - 1) + ";" + Environment.NewLine;
                        resultadoTraduccion += "L" + falso + ":" + Environment.NewLine;
                        contadorTemporal++;
                        contadorEtiqueta++;
                        resultadoTraduccion += "HP = T" + posicionAnterior + ";//Regresamos la posicion de HP" + Environment.NewLine;
                    }
                    else if (variable.contenido.tipo == Simbolo.EnumTipo.objeto)
                    {
                        int tmpSize;

                        int posicion = variable.direccionHeap;

                        for (int i = 0; i < (variable.size / variable.contenido.size); i++)
                        {
                            foreach (KeyValuePair<string, Simbolo> pair in variable.contenido.atributos.tabla)
                            {
                                tmpSize = pair.Value.size;
                                dato = "0";
                                if (pair.Value.tipo == Simbolo.EnumTipo.arreglo)
                                {
                                    declaracionTemp = new Declaracion(0, contadorEtiqueta, entornoGlobal);


                                    int tmpHP, tmpSP;

                                    if (variable.direccionHeap == -1)
                                    {
                                        tmpHP = -1;
                                        tmpSP = variable.direccionRelativa;
                                    }
                                    else
                                    {
                                        tmpSP = -1;
                                        tmpHP = variable.direccionHeap;
                                    }

                                    Object[] tmp = declaracionTemp.Traducir(pair.Value, entorno, nombreVariable);

                                    if (contadorTemporal < int.Parse(tmp[0].ToString()))
                                    {
                                        contadorTemporal = int.Parse(tmp[0].ToString());
                                    }
                                    resultadoTraduccion += tmp[1];
                                    contadorEtiqueta = int.Parse(tmp[2].ToString());
                                }
                                else if (pair.Value.tipo == Simbolo.EnumTipo.objeto)
                                {
                                    foreach (KeyValuePair<string, Simbolo> pareja in pair.Value.atributos.tabla)
                                    {
                                        tmpSize = pareja.Value.size;
                                        dato = "0";
                                        if (pareja.Value.tipo == Simbolo.EnumTipo.cadena)
                                        {
                                            dato = "-201700893";
                                        }
                                        contadorTemporal++;

                                        resultadoTraduccion += "T" + contadorTemporal + " = HP;//Guardamos la posicion de HP anterior" + Environment.NewLine;
                                        int posicionAnterior = contadorTemporal;
                                        contadorTemporal++;
                                        resultadoTraduccion += "HP = " + posicion + ";//Actualizacion de HP" + Environment.NewLine;
                                        resultadoTraduccion += "T" + contadorTemporal + " = HP; //Iniciacion del for" + Environment.NewLine;
                                        //////resultadoTraduccion += "T" + contadorTemporal + " = " + posicion + "; //Iniciacion del for" + Environment.NewLine;

                                        resultadoTraduccion += "L" + contadorEtiqueta + ": //Etiqueta para generar el loop" + Environment.NewLine;
                                        contadorEtiqueta++;
                                        verdadero = contadorEtiqueta;

                                        resultadoTraduccion += "T_HP = HP + " + tmpSize + ";" + Environment.NewLine;
                                        resultadoTraduccion += "if (T" + contadorTemporal + " < T_HP) goto L" + contadorEtiqueta + ";" + Environment.NewLine;
                                        //////resultadoTraduccion += "if (T" + contadorTemporal + " < " + (tmpSize + posicion) + ") goto L" + contadorEtiqueta + ";" + Environment.NewLine;

                                        contadorEtiqueta++;
                                        falso = contadorEtiqueta;
                                        resultadoTraduccion += "goto L" + contadorEtiqueta + ";" + Environment.NewLine;
                                        resultadoTraduccion += "L" + verdadero + ": //Llenamos el arreglo" + Environment.NewLine;
                                        resultadoTraduccion += "HEAP[(int)T" + contadorTemporal + "] = " + dato + ";" + Environment.NewLine;
                                        resultadoTraduccion += "T" + contadorTemporal + " = T" + contadorTemporal + " + 1; //Incremento del for" + Environment.NewLine;
                                        resultadoTraduccion += "goto L" + (verdadero - 1) + ";" + Environment.NewLine;
                                        resultadoTraduccion += "L" + falso + ":" + Environment.NewLine;
                                        contadorTemporal++;
                                        contadorEtiqueta++;
                                        posicion += tmpSize;
                                        resultadoTraduccion += "HP = T" + posicionAnterior + ";//Regresamos la posicion de HP" + Environment.NewLine;
                                    }
                                }
                                else
                                {
                                    if (pair.Value.tipo == Simbolo.EnumTipo.cadena)
                                    {
                                        dato = "-201700893";
                                    }
                                    contadorTemporal++;

                                    resultadoTraduccion += "T" + contadorTemporal + " = HP;//Guardamos la posicion de HP anterior" + Environment.NewLine;
                                    int posicionAnterior = contadorTemporal;
                                    contadorTemporal++;
                                    resultadoTraduccion += "HP = " + posicion + ";//Actualizacion de HP" + Environment.NewLine;
                                    resultadoTraduccion += "T" + contadorTemporal + " = HP; //Iniciacion del for" + Environment.NewLine;
                                    //////resultadoTraduccion += "T" + contadorTemporal + " = " + posicion + "; //Iniciacion del for" + Environment.NewLine;

                                    resultadoTraduccion += "L" + contadorEtiqueta + ": //Etiqueta para generar el loop" + Environment.NewLine;
                                    contadorEtiqueta++;
                                    verdadero = contadorEtiqueta;

                                    resultadoTraduccion += "T_HP = HP + " + tmpSize + ";" + Environment.NewLine;
                                    resultadoTraduccion += "if (T" + contadorTemporal + " < T_HP) goto L" + contadorEtiqueta + ";" + Environment.NewLine;
                                    //////resultadoTraduccion += "if (T" + contadorTemporal + " < " + (tmpSize + posicion) + ") goto L" + contadorEtiqueta + ";" + Environment.NewLine;

                                    contadorEtiqueta++;
                                    falso = contadorEtiqueta;
                                    resultadoTraduccion += "goto L" + contadorEtiqueta + ";" + Environment.NewLine;
                                    resultadoTraduccion += "L" + verdadero + ": //Llenamos el arreglo" + Environment.NewLine;
                                    resultadoTraduccion += "HEAP[(int)T" + contadorTemporal + "] = " + dato + ";" + Environment.NewLine;
                                    resultadoTraduccion += "T" + contadorTemporal + " = T" + contadorTemporal + " + 1; //Incremento del for" + Environment.NewLine;
                                    resultadoTraduccion += "goto L" + (verdadero - 1) + ";" + Environment.NewLine;
                                    resultadoTraduccion += "L" + falso + ":" + Environment.NewLine;
                                    contadorTemporal++;
                                    contadorEtiqueta++;
                                    posicion += tmpSize;
                                    resultadoTraduccion += "HP = T" + posicionAnterior + ";//Regresamos la posicion de HP" + Environment.NewLine;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (variable.contenido.tipo == Simbolo.EnumTipo.cadena)
                        {
                            dato = "-201700893";
                        }
                        contadorTemporal++;

                        resultadoTraduccion += "T" + contadorTemporal + " = HP;//Guardamos la posicion de HP anterior" + Environment.NewLine;
                        int posicionAnterior = contadorTemporal;
                        contadorTemporal++;
                        resultadoTraduccion += "HP = " + direccionHeap + ";//Actualizacion de HP" + Environment.NewLine;
                        resultadoTraduccion += "T" + contadorTemporal + " = HP; //Iniciacion del for" + Environment.NewLine;
                        //////resultadoTraduccion += "T" + contadorTemporal + " = " + direccionHeap + "; //Iniciacion del for" + Environment.NewLine;

                        resultadoTraduccion += "L" + contadorEtiqueta + ": //Etiqueta para generar el loop" + Environment.NewLine;
                        contadorEtiqueta++;
                        verdadero = contadorEtiqueta;

                        resultadoTraduccion += "T_HP = HP + " + size + ";" + Environment.NewLine;
                        resultadoTraduccion += "if (T" + contadorTemporal + " < T_HP) goto L" + contadorEtiqueta + ";" + Environment.NewLine;
                        //////resultadoTraduccion += "if (T" + contadorTemporal + " < " + (size + direccionHeap) + ") goto L" + contadorEtiqueta + ";" + Environment.NewLine;

                        contadorEtiqueta++;
                        falso = contadorEtiqueta;
                        resultadoTraduccion += "goto L" + contadorEtiqueta + ";" + Environment.NewLine;
                        resultadoTraduccion += "L" + verdadero + ": //Llenamos el arreglo" + Environment.NewLine;
                        resultadoTraduccion += "HEAP[(int)T" + contadorTemporal + "] = " + dato + ";" + Environment.NewLine;
                        resultadoTraduccion += "T" + contadorTemporal + " = T" + contadorTemporal + " + 1; //Incremento del for" + Environment.NewLine;
                        resultadoTraduccion += "goto L" + (verdadero - 1) + ";" + Environment.NewLine;
                        resultadoTraduccion += "L" + falso + ":" + Environment.NewLine;
                        contadorTemporal++;
                        contadorEtiqueta++;
                        resultadoTraduccion += "HP = T" + posicionAnterior + ";//Regresamos la posicion de HP" + Environment.NewLine;
                    }
                    resultadoTraduccion += "";
                }
            }
            else if (variable.tipo == Simbolo.EnumTipo.objeto)
            {
                if (variable.direccionHeap != -1)
                {
                    int tmpSize;

                    int posicion = variable.direccionHeap;

                    string dato = "";
                    foreach (KeyValuePair<string, Simbolo> pareja in variable.atributos.tabla)
                    {
                        tmpSize = pareja.Value.size;
                        dato = "0";
                        if (pareja.Value.tipo == Simbolo.EnumTipo.cadena)
                        {
                            dato = "-201700893";
                        }
                        contadorTemporal++;

                        resultadoTraduccion += "T" + contadorTemporal + " = HP;//Guardamos la posicion de HP anterior" + Environment.NewLine;
                        int posicionAnterior = contadorTemporal;
                        contadorTemporal++;
                        resultadoTraduccion += "HP = " + posicion + ";//Actualizacion de HP" + Environment.NewLine;
                        resultadoTraduccion += "T" + contadorTemporal + " = HP; //Iniciacion del for" + Environment.NewLine;
                        //////resultadoTraduccion += "T" + contadorTemporal + " = " + posicion + "; //Iniciacion del for" + Environment.NewLine;

                        resultadoTraduccion += "L" + contadorEtiqueta + ": //Etiqueta para generar el loop" + Environment.NewLine;
                        contadorEtiqueta++;
                        verdadero = contadorEtiqueta;

                        resultadoTraduccion += "T_HP = HP + " + tmpSize + ";" + Environment.NewLine;
                        resultadoTraduccion += "if (T" + contadorTemporal + " < T_HP) goto L" + contadorEtiqueta + ";" + Environment.NewLine;
                        //////resultadoTraduccion += "if (T" + contadorTemporal + " < " + (tmpSize + posicion) + ") goto L" + contadorEtiqueta + ";" + Environment.NewLine;

                        contadorEtiqueta++;
                        falso = contadorEtiqueta;
                        resultadoTraduccion += "goto L" + contadorEtiqueta + ";" + Environment.NewLine;
                        resultadoTraduccion += "L" + verdadero + ": //Llenamos el arreglo" + Environment.NewLine;
                        resultadoTraduccion += "HEAP[(int)T" + contadorTemporal + "] = " + dato + ";" + Environment.NewLine;
                        resultadoTraduccion += "T" + contadorTemporal + " = T" + contadorTemporal + " + 1; //Incremento del for" + Environment.NewLine;
                        resultadoTraduccion += "goto L" + (verdadero - 1) + ";" + Environment.NewLine;
                        resultadoTraduccion += "L" + falso + ":" + Environment.NewLine;
                        contadorTemporal++;
                        contadorEtiqueta++;
                        resultadoTraduccion += "HP = T" + posicionAnterior + ";//Regresamos la posicion de HP" + Environment.NewLine;
                    }
                }
            }
            else if (variable.tipo == Simbolo.EnumTipo.cadena)
            {
                int tmpSize;
                tmpSize = variable.size;
                int posicion = variable.direccionHeap;
                string dato = "-201700893";
                contadorTemporal++;

                resultadoTraduccion += "T" + contadorTemporal + " = HP;//Guardamos la posicion de HP anterior" + Environment.NewLine;
                int posicionAnterior = contadorTemporal;
                contadorTemporal++;
                resultadoTraduccion += "HP = " + variable.direccionHeap + ";//Actualizacion de HP" + Environment.NewLine;
                resultadoTraduccion += "T" + contadorTemporal + " = HP ; //Iniciacion del for" + Environment.NewLine;
                //////resultadoTraduccion += "T" + contadorTemporal + " = " + posicion + "; //Iniciacion del for" + Environment.NewLine;

                resultadoTraduccion += "L" + contadorEtiqueta + ": //Etiqueta para generar el loop" + Environment.NewLine;
                contadorEtiqueta++;
                verdadero = contadorEtiqueta;

                resultadoTraduccion += "T_HP = HP + " + tmpSize + ";" + Environment.NewLine;
                resultadoTraduccion += "if (T" + contadorTemporal + " < T_HP) goto L" + contadorEtiqueta + ";" + Environment.NewLine;
                //////resultadoTraduccion += "if (T" + contadorTemporal + " < " + (tmpSize + posicion) + ") goto L" + contadorEtiqueta + ";" + Environment.NewLine;

                contadorEtiqueta++;
                falso = contadorEtiqueta;
                resultadoTraduccion += "goto L" + contadorEtiqueta + ";" + Environment.NewLine;
                resultadoTraduccion += "L" + verdadero + ": //Llenamos el arreglo" + Environment.NewLine;
                resultadoTraduccion += "HEAP[(int)T" + contadorTemporal + "] = " + dato + ";" + Environment.NewLine;
                resultadoTraduccion += "T" + contadorTemporal + " = T" + contadorTemporal + " + 1; //Incremento del for" + Environment.NewLine;
                resultadoTraduccion += "goto L" + (verdadero - 1) + ";" + Environment.NewLine;
                resultadoTraduccion += "L" + falso + ":" + Environment.NewLine;
                contadorTemporal++;
                contadorEtiqueta++;
                resultadoTraduccion += "HP = T" + posicionAnterior + ";//Regresamos la posicion de HP" + Environment.NewLine;
            }
            else if (variable.tipo == Simbolo.EnumTipo.procedimiento || variable.tipo == Simbolo.EnumTipo.funcion)
            {
                //Agregar todos al HEAP
                resultadoTraduccion += "void " + nombreVariable + "(){//Declaracion de procedimiento " + nombreVariable + Environment.NewLine;
                SegundaPasada pasada = new SegundaPasada(contadorEtiqueta, entornoGlobal);
                pasada.HP = 1;
                pasada.iniciarSegundaPasada(variable.root, 1, variable.atributos);
                resultadoTraduccion += pasada.traduccion;
                resultadoTraduccion += "return;" + Environment.NewLine + "}" + Environment.NewLine;
                if (contadorTemporal < pasada.contadorTemporal)
                {
                    contadorTemporal = pasada.contadorTemporal;
                }
                contadorEtiqueta = pasada.contadorEtiqueta;
            }
            else
                    {
                        if (variable.root != null)
                        {
                            resultadoTraduccion = ResolverExpresionAsignacion(variable.root, entorno);
                            if (!(resultadoTraduccion is null) && !resultadoTraduccion.StartsWith("T") && !resultadoTraduccion.StartsWith("S_"))
                            {
                                contadorTemporal++;
                                resultadoTraduccion = "//Inicio de declaracion de identificador " + nombreVariable + Environment.NewLine +
                                    "T" + contadorTemporal + " = " + resultadoTraduccion;
                            }
                        }
                        else
                        {
                            contadorTemporal++;
                            resultadoTraduccion = "T" + contadorTemporal + " = 0;";
                        }
                        if (!(resultadoTraduccion is null))
                        {
                            resultadoTraduccion += Environment.NewLine + "T_SP = SP + " + variable.direccionRelativa + ";";
                            resultadoTraduccion += Environment.NewLine + "STACK[(int)T_SP] = T" + contadorTemporal + ";" +
                                Environment.NewLine + "//Fin de declaracion de identificador " + nombreVariable;
                        }
                        else
                        {
                            resultadoTraduccion += Environment.NewLine + "T_SP = SP + " + variable.direccionRelativa + ";" + Environment.NewLine;
                            resultadoTraduccion += "//Inicio de declaracion de identificador " + nombreVariable + Environment.NewLine + "STACK[(int)T_SP] = 0;" +
                                Environment.NewLine + "//Fin de declaracion de identificador " + nombreVariable;
                        }
                    }
            retorno[0] = contadorTemporal;
            retorno[1] = resultadoTraduccion;
            retorno[2] = contadorEtiqueta;
            return retorno;
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
        public string ResolverExpresion(ParseTreeNode root, Entorno entorno, int pointer)
        {
            Simbolo simbolo;
            string traduccion = "";
            if (root.ChildNodes.Count == 1)
            {
                //Es un solo termino
                if (root.ChildNodes[0].ToString().Equals("VARIABLE"))
                {
                    //Es una variable, debemos buscar su valor
                    simbolo = entorno.buscar(removerExtras(root.ChildNodes[0].ChildNodes[0].ToString()));
                    if (pointer == 0)
                    {
                        //Esta en el cuerpo principal del programa
                        traduccion += "STACK[" + simbolo.direccionAbsoluta + "];";
                    }
                    else
                    {
                        //Es una funcion o procedimiento
                        if (!(simbolo is null))
                        {
                            contadorTemporal++;
                            traduccion += "T" + contadorTemporal + " = HEAP[(int)HP + " + simbolo.direccionRelativa + "];";
                        }
                        else
                        {
                            if(removerExtras(root.ChildNodes[0].ChildNodes[0].ToString()) == "n")
                            {
                                traduccion += "T" + contadorTemporal + " = HEAP[(int)HP + 1];";
                            }
                        }
                    }
                    
                    return traduccion;
                }
                else if (root.ChildNodes[0].ToString().Equals("EXPRESION") || root.ToString().Equals("RANGO"))
                {
                    //Expresion anidada
                    return ResolverExpresion(root.ChildNodes[0], entorno, pointer);
                }
                else if (root.ChildNodes[0].ToString().Equals("ESTRUCTURA"))
                {
                    if (!root.ChildNodes[0].ChildNodes[0].ToString().Equals("LLAMADA"))
                    {
                        int[] datos = obtenerDatosEstructura(root.ChildNodes[0], entorno);
                        if (!(datos is null))
                        {
                            traduccion += "HEAP[" + datos[0] + "];";
                            return traduccion;
                        }
                    }
                    else
                    {
                        traduccion += ejecutarLlamada(root.ChildNodes[0].ChildNodes[0], traduccion);
                        contadorTemporal++;
                        traduccion += "T" + contadorTemporal + " = HEAP[(int)HP + 0];" + Environment.NewLine;
                        traduccion += "HP = HP - S_HP;" + Environment.NewLine;
                    }
                }
                else if (root.ChildNodes[0].ToString().Equals("LLAMADA"))
                {
                    traduccion += ejecutarLlamada(root.ChildNodes[0], traduccion);
                    contadorTemporal++;
                    traduccion += "T" + contadorTemporal + "HEAP[(int)HP + 0];";
                    traduccion += "HP = S_HP;" + Environment.NewLine;
                }
                else
                {
                    //Es un valor puntual, no debemos de buscar nada
                    string res = removerExtras(root.ChildNodes[0].ToString()) + ";";
                    if ((root.ChildNodes[0].ToString()).Contains("(boleano)"))
                    {
                        if (root.ChildNodes[0].ToString().Contains("false"))
                            res = "0;";
                        else
                            res = "1;";
                    }
                    return res;
                }
            }
            else if (root.ChildNodes.Count == 3)
            {
                //Es una operacion binaria OPERADOR1 (+, -, * , /, %, AND, OR, >, <, >=, <=, <>, =) OPERADOR2
                string operador1 = ResolverExpresion(root.ChildNodes[0], entorno, pointer);
                if (!operador1.StartsWith("T") && !operador1.StartsWith("S_"))
                {
                    contadorTemporal++;
                    operador1 = "T" + contadorTemporal + " = " + operador1;
                }
                int temporalOperador1 = contadorTemporal;
                string operador2 = ResolverExpresion(root.ChildNodes[2], entorno, pointer);
                if (!operador2.StartsWith("T") && !operador2.StartsWith("S_"))
                {
                    contadorTemporal++;
                    operador2 = "T" + contadorTemporal + " = " + operador2;
                }
                int temporalOperador2 = contadorTemporal;
                traduccion += operador2 + Environment.NewLine + operador1;
                contadorTemporal++;
                string operador = removerExtras(root.ChildNodes[1].ToString());
                if (operador.Equals("<>"))
                    operador = "!=";
                if (operador.Equals("="))
                    operador = "==";
                if (operador.ToLower().Equals("or"))
                    operador = "||";
                if (operador.ToLower().Equals("and"))
                    operador = "&&";
                traduccion += Environment.NewLine + "T" + contadorTemporal + " = T" + temporalOperador1 + " "
                        + operador + " T" + temporalOperador2 + ";";
            }
            else if (root.ChildNodes.Count == 2)
            {
                //Es una operacion unaria (NOT, -)OPERADOR1
                string operador1 = ResolverExpresion(root.ChildNodes[1], entorno, pointer);
                if (!operador1.StartsWith("T") && !operador1.StartsWith("S_"))
                {
                    contadorTemporal++;
                    operador1 = "T" + contadorTemporal + " = " + operador1;
                }
                contadorTemporal++;
                string operador = removerExtras(root.ChildNodes[0].ToString());
                if (operador.ToLower().Equals("not"))
                    operador = "!";
                traduccion += "T" + contadorTemporal + " = " + operador + "T" + (contadorTemporal - 1);
            }
            return traduccion;
        }
        private int[] obtenerDatosEstructura(ParseTreeNode root, Entorno entorno)
        {
            //Recibimos un nodo tipo ESTRUCTURA
            int[] retorno = new int[3];
            string nombreVariable = acumulado + removerExtras(root.ChildNodes[0].ToString()); //Nombre de la variable a buscar
            Simbolo variable = entorno.buscar(nombreVariable);
            if (variable is null)
            {
                int sizTemp = -1;
                int posTemp = -1;
                foreach (KeyValuePair<string, Simbolo> pair in entorno.tabla)
                {
                    if (pair.Key.Contains(nombreVariable))
                    {
                        if (sizTemp == -1)
                        {
                            sizTemp = pair.Value.size;
                            posTemp += pair.Value.direccionAbsoluta + 1;
                        }
                        else
                        {
                            sizTemp += pair.Value.size;
                        }
                        MessageBox.Show("Variable: " + pair.Key + Environment.NewLine + "Posicion absoluta: " + posTemp + Environment.NewLine + "Size: " + sizTemp);
                    }
                }
                retorno[0] = posTemp; //Posicion inicial
                retorno[1] = sizTemp; //Size
                retorno[2] = 0; //Es cadena 0 -> no | 1 -> si
                if (posTemp != -1)
                    return retorno;
                return null;
            }
            if (variable.tipo == Simbolo.EnumTipo.arreglo && root.ChildNodes.Count == 4)
            {
                //Es un arreglo y ESTRUCTURA representa a un arreglo
                int limiteInferior, limiteSuperior;
                limiteInferior = int.Parse(variable.limiteInferior[0].ToString());
                limiteSuperior = int.Parse(variable.limiteSuperior[0].ToString());

            }
            else if (variable.tipo == Simbolo.EnumTipo.objeto && root.ChildNodes.Count == 2)
            {
                //Es un objeto y ESTRUCTURA representa a un objeto
                if (root.ChildNodes[1].ChildNodes.Count > 0)
                {
                    if (root.ChildNodes[1].ChildNodes[1].ChildNodes.Count > 0)
                    {
                        acumulado = removerExtras(root.ChildNodes[1].ChildNodes[0].ToString()) + ".";
                        return obtenerDatosEstructura(root.ChildNodes[1].ChildNodes[1], variable.atributos);
                    }
                    return obtenerDatosEstructura(root.ChildNodes[1], variable.atributos);
                }
                retorno[0] = variable.direccionAbsoluta; //Posicion inicial
                retorno[1] = variable.size; //Size
                retorno[2] = 0; //Es cadena 0 -> no | 1 -> si
            }
            else
            {
                if (root.ChildNodes[1].ChildNodes.Count > 0)
                    return null;
                if (acumulado != "")
                {
                    //MessageBox.Show("Posicion absoluta: " + variable.direccionAbsoluta + Environment.NewLine + "Size: " + variable.size);
                }
                if (variable.direccionHeap == -1)
                    retorno[0] = variable.direccionAbsoluta; //Posicion inicial
                else
                    retorno[0] = variable.direccionHeap;
                retorno[1] = variable.size; //Size
                if (variable.tipo == Simbolo.EnumTipo.cadena)
                    retorno[2] = 1; //Es cadena 0 -> no | 1 -> si
                else
                    retorno[2] = 0; //Es cadena 0 -> no | 1 -> si
            }
            return retorno;
        }
        private string ejecutarLlamada(ParseTreeNode root, string traduccion)
        {
            string nombre = removerExtras(root.ChildNodes[0].ToString());
            Simbolo sim = entornoGlobal.buscar(nombre);
            string str = "";
                MessageBox.Show("ACA " + nombre);
            if (!(sim is null))
            {
                if (sim.tipo == Simbolo.EnumTipo.procedimiento || sim.tipo == Simbolo.EnumTipo.funcion)
                {
                    int suma = 0;
                    Entorno eTmp = entornoGlobal;
                    while (eTmp.anterior != null)
                    {
                        eTmp = eTmp.anterior;
                    }
                    foreach (KeyValuePair<string, Simbolo> t in eTmp.tabla)
                    {
                        suma += t.Value.size;
                    }
                    traduccion += "S_HP = " + suma + ";" + Environment.NewLine;
                    if (root.ChildNodes.Count > 1)
                    {
                        if (root.ChildNodes[1].ChildNodes[0].ToString().Equals("VARIABLE") || root.ChildNodes[1].ChildNodes.Count > 1 || (root.ChildNodes[1].ChildNodes[0].ChildNodes.Count > 1 && !root.ChildNodes[1].ChildNodes[0].ToString().Equals("ESTRUCTURA")))
                        {
                            Simbolo simX = null;
                            if (root.ChildNodes[1].ChildNodes[0].ToString().Equals("VARIABLE"))
                            {
                                simX = entornoGlobal.buscar(removerExtras(root.ChildNodes[1].ChildNodes[0].ChildNodes[0].ToString()));
                                if (simX.tipo != Simbolo.EnumTipo.cadena)
                                    simX = null;
                            }
                            if (simX is null)
                            {
                                //declaracion.contadorTemporal = 0;
                                str = ResolverExpresion(root.ChildNodes[1], entornoGlobal, 1);
                                if (!str.StartsWith("T") && !str.StartsWith("S_"))
                                {
                                    //declaracion.contadorTemporal++;
                                    contadorTemporal++;
                                    str = "T" + contadorTemporal + " = " + str + Environment.NewLine;
                                }
                                //if (declaracion.contadorTemporal > contadorTemporal)
                                //{
                                //    contadorTemporal = declaracion.contadorTemporal;
                                //}
                                traduccion += str + Environment.NewLine;
                                //Es un numero o boolean, puede ser un objeto o un arreglo
                                traduccion += "HP = HP + S_HP;" + Environment.NewLine;
                                traduccion += "HEAP[(int)HP + 1] = T" + contadorTemporal + ";" + Environment.NewLine;
                                traduccion += "HP = HP - S_HP;" + Environment.NewLine;
                            }
                            else
                            {
                                bool tipo = false;
                                if (simX.tipo == Simbolo.EnumTipo.cadena)
                                    tipo = true;
                                //Es una cadena
                            }
                        }
                        else
                        {
                            str = removerExtras(root.ChildNodes[1].ChildNodes[0].ToString());
                            if (str.Equals(""))
                            {
                                str = " ";
                            }
                            //Es un valor
                        }

                        ParseTreeNode nodoTemporal = root.ChildNodes[2];
                        while (nodoTemporal.ChildNodes.Count > 0)
                        {
                            if (nodoTemporal.ChildNodes[0].ChildNodes[0].ToString().Equals("VARIABLE") || nodoTemporal.ChildNodes[0].ChildNodes.Count > 1 || (nodoTemporal.ChildNodes[0].ChildNodes[0].ChildNodes.Count > 1 && !nodoTemporal.ChildNodes[0].ChildNodes[0].ToString().Equals("ESTRUCTURA")))
                            {
                                Simbolo simX = null;
                                if (nodoTemporal.ChildNodes[0].ChildNodes[0].ToString().Equals("VARIABLE"))
                                {
                                    simX = entornoGlobal.buscar(removerExtras(nodoTemporal.ChildNodes[0].ChildNodes[0].ChildNodes[0].ToString()));
                                    if (simX.tipo != Simbolo.EnumTipo.cadena)
                                        simX = null;
                                }
                                if (simX is null)
                                {
                                    //declaracion.contadorTemporal = 0;
                                    str = ResolverExpresion(nodoTemporal.ChildNodes[0], entornoGlobal, 1);
                                    if (!str.StartsWith("T") && !str.StartsWith("S_"))
                                    {
                                        contadorTemporal++;
                                        str = "T" + contadorTemporal + " = " + str + Environment.NewLine;
                                    }
                                    //if (declaracion.contadorTemporal > contadorTemporal)
                                    //{
                                    //    contadorTemporal = declaracion.contadorTemporal;
                                    //}
                                    traduccion += str + Environment.NewLine;
                                    //Es un numero o boolean, puede ser un objeto o un arreglo
                                }
                                else
                                {
                                    bool tipo = false;
                                    if (simX.tipo == Simbolo.EnumTipo.cadena)
                                        tipo = true;
                                    //Es una cadena
                                }
                            }
                            else
                            {
                                str = removerExtras(nodoTemporal.ChildNodes[0].ChildNodes[0].ToString());
                                if (str.Equals(""))
                                {
                                    str = " ";
                                }
                                //Es un valor
                            }
                            nodoTemporal = nodoTemporal.ChildNodes[1];
                        }
                    }
                    //traduccion += "HEAP[(int)HP + 1] = 6;" + Environment.NewLine;
                    traduccion += "HP = HP + S_HP;" + Environment.NewLine;
                    traduccion += nombre + "();" + Environment.NewLine;
                }
                else
                {
                    //ERROR
                }
            }
            return traduccion;
        }
        public string ResolverExpresionAsignacion(ParseTreeNode root, Entorno entorno)
        {
            string traduccion = "";
            if (root.ChildNodes.Count == 1)
            {
                //Es un solo termino
                if (root.ChildNodes[0].ToString().Equals("VARIABLE"))
                {
                    //Reportar error, no se puede asignar una variable a otra variable dentro de la misma declaracion
                    // var numero1 : integer = 1;
                    // var numero2 : integer = numero2;  <- aca esta el error
                    return null;
                }
                else if (root.ChildNodes[0].ToString().Equals("EXPRESION") || root.ToString().Equals("RANGO"))
                {
                    //Expresion anidada
                    return ResolverExpresionAsignacion(root.ChildNodes[0], entorno);
                }
                else if (root.ChildNodes[0].ToString().Equals("ESTRUCTURA"))
                {
                    //Reportar error, no se puede asignar una variable a otra variable dentro de la misma declaracion
                    // var numero1 : integer = 1;
                    // var numero2 : integer = numero2;  <- aca esta el error
                    return null;
                }
                else
                {
                    //Es un valor puntual, no debemos de buscar nada
                    string res = removerExtras(root.ChildNodes[0].ToString()) + ";";
                    if ((root.ChildNodes[0].ToString()).Contains("(boleano)"))
                    {
                        if (root.ChildNodes[0].ToString().Contains("false"))
                            res = "0;";
                        else
                            res = "1;";
                    }
                    return res;
                }
            }
            else if (root.ChildNodes.Count == 3)
            {
                //Es una operacion binaria OPERADOR1 (+, -, * , /, %, AND, OR, >, <, >=, <=, <>, =) OPERADOR2
                string operador1 = ResolverExpresionAsignacion(root.ChildNodes[0], entorno);
                if (!operador1.StartsWith("T") && !operador1.StartsWith("S_"))
                {
                    contadorTemporal++;
                    operador1 = "T" + contadorTemporal + " = " + operador1;
                }
                int temporalOperador1 = contadorTemporal;
                string operador2 = ResolverExpresionAsignacion(root.ChildNodes[2], entorno);
                if (!operador2.StartsWith("T") && !operador2.StartsWith("S_"))
                {
                    contadorTemporal++;
                    operador2 = "T" + contadorTemporal + " = " + operador2;
                }
                int temporalOperador2 = contadorTemporal;
                traduccion += operador1 + Environment.NewLine + operador2;
                contadorTemporal++;
                string operador = removerExtras(root.ChildNodes[1].ToString());
                if (operador.Equals("<>"))
                    operador = "!=";
                if (operador.Equals("="))
                    operador = "==";
                if (operador.ToLower().Equals("or"))
                    operador = "||";
                if (operador.ToLower().Equals("and"))
                    operador = "&&";
                traduccion += Environment.NewLine + "T" + contadorTemporal + " = T" + temporalOperador1 + " "
                        + operador + " T" + temporalOperador2 + ";";
            }
            else if (root.ChildNodes.Count == 2)
            {
                //Es una operacion unaria (NOT, -)OPERADOR1
                string operador1 = ResolverExpresionAsignacion(root.ChildNodes[1], entorno);
                if (!operador1.StartsWith("T") && !operador1.StartsWith("S_"))
                {
                    contadorTemporal++;
                    operador1 = "T" + contadorTemporal + " = " + operador1;
                }
                contadorTemporal++;
                string operador = removerExtras(root.ChildNodes[0].ToString());
                if (operador.ToLower().Equals("not"))
                    operador = "!";
                traduccion += "T" + contadorTemporal + " = " + operador + "T" + (contadorTemporal - 1);
            }
            return traduccion;
        }
    }
}
