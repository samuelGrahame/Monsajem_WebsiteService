using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monsajem_Incs.HttpService.JavaScript
{
    public class IF_Js
    {
        public void ElseIf(Bool_js Condition, Action Action)
        {
            js.SendJS(" else if (" + Condition.HowGet + ")");
            
        }
        public void Else(Action Action)
        {
            js.SendJS("else{");
            
        }
    }
}
