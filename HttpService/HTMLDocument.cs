using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Monsajem_Incs.HttpService
{
    public class HTMLTag
    {
        public string Name;
        public Dictionary<string, string> Options = new Dictionary<string, string>();
        public string InnerText;
        public List<HTMLTag> InnerTags = new List<HTMLTag>();


        public static HTMLTag[] Parse(string HTMLDocument)
        {

            return HTMLDocument.ToStructrure<char, HTMLTag[]>(
                (c) =>
                {
                    var Documents = new HTMLTag[0];

                    while(true)
                    {
                        c.Data = new string(c.Data.ToArray()).Trim();

                        if (c.Data.Count() == 0)
                            return Documents;
                        else if (new string(c.Data.Take(2).ToArray()) == "</")
                        {
                            c.Data = c.Data.Skip(2);
                            return Documents;
                        }

                        if (c.Data.First().ToString() != "<")
                            throw new FormatException("Invalid Format");

                        ArrayExtentions.Insert(ref Documents, new HTMLTag());
                        var Document = Documents[Documents.Length - 1];


                        c.Data = c.Data.Skip(1);

                        Document.Name =
                            new string(c.Data.
                                TakeWhile((q) => q.ToString() != " " &
                                                 q.ToString() != ">" &
                                                 q.ToString() != "/").ToArray());
                        c.Data = c.Data.Skip(Document.Name.Length);
                        c.Data = new string(c.Data.ToArray()).Trim();

                        if (c.Data.First().ToString() == "/")
                            c.Data = c.Data.Skip(2);
                        else if (c.Data.First().ToString() == ">")
                        {
                            c.Data = c.Data.Skip(1);
                            Document.InnerTags.AddRange(c.Repliy());

                            var EndName = new string(c.Data.TakeWhile((q) => q.ToString() != ">").ToArray());

                            if (EndName != Document.Name)
                                return Documents;
                            c.Data = c.Data.Skip(EndName.Length);
                        }
                        else
                        {
                            while (c.Data.First().ToString() != ">" &
                                   c.Data.First().ToString() != "/")
                            {
                                var OptionName =
                                       new string(c.Data.TakeWhile((q) => q.ToString() != "=").ToArray());
                                c.Data = c.Data.Skip(OptionName.Length + 2);

                                var OptionValue = new string(c.Data.TakeWhile((q) => q.ToString() != "\"").ToArray());

                                Document.Options.Add(OptionName, OptionValue);

                                c.Data = c.Data.Skip(OptionValue.Length+1);
                                c.Data = new string(c.Data.ToArray()).Trim();
                            }

                            if (c.Data.First().ToString() == ">")
                            {
                                c.Data = c.Data.Skip(1);
                                Document.InnerTags.AddRange(c.Repliy());

                                var EndName = new string(c.Data.TakeWhile((q) => q.ToString() != ">").ToArray());

                                if (EndName != Document.Name)
                                    return Documents;
                                c.Data = c.Data.Skip(EndName.Length);
                            }
                            else
                                c.Data = c.Data.Skip(2);
                        }
                    }
                });
        }
    }
}
