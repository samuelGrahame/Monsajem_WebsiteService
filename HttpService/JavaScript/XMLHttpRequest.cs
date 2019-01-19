using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Monsajem_Incs.HttpService.JavaScript.js;

namespace Monsajem_Incs.HttpService.JavaScript
{

    public class XMLHttpRequest_js:
        Ref_js
    {
        public XMLHttpRequest_js()
        { }

        public void Open_Get(String_js path,bool Async)
        {
            js.SendJS(HowGet + ".open('Get', " + path.HowGet + ","+ new Bool_js(Async).HowGet + ");");
        }
        public void Open_Post(String_js path,bool Async)
        {
            js.SendJS(
                HowGet + ".open('Post', " + path.HowGet + "," + new Bool_js(Async).HowGet + ");");
            //".setRequestHeader('Content-type', 'application/x-www-form-urlencoded');"
        }

        public void Send(Ref_js Data)
        {
            js.SendJS(HowGet + ".send(" + Data.HowGet + ");");
        }

        public void Send()
        {
            js.SendJS(HowGet + ".send();");
        }

        public Runable_js onreadystatechange
        {
            get=> GetInner<Runable_js>("onreadystatechange");
            set=> onreadystatechange.Set(value);
        }

        public String_js responseText
        {
            get => GetInner<String_js>("responseText");
        }

        public Number_js readyState
        {
            get => GetInner<Number_js>("readyState" );
        }

        public Number_js status
        {
            get => GetInner<Number_js>("status");
        }

        internal override string HowDeclare => "new XMLHttpRequest()";
    }

}
