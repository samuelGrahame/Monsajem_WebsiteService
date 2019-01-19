using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Monsajem_Incs.HttpService
{
    public class RequestInfo:
        IComparable<RequestInfo>
    {
        public Thread Thread;
        public HttpListenerContext Context;

        public int CompareTo(RequestInfo other)
        {
            var Result =this.Context.Request.RemoteEndPoint.Address.Address -
                         other.Context.Request.RemoteEndPoint.Address.Address;
            if (Result != 0)
                return (int)Result;

            Result = this.Thread.ManagedThreadId - other.Thread.ManagedThreadId;
            return (int)Result;
        }
    }
}
