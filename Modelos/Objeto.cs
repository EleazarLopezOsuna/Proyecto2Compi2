using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto2_Compiladores2.Modelos
{
    class Objeto
    {
        public Dictionary<String, Simbolo> parametros;

        public Objeto(string nombre)
        {
            parametros = new Dictionary<string, Simbolo>();
        }
    }
}
