using System;
using System.Text;
using System.IO;

namespace LispMachine
{
    public class StandardEvaluationEnvironment : EvaluationEnvironment
    {
        private string predefined = @"

            (define + (lambda (x y) (LispMachine.StandardLibrary\Plus x y)))
            (define ++ (lambda args (let (++inner (lambda (arglist)
                (
                    if
                    (> (count arglist) 0)
                    (+ (first arglist) (++inner (rest arglist)))
                    0
                )))
                (++inner args)
            )))

            (define - (lambda (x y) (LispMachine.StandardLibrary\Minus x y)))


            (define * (lambda (x y) (LispMachine.StandardLibrary\Multiply x y)))

    
            (define / (lambda (x y) (LispMachine.StandardLibrary\Divide x y)))


            (define > (lambda (x y) (LispMachine.StandardLibrary\More x y)))
            (define < (lambda (x y) (LispMachine.StandardLibrary\More y x)))
            (define <= (lambda (x y) (LispMachine.StandardLibrary\MoreEqual y x)))
            (define >= (lambda (x y) (LispMachine.StandardLibrary\MoreEqual x y)))
            (define = (lambda (x y) (LispMachine.StandardLibrary\Equal x y)))

            (define & (lambda (x y) (LispMachine.StandardLibrary\And x y)))
            (define | (lambda (x y) (LispMachine.StandardLibrary\Or x y)))
            (define ! (lambda (x) (LispMachine.StandardLibrary\Not x)))

            (define println (lambda (x) (LispMachine.StandardLibrary\Println x)))
            (define readln (lambda () (LispMachine.StandardLibrary\Readln)))

            (define count (lambda (x) (LispMachine.StandardLibrary\Count x)))
            (define cons (lambda (x seq) (LispMachine.StandardLibrary\Cons x seq)))
            (define conj (lambda (seq x) (LispMachine.StandardLibrary\Conj seq x)))
            (define first (lambda (seq) (LispMachine.StandardLibrary\First seq)))
            (define second (lambda (seq) (LispMachine.StandardLibrary\Second seq)))
            (define rest (lambda (seq) (LispMachine.StandardLibrary\Rest seq)))
            (define list (lambda args args))
            (define apply (lambda (fn seq) (LispMachine.StandardLibrary\Apply fn seq)))



            (defmacro if (x y z) (quote (cond x y true z)))

            ";

        public StandardEvaluationEnvironment() : base()
        {
        
        }

        public void Init()
        {
            SExprParser parser = new SExprParser(new StringReader(predefined));
            SExpr expr;

            while ((expr = parser.GetSExpression()) != null) {
                var evald = Evaluator.Evaluate(expr, this);
            };
        }
    }
}