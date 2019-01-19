using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monsajem_Incs.HttpService.JavaScript
{
    public class Array_js<t> :
        Ref_js
        where t : Ref_js, new()
    {
        public Array_js()
        { }
        public int Length;

        public Array_js(IEnumerable<t> obj)
        {
            this.HowGet = "[" + GetItems(obj) + "]";
            this.Length = obj.Count();
        }

        private string GetItems(IEnumerable<t> obj)
        {
            var Script = "";
            var Length = obj.Count();
            var Enumer = obj.GetEnumerator();
            Enumer.MoveNext();
            for (int i = 0; i < Length - 1; i++)
            {
                Script += Enumer.Current.HowGet + ",";
                Enumer.MoveNext();
            }
            if (Length > 0)
                Script += Enumer.Current;
            return Script;
        }

        public t this[Number_js Position]
        {
            get
            {
                var Result = new t();
                Result.Name = this.HowGet + "[" + Position.HowGet + "]";
                return Result;
            }
            set
            {
                this[Position].Set(value);
            }
        }

        public void clear()
        {
            js.SendJS(this.HowGet + "=[];");
        }

        public void push(IEnumerable<t> obj)
        {
            js.SendJS(this.HowGet + ".push("+GetItems(obj)+");");
            this.Length += obj.Count();
        }

        public void push(t obj)
        {
            js.SendJS(this.HowGet + ".push(" + obj.HowGet + ");");
            this.Length += 1;
        }

        public t Pop()
        {
            var Result = new t();
            Result.Name=this.HowGet + ".pop()";
            this.Length -= 1;
            return Result;
        }

        public void ForEach(Action<t> Actions)
        {
            var Value = new t();
            Value.Name = js.GetNewName();
            js.SendJS(HowGet + ".forEach(function(" + Value.HowGet + "){");
            Actions(Value);
            js.SendJS("});");
        }

        internal override string HowDeclare => "[]";
    }
}
