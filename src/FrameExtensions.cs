using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Spearing.Utilities.Data.Frames
{
    public static class FrameExtensions
    {

        #region Stuff to implement
        //// TODO - finish this!
        //public static Frame Merge(this Frame frame, params Frame[] frames)
        //{
        //    Frame newFrame = new Frame();

        //    return newFrame;
        //}

        //public static void RenameColumn(this Frame frame, string oldName, string newName)
        //{
        //    int index = frame.Columns.IndexOf(frame[oldName]);
        //    frame[newName] = frame[oldName].Duplicate();
        //    frame.DeleteColumn(oldName);

        //    frame.MoveColumn(frame[newName], index);
        //    //frame.Columns.Remove(frame[newName]);
        //    //frame.Columns.Insert(index, frame[newName]);
        //}

        //> summary(my.data)
        //   name age            hgt wgt            race year
        // Barb:1   Min.   :18.0   Min.   :64.0   Min.   : 128.0   Af.Am:1   Fr:2  
        // Bob :1   1st Qu.:18.0   1st Qu.:66.0   1st Qu.: 156.0   Asian:1   Jr:1  
        // Fred:1   Median :20.0   Median :67.0   Median : 180.0   Cauc :2   So:1  
        // Jeff:1   Mean   :20.2   Mean   :67.8   Mean   : 356.8   NA's :1   Sr:1  
        // Sue :1   3rd Qu.:21.0   3rd Qu.:70.0   3rd Qu.: 202.0                   
        //          Max.   :24.0   Max.   :72.0   Max.   :1118.0  
        // TODO - code this!
        //public static string Summary(this Frame frame)
        //{
        //    throw new Exception("Not done yet");
        //}

        //public static string Str(this Frame frame)
        //{
        //    throw new Exception("Not done yet");
        //}
        #endregion


            /// <summary>
            /// Returns a list of the previous rows based on a count
            /// </summary>
            /// <param name="row"></param>
            /// <param name="count"></param>
            /// <returns></returns>
        public static List<Row> PreviousRows(this Row row, int count)
        {
            var index = row.Index - count;
            if (index < 0)
                return null;

            return row.Frame.GetRange(index, count);
        }

        /// <summary>
        /// Returns a previous row a specified count back
        /// </summary>
        /// <param name="row"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Row Previous(this Row row, int count)
        {
            var index = row.Index - count;
            if (index < 0)
                return null;

            return row.Index == 0 ? null : row.Frame[index];
        }

        /// <summary>
        /// Returns the previous row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static Row Previous(this Row row)
        {
            return row.Index == 0 ? null : row.Frame[row.Index - 1];
        }

        /// <summary>
        /// Returns the next row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static Row Next(this Row row)
        {
            return (row.Index +1 )>= row.Frame.Count ? null : row.Frame[row.Index + 1];
        }

        private static string MakeStr(object value, Type type)
        {
            char quoteChar = '"';
            string quote = Char.ToString(quoteChar);

            if (type == typeof(string)
                || type == typeof(char)
                || type == typeof(object)
                || type == typeof(DateTime)
                || type == typeof(DateTime?))
            {
                return quote + value?.ToString().Replace(quote, new String(quoteChar, 2)) + quote;
            }
            else if (type == typeof(bool)
                || type == typeof(byte)
                || type == typeof(sbyte)
                || type == typeof(decimal)
                || type == typeof(double)
                || type == typeof(float)
                || type == typeof(int)
                || type == typeof(uint)
                || type == typeof(long)
                || type == typeof(ulong)
                || type == typeof(short)
                || type == typeof(ushort)
            )
            {
                return value.ToString();
            }
            else if (type == typeof(bool?)
                || type == typeof(byte?)
                || type == typeof(sbyte?)
                || type == typeof(char?)
                || type == typeof(decimal?)
                || type == typeof(double?)
                || type == typeof(float?)
                || type == typeof(int?)
                || type == typeof(uint?)
                || type == typeof(long?)
                || type == typeof(ulong?)
                || type == typeof(short?)
                || type == typeof(ushort?)
            )
            {
                return value?.ToString();
            }
            else
            {
                return quote + value?.ToString().Replace(quote, new String(quoteChar, 2)) + quote;
            }
        }        
        
        /// <summary>
        /// Saves a frame a CSV file
        /// </summary>
        /// <param name="frame">Frame that contains the data</param>
        /// <param name="path">Path and file name the text file should be saved to / as</param>
        /// <param name="showHeader">Whether the header row is included</param>
        /// <param name="delimiter">String delimiter; comma by default</param>
        public static void SaveCsv(this Frame frame, string path, bool showHeader = true, string delimiter = ",")
        {
            var csv = new StringBuilder();

            string quote = Char.ToString('"');

            if (showHeader)
                csv.AppendLine(String.Join(delimiter, frame.Columns.Select(col => quote + col.Name + quote)));

            Type[] types = frame.Types();//.Columns.Select(col => col.GetSubType()).ToArray();

            frame.ForEach(row =>
            {
                var rowStr = GetCsv(row, types, delimiter);
                csv.AppendLine(rowStr);
            });

            File.WriteAllText(path, csv.ToString());
        }

        /// <summary>
        /// Returns an array of delimited strings
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string[] GetCsvs(this IEnumerable<Row> rows, string delimiter = ",")
        {
            var rowArr = rows.ToArray();

            if (rowArr.Length == 0)
                return new string[] { };

            var frame = rows.First().Frame;
            Type[] types = frame.Types();//.Columns.Select(col => col.GetSubType()).ToArray();

            return rows
                .Select(row => GetCsv(row, types, delimiter))
                .ToArray();
        }

        /// <summary>
        /// Retrurns the delimited string for a row
        /// </summary>
        /// <param name="row"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string GetCsv(this Row row, string delimiter = ",")
        {
            var frame = row.Frame;
            Type[] types = frame.Types();//.Columns.Select(col => col.GetSubType()).ToArray();

            return GetCsv(row, types, delimiter);
        }
        private static string GetCsv(Row row, Type[] types, string delimiter = ",")
        {
            var values = row.Values;
            var rowStr = String.Join(delimiter, values.Select((val, i) => MakeStr(val, types[i])));
            return rowStr;
        }

        private static Type[] Types(this Frame frame)
        {
            return frame.Columns.Select(col => col.GetSubType()).ToArray();
        }


        private static string ToLineString(this Row row, List<int> maxes, IColumn[] columns)
        {
            return String.Join("",
                columns
                .Select((col, i) =>
                {
                    var value = col.Value(row.Index);

                    //return (value == null ? String.Empty : value).ToString().PadLeft(maxes[i]);
                    return (value == null ? "<NA>" : value).ToString().PadLeft(maxes[i]);
                })
            );
        }

        public static bool IsNa(this Row row, string colName)
        {
            //row.Frame[colName].GetSubType()
            return row.Get<object>(colName) == null;
        }

        

        private static Column ToColumn<T>(this IEnumerable<T> list, Type t)
        {
            if (t == typeof(bool)) return new Column(new Column<bool>(list.Cast<bool>()));
            else if (t == typeof(DateTime)) return new Column(new Column<DateTime>(list.Cast<DateTime>()));
            else if (t == typeof(byte)) return new Column(new Column<byte>(list.Cast<byte>()));
            else if (t == typeof(sbyte)) return new Column(new Column<sbyte>(list.Cast<sbyte>()));
            else if (t == typeof(char)) return new Column(new Column<char>(list.Cast<char>()));
            else if (t == typeof(decimal)) return new Column(new Column<decimal>(list.Cast<decimal>()));
            else if (t == typeof(double)) return new Column(new Column<double>(list.Cast<double>()));
            else if (t == typeof(float)) return new Column(new Column<float>(list.Cast<float>()));
            else if (t == typeof(int)) return new Column(new Column<int>(list.Cast<int>()));
            else if (t == typeof(uint)) return new Column(new Column<uint>(list.Cast<uint>()));
            else if (t == typeof(long)) return new Column(new Column<long>(list.Cast<long>()));
            else if (t == typeof(ulong)) return new Column(new Column<ulong>(list.Cast<ulong>()));
            else if (t == typeof(object)) return new Column(new Column<object>(list.Cast<object>()));
            else if (t == typeof(short)) return new Column(new Column<short>(list.Cast<short>()));
            else if (t == typeof(ushort)) return new Column(new Column<ushort>(list.Cast<ushort>()));
            else if (t == typeof(string)) return new Column(new Column<string>(list.Cast<string>()));
            else if (t == typeof(bool?)) return new Column(new Column<bool?>(list.Cast<bool?>()));
            else if (t == typeof(DateTime?)) return new Column(new Column<DateTime?>(list.Cast<DateTime?>()));
            else if (t == typeof(byte?)) return new Column(new Column<byte?>(list.Cast<byte?>()));
            else if (t == typeof(sbyte?)) return new Column(new Column<sbyte?>(list.Cast<sbyte?>()));
            else if (t == typeof(char?)) return new Column(new Column<char?>(list.Cast<char?>()));
            else if (t == typeof(decimal?)) return new Column(new Column<decimal?>(list.Cast<decimal?>()));
            else if (t == typeof(double?)) return new Column(new Column<double?>(list.Cast<double?>()));
            else if (t == typeof(float?)) return new Column(new Column<float?>(list.Cast<float?>()));
            else if (t == typeof(int?)) return new Column(new Column<int?>(list.Cast<int?>()));
            else if (t == typeof(uint?)) return new Column(new Column<uint?>(list.Cast<uint?>()));
            else if (t == typeof(long?)) return new Column(new Column<long?>(list.Cast<long?>()));
            else if (t == typeof(ulong?)) return new Column(new Column<ulong?>(list.Cast<ulong?>()));
            else if (t == typeof(short?)) return new Column(new Column<short?>(list.Cast<short?>()));
            else if (t == typeof(ushort?)) return new Column(new Column<ushort?>(list.Cast<ushort?>()));
            else return new Column(new Column<T>(list));
        }

        /// <summary>
        /// Returns the typed value for a column / row by column name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T Get<T>(this Row row, string name)
        {
            var col = row.Frame[name] as Column;

            return (col.Col as Column<T>)[row.Index];
            //return (row.Frame[name] as Column<T>)[row.Index];
        }

        /// <summary>
        /// Returns the typed value for a column / row by column index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T Get<T>(this Row row, int index)
        {
            var col = row.Frame.Columns.ElementAt(index);
            return (col.Col as Column<T>)[row.Index];
        }

        /// <summary>
        /// Sets the value for a column
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        public static void Set<T>(this Row row, string columnName, T value)
        {
            var column = row.Frame[columnName].As<T>();
            column[row.Index] = value;
            //this.Frame.Column<T>(columnName)[this.Index] = value;
        }


        /// <summary>
        /// Returns a typed column collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <returns></returns>
        public static Column<T> As<T>(this Column col)
        {
            return col.Col as Column<T>;
        }

        //public static Column<decimal> Round(this Column<decimal> column, int round)
        //{
        //    return column.Select(value => Math.Round(value, round)).ToColumn();
        //}
        //public static Column<double> Round(this Column<double> column, int round)
        //{
        //    return column.Select(value => Math.Round(value, round)).ToColumn();
        //}

        internal static IEnumerable<T> Get<T>(this IEnumerable<T> columns, params string[] names)
            where T : IColumn
        {
            return columns
                .Where(col => names.Contains(col.Name));
        }

        internal static Frame AsFrame<T>(this IEnumerable<T> columns)
            where T : IColumn
        {
            var frame = new Frame();

            columns.ToList()
                .ForEach(col => frame[col.Name] = col.Duplicate());

            return frame;
        }

        /// <summary>
        /// Returns a typed column based on the values passed in
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Column col<T>(params T[] values)
        {
            return values.ToColumn();
        }
        //public static Column<T> col<T>(params T[] values)
        //{
        //    return values.ToColumn();
        //}

        /// <summary>
        /// Returns a typed column based on IEnumerable<T> list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Column ToColumn<T>(this IEnumerable<T> list)
        {
            var col = new Column<T>(list);
            return new Column(col);
        }

        //public static Column<T> ToColumn<T>(this IEnumerable<T> list)
        //{
        //    return new Column<T>(list);
        //}

        //public static Column ToColumn(this IColumn column)
        //{
        //    return new Column(column);
        //}


        /// <summary>
        /// Returns a new frame based on a subset of columns from an existing frame
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        public static Frame SubFrame(this Frame frame, params string[] columnNames)
        {
            return frame
                .Columns
                .Get(columnNames)
                .AsFrame();
        }




        //public static IEnumerable<Row> Set<T>(this IEnumerable<Row> rows, string columnName, T value)
        //{
        //    var rowList = rows.ToList();

        //    foreach(var row in rowList)
        //    {
        //        row.SetValue(columnName, value);
        //    }
        //    return rowList;
        //}

        //public static IEnumerable<Row> Set<T>(this IEnumerable<Row> rows, string columnName, Func<Row, T> func)
        //{
        //    var rowList = rows.ToList();

        //    foreach (var row in rows.ToList())
        //    {
        //        row.SetValue(columnName, func(row));
        //    }
        //    return rowList;
        //}


        /// <summary>
        /// Melts a frame
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="keys"></param>
        /// <param name="columns"></param>
        /// <param name="variableColName"></param>
        /// <param name="valueColName"></param>
        /// <returns></returns>
        public static Frame Melt(this Frame frame, string[] keys, string[] columns, string variableColName = "Variable", string valueColName = "Value")
        {
            // # rows = frame.Count * columns.Length
            // # columns = keys.Length + 2 (variable, value)

            if (keys.Length == 0)
                throw new Exception("keys must contain one or more column names");

            if (columns.Length == 0)
                throw new Exception("columns must contain one or more column name");

            Frame newFrame = new Frame();

            foreach (var key in keys)
                newFrame[key] = frame[key].DuplicateThis(columns.Length);

            newFrame[variableColName] = 
                columns
                .SelectMany(col => Enumerable.Range(0, newFrame.Count / columns.Length).Select(i => col))
                .ToColumn();

            Column merged = null;
            foreach(var column in columns)
            {
                merged = merged == null ? frame[column] : merged.UnionThis(frame[column]);
            }

            newFrame[valueColName] = merged;

            return newFrame;
        }

        /// <summary>
        /// Melts a frame
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="key"></param>
        /// <param name="columns"></param>
        /// <param name="variableColName"></param>
        /// <param name="valueColName"></param>
        /// <returns></returns>
        public static Frame Melt(this Frame frame, string key, string[] columns, string variableColName = "Variable", string valueColName = "Value")
        {
            return frame.Melt(
                new string[] { key },
                columns,
                variableColName,
                valueColName
            );
        }

        /// <summary>
        /// Returns a typed array based on the parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T[] c<T>(params T[] values)
        {
            return values.ToArray();
        }

        /// <summary>
        /// Returns a frame based on a typed collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Frame ToFrame<T>(this IEnumerable<T> values)
        {
            // TODO - improve this?
            if (values is T[] == false && values is List<T> == false)
                values = values.ToArray();

            PropertyInfo[] properties = null;
            properties = values.GetType().GetProperties();

            Frame frame = new Frame();

            var t = typeof(T);

            var typeProps = t.GetProperties().ToList();

            if (typeProps.Count == 0)
            {
                frame["Column1"] = values.ToColumn();
                return frame;
            }

            typeProps
                .ForEach(p =>
                {
                    frame[p.Name] = values.Select(pp => p.GetValue(pp)).ToColumn(p.PropertyType);
                });

            return frame;
        }

        /// <summary>
        /// Returns a frame from a collection of rows
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static Frame ToFrame(this IEnumerable<Row> rows)
        {
            Frame frame = new Frame();

            if (rows == null)
                return frame;

            rows = rows.ToArray();
            if (rows.Count() == 0)
                return frame;

            var indexes = rows.Select(row => row.Index).ToList();

            // Copy each column from the old frame but only the rows
            Frame oldFrame = rows.First().Frame;

            foreach (var col in oldFrame.Columns)
                frame[col.Name] = col.DuplicateThis(indexes);

            //oldFrame.Columns
            //    .ForEach(col => frame[col.Name] = col.Duplicate(indexes));

            return frame;
        }

        /// <summary>
        /// Performs a SQL styled inner join of two frames
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="leftColumnName"></param>
        /// <param name="rightColumnName"></param>
        /// <returns></returns>
        public static Frame Join<T>(this Frame left, Frame right, string leftColumnName, string rightColumnName = "")
        {
            if (String.IsNullOrEmpty(rightColumnName))
                rightColumnName = leftColumnName;

            Frame joinedFrame = left.Join(
                right,
                row => row.Get<T>(leftColumnName),
                row => row.Get<T>(rightColumnName)
            );

            return joinedFrame;
        }

        /// <summary>
        /// Performs a SQL styled inner join of two frames
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="getLeftKey"></param>
        /// <param name="getRightKey"></param>
        /// <returns></returns>
        public static Frame Join<T>(this Frame left, Frame right, Func<Row, T> getLeftKey, Func<Row, T> getRightKey = null)
        {
            if (getRightKey == null)
                getRightKey = getLeftKey;

            //Func<Row, Row, Tuple<int, int>> selector = (r1, r2) => new Tuple<int, int>(r1.Index, r2.Index);

            IEnumerable<Tuple<int, int>> joined = left.Join(right, getLeftKey, getRightKey, (r1, r2) => new Tuple<int, int>(r1.Index, r2.Index));

            var leftIndexes = joined.Select(i => i.Item1).ToList();
            var rightIndexes = joined.Select(i => i.Item2).ToList();

            Frame joinedFrame = new Frame();
            var leftNames = left.Columns.Select(col => col.Name).ToArray();
            var rightNames = right.Columns.Select(col => col.Name).ToArray();

            foreach (var col in left.Columns)
            {
                string leftName = rightNames.Contains(col.Name) ? "Left." + col.Name : col.Name;
                //string leftName = col.Name;
                joinedFrame[leftName] = left[col.Name].DuplicateThis(leftIndexes);
            }

            foreach (var col in right.Columns)
            {
                string rightName = leftNames.Contains(col.Name) ? "Right." + col.Name : col.Name;
                joinedFrame[rightName] = right[col.Name].DuplicateThis(rightIndexes);

                //if (!leftNames.Contains(col.Name))
                //    joinedFrame[col.Name] = right[col.Name].DuplicateThis(rightIndexes);
            }

            return joinedFrame;
        }

        /// <summary>
        /// Performs a SQL styled outer join of two frames
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="getLeftKey"></param>
        /// <param name="getRightKey"></param>
        /// <returns></returns>
        public static Frame OuterJoin<T>(this Frame left, Frame right, Func<Row, T> getLeftKey, Func<Row, T> getRightKey = null)
        {
            if (getRightKey == null)
                getRightKey = getLeftKey;

            //Func<Row, Row, Tuple<int, int>> selector = (x, y) => new Tuple<int, int>(x.Left.Index, y == null ? -1 : y.Index);

            IEnumerable<Tuple<int,int>> joined = left.GroupJoin(right, getLeftKey, getRightKey, (x,y) => new { Left = x, Right = y })
                .SelectMany(x => x.Right.DefaultIfEmpty(),
                    (x, y) => new Tuple<int, int>(x.Left.Index, y == null ? -1 : y.Index));

            var leftIndexes = joined.Select(i => i.Item1).ToList();
            var rightIndexes = joined.Select(i => i.Item2).ToList();

            Frame joinedFrame = new Frame();
            var leftNames = left.Columns.Select(col => col.Name).ToArray();
            var rightNames = right.Columns.Select(col => col.Name).ToArray();

            foreach (var col in left.Columns)
            {
                string leftName = rightNames.Contains(col.Name) ? "Left." + col.Name : col.Name;
                joinedFrame[leftName] = left[col.Name].DuplicateThis(leftIndexes);
            }

            foreach (var col in right.Columns)
            {
                string rightName = leftNames.Contains(col.Name) ? "Right." + col.Name : col.Name;
                joinedFrame[rightName] = right[col.Name].DuplicateThis(rightIndexes);
            }

            return joinedFrame;
        }


        //public static Frame Join<T>(this Frame left, Frame right, string[] leftColumns, string[] rightColumns = null)
        //{
        //    if (rightColumns == null)
        //        rightColumns = leftColumns;

        //    if (leftColumns.Length == 0)
        //        throw new Exception("join columns must be defined");

        //    if (leftColumns.Length != rightColumns.Length)
        //        throw new Exception("left and right column counts must match");

        //    // if any of the field names are the same prefix them with the frame name / side


        //    //leftColumns.Join()



        //    return null;
        //}



        // TODO - add the row number
        private static string[] Lines(this IEnumerable<Row> frameRows, string[] columnNames)
        {
            var rows = frameRows.ToList();

            if (rows.Count == 0)
                return new string[] { };

            var frame = rows.First().Frame;

            Column[] columns = 
                frame.Columns
                    .Where(col => columnNames.Contains(col.Name))
                    .ToArray();
            
            // TODO - what if the column name is much longer than max value?
            List<int> maxes =
                Enumerable.Range(0, columns.Length)
                .Select(i => rows.SelectMany(row =>
                {
                    int index = frame.IndexOf(row);
                    //object value = columns[i].Value(row.Index);
                    object value = columns[i].Value(index);
                    return new List<int>() { value == null ? 0 : value.ToString().Length, columns[i].Name.Length };
                }).Max() + 3)
                .ToList();

            int indexMax = rows.Max(row => row.Index).ToString().Length + 3;

            string header = 
                "".PadLeft(indexMax)
                + String.Join("",
                    columns
                        .Select((col, i) => col.Name.PadLeft(maxes[i]))
                    );

            var list = new List<string>();

            list.Add(header);

            list.AddRange(rows.Select(row => 
                row.Index.ToString().PadLeft(indexMax)
                + row.ToLineString(maxes, columns)));

            return list.ToArray();
        }




        /// <summary>
        /// Returns the first number of rows or all rows excluding last n
        /// </summary>
        /// <param name="rows">Frame / rows to be filtered</param>
        /// <param name="n">If positive, returns the first n rows. If negative, returns all rows less the n number of rows at the end.</param>
        /// <returns></returns>
        public static IEnumerable<T> Head<T>(this IEnumerable<T> rows, int n = 6)
        {
            if (n >= 0)
                return rows.Take(n);
            else
            {
                var r = rows.ToArray();
                return rows.Take(r.Length + n);
            }
        }

        /// <summary>
        /// Returns the last number of rows or all rows excluding first n
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rows"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static IEnumerable<T> Tail<T>(this IEnumerable<T> rows, int n = 6)
        {
            var r = rows.ToArray();
            if (n >= 0)
                return r.Reverse().Take(n).Reverse();
            else
            {
                return rows.Reverse().Take(r.Length + n).Reverse();
            }
        }


        /// <summary>
        /// Returns the text for collection of rows
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static string Text(this IEnumerable<Row> rows, params string[] columns)
        {
            StringBuilder sb = new StringBuilder();

            if (rows == null || rows.Count() == 0)
            {
                return "No data";
            }

            if (columns.Length == 0)
                columns = rows.First().Frame.Columns.Select(col => col.Name).ToArray();

            foreach(var line in rows.Lines(columns))
                sb.AppendLine(line);

            return sb.ToString();
        }




        //public static IEnumerable<Row> OrderBy(this IEnumerable<Row> rows, string columnName)
        //{
        //    return rows.OrderBy(row => )
        //}

        //public static Frame Subframe(this IEnumerable<Row> rows, params string[] columnNames)
        //{
        //    if (rows == null)
        //        return null;

        //    rows = rows.ToArray();

        //    Frame oldFrame = rows.Count() == 0 ? null : rows.First().Frame;

        //    var columns =
        //        oldFrame.Columns
        //            .Where(col => columnNames.Contains(col.Name));

        //    Frame frame = new Frame();
        //    foreach (var col in columns)
        //        frame[col.Name] = col;

        //    return frame;
        //}

        //public static Frame Slice(this IEnumerable<Row> rows, params string[] columns)
        //{
        //    if (rows == null || rows.Count() == 0 || columns.Length == 0)
        //        return null;

        //    var oldFrame = rows.First().Frame;
        //    var cols = oldFrame.Columns.Where(col => columns.Contains(col.Name)).ToList();

        //    var newFrame = new Frame();

        //    cols.ForEach(col => newFrame[col.Name] = col.Duplicate());

        //    return newFrame;
        //}


        /// <summary>
        /// Displays the data in a frame or collectin of rows
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        public static void Print(this IEnumerable<Row> rows, params string[] columns)
        {
            string text = rows.Text(columns);
            Console.Write(text);
        }

        /// <summary>
        /// Displays the data in a column
        /// </summary>
        /// <param name="column"></param>
        public static void Print(this IColumn column)
        {
            Console.Write(column.ToString());
        }
    }
}
