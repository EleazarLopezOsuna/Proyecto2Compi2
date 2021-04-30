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

        public Simbolo(EnumTipo tipo, int direccionAbsoluta, int direccionRelativa, int fila, int columna, ParseTreeNode root)
        {
            this.tipo = tipo;
            this.constante = false;
            this.direccionAbsoluta = direccionAbsoluta;
            this.direccionRelativa = direccionRelativa;
            this.fila = fila;
            this.columna = columna;
            this.root = root;
            size = 1;
        }
        public Simbolo(EnumTipo tipo, int direccionAbsoluta, int direccionRelativa, int fila, int columna, ParseTreeNode root, int size, Simbolo contenido)
        {
            this.tipo = tipo;
            this.constante = false;
            this.direccionAbsoluta = direccionAbsoluta;
            this.direccionRelativa = direccionRelativa;
            this.fila = fila;
            this.columna = columna;
            this.root = root;
            this.size = size;
            this.contenido = contenido;
        }
        public Simbolo(EnumTipo tipo, int direccionAbsoluta, int direccionRelativa, int fila, int columna, ParseTreeNode root, int size, Entorno atributos)
        {
            this.tipo = tipo;
            this.constante = false;
            this.direccionAbsoluta = direccionAbsoluta;
            this.direccionRelativa = direccionRelativa;
            this.fila = fila;
            this.columna = columna;
            this.root = root;
            this.size = size;
            this.atributos = atributos;
        }
        public enum EnumTipo
        {
            cadena, entero, real, boleano, nulo, error, funcion, procedimiento, objeto, arreglo
        }
    }
}
