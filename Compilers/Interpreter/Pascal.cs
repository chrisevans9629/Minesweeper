using System.Collections.Generic;

namespace Minesweeper.Test
{
    public enum Tokens
    {
        Procedure = ' '
    }
    public class Pascal
    {
        public const string Function = "FUNCTION";
        public const string Procedure = "PROCEDURE";
        public const string Program = "PROGRAM";
        public const string Real = "REAL";
        public const string Begin = "BEGIN";
        public const string End = "END";
        public const string Var = "VAR";
        public const string Int = "INTEGER";
        public const string RealConst = "Real_Const";
        public const string Num = "NUM";
        public const string Id = "ID";
        public const string Assign = "ASSIGN";
        public const string IntDiv = "DIV";

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
            Sub
        };

        public static IEnumerable<string> Reservations = new List<string>()
        {
            IntDiv,
            Begin,
            End,
            Var,
            Int,
            Real,
            Program,
            Procedure,
            Function
        };
    }
}