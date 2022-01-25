using System;
using System.Collections.Generic;
using System.Linq;

namespace LispMachine
{
    public class Macro 
    {
        public List<SExprSymbol> MacroArguments { get; }
        public SExpr Body { get; } 
        
        
        public Macro(List<SExprSymbol> args, SExpr body)
        {
            Body = body;
            MacroArguments = args;
        }
    }
 }