using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Irony.Parsing;
using Proyecto2_Compiladores2.Modelos;

namespace Proyecto2_Compiladores2.Traduccion
{
    class Declaracion
    {
        public int contadorTemporal;
        public int contadorEtiqueta;
        private Declaracion declaracionTemp;
        private string acumulado;
        public Declaracion(int contadorTemporal, int contadorEtiqueta) {
            this.contadorTemporal = contadorTemporal;
            this.contadorEtiqueta = contadorEtiqueta;
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
                        resultadoTraduccion += "T" + contadorTemporal + " = " + posicion + "; //Iniciacion del for" + Environment.NewLine;
                        resultadoTraduccion += "L" + contadorEtiqueta + ": //Etiqueta para generar el loop" + Environment.NewLine;
                        contadorEtiqueta++;
                        verdadero = contadorEtiqueta;
                        resultadoTraduccion += "if (T" + contadorTemporal + " < " + (tmpSize + posicion) + ") goto L" + contadorEtiqueta + ";" + Environment.NewLine;
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
                    }
                    else if (variable.contenido.tipo == Simbolo.EnumTipo.objeto)
                    {
                        int tmpDireccionHeap;
                        int tmpSize;
                        int posicion = variable.direccionHeap;
                        for (int i = 0; i < (variable.size / variable.contenido.size); i ++)
                        {
                            foreach (KeyValuePair<string, Simbolo> pair in variable.contenido.atributos.tabla)
                            {
                                tmpSize = pair.Value.size;
                                //MessageBox.Show("Nombre: " + pair.Key + Environment.NewLine + "Direccion Aboluta: " + pair.Value.direccionAbsoluta
                                //    + Environment.NewLine + "Posicion Heap: " + pair.Value.direccionHeap + Environment.NewLine + "Size: " + pair.Value.size
                                //    + Environment.NewLine + "Posicion Relativa: " + pair.Value.direccionRelativa);
                                dato = "0";
                                if (pair.Value.tipo == Simbolo.EnumTipo.arreglo)
                                {
                                    declaracionTemp = new Declaracion(0, contadorEtiqueta);
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
                                        tmpDireccionHeap = pareja.Value.direccionAbsoluta;
                                        if (pareja.Value.tipo == Simbolo.EnumTipo.cadena)
                                        {
                                            dato = "-201700893";
                                        }
                                        contadorTemporal++;
                                        resultadoTraduccion += "T" + contadorTemporal + " = " + posicion + "; //Iniciacion del for" + Environment.NewLine;
                                        resultadoTraduccion += "L" + contadorEtiqueta + ": //Etiqueta para generar el loop" + Environment.NewLine;
                                        contadorEtiqueta++;
                                        verdadero = contadorEtiqueta;
                                        resultadoTraduccion += "if (T" + contadorTemporal + " < " + (tmpSize + posicion) + ") goto L" + contadorEtiqueta + ";" + Environment.NewLine;
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
                                    }
                                }
                                else
                                {
                                    tmpDireccionHeap = pair.Value.direccionAbsoluta;
                                    if (pair.Value.tipo == Simbolo.EnumTipo.cadena)
                                    {
                                        dato = "-201700893";
                                    }
                                    contadorTemporal++;
                                    resultadoTraduccion += "T" + contadorTemporal + " = " + posicion + "; //Iniciacion del for" + Environment.NewLine;
                                    resultadoTraduccion += "L" + contadorEtiqueta + ": //Etiqueta para generar el loop" + Environment.NewLine;
                                    contadorEtiqueta++;
                                    verdadero = contadorEtiqueta;
                                    resultadoTraduccion += "if (T" + contadorTemporal + " < " + (tmpSize + posicion) + ") goto L" + contadorEtiqueta + ";" + Environment.NewLine;
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
                        resultadoTraduccion += "T" + contadorTemporal + " = " + direccionHeap + "; //Iniciacion del for" + Environment.NewLine;
                        resultadoTraduccion += "L" + contadorEtiqueta + ": //Etiqueta para generar el loop" + Environment.NewLine;
                        contadorEtiqueta++;
                        verdadero = contadorEtiqueta;
                        resultadoTraduccion += "if (T" + contadorTemporal + " < " + (size + direccionHeap) + ") goto L" + contadorEtiqueta + ";" + Environment.NewLine;
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
                    }
                    resultadoTraduccion += "";
                }
            }
            else if(variable.tipo == Simbolo.EnumTipo.objeto)
            {
                if (variable.direccionHeap != -1)
                {
                    int tmpDireccionHeap;
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
                        resultadoTraduccion += "T" + contadorTemporal + " = " + posicion + "; //Iniciacion del for" + Environment.NewLine;
                        resultadoTraduccion += "L" + contadorEtiqueta + ": //Etiqueta para generar el loop" + Environment.NewLine;
                        contadorEtiqueta++;
                        verdadero = contadorEtiqueta;
                        resultadoTraduccion += "if (T" + contadorTemporal + " < " + (tmpSize + posicion) + ") goto L" + contadorEtiqueta + ";" + Environment.NewLine;
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
                resultadoTraduccion += "T" + contadorTemporal + " = " + posicion + "; //Iniciacion del for" + Environment.NewLine;
                resultadoTraduccion += "L" + contadorEtiqueta + ": //Etiqueta para generar el loop" + Environment.NewLine;
                contadorEtiqueta++;
                verdadero = contadorEtiqueta;
                resultadoTraduccion += "if (T" + contadorTemporal + " < " + (tmpSize + posicion) + ") goto L" + contadorEtiqueta + ";" + Environment.NewLine;
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
            }
            else
            {
                if (variable.root != null)
                {
                    resultadoTraduccion += ResolverExpresion(variable.root, entorno);
                    if (!resultadoTraduccion.StartsWith("T"))
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
                resultadoTraduccion += Environment.NewLine + "STACK[" + variable.direccionAbsoluta + "] = T" + contadorTemporal + ";" +
                    Environment.NewLine + "//Fin de declaracion de identificador " + nombreVariable;
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
        public string ResolverExpresion(ParseTreeNode root, Entorno entorno)
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
                    traduccion += "STACK[" + simbolo.direccionAbsoluta + "];";
                    return traduccion;
                }
                else if (root.ChildNodes[0].ToString().Equals("EXPRESION") || root.ToString().Equals("RANGO"))
                {
                    //Expresion anidada
                    return ResolverExpresion(root.ChildNodes[0], entorno);
                }
                else if (root.ChildNodes[0].ToString().Equals("ESTRUCTURA"))
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
                string operador1 = ResolverExpresion(root.ChildNodes[0], entorno);
                if (!operador1.StartsWith("T"))
                {
                    contadorTemporal++;
                    operador1 = "T" + contadorTemporal + " = " + operador1;
                }
                int temporalOperador1 = contadorTemporal;
                string operador2 = ResolverExpresion(root.ChildNodes[2], entorno);
                if (!operador2.StartsWith("T"))
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
                string operador1 = ResolverExpresion(root.ChildNodes[1], entorno);
                if (!operador1.StartsWith("T"))
                {
                    contadorTemporal++;
                    operador1 = "T" + contadorTemporal + " = " + operador1;
                }
                contadorTemporal++;
                string operador = removerExtras(root.ChildNodes[0].ToString());
                if (operador.ToLower().Equals("not"))
                    operador = "!";
                traduccion += "T" + contadorTemporal + " = "  + operador + "T" + (contadorTemporal - 1);
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
    }
}
