using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Monsajem_Incs;
using Monsajem_Incs.HttpService.JavaScript;
using Monsajem_Incs.HttpService;
using System.IO;
using System.Reflection;

namespace TestOnIt
{
    static class Program
    {

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var Address = "http://127.0.0.1:8080/";

            var Service = new Monsajem_Incs.HttpService.Service(Address,
            () =>
            {
                js.Document.Append(js.Ajax.Get("/Files/HTMLPage.html"));

                js.Document.GetElementById("Btn_Done").OnClick.Set(() =>
                {
                    // Run your code in Server
                    js.RunAtServer(() =>
                    {
                        string StrValue = js.Document.GetElementById("TxtBox").Value;
                        var Files = js.Document.GetElementById("fileUploader").Files;
                        js.Document.GetElementById("Btn_Done").Value = "Action Done";
                    });
                });
            });

            Service.AddService("Pages", () =>
            {
                var _textStreamReader =
                    new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("TestOnIt.Files." + Request.Steps[1]));
                var Message = _textStreamReader.ReadToEnd();
                Request.Response(Message);
                Request.CloseService();
            }, null);

            Service.AddService("Files", () =>
            {
                var _textStreamReader = Assembly.GetExecutingAssembly().GetManifestResourceStream("TestOnIt.Files." + Request.Steps[1]);
                var Buffer = new byte[_textStreamReader.Length];
                _textStreamReader.Read(Buffer, 0, Buffer.Length);
                Request.Response(Buffer);
                Request.CloseService();
            }, null);
            Service.Start();

            System.Diagnostics.Process.Start(Address);

            Application.Run();
        }
    }
}
