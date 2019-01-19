using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monsajem_Incs.HttpService
{
    public class MultyPartData
    {

        public string Name { get => ContentDisposition.Where((c) => c.Name == "name").First().Value; }
        public WebData[] ContentDisposition;
        public string ContentType;
        public byte[] Data;

        public static MultyPartData[] CreateFileData(System.IO.Stream Stream,string Token)
        {
            var Results = new MultyPartData[0];
            while (ReadLine(Stream).Substring(Token.Length) != "--")
            {
                var result = new MultyPartData();
                {
                    var cd = new WebData[0];
                    var From = 32;
                    var ContentDisposition = ReadLine(Stream);
                    while(From<ContentDisposition.Length)
                    {
                        var ToIndex = ContentDisposition.IndexOf("=",From);
                        var Name = ContentDisposition.Substring(From, ToIndex - From);
                        From = ToIndex + 2;
                        ToIndex = ContentDisposition.IndexOf("\"",From);
                        var Value = ContentDisposition.Substring(From, ToIndex - From);
                        From = ToIndex + 3;
                        ArrayExtentions.Insert(ref cd, new WebData(Name, Value));
                    }
                    result.ContentDisposition = cd;
                }
                result.ContentType = ReadLine(Stream);
                if(result.ContentType!="")
                {
                    ReadLine(Stream);
                }       
                {
                    var Name = result.ContentDisposition.Where((c) => c.Name == "name").First();
                    var Len = Name.Value.Substring(0, Name.Value.IndexOf(";"));
                    if (Len == "")
                        throw new FormatException("multipart/form-data Is not valid for us" +
                            System.Environment.NewLine + "please use our framework.");
                    Name.Value = Name.Value.Substring(Len.Length+1);
                    var DataLen = int.Parse(Len);
                    var Buffer = new byte[DataLen];
                    var Readed = Stream.Read(Buffer, 0, Buffer.Length);
                    while (Readed < Buffer.Length)
                    {
                        Readed += Stream.Read(Buffer, Readed, Buffer.Length - Readed);
                    }
                    ReadLine(Stream);
                    result.Data = Buffer;
                }
                ArrayExtentions.Insert(ref Results,result);
            }
            return Results;
        }

        private static string ReadLine(System.IO.Stream stream)
        {
            int next_char;
            string data = "";
            while (true)
            {
                next_char = stream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { System.Threading.Thread.Sleep(1); continue; };
                data += Convert.ToChar(next_char);
            }
            return data;
        }
    }
}
