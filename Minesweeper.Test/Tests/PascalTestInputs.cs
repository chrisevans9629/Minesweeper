namespace Minesweeper.Test.Tests
{
    public static class PascalTestInputs
    {

        public const string PascalProgramWithProceduresWithMultipleParameters =
            "program Main;\r\n   var x, y: real;\r\n\r\n   procedure Foo(a, b : INTEGER; c : REAL);\r\n      var y : integer;\r\n   begin\r\n      x := a + x + y;\r\n   end;\r\n\r\nbegin { Main }\r\n\r\nend.  { Main }";

        public const string PascalProgramWithProceduresWithParameters =
            "program Main;\r\n   var x, y: real;\r\n\r\n   procedure Alpha(a : integer);\r\n      var y : integer;\r\n   begin\r\n      x := a + x + y;\r\n   end;\r\n\r\nbegin { Main }\r\n\r\nend.  { Main }";
        public const string PascalProgramWithProcedures =
            "PROGRAM Part12;\r\nVAR\r\n   a : INTEGER;\r\n\r\nPROCEDURE P1;\r\nVAR\r\n   a : REAL;\r\n   k : INTEGER;\r\n\r\n   PROCEDURE P2;\r\n   VAR\r\n      a, z : INTEGER;\r\n   BEGIN {P2}\r\n      z := 777;\r\n   END;  {P2}\r\n\r\nBEGIN {P1}\r\n\r\nEND;  {P1}\r\n\r\nBEGIN {Part12}\r\n   a := 10;\r\nEND.  {Part12}";
    }
}