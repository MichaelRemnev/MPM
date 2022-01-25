using System;
using System.Collections.Generic;
using System.Linq;

namespace LispMachine
{
    public class StandardLibrary
    {
        public static double Plus(double x, double y)
        {
            return x + y;
        }

        public static double Minus(double x, double y)
        {
            return x - y;
        }

        public static double Multiply(double x, double y)
        {
            return x * y;
        }

        public static double Divide(double x, double y)
        {
            return x / y;
        }

        public static bool More(double x, double y)
        {
            return x > y;
        }

        public static bool MoreEqual(double x, double y)
        {
            return x >= y;
        }

        public static bool Equal(double x, double y)
        {
            return x == y;
        }

        public static bool Or (object x, object y)
        {
            var xBool = !((x == null) || (x is bool xTempBool && !xTempBool));
            var yBool = !((y == null) || (y is bool yTempBool && !yTempBool));
            return xBool || yBool;
        }

        public static bool And(object x, object y)
        {
            var xBool = !((x == null) || (x is bool xTempBool && !xTempBool));
            var yBool = !((y == null) || (y is bool yTempBool && !yTempBool));
            return xBool && yBool;
        }

        public static bool Not(object obj)
        {
            return ((obj == null) || (obj is bool objTempBool && !objTempBool));
        }

        public static string Println(object x)
        {
            string ret;
            if(x != null)
                ret =  x.ToString();
            else 
                ret = "nullptr/void";
            Console.WriteLine(ret);
            return ret;
        }

        public static string Readln()
        {
            string ret = Console.ReadLine();
            return ret;
        }

        public static int Count(List<object> list)
        {
            return list.Count;
        }

        public static List<object> Cons(object x, List<object> list)
        {
            list.Insert(0, x);
            return list;
        }

        public static List<object> Conj(List<object> list, object x)
        {
            list.Add(x);
            return list;
        }

        public static object First(List<object> list)
        {
            return list[0];
        }

        public static object Second(List<object> list)
        {
            return list[1];
        }

        public static List<object> Rest(List<object> list)
        {
            if(list.Count == 0)
                return list;
            var temp = new List<object>(list);
            temp.RemoveAt(0);
            return temp;
        }

        public static object Apply(SExprLambda lambda, List<object> list)
        {
            EvaluationEnvironment lambdaEnv = new EvaluationEnvironment(lambda.Environment);    //для замыканий

            var Arguments = list.Select(x => Evaluator.CreateSExprFromObject(x)).ToList();

            if(lambda is SExprVariadicLambda variadicLambda)
            {
                var listSymbol = variadicLambda.ArgListSymbol;
                lambdaEnv[listSymbol.Value] = new SExprList(Arguments);
            }
            else
            {
                var lambdaSymbolArguments = lambda.LambdaArguments;

                if(lambdaSymbolArguments.Count != Arguments.Count)
                    throw new EvaluationException($"Wrong argument count passed, should be {lambdaSymbolArguments.Count} instead of {Arguments.Count} ");

                for (int i = 0; i < Arguments.Count; i++)
                {
                    lambdaEnv[lambdaSymbolArguments[i].Value] = Arguments[i];
                }
            }

            if(lambda.Body.Count == 0)
                return new SExprObject(null);

            SExpr ret = null;
            for (int i = 0; i < lambda.Body.Count; i++)
            {
                var bodyExpr = lambda.Body[i];
                ret = Evaluator.Evaluate(bodyExpr, lambdaEnv);
            }

            return ret;
        }



        
        public static string GetExampleString()
        {
            return "some text";
        }

        public static string GetExampleString(int x)
        {
            return "some text " + x;
        }

        public static void ThrowsException()
        {
            throw new SystemException();
        }

        public static Exception ReturnsException()
        {
            return new SystemException();
        }

    }
}