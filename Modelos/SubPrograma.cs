using System;
using System.Collections;
using System.Collections.Generic;
using Irony.Parsing;

namespace Proyecto2_Compiladores2.Modelos
{
    class SubPrograma
    {
        public Dictionary<string, Simbolo> parametrosVariable;
        public Dictionary<string, Simbolo> parametrosValor;
        public Dictionary<string, string> correlacionParametros;
        public ArrayList ordenParametros;
        public ParseTreeNode root;
        public Simbolo.EnumTipo tipo;
        public Simbolo retorno;
        public Entorno entorno;

        public SubPrograma(ParseTreeNode root, Entorno entorno, int direccionAbsoluta, int fila, int columna)
        {
            this.root = root;
            parametrosValor = new Dictionary<string, Simbolo>();
            parametrosVariable = new Dictionary<string, Simbolo>();
            correlacionParametros = new Dictionary<string, string>();
            retorno = new Simbolo(Simbolo.EnumTipo.nulo, direccionAbsoluta, direccionAbsoluta, fila, columna);
            tipo = Simbolo.EnumTipo.nulo;
            this.entorno = entorno;
            ordenParametros = new ArrayList();
        }
        public void agregarEntorno()
        {
            foreach (KeyValuePair<string, Simbolo> parametro in parametrosVariable)
            {
                entorno.insertar(parametro.Key, parametro.Value);
            }
            foreach (KeyValuePair<string, Simbolo> parametro in parametrosValor)
            {
                entorno.insertar(parametro.Key, parametro.Value);
            }
        }
        public void buscarTipo(string cadena, Entorno entorno)
        {
            switch (cadena)
            {
                case "string":
                    tipo = Simbolo.EnumTipo.cadena;
                    break;
                case "integer":
                    tipo = Simbolo.EnumTipo.entero;
                    break;
                case "real":
                    tipo = Simbolo.EnumTipo.real;
                    break;
                case "boolean":
                    tipo = Simbolo.EnumTipo.boleano;
                    break;
                default:
                    Simbolo sim = entorno.buscar(cadena);
                    if (sim.tipo == Simbolo.EnumTipo.arreglo || sim.tipo == Simbolo.EnumTipo.objeto)
                    {
                        tipo = sim.tipo;
                        break;
                    }
                    tipo = Simbolo.EnumTipo.error;
                    break;
            }
            retorno.tipo = this.tipo;
        }
    }
}
