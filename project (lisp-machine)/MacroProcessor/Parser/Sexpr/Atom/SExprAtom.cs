using System;

namespace LispMachine
{

    public class SExprAtom : SExpr
    {
        public override void PrintSExpr()
        {
            Console.WriteLine("ATOM");
        }

        public override string GetText()
        {
            return "ATOM";
        }
    }
}
