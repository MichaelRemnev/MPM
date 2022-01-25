using System;
using System.Collections.Generic;
using System.Linq;

namespace LispMachine
{
    public class MacroExpander
    {
        private static Dictionary<string, Macro> MacroTable = new Dictionary<string, Macro>();


        private Macro Get(string symbol)
        {            
            Macro ret;
            MacroTable.TryGetValue(symbol, out ret);
            return ret;
        }

        private void Set(string symbol, Macro value)
        {
            MacroTable[symbol] = value;
        }

        public Macro this[string index]
        {
            get => Get(index);
            set => Set(index, value);
        }

        public SExpr Expand(SExpr expr)
        {
            if (expr is SExprList list)     
            {
                var args = list.GetArgs();
                var head = list[0];
                if (head is SExprSymbol listHeadSymbol)
                {
                    var value = listHeadSymbol.Value;

                    Macro macro;
                    if((macro = this[value]) != null)
                        return ExpandMacro(macro, args);
                }       

                return expr;
            }
            else
            {
                return expr;
            }
        }

        public SExpr ExpandMacro(Macro macro, List<SExpr> args)
        {
            var expr = macro.Body;
            if(args.Count != macro.MacroArguments.Count)
                throw new MacroException($"Wrong parameter count: {args.Count} instead of {macro.MacroArguments.Count}");
            
            
            var expanded = ExpandExprRec(expr, macro.MacroArguments, args);
            return expanded;
        }

        private SExpr ExpandExprRec(SExpr expr, List<SExprSymbol> argNames, List<SExpr> macroArgs)
        {
            

            if (expr is SExprList list)     
            {
                list = new SExprList(list.GetElements());
                var args = list.GetArgs();
                var head = list[0];

                if (head is SExprSymbol listHeadSymbol)
                {
                    var value = listHeadSymbol.Value;

                    if(value == "tilde")
                    {
                        if(args.Count != 1)
                           throw new MacroException($"Wrong parameter count after tilde: {args.Count} instead of 1"); 
                        var arg = args[0];

                    }

                    for (int i = 1; i < list.GetElements().Count; i++)
                    {
                        var bodyExpr = list[i];
                        var expanded = ExpandExprRec(bodyExpr, argNames, macroArgs);
                        list[i] = expanded;
                    }
                    return list;
                }       

                return expr;
            }
            else if (expr is SExprSymbol symbol)
            {
                var value = symbol.Value;
                for (int i = 0; i < argNames.Count; i++)
                {
                    var argName = argNames[i];
                    if(argName.Value == value)
                        return macroArgs[i];
                }
                return expr;
            }

            return expr;
        }
    }

    public class MacroException : Exception
    {
        public MacroException() { }

        public MacroException(string message) : base(message) { }

        public MacroException(string message, Exception innerException) : base(message, innerException) { }
    }

}