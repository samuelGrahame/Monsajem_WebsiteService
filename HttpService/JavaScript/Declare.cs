using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Monsajem_Incs.HttpService.JavaScript
{
    public class Declare_js
    {
        public String_js AsString
        {
           get
           {
                var Result= new String_js();
                Result.DeclareNewName();
                return Result;
           }
        }
        public String_js this[string Value]=>
            new String_js(Value);
        public String_js[] this[string[] Values]
        {
            get
            {
                var Result = new String_js[Values.Length];
                for(int i=0;i<Values.Length;i++)
                {
                    Result[i] = AsString;
                }
                return Result;
            }
        }

        public Number_js AsNumber
        {
            get
            {
                var Result = new Number_js();
                Result.DeclareNewName();
                return Result;
            }
        }
        public Number_js this[int Value] =>
            new Number_js(Value);

        public XMLHttpRequest_js AsXMLHttpRequest
        {
            get
            {
                var Result = new XMLHttpRequest_js();
                Result.DeclareNewName();
                Result.HowSet(Result.HowDeclare+";");
                return Result;
            }
        }

        public Ref_js AsObject
        {
            get
            {
                return Ref_js.Declare;
            }
        }
        public Ref_js[] this[Ref_js[] Values]
        {
            get
            {
                for (int i = 0; i < Values.Length; i++)
                {
                    Values[i] = AsObject;
                }
                return Values;
            }
        }

        public Runable_js this[Action Value]
        {
            get
            {
                var Result = new Runable_js();
                Result.DeclareNewName();
                Result.Set(Value);
                return Result;
            }
        }

        public Declare_Array_js AsArray
        {
            get
            {
                return new Declare_Array_js();
            }
        }
    }

    public class Declare_Array_js
    {
        public Array_js<String_js> AsString
        {
            get
            {
                var Result = new Array_js<String_js>();
                Result.DeclareNewName();
                return Result;
            }
        }
        public Array_js<Number_js> AsNumber
        {
            get
            {
                var Result = new Array_js<Number_js>();
                Result.DeclareNewName();
                return Result;
            }
        }

        public Array_js<Ref_js> AsObject
        {
            get
            {
                var Result = new Array_js<Ref_js>();
                Result.DeclareNewName();
                return Result;
            }
        }
    }
}
