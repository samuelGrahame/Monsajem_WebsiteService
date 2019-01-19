using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monsajem_Incs.HttpService.JavaScript
{
    public class Number_js :
        Ref_js
    {
        public Number_js()
        { }
        public Number_js(int Value)
        {
            this.DeclareNewName();
            HowSet(Value.ToString());
        }

        public Number_js(string Value)
        {
            this.DeclareNewName();
            HowSet(Value);
        }

        public static Number_js operator +(Number_js op1, Number_js op2)
        {
            return new Number_js(op1.HowGet + " + " + op2.HowGet);
        }
        public static Number_js operator -(Number_js op1, Number_js op2)
        {
            return new Number_js(op1.HowGet + " - " + op2.HowGet);
        }
        public static Number_js operator *(Number_js op1, Number_js op2)
        {
            return new Number_js(op1.HowGet + " * " + op2.HowGet);
        }
        public static Number_js operator /(Number_js op1, Number_js op2)
        {
            return new Number_js(op1.HowGet + " / " + op2.HowGet);
        }

        public static Bool_js operator ==(Number_js op1, Number_js op2)
        {
            return new Bool_js(op1.HowGet + " == " + op2.HowGet);
        }
        public static Bool_js operator !=(Number_js op1, Number_js op2)
        {
            return new Bool_js(op1.HowGet + " != " + op2.HowGet);
        }
        public static Bool_js operator >(Number_js op1, Number_js op2)
        {
            return new Bool_js(op1.HowGet + " > " + op2.HowGet);
        }
        public static Bool_js operator <(Number_js op1, Number_js op2)
        {
            return new Bool_js(op1.HowGet + " < " + op2.HowGet);
        }
        public static Bool_js operator <=(Number_js op1, Number_js op2)
        {
            return new Bool_js(op1.HowGet + " <= " + op2.HowGet);
        }
        public static Bool_js operator >=(Number_js op1, Number_js op2)
        {
            return new Bool_js(op1.HowGet + " >= " + op2.HowGet);
        }

        public static implicit operator Number_js(int value)
        {
            return new Number_js(value);
        }

        public void Set(Number_js Value)
        {
            HowSet(Value.HowGet);
        }

        internal override string HowDeclare => "0";

        public static implicit operator int(Number_js Value)
        {
            Request.Fetch(Value); return int.Parse(Request.Data.ToString());
        }
    }
}
