namespace Minesweeper.Test
{
    public class Pascal
    {
        public const string Program = "PROGRAM";
        public const string Real = "REAL";
        public const string Colon = "SEMICOLON";
        public const string Begin = "BEGIN";
        public const string End = "END";
        public const string Dot = "DOT";
        public const string Id = "ID";
        public const string Assign = "ASSIGN";
        public const string Semi = "SEMI";
        public const string Comma = "COMMA";
        public const string Var = "VAR";
        public const string Int = "INTEGER";
        public const string RealConst = "Real_Const";

        public static void AddPascalTokens(RegexLexer lex)
        {
            lex.Add(Begin, Begin);
            lex.Add(End, End);
            lex.Add(Dot, ".");
            lex.Add(Assign, ":=");
            lex.Add(Semi, ":");
            lex.Add(Id, "[a-zA-Z]+");

        }
    }
}