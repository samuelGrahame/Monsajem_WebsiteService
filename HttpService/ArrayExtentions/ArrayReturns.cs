﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayExtentions
{
    public static class ArrayReturns
    {
        public static ref t first<t>(this t[] ar)
        {
            return ref ar[0];
        }

        public static ref t Last<t>(this t[] ar)
        {
            return ref ar[ar.Length - 1];
        }

        public static t[] From<t>(this t[] ar, int from)
        {
            var Result = new t[ar.Length - from];
            Array.Copy(ar, from, Result, 0, Result.Length);
            return Result;
        }
        public static t[] To<t>(this t[] ar, int to)
        {
            var Result = new t[to+1];
            Array.Copy(ar, 0, Result, 0, Result.Length);
            return Result;
        }

        public static t[] DeleteByPosition<t>(this t[] ar, int Position)
        {
            Array.Copy(ar, 0, ar, 0, Position);
            Array.Copy(ar, Position + 1, ar, Position, (ar.Length - Position) - 1);
            Array.Resize(ref ar, ar.Length - 1);
            return ar;
        }
        public static t[] DeleteFrom<t>(this t[] ar, int from)
        {
            Array.Resize(ref ar, from);
            return ar;
        }
        public static t[] DeleteFromTo<t>(this t[] ar, int from,int To)
        {
            Array.Copy(ar, To+1, ar, from, ar.Length-(To+1));
            Array.Resize(ref ar, ar.Length - (To - from + 1));
            return ar;
        }
        public static t[] DeleteTo<t>(this t[] ar, int To)
        {
            Array.Copy(ar, To+1, ar, 0, ar.Length - (To+1));
            Array.Resize(ref ar, ar.Length - (To+1));
            return ar;
        }

        public static t[] BinaryDelete<t>(this t[] ar, t Value)
        {
            var Place = Array.BinarySearch(ar, Value);
            if(Place >= 0)
            {
               ar=ar.DeleteByPosition(Place);
            }
            return ar;
        }

        public static t[] BinaryDelete<t>(this t[] ar, IEnumerable<t> Values)
        {
            foreach (var Value in Values)
               ar=ar.BinaryDelete(Value);
            return ar;
        }

        public static t[] BinaryInsert<t>(this t[] ar, t Value)
        {
            var Place = Array.BinarySearch(ar, Value);
            if (Place < 0)
                Place = (Place * -1) - 1;
            ar=ar.Insert(Value, Place);
            return ar;
        }
        public static t[] BinaryInsert<t>(this t[] ar, IEnumerable<t> Values)
        {
            foreach(var Value in Values)
            {
                ar = ar.BinaryInsert(Value);
            }
            return ar;
        }

        public static t[] Insert<t>(this t[] ar, t Value)
        {
            Array.Resize(ref ar, ar.Length + 1);
            ar[ar.Length - 1] = Value;
            return ar;
        }

        public static t[] Insert<t>(this t[] ar,params t[] Values)
        {
            var From = ar.Length;
            Array.Resize(ref ar, ar.Length + Values.Length);
            Array.Copy(Values, 0, ar, From, Values.Length);
            return ar;
        }
        public static t[] Insert<t>(this t[] ar, IEnumerable<t> Values)
        {
            var From = ar.Length;
            var Count = Values.Count();
            Array.Resize(ref ar, ar.Length + Count);
            var i = From;
            Count = ar.Length;
            var Reader = Values.GetEnumerator();
            Reader.MoveNext();
            while(i<Count)
            {
                ar[i] = Reader.Current;
                Reader.MoveNext();
                i++;
            }
            Reader.Dispose();
            return ar;
        }
        public static t[] Insert<t>(this t[] ar, t[] Values,int From)
        {
            var ArLen = ar.Length;
            Array.Resize(ref ar, ar.Length + Values.Length);
            Array.Copy(ar, From, ar, ArLen+1, ArLen - From);
            Array.Copy(Values, 0, ar, From, Values.Length);
            return ar;
        }
        public static t[] Insert<t>(this t[] ar, IEnumerable<t> Values, int From)
        {
            var ArLen = ar.Length;
            var Count = Values.Count();
            Array.Resize(ref ar, ar.Length + Count);
            Array.Copy(ar, From, ar, ArLen+1, ArLen - From);
            var i = From;
            Count = ar.Length;
            var Reader = Values.GetEnumerator();
            Reader.MoveNext();
            while (i < Count)
            {
                ar[i] = Reader.Current;
                Reader.MoveNext();
                i++;
            }
            Reader.Dispose();
            return ar;
        }
        public static t[] Insert<t>(this t[] ar, t Value, int Position)
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
            return ar;
        }

        public static t[] DropFromInsertTo<t>(this t[] ar, int From, int To, t Value)
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
            return ar;
        }
        public static t[] DropFromInsertTo<t>(this t[] ar, int From, int To)
        {
            var Value = ar[From];
            if (From < To)
            {
                Array.Copy(ar, From + 1, ar, From, (To - From));
            }
            else if (From > To)
            {
                Array.Copy(ar, To, ar, To + 1, (From - To));
            }
            ar[To] = Value;
            return ar;
        }
    }
}
