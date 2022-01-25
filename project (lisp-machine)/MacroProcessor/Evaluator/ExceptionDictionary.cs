using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace LispMachine
{
    

    public class ExceptionDictionary 
    {

        private OrderedDictionary Dict = new OrderedDictionary();


        private List<SExpr> Get(Type thrownType)
        {            
            
            //todo: сначала обращаемся непосредственно к словарю, нашли - возвращаем
            //иначе проверка на наследование: итерируемся по всем и проверяем, не наследуется ли

            List<SExpr> ret = (List<SExpr>)Dict[thrownType];
            if(ret != null)
                return ret;
            foreach (DictionaryEntry pair in Dict)
            {
                var catchType = (Type)pair.Key;
                var catchValue = (List<SExpr>)pair.Value;

                if(thrownType.IsSubclassOf(catchType))
                    return catchValue;

            }

            //проверка наслеования

            return null;
        }

        private void Set(Type type, List<SExpr> value)
        {
            Dict.Add(type, value);
        }

        public List<SExpr> this[Type index]
        {
            get => Get(index);
            set => Set(index, value);
        }
    }
}