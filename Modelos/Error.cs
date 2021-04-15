using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto2_Compiladores2.Modelos
{
    class Error
    {
        public int fila;
        public int columna;
        public String tipo;
        public String error;

        public Error(int fila, int columna, String tipo, String error)
        {
            this.fila = fila;
            this.columna = columna;
            this.tipo = tipo;
            this.error = error;
        }
    }
}
