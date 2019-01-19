using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Text.Encoding;

namespace Monsajem_Incs.HttpService.JavaScript
{
    
    public class String_js:
        Ref_js
    {
        public String_js() :
            base(){}

        public String_js(string Value)
        {
            this.DeclareNewName();
            HowSet("decodeURIComponent('" + DataURL_js.ByteToData(UTF8.GetBytes(Value)) + "')");
        }

        public String_js substring(Number_js Start)
        {
            return new String_js() { HowGet = HowGet + ".substring(" + Start.HowGet + ")" };
        }

        public String_js substring(Number_js Start,Number_js End)
        {
            return new String_js() { HowGet = HowGet + ".substring(" + Start.HowGet + "," + End.HowGet + ")" };
        }

        public String_js HtmlString
        {
            get => new String_js() { HowGet = HowGet + ".replace(/[\u00A0-\u9999<>\\&]/gim,function(i){return'&#'+ i.charCodeAt(0)+';';});" };
        }

        public Number_js length
        {
            get=> new Number_js(HowGet + ".length");
        }

        public static String_js operator +(String_js op1, String_js op2)
        {
            return new String_js() { HowGet = op1.HowGet + " + " + op2.HowGet };
        }
        
        public static Bool_js operator ==(String_js op1, String_js op2)
        {
            return new Bool_js() { HowGet = op1.HowGet + " == " + op2.HowGet };
        }
        public static Bool_js operator !=(String_js op1, String_js op2)
        {
            return new Bool_js() { HowGet = op1.HowGet + " != " + op2.HowGet };
        }

        public static implicit operator String_js(string value)
        {
            return new String_js(value);
        }

        public void Set(String_js Value)
        {
            HowSet(Value.HowGet);
        }

        internal override string HowDeclare => "''";

        public static implicit operator string (String_js Value)
        {
            Request.Fetch(Value); return Request.Data.ToString();
        }
    }
}
