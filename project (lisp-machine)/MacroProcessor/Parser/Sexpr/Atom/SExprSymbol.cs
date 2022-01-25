using System;

namespace LispMachine
{
    public class SExprSymbol : SExprAtom
    {
        public string Value { get; }

        public SExprSymbol(string value)
        {
            Value = value;
        }

        public override void PrintSExpr()
        {
            Console.WriteLine(Value);
        }

        public override string GetText()
        {
            return Value;
        }
    }
}
