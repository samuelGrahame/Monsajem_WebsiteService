using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monsajem_Incs.HttpService.JavaScript
{
    public class Worker_js:
        Ref_js
    {
        public string Name;
        public Worker_js(string Name)
        {
            this.Name = Name+".";
        }

        public Runable_js onmessage
        {
            get => new Runable_js { Name = Name + "onmessage" };
            set => new Runable_js { Name = Name + "onmessage" }.Set(value);
        }

        public void postMessage()
        {
            js.SendJS(Name + "postMessage(null);");
        }
        public void postMessage(String_js value)
        {
            js.SendJS(Name + "postMessage(" +value.HowGet + ");");
        }
    }

}
