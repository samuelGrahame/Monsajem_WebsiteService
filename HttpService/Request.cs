using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Linq;
namespace Monsajem_Incs.HttpService
{
    public class RequestData
    {
        public new string ToString()
        {
            return new StreamReader(Request.MyContext.Request.InputStream).ReadToEnd();
        }

        public t To<t>()
        {
            return (t)Convert.FromBase64String(
                                     new StreamReader(Request.MyContext.Request.InputStream).ReadToEnd())
                                             .Deserialize();
        }
    }

    public class Request:
        IComparable<Request>
    {
        public string Address;
        public System.Threading.Thread Service;
        private int Id;
        public HttpListenerContext Context;
        public int CompareTo(Request other)
        {
            return this.Id-other.Id;
        }

        [ThreadStaticAttribute]
        public static HttpListenerContext MyContext;
        [ThreadStaticAttribute]
        public static string[] Steps;
        public static RequestData Data{ get => new RequestData(); }
        private static Request[] FetchedRequests=new Request[0];

        public static void Ready(HttpListenerContext MyContext)
        {
            HttpService.Request.MyContext = MyContext;
            Steps = new string[0];
            var Request = MyContext.Request;

            if (Request.RawUrl.Length > 1)
            {
                var Position = 1;
                var End = Request.Url.AbsolutePath.IndexOf("/", Position);
                while (End != -1)
                {
                    ArrayExtentions.Insert(ref Steps, Request.Url.AbsolutePath.Substring(Position, End - Position));
                    Position = End + 1;
                    End = Request.Url.AbsolutePath.IndexOf("/", Position);
                }
                if (Position != Request.Url.AbsolutePath.Length)
                    ArrayExtentions.Insert(ref Steps, Request.Url.AbsolutePath.Substring(Position, Request.Url.AbsolutePath.Length - Position));
            }
        }

        private static string WebToStringParse(string value)
        {
            var Result = "";
            for(int i=0; i<value.Length;i++)
            {

                if (value[i].ToString() == "+")
                    Result += " ";
                else if (value[i].ToString() == "%")
                {
                    var Buffer = new byte[0];
                    while (value[i].ToString() == "%")
                    {
                        ArrayExtentions.Insert(ref Buffer,
                            (byte)HexToDecimal(value.Substring(i + 1, 2)));
                        i += 3;
                        if (! (i < value.Length))
                            break;
                    }
                    Result += System.Text.Encoding.UTF8.GetString(Buffer);
                }
                else
                    Result += value[i].ToString();
            }
            return Result;
        }

        private static int HexToDecimal(string HexValue)
        {
            var Result = 0;
            int BaseValue = System.Text.Encoding.UTF8.GetBytes("0")[0];
            for(int i= HexValue.Length-1; i != -1;i--)
            {
                var CurrentValue = 
                    System.Text.Encoding.UTF8.GetBytes(HexValue[i].ToString())[0]-BaseValue;
                if (CurrentValue > 15)
                    CurrentValue -= 7;
                CurrentValue = CurrentValue * (int)System.Math.Pow(16, HexValue.Length-1-i);
                Result = Result + CurrentValue;
            }
            return Result;
        }

        public static string GetDataFrom(string Path)
        {
            if (Path.Substring(0, 1) == "/")
                if (MyContext.Request.IsSecureConnection)
                    Path = "https://127.0.0.1:"+MyContext.Request.LocalEndPoint.Port+Path;
                else
                    Path = "http://127.0.0.1:" + MyContext.Request.LocalEndPoint.Port + Path;

            var Request =(HttpWebResponse)HttpWebRequest.Create(Path).GetResponse();
            
            if (Request.CharacterSet.ToLower() == "utf-8")
                return (new System.IO.StreamReader(
                    Request.GetResponseStream(), System.Text.Encoding.UTF8).ReadToEnd());
            else if (Request.CharacterSet.ToLower() == "utf-16")
                return (new System.IO.StreamReader(
                   Request.GetResponseStream(), System.Text.Encoding.Unicode).ReadToEnd());
            else
                throw new FormatException("CharacterSet " + Request.CharacterSet + " Not Defined");
        }

        public static void Response(System.IO.Stream Stream)
        {
            try
            {
                try
                {
                    MyContext.Response.ContentType = "Data";
                }
                catch{}
                var Buffer = new byte[Stream.Length];
                Stream.Read(Buffer, 0, Buffer.Length);
                MyContext.Response.OutputStream.Write(Buffer, 0,Buffer.Length);
                Stream.Close();
                MyContext.Response.OutputStream.Flush();
            }
            catch { CloseService(); }
        }

        public static void Response(byte[] Buffer)
        {
            try
            {
                FlushScript();
                try
                {
                    MyContext.Response.ContentType = "Data";
                }
                catch{}
                MyContext.Response.OutputStream.Write(Buffer, 0, Buffer.Length);
                MyContext.Response.OutputStream.Flush();
            }
            catch { CloseService(); }
        }

        public static void Response(string Data)
        {
            try
            {
                FlushScript();
                try
                {
                    MyContext.Response.ContentType = "text/html; charset=UTF-8";
                }
                catch{}
                var Buffer = System.Text.Encoding.UTF8.GetBytes(Data);
                MyContext.Response.OutputStream.Write(Buffer, 0, Buffer.Length);
                MyContext.Response.OutputStream.Flush();
            }
            catch{CloseService();}
        }

        public static void Redirect(string Path)
        {
            MyContext.Response.Redirect(Path);
            CloseService();
        }

        public static void Fetch()
        {
            var rq = new Request();
            rq.Service = System.Threading.Thread.CurrentThread;
            rq.Address = MyContext.Request.UserHostAddress;
            rq.Id = rq.Service.ManagedThreadId;
            lock(FetchedRequests)
                ArrayExtentions.BinaryInsert(ref FetchedRequests, rq);
            JavaScript.js.Document.Append(JavaScript.js.Ajax.Post("/Continue", rq.Id));
            FlushScript();
            MyContext.Response.OutputStream.Close();
            MyContext = null;
            rq.Service.Suspend();
            if (rq.Context == null)
            {
                lock (FetchedRequests)
                    ArrayExtentions.BinaryDelete(ref FetchedRequests, rq);
                rq.Service.Abort();
            }
            MyContext = rq.Context;
            lock (FetchedRequests)
                ArrayExtentions.BinaryDelete(ref FetchedRequests, rq);
        }

        public static void BeginFetch()
        {
            JavaScript.js.Document.Append(JavaScript.js.Ajax.Post("/Continue", 
                System.Threading.Thread.CurrentThread.ManagedThreadId));
        }

        public static void EndFetch()
        {
            var rq = new Request();
            rq.Service = System.Threading.Thread.CurrentThread;
            rq.Address = MyContext.Request.UserHostAddress;
            rq.Id = rq.Service.ManagedThreadId;
            lock (FetchedRequests)
                ArrayExtentions.BinaryInsert(ref FetchedRequests, rq);
            FlushScript();
            MyContext.Response.OutputStream.Close();
            MyContext = null;
            rq.Service.Suspend();
            if (rq.Context == null)
            {
                lock (FetchedRequests)
                    ArrayExtentions.BinaryDelete(ref FetchedRequests, rq);
                rq.Service.Abort();
            }
            MyContext = rq.Context;
            lock (FetchedRequests)
                ArrayExtentions.BinaryDelete(ref FetchedRequests, rq);
        }

        public static void Fetch(JavaScript.Ref_js obj)
        {
            var rq = new Request();
            rq.Service = System.Threading.Thread.CurrentThread;
            rq.Address = MyContext.Request.UserHostAddress;
            rq.Id = rq.Service.ManagedThreadId;
            lock (FetchedRequests)
                ArrayExtentions.BinaryInsert(ref FetchedRequests, rq);
            JavaScript.js.Document.Append(JavaScript.js.Ajax.Post("/ContinueWithData?id="+rq.Id, obj));
            FlushScript();
            MyContext.Response.OutputStream.Close();
            MyContext = null;
            rq.Service.Suspend();
            if (rq.Context == null)
            {
                lock (FetchedRequests)
                    ArrayExtentions.BinaryDelete(ref FetchedRequests, rq);
                rq.Service.Abort();
            }
            MyContext = rq.Context;
            lock (FetchedRequests)
                ArrayExtentions.BinaryDelete(ref FetchedRequests, rq);
        }

        public static FileData[] Fetch(JavaScript.HtmlElement obj)
        {
            
            var rq = new Request();
            rq.Service = System.Threading.Thread.CurrentThread;
            rq.Address = MyContext.Request.UserHostAddress;
            rq.Id = rq.Service.ManagedThreadId;
            lock (FetchedRequests)
                ArrayExtentions.BinaryInsert(ref FetchedRequests, rq);
            JavaScript.js.Document.Append(JavaScript.js.Ajax.Post("/ContinueWithData?id=" + rq.Id, 
               (JavaScript.Ref_js) new JavaScript.MultyPartData_js()
               {
                    HowGet =
                        "(function () {var Data = new FormData(); var El = " + obj.HowGet +
                        ";  for (var i = 0; i < El.files.length; i++){Data.append(El.files[i].size + ';' + El.name, El.files[i]);}; return Data;}).call()"
               }));
            FlushScript();
            MyContext.Response.OutputStream.Close();
            MyContext = null;
            rq.Service.Suspend();
            if (rq.Context == null)
            {
                lock (FetchedRequests)
                    ArrayExtentions.BinaryDelete(ref FetchedRequests, rq);
                rq.Service.Abort();
            }
            MyContext = rq.Context;
            lock (FetchedRequests)
                ArrayExtentions.BinaryDelete(ref FetchedRequests, rq);
            var Token = Request.MyContext.Request.Headers.Get("Content-Type").Substring(30);
            Token = "--" + Token;
            var Data = MultyPartData.CreateFileData(Request.MyContext.Request.InputStream, Token);
            var Files = new FileData[Data.Length];
            for(int i=0;i<Data.Length; i++)
            {
                Files[i] = new FileData()
                {
                    Name = Data[i].ContentDisposition.Where((c) => c.Name == "filename").First().Value,
                    Data = Data[i].Data
                };
            }
            return Files;
        }

        internal static void Continue(int id)
        {
            var Index = Array.BinarySearch(FetchedRequests, new Request() { Id = id });
            var rq = FetchedRequests[Index];
            rq.Continue(MyContext);
        }

        internal void Continue(HttpListenerContext MyContext)
        {
            if (MyContext.Request.UserHostAddress != this.Address)
            {
                this.Service.Abort();
                lock (FetchedRequests)
                    ArrayExtentions.BinaryDelete(ref FetchedRequests, this);
                System.Threading.Thread.CurrentThread.Abort();
            }
            this.Context = MyContext;
            this.Service.Resume();
            System.Threading.Thread.CurrentThread.Abort();
        }

        public static void CloseService()
        {
            FlushScript();
            MyContext.Response.OutputStream.Close();
            System.Threading.Thread.CurrentThread.Abort();
        }

        internal static void FlushScript()
        {
            if(JavaScript.js.SendJS == HttpService.Service.CountiniueSendJs)
            {
                JavaScript.js.CurrentFunction.EndFunction();
                JavaScript.js.CurrentFunction.Run_Exact();
                JavaScript.js.SendJS = HttpService.Service.BeginSendJs;
                Response("</script></body></html>");
            }
        }
    }

    public class MimeType
    {
        public MimeType(string Name)
        {
            this.Name = Name;
        }
        public readonly string Name;

        public static readonly MimeType Text_Plain = new MimeType("text/plain");
        public static readonly MimeType Text_Html = new MimeType("text/html");
        public static readonly MimeType Text_Css = new MimeType("text/css");
        public static readonly MimeType Text_Javascript = new MimeType("text/javascript");
    }

}
