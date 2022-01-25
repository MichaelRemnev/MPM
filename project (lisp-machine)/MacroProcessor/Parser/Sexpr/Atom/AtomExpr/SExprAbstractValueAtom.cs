using System;


namespace LispMachine
{
    public abstract class SExprAbstractValueAtom : SExprAtom
    {
        abstract public object GetCommonValue();

    }
}
