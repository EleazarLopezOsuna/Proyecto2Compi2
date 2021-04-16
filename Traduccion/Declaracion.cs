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
        public Declaracion(int contadorTemporal) {
            this.contadorTemporal = contadorTemporal;
        }
        public Object[] Traducir(Simbolo variable, Entorno entorno)
        {
            Object[] retorno = new Object[2];
            string resultadoTraduccion = "";
            if (variable.root != null)
            {
                resultadoTraduccion = ResolverExpresion(variable.root, entorno);
                if (!resultadoTraduccion.StartsWith("T"))
                {
                    contadorTemporal++;
                    resultadoTraduccion = "T" + contadorTemporal + " = " + resultadoTraduccion;
                }
            }
            else
            {
                contadorTemporal++;
                resultadoTraduccion = "T" + contadorTemporal + " = 0";
            }
            resultadoTraduccion += Environment.NewLine + "STACK[" + variable.direccionAbsoluta + "] = T" + contadorTemporal + ";";
            retorno[0] = contadorTemporal;
            retorno[1] = resultadoTraduccion;
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
                else if (root.ChildNodes[0].ToString().Equals("EXPRESION"))
                {
                    //Expresion anidada
                    return ResolverExpresion(root.ChildNodes[0], entorno);
                }
                else
                {
                    //Es un valor puntual, no debemos de buscar nada
                    return removerExtras(root.ChildNodes[0].ToString()) + ";";
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
                traduccion += Environment.NewLine + "T" + contadorTemporal + " = T" + temporalOperador1 + " " 
                        + removerExtras(root.ChildNodes[1].ToString()) + " T" + temporalOperador2 + ";";
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
