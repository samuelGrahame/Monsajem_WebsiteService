using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Monsajem_Incs.HttpService.JavaScript.js;

namespace Monsajem_Incs.HttpService.JavaScript
{
    public class Ajax_js
    {
        private String_js Recive(XMLHttpRequest_js XmlHttp,Action AfterThat)
        {
            
            XmlHttp.onreadystatechange.WaitForHandle(WhenHandled: (c)=>
            {
                js.If(XmlHttp.readyState == 4,()=> {

                    js.If(XmlHttp.status == 200, () => {
                        c();
                    });
                });
            },HowHandle:AfterThat);
            return XmlHttp.responseText;
        }

        public String_js Get(String_js Path)
        {
            var XmlHttp =js.Declare.AsXMLHttpRequest;
            return Recive(XmlHttp,()=>{
                XmlHttp.Open_Get(Path, true);
                XmlHttp.Send();
            });
        }
        public String_js Post(String_js Path, Ref_js Data)
        {
            var XmlHttp = js.Declare.AsXMLHttpRequest;
            return Recive(XmlHttp, () =>
            {
                XmlHttp.Open_Post(Path, true);
                XmlHttp.Send(Data);
            });
        }

        public String_js Get(String_js Path, Ref_js Data)
        {
            var XmlHttp = js.Declare.AsXMLHttpRequest;
            return Recive(XmlHttp, () =>
            {
                XmlHttp.Open_Get(Path, true);
                XmlHttp.Send(Data);
            });
        }

        public String_js Get(string Path, Ref_js Data)
        {
            return Get(new String_js(Path), Data);
        }
        public String_js Get(string Path)
        {
           return Get(new String_js(Path));
        }
        public String_js Post(string Path, Ref_js Data)
        {
            return Post(new String_js(Path), Data);
        }
        public String_js Post<t>(String_js Path, t Data)
        {
            return Post(Path,(Ref_js) new String_js(Convert.ToBase64String((Data.Serialization()))));
        }
        public String_js Post<t>(string Path, t Data)
        {
            return Post(new String_js(Path),(Ref_js)
                             new String_js(Convert.ToBase64String((Data.Serialization()))));
        }
    }
}
