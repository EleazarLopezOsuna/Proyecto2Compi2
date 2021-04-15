using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto2_Compiladores2.Modelos
{
    class Entorno
    {
        public Dictionary<String, Simbolo> tabla;
        public Entorno anterior;
        public string nombreEntorno;

        public Entorno(Entorno anterior, string nombreEntorno)
        {
            this.anterior = anterior;
            this.tabla = new Dictionary<String, Simbolo>();
            this.nombreEntorno = nombreEntorno;
        }

        public bool insertar(String nombre, Simbolo sim)
        {
            nombre = nombre.ToLower();
            if (tabla.ContainsKey(nombre))
            {
                return false;
            }
            tabla.Add(nombre, sim);
            return true;
        }

        public Simbolo buscar(String nombre)
        {
            nombre = nombre.ToLower();
            for (Entorno e = this; e != null; e = e.anterior)
            {
                if (e.tabla.ContainsKey(nombre))
                {
                    e.tabla.TryGetValue(nombre, out Simbolo sim);
                    return sim;
                }
            }
            return null;
        }

        public bool modificar(String nombre, Simbolo simbolo)
        {
            nombre = nombre.ToLower();
            for (Entorno e = this; e != null; e = e.anterior)
            {
                if (e.tabla.ContainsKey(nombre))
                {
                    Simbolo nuevo;
                    e.tabla.TryGetValue(nombre, out nuevo);
                    if (simbolo is null)
                        return false;
                    e.tabla.Remove(nombre);
                    e.tabla.Add(nombre, nuevo);
                    return true;
                }
            }
            return false;
        }
    }
}
