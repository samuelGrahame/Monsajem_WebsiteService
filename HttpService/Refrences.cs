using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monsajem_Incs.HttpService
{
    internal static class ArrayExtentions
    {
        public static void Delete<t>(ref t[] ar, int Position)
        {
            if (Position == ar.Length - 1)
            {
                Array.Resize(ref ar, Position);
                return;
            }
            Array.Copy(ar, 0, ar, 0, Position);
            Array.Copy(ar, Position + 1, ar, Position, (ar.Length - Position) - 1);
            Array.Resize(ref ar, ar.Length - 1);
        }

        public static int BinaryDelete<t>(ref t[] ar, t Value)
        {
            var Place = Array.BinarySearch(ar, Value);
            if (Place >= 0)
            {
                Delete(ref ar, Place);
            }
            return Place;
        }

        public static int BinaryInsert<t>(ref t[] ar, t Value)
        {
            var Place = Array.BinarySearch(ar, Value);
            if (Place < 0)
                Place = (Place * -1) - 1;
            Insert(ref ar, Value, Place);
            return Place;
        }

        public static void Insert<t>(ref t[] ar, t Value)
        {
            Array.Resize(ref ar, ar.Length + 1);
            ar[ar.Length - 1] = Value;
        }

        public static void Insert<t>(ref t[] ar, t[] Value)
        {
            var From = ar.Length;
            Array.Resize(ref ar, ar.Length + Value.Length);
            Array.Copy(Value, 0, ar, From, Value.Length);
        }

        public static void DropFromInsertTo<t>(ref t[] ar, int From, int To, t Value)
        {
            if (From < To)
            {
                Array.Copy(ar, From + 1, ar, From, (To - From));
            }
            else if (From > To)
            {
                Array.Copy(ar, To, ar, To + 1, (From - To));
            }
            ar[To] = Value;
        }

        public static void Insert<t>(ref t[] ar, t Value, int Position)
        {
            Array.Resize(ref ar, ar.Length + 1);
            if (Position == ar.Length - 1)
                ar[ar.Length - 1] = Value;
            else
            {
                Array.Copy(
                    ar, Position,
                    ar, Position + 1, ar.Length - Position - 1);
                ar[Position] = Value;
            }
        }
    }
    internal static class Serializations
    {
        public static byte[] SerializationDelegate(this Delegate obj)
        {
            var Result = new byte[0];

            var Method = obj.Method.Serialization();

            ArrayExtentions.Insert(ref Result,
                System.BitConverter.GetBytes(Method.Length));

            ArrayExtentions.Insert(ref Result, Method);

            var STarger = obj.Target.DynamicSerialization();

            ArrayExtentions.Insert(ref Result,
                System.BitConverter.GetBytes(STarger.Length));

            ArrayExtentions.Insert(ref Result, STarger);

            return Result;
        }

        public class MethodInfo
        {
            public System.Reflection.MethodInfo Method;
            public object Target;
        }

        public static MethodInfo DeserializeDelegate(this byte[] arrBytes)
        {
            var from = 4;
            var Len = System.BitConverter.ToInt32(arrBytes, 0);
            var Method = new byte[Len];
            Array.Copy(arrBytes, from, Method, 0, Len);
            from += Len;

            Len = System.BitConverter.ToInt32(arrBytes, from);
            from += 4;
            var BTarget = new byte[Len];
            Array.Copy(arrBytes, from, BTarget, 0, Len);
            var Target = DynamicDeserialize(BTarget);
            var ms = Target.GetType().GetMethods();
            return new MethodInfo()
            {
                Method = (System.Reflection.MethodInfo)
                                                    Method.Deserialize(),
                Target = Target
            };
        }

        public static byte[] DynamicSerialization<t>(this t obj)
            where t : new()
        {
            var Result = new byte[0];
            var Type = obj.GetType();
            var Filds = Type.GetFields();

            ArrayExtentions.Insert(ref Result, System.BitConverter.GetBytes(Type.Assembly.FullName.Length * 2));
            ArrayExtentions.Insert(ref Result,Encoding.Unicode.GetBytes(Type.Assembly.FullName));

            ArrayExtentions.Insert(ref Result, System.BitConverter.GetBytes(Type.FullName.Length * 2));
            ArrayExtentions.Insert(ref Result, Encoding.Unicode.GetBytes(Type.FullName));

            for (int i = 0; i < Filds.Length; i++)
            {
                var Sfield = Filds[i].GetValue(obj).Serialization();
                ArrayExtentions.Insert(ref Result, System.BitConverter.GetBytes(Sfield.Length));
                ArrayExtentions.Insert(ref Result, Sfield);
            }

            return Result;
        }

        public static object DynamicDeserialize(this byte[] arrBytes)
        {
            object Result;
            var from = 4;
            var Len = System.BitConverter.ToInt32(arrBytes, 0);
            var AName = Encoding.Unicode.GetString(arrBytes, from, Len);
            from += Len;
            Len = System.BitConverter.ToInt32(arrBytes, from);
            from += 4;
            var Tname = Encoding.Unicode.GetString(arrBytes, from, Len);
            from += Len;
            Result = Activator.CreateInstance(AName, Tname).Unwrap();

            var Filds = Result.GetType().GetFields();

            for (int i = 0; i < Filds.Length; i++)
            {
                var SField = new byte[System.BitConverter.ToInt32(arrBytes, from)];
                from += 4;
                Array.Copy(arrBytes, from, SField, 0, SField.Length);
                Filds[i].SetValue(Result, SField.Deserialize());
                from += SField.Length;
            }
            return Result;
        }

        public static byte[] Serialization<t>(this t obj)
        {
            if (obj == null)
                return new byte[0];
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static Object Deserialize(this byte[] arrBytes)
        {
            if (arrBytes.Length == 0)
                return null;

            System.IO.MemoryStream memStream = new System.IO.MemoryStream();
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binForm =
                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, System.IO.SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);

            return obj;
        }

        public static Boolean ExactEqual<t>(this t obj1, t obj2)
        {
            var s1 = obj1.Serialization();
            var s2 = obj2.Serialization();
            if (s1.Length != s2.Length)
                return false;
            for (int i = 0; i < s1.Length; i++)
                if (s1[i] != s2[i])
                    return false;
            return true;
        }

        public static t Copy<t>(this t obj)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bf.Serialize(ms, obj);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                return (t)bf.Deserialize(ms);
            }
        }
    }

    public static class LinqEx
    {
        public static void Browse<t>(this IEnumerable<t> Table, Action<t> Browser)
        {
            foreach (t Value in Table)
            {
                Browser(Value);
            }
        }

        public class StructrureInfo<t, r>
        {
            public StructrureInfo(
                Func<StructrureInfo<t, r>, r> Function,
                IEnumerable<t> Data)
            {
                this.Function = Function;
                this.Data = Data;
            }

            public IEnumerable<t> Data;
            private Func<StructrureInfo<t, r>, r> Function;

            public r Repliy()
            {
                return Data.ToStructrure(Function);
            }
        }

        public static r ToStructrure<t, r>(this IEnumerable<t> Table,
            Func<StructrureInfo<t, r>, r> Function)
        {
            return Function(new StructrureInfo<t, r>(Function, Table));
        }
    }
}
