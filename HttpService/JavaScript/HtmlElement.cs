using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monsajem_Incs.HttpService.JavaScript
{
    public class HtmlElement:
        Ref_js
    {

        public String_js innerHTML {
            get => GetInner<String_js>("innerHTML");
            set => innerHTML.Set(value);
        }
        public String_js outerHTML {
            get => GetInner<String_js>("outerHTML" );
            set => outerHTML.Set(value);
        }
        public String_js textContent
        {
            get => GetInner<String_js>("textContent" );
            set => textContent.Set(value);
        }

        public String_js Attribute(String_js Name)
        {
            return new String_js()
            {
                HowSet = (value) => js.SendJS(
                    this.HowGet + ".setAttribute(" + Name.HowGet + "," + value + ");"),
                HowGet = this.HowGet + ".getAttribute(" + Name.HowGet +");"
            };
        }

        // how convert to DataUrl

        //public void Attribute(String_js Name, byte[] Value)
        //{
        //    Attribute(Name, new String_js() { Script = "'data:;base64," + Convert.ToBase64String(Value) + "'" });
        //}

        public void AddEventListener(String_js EventName,Runable_js Action)
        {
            js.SendJS(HowGet + ".addEventListener(" + EventName.HowGet + "," + Action.HowGet+"); ");
        }
        public void SetEvent(String_js EventName, Runable_js Action)
        {
            this.Attribute("on" + EventName).Set(Action.HowGet+"();");
        }
        public void GetEvent(String_js EventName)
        {
            this.Attribute("on" + EventName);
        }
        public unsafe void DropEvent(String_js EventName)
        {
            this.Attribute("on" + EventName).Set("");
        }


        public Runable_js OnClick
        {
            get => GetInner<Runable_js>("onclick");
            set => OnClick.Set(value);
        }

        public String_js Value
        {
            get => GetInner<String_js>("value");
            set => Value.Set(value);
        }

        public ElementStyle_js Style
        {
            get => new ElementStyle_js(this);
        }

        public FileData[] Files
        {
            get => Request.Fetch(this);
        }

        public HtmlElement parentElement
        {
            get => GetInner<HtmlElement>("parentElement").StandAlone();
        }

        public HtmlElement firstElementChild
        {
            get => GetInner<HtmlElement>("firstElementChild").StandAlone();
        }

        public HtmlElement nextElementSibling
        {
            get => GetInner<HtmlElement>("nextElementSibling").StandAlone();
        }

        internal override string HowDeclare => throw new NotImplementedException();

        public void Using(Action<HtmlElement> Action)
        {
            Action(this);
        }

        public void appendChild(HtmlElement value)
        {
            js.SendJS(HowGet+".appendChild("+value.HowGet+");");
        }

        public String_js GetInnerData()
        {
            var Result = new String_js();
            Result.HowGet="(function(){var Data='';function GetElementData(Element){while (Element != null){if (Element.getAttribute('name') != null){if (Data.length > 0)Data = Data + '&';Data = Data + Element.getAttribute('name') + '=' + encodeURI(Element.value);}if (Element.firstElementChild != null)GetElementData(Element.firstElementChild);Element = Element.nextElementSibling;}}GetElementData(" + HowGet + ".firstElementChild);return Data;}).call()";
            return Result;
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
                js.Document.Head.appendChild(inner);
                inner.Set(next);
            });
            inner.Set(IDoc.Body.firstElementChild);
            js.While(inner != null, () =>
            {
                var next = inner.nextElementSibling;
                this.appendChild(inner);
                inner.Set(next);
            });
            js.Eval(scripts);
        }

        public static Bool_js operator ==(HtmlElement obj1,HtmlElement obj2)
        {
            if (obj2 is null & !(obj1 is null))
            {
                return new Bool_js(obj1.HowGet + " == null");
            }
            if (obj1 is null & !(obj2 is null))
            {
                return new Bool_js(obj2.HowGet + " == null");
            }
            if (!(obj1 is null & obj2 is null))
            {
                return new Bool_js(obj1.HowGet + " == " + obj2.HowGet);
            }
            throw new NullReferenceException("in != and == operator both arguments can't be null.");
        }
        public static Bool_js operator !=(HtmlElement obj1, HtmlElement obj2)
        {
            if (obj2 is null & !(obj1 is null))
            {
                return new Bool_js(obj1.HowGet + " != null");
            }
            if (obj1 is null & !(obj2 is null))
            {
                return new Bool_js(obj2.HowGet + " != null");
            }
            if (!(obj1 is null & obj2 is null))
            {
                return new Bool_js(obj1.HowGet + " != " + obj2.HowGet);
            }
            throw new NullReferenceException("in != and == operator both arguments can't be null.");
        }
    }
}
