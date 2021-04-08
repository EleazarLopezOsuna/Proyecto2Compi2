using System;
using System.Collections.Generic;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace Proyecto2_Compiladores2.Analizador
{
    class Gramatica:Grammar
    {
        public Gramatica() : base(caseSensitive: false)
        {
            #region Palabras Reservadas
            var tipo_string = ToTerm("string");
            var tipo_integer = ToTerm("integer");
            var tipo_real = ToTerm("real");
            var tipo_boolean = ToTerm("boolean");
            var object_res = ToTerm("object");
            var array_res = ToTerm("array");
            #endregion

            #region EXPRESIONes Regulares
            RegexBasedTerminal N_entero = new RegexBasedTerminal("entero", @"-?[0-9]+");
            StringLiteral cadena = new StringLiteral("cadena", "\'", StringOptions.AllowsLineBreak);
            RegexBasedTerminal N_real = new RegexBasedTerminal("real", @"-?[0-9]+(\.[0-9]+)");
            RegexBasedTerminal boleano = new RegexBasedTerminal("boleano", "(true|false)");
            IdentifierTerminal id = new IdentifierTerminal("id");
            CommentTerminal comentarioLinea = new CommentTerminal("comentario linea", "//", "\n", "\r\n");
            CommentTerminal comentarioBloque = new CommentTerminal("comentario bloque", "(*", "*)");
            CommentTerminal comentarioBloque2 = new CommentTerminal("comentario bloque", "{", "}");

            base.NonGrammarTerminals.Add(comentarioBloque);
            base.NonGrammarTerminals.Add(comentarioLinea);
            base.NonGrammarTerminals.Add(comentarioBloque2);
            #endregion

            #region Presedencia
            this.RegisterOperators(1, Associativity.Neutral, ">", ">=", "<", "<=", "=", "<>");
            this.RegisterOperators(2, Associativity.Left, "+", "-", "or");
            this.RegisterOperators(3, Associativity.Left, "*", "/", "%", "and");
            this.RegisterOperators(4, Associativity.Right, "not");
            #endregion

            #region No Terminales
            NonTerminal IF_D = new NonTerminal("IF");
            NonTerminal CASE = new NonTerminal("CASE");
            NonTerminal IF_S = new NonTerminal("IF");
            NonTerminal FOR = new NonTerminal("FOR");
            NonTerminal ABAJO = new NonTerminal("ABAJO");
            NonTerminal ARRIBA = new NonTerminal("ARRIBA");
            NonTerminal REPEAT = new NonTerminal("REPEAT");
            NonTerminal WHILE = new NonTerminal("WHILE");
            NonTerminal FUNCION_HEAD = new NonTerminal("FUNCION_HEAD");
            NonTerminal PROCEDIMIENTO_HEAD = new NonTerminal("PROCEDIMIENTO_HEAD");
            NonTerminal OPCION_CASE = new NonTerminal("OPCION_CASE");
            NonTerminal DECLARACION_CAMPOS_TYPE = new NonTerminal("DECLARACION_CAMPOS_TYPE");
            NonTerminal D_CONSTANTE = new NonTerminal("D_CONSTANTE");
            NonTerminal FUNCION = new NonTerminal("FUNCION");
            NonTerminal PROCEDIMIENTO = new NonTerminal("PROCEDIMIENTO");
            NonTerminal SUBPROGRAMA = new NonTerminal("SUBPROGRAMA");
            NonTerminal D_VARIABLE = new NonTerminal("D_VARIABLE");
            NonTerminal ESTRUCTURA = new NonTerminal("ESTRUCTURA");
            NonTerminal CONTROLADOR = new NonTerminal("CONTROLADOR");
            NonTerminal EXPRESION = new NonTerminal("EXPRESION");
            NonTerminal EB = new NonTerminal("EXPRESION");
            NonTerminal EU = new NonTerminal("EXPRESION");
            NonTerminal VALOR = new NonTerminal("VALOR");
            NonTerminal LLAMADA = new NonTerminal("LLAMADA");
            NonTerminal OB = new NonTerminal("OPERADOR");
            NonTerminal OU = new NonTerminal("OPERADOR");
            NonTerminal PA = new NonTerminal("PA");
            NonTerminal PF = new NonTerminal("PF");
            NonTerminal PFVL = new NonTerminal("PFVL"); //Parametros Formales por VaLor
            NonTerminal PFVR = new NonTerminal("PFVR"); //Parametros Formales por VARIABLE
            NonTerminal PROGRAMA = new NonTerminal("PROGRAMA");
            NonTerminal RANGO = new NonTerminal("RANGO");
            NonTerminal R_OPCION_CASE = new NonTerminal("OPCION_CASE");
            NonTerminal R_DECLARACION_CAMPOS_TYPE = new NonTerminal("DECLARACION_CAMPOS_TYPE");
            NonTerminal R_CONSTANTE = new NonTerminal("D_CONSTANTE");
            NonTerminal R_SUBPROGRAMA = new NonTerminal("SUBPROGRAMA");
            NonTerminal R_VARIABLE = new NonTerminal("D_VARIABLE");
            NonTerminal R_TYPE = new NonTerminal("Z_TIPOS");
            NonTerminal R_EXPRESION = new NonTerminal("EXPRESION");
            NonTerminal R_ID = new NonTerminal("R_ID");
            NonTerminal R_OBJETO_CAMPO = new NonTerminal("ESTRUCTURA");
            NonTerminal R_PA = new NonTerminal("PA");
            NonTerminal R_PF = new NonTerminal("PF");
            NonTerminal R_PFVL = new NonTerminal("PFVL");
            NonTerminal R_RANGO = new NonTerminal("RANGO");
            NonTerminal R_SENTENCIA = new NonTerminal("SENTENCIA");
            NonTerminal R_INDICE = new NonTerminal("INDICE");
            NonTerminal SENTENCIA = new NonTerminal("SENTENCIA");
            NonTerminal BEGIN_END = new NonTerminal("BEGIN_END");
            NonTerminal ASIGNACION = new NonTerminal("ASIGNACION");
            NonTerminal T_DATO = new NonTerminal("T_DATO");
            NonTerminal T_ELEMENTAL = new NonTerminal("T_ELEMENTAL");
            NonTerminal T_ESTRUCTURADO = new NonTerminal("T_ESTRUCTURADO");
            NonTerminal T_ORDINAL = new NonTerminal("T_ORDINAL");
            NonTerminal VARIABLE = new NonTerminal("VARIABLE");
            NonTerminal Z_CONSTANTES = new NonTerminal("Z_CONSTANTES");
            NonTerminal Z_VARIABLES = new NonTerminal("Z_VARIABLES");
            NonTerminal Z_DECLARACIONES = new NonTerminal("Z_DECLARACIONES");
            NonTerminal Z_TIPOS = new NonTerminal("Z_TIPOS");
            NonTerminal Z_CONSTANTES_P = new NonTerminal("Z_CONSTANTES");
            NonTerminal Z_SUBPROGRAMAS_P = new NonTerminal("Z_SUBPROGRAMAS");
            NonTerminal Z_VARIABLES_P = new NonTerminal("Z_VARIABLES");
            NonTerminal Z_DECLARACIONES_P = new NonTerminal("Z_DECLARACIONES");
            NonTerminal Z_TIPOS_P = new NonTerminal("Z_TIPOS");
            NonTerminal BREAK = new NonTerminal("BREAK");
            NonTerminal CONTINUE = new NonTerminal("CONTINUE");
            #endregion

            #region Gramatica

            IF_D.Rule = ToTerm("if") + EXPRESION + ToTerm("then") + SENTENCIA + ToTerm("else") + SENTENCIA
                ;

            CASE.Rule = ToTerm("case") + EXPRESION + ToTerm("of") + OPCION_CASE + R_OPCION_CASE + ToTerm("else") + SENTENCIA + ToTerm("end")
                | ToTerm("case") + EXPRESION + ToTerm("of") + OPCION_CASE + R_OPCION_CASE + ToTerm("else") + SENTENCIA + ToTerm(";") + ToTerm("end")
                | ToTerm("case") + EXPRESION + ToTerm("of") + OPCION_CASE + R_OPCION_CASE + ToTerm("end")
                ;

            IF_S.Rule = ToTerm("if") + EXPRESION + ToTerm("then") + SENTENCIA
                ;

            FOR.Rule = ToTerm("for") + ASIGNACION + ARRIBA + SENTENCIA
                | ToTerm("for") + ASIGNACION + ABAJO + SENTENCIA
                ;

            ABAJO.Rule = ToTerm("downto") + EXPRESION + ToTerm("do")
                ;

            ARRIBA.Rule = ToTerm("to") + EXPRESION + ToTerm("do")
                ;

            REPEAT.Rule = ToTerm("repeat") + SENTENCIA + R_SENTENCIA + ToTerm("until") + EXPRESION
                ;

            WHILE.Rule = ToTerm("while") + EXPRESION + ToTerm("do") + SENTENCIA
                ;

            FUNCION_HEAD.Rule = ToTerm("function") + id + ToTerm(":") + T_ELEMENTAL + ToTerm(";")
                | ToTerm("function") + id + ToTerm("(") + PF + R_PF + ToTerm(")") + ToTerm(":") + T_ELEMENTAL + ToTerm(";")
                | ToTerm("function") + id + ToTerm("(") + ToTerm(")") + ToTerm(":") + T_ELEMENTAL + ToTerm(";")
                ;

            PROCEDIMIENTO_HEAD.Rule = ToTerm("procedure") + id + ToTerm(";")
                | ToTerm("procedure") + id + ToTerm("(") + PF + R_PF + ToTerm(")") + ToTerm(";")
                | ToTerm("procedure") + id + ToTerm("(") + ToTerm(")") + ToTerm(";")
                ;

            OPCION_CASE.Rule = RANGO + ToTerm(":") + SENTENCIA
                | Empty
                ;

            DECLARACION_CAMPOS_TYPE.Rule = ToTerm("var") + id + R_ID + ToTerm(":") + T_ELEMENTAL
                | id + R_ID + ToTerm(":") + T_ELEMENTAL
                | ToTerm("const") + id + ToTerm(":") + T_ELEMENTAL + ToTerm("=") + EXPRESION
                | id + ToTerm("=") + EXPRESION
                ;

            D_CONSTANTE.Rule = id + ToTerm("=") + EXPRESION
                | id + ToTerm(":") + T_DATO + ToTerm("=") + EXPRESION
                ;

            FUNCION.Rule = FUNCION_HEAD + Z_DECLARACIONES + BEGIN_END
                ;

            PROCEDIMIENTO.Rule = PROCEDIMIENTO_HEAD + Z_DECLARACIONES + BEGIN_END
                ;

            SUBPROGRAMA.Rule = FUNCION
                | PROCEDIMIENTO
                ;

            D_VARIABLE.Rule = id + R_ID + ToTerm(":") + T_DATO
                | id + ToTerm(":") + T_DATO + ToTerm("=") + EXPRESION
                | id + ToTerm(":") + T_DATO
                ;

            ESTRUCTURA.Rule = id + ToTerm("[") + EXPRESION + R_EXPRESION + ToTerm("]") + R_OBJETO_CAMPO
                | id + R_OBJETO_CAMPO
                | LLAMADA + R_OBJETO_CAMPO
                ;

            CONTROLADOR.Rule = IF_S
                | IF_D
                | CASE
                | WHILE
                | REPEAT
                | FOR
                ;

            EXPRESION.Rule = VALOR
                | VARIABLE
                | EU
                | EB
                | LLAMADA
                | ToTerm("(") + EXPRESION + ToTerm(")")
                ;

            EB.Rule = EXPRESION + OB + EXPRESION
                ;

            EU.Rule = OU + EXPRESION
                ;

            VALOR.Rule = N_entero | N_real | boleano | cadena
                ;

            LLAMADA.Rule = id
                | id + ToTerm("(") + ToTerm(")")
                | id + ToTerm("(") + PA + R_PA + ToTerm(")")
                ;

            OB.Rule = ToTerm("+")
                | ToTerm("-")
                | ToTerm("*")
                | ToTerm("%")
                | ToTerm("/")
                | ToTerm("and")
                | ToTerm("or")
                | "="
                | ToTerm("<>")
                | ToTerm("<")
                | ToTerm(">")
                | ToTerm("<=")
                | ToTerm(">=")
                ;

            OU.Rule = ToTerm("not")
                | ToTerm("-")
                ;

            PA.Rule = EXPRESION
                | VARIABLE
                ;

            PF.Rule = PFVL
                | PFVR
                ;

            PFVL.Rule = id + R_ID + ToTerm(":") + id
                ;

            PFVR.Rule = ToTerm("var") + id + R_ID + ToTerm(":") + id
                ;

            PROGRAMA.Rule = ToTerm("program") + id + ToTerm(";") + Z_DECLARACIONES_P + BEGIN_END + ToTerm(".")
                ;

            RANGO.Rule = EXPRESION
                | EXPRESION + ToTerm("..") + EXPRESION
                | RANGO + R_RANGO
                ;

            R_OPCION_CASE.Rule = ToTerm(";") + OPCION_CASE + R_OPCION_CASE
                | Empty
                ;

            R_DECLARACION_CAMPOS_TYPE.Rule = ToTerm(";") + DECLARACION_CAMPOS_TYPE + R_DECLARACION_CAMPOS_TYPE
                | DECLARACION_CAMPOS_TYPE + R_DECLARACION_CAMPOS_TYPE
                | Empty
                | ToTerm(";")
                ;

            R_CONSTANTE.Rule = ToTerm(";") + D_CONSTANTE + R_CONSTANTE
                | ToTerm(";")
                ;

            R_SUBPROGRAMA.Rule = ToTerm(";") + SUBPROGRAMA + R_SUBPROGRAMA
                | ToTerm(";")
                ;

            R_VARIABLE.Rule = ToTerm(";") + D_VARIABLE + R_VARIABLE
                | ToTerm(";")
                ;

            R_TYPE.Rule = ToTerm(";") + id + ToTerm("=") + T_DATO + R_TYPE
                | ToTerm(";")
                ;

            R_EXPRESION.Rule = ToTerm(",") + EXPRESION + R_EXPRESION
                | Empty
                ;

            R_ID.Rule = ToTerm(",") + id + R_ID
                | Empty
                ;

            R_OBJETO_CAMPO.Rule = ToTerm(".") + id + R_OBJETO_CAMPO
                | ToTerm(".") + id + ToTerm("[") + EXPRESION + R_EXPRESION + ToTerm("]") + R_OBJETO_CAMPO
                | ToTerm(".") + LLAMADA + R_OBJETO_CAMPO
                | Empty
                ;

            R_PA.Rule = ToTerm(",") + PA + R_PA
                | Empty
                ;

            R_PF.Rule = ToTerm(";") + PF + R_PF
                | ToTerm(",") + PF + R_PF
                | Empty
                ;

            R_PFVL.Rule = ToTerm(";") + PFVL + R_PFVL
                | ToTerm(",") + PFVL + R_PFVL
                | Empty
                ;

            R_RANGO.Rule = ToTerm(",") + RANGO + R_RANGO
                | Empty
                ;

            R_SENTENCIA.Rule = ToTerm(";") + SENTENCIA + R_SENTENCIA
                | Empty
                ;

            R_INDICE.Rule = ToTerm(",") + T_ORDINAL + R_INDICE
                | Empty
                ;

            SENTENCIA.Rule = BREAK
                | CONTINUE
                | ASIGNACION
                | BEGIN_END
                | LLAMADA
                | CONTROLADOR
                | Empty
                ;

            BREAK.Rule = ToTerm("break")
                ;

            CONTINUE.Rule = ToTerm("continue")
                ;

            BEGIN_END.Rule = ToTerm("begin") + SENTENCIA + R_SENTENCIA + ToTerm("end")
                | ToTerm("begin") + ToTerm("end")
                ;

            ASIGNACION.Rule = VARIABLE + ":=" + EXPRESION
                ;

            T_DATO.Rule = tipo_integer
                | tipo_boolean
                | tipo_real
                | tipo_string
                | array_res + ToTerm("[") + T_ORDINAL + R_INDICE + ToTerm("]") + ToTerm("of") + T_DATO
                | object_res + DECLARACION_CAMPOS_TYPE + R_DECLARACION_CAMPOS_TYPE + ToTerm("end")
                | id
                ;

            T_ELEMENTAL.Rule = tipo_boolean
                | tipo_integer
                | tipo_string
                | tipo_real
                | id
                ;

            T_ESTRUCTURADO.Rule = array_res + ToTerm("[") + T_ORDINAL + R_INDICE + ToTerm("]") + ToTerm("of") + T_DATO
                | id
                ;

            T_ORDINAL.Rule = EXPRESION
                | EXPRESION + ToTerm("..") + EXPRESION
                ;

            VARIABLE.Rule = id
                | ESTRUCTURA
                ;

            Z_CONSTANTES_P.Rule = Empty
                | ToTerm("const") + D_CONSTANTE + R_CONSTANTE + Z_DECLARACIONES_P
                | D_CONSTANTE + R_CONSTANTE + Z_DECLARACIONES_P
                ;

            Z_SUBPROGRAMAS_P.Rule = Empty
                | SUBPROGRAMA + R_SUBPROGRAMA + Z_DECLARACIONES_P
                ;

            Z_VARIABLES_P.Rule = Empty
                | ToTerm("var") + D_VARIABLE + R_VARIABLE + Z_DECLARACIONES_P
                | D_VARIABLE + R_VARIABLE + Z_DECLARACIONES_P
                ;

            Z_DECLARACIONES_P.Rule = Z_CONSTANTES_P
                | Z_TIPOS_P
                | Z_SUBPROGRAMAS_P
                | Z_VARIABLES_P
                | Empty
                ;

            Z_TIPOS_P.Rule = Empty
                | ToTerm("type") + id + ToTerm("=") + T_DATO + R_TYPE + Z_DECLARACIONES_P
                | id + ToTerm("=") + T_DATO + R_TYPE + Z_DECLARACIONES_P
                ;

            Z_CONSTANTES.Rule = Empty
                | ToTerm("const") + D_CONSTANTE + R_CONSTANTE + Z_DECLARACIONES
                | D_CONSTANTE + R_CONSTANTE + Z_DECLARACIONES
                ;

            Z_VARIABLES.Rule = Empty
                | ToTerm("var") + D_VARIABLE + R_VARIABLE + Z_DECLARACIONES
                | D_VARIABLE + R_VARIABLE + Z_DECLARACIONES
                ;

            Z_DECLARACIONES.Rule = Z_CONSTANTES
                | Z_TIPOS
                | Z_VARIABLES
                | Empty
                ;

            Z_TIPOS.Rule = Empty
                | ToTerm("type") + id + ToTerm("=") + T_DATO + R_TYPE + Z_DECLARACIONES
                | id + ToTerm("=") + T_DATO + R_TYPE + Z_DECLARACIONES
                ;

            #endregion

            #region Preferencias
            this.Root = PROGRAMA;
            this.MarkPunctuation(",", "(", ")", "[", "]", ":", ";", ".", "..", ":=", "program");
            this.MarkPunctuation("type", "program", "var", "const", "begin", "end", "of", "procedure", "function");
            this.MarkPunctuation("case", "do", "else", "for", "repeat", "then", "until", "while", "if", "to", "downto");
            this.MarkTransient(VALOR, SUBPROGRAMA, Z_DECLARACIONES, VARIABLE, T_ELEMENTAL, SENTENCIA, PF, PA, OU, OB, CONTROLADOR);
            #endregion
        }
    }
}
