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
        public ArrayList entornos;
        public PrimeraPasada()
        {
            posicionRelativa = 0;
        }
        public void iniciarPrimeraPasada(ParseTreeNode root, int posicionAbsoluta)
        {
            entornos = new ArrayList();
            entornoGlobal = new Entorno(null, "global");
            entornos.Add(entornoGlobal);
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
            string nombre = "";
            string nombreTemp = "";
            int size = 1;
            ParseTreeNode nodoTemp;
            ParseTreeNode hijoTemp;
            int contador = 1;
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
                                    simbolo = new Simbolo(Simbolo.EnumTipo.real, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("boolean"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.boleano, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("integer"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.entero, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("string"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.cadena, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
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
                                        simbolo = new Simbolo(tmp.tipo, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
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
                                            posicionAbsoluta++;
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
                                    simbolo = new Simbolo(Simbolo.EnumTipo.real, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null);
                                }
                                else if (root.ChildNodes[2].ChildNodes[0].ToString().Contains("boolean"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.boleano, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null);
                                }
                                else if (root.ChildNodes[2].ChildNodes[0].ToString().Contains("integer"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.entero, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null);
                                }
                                else if (root.ChildNodes[2].ChildNodes[0].ToString().Contains("string"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.cadena, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null);
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
                                        MessageBox.Show("Aca hay que mandar los datos del objeto padre");
                                        simbolo = new Simbolo(tmp.tipo, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[2]);
                                    }
                                }
                                if (simbolo is null) { }
                                else
                                {
                                    if (simbolo.tipo != Simbolo.EnumTipo.error)
                                    {
                                        while (temp.ChildNodes.Count != 0)
                                        {
                                            if (!entorno.insertar(removerExtras(temp.ChildNodes[0].ToString()), new Simbolo(simbolo.tipo, posicionAbsoluta, posicionRelativa, temp.ChildNodes[0].Token.Location.Line, temp.ChildNodes[0].Token.Location.Column, simbolo.root)))
                                            {
                                                Error error = new Error(root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, "Semantico", "El identificador " + removerExtras(root.ChildNodes[0].ToString()) + " ya existe");
                                                errores.Add(error);
                                            }
                                            else
                                            {
                                                posicionAbsoluta++;
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
                                    simbolo = new Simbolo(Simbolo.EnumTipo.real, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("boolean"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.boleano, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("integer"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.entero, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("string"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.cadena, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null);
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
                                        simbolo = new Simbolo(tmp.tipo, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null);
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
                                        posicionAbsoluta++;
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
                                    simbolo = new Simbolo(Simbolo.EnumTipo.real, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("boolean"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.boleano, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("integer"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.entero, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("string"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.cadena, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
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
                                        simbolo = new Simbolo(tmp.tipo, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[3]);
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
                                            posicionAbsoluta++;
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
                                    simbolo = new Simbolo(Simbolo.EnumTipo.real, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[2]);
                                }
                                else if (root.ChildNodes[2].ChildNodes[0].ToString().Contains("boolean"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.boleano, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[2]);
                                }
                                else if (root.ChildNodes[2].ChildNodes[0].ToString().Contains("integer"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.entero, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[2]);
                                }
                                else if (root.ChildNodes[2].ChildNodes[0].ToString().Contains("string"))
                                {
                                    simbolo = new Simbolo(Simbolo.EnumTipo.cadena, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[2]);
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
                                        simbolo = new Simbolo(tmp.tipo, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, root.ChildNodes[2]);
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
                                            posicionAbsoluta++;
                                            posicionRelativa++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "Z_TIPOS":
                    nombre = removerExtras(root.ChildNodes[0].ToString());
                    if (!entorno.tabla.ContainsKey(nombre))
                    {
                        if (root.ChildNodes[2].ChildNodes[0].ToString().Equals("array (Keyword)"))
                        {
                            //Es un arreglo
                            nodoTemp = root.ChildNodes[2];
                            int numero1, numero2;
                            while (nodoTemp.ChildNodes.Count > 0)
                            {
                                hijoTemp = nodoTemp.ChildNodes[contador];
                                numero1 = int.Parse(removerExtras(hijoTemp.ChildNodes[0].ChildNodes[0].ToString()));
                                numero2 = int.Parse(removerExtras(hijoTemp.ChildNodes[1].ChildNodes[0].ToString()));
                                size *= (numero2 - numero1 + 1);
                                if(contador == 1)
                                {
                                    nodoTemp = nodoTemp.ChildNodes[2];
                                    contador = 0;
                                }
                                else
                                {
                                    nodoTemp = nodoTemp.ChildNodes[1];
                                }
                            }
                            if (root.ChildNodes[2].ChildNodes[3].ChildNodes[0].ToString().Contains("real"))
                            {
                                simbolo = new Simbolo(Simbolo.EnumTipo.real, 0, 0, 0, 0, null);
                            }
                            else if (root.ChildNodes[2].ChildNodes[3].ChildNodes[0].ToString().Contains("boolean"))
                            {
                                simbolo = new Simbolo(Simbolo.EnumTipo.boleano, 0, 0, 0, 0, null);
                            }
                            else if (root.ChildNodes[2].ChildNodes[3].ChildNodes[0].ToString().Contains("integer"))
                            {
                                simbolo = new Simbolo(Simbolo.EnumTipo.entero, 0, 0, 0, 0, null);
                            }
                            else if (root.ChildNodes[2].ChildNodes[3].ChildNodes[0].ToString().Contains("string"))
                            {
                                simbolo = new Simbolo(Simbolo.EnumTipo.cadena, 0, 0, 0, 0, null);
                            }
                            else if (root.ChildNodes[2].ChildNodes[3].ChildNodes[0].ToString().Contains("id"))
                            {
                                Simbolo tmp = entorno.buscar(removerExtras(root.ChildNodes[2].ChildNodes[3].ChildNodes[0].ToString()));
                                if (tmp is null)
                                {
                                    Error error = new Error(root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, "Semantico", "El objeto o arreglo no existe");
                                    errores.Add(error);
                                }
                                else
                                {
                                    simbolo = tmp;
                                }
                            }
                            if (simbolo is null) { }
                            else
                            {
                                simbolo.constante = false;
                                if (simbolo.tipo != Simbolo.EnumTipo.error)
                                {
                                    if (!entorno.insertar(removerExtras(root.ChildNodes[0].ToString()), new Simbolo(Simbolo.EnumTipo.arreglo, posicionAbsoluta, posicionRelativa, root.ChildNodes[2].ChildNodes[3].ChildNodes[0].Token.Location.Line, root.ChildNodes[2].ChildNodes[3].ChildNodes[0].Token.Location.Column, null, size * simbolo.size, simbolo)))
                                    {
                                        Error error = new Error(root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, "Semantico", "El identificador " + removerExtras(root.ChildNodes[0].ToString()) + " ya existe");
                                        errores.Add(error);
                                    }
                                    else
                                    {
                                        posicionAbsoluta++;
                                        posicionRelativa++;
                                    }
                                }
                            }
                            recorrer(root.ChildNodes[4], entorno);
                        }
                        else
                        {
                            //Es un objeto
                            nombre = removerExtras(root.ChildNodes[0].ToString());
                            if (!(entorno.buscar(nombre) is null))
                            {
                                //La variable ya existe
                            }
                            else
                            {
                                nodoTemp = root.ChildNodes[2];
                                size = 0;
                                Entorno nuevoEntorno = new Entorno(entorno, nombre);
                                entornos.Add(nuevoEntorno);
                                while (nodoTemp.ChildNodes.Count > 0)
                                {
                                    if (nuevoEntorno.tabla.Count == 0)
                                    {
                                        hijoTemp = nodoTemp.ChildNodes[1];
                                    }
                                    else
                                    {
                                        hijoTemp = nodoTemp.ChildNodes[0];
                                    }
                                    nombreTemp = removerExtras(hijoTemp.ChildNodes[0].ToString());
                                    if(nuevoEntorno.buscar(nombreTemp) is null)
                                    {
                                        if (hijoTemp.ChildNodes[2].ToString().Contains("real"))
                                        {
                                            simbolo = new Simbolo(Simbolo.EnumTipo.real, posicionAbsoluta + 1, nuevoEntorno.tabla.Count, 0, 0, null);
                                        }
                                        else if (hijoTemp.ChildNodes[2].ToString().Contains("boolean"))
                                        {
                                            simbolo = new Simbolo(Simbolo.EnumTipo.boleano, posicionAbsoluta + 1, nuevoEntorno.tabla.Count, 0, 0, null);
                                        }
                                        else if (hijoTemp.ChildNodes[2].ToString().Contains("integer"))
                                        {
                                            simbolo = new Simbolo(Simbolo.EnumTipo.entero, posicionAbsoluta + 1, nuevoEntorno.tabla.Count, 0, 0, null);
                                        }
                                        else if (hijoTemp.ChildNodes[2].ToString().Contains("string"))
                                        {
                                            simbolo = new Simbolo(Simbolo.EnumTipo.cadena, posicionAbsoluta + 1, nuevoEntorno.tabla.Count, 0, 0, null);
                                        }
                                        else if (hijoTemp.ChildNodes[2].ToString().Contains("id"))
                                        {
                                            Expresion tmp = buscarVariable(hijoTemp.ChildNodes[0], nuevoEntorno);
                                            if (tmp.tipo == Simbolo.EnumTipo.error)
                                            {
                                                Error error = new Error(hijoTemp.ChildNodes[0].Token.Location.Line, hijoTemp.ChildNodes[0].Token.Location.Column, "Semantico", "El objeto o arreglo no existe");
                                                errores.Add(error);
                                            }
                                            else
                                            {
                                                simbolo = nuevoEntorno.buscar(removerExtras(nodoTemp.ChildNodes[2].ToString()));
                                            }
                                        }
                                        if (simbolo is null) { }
                                        else
                                        {
                                            if (simbolo.tipo != Simbolo.EnumTipo.error)
                                            {
                                                if (!nuevoEntorno.insertar(removerExtras(nombreTemp), simbolo))
                                                {
                                                    Error error = new Error(hijoTemp.ChildNodes[0].Token.Location.Line, hijoTemp.ChildNodes[0].Token.Location.Column, "Semantico", "El identificador " + removerExtras(nodoTemp.ChildNodes[0].ToString()) + " ya existe");
                                                    errores.Add(error);
                                                }
                                                else
                                                {
                                                    size += simbolo.size;
                                                    posicionAbsoluta += simbolo.size;
                                                }
                                            }
                                        }
                                    }
                                    if (nuevoEntorno.tabla.Count == 1)
                                    {
                                        nodoTemp = nodoTemp.ChildNodes[2];
                                    }
                                    else
                                    {
                                        nodoTemp = nodoTemp.ChildNodes[1];
                                    }
                                }
                                simbolo = new Simbolo(Simbolo.EnumTipo.objeto, posicionAbsoluta - size, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null, size, nuevoEntorno);
                                if (!entorno.insertar(removerExtras(root.ChildNodes[0].ToString()), simbolo))
                                {
                                    Error error = new Error(root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, "Semantico", "El identificador " + removerExtras(root.ChildNodes[0].ToString()) + " ya existe");
                                    errores.Add(error);
                                }
                                else
                                {
                                    posicionAbsoluta++;
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
