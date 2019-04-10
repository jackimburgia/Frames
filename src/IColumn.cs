using System;
using System.Collections.Generic;

namespace Spearing.Utilities.Data.Frames
{
    public interface IColumn
    {
        string Name { get; set; }
        int Count { get; }
        object Value(int index);
        Column Duplicate(List<int> indexes);
        Column Duplicate();
        Column Duplicate(int count);
        Column Union(Column column);


        Type GetSubType();
        //string ValueString();
    }


    //public class IColumn2<T> : List<T>, IColumn
    //{
    //    public IColumn2(IEnumerable<T> col) : base(col)
    //    {

    //    }
    //    public string Name { get; set; }
    //    //public abstract int Count { get; }
    //    public abstract object Value(int index);
    //    public abstract IColumn Duplicate(List<int> indexes);
    //    public abstract IColumn Duplicate();

    //    public static implicit operator IColumn2<T>(T[] col)
    //    {
    //        return new Column<T>(col);
    //    }


    //    public abstract Type GetSubType();
    //}
}
