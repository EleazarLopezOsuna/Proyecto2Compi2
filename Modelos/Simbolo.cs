using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Irony.Parsing;

namespace Proyecto2_Compiladores2.Modelos
{
    class Simbolo
    {
        public EnumTipo tipo;
        public int fila;
        public int columna;
        public bool constante;
        public int direccionAbsoluta;
        public int direccionRelativa;
        public int size;
        public Simbolo contenido;
        public ParseTreeNode root;
        public Entorno atributos;
        public int direccionHeap;
        public ArrayList limiteInferior;
        public ArrayList limiteSuperior;
        public int tipoParametro; // 0 -> no aplica | 1 -> por valor | 2 -> por referencia

        public Simbolo(EnumTipo tipo, int direccionAbsoluta, int direccionRelativa, int fila, int columna, ParseTreeNode root)
        {
            this.tipo = tipo;
            constante = false;
            this.direccionAbsoluta = direccionAbsoluta;
            this.direccionRelativa = direccionRelativa;
            this.fila = fila;
            this.columna = columna;
            this.root = root;
            size = 1;
            direccionHeap = -1;
            limiteInferior = new ArrayList();
            limiteSuperior = new ArrayList();
            tipoParametro = 0;
        }
        public Simbolo(EnumTipo tipo, int direccionAbsoluta, int direccionRelativa, int fila, int columna, ParseTreeNode root, int size, Simbolo contenido)
        {
            this.tipo = tipo;
            constante = false;
            this.direccionAbsoluta = direccionAbsoluta;
            this.direccionRelativa = direccionRelativa;
            this.fila = fila;
            this.columna = columna;
            this.root = root;
            this.size = size;
            this.contenido = contenido;
            direccionHeap = -1;
            limiteInferior = new ArrayList();
            limiteSuperior = new ArrayList();
            tipoParametro = 0;
        }
        public Simbolo(EnumTipo tipo, int direccionAbsoluta, int direccionRelativa, int fila, int columna, ParseTreeNode root, int size, Entorno atributos, int direccionHeap)
        {
            this.tipo = tipo;
            constante = false;
            this.direccionAbsoluta = direccionAbsoluta;
            this.direccionRelativa = direccionRelativa;
            this.fila = fila;
            this.columna = columna;
            this.root = root;
            this.size = size;
            this.atributos = atributos;
            this.direccionHeap = direccionHeap;
            limiteInferior = new ArrayList();
            limiteSuperior = new ArrayList();
            tipoParametro = 0;
        }
        public enum EnumTipo
        {
            cadena, entero, real, boleano, nulo, error, funcion, procedimiento, objeto, arreglo
        }
    }
}
