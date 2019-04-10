using System.Linq;

namespace Spearing.Utilities.Data.Frames
{
    public class Row
    {
        /// <summary>
        /// Original index of row when frame created
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Frame this row is part of
        /// </summary>
        public Frame Frame { get; }

        public Row(Frame frame)
        {
            this.Frame = frame;
        }

        ///// <summary>
        ///// Returns a typed value for a column
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="columnName"></param>
        ///// <returns></returns>
        //public T GetValue<T>(string columnName)
        //{
        //    return this.Frame.Column<T>(columnName)[this.Index];
        //}

        //public T GetValue<T>(int columnIndex)
        //{
        //    var name = this.Frame.Columns[columnIndex].Name;
        //    return this.GetValue<T>(name);
        //}



        /// <summary>
        /// Returns the values in a row as an array of objects
        /// </summary>
        public object[] Values
        {
            get
            {
                return this.Frame.Columns.Select(col => col.Value(this.Index)).ToArray();
            }
        }

        public override string ToString()
        {
            return new Row[] { this }.Text();
        }

        public void Print()
        {
            new Row[] { this }.Print();
        }
    }
}
