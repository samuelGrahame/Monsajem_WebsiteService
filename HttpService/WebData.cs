using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monsajem_Incs.HttpService
{
    [Serializable]
    public class WebData
    {
        public WebData(string Name,string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
        public string Name;
        public string Value;

        public static WebData[] GetData(string Data)
        {
            var WebDatas = new WebData[0];
            if (Data.Length > 0)
            {
                var IndexEqual = Data.IndexOf("=");
                var IndexAnd = Data.IndexOf("&");
                while (IndexAnd > 0)
                {
                    ArrayExtentions.Insert(ref WebDatas,
                        new WebData(
                            Data.Substring(0, IndexEqual),
                            WebToStringParse(Data.Substring(IndexEqual + 1, (IndexAnd - IndexEqual) - 1))));
                    Data = Data.Substring(IndexAnd + 1);
                    IndexEqual = Data.IndexOf("=");
                    IndexAnd = Data.IndexOf("&");
                }
                ArrayExtentions.Insert(ref WebDatas,
                        new WebData(
                            Data.Substring(0, IndexEqual),
                            WebToStringParse(Data.Substring(IndexEqual + 1))));
            }
            return WebDatas;
        }

        public static string WebToStringParse(string value)
        {
            var Result = "";
            for (int i = 0; i < value.Length; i++)
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
                        if (!(i < value.Length))
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
            for (int i = HexValue.Length - 1; i != -1; i--)
            {
                var CurrentValue =
                    System.Text.Encoding.UTF8.GetBytes(HexValue[i].ToString())[0] - BaseValue;
                if (CurrentValue > 15)
                    CurrentValue -= 7;
                CurrentValue = CurrentValue * (int)System.Math.Pow(16, HexValue.Length - 1 - i);
                Result = Result + CurrentValue;
            }
            return Result;
        }

        private static string DecimalToHex(int Value)
        {
            if (Value < 9)
                return Value.ToString();
            var Result = "";
            int BaseValue = System.Text.Encoding.UTF8.GetBytes("0")[0] + 7;
            while (Value > 9)
            {
                var CurrentValue = Value % 16;
                Value = Value / 16;

                if (CurrentValue < 10)
                    Result = CurrentValue.ToString() + Result;
                else
                    Result = System.Text.Encoding.UTF8.GetString(
                                new byte[] { (byte)(CurrentValue + BaseValue) }) +
                                Result;
            }
            if (Value > 0)
                Result = Value.ToString() + Result;
            return Result;
        }

    }
}
