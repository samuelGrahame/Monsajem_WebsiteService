using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Monsajem_Incs.HttpService.JavaScript
{
    public class Runable_js :
        Runable_base_js
    {
        public Runable_js()
        {

        }

        internal override string HowDeclare => "function(){}";

        public void Run()
        {
            base.Run();
        }

        public new void WaitForHandle(Action<Action> WhenHandled=null,Action HowHandle=null)
        {
            base.WaitForHandle(WhenHandled, HowHandle);
        }

        public new void Set(Action Actions)
        {
            base.Set(Actions);
        }

        public static implicit operator Runable_js(Action Value)
        {
            var Result = new Runable_js();
            Result.Set(Value);
            return Result;
        }
    }
}
