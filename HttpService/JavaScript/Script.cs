using System;
using System.Collections.Generic;
using System.Linq;
using static System.Text.Encoding;
using System.Threading.Tasks;
using static ArrayExtentions.ArrayExtentions;

namespace Monsajem_Incs.HttpService.JavaScript
{
    public static class js
    {
        [ThreadStaticAttribute]
        internal static int Declared;
        [ThreadStaticAttribute]
        internal static Action<string> SendJS;
        [ThreadStaticAttribute]
        internal static Runable_base_js CurrentFunction;

        internal static string GetNewName()
        {
            Declared += 1;
            return "v" + Declared;
        }

        internal static string[] GetNewNames(int Count)
        {
            var Results = new string[Count];
            for(;Count>-1;--Count)
            {
                Declared += 1;
                Results[Count]= "v" + Declared;
            }
            return Results;
        }

        internal static string DeclareGlobal()
        {
            Declared += 1;
            js.SendJS("var s=document.createElement('script');s.innerHTML='var v"+Declared+"=null;';document.body.appendChild(s);");
            return "v" + Declared;
        }

        internal static string GetJsFromActions(Action Action)
        {
            var SendJs = js.SendJS;
            var scripts = "";
            js.SendJS = (Script) => scripts += Script;
            Action?.Invoke();
            js.SendJS = SendJs;
            return scripts;
        }

        public static Document Document { get => new Document() { HowGet = "document" }; }

        public static void Alert(String_js Value)
        {
            js.SendJS("alert(" + Value.HowGet + ");");
        }

        public static void Alert(string Value)
        {
            Alert(new String_js(Value));
        }

        public static StyleValues_js Style
        {
            get => new StyleValues_js();
        }


        public static Declare_js Declare
        {
            get => new Declare_js();
        }


        public static void If(Bool_js Condition, Action ok, Action Else = null)
        {
            var Flow = js.Declare[()=> { }];
            var OnOk = js.Declare[() => { ok(); Flow.Run(); }];
            var OnElse = js.Declare[() => { Else?.Invoke(); Flow.Run(); }];
            Flow.WaitForHandle(() =>
            {
                SendJS("if(" + Condition.HowGet + ")");
                SendJS("{");
                OnOk.Run_Exact();
                js.SendJS("} else{");
                OnElse.Run_Exact();
                js.SendJS("}");
            });
        }

        public static void While(Bool_js Condition, Action ok)
        {
            var Flow = js.Declare[() => { }];
            var OnOk = js.Declare[() => { ok(); }];
            Flow.WaitForHandle(() =>
            {
                SendJS("while(" + Condition.HowGet + ")");
                SendJS("{");
                OnOk.Run();
                js.SendJS("}");
                Flow.Run_Exact();
            });
        }

        public static void Wait(Number_js Time)
        {
            var Action= js.Declare[() => { }];
            Action.WaitForHandle(HowHandle: () => Action.RunAfterTime(Time));
        }

        public static void Run(string ActionName,params Ref_js[] Parametrs)
        {
            js.SendJS(ActionName+"(");
            for (int i = 0; i < Parametrs.Length - 1; i++)
                js.SendJS(Parametrs[i].HowGet + ",");
            if (Parametrs.Length > 0)
                js.SendJS(Parametrs.Last().HowGet);
            js.SendJS(");");
        }
        public static t Run<t>(string ActionName, params Ref_js[] Parametrs)
            where t : Ref_js, new()
        {
            var Result = Declare.AsObject;
            js.SendJS(Result.HowGet+"="+ ActionName + "(");
            for (int i = 0; i < Parametrs.Length - 1; i++)
                js.SendJS(Parametrs[i].HowGet + ",");
            if (Parametrs.Length > 0)
                js.SendJS(Parametrs.Last().HowGet);
            js.SendJS(");");
            return Result.Convert<t>();
        }

        public static void Eval(String_js Script)
        {
            js.SendJS("eval(" + Script.HowGet + ");");
        }

        public static void EvalGlobal(String_js Script)
        {
            js.SendJS("var s=document.createElement('script');s.innerHTML=" + Script.HowGet + ";document.body.appendChild(s);");
        }

        public static String_js ToDataUrl(Action Actions)
        {
            var Result = new String_js() { Name = js.DeclareGlobal() };
            js.SendJS(Result.HowGet + " =(function(){var myScriptList=[decodeURIComponent('");
            Action<string> Sender = js.SendJS;
            js.SendJS = (Script) =>
            {
                Sender(DataURL_js.ByteToData(UTF8.GetBytes(Script)));
            };
            Actions();
            js.SendJS = Sender;
            js.SendJS("')];var myBlobObject=new Blob(myScriptList,{type:'text/javascript'});return window.URL.createObjectURL(myBlobObject);}).call()");
            return Result;
        }

        public static Ajax_js Ajax
        { get => new Ajax_js(); }

        public static Runable_js onload
        {
            get => new Runable_js { Name = "onload" };
        }

        public static void Return(String_js Value)
        {
            js.SendJS("return " + Value.HowGet + ";");
        }
        public static void Return(string Value)
        {
            Return(new String_js(Value));
        }

        public static void Return(Bool_js Value)
        {
            js.SendJS("return " + Value.HowGet + ";");
        }
        public static void Return(Boolean Value)
        {
            Return(new Bool_js(Value));
        }

        public static void Return(Number_js Value)
        {
            js.SendJS("return " + Value.HowGet + ";");
        }
        public static void Return(int Value)
        {
            Return(new Number_js(Value));
        }

        public static void Return()
        {
            js.SendJS("return;");
        }

        public static void BackGroundWork(Action actions)
        {
            var Script = ToDataUrl(actions).HowGet;
            js.SendJS(
                "var Worker = new Worker(" + ToDataUrl(actions).HowGet +
                 ");Worker.onmessage =function (event){eval(event.data);};Worker.postMessage('getMessage();');");
        }

        public static void RunAtServer(Action Action)
        {
            Document.Append(Ajax.Post("/Action", (Ref_js)new String_js()
            { HowGet = "\"" + System.Convert.ToBase64String(Action.SerializationDelegate()) + "\"" }));
        }
        public static void RunAtServer(Action<string> Action, String_js Data)
        {
            var ActionData = System.Convert.ToBase64String(Action.SerializationDelegate());
            Document.Append(Ajax.Post("/ActionWithJSData",
            (Ref_js)new String_js()
            {
                HowGet = "\"" + ActionData.Length + "," + ActionData + "\"+" + Data.HowGet
            }));
        }
        public static void RunAtServer(Action<WebData[]> Action, FromData_js Data)
        {
            var ActionData = System.Convert.ToBase64String(Action.SerializationDelegate());
            Document.Append(Ajax.Post("/ActionWithJDData",
            (Ref_js)new String_js()
            {
                HowGet = "\"" + ActionData.Length + "," + ActionData + "\"+" + Data.HowGet
            }));
        }

        public static void RunAtServer(Action<MultyPartData[]> Action, MultyPartData_js Data)
        {
            var ActionData = System.Convert.ToBase64String(Action.SerializationDelegate());
            Document.Append(Ajax.Post("/ActionWithJMData?Length=" +
                            EncodeURIComponent(ActionData.Length.ToString()) +
                            "&Action=" + EncodeURIComponent(ActionData),
                (Ref_js)new String_js()
                {
                    HowGet = Data.HowGet
                }));
        }

        public static void DropState()
        {
            js.SendJS("window.onpopstate = function (e){ if(e.state.OnPop != undefined) {onpopstate = eval(e.state.OnPop); }}; history.back();");
            Request.Fetch();
        }

        public static void PushState(Runable_js Onpop)
        {
            //var Name = new String_js();
            //Name.Name = js.GetNewName();
            //js.SendJS(" if (onpopstate != null){ history.replaceState({OnPop:'('+onpopstate.toString()+')'}, '',null);} else { history.replaceState('','',null);} ");

            //var Ac = new Runable_js<String_js>((v) =>
            //{
            //    js.SendJS(" if (" + Name.HowGet + ".state.OnPop != undefined){onpopstate =eval(" + Name.HowGet + ".state.OnPop);} ");
            //    Onpop.Run();
            //}, Name);

            //js.SendJS("onpopstate = " + Ac.HowGet + ";history.pushState(null, null, location.href);");
        }

        public static void PushState<t>(t ValueOnpop, Action<t> Onpop)
            where t : Ref_js, new()
        {
            //var Value = new t();
            //var Name = new t();
            //Name.Name = js.GetNewName();
            //Value.Name = Name.HowGet + ".state.vl";
            //js.SendJS(" if (onpopstate != null){ history.replaceState({OnPop:'('+onpopstate.toString()+')',vl:" + ValueOnpop.HowGet + "}, '',null);} else { history.replaceState({vl:" + ValueOnpop.HowGet + "},'',null);} ");

            //var Ac = new Runable_js<t>((v) =>
            //{
            //    js.SendJS(" if (" + Name.HowGet + ".state.OnPop != undefined){onpopstate =eval(" + Name.HowGet + ".state.OnPop);} ");
            //    new Runable_js<t>(Onpop, Name).Run(Value);
            //}, Name);

            //js.SendJS("onpopstate = " + Ac.HowGet + ";history.pushState(null, null, location.href);");
        }

        public static void LockBack()
        {
            js.SendJS("if(onpopstate != null){ history.pushState({ OnPop: '(' + onpopstate.toString() + ')' }, null, location.href); } else { history.pushState(null, null, location.href); };window.onpopstate = function() { history.go(1);};");
        }

        public static void UnLockBack()
        {
            DropState();
        }

        public static void Redirect(string Path)
        {
            js.SendJS("window.location = \"" + Path + "\";");
        }

        public static String_js EncodeURIComponent(String_js value)
        {
            return new String_js() { HowGet = "encodeURIComponent(" + value.HowGet + ")" };
        }

        public static Document MakeDocumentFromString(String_js Value)
        {
            var Name = js.DeclareGlobal();
            js.SendJS(Name + " = new DOMParser().parseFromString(" + Value.HowGet + ",'text/html');");
            return new Document() { HowGet = Name };
        }

        public static String_js DropScriptFromHTML(String_js value)
        {
            return new String_js()
            {
                HowGet = "(function(Response){function GetScript(Element){while (Element != null)" +
            "{if(Element.tagName.toLowerCase()=='script'){" +
            "Element.innerHTML='';}else if(Element.firstElementChild!=null){" +
            "GetScript(Element.firstElementChild);}Element=Element.nextElementSibling;" +
            "}}var Element=new DOMParser().parseFromString(Response,'text/html').firstElementChild;" +
            "Response='';GetScript(Element);while(Element!=null){Response+=Element.innerHTML;" +
            "Element=Element.nextElementSibling;}return(Response);}).call(null," + value.HowGet + ")"
            };
        }
    }
}
