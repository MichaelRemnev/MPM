using System;
using System.IO;

namespace LispMachine
{
    public class SExprParser
    {
        private Lexer lexer;
        private Lexeme currentLexeme;
        public static void TestMethod()
        {
            Console.WriteLine("Parser message");
        }

        public SExprParser(TextReader reader)
        {
            lexer = new Lexer(reader);
            
        }

        /// <summary>
        /// The method reads and returns next S-Expression from the reader passed to the constructor
        /// </summary>
        /// <returns>SExpr - next SExpression</returns>
        public SExpr GetSExpression()
        {
            currentLexeme = lexer.GetLexeme(); // first token === lexeme

            return GetSExpressionRecursive();
        }

        private SExpr GetSExpressionRecursive()
        {

            var lexemeType = currentLexeme.Type;
            if (lexemeType == LexemeType.EOF)
                return null;

            if (lexemeType == LexemeType.RBRACE) 
                throw new ParserException("Unexpected right brace!");

            if (lexemeType == LexemeType.LBRACE)
            {
                SExprList list = new SExprList();

                while ((currentLexeme = lexer.GetLexeme()).Type != LexemeType.RBRACE)
                {
                    if (currentLexeme.Type == LexemeType.EOF)
                        throw new ParserException("Not enough right braces!");
                    list.AddSExprToList(GetSExpressionRecursive());
                }

                return list;
            }

            //Атомарное S-выражение
            string text = currentLexeme.Text;
            if(text[0] == '"' && text[text.Length - 1] == '"')
                return new SExprString(text.Substring(1, text.Length - 2));

            int intRes;
            if (int.TryParse(text, out intRes))
                return new SExprInt(intRes);

            double doubleRes;
            if (double.TryParse(text, out doubleRes))
                return new SExprFloat(doubleRes);

            bool boolRes;
            if (bool.TryParse(text, out boolRes))
                return new SExprBool(boolRes);

            return new SExprSymbol(text);
        }
    }

    public class ParserException : Exception
    {
        public ParserException() { }

        public ParserException(string message) : base(message) { }

        public ParserException(string message, Exception innerException) : base(message, innerException) { }
    }
}

