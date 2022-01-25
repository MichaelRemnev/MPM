
namespace LispMachine
{
    public class SExprBool : SExprValueAtom<bool>
    {
        public SExprBool(bool value) : base(value)
        { }
    }
}
