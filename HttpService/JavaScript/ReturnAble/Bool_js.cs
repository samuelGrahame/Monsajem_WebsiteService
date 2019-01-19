using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monsajem_Incs.HttpService.JavaScript
{
    public class Bool_js :
        Ref_js
    {
        public Bool_js()
        { }
        public Bool_js(string Value)
        { HowGet = Value; }

        public Bool_js(bool Value)
        {
            if (Value == true)
                HowGet = "true";
            else
                HowGet = "false";
        }

        public static implicit operator Bool_js(bool Value)
        {
            return new Bool_js(Value);
        }

        public static Bool_js operator &(Bool_js op1, Bool_js op2)
        {
            return new Bool_js(op1.HowGet + " & " + op2.HowGet);
        }

        public static Bool_js operator |(Bool_js op1, Bool_js op2)
        {
            return new Bool_js(op1.HowGet + " | " + op2.HowGet);
        }

        public void Set(Bool_js Value)
        {
            HowSet(Value.HowGet);
        }

        internal override string HowDeclare => "false";

        public static implicit operator bool(Bool_js Value)
        {
            Request.Fetch(Value);
            return bool.Parse(Request.Data.ToString());
        }
    }
}