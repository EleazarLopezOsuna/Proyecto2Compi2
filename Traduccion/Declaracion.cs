using System;
using System.Collections.Generic;
using System.Text;
using Irony.Parsing;
using Proyecto2_Compiladores2.Modelos;

namespace Proyecto2_Compiladores2.Traduccion
{
    class Declaracion
    {
        public int contadorTemporal;
        public int contadorEtiqueta;
        public Declaracion(int contadorTemporal, int contadorEtiqueta) {
            this.contadorTemporal = contadorTemporal;
            this.contadorEtiqueta = contadorEtiqueta;
        }
        public Object[] Traducir(Simbolo variable, Entorno entorno, string nombreVariable)
        {
            Object[] retorno = new Object[3];
            string resultadoTraduccion = "";
            if (variable.tipo == Simbolo.EnumTipo.arreglo)
            {
                if (variable.direccionHeap != -1)
                {
                    contadorEtiqueta++;
                    resultadoTraduccion += "L" + contadorEtiqueta + ": //Nos permite marcar el inicio del for que nos permite poner valores iniciales (0 | -36.3636) al arreglo" + Environment.NewLine;
                    int direccionHeap = variable.direccionHeap;
                    int size = variable.size;
                    string dato = "0";
                    if (variable.contenido.tipo == Simbolo.EnumTipo.cadena)
                    {
                        dato = "-36.3636";
                    }
                    for (int i = direccionHeap; i < size + direccionHeap; i++)
                    {
                        resultadoTraduccion += "HEAP[" + i + "] = " + dato + "; //Asignacion del dato" + Environment.NewLine;
                    }
                    resultadoTraduccion += "";
                }
            }
            else if(variable.tipo == Simbolo.EnumTipo.objeto)
            {
                if (variable.direccionHeap != -1)
                {

                }
            }
            else if (variable.tipo == Simbolo.EnumTipo.cadena)
            {
                
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
    }
}
