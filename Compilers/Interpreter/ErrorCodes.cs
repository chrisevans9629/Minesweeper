using System;

namespace Minesweeper.Test
{
    public enum ErrorCode
    {
        UnexpectedToken,
        IdNotFound,
        DuplicateId
    }

    public class PascalException : Exception
    {
        public ErrorCode Error { get; }
        public TokenItem Token { get; }

        public PascalException(ErrorCode error, TokenItem token, string message) : base(message)
        {
            Error = error;
            Token = token;
        }

        public static string Location(TokenItem current)
        {
            if (current == null) return "";
            return $" at index {current.Index} column {current.Column} line {current.Line}";
        }
    }

    public class LexerException : PascalException
    {
        public LexerException(ErrorCode error, TokenItem token, string message) : base(error, token, message)
        {
        }
    }

    public class ParserException : PascalException
    {
        public ParserException(ErrorCode error, TokenItem token, string message) : base(error, token, message)
        {
        }
    }

    public class SemanticException : PascalException
    {
        public SemanticException(ErrorCode error, TokenItem token, string message) : base(error, token, message)
        {
        }
    }

    public class RuntimeException : PascalException
    {
        public RuntimeException(ErrorCode error, TokenItem token, string message) : base(error, token, message)
        {
        }
    }
}