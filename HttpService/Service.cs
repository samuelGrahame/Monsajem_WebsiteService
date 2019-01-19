using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;

namespace Monsajem_Incs.HttpService
{
    public class Service :
        Responsor
    {
        public HttpListener MyService = new HttpListener();
        private System.Threading.Thread trd_Service;

        public Service(
            string Prefixe,
            Action Way,
            Action Service) :
            base(Way, Service)
        {
            MyService.Prefixes.Add(Prefixe);
            this.AddService("Action", () =>
            {
                var Action =
                     Serializations.DeserializeDelegate(
                         System.Convert.FromBase64String(Request.Data.ToString()));
                Action.Method.Invoke(Action.Target, null);
            });
            this.AddService("ActionWithJDData",
            () =>
            {
                var RequestData = Request.Data.ToString();
                var Lenth = RequestData.Substring(0,
                        RequestData.IndexOf(","));
                var ResponsorData = RequestData.Substring(
                                    Lenth.Length + 1, int.Parse(Lenth));
                RequestData = RequestData.Substring(
                                    Lenth.Length + ResponsorData.Length + 1);

                var Action = Serializations.DeserializeDelegate(
                            Convert.FromBase64String(ResponsorData));

                Action.Method.Invoke(Action.Target, WebData.GetData(RequestData));
            });

            this.AddService("ActionWithJMData",
            () =>
            {
                var len = Request.MyContext.Request.QueryString.Get("Length");
                var Action = Convert.FromBase64String(
                                Request.MyContext.Request.QueryString.Get("Action")).DeserializeDelegate();
                var Token = Request.MyContext.Request.Headers.Get("Content-Type").Substring(30);
                Token = "--" + Token;
                Action.Method.Invoke(Action.Target,
                    MultyPartData.CreateFileData(
                        Request.MyContext.Request.InputStream, Token));
            });

            this.AddService("Continue",
            () =>
            {
                Request.Continue(Request.Data.To<int>());
            });
            this.AddService("ContinueWithData",
            () =>
            {
                Request.Continue(int.Parse(Request.MyContext.Request.QueryString.Get("id")));
            });
        }

        public Service(
            string Prefixe,
            Action Service) :
            this(Prefixe, null, Service)
        { }

        public void Start()
        {
            MyService.Start();
            trd_Service = new System.Threading.Thread(() =>
            {
                while (true)
                {

                    var c = MyService.GetContext();
                    new System.Threading.Thread(() =>
                    {
                        Request.Ready(c);
                        Response();
                    }).Start();
                }
            });
            trd_Service.Start();
        }

        public void Stop()
        {
            trd_Service.Abort();
            MyService.Stop();
            trd_Service = null;
        }

        private void Response()
        {
            try
            {

                JavaScript.js.SendJS = BeginSendJs;
                Responsor rs = this;
                for (int Step = 0; Step < Request.Steps.Length; Step++)
                {
                    var Index = Array.BinarySearch(rs.Childs,
                            new ResponsorData() { RequestName = Request.Steps[Step] });
                    if (Index < 0)
                    {
                        Request.MyContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        Request.MyContext.Response.OutputStream.Flush(); // not found
                        Request.MyContext.Response.OutputStream.Close();
                        System.Threading.Thread.CurrentThread.Abort();
                    }
                    rs = rs.Childs[Index].Responsor;
                    rs.Way?.Invoke();
                    Request.FlushScript();
                }
                rs.Way?.Invoke();
                Request.FlushScript();
                rs.Service?.Invoke();
                Request.FlushScript();
                Request.MyContext.Response.OutputStream.Flush();
                Request.MyContext.Response.OutputStream.Close();
            }
            catch (System.Net.Sockets.SocketException)
            {
                System.Threading.Thread.CurrentThread.Abort();
            }
        }

        internal static void BeginSendJs(string Script)
        {
            Request.Response("<html><body><script>");
            JavaScript.js.SendJS = CountiniueSendJs;
            var Main = JavaScript.js.Declare[() => { }];
            JavaScript.js.CurrentFunction = Main;
            Main.WaitForHandle();
            CountiniueSendJs(Script);
        }

        internal static void CountiniueSendJs(string Script)
        {
            try
            {
                try
                {
                    Request.MyContext.Response.ContentType = "text/html; charset=UTF-8";
                }
                catch { }
                byte[] Buffer = System.Text.Encoding.UTF8.GetBytes(Script);
                Request.MyContext.Response.OutputStream.Write(Buffer, 0, Buffer.Length);
                Request.MyContext.Response.OutputStream.Flush();
            }
            catch { Request.CloseService(); }
        }
    }

    public class ResponsorData :
        IComparable<ResponsorData>
    {
        public Responsor Responsor;
        public string RequestName;

        public int CompareTo(ResponsorData other)
        {
            return RequestName.CompareTo(other.RequestName);
        }
    }

    public class Responsor
    {
        public Action Way;
        public Action Service;
        public ResponsorData[] Childs = new ResponsorData[0];
        public event Action<System.Net.EndPoint> ClientMisstake;

        public Responsor(Action Way,
                         Action Service)
        {
            this.Service = Service;
            this.Way = Way;
            this.ClientMisstake += ClientMisstake;
        }

        public Responsor AddService(
            string ServiceName,
            Action Way,
            Action Service)
        {
            var Response = new ResponsorData()
            {
                RequestName = ServiceName,
                Responsor = new Responsor(Way, Service)
            };

            ArrayExtentions.BinaryInsert(ref Childs, Response);
            return Response.Responsor;
        }

        public Responsor AddService(
            string ServiceName,
            Action Service)
        {
            var Response = new ResponsorData()
            {
                RequestName = ServiceName,
                Responsor = new Responsor(null, Service)
            };

            ArrayExtentions.BinaryInsert(ref Childs, Response);
            return Response.Responsor;
        }

        public Responsor<DataType> AddService<DataType>(
            string ServiceName,
            Func<DataType> Way,
            Action<DataType> Service)
        {
            var Response = new ResponsorData()
            {
                RequestName = ServiceName,
                Responsor = new Responsor(
                    () => Responsor<DataType>.Data = Way(),
                    () => Service(Responsor<DataType>.Data))
            };

            ArrayExtentions.BinaryInsert(ref Childs, Response);
            return new Responsor<DataType>(Response.Responsor);
        }
    }

    public class Responsor<DataType>
    {
        [ThreadStaticAttribute]
        public static DataType Data;

        public Responsor(Responsor MyResponsor)
        {
            this.MyResponsor = MyResponsor;
        }

        public Responsor MyResponsor;

        public Responsor AddService(string ServiceName,
            Action<DataType> Way,
            Action<DataType> Service)
        {
            return MyResponsor.AddService(
                ServiceName,
                () => Way(Data),
                () => Service(Data));
        }

        public Responsor AddService(string ServiceName,
            Action<DataType> Service)
        {
            return MyResponsor.AddService(
                ServiceName,
                () => Service(Data));
        }

        public Responsor<NewDataType> AddService<NewDataType>(
            string ServiceName,
            Func<DataType, NewDataType> Way,
            Action<NewDataType> Service)
        {
            return MyResponsor.AddService(
                ServiceName,
                () => Way(Data), Service);
        }
    }
}
