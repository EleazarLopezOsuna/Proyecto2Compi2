using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Irony.Parsing;
using Proyecto2_Compiladores2.Modelos;
using Proyecto2_Compiladores2.Traduccion;

namespace Proyecto2_Compiladores2.Analizador
{
    class SegundaPasada
    {
        public Entorno entorno;
        int posicionAbsoluta;
        public int posicionRelativa;
        public ArrayList errores;
        public string traduccion;
        private Declaracion declaracion;
        public int contadorTemporal;
        public int contadorEtiqueta;
        private int temporalSwitch;
        private int temporalSalidaSwitch;
        public int SP, HP;
        private string acumulado;
        private Entorno entornoGlobal;
        public SegundaPasada(int contadorEtiqueta, Entorno entornoGlobal)
        {
            posicionRelativa = 0;
            declaracion = new Declaracion(0, contadorEtiqueta, entornoGlobal);
            contadorTemporal = 1;
            temporalSwitch = 0;
            temporalSalidaSwitch = 0;
            this.contadorEtiqueta = contadorEtiqueta;
            SP = HP = 0;
            acumulado = "";
        }
        public void iniciarSegundaPasada(ParseTreeNode root, int posicionAbsoluta, Entorno entorno)
        {
            this.entorno = entorno;
            traduccion = "";
            this.posicionAbsoluta = posicionAbsoluta;
            errores = new ArrayList();
            recorrer(root, entorno, 0, 0);
        }
        private Expresion buscarVariable(ParseTreeNode root, Entorno entorno)
        {
            Simbolo resultadoBusqueda = entorno.buscar(removerExtras(root.ToString()));
            if (resultadoBusqueda is null)
            {
                return new Expresion(Simbolo.EnumTipo.error);
            }
            else
            {
                return new Expresion(resultadoBusqueda.tipo);
            }
        }
        private void recorrer(ParseTreeNode root, Entorno entorno, int etiquetaFinCiclo, int etiquetaInicioCiclo)
        {
            Simbolo simbolo = null;
            string str;
            int etiquetaVerdadera;
            int etiquetaFalsa;
            int tmp;
            string nombreVariable;
            int etiquetaInicial;
            int etiquetaSalida;
            switch (root.ToString())
            {
                case "PROGRAMA":
                case "BEGIN_END":
                case "SENTENCIA":
                    if (root.ChildNodes.Count > 0)
                    {
                        foreach (ParseTreeNode hijo in root.ChildNodes)
                        {
                            recorrer(hijo, entorno, etiquetaFinCiclo, etiquetaInicioCiclo);
                        }
                    }
                    break;
                case "IF":
                    if (root.ChildNodes.Count == 3)
                    {
                        //if expresion then instrucciones else instrucciones
                        //      0                1                 2
                        traduccion += "//Inicio de calculo de la expresion condicional de IF" + Environment.NewLine;
                        declaracion.contadorTemporal = 0;
                        str = declaracion.ResolverExpresion(root.ChildNodes[0], entorno, HP) + Environment.NewLine;
                        traduccion += str;
                        traduccion += "//Fin de calculo de la expresion condicional de IF" + Environment.NewLine;
                        if (declaracion.contadorTemporal > contadorTemporal)
                            contadorTemporal = declaracion.contadorTemporal;
                        contadorEtiqueta++;
                        etiquetaVerdadera = contadorEtiqueta;
                        traduccion += "if (T" + declaracion.contadorTemporal + ") goto L" + etiquetaVerdadera + "; //Movimiento a etiqueta verdadera" + Environment.NewLine;
                        declaracion.contadorTemporal = 0;
                        contadorEtiqueta++;
                        etiquetaSalida = contadorEtiqueta;
                        contadorEtiqueta++;
                        etiquetaFalsa = contadorEtiqueta;
                        traduccion += "goto L" + etiquetaFalsa + "; //Movimiento a etiqueta falsa" + Environment.NewLine;
                        traduccion += "L" + etiquetaVerdadera + ":" + Environment.NewLine;
                        recorrer(root.ChildNodes[1], entorno, etiquetaFinCiclo, etiquetaInicioCiclo);
                        traduccion += "goto L" + etiquetaSalida + "; //Movimiento a etiqueta de escape del if" + Environment.NewLine;
                        traduccion += "L" + etiquetaFalsa + ":" + Environment.NewLine;
                        recorrer(root.ChildNodes[2], entorno, etiquetaFinCiclo, etiquetaInicioCiclo);
                        //Codigo para salir del if cuando fue verdadero
                        traduccion += "L" + etiquetaSalida + ": //Salio del if" + Environment.NewLine;
                    }
                    else if (root.ChildNodes.Count == 2)
                    {
                        //if expresion then instrucciones
                        //      0                1
                        traduccion += "//Inicio de calculo de la expresion condicional de IF" + Environment.NewLine;
                        declaracion.contadorTemporal = 0;
                        str = declaracion.ResolverExpresion(root.ChildNodes[0], entorno, HP) + Environment.NewLine;
                        traduccion += str;
                        traduccion += "//Fin de calculo de la expresion condicional de IF" + Environment.NewLine;
                        if (declaracion.contadorTemporal > contadorTemporal)
                            contadorTemporal = declaracion.contadorTemporal;
                        contadorEtiqueta++;
                        etiquetaVerdadera = contadorEtiqueta;
                        traduccion += "if (T" + declaracion.contadorTemporal + ") goto L" + etiquetaVerdadera + "; //Movimiento a etiqueta verdadera" + Environment.NewLine;
                        declaracion.contadorTemporal = 0;
                        contadorEtiqueta++;
                        etiquetaSalida = contadorEtiqueta;
                        traduccion += "goto L" + etiquetaSalida + "; //Movimiento a etiqueta falsa" + Environment.NewLine;
                        traduccion += "L" + etiquetaVerdadera + ":" + Environment.NewLine;
                        recorrer(root.ChildNodes[1], entorno, etiquetaFinCiclo, etiquetaInicioCiclo);
                        //Codigo para salir del if sin importar si fue verdadero o falso
                        traduccion += "L" + etiquetaSalida + ": //Salio del if" + Environment.NewLine;
                    }
                    break;
                case "WHILE":
                    //while expresion do instrucciones
                    //         0              1
                    contadorEtiqueta++;
                    etiquetaInicial = contadorEtiqueta;
                    traduccion += "L" + etiquetaInicial + ":" + Environment.NewLine;
                    traduccion += "//Inicio de calculo de la expresion condicional del WHILE" + Environment.NewLine;
                    declaracion.contadorTemporal = 0;
                    str = declaracion.ResolverExpresion(root.ChildNodes[0], entorno, HP) + Environment.NewLine;
                    traduccion += str;
                    traduccion += "//Fin de calculo de la expresion condicional del WHILE" + Environment.NewLine;
                    if (declaracion.contadorTemporal > contadorTemporal)
                        contadorTemporal = declaracion.contadorTemporal;
                    contadorEtiqueta++;
                    etiquetaVerdadera = contadorEtiqueta;
                    traduccion += "if (T" + declaracion.contadorTemporal + ") goto L" + etiquetaVerdadera + "; //Movimiento hacia etiqueta de ejecucion del WHILE" + Environment.NewLine;
                    contadorEtiqueta++;
                    etiquetaSalida = contadorEtiqueta;
                    traduccion += "goto L" + etiquetaSalida + ";" + Environment.NewLine;
                    traduccion += "L" + etiquetaVerdadera + ":" + Environment.NewLine;
                    declaracion.contadorTemporal = 0;
                    recorrer(root.ChildNodes[1], entorno, etiquetaSalida, etiquetaInicial);
                    traduccion += "goto L" + etiquetaInicial + ";" + Environment.NewLine;
                    traduccion += "L" + etiquetaSalida + ":" + Environment.NewLine;
                    break;
                case "REPEAT":
                    // repeat instrucciones instrucciones until condicion
                    //              0               1               2
                    contadorEtiqueta++;
                    etiquetaInicial = contadorEtiqueta;
                    traduccion += "L" + etiquetaInicial + ": //Etiqueta que permite generar el ciclo" + Environment.NewLine;
                    declaracion.contadorTemporal = 0;
                    traduccion += "//Inicio de instrucciones pertenecientes al bloque REPEAT" + Environment.NewLine;
                    contadorEtiqueta++;
                    etiquetaSalida = contadorEtiqueta;
                    recorrer(root.ChildNodes[0], entorno, etiquetaSalida, etiquetaInicial);
                    recorrer(root.ChildNodes[1], entorno, etiquetaSalida, etiquetaInicial);
                    traduccion += "//Fin de instrucciones pertenecientes al bloque REPEAT" + Environment.NewLine;
                    traduccion += "//Inicio de calculo de la expresion condicional del REPEAT" + Environment.NewLine;
                    declaracion.contadorTemporal = 0;
                    str = declaracion.ResolverExpresion(root.ChildNodes[2], entorno, HP) + Environment.NewLine;
                    traduccion += str;
                    traduccion += "//Fin de calculo de la expresion condicional del REPEAT" + Environment.NewLine;
                    if (declaracion.contadorTemporal > contadorTemporal)
                        contadorTemporal = declaracion.contadorTemporal;
                    traduccion += "if (!T" + declaracion.contadorTemporal + ") goto L" + etiquetaInicial + "; //Movimiento hacia el ciclo REPEAT" + Environment.NewLine;
                    traduccion += "goto L" + etiquetaSalida + ";" + Environment.NewLine;
                    traduccion += "L" + etiquetaSalida + ":" + Environment.NewLine;
                    break;
                case "FOR":
                    // for asignacion TO-DOWNTO instrucciones
                    //         0           1         2
                    contadorEtiqueta++;
                    traduccion += "//Inicializacion de variable" + Environment.NewLine;
                    etiquetaInicial = contadorEtiqueta;
                    contadorTemporal++;
                    tmp = contadorTemporal;
                    contadorEtiqueta++;
                    etiquetaSalida = contadorEtiqueta;
                    recorrer(root.ChildNodes[0], entorno, etiquetaSalida, etiquetaInicial);
                    traduccion += "L" + etiquetaInicial + ": //Etiqueta que permite generar el ciclo FOR" + Environment.NewLine;
                    nombreVariable = removerExtras(root.ChildNodes[0].ChildNodes[0].ChildNodes[0].ToString());
                    simbolo = entorno.buscar(removerExtras(nombreVariable));
                    traduccion += "T" + tmp + " = STACK[" + simbolo.direccionRelativa + "];" + Environment.NewLine;
                    declaracion.contadorTemporal = 0;
                    traduccion += "//Inicio de calculo de la expresion condicional del FOR" + Environment.NewLine;
                    declaracion.contadorTemporal = 0;
                    str = declaracion.ResolverExpresion(root.ChildNodes[1].ChildNodes[0], entorno, HP);
                    //tmp = contadorTemporal;
                    if (!str.StartsWith("T") && !str.StartsWith("S_"))
                    {
                        contadorTemporal++;
                        str = "T" + contadorTemporal + " = " + str;
                    }
                    traduccion += str + Environment.NewLine;
                    traduccion += "//Fin de calculo de la expresion condicional del FOR" + Environment.NewLine;
                    contadorEtiqueta++;
                    etiquetaVerdadera = contadorEtiqueta;
                    if (root.ChildNodes[1].ToString().Equals("ARRIBA"))
                    {
                        //TO
                        traduccion += "if (T" + tmp + " <= T" + contadorTemporal + ") goto L" + etiquetaVerdadera + ";" + Environment.NewLine;
                    }
                    else
                    {
                        //DOWN TO
                        traduccion += "if (T" + tmp + " >= T" + contadorTemporal + ") goto L" + etiquetaVerdadera + ";" + Environment.NewLine;
                    }
                    if (declaracion.contadorTemporal > contadorTemporal)
                        contadorTemporal = declaracion.contadorTemporal;
                    traduccion += "goto L" + etiquetaSalida + ";" + Environment.NewLine;
                    traduccion += "//Inicio de instrucciones pertenecientes al bloque FOR" + Environment.NewLine;
                    traduccion += "L" + etiquetaVerdadera + ": //Etiqueta verdadera, se ejecutan las instrucciones" + Environment.NewLine;
                    recorrer(root.ChildNodes[2], entorno, etiquetaSalida, etiquetaInicial);

                    contadorTemporal++;
                    traduccion += "T" + contadorTemporal + " = STACK[" + simbolo.direccionAbsoluta + "] + 1 ;" + Environment.NewLine;
                    traduccion += "STACK[" + simbolo.direccionAbsoluta + "] = T" + contadorTemporal + ";" + Environment.NewLine;

                    traduccion += "goto L" + etiquetaInicial + ";" + Environment.NewLine;
                    traduccion += "//Fin de instrucciones pertenecientes al bloque FOR" + Environment.NewLine;
                    traduccion += "L" + etiquetaSalida + ": //Salida del FOR" + Environment.NewLine;
                    break;
                case "CASE":
                    //Case
                    // expresion case listaCase
                    //     0       1      2
                    traduccion += "//Inicio de calculo de la expresion del SWITCH" + Environment.NewLine;
                    declaracion.contadorTemporal = 0;
                    str = declaracion.ResolverExpresion(root.ChildNodes[0], entorno, HP);
                    //tmp = contadorTemporal;
                    if (!str.StartsWith("T") && !str.StartsWith("S_"))
                    {
                        contadorTemporal++;
                        str = "T" + contadorTemporal + " = " + str;
                    }
                    temporalSwitch = contadorTemporal;
                    contadorTemporal++;
                    contadorEtiqueta++;
                    temporalSalidaSwitch = contadorEtiqueta;
                    traduccion += str + Environment.NewLine;
                    traduccion += "//Fin de calculo de la expresion del SWITCH" + Environment.NewLine;
                    recorrer(root.ChildNodes[1], entorno, etiquetaFinCiclo, etiquetaInicioCiclo);
                    recorrer(root.ChildNodes[2], entorno, etiquetaFinCiclo, etiquetaInicioCiclo);
                    if (root.ChildNodes.Count == 4)
                    {
                        //Case-else
                        // expresion case listaCase sentencia(else)
                        //     0       1      2            3
                        recorrer(root.ChildNodes[3], entorno, etiquetaFinCiclo, etiquetaInicioCiclo);
                    }
                    traduccion += "L" + temporalSalidaSwitch + ": //Salida del SWITCH" + Environment.NewLine;
                    break;
                case "OPCION_CASE":
                    if (root.ChildNodes.Count > 0)
                    {
                        if (!root.ChildNodes[0].ToString().Equals("OPCION_CASE"))
                        {
                            traduccion += "//Inicio de calculo de expresion del CASE" + Environment.NewLine;
                            declaracion.contadorTemporal = 0;
                            str = declaracion.ResolverExpresion(root.ChildNodes[0], entorno, HP);
                            if (declaracion.contadorTemporal > contadorTemporal)
                                contadorTemporal = declaracion.contadorTemporal;
                            //tmp = contadorTemporal;
                            if (!str.StartsWith("T") && !str.StartsWith("S_"))
                            {
                                contadorTemporal++;
                                str = "T" + contadorTemporal + " = " + str;
                            }
                            traduccion += str + Environment.NewLine;
                            traduccion += "//Fin de calculo de expresion del CASE" + Environment.NewLine;
                            contadorEtiqueta++;
                            etiquetaVerdadera = contadorEtiqueta;
                            contadorEtiqueta++;
                            etiquetaSalida = contadorEtiqueta;
                            contadorEtiqueta++;
                            etiquetaFalsa = contadorEtiqueta;
                            traduccion += "if (T" + contadorTemporal + " == T" + temporalSwitch + ") goto L" + etiquetaVerdadera + "; //Entra al case en caso que se cumpla la condicion" + Environment.NewLine;
                            declaracion.contadorTemporal = 0;
                            traduccion += "goto L" + etiquetaFalsa + "; //Movimiento a la etiqueta falsa" + Environment.NewLine;
                            traduccion += "L" + etiquetaVerdadera + ":" + Environment.NewLine;
                            contadorTemporal++;
                            recorrer(root.ChildNodes[1], entorno, etiquetaFinCiclo, etiquetaInicioCiclo);
                            traduccion += "goto L" + temporalSalidaSwitch + "; //Movimiento a etiqueta de escape del CASE" + Environment.NewLine;
                            traduccion += "L" + etiquetaFalsa + ":" + Environment.NewLine;
                            //Codigo para salir del if cuando fue verdadero
                            traduccion += "L" + etiquetaSalida + ": //Salio del if" + Environment.NewLine;
                        }
                        else
                        {
                            recorrer(root.ChildNodes[0], entorno, etiquetaFinCiclo, etiquetaInicioCiclo);
                            recorrer(root.ChildNodes[1], entorno, etiquetaFinCiclo, etiquetaInicioCiclo);
                        }
                    }
                    break;
                case "LLAMADA":
                    if (root.ChildNodes[0].ToString().Contains("writeln (id)") || root.ChildNodes[0].ToString().Contains("write (id)"))
                    {
                        if (root.ChildNodes[1].ChildNodes[0].ToString().Equals("VARIABLE") || root.ChildNodes[1].ChildNodes.Count > 1)
                        {
                            Simbolo st = entorno.buscar(removerExtras(root.ChildNodes[1].ChildNodes[0].ChildNodes[0].ToString()));
                            if (st.tipo != Simbolo.EnumTipo.cadena)
                            {
                                declaracion.contadorTemporal = 0;
                                str = declaracion.ResolverExpresion(root.ChildNodes[1], entorno, HP);
                                if (!str.StartsWith("T") && !str.StartsWith("S_"))
                                {
                                    declaracion.contadorTemporal++;
                                    str = "T" + declaracion.contadorTemporal + " = " + str + Environment.NewLine;
                                }
                                if (declaracion.contadorTemporal > contadorTemporal)
                                {
                                    contadorTemporal = declaracion.contadorTemporal;
                                }
                                traduccion += str;
                                traduccion += "printf(\"%f\",T" + declaracion.contadorTemporal + ");" + Environment.NewLine; //Eliminar luego, es solo para control
                            }
                            else
                            {
                                imprimirBucle(st.direccionHeap, st.size, true);
                            }
                        }
                        else if (root.ChildNodes[1].ChildNodes[0].ToString().Equals("ESTRUCTURA"))
                        {
                            if (!root.ChildNodes[1].ChildNodes[0].ChildNodes[0].ToString().Equals("LLAMADA"))
                            {
                                int[] datos = obtenerDatosEstructura(root.ChildNodes[1].ChildNodes[0], entorno);
                                acumulado = "";
                                if (datos != null)
                                {
                                    imprimirBucle(datos[0], datos[1], Convert.ToBoolean(datos[2]));
                                }
                            }
                            else
                            {
                                ejecutarLlamada(root.ChildNodes[1].ChildNodes[0].ChildNodes[0]);
                                traduccion += "printf(\"%f\", HEAP[(int)HP + 0]);" + Environment.NewLine;
                                traduccion += "HP = S_HP;" + Environment.NewLine;
                            }
                        }
                        else
                        {
                            traduccion += "printf(\"" + removerExtras(root.ChildNodes[1].ChildNodes[0].ToString()) + "\");" + Environment.NewLine; //Eliminar luego, es solo para control
                        }
                        ParseTreeNode nodoTemporal = root.ChildNodes[2];
                        while (nodoTemporal.ChildNodes.Count > 0)
                        {
                            if (nodoTemporal.ChildNodes[0].ChildNodes[0].ToString().Equals("VARIABLE") || nodoTemporal.ChildNodes[0].ChildNodes.Count > 1 || (nodoTemporal.ChildNodes[0].ChildNodes[0].ChildNodes.Count > 1 && !nodoTemporal.ChildNodes[0].ChildNodes[0].ToString().Equals("ESTRUCTURA")))
                            {
                                Simbolo sim = null;
                                if (nodoTemporal.ChildNodes[0].ChildNodes[0].ToString().Equals("VARIABLE"))
                                {
                                    sim = entorno.buscar(removerExtras(nodoTemporal.ChildNodes[0].ChildNodes[0].ChildNodes[0].ToString()));
                                    if (sim.tipo != Simbolo.EnumTipo.cadena)
                                        sim = null;
                                }
                                if (sim is null)
                                {
                                    declaracion.contadorTemporal = 0;
                                    str = declaracion.ResolverExpresion(nodoTemporal.ChildNodes[0], entorno, HP);
                                    if (!str.StartsWith("T") && !str.StartsWith("S_"))
                                    {
                                        declaracion.contadorTemporal++;
                                        str = "T" + declaracion.contadorTemporal + " = " + str + Environment.NewLine;
                                    }
                                    if (declaracion.contadorTemporal > contadorTemporal)
                                    {
                                        contadorTemporal = declaracion.contadorTemporal;
                                    }
                                    traduccion += str + Environment.NewLine;
                                    traduccion += "printf(\"%f\",T" + declaracion.contadorTemporal + ");" + Environment.NewLine; //Eliminar luego, es solo para control
                                }
                                else
                                {
                                    bool tipo = false;
                                    if (sim.tipo == Simbolo.EnumTipo.cadena)
                                        tipo = true;
                                    imprimirBucle(sim.direccionHeap, sim.size, tipo);
                                }
                            }
                            else if (nodoTemporal.ChildNodes[0].ChildNodes[0].ToString().Equals("ESTRUCTURA"))
                            {
                                int[] datos = obtenerDatosEstructura(nodoTemporal.ChildNodes[0].ChildNodes[0], entorno);
                                acumulado = "";
                                if (datos != null)
                                {
                                    imprimirBucle(datos[0], datos[1], Convert.ToBoolean(datos[2]));
                                }
                            }
                            else
                            {
                                str = removerExtras(nodoTemporal.ChildNodes[0].ChildNodes[0].ToString());
                                if (str.Equals(""))
                                {
                                    str = " ";
                                }
                                traduccion += "printf(\"" + str + "\");" + Environment.NewLine; //Eliminar luego, es solo para control
                            }
                            nodoTemporal = nodoTemporal.ChildNodes[1];
                        }
                        if (root.ChildNodes[0].ToString().Contains("writeln (id)"))
                        {
                            traduccion += "printf(\"%c\", 10);" + Environment.NewLine;
                        }
                    }
                    else
                    {
                        ejecutarLlamada(root);
                        traduccion += "HP = S_HP;" + Environment.NewLine;
                    }
                    break;
                case "BREAK":
                    traduccion += "goto L" + etiquetaFinCiclo + "; //Salimos del ciclo en el cual estamos" + Environment.NewLine;
                    break;
                case "CONTINUE":
                    traduccion += "goto L" + etiquetaInicioCiclo + "; //Salimos del ciclo en el cual estamos" + Environment.NewLine;
                    break;
                case "ASIGNACION":
                    if (root.ChildNodes.Count == 2)
                    {
                        if (root.ChildNodes[0].ToString().Equals("VARIABLE"))
                        {
                            nombreVariable = removerExtras(root.ChildNodes[0].ChildNodes[0].ToString());
                            simbolo = entorno.buscar(removerExtras(nombreVariable));
                            if (simbolo is null)
                            {
                                MessageBox.Show("Error la variable no existe");
                            }
                            else
                            {
                                //identificador := expresion
                                //     0               1
                                if (!root.ChildNodes[1].ChildNodes[0].ToString().Contains("cadena"))
                                {
                                    if (!root.ChildNodes[1].ChildNodes[0].ToString().Equals("ESTRUCTURA"))
                                    {
                                        if (root.ChildNodes[1].ChildNodes[0].ToString().Equals("VARIABLE"))
                                        {
                                            Simbolo sm = entorno.buscar(removerExtras(root.ChildNodes[1].ChildNodes[0].ChildNodes[0].ToString()));
                                            if (!(sm is null))
                                            {
                                                int[] izquierda = { simbolo.direccionAbsoluta, simbolo.size };
                                                if (simbolo.direccionHeap != -1)
                                                {
                                                    izquierda[0] = simbolo.direccionHeap;
                                                }
                                                int[] derecha = { sm.direccionAbsoluta, sm.size };
                                                if (sm.direccionHeap != -1)
                                                {
                                                    derecha[0] = sm.direccionHeap;
                                                }
                                                if (izquierda[1] == derecha[1])
                                                {
                                                    asignacionBucle(izquierda, derecha, simbolo.direccionHeap, sm.direccionHeap);
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Error de tipos");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            declaracion.contadorTemporal = 0;
                                            str = declaracion.ResolverExpresion(root.ChildNodes[1], entorno, HP) + Environment.NewLine;
                                            if (declaracion.contadorTemporal > contadorTemporal)
                                                contadorTemporal = declaracion.contadorTemporal;
                                            if (!str.StartsWith("T") && !str.StartsWith("S_"))
                                            {
                                                contadorTemporal++;
                                                str = "//Inicio de modificacion de identificador " + nombreVariable + Environment.NewLine +
                                                    "T" + contadorTemporal + " = " + str;
                                            }
                                            else
                                            {
                                                str = "//Inicio de modificacion de identificador " + nombreVariable + Environment.NewLine + str;
                                            }
                                            traduccion += str;
                                            if (simbolo.direccionHeap == -1)
                                            {
                                                if (declaracion.contadorTemporal > contadorTemporal)
                                                {
                                                    contadorTemporal = declaracion.contadorTemporal;
                                                    traduccion += "STACK[" + simbolo.direccionAbsoluta + "] = T" + contadorTemporal + ";" + Environment.NewLine;
                                                }
                                                else
                                                {
                                                    if (declaracion.contadorTemporal == 0)
                                                        declaracion.contadorTemporal = contadorTemporal;
                                                    traduccion += "STACK[" + simbolo.direccionAbsoluta + "] = T" + declaracion.contadorTemporal + ";" + Environment.NewLine;
                                                }
                                            }
                                            else
                                            {
                                                if (declaracion.contadorTemporal > contadorTemporal)
                                                {
                                                    contadorTemporal = declaracion.contadorTemporal;
                                                    traduccion += "HEAP[(int)HP + " + simbolo.direccionRelativa + "] = T" + contadorTemporal + ";" + Environment.NewLine;
                                                }
                                                else
                                                {
                                                    if (declaracion.contadorTemporal == 0)
                                                        declaracion.contadorTemporal = contadorTemporal;
                                                    traduccion += "HEAP[(int)HP + " + simbolo.direccionRelativa + "] = T" + declaracion.contadorTemporal + ";" + Environment.NewLine;
                                                }
                                            }
                                            declaracion.contadorTemporal = 0;
                                        }
                                    }
                                    else
                                    {
                                        int[] derecha = obtenerDatosEstructura(root.ChildNodes[1].ChildNodes[0], entorno);
                                        acumulado = "";
                                        int[] izquierda = { simbolo.direccionAbsoluta, simbolo.size };
                                        if (izquierda[1] == derecha[1])
                                        {
                                            asignacionBucle(izquierda, derecha, simbolo.direccionHeap, derecha[2]);
                                        }
                                        else
                                        {
                                            MessageBox.Show("Error de tipos");
                                        }
                                    }
                                }
                                else
                                {
                                    if (simbolo.tipo == Simbolo.EnumTipo.cadena)
                                    {
                                        string contenido = removerExtras(root.ChildNodes[1].ChildNodes[0].ToString());
                                        for (int i = 0; i < contenido.Length; i++)
                                        {
                                            if (i == 63)
                                                break;
                                            traduccion += "HEAP[" + (simbolo.direccionHeap + i) + "] = " + Convert.ToInt32(contenido[i]) + "; //Letra: " + contenido[i] + Environment.NewLine;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Error de tipos");
                                    }
                                }
                            }
                        }
                        else if (root.ChildNodes[0].ToString().Equals("ESTRUCTURA"))
                        {
                            int[] izquierda = new int[2];
                            int[] derecha = new int[2];
                            if (root.ChildNodes[0].ChildNodes.Count == 4)
                            {
                                //Es un arreglo
                                if (root.ChildNodes[1].ChildNodes.Count == 1 && root.ChildNodes[1].ChildNodes[0].ToString().Equals("ESTRUCTURA"))
                                {
                                    // arreglo[pos{, pos}] = arreglo[pos{, pos}]
                                    // arreglo[pos{, pos}] = objeto.atributo{.atributo}
                                    izquierda = obtenerDatosEstructura(root.ChildNodes[0], entorno);
                                    acumulado = "";
                                    derecha = obtenerDatosEstructura(root.ChildNodes[1], entorno);
                                    acumulado = "";

                                    if (izquierda != null && derecha != null) // Aseguramos que ambos lados de la igualdad existan
                                    {
                                        // 0 -> Representa la posicion de inicio en el heap
                                        // 1 -> Representa el tamaño que ocupa dentro del heap
                                        if (izquierda[1] == derecha[1])
                                        {
                                            asignacionBucle(izquierda, derecha, 1, 1);
                                        }
                                    }
                                }
                                else
                                {
                                    // arreglo[pos{, pos}] = expresion
                                    izquierda = obtenerDatosEstructura(root.ChildNodes[0], entorno);
                                    acumulado = "";
                                }
                            }
                            else
                            {
                                //Es un objeto
                                if (root.ChildNodes[1].ChildNodes.Count == 1 && root.ChildNodes[1].ChildNodes[0].ToString().Equals("ESTRUCTURA"))
                                {
                                    // objeto.atributo{.atributo} = arreglo[pos{, pos}]
                                    // objeto.atributo{.atributo} = objeto.atributo{.atributo}
                                    izquierda = obtenerDatosEstructura(root.ChildNodes[0], entorno);
                                    acumulado = "";
                                    derecha = obtenerDatosEstructura(root.ChildNodes[1].ChildNodes[0], entorno);
                                    acumulado = "";

                                    if (izquierda != null && derecha != null) // Aseguramos que ambos lados de la igualdad existan
                                    {
                                        // 0 -> Representa la posicion de inicio en el heap
                                        // 1 -> Representa el tamaño que ocupa dentro del heap
                                        if (izquierda[1] == derecha[1])
                                        {
                                            asignacionBucle(izquierda, derecha, 1, 1);
                                        }
                                    }
                                }
                                else
                                {
                                    // objeto.atributo{.atributo} = expresion
                                    izquierda = obtenerDatosEstructura(root.ChildNodes[0], entorno);
                                    acumulado = "";
                                    //MessageBox.Show("Posicion inicial: " + izquierda[0] + Environment.NewLine + "Size: " + izquierda[1]);
                                    if (izquierda != null) // Aseguramos que el lado izquierdo exista
                                    {
                                        // 0 -> Representa la posicion de inicio en el heap
                                        // 1 -> Representa el tamaño que ocupa dentro del heap
                                        if (root.ChildNodes[1].ChildNodes[0].ToString().Equals("VARIABLE"))
                                        {
                                            Simbolo sm = entorno.buscar(removerExtras(root.ChildNodes[1].ChildNodes[0].ChildNodes[0].ToString()));
                                            if (!(sm is null))
                                            {
                                                if (sm.direccionHeap == -1)
                                                    derecha[0] = sm.direccionAbsoluta;
                                                else
                                                    derecha[0] = sm.direccionHeap;
                                                derecha[1] = sm.size;
                                                //MessageBox.Show("Posicion Absoluta: " + derecha[0] + Environment.NewLine + "Size: " + derecha[1]);
                                                asignacionBucle(izquierda, derecha, 1, sm.direccionHeap);
                                            }
                                        }
                                        else
                                        {
                                            if (izquierda[1] == 1 || izquierda[1] == 64)
                                            {
                                                //El resultado final de la parte izquierda es de tipo integer, real o boolean
                                                if (!root.ChildNodes[1].ChildNodes[0].ToString().Contains("cadena"))
                                                {
                                                    declaracion = new Declaracion(0, contadorEtiqueta, entornoGlobal);
                                                    declaracion.contadorTemporal = 0;
                                                    str = declaracion.ResolverExpresion(root.ChildNodes[1], entorno, HP) + Environment.NewLine;

                                                    if (declaracion.contadorTemporal > contadorTemporal)
                                                        contadorTemporal = declaracion.contadorTemporal;

                                                    if (!str.StartsWith("T") && !str.StartsWith("S_"))
                                                    {
                                                        contadorTemporal++;
                                                        str = "//Inicio de modificacion de identificador " + Environment.NewLine +
                                                            "T" + contadorTemporal + " = " + str;
                                                    }
                                                    else
                                                    {
                                                        str = "//Inicio de modificacion de identificador " + Environment.NewLine + str;
                                                    }
                                                    traduccion += str;
                                                    if (declaracion.contadorTemporal > contadorTemporal)
                                                    {
                                                        contadorTemporal = declaracion.contadorTemporal;
                                                        traduccion += "HEAP[" + izquierda[0] + "] = T" + contadorTemporal + ";" + Environment.NewLine;
                                                    }
                                                    else
                                                    {
                                                        if (declaracion.contadorTemporal == 0)
                                                            declaracion.contadorTemporal = contadorTemporal;
                                                        traduccion += "HEAP[" + izquierda[0] + "] = T" + declaracion.contadorTemporal + ";" + Environment.NewLine;
                                                    }
                                                    declaracion.contadorTemporal = 0;
                                                }
                                                else
                                                {
                                                    string contenido = removerExtras(root.ChildNodes[1].ChildNodes[0].ToString());
                                                    for (int i = 0; i < contenido.Length; i++)
                                                    {
                                                        if (i == 63)
                                                            break;
                                                        traduccion += "HEAP[" + (izquierda[0] + i) + "] = " + Convert.ToInt32(contenido[i]) + "; //Letra: " + contenido[i] + Environment.NewLine;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //El resultado final de la parte izquierda es de tipo cadena, arreglo u objeto
                                                asignacionBucle(izquierda, derecha, 1, 1);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }
        private void ejecutarLlamada(ParseTreeNode root)
        {
            string nombre = removerExtras(root.ChildNodes[0].ToString());
            Simbolo sim = entorno.buscar(nombre);
            string str = "";
            if (!(sim is null))
            {
                if (sim.tipo == Simbolo.EnumTipo.procedimiento || sim.tipo == Simbolo.EnumTipo.funcion)
                {
                    int suma = 0;
                    Entorno eTmp = entorno;
                    while (eTmp.anterior != null)
                    {
                        eTmp = eTmp.anterior;
                    }
                    foreach (KeyValuePair<string, Simbolo> t in eTmp.tabla)
                    {
                        suma += t.Value.size;
                    }
                    traduccion += "S_HP = HP;" + Environment.NewLine;
                    traduccion += "HP = " + suma + ";" + Environment.NewLine;
                    if (root.ChildNodes.Count > 1)
                    {
                        if (root.ChildNodes[1].ChildNodes[0].ToString().Equals("VARIABLE") || root.ChildNodes[1].ChildNodes.Count > 1 || (root.ChildNodes[1].ChildNodes[0].ChildNodes.Count > 1 && !root.ChildNodes[1].ChildNodes[0].ToString().Equals("ESTRUCTURA")))
                        {
                            Simbolo simX = null;
                            if (root.ChildNodes[1].ChildNodes[0].ToString().Equals("VARIABLE"))
                            {
                                simX = entorno.buscar(removerExtras(root.ChildNodes[1].ChildNodes[0].ChildNodes[0].ToString()));
                                if (simX.tipo != Simbolo.EnumTipo.cadena)
                                    simX = null;
                            }
                            if (simX is null)
                            {
                                declaracion.contadorTemporal = 0;
                                str = declaracion.ResolverExpresion(root.ChildNodes[1], entorno, HP);
                                if (!str.StartsWith("T") && !str.StartsWith("S_"))
                                {
                                    declaracion.contadorTemporal++;
                                    str = "T" + declaracion.contadorTemporal + " = " + str + Environment.NewLine;
                                }
                                if (declaracion.contadorTemporal > contadorTemporal)
                                {
                                    contadorTemporal = declaracion.contadorTemporal;
                                }
                                traduccion += str + Environment.NewLine;
                                //Es un numero o boolean, puede ser un objeto o un arreglo
                            }
                            else
                            {
                                bool tipo = false;
                                if (simX.tipo == Simbolo.EnumTipo.cadena)
                                {
                                    traduccion += "HEAP[(int)HP + 1] = T" + declaracion.contadorTemporal + ";" + Environment.NewLine;
                                }
                                else
                                {
                                    traduccion += "HEAP[(int)HP + 1] = T" + declaracion.contadorTemporal + ";" + Environment.NewLine;
                                }
                            }
                        }
                        else
                        {
                            str = removerExtras(root.ChildNodes[1].ChildNodes[0].ToString());
                            if (str.Equals(""))
                            {
                                str = " ";
                            }
                            //Es un valor
                            traduccion += "HEAP[(int)HP + 1] = " + str + ";" + Environment.NewLine;
                        }

                        ParseTreeNode nodoTemporal = root.ChildNodes[2];
                        while (nodoTemporal.ChildNodes.Count > 0)
                        {
                            if (nodoTemporal.ChildNodes[0].ChildNodes[0].ToString().Equals("VARIABLE") || nodoTemporal.ChildNodes[0].ChildNodes.Count > 1 || (nodoTemporal.ChildNodes[0].ChildNodes[0].ChildNodes.Count > 1 && !nodoTemporal.ChildNodes[0].ChildNodes[0].ToString().Equals("ESTRUCTURA")))
                            {
                                Simbolo simX = null;
                                if (nodoTemporal.ChildNodes[0].ChildNodes[0].ToString().Equals("VARIABLE"))
                                {
                                    simX = entorno.buscar(removerExtras(nodoTemporal.ChildNodes[0].ChildNodes[0].ChildNodes[0].ToString()));
                                    if (simX.tipo != Simbolo.EnumTipo.cadena)
                                        simX = null;
                                }
                                if (simX is null)
                                {
                                    declaracion.contadorTemporal = 0;
                                    str = declaracion.ResolverExpresion(nodoTemporal.ChildNodes[0], entorno, HP);
                                    if (!str.StartsWith("T") && !str.StartsWith("S_"))
                                    {
                                        declaracion.contadorTemporal++;
                                        str = "T" + declaracion.contadorTemporal + " = " + str + Environment.NewLine;
                                    }
                                    if (declaracion.contadorTemporal > contadorTemporal)
                                    {
                                        contadorTemporal = declaracion.contadorTemporal;
                                    }
                                    traduccion += str + Environment.NewLine;
                                    //Es un numero o boolean, puede ser un objeto o un arreglo
                                }
                                else
                                {
                                    bool tipo = false;
                                    if (simX.tipo == Simbolo.EnumTipo.cadena)
                                        tipo = true;
                                    //Es una cadena
                                }
                            }
                            else
                            {
                                str = removerExtras(nodoTemporal.ChildNodes[0].ChildNodes[0].ToString());
                                if (str.Equals(""))
                                {
                                    str = " ";
                                }
                                //Es un valor
                                traduccion += "HEAP[(int)HP + 2] = " + str + ";" + Environment.NewLine;
                            }
                            nodoTemporal = nodoTemporal.ChildNodes[1];
                        }
                    }
                    //traduccion += "HEAP[(int)HP + 1] = 6;" + Environment.NewLine;
                    traduccion += "HP = HP + S_HP;" + Environment.NewLine;
                    traduccion += nombre.ToLower() + "();" + Environment.NewLine;
                }
                else
                {
                    MessageBox.Show("Error se esperaba funcion o procedimiento");
                }
            }
        }
        private void asignacionBucle(int[] izquierda, int[] derecha, int lugarIzquierda, int lugarDerecha)
        {
            int etiquetaInicial;
            int etiquetaVerdadera;
            int etiquetaFalsa;
            contadorTemporal++;
            int tmpContadorIzquierda = contadorTemporal;
            contadorTemporal++;
            int tmpContadorDerecha = contadorTemporal;
            contadorTemporal++;
            contadorEtiqueta++;
            etiquetaInicial = contadorEtiqueta;
            contadorEtiqueta++;
            etiquetaVerdadera = contadorEtiqueta;
            contadorEtiqueta++;
            etiquetaFalsa = contadorEtiqueta;
            contadorEtiqueta++;
            traduccion += "T" + tmpContadorIzquierda + " = " + izquierda[0] + ";" + Environment.NewLine;
            traduccion += "T" + tmpContadorDerecha + " = " + derecha[0] + ";" + Environment.NewLine;
            traduccion += "L" + etiquetaInicial + ":" + Environment.NewLine;
            traduccion += "if (T" + tmpContadorIzquierda + " < " + (izquierda[0] + izquierda[1]) + ") goto L" + etiquetaVerdadera + ";" + Environment.NewLine;
            traduccion += "goto L" + etiquetaFalsa + ";" + Environment.NewLine;
            traduccion += "L" + etiquetaVerdadera + ":" + Environment.NewLine;
            if (lugarDerecha == -1)
                traduccion += "T" + contadorTemporal + " = STACK[(int)T" + tmpContadorDerecha + "];" + Environment.NewLine;
            else
                traduccion += "T" + contadorTemporal + " = HEAP[(int)T" + tmpContadorDerecha + "];" + Environment.NewLine;
            if (lugarIzquierda == -1)
                traduccion += "STACK[(int)T" + tmpContadorIzquierda + "] = T" + contadorTemporal + ";" + Environment.NewLine;
            else
                traduccion += "HEAP[(int)T" + tmpContadorIzquierda + "] = T" + contadorTemporal + ";" + Environment.NewLine;
            traduccion += "T" + tmpContadorIzquierda + " = T" + tmpContadorIzquierda + " + 1;" + Environment.NewLine;
            traduccion += "T" + tmpContadorDerecha + " = T" + tmpContadorDerecha + " + 1;" + Environment.NewLine;
            traduccion += "goto L" + etiquetaInicial + ";" + Environment.NewLine;
            traduccion += "L" + etiquetaFalsa + ":" + Environment.NewLine;
        }
        private void imprimirBucle(int direccionHeap, int size, bool cadena)
        {
            int etiquetaInicial;
            int etiquetaVerdadera;
            int etiquetaFalsa;
            contadorTemporal++;
            traduccion += "T" + contadorTemporal + " = " + direccionHeap + ";" + Environment.NewLine;
            int tempTemo = contadorTemporal;
            contadorTemporal++;
            contadorEtiqueta++;
            etiquetaInicial = contadorEtiqueta;
            traduccion += "L" + etiquetaInicial + ":" + Environment.NewLine;
            contadorEtiqueta++;
            etiquetaVerdadera = contadorEtiqueta;
            contadorEtiqueta++;
            etiquetaFalsa = contadorEtiqueta;
            contadorEtiqueta++;
            traduccion += "if (T" + tempTemo + " < " + (direccionHeap + size) + ") goto L" + etiquetaVerdadera + ";" + Environment.NewLine;
            traduccion += "goto L" + etiquetaFalsa + ";" + Environment.NewLine;
            traduccion += "L" + etiquetaVerdadera + ":" + Environment.NewLine;
            traduccion += "T" + contadorTemporal + " = " + "HEAP[(int)HP + (int)T" + tempTemo + "];" + Environment.NewLine;
            int tempSt = contadorTemporal;
            contadorTemporal++;
            traduccion += "if (T" + tempSt + " != -201700893) goto L" + contadorEtiqueta + ";" + Environment.NewLine;
            int tmpEt = contadorEtiqueta;
            contadorEtiqueta++;
            traduccion += "goto L" + etiquetaFalsa + ";" + Environment.NewLine;
            traduccion += "L" + tmpEt + ":" + Environment.NewLine;
            if (cadena)
                traduccion += "printf(\"%c\", (int)T" + tempSt + ");" + Environment.NewLine;
            else
                traduccion += "printf(\"%f\", T" + tempSt + ");" + Environment.NewLine;
            traduccion += "T" + tempTemo + " = T" + tempTemo + " + 1;" + Environment.NewLine;
            traduccion += "goto L" + etiquetaInicial + ";" + Environment.NewLine;
            traduccion += "L" + etiquetaFalsa + ":" + Environment.NewLine;
        }
        private int[] obtenerDatosEstructura(ParseTreeNode root, Entorno entorno)
        {
            //Recibimos un nodo tipo ESTRUCTURA
            int[] retorno = new int[3];
            if (!root.ChildNodes[0].ToString().Equals("LLAMADA"))
            {
                string nombreVariable = acumulado + removerExtras(root.ChildNodes[0].ToString()); //Nombre de la variable a buscar
                Simbolo variable = entorno.buscar(nombreVariable);
                if (variable is null)
                {
                    int sizTemp = -1;
                    int posTemp = -1;
                    foreach (KeyValuePair<string, Simbolo> pair in entorno.tabla)
                    {
                        if (pair.Key.Contains(nombreVariable))
                        {
                            if (sizTemp == -1)
                            {
                                sizTemp = pair.Value.size;
                                posTemp += pair.Value.direccionAbsoluta + 1;
                            }
                            else
                            {
                                sizTemp += pair.Value.size;
                            }
                            MessageBox.Show("Variable: " + pair.Key + Environment.NewLine + "Posicion absoluta: " + posTemp + Environment.NewLine + "Size: " + sizTemp);
                        }
                    }
                    retorno[0] = posTemp; //Posicion inicial
                    retorno[1] = sizTemp; //Size
                    retorno[2] = 0; //Es cadena 0 -> no | 1 -> si
                    if (posTemp != -1)
                        return retorno;
                    return null;
                }
                if (variable.tipo == Simbolo.EnumTipo.arreglo && root.ChildNodes.Count == 4)
                {
                    //Es un arreglo y ESTRUCTURA representa a un arreglo
                    int limiteInferior, limiteSuperior;
                    limiteInferior = int.Parse(variable.limiteInferior[0].ToString());
                    limiteSuperior = int.Parse(variable.limiteSuperior[0].ToString());

                }
                else if (variable.tipo == Simbolo.EnumTipo.objeto && root.ChildNodes.Count == 2)
                {
                    //Es un objeto y ESTRUCTURA representa a un objeto
                    if (root.ChildNodes[1].ChildNodes.Count > 0)
                    {
                        if (root.ChildNodes[1].ChildNodes[1].ChildNodes.Count > 0)
                        {
                            acumulado = removerExtras(root.ChildNodes[1].ChildNodes[0].ToString()) + ".";
                            return obtenerDatosEstructura(root.ChildNodes[1].ChildNodes[1], variable.atributos);
                        }
                        return obtenerDatosEstructura(root.ChildNodes[1], variable.atributos);
                    }
                    retorno[0] = variable.direccionAbsoluta; //Posicion inicial
                    retorno[1] = variable.size; //Size
                    retorno[2] = 0; //Es cadena 0 -> no | 1 -> si
                }
                else
                {
                    if (root.ChildNodes[1].ChildNodes.Count > 0)
                        return null;
                    if (acumulado != "")
                    {
                        //MessageBox.Show("Posicion absoluta: " + variable.direccionAbsoluta + Environment.NewLine + "Size: " + variable.size);
                    }
                    if (variable.direccionHeap == -1)
                        retorno[0] = variable.direccionAbsoluta; //Posicion inicial
                    else
                        retorno[0] = variable.direccionHeap;
                    retorno[1] = variable.size; //Size
                    if (variable.tipo == Simbolo.EnumTipo.cadena)
                        retorno[2] = 1; //Es cadena 0 -> no | 1 -> si
                    else
                        retorno[2] = 0; //Es cadena 0 -> no | 1 -> si
                }
            }
            else
            {
                MessageBox.Show("ES UNA LLAMADA");
            }
            return retorno;
        }
        private string removerExtras(string token)
        {
            token = token.Replace(" (id)", "");
            token = token.Replace(" (Keyword)", "");
            token = token.Replace(" (Key symbol)", "");
            token = token.Replace(" (entero)", "");
            token = token.Replace(" (cadena)", "");
            token = token.Replace(" (real)", "");
            token = token.Replace(" (boleano)", "");

            return token;
        }
    }
}
