using System;

namespace LispMachine
{
    public class SExprValueAtom<T> : SExprAbstractValueAtom
    {
        public T Value { get; }

        protected SExprValueAtom(T value)
        {
            Value = value;
        }

        public override void PrintSExpr()
        {
            Console.WriteLine(Value);
        }

        public override string GetText()
        {
            if(Value != null)
                return Value.ToString();
            return "nullptr/void";
        }

        public override object GetCommonValue()
        {
            return Value;
        }
    }
}

