using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace UpgradeHelpers.VB6.Utils
{
    ///<summary>
    ///This simulates the XarrayDbObject funcionality based on DataTable class.
    ///</summary>
    ///<remarks>
    ///This class only supports two-dimensional arrays. Multi-dimensional arrays are not supported.
    ///</remarks>
    public class XArrayHelper : DataTable
    {
        ///<summary>
        /// Stores the LowerBounds to handle indexes.
        ///</summary>
        private int[] DimensionLowerBounds = null;

        ///<summary>
        /// Stores the lenghts to handle indexes.
        ///</summary>
        private int[] DimensionLengths = null;

        ///<summary>
        /// Constructor for the XArrayHelper.
        ///</summary>
        public XArrayHelper()
        {
            DimensionLowerBounds = null;
            DimensionLengths = null;
        }

        ///<summary>
        ///This function is a Factory to create Xarray instances. 
        ///</summary>
        ///<param name="Lengths">The length of each dimension.</param>
        ///<param name="LowerBounds">The lower bounds to use for each dimension.</param>
        ///<returns>A new XArrayHelper instance.</returns>
        public static XArrayHelper CreateInstanceXarray(int[] Lengths, int[] LowerBounds)
        {
            XArrayHelper xarr = new XArrayHelper();

            xarr.DimensionLengths = Lengths;
            xarr.DimensionLowerBounds = LowerBounds;
            for (int col = 0; col <= Lengths[1]; col++)
            {
                xarr.Columns.Add(new DataColumn());
            }

            for (int i = 0; i <= Lengths[0]; i++)
            {
                DataRow row = xarr.NewRow();
                xarr.Rows.Add(row);
            }
            return xarr;
        }

        ///<summary>
        ///This function redimensions a Xarray instance.
        ///</summary>
        ///<param name="Lengths">The length of each dimension.</param>
        ///<param name="LowerBounds">The lower bounds to use for each dimension.</param>
        ///<returns>It returns a redimensioned instance of itself.</returns>
        ///<remarks></remarks>
        public XArrayHelper RedimXArray(int[] Lengths, int[] LowerBounds)
        {

            DimensionLengths = Lengths;
            DimensionLowerBounds = LowerBounds;

            if (this.Columns.Count == 0)
            {
                for (int colIndex = 0; colIndex <= Lengths[1]; colIndex++)
                {
                    this.Columns.Add(new DataColumn());
                }
            }
            else if (this.Columns.Count < (Lengths[1] + 1))
            {
                for (int colIndex = this.Columns.Count; colIndex <= Lengths[1]; colIndex++)
                {
                    this.Columns.Add(new DataColumn());
                }
            }
            else if (this.Columns.Count > (Lengths[1] + 1))
            {
                for (int colIndex = Lengths[1] + 1; colIndex <= this.Columns.Count - 1; colIndex++)
                {
                    this.Columns.RemoveAt(colIndex);
                }
            }

            if (this.Rows.Count == 0)
            {
                for (int rowIndex = 0; rowIndex <= Lengths[0]; rowIndex++)
                {
                    DataRow row = this.NewRow();
                    this.Rows.Add(row);
                }
            }
            else if (this.Rows.Count < (Lengths[0] + 1))
            {
                for (int rowIndex = this.Rows.Count; rowIndex <= Lengths[0]; rowIndex++)
                {
                    DataRow row = this.NewRow();
                    this.Rows.Add(row);
                }
            }
            else if (this.Rows.Count > (Lengths[0] + 1))
            {
                for (int rowIndex = Lengths[0] + 1; rowIndex <= this.Rows.Count - 1; rowIndex++)
                {
                    this.Rows.RemoveAt(rowIndex);
                }
            }
            return this;

        }

        ///<summary>
        ///Gets the upper bound of the specified dimension.
        ///</summary>
        ///<param name="Dimension">A zero-based dimension whose upper bound needs to be determined.</param>
        ///<returns>The upper bound for the specificed dimension.</returns>
        public int GetUpperBound(int Dimension)
        {
            return DimensionLengths[Dimension];
        }

        ///<summary>
        ///Gets the Lower bound of the specified dimension.
        ///</summary>
        ///<param name="Dimension">A zero-based dimension whose lower bound needs to be determined.</param>
        ///<returns>The lower bound for the specificed dimension.</returns>
        public int GetLowerBound(int Dimension)
        {
            return DimensionLowerBounds[Dimension];
        }

        ///<summary>
        ///Gets the number of elements in the specified dimension.
        ///</summary>
        ///<param name="Dimension">A zero-based dimension whose length needs to be determined.</param>
        ///<returns>The length of elements of the specified dimension.</returns>
        public int GetLength(int Dimension)
        {
            return DimensionLengths[Dimension];
        }

        ///<summary>
        ///Returns the element at the specified row and column.
        ///</summary>
        ///<param name="row">Row index where the element is located.</param>
        ///<param name="column">Column index where the element is located.</param>
        ///<value>Value for the specified element.</value>
        ///<returns>The element at the specified index.</returns>
        public Object this[int row, int column]
        {
            get
            {
                return this.Rows[row - this.DimensionLowerBounds[0]][column - this.DimensionLowerBounds[1]];
            }
            set
            {
                this.Rows[row - this.DimensionLowerBounds[0]][column - this.DimensionLowerBounds[1]] = value;
            }
        }

        ///<summary>
        ///Gets the value at the specified position.
        ///</summary>
        ///<param name="row">Index row where the element is located.</param>
        ///<param name="column">Index column where the element is located.</param>
        ///<returns>The value at the specified position.</returns>
        public Object GetValue(int row, int column)
        {
            return this.Rows[row - this.DimensionLowerBounds[0]][column - this.DimensionLowerBounds[1]];
        }

        ///<summary>
        ///Sets a value to the element at the specified position.
        ///</summary>
        ///<param name="value">The new value for the specified element.</param>
        ///<param name="row">Index row where the element is located.</param>
        ///<param name="column">Index column where the element is located.</param>
        public void SetValue(Object value, int row, int column)
        {
            this.Rows[row - this.DimensionLowerBounds[0]][column - this.DimensionLowerBounds[1]] = value;
        }

        ///<summary>
        ///Clears a range of elements in the XArrayHelper.
        ///</summary>
        ///<param name="arr">XArrayHelper whose elements need to be cleared.</param>
        ///<param name="index">The starting index of the range of elements.</param>
        ///<param name="length">The number of elements to be cleared.</param>
        public static void Clear(XArrayHelper arr, int index, int length)
        {

            int realIndexi = arr.GetLowerBound(0);
            int realIndexj = arr.GetLowerBound(1);

            index = index - arr.GetLowerBound(0);

            while (index > 0)
            {
                if (index > arr.GetUpperBound(1))
                {
                    realIndexi = realIndexi + 1;
                    index = index - arr.GetLength(1);
                }
                else
                {
                    realIndexj = realIndexj + index;
                    index = 0;
                }
            }

            for (int j = realIndexj; j <= arr.GetUpperBound(1); j++)
            {
                if (length < 0) return;
                arr[realIndexi, j] = null;
                length = length - 1;
            }

            realIndexi = realIndexi + 1;

            for (int i = realIndexi; i <= arr.GetUpperBound(0); i++)
            {
                for (int j = arr.GetLowerBound(1); j <= arr.GetUpperBound(1); j++)
                {
                    if (length < 1) return;
                    arr[i, j] = null;
                    length = length - 1;
                }
            }
        }


        ///<summary>
        ///Creates a cleared a XArrayHelper.
        ///</summary>
        ///<param name="arr">XArrayHelper whose elements need to be cleared.</param>
        public void Clear(ref XArrayHelper arr)
        {
            int[] length = new int[] { 1, 0 };
            int[] lowerB = new int[] { arr.DimensionLowerBounds[0], arr.DimensionLowerBounds[1] };
            this.Clear();
            arr.RedimXArray(length, lowerB);
        }

        ///<summary>
        ///Adds a new row to the current instance of XArrayHelper.
        ///</summary>
        public void AppendRows()
        {
            int[] length = new int[] { this.DimensionLengths[0] + 1, this.DimensionLengths[1] };
            int[] lowerB = new int[] { this.DimensionLowerBounds[0], this.DimensionLowerBounds[1] };
            this.RedimXArray(length, lowerB);
        }

        ///<summary>
        ///Adds a new row to the current instance of XArrayHelper and sets a value to the specified
        ///row and column.
        ///</summary>
        ///<param name="value">The value to be set the specified position.</param>
        ///<param name="row">The row in the XArrayHelper where to be set the value.</param>
        ///<param name="column">The column in the XArrayHelper where to be set the value.</param>
        public void AppendRows(Object value, int row, int column)
        {
            int[] length = new int[] { this.DimensionLengths[0] + 1, this.DimensionLengths[1] };
            int[] lowerB = new int[] { this.DimensionLowerBounds[0], this.DimensionLowerBounds[1] };
            this.RedimXArray(length, lowerB);

            this.Rows[row - this.DimensionLowerBounds[0]][column - this.DimensionLowerBounds[1]] = value;
        }

        ///<summary>
        ///Deletes a row in the specified position and redimensions the XArrayHelper.
        ///</summary>
        ///<param name="row">The row in the XArrayHelper to be deleted.</param>
        public void DeleteRows(int row)
        {
            this.Rows[row - this.DimensionLowerBounds[0]].Delete();
            int[] length = new int[] { this.DimensionLengths[0] - 1, this.DimensionLengths[1] };
            int[] lowerB = new int[] { this.DimensionLowerBounds[0], this.DimensionLowerBounds[1] };
            this.RedimXArray(length, lowerB);
        }

        ///<summary>
        ///Creates a XArrayHelper and copies the values from an object array.
        ///</summary>
        ///<param name="array">The source array to be copied.</param>
        public void LoadRows(Object[,] array)
        {
            this.RedimXArray(new int[] { array.GetUpperBound(0), array.GetUpperBound(1) }, new int[] { array.GetLowerBound(0), array.GetLowerBound(1) });
            for (int row = array.GetLowerBound(0); row <= array.GetUpperBound(0); row++)
            {
                for (int col = array.GetLowerBound(1); col <= array.GetUpperBound(1); col++)
                {
                    this.SetValue(array[row, col], row, col);
                }
            }
        }

        ///<summary>
        ///Creates a XArrayHelper and copies the values from a XArrayHelper.
        ///</summary>
        ///<param name="table">The source XArrayHelper to be copied.</param>
        public void LoadRows(XArrayHelper table)
        {
            this.RedimXArray(new int[] { table.GetUpperBound(0), table.GetUpperBound(1) }, new int[] { table.GetLowerBound(0), table.GetLowerBound(1) });
            for (int row = table.GetLowerBound(0); row <= table.GetUpperBound(0); row++)
            {
                for (int col = table.GetLowerBound(1); col <= table.GetUpperBound(1); col++)
                {
                    this.SetValue(table.GetValue(row, col), row, col);
                }
            }
        }

        ///<summary>
        ///Finds a value into a XArrayHelper.
        ///</summary>
        ///<param name="value">The value to be found.</param>
        ///<returns>True if the value is found into the XArrayHelper.</returns>
        public Object Find(Object value)
        {
            Boolean result = false;
            for (int row = this.GetLowerBound(0); row <= this.GetUpperBound(0); row++)
            {
                for (int col = this.GetLowerBound(1); col <= this.GetUpperBound(1); col++)
                {
                    if (this.GetValue(row, col) == value)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        ///<summary>
        ///Finds a value into a XArrayHelper from a specified position.
        ///</summary>
        ///<param name="value">The value to be found.</param>
        ///<param name="lowerBound">The lowerbound where to start searching.</param>
        ///<param name="upperBound">The upperbound where to finish searching.</param>
        ///<returns>The index where the values is found or -1 if it is not found.</returns>
        public Object Find(Object value, int lowerBound, int upperBound)
        {
            long index = -1;
            for (int row = lowerBound; row <= this.GetUpperBound(0); row++)
            {
                for (int col = upperBound; col <= this.GetUpperBound(1); col++)
                {
                    if (this.GetValue(row, col) == value)
                    {
                        index = row;
                        break;
                    }
                }
            }
            return index;
        }
    }
}

