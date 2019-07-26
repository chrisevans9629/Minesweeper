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
        public PascalException(ErrorCode error, TokenItem token, string message) : base(message)
        {
            
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
}