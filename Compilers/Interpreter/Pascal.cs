using System.Collections.Generic;

namespace Minesweeper.Test
{
    public enum Tokens
    {
        Procedure = ' '
    }
    public class Pascal
    {
        public const string For = "FOR";
        public const string Do = "DO";
        public const string To = "TO";
        public const string If = "IF";
        public const string Then = "THEN";
        public const string Else = "ELSE";
        public const string Function = "FUNCTION";
        public const string Procedure = "PROCEDURE";
        public const string Program = "PROGRAM";
        public const string Begin = "BEGIN";
        public const string End = "END";
        public const string Var = "VAR";
        public const string Real = "REAL";
        public const string Int = "INTEGER";
        public const string Boolean = "BOOLEAN";

        public static IEnumerable<string> BuiltInTypes = new List<string>()
        {
            Real,
            Int,
            Boolean
        };

        public const string BoolConst = "Bool_Const";
        public const string RealConst = "Real_Const";
        public const string IntegerConst = "Integer_Const";
        public const string Id = "ID";
        public const string Assign = "ASSIGN";
        public const string IntDiv = "DIV";


        public const string Equal = "=";
        public const string Dot      = ".";
        public const string Colon    = ":";

        public const string Semi     = ";";
        public const string Comma    = ",";
      
        public const string LParinth = "(";
        public const string RParinth = ")";
        public const string Multi    = "*";
        public const string FloatDiv = "/";
        public const string Add      = "+";
        public const string Sub      = "-";


        public static IEnumerable<string> SingleTokens = new List<string>()
        {
            Dot,
            Colon,
            Semi,
            Comma,
            LParinth,
            RParinth,
            Multi,
            FloatDiv,
            Add,
            Sub,
            Equal
        };

        public static Token CreateToken(string name)
        {
            return new Token(){Name = name};
        }

        public static Dictionary<string,Token> Reservations = new Dictionary<string,Token>()
        {
            { IntDiv,CreateToken(IntDiv)} ,
            { Begin, CreateToken(Begin)},
            {End, CreateToken(End)},
            {Var, CreateToken(Var)},
            {Int, CreateToken(Int)},
            {Real, CreateToken(Real)},
            {Program, CreateToken(Program)},
            {Procedure, CreateToken(Procedure)},
            {Function, CreateToken(Function)},
            {If, CreateToken(If)},
            {Then, CreateToken(Then)},
            {Else, CreateToken(Else)},
            {"TRUE", CreateToken(BoolConst) },
            {"FALSE", CreateToken(BoolConst) },
            {For, CreateToken(For) },
            {To, CreateToken(To) },
            {Do, CreateToken(Do) }
            
        };
    }
}