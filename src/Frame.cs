using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using System.IO;
using System.Net;

namespace Spearing.Utilities.Data.Frames
{
    public class Frame : List<Row>
    {

        #region Stuff to implement

        //// TODO - implement
        //public void MoveColumn(IColumn column, int index)
        //{
            
        //}

        #endregion


        private Dictionary<string, Column> columnDictionary { get; set; } = new Dictionary<string, Column>();

        // TODO - improve this
        /// <summary>
        /// Returns the columns in the frame
        /// </summary>
        public Column[] Columns { get { return this.columnDictionary.Select(c => c.Value).ToArray(); } }

        /// <summary>
        /// Returns a typed column based on it's index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public Column<T> Column<T>(int columnIndex)
        {
            return this.Columns[columnIndex] as Column<T>;
        }

        /// <summary>
        /// Returns a typed columm based on it's name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public Column<T> Column<T>(string columnIndex)
        {
            return this.columnDictionary[columnIndex] as Column<T>;
        }

        /// <summary>
        /// Returns whether a frame contains a specific column
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsColumn(string name)
        {
            return this.columnDictionary.ContainsKey(name);
        }


        public IEnumerable<Row> this[Func<Row, bool> test]
        {
            get
            {
                return this.Where(test);
            }
        }


        /// <summary>
        /// Returns a column based on it's name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Column this[string name]
        {
            get
            {
                return this.columnDictionary[name];
            }
            set
            {
                if (this.Count == 0)
                {
                    var rows = Enumerable.Range(0, value.Count)
                        .Select(i => new Row(this) { Index = i })
                        .ToList();
                    this.AddRange(rows);
                }
                else if (this.Count != value.Count)
                {
                    throw new Exception("Row counts don't match: " + this.Count + ", " + value.Count);
                }

                value.Name = name;

                if (this.columnDictionary.ContainsKey(name))
                    this.columnDictionary[name] = value;
                else
                    this.columnDictionary.Add(name, value);
            }
        }

        //public Frame this[params string[] columnNames]
        //{
        //    get
        //    {
        //        var columns =
        //            this.Columns
        //                .Where(col => columnNames.Contains(col.Name));

        //        Frame frame = new Frame();
        //        foreach (var col in columns)
        //            frame[col.Name] = col;

        //        return frame;
        //    }
        //}

        /// <summary>
        /// Returns the value for a row and column index combination
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public object this[int row, int column]
        {
            get
            {
                var col = this.columnDictionary.ElementAt(column);
                //return this[row].

                return col.Value.Value(row);
            }
        }

        /// <summary>
        /// Deletes a column based on it's name
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public Frame DeleteColumn(string columnName)
        {
            this.columnDictionary.Remove(columnName);
            return this;
        }

        //private static string[] GetColumnNames(string[] columns, int count)
        //{
        //    int missing = count - columns.Length;
        //    var missingCols = Enumerable.Range(columns.Length + 1, missing).Select(i => "Column " + i.ToString()).AsEnumerable();
        //    return columns.Union(missingCols).ToArray();
        //}


        public new void Add(Row row)
        {
            //base.Add()
            throw new Exception("Not supported yet");
        }

        public new void RemoveAt(int index)
        {

        }

        #region Read CSV


        private static string[] GetHeaderNames(string[] userNames, string[] csvNames, bool hasHeader)
        {
            return userNames
                .Select((userName, i) =>
                {
                    if (hasHeader)
                    {
                        var csvName = csvNames.ElementAtOrDefault(i);

                        if (String.IsNullOrEmpty(userName) == false)
                            return userName;
                        else if (string.IsNullOrEmpty(csvName) == false)
                            return csvName;
                        else
                            return "Column" + i.ToString();
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(userName) == false)
                            return userName;
                        else
                            return "Column" + i.ToString();
                    }
                }).ToArray();
        }

        private static bool IsUrl(string path)
        {
            return (Uri.IsWellFormedUriString(path, UriKind.Absolute));
        }

        private static string[] ReadCsv(string path, CsvHelper.Configuration.CsvConfiguration config, Action<CsvReader> doSomething)
        {
            bool isUrl = IsUrl(path);

            if (isUrl)
            {
                var webRequest = WebRequest.Create(path);
                using (WebResponse response = webRequest.GetResponse())
                using (Stream content = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(content))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        doSomething(csv);
                    }

                    return csv.FieldHeaders;
                }
            }
            else
            {
                using (StreamReader reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, config))
                {
                    if (config.HasHeaderRecord)
                        csv.ReadHeader();

                    while (csv.Read())
                    {
                        doSomething(csv);
                    }

                    if (config.HasHeaderRecord)
                        return csv.FieldHeaders;
                    else
                        return new string[] { };
                }
            }
        }

        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="path"></param>
        /// <param name="colName1"></param>
        /// <param name="hasHeader"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static Frame ReadCSV<A>(string path, string colName1 = "", bool hasHeader = true, string delimiter = ",")
        {

            var fld0 = new List<A>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1 },
                fieldNames,
                hasHeader);

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));

            return frame;
        }

        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        public static Frame ReadCSV<A, B>(string path, string colName1 = "", string colName2 = "", bool hasHeader = true, string delimiter = ",")
        {

            var fld0 = new List<A>();
            var fld1 = new List<B>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                    fld1.Add(csv.GetField<B>(1));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1, colName2 },
                fieldNames,
                hasHeader);

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));
            frame[columnNames[1]] = new Column(new Column<B>(fld1));

            return frame;
        }
        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        public static Frame ReadCSV<A, B, C>(string path, string colName1 = "", string colName2 = "", string colName3 = "", bool hasHeader = true, string delimiter = ",")
        {

            var fld0 = new List<A>();
            var fld1 = new List<B>();
            var fld2 = new List<C>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                    fld1.Add(csv.GetField<B>(1));
                    fld2.Add(csv.GetField<C>(2));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1, colName2, colName3},
                fieldNames,
                hasHeader);

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));
            frame[columnNames[1]] = new Column(new Column<B>(fld1));
            frame[columnNames[2]] = new Column(new Column<C>(fld2));

            return frame;
        }
        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        public static Frame ReadCSV<A, B, C, D>(string path, string colName1 = "", string colName2 = "", string colName3 = "", string colName4 = "", bool hasHeader = true, string delimiter = ",")
        {

            var fld0 = new List<A>();
            var fld1 = new List<B>();
            var fld2 = new List<C>();
            var fld3 = new List<D>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                    fld1.Add(csv.GetField<B>(1));
                    fld2.Add(csv.GetField<C>(2));
                    fld3.Add(csv.GetField<D>(3));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1, colName2, colName3, colName4 },
                fieldNames,
                hasHeader);

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));
            frame[columnNames[1]] = new Column(new Column<B>(fld1));
            frame[columnNames[2]] = new Column(new Column<C>(fld2));
            frame[columnNames[3]] = new Column(new Column<D>(fld3));

            return frame;
        }
        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        public static Frame ReadCSV<A, B, C, D, E>(string path, string colName1 = "", string colName2 = "", string colName3 = "", string colName4 = "", string colName5 = "", bool hasHeader = true, string delimiter = ",")
        {
            var fld0 = new List<A>();
            var fld1 = new List<B>();
            var fld2 = new List<C>();
            var fld3 = new List<D>();
            var fld4 = new List<E>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                    fld1.Add(csv.GetField<B>(1));
                    fld2.Add(csv.GetField<C>(2));
                    fld3.Add(csv.GetField<D>(3));
                    fld4.Add(csv.GetField<E>(4));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1, colName2, colName3, colName4, colName5 },
                fieldNames,
                hasHeader);

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));
            frame[columnNames[1]] = new Column(new Column<B>(fld1));
            frame[columnNames[2]] = new Column(new Column<C>(fld2));
            frame[columnNames[3]] = new Column(new Column<D>(fld3));
            frame[columnNames[4]] = new Column(new Column<E>(fld4));

            return frame;
        }
        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        public static Frame ReadCSV<A, B, C, D, E, F>(string path, string colName1 = "", string colName2 = "", string colName3 = "", string colName4 = "", string colName5 = "", string colName6 = "", bool hasHeader = true, string delimiter = ",")
        {

            var fld0 = new List<A>();
            var fld1 = new List<B>();
            var fld2 = new List<C>();
            var fld3 = new List<D>();
            var fld4 = new List<E>();
            var fld5 = new List<F>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                    fld1.Add(csv.GetField<B>(1));
                    fld2.Add(csv.GetField<C>(2));
                    fld3.Add(csv.GetField<D>(3));
                    fld4.Add(csv.GetField<E>(4));
                    fld5.Add(csv.GetField<F>(5));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1, colName2, colName3, colName4, colName5, colName6 },
                fieldNames,
                hasHeader);

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));
            frame[columnNames[1]] = new Column(new Column<B>(fld1));
            frame[columnNames[2]] = new Column(new Column<C>(fld2));
            frame[columnNames[3]] = new Column(new Column<D>(fld3));
            frame[columnNames[4]] = new Column(new Column<E>(fld4));
            frame[columnNames[5]] = new Column(new Column<F>(fld5));

            return frame;

        }

        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        public static Frame ReadCSV<A, B, C, D, E, F, G>(string path, string colName1 = "", string colName2 = "", string colName3 = "", string colName4 = "", string colName5 = "", string colName6 = "", string colName7 = "", bool hasHeader = true, string delimiter = ",")
        {
            var fld0 = new List<A>();
            var fld1 = new List<B>();
            var fld2 = new List<C>();
            var fld3 = new List<D>();
            var fld4 = new List<E>();
            var fld5 = new List<F>();
            var fld6 = new List<G>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                    fld1.Add(csv.GetField<B>(1));
                    fld2.Add(csv.GetField<C>(2));
                    fld3.Add(csv.GetField<D>(3));
                    fld4.Add(csv.GetField<E>(4));
                    fld5.Add(csv.GetField<F>(5));
                    fld6.Add(csv.GetField<G>(6));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1, colName2, colName3, colName4, colName5, colName6, colName7 },
                fieldNames,
                hasHeader);            

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));
            frame[columnNames[1]] = new Column(new Column<B>(fld1));
            frame[columnNames[2]] = new Column(new Column<C>(fld2));
            frame[columnNames[3]] = new Column(new Column<D>(fld3));
            frame[columnNames[4]] = new Column(new Column<E>(fld4));
            frame[columnNames[5]] = new Column(new Column<F>(fld5));
            frame[columnNames[6]] = new Column(new Column<G>(fld6));

            return frame;
        }

        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        public static Frame ReadCSV<A, B, C, D, E, F, G, H>(string path, string colName1 = "", string colName2 = "", string colName3 = "", string colName4 = "", string colName5 = "", string colName6 = "", string colName7 = "", string colName8 ="", bool hasHeader = true, string delimiter = ",")
        {

            var fld0 = new List<A>();
            var fld1 = new List<B>();
            var fld2 = new List<C>();
            var fld3 = new List<D>();
            var fld4 = new List<E>();
            var fld5 = new List<F>();
            var fld6 = new List<G>();
            var fld7 = new List<H>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                    fld1.Add(csv.GetField<B>(1));
                    fld2.Add(csv.GetField<C>(2));
                    fld3.Add(csv.GetField<D>(3));
                    fld4.Add(csv.GetField<E>(4));
                    fld5.Add(csv.GetField<F>(5));
                    fld6.Add(csv.GetField<G>(6));
                    fld7.Add(csv.GetField<H>(7));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1, colName2, colName3, colName4, colName5, colName6, colName7, colName8 },
                fieldNames,
                hasHeader);

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));
            frame[columnNames[1]] = new Column(new Column<B>(fld1));
            frame[columnNames[2]] = new Column(new Column<C>(fld2));
            frame[columnNames[3]] = new Column(new Column<D>(fld3));
            frame[columnNames[4]] = new Column(new Column<E>(fld4));
            frame[columnNames[5]] = new Column(new Column<F>(fld5));
            frame[columnNames[6]] = new Column(new Column<G>(fld6));
            frame[columnNames[7]] = new Column(new Column<H>(fld7));

            return frame;
        }

        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        public static Frame ReadCSV<A, B, C, D, E, F, G, H, I>(string path, string colName1 = "", string colName2 = "", string colName3 = "", string colName4 = "", string colName5 = "", string colName6 = "", string colName7 = "", string colName8 = "", string colName9="", bool hasHeader = true, string delimiter = ",")
        {

            var fld0 = new List<A>();
            var fld1 = new List<B>();
            var fld2 = new List<C>();
            var fld3 = new List<D>();
            var fld4 = new List<E>();
            var fld5 = new List<F>();
            var fld6 = new List<G>();
            var fld7 = new List<H>();
            var fld8 = new List<I>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                    fld1.Add(csv.GetField<B>(1));
                    fld2.Add(csv.GetField<C>(2));
                    fld3.Add(csv.GetField<D>(3));
                    fld4.Add(csv.GetField<E>(4));
                    fld5.Add(csv.GetField<F>(5));
                    fld6.Add(csv.GetField<G>(6));
                    fld7.Add(csv.GetField<H>(7));
                    fld8.Add(csv.GetField<I>(8));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1, colName2, colName3, colName4, colName5, colName6, colName7, colName8, colName9 },
                fieldNames,
                hasHeader);

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));
            frame[columnNames[1]] = new Column(new Column<B>(fld1));
            frame[columnNames[2]] = new Column(new Column<C>(fld2));
            frame[columnNames[3]] = new Column(new Column<D>(fld3));
            frame[columnNames[4]] = new Column(new Column<E>(fld4));
            frame[columnNames[5]] = new Column(new Column<F>(fld5));
            frame[columnNames[6]] = new Column(new Column<G>(fld6));
            frame[columnNames[7]] = new Column(new Column<H>(fld7));
            frame[columnNames[8]] = new Column(new Column<I>(fld8));

            return frame;
        }

        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        public static Frame ReadCSV<A, B, C, D, E, F, G, H, I, J>(string path, string colName1 = "", string colName2 = "", string colName3 = "", string colName4 = "", string colName5 = "", string colName6 = "", string colName7 = "", string colName8 = "", string colName9 = "", string colName10 ="", bool hasHeader = true, string delimiter = ",")
        {

            var fld0 = new List<A>();
            var fld1 = new List<B>();
            var fld2 = new List<C>();
            var fld3 = new List<D>();
            var fld4 = new List<E>();
            var fld5 = new List<F>();
            var fld6 = new List<G>();
            var fld7 = new List<H>();
            var fld8 = new List<I>();
            var fld9 = new List<J>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                    fld1.Add(csv.GetField<B>(1));
                    fld2.Add(csv.GetField<C>(2));
                    fld3.Add(csv.GetField<D>(3));
                    fld4.Add(csv.GetField<E>(4));
                    fld5.Add(csv.GetField<F>(5));
                    fld6.Add(csv.GetField<G>(6));
                    fld7.Add(csv.GetField<H>(7));
                    fld8.Add(csv.GetField<I>(8));
                    fld9.Add(csv.GetField<J>(9));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1, colName2, colName3, colName4, colName5, colName6, colName7, colName8, colName9, colName10 },
                fieldNames,
                hasHeader);

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));
            frame[columnNames[1]] = new Column(new Column<B>(fld1));
            frame[columnNames[2]] = new Column(new Column<C>(fld2));
            frame[columnNames[3]] = new Column(new Column<D>(fld3));
            frame[columnNames[4]] = new Column(new Column<E>(fld4));
            frame[columnNames[5]] = new Column(new Column<F>(fld5));
            frame[columnNames[6]] = new Column(new Column<G>(fld6));
            frame[columnNames[7]] = new Column(new Column<H>(fld7));
            frame[columnNames[8]] = new Column(new Column<I>(fld8));
            frame[columnNames[9]] = new Column(new Column<J>(fld9));

            return frame;
        }

        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        public static Frame ReadCSV<A, B, C, D, E, F, G, H, I, J, K>(string path, string colName1 = "", string colName2 = "", string colName3 = "", string colName4 = "", string colName5 = "", string colName6 = "", string colName7 = "", string colName8 = "", string colName9 = "", string colName10 = "", string colName11 = "", bool hasHeader = true, string delimiter = ",")
        {

            var fld0 = new List<A>();
            var fld1 = new List<B>();
            var fld2 = new List<C>();
            var fld3 = new List<D>();
            var fld4 = new List<E>();
            var fld5 = new List<F>();
            var fld6 = new List<G>();
            var fld7 = new List<H>();
            var fld8 = new List<I>();
            var fld9 = new List<J>();
            var fld10 = new List<K>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                    fld1.Add(csv.GetField<B>(1));
                    fld2.Add(csv.GetField<C>(2));
                    fld3.Add(csv.GetField<D>(3));
                    fld4.Add(csv.GetField<E>(4));
                    fld5.Add(csv.GetField<F>(5));
                    fld6.Add(csv.GetField<G>(6));
                    fld7.Add(csv.GetField<H>(7));
                    fld8.Add(csv.GetField<I>(8));
                    fld9.Add(csv.GetField<J>(9));
                    fld10.Add(csv.GetField<K>(10));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1, colName2, colName3, colName4, colName5, colName6, colName7, colName8, colName9, colName10, colName11 },
                fieldNames,
                hasHeader);

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));
            frame[columnNames[1]] = new Column(new Column<B>(fld1));
            frame[columnNames[2]] = new Column(new Column<C>(fld2));
            frame[columnNames[3]] = new Column(new Column<D>(fld3));
            frame[columnNames[4]] = new Column(new Column<E>(fld4));
            frame[columnNames[5]] = new Column(new Column<F>(fld5));
            frame[columnNames[6]] = new Column(new Column<G>(fld6));
            frame[columnNames[7]] = new Column(new Column<H>(fld7));
            frame[columnNames[8]] = new Column(new Column<I>(fld8));
            frame[columnNames[9]] = new Column(new Column<J>(fld9));
            frame[columnNames[10]] = new Column(new Column<K>(fld10));

            return frame;
        }


        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        public static Frame ReadCSV<A, B, C, D, E, F, G, H, I, J, K, L>(string path, string colName1 = "", string colName2 = "", string colName3 = "", string colName4 = "", string colName5 = "", string colName6 = "", string colName7 = "", string colName8 = "", string colName9 = "", string colName10 = "", string colName11 = "", string colName12 ="", bool hasHeader = true, string delimiter = ",")
        {

            var fld0 = new List<A>();
            var fld1 = new List<B>();
            var fld2 = new List<C>();
            var fld3 = new List<D>();
            var fld4 = new List<E>();
            var fld5 = new List<F>();
            var fld6 = new List<G>();
            var fld7 = new List<H>();
            var fld8 = new List<I>();
            var fld9 = new List<J>();
            var fld10 = new List<K>();
            var fld11 = new List<L>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                    fld1.Add(csv.GetField<B>(1));
                    fld2.Add(csv.GetField<C>(2));
                    fld3.Add(csv.GetField<D>(3));
                    fld4.Add(csv.GetField<E>(4));
                    fld5.Add(csv.GetField<F>(5));
                    fld6.Add(csv.GetField<G>(6));
                    fld7.Add(csv.GetField<H>(7));
                    fld8.Add(csv.GetField<I>(8));
                    fld9.Add(csv.GetField<J>(9));
                    fld10.Add(csv.GetField<K>(10));
                    fld11.Add(csv.GetField<L>(11));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1, colName2, colName3, colName4, colName5, colName6, colName7, colName8, colName9, colName10, colName11, colName12 },
                fieldNames,
                hasHeader);

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));
            frame[columnNames[1]] = new Column(new Column<B>(fld1));
            frame[columnNames[2]] = new Column(new Column<C>(fld2));
            frame[columnNames[3]] = new Column(new Column<D>(fld3));
            frame[columnNames[4]] = new Column(new Column<E>(fld4));
            frame[columnNames[5]] = new Column(new Column<F>(fld5));
            frame[columnNames[6]] = new Column(new Column<G>(fld6));
            frame[columnNames[7]] = new Column(new Column<H>(fld7));
            frame[columnNames[8]] = new Column(new Column<I>(fld8));
            frame[columnNames[9]] = new Column(new Column<J>(fld9));
            frame[columnNames[10]] = new Column(new Column<K>(fld10));
            frame[columnNames[11]] = new Column(new Column<L>(fld11));

            return frame;
        }

        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        public static Frame ReadCSV<A, B, C, D, E, F, G, H, I, J, K, L, M>(string path, string colName1 = "", string colName2 = "", string colName3 = "", string colName4 = "", string colName5 = "", string colName6 = "", string colName7 = "", string colName8 = "", string colName9 = "", string colName10 = "", string colName11 = "", string colName12 = "", string colName13 = "", bool hasHeader = true, string delimiter = ",")
        {

            var fld0 = new List<A>();
            var fld1 = new List<B>();
            var fld2 = new List<C>();
            var fld3 = new List<D>();
            var fld4 = new List<E>();
            var fld5 = new List<F>();
            var fld6 = new List<G>();
            var fld7 = new List<H>();
            var fld8 = new List<I>();
            var fld9 = new List<J>();
            var fld10 = new List<K>();
            var fld11 = new List<L>();
            var fld12 = new List<M>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                    fld1.Add(csv.GetField<B>(1));
                    fld2.Add(csv.GetField<C>(2));
                    fld3.Add(csv.GetField<D>(3));
                    fld4.Add(csv.GetField<E>(4));
                    fld5.Add(csv.GetField<F>(5));
                    fld6.Add(csv.GetField<G>(6));
                    fld7.Add(csv.GetField<H>(7));
                    fld8.Add(csv.GetField<I>(8));
                    fld9.Add(csv.GetField<J>(9));
                    fld10.Add(csv.GetField<K>(10));
                    fld11.Add(csv.GetField<L>(11));
                    fld12.Add(csv.GetField<M>(12));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1, colName2, colName3, colName4, colName5, colName6, colName7, colName8, colName9, colName10, colName11, colName12, colName13 },
                fieldNames,
                hasHeader);

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));
            frame[columnNames[1]] = new Column(new Column<B>(fld1));
            frame[columnNames[2]] = new Column(new Column<C>(fld2));
            frame[columnNames[3]] = new Column(new Column<D>(fld3));
            frame[columnNames[4]] = new Column(new Column<E>(fld4));
            frame[columnNames[5]] = new Column(new Column<F>(fld5));
            frame[columnNames[6]] = new Column(new Column<G>(fld6));
            frame[columnNames[7]] = new Column(new Column<H>(fld7));
            frame[columnNames[8]] = new Column(new Column<I>(fld8));
            frame[columnNames[9]] = new Column(new Column<J>(fld9));
            frame[columnNames[10]] = new Column(new Column<K>(fld10));
            frame[columnNames[11]] = new Column(new Column<L>(fld11));
            frame[columnNames[12]] = new Column(new Column<M>(fld12));

            return frame;
        }

        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        public static Frame ReadCSV<A, B, C, D, E, F, G, H, I, J, K, L, M, N>(string path, string colName1 = "", string colName2 = "", string colName3 = "", string colName4 = "", string colName5 = "", string colName6 = "", string colName7 = "", string colName8 = "", string colName9 = "", string colName10 = "", string colName11 = "", string colName12 = "", string colName13 = "", string colName14 = "", bool hasHeader = true, string delimiter = ",")
        {

            var fld0 = new List<A>();
            var fld1 = new List<B>();
            var fld2 = new List<C>();
            var fld3 = new List<D>();
            var fld4 = new List<E>();
            var fld5 = new List<F>();
            var fld6 = new List<G>();
            var fld7 = new List<H>();
            var fld8 = new List<I>();
            var fld9 = new List<J>();
            var fld10 = new List<K>();
            var fld11 = new List<L>();
            var fld12 = new List<M>();
            var fld13 = new List<N>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                    fld1.Add(csv.GetField<B>(1));
                    fld2.Add(csv.GetField<C>(2));
                    fld3.Add(csv.GetField<D>(3));
                    fld4.Add(csv.GetField<E>(4));
                    fld5.Add(csv.GetField<F>(5));
                    fld6.Add(csv.GetField<G>(6));
                    fld7.Add(csv.GetField<H>(7));
                    fld8.Add(csv.GetField<I>(8));
                    fld9.Add(csv.GetField<J>(9));
                    fld10.Add(csv.GetField<K>(10));
                    fld11.Add(csv.GetField<L>(11));
                    fld12.Add(csv.GetField<M>(12));
                    fld13.Add(csv.GetField<N>(13));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1, colName2, colName3, colName4, colName5, colName6, colName7, colName8, colName9, colName10, colName11, colName12, colName13, colName14 },
                fieldNames,
                hasHeader);

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));
            frame[columnNames[1]] = new Column(new Column<B>(fld1));
            frame[columnNames[2]] = new Column(new Column<C>(fld2));
            frame[columnNames[3]] = new Column(new Column<D>(fld3));
            frame[columnNames[4]] = new Column(new Column<E>(fld4));
            frame[columnNames[5]] = new Column(new Column<F>(fld5));
            frame[columnNames[6]] = new Column(new Column<G>(fld6));
            frame[columnNames[7]] = new Column(new Column<H>(fld7));
            frame[columnNames[8]] = new Column(new Column<I>(fld8));
            frame[columnNames[9]] = new Column(new Column<J>(fld9));
            frame[columnNames[10]] = new Column(new Column<K>(fld10));
            frame[columnNames[11]] = new Column(new Column<L>(fld11));
            frame[columnNames[12]] = new Column(new Column<M>(fld12));
            frame[columnNames[13]] = new Column(new Column<N>(fld13));

            return frame;
        }

        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        public static Frame ReadCSV<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(string path, string colName1 = "", string colName2 = "", string colName3 = "", string colName4 = "", string colName5 = "", string colName6 = "", string colName7 = "", string colName8 = "", string colName9 = "", string colName10 = "", string colName11 = "", string colName12 = "", string colName13 = "", string colName14 = "", string colName15 = "", bool hasHeader = true, string delimiter = ",")
        {

            var fld0 = new List<A>();
            var fld1 = new List<B>();
            var fld2 = new List<C>();
            var fld3 = new List<D>();
            var fld4 = new List<E>();
            var fld5 = new List<F>();
            var fld6 = new List<G>();
            var fld7 = new List<H>();
            var fld8 = new List<I>();
            var fld9 = new List<J>();
            var fld10 = new List<K>();
            var fld11 = new List<L>();
            var fld12 = new List<M>();
            var fld13 = new List<N>();
            var fld14 = new List<O>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                    fld1.Add(csv.GetField<B>(1));
                    fld2.Add(csv.GetField<C>(2));
                    fld3.Add(csv.GetField<D>(3));
                    fld4.Add(csv.GetField<E>(4));
                    fld5.Add(csv.GetField<F>(5));
                    fld6.Add(csv.GetField<G>(6));
                    fld7.Add(csv.GetField<H>(7));
                    fld8.Add(csv.GetField<I>(8));
                    fld9.Add(csv.GetField<J>(9));
                    fld10.Add(csv.GetField<K>(10));
                    fld11.Add(csv.GetField<L>(11));
                    fld12.Add(csv.GetField<M>(12));
                    fld13.Add(csv.GetField<N>(13));
                    fld14.Add(csv.GetField<O>(14));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1, colName2, colName3, colName4, colName5, colName6, colName7, colName8, colName9, colName10, colName11, colName12, colName13, colName14, colName15 },
                fieldNames,
                hasHeader);

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));
            frame[columnNames[1]] = new Column(new Column<B>(fld1));
            frame[columnNames[2]] = new Column(new Column<C>(fld2));
            frame[columnNames[3]] = new Column(new Column<D>(fld3));
            frame[columnNames[4]] = new Column(new Column<E>(fld4));
            frame[columnNames[5]] = new Column(new Column<F>(fld5));
            frame[columnNames[6]] = new Column(new Column<G>(fld6));
            frame[columnNames[7]] = new Column(new Column<H>(fld7));
            frame[columnNames[8]] = new Column(new Column<I>(fld8));
            frame[columnNames[9]] = new Column(new Column<J>(fld9));
            frame[columnNames[10]] = new Column(new Column<K>(fld10));
            frame[columnNames[11]] = new Column(new Column<L>(fld11));
            frame[columnNames[12]] = new Column(new Column<M>(fld12));
            frame[columnNames[13]] = new Column(new Column<N>(fld13));
            frame[columnNames[14]] = new Column(new Column<O>(fld14));

            return frame;
        }

        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        public static Frame ReadCSV<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>(string path, string colName1 = "", string colName2 = "", string colName3 = "", string colName4 = "", string colName5 = "", string colName6 = "", string colName7 = "", string colName8 = "", string colName9 = "", string colName10 = "", string colName11 = "", string colName12 = "", string colName13 = "", string colName14 = "", string colName15 = "", string colName16 = "", bool hasHeader = true, string delimiter = ",")
        {

            var fld0 = new List<A>();
            var fld1 = new List<B>();
            var fld2 = new List<C>();
            var fld3 = new List<D>();
            var fld4 = new List<E>();
            var fld5 = new List<F>();
            var fld6 = new List<G>();
            var fld7 = new List<H>();
            var fld8 = new List<I>();
            var fld9 = new List<J>();
            var fld10 = new List<K>();
            var fld11 = new List<L>();
            var fld12 = new List<M>();
            var fld13 = new List<N>();
            var fld14 = new List<O>();
            var fld15 = new List<P>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                    fld1.Add(csv.GetField<B>(1));
                    fld2.Add(csv.GetField<C>(2));
                    fld3.Add(csv.GetField<D>(3));
                    fld4.Add(csv.GetField<E>(4));
                    fld5.Add(csv.GetField<F>(5));
                    fld6.Add(csv.GetField<G>(6));
                    fld7.Add(csv.GetField<H>(7));
                    fld8.Add(csv.GetField<I>(8));
                    fld9.Add(csv.GetField<J>(9));
                    fld10.Add(csv.GetField<K>(10));
                    fld11.Add(csv.GetField<L>(11));
                    fld12.Add(csv.GetField<M>(12));
                    fld13.Add(csv.GetField<N>(13));
                    fld14.Add(csv.GetField<O>(14));
                    fld15.Add(csv.GetField<P>(15));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1, colName2, colName3, colName4, colName5, colName6, colName7, colName8, colName9, colName10, colName11, colName12, colName13, colName14, colName15, colName16 },
                fieldNames,
                hasHeader);

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));
            frame[columnNames[1]] = new Column(new Column<B>(fld1));
            frame[columnNames[2]] = new Column(new Column<C>(fld2));
            frame[columnNames[3]] = new Column(new Column<D>(fld3));
            frame[columnNames[4]] = new Column(new Column<E>(fld4));
            frame[columnNames[5]] = new Column(new Column<F>(fld5));
            frame[columnNames[6]] = new Column(new Column<G>(fld6));
            frame[columnNames[7]] = new Column(new Column<H>(fld7));
            frame[columnNames[8]] = new Column(new Column<I>(fld8));
            frame[columnNames[9]] = new Column(new Column<J>(fld9));
            frame[columnNames[10]] = new Column(new Column<K>(fld10));
            frame[columnNames[11]] = new Column(new Column<L>(fld11));
            frame[columnNames[12]] = new Column(new Column<M>(fld12));
            frame[columnNames[13]] = new Column(new Column<N>(fld13));
            frame[columnNames[14]] = new Column(new Column<O>(fld14));
            frame[columnNames[15]] = new Column(new Column<P>(fld15));

            return frame;
        }

        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        public static Frame ReadCSV<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q>(string path, string colName1 = "", string colName2 = "", string colName3 = "", string colName4 = "", string colName5 = "", string colName6 = "", string colName7 = "", string colName8 = "", string colName9 = "", string colName10 = "", string colName11 = "", string colName12 = "", string colName13 = "", string colName14 = "", string colName15 = "", string colName16 = "", string colName17 = "", bool hasHeader = true, string delimiter = ",")
        {

            var fld0 = new List<A>();
            var fld1 = new List<B>();
            var fld2 = new List<C>();
            var fld3 = new List<D>();
            var fld4 = new List<E>();
            var fld5 = new List<F>();
            var fld6 = new List<G>();
            var fld7 = new List<H>();
            var fld8 = new List<I>();
            var fld9 = new List<J>();
            var fld10 = new List<K>();
            var fld11 = new List<L>();
            var fld12 = new List<M>();
            var fld13 = new List<N>();
            var fld14 = new List<O>();
            var fld15 = new List<P>();
            var fld16 = new List<Q>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                    fld1.Add(csv.GetField<B>(1));
                    fld2.Add(csv.GetField<C>(2));
                    fld3.Add(csv.GetField<D>(3));
                    fld4.Add(csv.GetField<E>(4));
                    fld5.Add(csv.GetField<F>(5));
                    fld6.Add(csv.GetField<G>(6));
                    fld7.Add(csv.GetField<H>(7));
                    fld8.Add(csv.GetField<I>(8));
                    fld9.Add(csv.GetField<J>(9));
                    fld10.Add(csv.GetField<K>(10));
                    fld11.Add(csv.GetField<L>(11));
                    fld12.Add(csv.GetField<M>(12));
                    fld13.Add(csv.GetField<N>(13));
                    fld14.Add(csv.GetField<O>(14));
                    fld15.Add(csv.GetField<P>(15));
                    fld16.Add(csv.GetField<Q>(16));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1, colName2, colName3, colName4, colName5, colName6, colName7, colName8, colName9, colName10, colName11, colName12, colName13, colName14, colName15, colName16, colName17 },
                fieldNames,
                hasHeader);

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));
            frame[columnNames[1]] = new Column(new Column<B>(fld1));
            frame[columnNames[2]] = new Column(new Column<C>(fld2));
            frame[columnNames[3]] = new Column(new Column<D>(fld3));
            frame[columnNames[4]] = new Column(new Column<E>(fld4));
            frame[columnNames[5]] = new Column(new Column<F>(fld5));
            frame[columnNames[6]] = new Column(new Column<G>(fld6));
            frame[columnNames[7]] = new Column(new Column<H>(fld7));
            frame[columnNames[8]] = new Column(new Column<I>(fld8));
            frame[columnNames[9]] = new Column(new Column<J>(fld9));
            frame[columnNames[10]] = new Column(new Column<K>(fld10));
            frame[columnNames[11]] = new Column(new Column<L>(fld11));
            frame[columnNames[12]] = new Column(new Column<M>(fld12));
            frame[columnNames[13]] = new Column(new Column<N>(fld13));
            frame[columnNames[14]] = new Column(new Column<O>(fld14));
            frame[columnNames[15]] = new Column(new Column<P>(fld15));
            frame[columnNames[16]] = new Column(new Column<Q>(fld16));

            return frame;
        }

        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        public static Frame ReadCSV<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R>(string path, string colName1 = "", string colName2 = "", string colName3 = "", string colName4 = "", string colName5 = "", string colName6 = "", string colName7 = "", string colName8 = "", string colName9 = "", string colName10 = "", string colName11 = "", string colName12 = "", string colName13 = "", string colName14 = "", string colName15 = "", string colName16 = "", string colName17 = "", string colName18 = "", bool hasHeader = true, string delimiter = ",")
        {

            var fld0 = new List<A>();
            var fld1 = new List<B>();
            var fld2 = new List<C>();
            var fld3 = new List<D>();
            var fld4 = new List<E>();
            var fld5 = new List<F>();
            var fld6 = new List<G>();
            var fld7 = new List<H>();
            var fld8 = new List<I>();
            var fld9 = new List<J>();
            var fld10 = new List<K>();
            var fld11 = new List<L>();
            var fld12 = new List<M>();
            var fld13 = new List<N>();
            var fld14 = new List<O>();
            var fld15 = new List<P>();
            var fld16 = new List<Q>();
            var fld17 = new List<R>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                    fld1.Add(csv.GetField<B>(1));
                    fld2.Add(csv.GetField<C>(2));
                    fld3.Add(csv.GetField<D>(3));
                    fld4.Add(csv.GetField<E>(4));
                    fld5.Add(csv.GetField<F>(5));
                    fld6.Add(csv.GetField<G>(6));
                    fld7.Add(csv.GetField<H>(7));
                    fld8.Add(csv.GetField<I>(8));
                    fld9.Add(csv.GetField<J>(9));
                    fld10.Add(csv.GetField<K>(10));
                    fld11.Add(csv.GetField<L>(11));
                    fld12.Add(csv.GetField<M>(12));
                    fld13.Add(csv.GetField<N>(13));
                    fld14.Add(csv.GetField<O>(14));
                    fld15.Add(csv.GetField<P>(15));
                    fld16.Add(csv.GetField<Q>(16));
                    fld17.Add(csv.GetField<R>(17));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1, colName2, colName3, colName4, colName5, colName6, colName7, colName8, colName9, colName10, colName11, colName12, colName13, colName14, colName15, colName16, colName17 , colName18},
                fieldNames,
                hasHeader);

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));
            frame[columnNames[1]] = new Column(new Column<B>(fld1));
            frame[columnNames[2]] = new Column(new Column<C>(fld2));
            frame[columnNames[3]] = new Column(new Column<D>(fld3));
            frame[columnNames[4]] = new Column(new Column<E>(fld4));
            frame[columnNames[5]] = new Column(new Column<F>(fld5));
            frame[columnNames[6]] = new Column(new Column<G>(fld6));
            frame[columnNames[7]] = new Column(new Column<H>(fld7));
            frame[columnNames[8]] = new Column(new Column<I>(fld8));
            frame[columnNames[9]] = new Column(new Column<J>(fld9));
            frame[columnNames[10]] = new Column(new Column<K>(fld10));
            frame[columnNames[11]] = new Column(new Column<L>(fld11));
            frame[columnNames[12]] = new Column(new Column<M>(fld12));
            frame[columnNames[13]] = new Column(new Column<N>(fld13));
            frame[columnNames[14]] = new Column(new Column<O>(fld14));
            frame[columnNames[15]] = new Column(new Column<P>(fld15));
            frame[columnNames[16]] = new Column(new Column<Q>(fld16));
            frame[columnNames[17]] = new Column(new Column<R>(fld17));

            return frame;
        }



        /// <summary>
        /// Reads a CSV file from local or the internet
        /// </summary>
        public static Frame ReadCSV<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S>(string path, string colName1 = "", string colName2 = "", string colName3 = "", string colName4 = "", string colName5 = "", string colName6 = "", string colName7 = "", string colName8 = "", string colName9 = "", string colName10 = "", string colName11 = "", string colName12 = "", string colName13 = "", string colName14 = "", string colName15 = "", string colName16 = "", string colName17 = "", string colName18 = "", string colName19 = "", bool hasHeader = true, string delimiter = ",")
        {

            var fld0 = new List<A>();
            var fld1 = new List<B>();
            var fld2 = new List<C>();
            var fld3 = new List<D>();
            var fld4 = new List<E>();
            var fld5 = new List<F>();
            var fld6 = new List<G>();
            var fld7 = new List<H>();
            var fld8 = new List<I>();
            var fld9 = new List<J>();
            var fld10 = new List<K>();
            var fld11 = new List<L>();
            var fld12 = new List<M>();
            var fld13 = new List<N>();
            var fld14 = new List<O>();
            var fld15 = new List<P>();
            var fld16 = new List<Q>();
            var fld17 = new List<R>();
            var fld18 = new List<S>();

            var fieldNames = ReadCsv(
                path,
                new CsvHelper.Configuration.CsvConfiguration() { HasHeaderRecord = hasHeader, Delimiter = delimiter },
                csv =>
                {
                    fld0.Add(csv.GetField<A>(0));
                    fld1.Add(csv.GetField<B>(1));
                    fld2.Add(csv.GetField<C>(2));
                    fld3.Add(csv.GetField<D>(3));
                    fld4.Add(csv.GetField<E>(4));
                    fld5.Add(csv.GetField<F>(5));
                    fld6.Add(csv.GetField<G>(6));
                    fld7.Add(csv.GetField<H>(7));
                    fld8.Add(csv.GetField<I>(8));
                    fld9.Add(csv.GetField<J>(9));
                    fld10.Add(csv.GetField<K>(10));
                    fld11.Add(csv.GetField<L>(11));
                    fld12.Add(csv.GetField<M>(12));
                    fld13.Add(csv.GetField<N>(13));
                    fld14.Add(csv.GetField<O>(14));
                    fld15.Add(csv.GetField<P>(15));
                    fld16.Add(csv.GetField<Q>(16));
                    fld17.Add(csv.GetField<R>(17));
                    fld18.Add(csv.GetField<S>(18));
                });

            string[] columnNames = GetHeaderNames(
                new string[] { colName1, colName2, colName3, colName4, colName5, colName6, colName7, colName8, colName9, colName10, colName11, colName12, colName13, colName14, colName15, colName16, colName17, colName18 , colName19},
                fieldNames,
                hasHeader);

            Frame frame = new Frame();

            frame[columnNames[0]] = new Column(new Column<A>(fld0));
            frame[columnNames[1]] = new Column(new Column<B>(fld1));
            frame[columnNames[2]] = new Column(new Column<C>(fld2));
            frame[columnNames[3]] = new Column(new Column<D>(fld3));
            frame[columnNames[4]] = new Column(new Column<E>(fld4));
            frame[columnNames[5]] = new Column(new Column<F>(fld5));
            frame[columnNames[6]] = new Column(new Column<G>(fld6));
            frame[columnNames[7]] = new Column(new Column<H>(fld7));
            frame[columnNames[8]] = new Column(new Column<I>(fld8));
            frame[columnNames[9]] = new Column(new Column<J>(fld9));
            frame[columnNames[10]] = new Column(new Column<K>(fld10));
            frame[columnNames[11]] = new Column(new Column<L>(fld11));
            frame[columnNames[12]] = new Column(new Column<M>(fld12));
            frame[columnNames[13]] = new Column(new Column<N>(fld13));
            frame[columnNames[14]] = new Column(new Column<O>(fld14));
            frame[columnNames[15]] = new Column(new Column<P>(fld15));
            frame[columnNames[16]] = new Column(new Column<Q>(fld16));
            frame[columnNames[17]] = new Column(new Column<R>(fld17));
            frame[columnNames[18]] = new Column(new Column<S>(fld18));

            return frame;
        }


        #endregion


        #region Methods

        public override string ToString()
        {
            return this.Text();
        }

        #endregion

    }


}
