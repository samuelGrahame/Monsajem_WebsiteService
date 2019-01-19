using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static ArrayExtentions.ArrayExtentions;

namespace Monsajem_Incs.HttpService.JavaScript
{
    public class Runable_base_js :
        Ref_js
    {
        public Runable_base_js()
        { }
        public Action[] EndBlocks = new Action[0];
        public Runable_base_js LastPoint;

        internal void Run_Exact(params Ref_js[] Parameters)
        {
            if (Parameters.Length > 0)
            {
                js.SendJS(HowGet + ".call(null,");
                for (int i = 0; i < Parameters.Length - 1; i++)
                {
                    js.SendJS(Parameters[i].HowGet + ",");
                }
                js.SendJS(Parameters[Parameters.Length - 1].HowGet);
            }
            else
            {
                js.SendJS(HowGet + ".call(");
            }
            js.SendJS(");");
        }

        public void RunAfterTime(Number_js Time, params Ref_js[] Parameters)
        {
            var Action= js.Declare[() =>
            { this.Run(); }];
            js.Run("setTimeout",Action,Time);
        }

        public void RunEveryTime(Number_js Time, params Ref_js[] Parameters)
        {
            var Action = js.Declare[() =>
            { this.Run(); }
            ];
            js.Run("setInterval", Action, Time);
        }

        internal void Run(params Ref_js[] Parameters)
        {
            if (LastPoint != null)
            {
                LastPoint.WaitForHandle(HowHandle: () => Run_Exact(Parameters));
            }
            else
            {
                Run_Exact(Parameters);
            }
        }
        internal Ref_js Result(params Ref_js[] Parameters)
        {
            if (LastPoint != null)
            {
                var Result = LastPoint.WaitForHandle(InputCount: 1, HowHandle: () => Run_Exact(Parameters));
                return Result[0];
            }
            else
            {
                Ref_js Result = null;
                Result = new Ref_js();
                Result.DeclareNewName();
                js.SendJS(Result.HowGet + "=");
                Run_Exact(Parameters);
                return Result;
            }
        }

        internal void WaitForHandle(Action HowHandle)
        {
            var next = js.GetNewName();
            Set(() =>
            {

                js.SendJS(next + "();");
            });
            js.SendJS("function " + next + "(){");
            Insert(ref js.CurrentFunction.EndBlocks,
                 () => { js.SendJS("}"); HowHandle?.Invoke(); });
        }

        internal Ref_js[] WaitForHandle(int InputCount, Action HowHandle)
        {
            var Result = js.Declare[new Ref_js[InputCount]];
            var next = js.GetNewName();
            Set(InputCount, (Inputs) =>
            {
                Result = Inputs;
                js.SendJS(next + "();");
            });
            js.SendJS("function " + next + "(){");
            Insert(ref js.CurrentFunction.EndBlocks,
                 () => { js.SendJS("}"); HowHandle?.Invoke(); });
            return Result;
        }

        internal Ref_js[] WaitForHandle(int InputCount, Action<Ref_js[], Action> WhenHandeled, Action HowHandle)
        {
            if (WhenHandeled == null)
                WhenHandeled = (r, c) => c();
            var Result = js.Declare[new Ref_js[InputCount]];
            var next = js.GetNewName();
            Set(InputCount, (Inputs) =>
             {
                 Result = Inputs;
                 WhenHandeled(Result, () => js.SendJS(next + "();"));
             });
            js.SendJS("function " + next + "(){");
            Insert(ref js.CurrentFunction.EndBlocks,
                 () => { js.SendJS("}"); HowHandle?.Invoke(); });
            return Result;
        }
        internal void WaitForHandle(Action<Action> WhenHandeled, Action HowHandle)
        {
            if (WhenHandeled == null)
                WhenHandeled = (c) => c();
            var next = js.GetNewName();
            Set(() =>
            {
                WhenHandeled(() => js.SendJS(next + "();"));
            });
            js.SendJS("function " + next + "(){");
            Insert(ref js.CurrentFunction.EndBlocks,
                 () => { js.SendJS("}"); HowHandle?.Invoke(); });
        }

        internal Ref_js[] BeginFunction(int Inputs = 0)
        {
            Ref_js[] Parametrs = new Ref_js[Inputs];
            for (int i = 0; i < Inputs - 1; i++)
            {
                Parametrs[i] = new Ref_js() { Name = "v" + i };
                js.SendJS(Parametrs[i].HowGet + ",");
            }
            if (Inputs > 0)
            {
                Parametrs[Inputs - 1] = new Ref_js() { Name = "v" + (Inputs - 1) };
                js.SendJS(Parametrs[Inputs - 1].HowGet);
            }
            js.SendJS("){");
            Insert(ref EndBlocks, () => js.SendJS("};"));
            return Parametrs;
        }
        internal void EndFunction()
        {
            var MustHaveLastPoint = EndBlocks.Length > 1;
            if (MustHaveLastPoint)
            {
                LastPoint = new Runable_base_js();
                LastPoint.GetNewName();
                LastPoint.Run();
            }
            while (EndBlocks.Length > 0)
                Pop(ref EndBlocks)();

            if (MustHaveLastPoint)
                LastPoint.make();
        }

        internal Ref_js[] BeginMake(int InputCount)
        {
            js.SendJS("function " + HowGet + "(");
            return BeginFunction(InputCount);
        }
        internal Ref_js[] BeginSet(int InputCount)
        {
            js.SendJS(HowGet + "=function(");
            return BeginFunction(InputCount);
        }

        internal void make(int InputCount, Action<Ref_js[]> Action)
        {
            var Parametrs = BeginMake(InputCount);
            var NextFunction = js.CurrentFunction;
            js.CurrentFunction = this;
            Action(Parametrs);
            js.CurrentFunction = NextFunction;
            EndFunction();
        }
        internal void make(Action Action)
        {
            BeginMake(0);
            var NextFunction = js.CurrentFunction;
            js.CurrentFunction = this;
            Action();
            js.CurrentFunction = NextFunction;
            EndFunction();
        }
        internal void make()
        {
            BeginMake(0);
            EndFunction();
        }
        internal void Set(int InputCount, Action<Ref_js[]> Action)
        {
            var Parametrs = BeginSet(InputCount);
            var NextFunction = js.CurrentFunction;
            js.CurrentFunction = this;
            Action(Parametrs);
            js.CurrentFunction = NextFunction;
            EndFunction();
        }
        internal void Set(Action Action)
        {
            BeginSet(0);
            var NextFunction = js.CurrentFunction;
            js.CurrentFunction = this;
            Action();
            js.CurrentFunction = NextFunction;
            EndFunction();
        }
    }
}
