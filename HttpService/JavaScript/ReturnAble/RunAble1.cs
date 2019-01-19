using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monsajem_Incs.HttpService.JavaScript
{
    public class Runable_js<t>:
        Runable_base_js
        where t:Ref_js,new()
    {
        public Runable_js()
        {
            
        }

        public void Run(t v1)
        {
            base.Run(v1);
        }

        public void Set(Action<t> Actions)
        {
            base.Set(1,(c)=>Actions(c[0].Convert<t>()));
        }
    }
}
