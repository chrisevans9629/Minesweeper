namespace Minesweeper.Test
{
    public static class PascalTestInputs
    {
        public const string PascalProgramWithProcedures =
            "PROGRAM Part12;\r\nVAR\r\n   a : INTEGER;\r\n\r\nPROCEDURE P1;\r\nVAR\r\n   a : REAL;\r\n   k : INTEGER;\r\n\r\n   PROCEDURE P2;\r\n   VAR\r\n      a, z : INTEGER;\r\n   BEGIN {P2}\r\n      z := 777;\r\n   END;  {P2}\r\n\r\nBEGIN {P1}\r\n\r\nEND;  {P1}\r\n\r\nBEGIN {Part12}\r\n   a := 10;\r\nEND.  {Part12}";
    }
}