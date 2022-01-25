namespace LispMachine
{
    public class SExprString : SExprValueAtom<string>
    {
        public SExprString(string str) : base(str)
        { }

        public override string GetText()
        {
            if(Value != null)
                return '"' + Value + '"';
            return "nullptr/void";
        }
    }
}
