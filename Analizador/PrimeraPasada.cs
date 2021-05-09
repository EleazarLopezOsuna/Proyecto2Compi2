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
        private int posicionAbsoluta;
        private int posicionRelativa;
        private int posicionHeap;
        public ArrayList errores;
        public PrimeraPasada()
        {
            posicionRelativa = 0;
            posicionHeap = 0;
            posicionAbsoluta = 0;
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
                                    simbolo.size = 64;
                                    simbolo.direccionHeap = posicionHeap;
                                    posicionHeap += 64;
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("id"))
                                {
                                    /* Agregar error, pascal no permite asignaciones del tipo
                                     *      type
                                     *          curso = object
                                     *          var codigo:integer;
                                     *          var nombre:string;
                                     *          var nota:integer;
                                     *          var creditos:integer;
                                     *      end;
                                     *      var curso1 : curso;
                                     *      var curso2 : curso = curso1; <- aca el error
                                     */
                                }
                                if (simbolo is null) { }
                                else
                                {
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
                                    simbolo.size = 64;
                                    simbolo.direccionHeap = posicionHeap;
                                    posicionHeap += 64;
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
                                        Simbolo tmpSim = entorno.buscar(removerExtras(root.ChildNodes[1].ChildNodes[0].ToString()));
                                        if (tmp.tipo == Simbolo.EnumTipo.objeto)
                                        {
                                            Dictionary<string, Simbolo> dicTemp = new Dictionary<string, Simbolo>();
                                            int indiceTemp = posicionHeap;
                                            foreach (KeyValuePair<string, Simbolo> pair in tmpSim.atributos.tabla)
                                            {
                                                //posicionHeap++;
                                                //dicTemp.Add(pair.Key, new Simbolo(pair.Value.tipo, posicionHeap, pair.Value.direccionRelativa, pair.Value.fila, pair.Value.columna, pair.Value.root, pair.Value.size, pair.Value.atributos, -1));
                                                if (pair.Value.tipo == Simbolo.EnumTipo.objeto)
                                                {
                                                    int indiceTemporal = posicionHeap + 1;
                                                    foreach (KeyValuePair<string, Simbolo> pareja in pair.Value.atributos.tabla)
                                                    {
                                                        posicionHeap++;
                                                        dicTemp.Add(pareja.Key, new Simbolo(pareja.Value.tipo, posicionHeap, pareja.Value.direccionRelativa, pareja.Value.fila, pareja.Value.columna, pareja.Value.root, pareja.Value.size, pareja.Value.atributos, -1));
                                                    }
                                                }
                                                else
                                                {
                                                    posicionHeap++;
                                                    dicTemp.Add(pair.Key, new Simbolo(pair.Value.tipo, posicionHeap, pair.Value.direccionRelativa, pair.Value.fila, pair.Value.columna, pair.Value.root, pair.Value.size, pair.Value.atributos, -1));
                                                }
                                            }
                                            Entorno entornoTemp = new Entorno(tmpSim.atributos.anterior, tmpSim.atributos.nombreEntorno);
                                            entornoTemp.tabla = dicTemp;
                                            simbolo = new Simbolo(tmp.tipo, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null, tmpSim.size, entornoTemp, indiceTemp);
                                        }
                                        else
                                        {
                                            simbolo = new Simbolo(tmp.tipo, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null, tmpSim.size, tmpSim.contenido);
                                            simbolo.direccionHeap = posicionHeap;
                                            posicionHeap += simbolo.size;
                                        }
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
                                    simbolo.size = 64;
                                    simbolo.direccionHeap = posicionHeap;
                                    posicionHeap += 64;
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
                                        Simbolo tmpSim = entorno.buscar(removerExtras(root.ChildNodes[1].ChildNodes[0].ToString()));
                                        if (tmp.tipo == Simbolo.EnumTipo.objeto)
                                        {
                                            Dictionary<string, Simbolo> dicTemp = new Dictionary<string, Simbolo>();
                                            int indiceTemp = posicionHeap;
                                            int contadory = 0;
                                            foreach (KeyValuePair<string, Simbolo> pair in tmpSim.atributos.tabla)
                                            {
                                                if (pair.Value.tipo == Simbolo.EnumTipo.objeto)
                                                {
                                                    foreach (KeyValuePair<string, Simbolo> pareja in pair.Value.atributos.tabla)
                                                    {
                                                        dicTemp.Add(pair.Key + "." + pareja.Key, new Simbolo(pareja.Value.tipo, posicionHeap, contadory, pareja.Value.fila, pareja.Value.columna, pareja.Value.root, pareja.Value.size, pareja.Value.atributos, -1));
                                                        posicionHeap += pareja.Value.size;
                                                        contadory += pareja.Value.size;
                                                    }
                                                }
                                                else
                                                {
                                                    dicTemp.Add(pair.Key, new Simbolo(pair.Value.tipo, posicionHeap, contadory, pair.Value.fila, pair.Value.columna, pair.Value.root, pair.Value.size, pair.Value.atributos, -1));
                                                    posicionHeap += pair.Value.size;
                                                    contadory += pair.Value.size;
                                                }
                                            }
                                            Entorno entornoTemp = new Entorno(tmpSim.atributos.anterior, tmpSim.atributos.nombreEntorno);
                                            entornoTemp.tabla = dicTemp;
                                            simbolo = new Simbolo(tmp.tipo, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null, contadory, entornoTemp, indiceTemp);
                                        }
                                        else
                                        {
                                            simbolo = new Simbolo(tmp.tipo, posicionAbsoluta, posicionRelativa, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null, tmpSim.size, tmpSim.contenido)
                                            {
                                                direccionHeap = posicionHeap,
                                                limiteInferior = tmpSim.limiteInferior,
                                                limiteSuperior = tmpSim.limiteSuperior
                                            };
                                            posicionHeap += simbolo.size;
                                        }
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
                                    simbolo.size = 64;
                                    simbolo.direccionHeap = posicionHeap;
                                    posicionHeap += 64;
                                }
                                else if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("id"))
                                {
                                    /* Agregar error, pascal no permite asignaciones del tipo
                                     *      type
                                     *          curso = object
                                     *          var codigo:integer;
                                     *          var nombre:string;
                                     *          var nota:integer;
                                     *          var creditos:integer;
                                     *      end;
                                     *      const curso1 : curso;
                                     *      const curso2 : curso = curso1; <- aca el error
                                     */
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
                                    simbolo.size = 64;
                                    simbolo.direccionHeap = posicionHeap;
                                    posicionHeap += 64;
                                }
                                else if (root.ChildNodes[2].ChildNodes[0].ToString().Contains("id"))
                                {
                                    /* Agregar error, pascal no permite asignaciones del tipo
                                     *      type
                                     *          curso = object
                                     *          var codigo:integer;
                                     *          var nombre:string;
                                     *          var nota:integer;
                                     *          var creditos:integer;
                                     *      end;
                                     *      const curso1 : curso;
                                     *      const curso2 = curso1; <- aca el error
                                     */
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
                            ArrayList inferiores = new ArrayList();
                            ArrayList superiores = new ArrayList();
                            while (nodoTemp.ChildNodes.Count > 0)
                            {
                                hijoTemp = nodoTemp.ChildNodes[contador];
                                numero1 = int.Parse(removerExtras(hijoTemp.ChildNodes[0].ChildNodes[0].ToString()));
                                numero2 = int.Parse(removerExtras(hijoTemp.ChildNodes[1].ChildNodes[0].ToString()));
                                inferiores.Add(numero1);
                                superiores.Add(numero2);
                                size *= (numero2 - numero1 + 1);
                                if (contador == 1)
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
                                simbolo = new Simbolo(Simbolo.EnumTipo.real, -1, -1, 0, 0, null);
                            }
                            else if (root.ChildNodes[2].ChildNodes[3].ChildNodes[0].ToString().Contains("boolean"))
                            {
                                simbolo = new Simbolo(Simbolo.EnumTipo.boleano, -1, -1, 0, 0, null);
                            }
                            else if (root.ChildNodes[2].ChildNodes[3].ChildNodes[0].ToString().Contains("integer"))
                            {
                                simbolo = new Simbolo(Simbolo.EnumTipo.entero, -1, -1, 0, 0, null);
                            }
                            else if (root.ChildNodes[2].ChildNodes[3].ChildNodes[0].ToString().Contains("string"))
                            {
                                simbolo = new Simbolo(Simbolo.EnumTipo.cadena, -1, -1, 0, 0, null);
                                simbolo.size = 64;
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
                                Simbolo stmp = new Simbolo(Simbolo.EnumTipo.arreglo, -1, -1, root.ChildNodes[2].ChildNodes[3].ChildNodes[0].Token.Location.Line, root.ChildNodes[2].ChildNodes[3].ChildNodes[0].Token.Location.Column, null, size * simbolo.size, simbolo)
                                {
                                    constante = false,
                                    limiteInferior = inferiores,
                                    limiteSuperior = superiores
                                };
                                if (simbolo.tipo != Simbolo.EnumTipo.error)
                                {
                                    if (!entorno.insertar(removerExtras(root.ChildNodes[0].ToString()), stmp))
                                    {
                                        Error error = new Error(root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, "Semantico", "El identificador " + removerExtras(root.ChildNodes[0].ToString()) + " ya existe");
                                        errores.Add(error);
                                    }
                                    else
                                    {
                                        //posicionAbsoluta++;
                                        //posicionRelativa++;
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
                                int inicioHeap = posicionHeap;
                                Entorno nuevoEntorno = new Entorno(entorno, nombre);
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
                                    if (nuevoEntorno.buscar(nombreTemp) is null)
                                    {
                                        if (hijoTemp.ChildNodes[2].ToString().Contains("real"))
                                        {
                                            simbolo = new Simbolo(Simbolo.EnumTipo.real, -1, -1, 0, 0, null);
                                        }
                                        else if (hijoTemp.ChildNodes[2].ToString().Contains("boolean"))
                                        {
                                            simbolo = new Simbolo(Simbolo.EnumTipo.boleano, -1, -1, 0, 0, null);
                                        }
                                        else if (hijoTemp.ChildNodes[2].ToString().Contains("integer"))
                                        {
                                            simbolo = new Simbolo(Simbolo.EnumTipo.entero, -1, -1, 0, 0, null);
                                        }
                                        else if (hijoTemp.ChildNodes[2].ToString().Contains("string"))
                                        {
                                            simbolo = new Simbolo(Simbolo.EnumTipo.cadena, -1, -1, 0, 0, null);
                                            simbolo.size = 64;
                                        }
                                        else if (hijoTemp.ChildNodes[2].ToString().Contains("id"))
                                        {
                                            Expresion tmp = buscarVariable(hijoTemp.ChildNodes[2], nuevoEntorno);
                                            if (tmp.tipo == Simbolo.EnumTipo.error)
                                            {
                                                Error error = new Error(hijoTemp.ChildNodes[0].Token.Location.Line, hijoTemp.ChildNodes[0].Token.Location.Column, "Semantico", "El objeto o arreglo no existe");
                                                errores.Add(error);
                                            }
                                            else
                                            {
                                                Simbolo tmpSim = nuevoEntorno.buscar(removerExtras(hijoTemp.ChildNodes[2].ToString()));
                                                if (tmp.tipo == Simbolo.EnumTipo.objeto)
                                                {
                                                    Dictionary<string, Simbolo> dicTemp = new Dictionary<string, Simbolo>();
                                                    foreach (KeyValuePair<string, Simbolo> pair in tmpSim.atributos.tabla)
                                                    {
                                                        //posicionHeap++;
                                                        //dicTemp.Add(pair.Key, new Simbolo(pair.Value.tipo, posicionHeap, nuevoEntorno.tabla.Count + pair.Value.direccionRelativa, pair.Value.fila, pair.Value.columna, pair.Value.root, pair.Value.size, pair.Value.atributos, -1));
                                                        if (pair.Value.tipo == Simbolo.EnumTipo.objeto)
                                                        {
                                                            foreach (KeyValuePair<string, Simbolo> pareja in pair.Value.atributos.tabla)
                                                            {
                                                                dicTemp.Add(pareja.Key, new Simbolo(pareja.Value.tipo, -1, -1, pareja.Value.fila, pareja.Value.columna, pareja.Value.root, pareja.Value.size, pareja.Value.atributos, -1));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            dicTemp.Add(pair.Key, new Simbolo(pair.Value.tipo, -1, -1, pair.Value.fila, pair.Value.columna, pair.Value.root, pair.Value.size, pair.Value.atributos, -1));
                                                        }
                                                    }
                                                    Entorno entornoTemp = new Entorno(tmpSim.atributos.anterior, tmpSim.atributos.nombreEntorno);
                                                    entornoTemp.tabla = dicTemp;
                                                    simbolo = new Simbolo(tmp.tipo, -1, -1, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null, tmpSim.size, entornoTemp, -1);
                                                }
                                                else
                                                {
                                                    simbolo = new Simbolo(tmp.tipo, -1, -1, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null, tmpSim.size, tmpSim.contenido);
                                                }
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
                                                    //size++;
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
                                simbolo = new Simbolo(Simbolo.EnumTipo.objeto, -1, -1, root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, null, size, nuevoEntorno, -1);
                                if (!entorno.insertar(removerExtras(root.ChildNodes[0].ToString()), simbolo))
                                {
                                    Error error = new Error(root.ChildNodes[0].Token.Location.Line, root.ChildNodes[0].Token.Location.Column, "Semantico", "El identificador " + removerExtras(root.ChildNodes[0].ToString()) + " ya existe");
                                    errores.Add(error);
                                }
                                else
                                {
                                    //posicionRelativa++;
                                    //posicionAbsoluta++;
                                }
                            }
                            recorrer(root.ChildNodes[4], entorno);
                        }
                    }
                    break;
                case "Z_SUBPROGRAMAS":
                case "SUBPROGRAMA":
                    if (root.ChildNodes.Count > 0)
                    {
                        nombre = removerExtras(root.ChildNodes[0].ChildNodes[0].ChildNodes[0].ToString());
                        MessageBox.Show(nombre);
                        simbolo = entorno.buscar(nombre);
                        if (simbolo is null)
                        {
                            Entorno nuevoEntorno = new Entorno(entorno, nombre);
                            //La variable no existe
                            if (root.ChildNodes[0].ToString().Equals("PROCEDIMIENTO"))
                            {
                                //Es un metodo, no tiene retorno
                            }
                            else
                            {
                                //Es una funcion, tiene retorno
                            }
                        }
                        if (root.ToString().Equals("Z_SUBPROGRAMAS"))
                        {
                            recorrer(root.ChildNodes[1], entorno);
                            recorrer(root.ChildNodes[2], entorno);
                        }
                        else
                        {
                            recorrer(root.ChildNodes[1], entorno);
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
