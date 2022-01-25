namespace LispMachine
{
    public class SExprObject : SExprValueAtom<object>
    {
        public SExprObject(object obj) : base(obj)
        { }
    }
}
