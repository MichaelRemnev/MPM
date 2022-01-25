using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace LispMachine
{
    public class Evaluator
    {
        private static StandardEvaluationEnvironment GlobalEnv = new StandardEvaluationEnvironment();

        private static bool GlobalEnvInitialized = false;

        private static MacroExpander Expander = new MacroExpander();

        static public SExpr Evaluate(SExpr expr)
        {
            if(!GlobalEnvInitialized)
            {
                GlobalEnv.Init();
                GlobalEnvInitialized = true;
            }

            return Evaluate(expr, GlobalEnv);
        }

     

        static public SExpr Evaluate(SExpr expr, EvaluationEnvironment env)
        {
            while(true)
            {
                if (expr is SExprSymbol symbol)
                {
                    string symbolValue = symbol.Value;

                    var ret = env[symbolValue]; 
                    if(ret == null)
                        throw new EvaluationException($"Symbol {symbolValue} not found");
                    return ret;
                }
                else if (expr is SExprAbstractValueAtom)
                {
                    return expr;
                }
                else if (expr is SExprList list)
                {
                    var args = list.GetArgs();

                    // тут мы рассматриваем различные специальные формы (!),
                    // которые НЕ являются функциями, так как 
                    // параметры в них оцениваются по другому, нежели в случае функций
                    // (простейший пример - if)
                    var head = list[0];

                    if (head is SExprSymbol listHeadSymbol)
                    {
                        var value = listHeadSymbol.Value;
                 
                        if (value == "cond")
                        {
                            if(args.Count % 2 != 0)
                                throw new EvaluationException("There should be an even number of arguments in cond statement");
                            
                            SExpr trueExpr = null;
                            for (int i = 0; i < args.Count; i += 2)
                            {
                                var cond = args[i];
                                var condExpr = args[i + 1];

                                SExpr evaluatedCond = Evaluate(cond, env);

                                if(evaluatedCond is SExprAbstractValueAtom atom && (atom.GetCommonValue() == null || (atom.GetCommonValue() is bool condBool && !condBool)))
                                    continue;
                                trueExpr = condExpr;
                                break;
                            }

                            if(trueExpr == null)
                                return new SExprObject(null);
                            else {
                                expr = trueExpr;
                                continue;
                            }
                        }
                        else if (value == "define")
                        {
                            //синтаксис: define symbol exp
                            if(args.Count != 2)
                                throw new EvaluationException($"Wrong parameter count in definintion, should be 2 instead of {args.Count}");
                            if(args[0] is SExprSymbol defineSymbol)
                            {
                                var ret = Evaluate(args[1], env);
                                GlobalEnv[defineSymbol.Value] = ret;
                                return ret;
                            }
                            else 
                                throw new EvaluationException("First argument in definition should be a symbol!");
                        }
                        else if (value == "defmacro")
                        {
                            //синтаксис: defmacro name (vars) list
                            if(args.Count < 2)
                                throw new EvaluationException($"Wrong parameter count in definition, should be 2 instead of {args.Count}");
                            
                            SExprSymbol defmacroSymbol = null;
                            if(args[0] is SExprSymbol defineSymbol)
                                defmacroSymbol = defineSymbol;
                            else
                                throw new EvaluationException("First argument in defmacro is not a symbol!");

                            if (args[1] is SExprList macroArguments)
                            {
                                List<SExprSymbol> symbolArguments = new List<SExprSymbol>();
                                foreach (var arg in macroArguments.GetElements())
                                {
                                    if(arg is SExprSymbol symbolArg) 
                                        symbolArguments.Add(symbolArg);
                                    else
                                        throw new EvaluationException("Parameter in macro definition is not symbolic");
                                } 

                                args.RemoveAt(0);
                                args.RemoveAt(0);
                                //сюда мы пришли с некоторым окружением, возможно неглобальным. Но мы ничего с ним не делаем, мы его теряем
                                Expander[defmacroSymbol.Value] = new Macro(symbolArguments, Evaluate(args[0], env));
                                return new SExprString($"Macro {defmacroSymbol.Value} evaluated");
                            }
                            else
                                throw new EvaluationException("Macro definition should have a list of symbol parameters");
                            
                        }
                        else if (value == "macroexpand") //раскрывает только верхний уровень - вроде так канонически
                        {
                            if(args.Count != 1)
                                throw new EvaluationException($"Wrong parameter count in macroexpand, should be 1 instead of {args.Count}");
                            var form = args[0];
                            var evaluatedForm = Evaluate(form, env);
            

                            return Expander.Expand(evaluatedForm);

                        }
                        else if (value == "lambda")
                        {
                            //синтаксис: (lambda (symbol...) body)
                            //пример (lambda (r) (* r (* r r)))

                            if (args.Count < 1)
                                throw new EvaluationException("No arg list in lambda!");

                            if (args[0] is SExprList lambdaArguments)
                            {
                                List<SExprSymbol> symbolArguments = new List<SExprSymbol>();
                                foreach (var arg in lambdaArguments.GetElements())
                                {
                                    if(arg is SExprSymbol symbolArg) 
                                        symbolArguments.Add(symbolArg);
                                    else
                                        throw new EvaluationException("Parameter in lambda definition is not symbolic");
                                } 

                                args.RemoveAt(0);
                                //сюда мы пришли с некоторым окружением, возможно неглобальным. Но мы ничего с ним не делаем, мы его теряем
                                var body = args;

                                return new SExprLambda(symbolArguments, body, env);

                            }
                            else if (args[0] is SExprSymbol symbolForList)
                            {
                                //подается на вход произваольное число аргументов, и все они помещаются в список
                                args.RemoveAt(0);
                                var body = args;

                                return new SExprVariadicLambda(symbolForList, body, env);
                            }

                            else
                                throw new EvaluationException("Lambda definition should have a list of symbol parameters or a symbol (for variadic lambdas)");
                        }
                        else if (value == "let")
                        {
                            if (args[0] is SExprList letBindings)
                            {
                                var letBindingsList = letBindings.GetElements();
                                if(letBindingsList.Count % 2 != 0)
                                    throw new EvaluationException("There should be an even number of elements in list of bindings");

                                var letEnvironment = new EvaluationEnvironment(env); 
                                for (int i = 0; i < letBindingsList.Count; i+=2)
                                {
                                    var symbolIndex = i;
                                    var valueIndex = i + 1;
                                    if(letBindingsList[symbolIndex] is SExprSymbol symbolArg) 
                                    {
                                        letEnvironment[symbolArg.Value] = Evaluate(letBindingsList[valueIndex], letEnvironment);
                                    }
                                    else
                                        throw new EvaluationException($"Parameter №{i} in let is not a symbol");
                                }

                                args.RemoveAt(0);
                                var body = args;
                                SExpr ret = null;

                                if(body.Count == 0)
                                    return new SExprObject(null);

                                for (int i = 0; i < body.Count - 1; i++)
                                {
                                    var bodyExpr = body[i];
                                    ret = Evaluate(bodyExpr, letEnvironment);
                                }

                                expr = body[body.Count - 1];
                                env = letEnvironment;
                                continue;

                            }
                            else
                                throw new EvaluationException("Second argument of let should be a list of bindings");
                        }
                        else if (value == "quote")
                        {
                            if(args.Count != 1)
                                throw new EvaluationException($"Wrong parameter count in quotation, should be 1 instead of {args.Count}");
                            return args[0];
                        }
                        else if (value == "throw")
                        {

                            if(args.Count != 1)
                                throw new EvaluationException($"Throw should only have one argument, not {args.Count}");

                            var throwExpr = args[0];
                            var evaluatedThrowExpr = Evaluate(throwExpr, env);
                            if(evaluatedThrowExpr is SExprAbstractValueAtom valueExpr && valueExpr.GetCommonValue() is Exception e)
                                throw e;
                            throw new EvaluationException("Argument of throw is not an exception!");
                        }
                        else if (value == "try")
                        {
                            int i;
                            List<SExpr> tryBody = new List<SExpr>();
                            for (i = 0; i < args.Count; i++)
                            {
                                var tryExpr = args[i];
                                if (tryExpr is SExprList tryList && tryList[0] is SExprSymbol trySymbol
                                    && trySymbol.Value == "catch")
                                    break;
                                else
                                    tryBody.Add(tryExpr);
                            }
                            //после этого должны быть только catch (может, 0?) и, опционально, finally  
                            //словарь для типов Exception, здесь список имеет особый вид: первый элемент - SExprSymbol - имя переменной (для нее создадим контекст внутренний)
                            ExceptionDictionary exceptionDict = new ExceptionDictionary();
                            List<SExpr> finallyBody = new List<SExpr>();
                            for (; i < args.Count; i++)
                            {
                                var tryExpr = args[i];
                                if (tryExpr is SExprList tryList && tryList[0] is SExprSymbol trySymbol)
                                {
                                    if (trySymbol.Value == "catch")
                                    {
                                        var catchArgs = tryList.GetArgs();
                                        if (catchArgs.Count < 2)
                                            throw new EvaluationException("Not enough elements in catch clause");
                                        
                                        var exceptionClassName = catchArgs[0].GetText();
                                        var exceptionType = Type.GetType(exceptionClassName);
                                        if (exceptionType == null)
                                            throw new EvaluationException("Exception type not found. Perhaps you should specify the namespace.");

                                        bool isExceptionType = exceptionType.IsSubclassOf(typeof(Exception)) || exceptionType == typeof(Exception);

                                        if(!isExceptionType)
                                            throw new EvaluationException($"{exceptionClassName} is not an exception type!");

                                        if(!(catchArgs[1] is SExprSymbol))
                                            throw new EvaluationException("Exception name in catch clause is not a symbol");

                                        catchArgs.RemoveAt(0);
                                        var catchBody = catchArgs;
                                        exceptionDict[exceptionType] = catchBody;
                                        continue;
                                    }
                                    else if (trySymbol.Value == "finally")
                                    {
                                        finallyBody = tryList.GetArgs();
                                        if(i != args.Count - 1)
                                            throw new EvaluationException("There shouldn't be any other clauses after finally in try-catch");
                                        break; //после finally ничего нет
                                    }
                                    else
                                        throw new EvaluationException("After first catch, only catch and finally clauses are allowed in try-catch");
                                }
                                else
                                    throw new EvaluationException("After first catch, only catch and finally clauses are allowed in try-catch");
                            }

                            try
                            {
                                if(tryBody.Count == 0)
                                    return new SExprObject(null);

                                SExpr ret = null;
                                foreach (var bodyExpr in tryBody)
                                {
                                    ret = Evaluate(bodyExpr, env);
                                }

                                return ret;
                            }
                            catch (Exception e)
                            {                                
                                //если бросали из внешнего метода, то там System.Reflection.TargetInvocationException, но это решается в коде для вызова методов C#

                                var exceptionType = e.GetType();
                                
                                List<SExpr> bodyForExceptionType = exceptionDict[exceptionType];
                                if(bodyForExceptionType != null)
                                {
                                    var exceptionSymbol = (SExprSymbol)bodyForExceptionType[0];
                                    bodyForExceptionType.RemoveAt(0);

                                    var catchEnvironment = new EvaluationEnvironment(env); 
                                    catchEnvironment[exceptionSymbol.Value] = new SExprObject(e);
                                
                                    if(bodyForExceptionType.Count == 0)
                                        return new SExprObject(e);
                                    
                                    SExpr ret = null;
                                    for (i = 0; i < bodyForExceptionType.Count; i++)
                                    {
                                        var bodyExpr = bodyForExceptionType[i];
                                        ret = Evaluate(bodyExpr, catchEnvironment);
                                    }

                                    return ret; //goes to finally
                                }
                                else
                                    throw e;

                            }
                            finally
                            {

                                foreach (var finallyExpr in finallyBody)
                                    Evaluate(finallyExpr, env);
                                //оцениваются (вдруг сайд эффекты), но не возвращаются

                            }
                        }
                        else if (value == "new")
                        {

                            string className = null;
                            if(args[0] is SExprSymbol classNameSymbol)
                                className = classNameSymbol.Value;
                            else
                                throw new EvaluationException("First argument of new is not a symbol!");
                            var type = Type.GetType(className);
                            if (type == null)
                                throw new EvaluationException("No such class found, maybe you should use the full name with namespace?");
                            
                            var arguments = new List<object>();
                            args.RemoveAt(0);
                            foreach(var arg in args)
                            {
                                var evaluatedArg = Evaluate(arg, env);
                                if(evaluatedArg is SExprLambda)
                                    throw new EvaluationException("Wrong parameter in native call, lambdas can't be passed");
                                arguments.Add(CreateObjectFromSExpr(evaluatedArg));
                            } 

                            var constructor = type.GetConstructor(arguments.Select (x => x.GetType()).ToArray());
                            if(constructor == null)
                                throw new EvaluationException("No constructor with such arguments found");

                            try {
                                var instance = constructor.Invoke(arguments.ToArray()); 
                                return CreateSExprFromObject(instance);  
                            }
                            catch (System.Reflection.TargetInvocationException e) {
                                throw e.InnerException;
                            }
                        }
                        else if (value[0] == '.')
                        {
                            if(args.Count < 1)
                                throw new EvaluationException($"Wrong parameter count in native method call: an instance should be provided after method name");
                            var methodName = value.Substring(1);
                            var instance = args[0]; 
                            var evaluatedInstance = Evaluate(instance, env);

                            args.RemoveAt(0);
                            var arguments = new List<object>();
                            foreach(var arg in args)
                            {
                                var evaluatedArg = Evaluate(arg, env);
                                if(evaluatedArg is SExprLambda)
                                    throw new EvaluationException("Wrong parameter in native call, lambdas can't be passed");
                                arguments.Add(CreateObjectFromSExpr(evaluatedArg));
                            }

                            var evaluatedInstanceObject = CreateObjectFromSExpr(evaluatedInstance);
                            var type = evaluatedInstanceObject.GetType();
                            var method = type.GetMethod(methodName, arguments.Select (x => x.GetType()).ToArray());
                            try {
                                var returnedObj = method.Invoke(evaluatedInstanceObject, arguments.ToArray()); 
                                return CreateSExprFromObject(returnedObj);     
                            }
                            catch (System.Reflection.TargetInvocationException e) {
                                throw e.InnerException;
                            }
                    
                        }
                        else if (value.Contains('\\'))
                        {
                            var splat = value.Split('\\');
                            var className = splat[0];
                            var methodName = splat[1];
                            var arguments = new List<object>();
                            foreach(var arg in args)
                            {
                                var evaluatedArg = Evaluate(arg, env);
                                arguments.Add(CreateObjectFromSExpr(evaluatedArg));
                            } 

                            var method = Type.GetType(className).GetMethod(methodName, arguments.Select(x => x.GetType()).ToArray());
                            try {
                                var returnedObj = method.Invoke(null, arguments.ToArray());
                                return CreateSExprFromObject(returnedObj);  
                            }
                            catch (System.Reflection.TargetInvocationException e) {
                                throw e.InnerException;
                            }
                        
                        }
                    }


                    if (head is SExprSymbol headSymbol && Expander[headSymbol.Value] != null)
                    {
                        var ret = Evaluate(Expander.Expand(expr), env);
                        return ret;
                    }

                    var Arguments = args.Select(x => Evaluator.Evaluate(x, env)).ToList(); 
                                      
                    var evaluatedHead = Evaluator.Evaluate(head, env);
            
                    if(evaluatedHead is SExprLambda lambda)
                    {
                        EvaluationEnvironment lambdaEnv = new EvaluationEnvironment(lambda.Environment);    //для замыканий

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

                        for (int i = 0; i < lambda.Body.Count - 1; i++)
                        {
                            var bodyExpr = lambda.Body[i];
                            Evaluator.Evaluate(bodyExpr, lambdaEnv);
                        }

                        var lastExpr = lambda.Body[lambda.Body.Count - 1];
                        expr = lastExpr;
                        env = lambdaEnv;
                        continue;
                    }

                    throw new EvaluationException("Not built-in function or lambda");

                }
            }

            return null; //unreachable
        }

        public static SExpr CreateSExprFromObject(object obj)
        {
            if (!(obj is string) && obj is IEnumerable enumerable)
            {
                SExprList ret = new SExprList();
                foreach (var elem in enumerable)
                    ret.AddSExprToList(CreateSExprFromObject(elem));
                return ret;
            }
            if(obj is SExpr expr)
                return expr;
            return new SExprObject(obj);
        }

        private static object CreateObjectFromSExpr(SExpr expr)
        {
            if (expr is SExprList list)
                return list.GetElements().Select(x => CreateObjectFromSExpr(x)).Cast<object>().ToList(); 
            if (expr is SExprAbstractValueAtom value)
                return value.GetCommonValue();
                //дополнительно можно лоцировать встроенные типы типа int и приводить
            if(expr is SExprLambda lambda)
                return lambda;
            throw new EvaluationException("Wrong argument in native call");        
        }


        public static SExpr Evaluate(string str)
        {
            var parser = new SExprParser(new StringReader(str));
            SExpr expr, evald = null;

            while ((expr = parser.GetSExpression()) != null) {
                evald = Evaluate(expr);
            };

            return evald;
        }
    }

    public class EvaluationException : Exception
    {
        public EvaluationException() { }

        public EvaluationException(string message) : base(message) { }

        public EvaluationException(string message, Exception innerException) : base(message, innerException) { }
    }
}