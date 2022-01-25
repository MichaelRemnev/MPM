using System;
using System.Collections.Generic;
using System.Linq;

namespace LispMachine
{
    class FunctionCall
    {
        private SExpr Function;
        private List<SExpr> Arguments;

        //function и arguments - не-evaluated
        public FunctionCall(SExpr function, List<SExpr> arguments)
        {
            Function = function;
            Arguments = arguments;
        }

        public SExpr Evaluate(EvaluationEnvironment env)
        {
            /*Arguments = Arguments.Select(x => Evaluator.Evaluate(x, env)).ToList();

            if (Function is SExprSymbol symbol)
            {
                string operation = symbol.Value;
                switch (operation)
                {
                    //изначально операции были релизованы тут, но такой способ на давал им быть объектами первого класса
                    //case "+":
                    //    return Sum();   
                    default:
                        break;
                }

            }
            */


            //если же ничего во встроенных функциях не нашли (default), или это вообще сразу лямбда
            //то это "лисповская" функция
            //включая встроенные - в глобальное (корневое) окружение при его создании в конструкторе
            //тогда сначала оцениваем нашу функцию (вдруг это лямбда), потом ищем в окружении такую функцию
            //var function = Evaluate(head, env);
            //вызов функции реализуется через замыкание - добавляем все параметры в контекст
            //если мы хотим реализовать переопределение встроенных функций, надо переместить код в начало
            
            /*var evaluatedHead = Evaluator.Evaluate(Function, env);
            
            if(evaluatedHead is SExprLambda lambda)
            {
                var lambdaSymbolArguments = lambda.LambdaArguments;

                if(lambdaSymbolArguments.Count != Arguments.Count)
                    throw new EvaluationException("Wrong argument count passed");

                //EvaluationEnvironment lambdaEnv = new EvaluationEnvironment(env);
                EvaluationEnvironment lambdaEnv = new EvaluationEnvironment(lambda.Environment);    //для замыканий

                for (int i = 0; i < Arguments.Count; i++)
                {
                    lambdaEnv[lambdaSymbolArguments[i].Value] = Arguments[i];
                }



                SExpr ret = null;
                foreach (var bodyExpr in lambda.Body)
                {
                    ret = Evaluator.Evaluate(bodyExpr, lambdaEnv);
                }

                return ret;
            }

            throw new EvaluationException("Not built-in function or lambda");
            */ 
            return null;
        }


        private SExpr Sum()
        {
            double sum = 0;
            foreach (SExprAbstractValueAtom arg in Arguments)
            {
                sum += Convert.ToDouble(arg.GetCommonValue());
            }
            return new SExprFloat(sum);
        }
    }
}
