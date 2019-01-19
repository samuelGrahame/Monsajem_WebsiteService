using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArrayExtentions.ArrayExtentions;

namespace Monsajem_Incs.HttpService.JavaScript
{
    internal static class Ref_js_ex
    {
        public static t StandAlone<t>(this t Value)
            where t:Ref_js,new()
        {
            var Result = new t();
            Result.DeclareNewName();
            Result.Set(Value);
            return Result;
        }
    }

    public class Ref_js
    {
        internal string HowGet;
        internal Action<string> HowSet;

        internal string Name
        {
            set
            {
                HowGet = value;
                HowSet = (V) => js.SendJS(value + "=" + V + ";");
            }
        }

        internal void GetNewName()
        {
            this.Name = js.GetNewName();
        }

        internal void DeclareNewName()
        {
            this.Name = js.DeclareGlobal();
        }

        public void Clear()
        {
            HowSet("null");
        }

        internal void Set(string Value)
        {
            HowSet(Value);
        }

        internal void Set(Ref_js Value)
        {
            HowSet(Value.HowGet);
        }

        public new String_js ToString()
        {
            return new String_js() { HowGet = this.HowGet + ".toString()" };
        }

        internal t GetInner<t>(string Name)
            where t:Ref_js,new()
        {
            return new t() { Name = this.HowGet + "." + Name };
        }

        internal t Convert<t>()
            where t:Ref_js,new()
        {
            var Result = new t();
            Result.HowGet = this.HowGet;
            Result.HowSet = this.HowSet;
            return Result;
        }
 
        public static implicit operator Ref_js(string Value)
        {
            return new Ref_js() { HowGet=Value };
        }
        public static Ref_js[] MakeJsObjects(params Type[] Types)
        {
            var Results = new Ref_js[0];
            var CP = new Type[0];
            for (int i = 0; i < Results.Length - 1; i++)
            {
                Results[i] =(Ref_js)Types[i].GetConstructor(CP).Invoke(null);
                Results[i].Name = js.GetNewName();
            }
            if (Results.Length > 0)
            {
                var Last = Results.Length - 1;
                Results[Last] =
                   (Ref_js)Types[Last].GetConstructor(CP).Invoke(null);
                Results[Last].Name = js.GetNewName();
            }
            return Results;
        }

        internal static Ref_js Declare
        {
            get
            {
                var Result = new Ref_js();
                Result.DeclareNewName();
                Result.Set(Result.HowDeclare);
                return Result;
            }
        }

        internal virtual string HowDeclare { get => "''"; }
    }
}
