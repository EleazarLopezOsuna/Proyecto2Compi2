using System;
using System.Collections.Generic;
using System.Text;

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

        public Simbolo(EnumTipo tipo, int direccionAbsoluta, int direccionRelativa, int fila, int columna)
        {
            this.tipo = tipo;
            this.constante = false;
            this.direccionAbsoluta = direccionAbsoluta;
            this.direccionRelativa = direccionRelativa;
            this.fila = fila;
            this.columna = columna;
        }

        public enum EnumTipo
        {
            cadena, entero, real, boleano, nulo, error, funcion, procedimiento, objeto, arreglo
        }
    }
}
