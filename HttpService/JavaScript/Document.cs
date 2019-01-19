using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monsajem_Incs.HttpService.JavaScript
{
    public class Document :
        Ref_js
    {

        public void Clear()
        {
            js.SendJS(HowGet + ".body.innerHTML = '';");
        }

        public void ClearScripts()
        {
            js.SendJS("while (" + HowGet + ".scripts.length > 0) { " + HowGet + ".scripts[0].outerHTML = ''; };");
        }

        public void FixPage()
        {
            //js.Function(() =>
            //{
            //    js.SendJS(
            //        "function DoGetValues(Element){while(Element != null){" +
            //    "if(Element.value != undefined)Element.setAttribute('value', Element.value);" +
            //    "if(Element.firstElementChild != null){DoGetValues(Element.firstElementChild);}" +
            //    "Element = Element.nextElementSibling;}};" +
            //    "DoGetValues(" + HowGet + ".firstElementChild);");
            //}).Run();
        }

        public HtmlElement GetElementById(string Id)
        {
            return new HtmlElement { Name = HowGet + ".getElementById('" + Id + "')" };
        }

        public HtmlElement Body
        {
            get => new HtmlElement { Name = HowGet + ".body" };
        }

        public HtmlElement Head
        {
            get => new HtmlElement { Name = HowGet + ".head" };
        }

        public HtmlElement firstElementChild
        {
            get
            {
                var Name = js.DeclareGlobal();
                js.SendJS(Name + " = " + HowGet + ".firstChild; ");
                return new HtmlElement { Name = Name };
            }
        }

        public FromData_js InnerData
        {
            get => new FromData_js()
            {
                HowGet =
                "(function(){var Data='';function GetElementData(Element){while (Element != null){if (Element.getAttribute('name') != null){if (Data.length > 0)Data = Data + '&';Data = Data + Element.getAttribute('name') + '=' + encodeURI(Element.value);}if (Element.firstElementChild != null)GetElementData(Element.firstElementChild);Element = Element.nextElementSibling;}}GetElementData(" + HowGet + ".firstElementChild);return Data;}).call()"
            };
        }

        public MultyPartData_js InnerData_MutiPart
        {
            get => new MultyPartData_js()
            {
                HowGet =
                "(function () {var Data = new FormData();function GetElementData(Element){while (Element != null){if (Element.getAttribute('name') != null){if (Element.tagName == 'INPUT' & Element.getAttribute('type') == 'file'){for (var i = 0; i < Element.files.length; i++){Data.append(Element.files[i].size + ';' + Element.name, Element.files[i]);}} else Data.append(Element.value.length + ';' + Element.name,Element.value);} if (Element.firstElementChild != null) GetElementData(Element.firstElementChild); Element = Element.nextElementSibling;}} GetElementData(" + HowGet + ".firstElementChild); return Data;}).call()"
            };
        }

        public String_js ScriptsAsString
        {
            get
            {
                var Name = js.DeclareGlobal();
                js.SendJS("var " + Name + " = (function(Response){function GetScript(Element){while (Element != null)" +
                    "{if(Element.tagName.toLowerCase()=='script'){Response+=Element.innerHTML;" +
                    "}else if(Element.firstElementChild!=null){" +
                    "GetScript(Element.firstElementChild);}Element=Element.nextElementSibling;" +
                    "}}var Element = Response.firstElementChild;" +
                    "Response='';GetScript(Element);return(Response);}).call(null," + HowGet + "); ");
                return new String_js()
                { Name = Name };
            }
        }

        public void Write(String_js value)
        {
            js.SendJS(HowGet + ".write(" + value.HowGet + ");");
        }

        public void WriteAndEval(String_js value)
        {
            js.SendJS(
                "(function(Response){function GetScript(Element){while (Element != null){if(Element.tagName.toLowerCase()=='script'){Response+=Element.innerHTML;Element.innerHTML='';}else if(Element.firstElementChild!=null){GetScript(Element.firstElementChild);}Element=Element.nextElementSibling;}}var Element=new DOMParser().parseFromString(Response,'text/html').firstElementChild;Response='';GetScript(Element);while(Element!=null){document.write(Element.innerHTML);Element=Element.nextElementSibling;}eval(Response);}).call(null," + value.HowGet + ");");
        }

        public void Append(String_js Data)
        {
            var IDoc = js.MakeDocumentFromString(Data);
            var scripts = IDoc.ScriptsAsString;
            IDoc.ClearScripts();
            var inner = IDoc.Head.firstElementChild;
            js.While(inner != null, () =>
            {
                var next = inner.nextElementSibling;
                this.Head.appendChild(inner);
                inner.Set(next);
            });
            inner.Set(IDoc.Body.firstElementChild);
            js.While(inner != null, () =>
            {
                var next = inner.nextElementSibling;
                this.Body.appendChild(inner);
                inner.Set(next);
            });
            js.EvalGlobal(scripts);
        }

        public void AppendFrom(String_js Address)
        {
           Append(js.Ajax.Get(Address));
        }

    }
}
