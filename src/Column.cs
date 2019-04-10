using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spearing.Utilities.Data.Frames
{
    public class Column : IColumn
    {
        internal IColumn Col { get; }

        public int Count { get { return this.Col.Count; } }
        public string Name {
            get { return this.Col.Name; }
            set { this.Col.Name = value; }
        }

        public Type GetSubType() { return this.Col.GetSubType(); }

        public Column DuplicateThis(List<int> indexes) { return this.Col.Duplicate(indexes); }
        internal Column DuplicateThis(int count) { return this.Col.Duplicate(count); }
        internal Column UnionThis(Column column) { return this.Col.Union(column); }

        Column IColumn.Duplicate(List<int> indexes) { return this.Col.Duplicate(indexes); }
        Column IColumn.Duplicate() { return this.Col.Duplicate(); }
        Column IColumn.Duplicate(int count) { return this.Col.Duplicate(count); }
        Column IColumn.Union(Column column) { return this.Col.Union(column); }

        public object Value(int index) { return this.Col.Value(index); }


        public static implicit operator Column(List<bool> col) { return new Column(new Column<bool>(col)); }
        public static implicit operator Column(List<DateTime> col) { return new Column(new Column<DateTime>(col)); }
        public static implicit operator Column(List<byte> col) { return new Column(new Column<byte>(col)); }
        public static implicit operator Column(List<sbyte> col) { return new Column(new Column<sbyte>(col)); }
        public static implicit operator Column(List<char> col) { return new Column(new Column<char>(col)); }
        public static implicit operator Column(List<decimal> col) { return new Column(new Column<decimal>(col)); }
        public static implicit operator Column(List<double> col) { return new Column(new Column<double>(col)); }
        public static implicit operator Column(List<float> col) { return new Column(new Column<float>(col)); }
        public static implicit operator Column(List<int> col) { return new Column(new Column<int>(col)); }
        public static implicit operator Column(List<uint> col) { return new Column(new Column<uint>(col)); }
        public static implicit operator Column(List<long> col) { return new Column(new Column<long>(col)); }
        public static implicit operator Column(List<ulong> col) { return new Column(new Column<ulong>(col)); }
        public static implicit operator Column(List<object> col) { return new Column(new Column<object>(col)); }
        public static implicit operator Column(List<short> col) { return new Column(new Column<short>(col)); }
        public static implicit operator Column(List<ushort> col) { return new Column(new Column<ushort>(col)); }
        public static implicit operator Column(List<string> col) { return new Column(new Column<string>(col)); }
        public static implicit operator Column(List<bool?> col) { return new Column(new Column<bool?>(col)); }
        public static implicit operator Column(List<DateTime?> col) { return new Column(new Column<DateTime?>(col)); }
        public static implicit operator Column(List<byte?> col) { return new Column(new Column<byte?>(col)); }
        public static implicit operator Column(List<sbyte?> col) { return new Column(new Column<sbyte?>(col)); }
        public static implicit operator Column(List<char?> col) { return new Column(new Column<char?>(col)); }
        public static implicit operator Column(List<decimal?> col) { return new Column(new Column<decimal?>(col)); }
        public static implicit operator Column(List<double?> col) { return new Column(new Column<double?>(col)); }
        public static implicit operator Column(List<float?> col) { return new Column(new Column<float?>(col)); }
        public static implicit operator Column(List<int?> col) { return new Column(new Column<int?>(col)); }
        public static implicit operator Column(List<uint?> col) { return new Column(new Column<uint?>(col)); }
        public static implicit operator Column(List<long?> col) { return new Column(new Column<long?>(col)); }
        public static implicit operator Column(List<ulong?> col) { return new Column(new Column<ulong?>(col)); }
        public static implicit operator Column(List<short?> col) { return new Column(new Column<short?>(col)); }
        public static implicit operator Column(List<ushort?> col) { return new Column(new Column<ushort?>(col)); }


        public static implicit operator Column(bool[] col) { return new Column(new Column<bool>(col)); }
        public static implicit operator Column(DateTime[] col) { return new Column(new Column<DateTime>(col)); }
        public static implicit operator Column(byte[] col) { return new Column(new Column<byte>(col)); }
        public static implicit operator Column(sbyte[] col) { return new Column(new Column<sbyte>(col)); }
        public static implicit operator Column(char[] col) { return new Column(new Column<char>(col)); }
        public static implicit operator Column(decimal[] col) { return new Column(new Column<decimal>(col)); }
        public static implicit operator Column(double[] col) { return new Column(new Column<double>(col)); }
        public static implicit operator Column(float[] col) { return new Column(new Column<float>(col)); }
        public static implicit operator Column(int[] col) { return new Column(new Column<int>(col)); }
        public static implicit operator Column(uint[] col) { return new Column(new Column<uint>(col)); }
        public static implicit operator Column(long[] col) { return new Column(new Column<long>(col)); }
        public static implicit operator Column(ulong[] col) { return new Column(new Column<ulong>(col)); }
        public static implicit operator Column(object[] col) { return new Column(new Column<object>(col)); }
        public static implicit operator Column(short[] col) { return new Column(new Column<short>(col)); }
        public static implicit operator Column(ushort[] col) { return new Column(new Column<ushort>(col)); }
        public static implicit operator Column(string[] col) { return new Column(new Column<string>(col)); }
        public static implicit operator Column(bool?[] col) { return new Column(new Column<bool?>(col)); }
        public static implicit operator Column(DateTime?[] col) { return new Column(new Column<DateTime?>(col)); }
        public static implicit operator Column(byte?[] col) { return new Column(new Column<byte?>(col)); }
        public static implicit operator Column(sbyte?[] col) { return new Column(new Column<sbyte?>(col)); }
        public static implicit operator Column(char?[] col) { return new Column(new Column<char?>(col)); }
        public static implicit operator Column(decimal?[] col) { return new Column(new Column<decimal?>(col)); }
        public static implicit operator Column(double?[] col) { return new Column(new Column<double?>(col)); }
        public static implicit operator Column(float?[] col) { return new Column(new Column<float?>(col)); }
        public static implicit operator Column(int?[] col) { return new Column(new Column<int?>(col)); }
        public static implicit operator Column(uint?[] col) { return new Column(new Column<uint?>(col)); }
        public static implicit operator Column(long?[] col) { return new Column(new Column<long?>(col)); }
        public static implicit operator Column(ulong?[] col) { return new Column(new Column<ulong?>(col)); }
        public static implicit operator Column(short?[] col) { return new Column(new Column<short?>(col)); }
        public static implicit operator Column(ushort?[] col) { return new Column(new Column<ushort?>(col)); }


        //public static implicit operator Column(Column<T> col)
        //{

        //}


        internal Column(IColumn col)
        {
            this.Col = col;
        }

        public override string ToString()
        {
            return this.Col.ToString();
        }
    }


    public class Column<T> : List<T>, IColumn
    {
        public string Name { get; set; }

        public Type GetSubType() { return typeof(T); }

        public Column(IEnumerable<T> list) : base(list) { }

        /// <summary>
        /// Returns the value at the index as an object
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object Value(int index)
        {
            return this[index];
        }


        Column IColumn.Duplicate(List<int> indexes)
        {
            IEnumerable<T> list = indexes.Select(i => i == -1 ? default(T) : this[i]);
            var c = new Column<T>(list)
            {
                Name = this.Name
            };
            return new Column(c);
        }

        Column IColumn.Duplicate()
        {
            IEnumerable<T> list = this.Select(i => i);
            var c = new Column<T>(list)
            {
                Name = this.Name
            };
            return new Column(c);
        }

        Column IColumn.Duplicate(int count)
        {
            IEnumerable<T> list = 
                Enumerable.Range(0, count)
                .SelectMany(x => this.Select(i => i));

            var c = new Column<T>(list)
            {
                Name = this.Name
            };
            return new Column(c);
        }

        Column IColumn.Union(Column column)
        {
            var toMerge = column.Col as Column<T>;
            var merged = this.Union(toMerge);

            var c = new Column<T>(merged)
            {
                Name = this.Name
            };
            return new Column(c);
        }



        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine(this.Name);

            var max = this.Max(value => value == null ? 0 : value.ToString().Length + 3);
            max = Math.Max(max, this.Name.Length + 3);

            sb.AppendLine(this.Name.PadLeft(max));

            this.ForEach(value =>
            {
                var str = value == null ? "".PadLeft(max) : value.ToString().PadLeft(max);
                sb.AppendLine(str);
            });

            return sb.ToString();
        }


        #region Addition Column Operators

        // Addition

        public static Column<T> operator +(Column<T> first, Column<T> second)
        {
            var values = first.Select((value, i) =>
            {
                dynamic val1 = value;
                dynamic val2 = second[i];
                return (T)(val1 + val2);
            }
            ).ToList();
            return new Column<T>(values) { };
        }



        public static Column<T> operator +(Column<T> first, IEnumerable<T> second)
        {
            var values = first.Select((value, i) =>
            {
                dynamic val1 = value;
                dynamic val2 = second.ElementAt(i);
                return (T)(val1 + val2);
            }
            ).ToList();

            return new Column<T>(values);
        }


        public static Column<T> operator +(Column<T> first, T second)
        {
            var values =
                first.Select(value =>
                {
                    dynamic val1 = value;
                    dynamic val2 = second;
                    return (T)(val1 + val2);
                });
            return new Column<T>(values);
        }

        #endregion



        #region Multiplication Column Operators

        public static Column<T> operator *(Column<T> first, Column<T> second)
        {
            var values = first.Select((value, i) =>
            {
                dynamic val1 = value;
                dynamic val2 = second[i];
                return (T)(val1 * val2);
            }
            ).ToList();
            return new Column<T>(values) { };
        }



        public static Column<T> operator *(Column<T> first, IEnumerable<T> second)
        {
            var values = first.Select((value, i) =>
            {
                dynamic val1 = value;
                dynamic val2 = second.ElementAt(i);
                return (T)(val1 * val2);
            }
            ).ToList();

            return new Column<T>(values);
        }

        public static Column<T> operator *(Column<T> first, T second)
        {
            var values =
                first.Select(value =>
                {
                    dynamic val1 = value;
                    dynamic val2 = second;
                    return (T)(val1 * val2);
                });
            return new Column<T>(values);
        }

        #endregion



        #region Division Column Operators

        public static Column<T> operator /(Column<T> first, Column<T> second)
        {
            var values = first.Select((value, i) =>
            {
                dynamic val1 = value;
                dynamic val2 = second[i];
                return (T)(val1 / val2);
            }
            ).ToList();
            return new Column<T>(values) { };
        }

        public static Column<T> operator /(Column<T> first, IEnumerable<T> second)
        {
            var values = first.Select((value, i) =>
            {
                dynamic val1 = value;
                dynamic val2 = second.ElementAt(i);
                return (T)(val1 / val2);
            }
            ).ToList();

            return new Column<T>(values);
        }

        public static Column<T> operator /(Column<T> first, T second)
        {
            var values =
                first.Select(value =>
                {
                    dynamic val1 = value;
                    dynamic val2 = second;
                    return (T)(val1 / val2);
                });
            return new Column<T>(values);
        }

        #endregion



        #region Subtraction Column Operators



        public static Column<T> operator -(Column<T> first, Column<T> second)
        {
            var values = first.Select((value, i) =>
            {
                dynamic val1 = value;
                dynamic val2 = second[i];
                return (T)(val1 - val2);
            }
            ).ToList();
            return new Column<T>(values) { };
        }

        public static Column<T> operator -(Column<T> first, IEnumerable<T> second)
        {
            var values = first.Select((value, i) =>
            {
                dynamic val1 = value;
                dynamic val2 = second.ElementAt(i);
                return (T)(val1 - val2);
            }
            ).ToList();

            return new Column<T>(values);
        }

        public static Column<T> operator -(Column<T> first, T second)
        {
            var values =
                first.Select(value =>
                {
                    dynamic val1 = value;
                    dynamic val2 = second;
                    return (T)(val1 - val2);
                });
            return new Column<T>(values);
        }

        #endregion


    }


}
