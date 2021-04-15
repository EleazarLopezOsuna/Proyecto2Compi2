using Irony.Parsing;
using System;

namespace Proyecto2_Compiladores2.Graficador
{
    class Arbol
    {

        private static int contador;
        private static String grafo;

        public static String getDot(ParseTreeNode raiz)
        {
            grafo = "digraph G{\n";
            grafo += "node[shape=\"box\"];\n";
            contador = 1;
            grafo += "nodo0[label=\"" + escapar(raiz.ToString()) + "\"];\n";
            recorrerAST("nodo0", raiz);
            grafo += "\n}";
            return grafo;
        }

        private static void recorrerAST(String padre, ParseTreeNode hijos)
        {
            foreach (ParseTreeNode hijo in hijos.ChildNodes)
            {
                String nombreHijo = "nodo" + contador.ToString();
                grafo += nombreHijo + "[label=\"" + escapar(hijo.ToString()) + "\"];\n";
                grafo += padre + "->" + nombreHijo + "\n";
                contador++;
                recorrerAST(nombreHijo, hijo);
            }
        }

        private static String escapar(String cadena)
        {
            cadena = cadena.Replace("\\", "\\\\");
            cadena = cadena.Replace("\"", "\\\"");
            return cadena;
        }
    }
}