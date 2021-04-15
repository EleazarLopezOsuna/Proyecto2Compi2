using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Irony.Parsing;
using Proyecto2_Compiladores2.Modelos;

namespace Proyecto2_Compiladores2.Analizador
{
    class PrimeraPasada
    {
        public Entorno entornoGlobal;
        int posicionAbsoluta;
        public int posicionRelativa;
        public ArrayList errores;
        public PrimeraPasada()
        {
            posicionRelativa = 0;
        }
        public void iniciarPrimeraPasada(ParseTreeNode root, int posicionAbsoluta)
        {
            entornoGlobal = new Entorno(null, "global");
            this.posicionAbsoluta = posicionAbsoluta;
            errores = new ArrayList();
            recorrer(root, entornoGlobal);
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
            switch (root.ToString())
            {
                case "PROGRAMA":
                case "Z_DECLARACIONES":
                case "Z_VARIABLES":
                case "Z_CONSTANTES":
                    if (root.ChildNodes.Count > 0)
                    {
                        foreach (ParseTreeNode hijo in root.ChildNodes)
                        {
                            recorrer(hijo, entorno);
                        }
                    }
                    break;
                case "D_VARIABLE":
                    if (root.ChildNodes.Count > 0)
                    {
                        if (root.ChildNodes[0].ToString().Equals("D_VARIABLE"))
                        {
                            foreach (ParseTreeNode hijo in root.ChildNodes)
                            {
                                recorrer(hijo, entorno);
                            }
                        }
                        else
                        {
                            if (root.ChildNodes.Count == 4)
                            {
                                //var identificador : tipo = expresion
                                //         0           1   2    3
                                if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("real"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.real, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("boolean"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.boleano, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("integer"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.entero, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("string"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.cadena, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("id"))
                                {
                                    Expresion tmp = buscarVariable(root.ChildNodes[1].ChildNodes[0], entorno);
                                    if (tmp.tipo == Simbolo.EnumTipo.error)
                                    {
                                        Error error = new Error(root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, "Semantico", "El objeto o arreglo no existe");
                                        errores.Add(error);
                                    }
                                    else
                                    {
                                        simbolo = new Simbolo(tmp.tipo, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
                                    }
                                }
                                if (simbolo is null) { }
                                else
                                {
                                    if(simbolo.tipo != Simbolo.EnumTipo.error)
                                    {
                                        if(!entorno.insertar(removerExtras(root.ChildNodes[0].ToString()), simbolo))
                                        {
                                            Error error = new Error(root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, "Semantico", "El identificador " + removerExtras(root.ChildNodes[0].ToString()) + " ya existe");
                                            errores.Add(error);
                                        }
                                        else
                                        {
                                            posicionRelativa++;
                                        }
                                    }
                                }
                            }
                            else if (root.ChildNodes.Count == 3)
                            {
                                //var identificador {, identificador}+ : tipo
                                //         0             1                 2
                                ParseTreeNode temp = root;
                                if (root.ChildNodes[2].ChildNodes[0].ToString().Contains("real"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.real, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[2]);
                                }
                                else if (root.ChildNodes[2].ChildNodes[0].ToString().Contains("boolean"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.boleano, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[2]);
                                }
                                else if (root.ChildNodes[2].ChildNodes[0].ToString().Contains("integer"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.entero, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[2]);
                                }
                                else if (root.ChildNodes[2].ChildNodes[0].ToString().Contains("string"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.cadena, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[2]);
                                }
                                else if (root.ChildNodes[2].ChildNodes[0].ToString().Contains("id"))
                                {
                                    Expresion tmp = buscarVariable(root.ChildNodes[1].ChildNodes[0], entorno);
                                    if (tmp.tipo == Simbolo.EnumTipo.error)
                                    {
                                        Error error = new Error(root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, "Semantico", "El objeto o arreglo no existe");
                                        errores.Add(error);
                                    }
                                    else
                                    {
                                        simbolo = new Simbolo(tmp.tipo, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[2]);
                                    }
                                }
                                if (simbolo is null) { }
                                else
                                {
                                    if (simbolo.tipo != Simbolo.EnumTipo.error)
                                    {
                                        while (temp.ChildNodes.Count != 0)
                                        {
                                            if (!entorno.insertar(removerExtras(temp.ChildNodes[0].ToString()), new Simbolo(simbolo.tipo, posicionAbsoluta + posicionRelativa, posicionRelativa, temp.ChildNodes[0].Token.Location.Line, temp.ChildNodes[0].Token.Location.Column, simbolo.root)))
                                            {
                                                Error error = new Error(root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, "Semantico", "El identificador " + removerExtras(root.ChildNodes[0].ToString()) + " ya existe");
                                                errores.Add(error);
                                            }
                                            else
                                            {
                                                posicionRelativa++;
                                            }
                                            temp = temp.ChildNodes[1];
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //var identificador : tipo
                                //         0            1
                                ParseTreeNode temp = root;
                                if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("real"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.real, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("boolean"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.boleano, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("integer"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.entero, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("string"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.cadena, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("id"))
                                {
                                    Expresion tmp = buscarVariable(root.ChildNodes[1].ChildNodes[0], entorno);
                                    if (tmp.tipo == Simbolo.EnumTipo.error)
                                    {
                                        Error error = new Error(root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, "Semantico", "El objeto o arreglo no existe");
                                        errores.Add(error);
                                    }
                                    else
                                    {
                                        simbolo = new Simbolo(tmp.tipo, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null);
                                    }
                                }
                                if (simbolo is null)
                                {
                                }
                                else
                                {
                                    if (!entorno.insertar(removerExtras(temp.ChildNodes[0].ToString()), simbolo))
                                    {
                                        Error error = new Error(root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, "Semantico", "El identificador " + removerExtras(temp.ChildNodes[0].ToString()) + " ya existe");
                                        errores.Add(error);
                                    }
                                    else
                                    {
                                        posicionRelativa++;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "D_CONSTANTE":
                    if (root.ChildNodes.Count > 0)
                    {
                        if (root.ChildNodes[0].ToString().Equals("D_CONSTANTE"))
                        {
                            foreach (ParseTreeNode hijo in root.ChildNodes)
                            {
                                recorrer(hijo, entorno);
                            }
                        }
                        else
                        {
                            if (root.ChildNodes.Count == 4)
                            {
                                //const identificador : tipo = expresion
                                //         0           1   2    3
                                if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("real"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.real, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("boolean"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.boleano, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("integer"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.entero, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("string"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.cadena, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("id"))
                                {
                                    Expresion tmp = buscarVariable(root.ChildNodes[1].ChildNodes[0], entorno);
                                    if (tmp.tipo == Simbolo.EnumTipo.error)
                                    {
                                        Error error = new Error(root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, "Semantico", "El objeto o arreglo no existe");
                                        errores.Add(error);
                                    }
                                    else
                                    {
                                        simbolo = new Simbolo(tmp.tipo, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
                                    }
                                }
                                if (simbolo is null) { }
                                else
                                {
                                    simbolo.constante = true;
                                    if (simbolo.tipo != Simbolo.EnumTipo.error)
                                    {
                                        if (!entorno.insertar(removerExtras(root.ChildNodes[0].ToString()), simbolo))
                                        {
                                            Error error = new Error(root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, "Semantico", "El identificador " + removerExtras(root.ChildNodes[0].ToString()) + " ya existe");
                                            errores.Add(error);
                                        }
                                        else
                                        {
                                            posicionRelativa++;
                                        }
                                    }
                                }
                            }
                            else if (root.ChildNodes.Count == 3)
                            {
                                //const identificador = expresion
                                //           0        1    2
                                if (root.ChildNodes[2].ChildNodes[0].ToString().Contains("real"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.real, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[2]);
                                }
                                else if (root.ChildNodes[2].ChildNodes[0].ToString().Contains("boolean"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.boleano, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[2]);
                                }
                                else if (root.ChildNodes[2].ChildNodes[0].ToString().Contains("integer"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.entero, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[2]);
                                }
                                else if (root.ChildNodes[2].ChildNodes[0].ToString().Contains("string"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.cadena, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[2]);
                                }
                                else if (root.ChildNodes[2].ChildNodes[0].ToString().Contains("id"))
                                {
                                    Expresion tmp = buscarVariable(root.ChildNodes[1].ChildNodes[0], entorno);
                                    if (tmp.tipo == Simbolo.EnumTipo.error)
                                    {
                                        Error error = new Error(root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, "Semantico", "El objeto o arreglo no existe");
                                        errores.Add(error);
                                    }
                                    else
                                    {
                                        simbolo = new Simbolo(tmp.tipo, posicionAbsoluta + posicionRelativa, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[2]);
                                    }
                                }
                                if (simbolo is null) { }
                                else
                                {
                                    simbolo.constante = true;
                                    if (simbolo.tipo != Simbolo.EnumTipo.error)
                                    {
                                        if (!entorno.insertar(removerExtras(root.ChildNodes[0].ToString()), simbolo))
                                        {
                                            Error error = new Error(root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, "Semantico", "El identificador " + removerExtras(root.ChildNodes[0].ToString()) + " ya existe");
                                            errores.Add(error);
                                        }
                                        else
                                        {
                                            posicionRelativa++;
                                        }
                                    }
                                }
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
