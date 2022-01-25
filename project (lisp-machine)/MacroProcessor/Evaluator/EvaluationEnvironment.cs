using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LispMachine
{
    public class EvaluationEnvironment
    {
        public Dictionary<string, SExpr> EnvDictionary { get; }
        private EvaluationEnvironment Parent = null;

        public EvaluationEnvironment()
        {
            EnvDictionary = new Dictionary<string, SExpr>();
        }

        public EvaluationEnvironment(EvaluationEnvironment parent)
        {
            EnvDictionary = new Dictionary<string, SExpr>();
            Parent = parent;
        }

        public SExpr Get(string symbol)
        {            
            SExpr ret;
            if(!EnvDictionary.TryGetValue(symbol, out ret))
                if(Parent != null)
                    ret = Parent[symbol];

            return ret;
        }

        public void Set(string symbol, SExpr value)
        {
            EnvDictionary[symbol] = value;
        }

        public SExpr this[string index]
        {
            get => Get(index);
            set => Set(index, value);
        }
    }
}
