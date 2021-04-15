using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto2_Compiladores2.Modelos
{
    class Expresion
    {
        public Simbolo.EnumTipo tipo;

        public Expresion(Simbolo.EnumTipo tipo)
        {
            this.tipo = tipo;
        }
    }
}
