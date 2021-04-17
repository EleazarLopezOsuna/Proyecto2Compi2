using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Irony.Parsing;
using Proyecto2_Compiladores2.Graficador;

namespace Proyecto2_Compiladores2.Analizador
{
    class Sintactico : Grammar
    {
        public static ParseTree Analizar(String cadena)
        {
            Gramatica gramatica = new Gramatica();
            LanguageData lenguaje = new LanguageData(gramatica);
            Parser parser = new Parser(lenguaje);
            ParseTree arbol = parser.Parse(cadena);
            return arbol;
        }

        public static void crearImagen(ParseTreeNode raiz)
        {
            String grafoDot = Arbol.getDot(raiz);
            try
            {
                // Create the file.
                using (FileStream fs = File.Create(@"C:\compiladores2\ArbolIrony.dot"))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(grafoDot);
                    fs.Write(info, 0, info.Length);
                }

                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C dot -Tpng C:/compiladores2/ArbolIrony.dot -o C:/compiladores2/ArbolIrony.png";
                process.StartInfo = startInfo;
                process.Start();

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}