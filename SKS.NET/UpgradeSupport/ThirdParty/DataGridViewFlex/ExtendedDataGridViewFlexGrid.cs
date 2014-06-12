// Author: mrojas
// Project: UpgradeHelpers.Windows.Forms
// Path: UpgradeHelpers.Windows.Forms\ExtendedDataGridView
// Creation date: 7/16/2009 2:51 PM
// Last modified: 9/17/2009 11:11 AM

#region Using directives
using UpgradeHelpers.Windows.Forms.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
#endregion

namespace UpgradeHelpers.Windows.Forms
{


    /// <summary>
    /// This partial class add functionality for compatibility with MSFlexGrid
    /// </summary>
    partial class DataGridViewFlex
    {
        /// <summary>
        /// DEFAULT_FIXED_ROWS 
        /// </summary>
        public const int DEFAULT_FIXED_ROWS = 1;
        /// <summary>
        /// DEFAULT_FIXED_COLUMNS;
        /// </summary>
        public const int DEFAULT_FIXED_COLUMNS = 1;
        /// <summary>
        /// DEFAULT_CELL_HEIGHT
        /// </summary>
        public const int DEFAULT_CELL_HEIGHT = 16;
        /// <summary>
        /// UNSETVALUE
        /// </summary>
        public const int UNSETVALUE = -5;

        /// <summary>
        /// Flag for indicate when the component starts to select
        /// </summary>
        public bool SelectionStarted;

        /// <summary>
        /// My values
        /// </summary>
        Dictionary<string, object> myValues = new Dictionary<string, object>();

        private T GetValueIfFound<T>(String keyname, T notfoundvalue)
        {
            if (!myValues.ContainsKey(keyname))
                return notfoundvalue;
            else
                return (T)myValues[keyname];
        } // GetValueIfFound(, keyname, notfoundvalue)


        private void SetSelectedCells(int prow, int pcol)
        {
            int c1 = CurrentColumnIndex, c2 = pcol;
            int r1 = CurrentRowIndex, r2 = prow;
            int temp;
            if (c1 > c2)
            {
                temp = c2;
                c2 = c1;
                c1 = temp;
            }
            if (r1 > r2)
            {
                temp = r2;
                r2 = r1;
                r1 = temp;
            }
            DataGridViewCell Firstcell = null;
            for (int i = 0; i < ColumnsCount; i++)
            {
                for (int j = 0; j < RowsCount; j++)
                {
                    DataGridViewCell cell = GetCell(j, i);
                    DataGridViewHeaderCell headerCell = cell as DataGridViewHeaderCell;
                    if (c1 <= i && i <= c2)
                    {
                        if (r1 <= j && j <= r2)
                        {
                            if (i == c1 && j == r1)
                                Firstcell = cell;
                            if (headerCell == null)
                            {
                                if (!cell.Selected)
                                {
                                    cell.Selected = true;
                                }
                            }
                            continue;
                        }
                    }
                    //Deselect any other Cell
                    if (headerCell == null)
                    {
                        if (cell.Selected && cell.Visible)
                        {
                            cell.Selected = false;
                        }
                    }
                }
            }
            _generalCurrentCell = Firstcell;
            Refresh();//TODO CHECK it was base.Refresh
        }



        /// <summary>
        /// Resets the behaviour to default property values.
        /// </summary>
        public void Reset()
        {
            AllowUserToAddRows = false; //Default Behaivor
            AllowUserToDeleteRows = false;
            RowHeadersWidth = DEFAULT_NEW_CUSTOM_COLUMN_WIDTH;
            RowTemplate.Height = DEFAULT_CELL_HEIGHT;
            ColumnHeadersHeight = DEFAULT_CELL_HEIGHT;
            AllowUserToResizeColumns = false;
            AllowUserToResizeRows = false;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            ColumnHeadersDefaultCellStyle.SelectionForeColor = ColumnHeadersDefaultCellStyle.ForeColor;
            ColumnHeadersDefaultCellStyle.SelectionBackColor = ColumnHeadersDefaultCellStyle.BackColor;
            RowHeadersDefaultCellStyle.SelectionForeColor = RowHeadersDefaultCellStyle.ForeColor;
            RowHeadersDefaultCellStyle.SelectionBackColor = RowHeadersDefaultCellStyle.BackColor;
            BorderStyle = BorderStyle.Fixed3D;
            ReadOnly = true;
            if (currentCellChanged == null)
            {
                currentCellChanged = new EventHandler(grid_CurrentCellChanged);
                CurrentCellChanged -= currentCellChanged;
                CurrentCellChanged += currentCellChanged;
            } // if
            if (rowpostPaint == null)
            {
                rowpostPaint = new DataGridViewRowPostPaintEventHandler(grid_RowPostPaint);
                RowPostPaint -= rowpostPaint;
                RowPostPaint += rowpostPaint;
            } // if
            EnableHeadersVisualStyles = false;
            AllowBigSelection = true;

        }

        EventHandler currentCellChanged;

        void grid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (!SelectedCells.Contains(_generalCurrentCell))
                _generalCurrentCell = CurrentCell;
        }

        #region Stripe Support
        /// <summary>
        /// Cell Style
        /// </summary>
        public DataGridViewCellStyle AltenateCellStyle = null;

        #endregion

        DataGridViewRowPostPaintEventHandler rowpostPaint;
        void grid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            this.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
            this.ColumnHeadersHeightSizeMode= DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            checkSelectionForGeneralCurrentCell = false;
            if (!isInitializing && FocusRect != FocusRectSettings.FocusNone)
            {
                DataGridViewCell generalCurrentCell = GeneralCurrentCell;
                if (generalCurrentCell != null && generalCurrentCell.RowIndex == e.RowIndex)
                {

                    generalCurrentCell.Style.SelectionForeColor = DefaultCellStyle.ForeColor;
                    Rectangle rect = GetCellDisplayRectangle(generalCurrentCell.ColumnIndex, generalCurrentCell.RowIndex, false);
                    const DataGridViewPaintParts parts = DataGridViewPaintParts.Background | DataGridViewPaintParts.Border | DataGridViewPaintParts.ContentBackground | DataGridViewPaintParts.ContentForeground | DataGridViewPaintParts.Focus;
                    e.PaintCells(rect, parts);
                    e.DrawFocus(rect, false);
                }
            }
            checkSelectionForGeneralCurrentCell = true;
        }

        /// <summary>
        /// Mouse Cell Row
        /// </summary>
        public int mouse_cell_row = -1;
        /// <summary>
        /// Mouse Cell Column
        /// </summary>
        public int mouse_cell_column = -1;


        void ExtendedDataGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            mouse_cell_row = e.RowIndex + (ColumnHeadersVisible ? 1 : 0);
            mouse_cell_column = e.ColumnIndex + (RowHeadersVisible ? 1 : 0);

        }




        #region Properties
        /// <summary>
        /// Is design mode?
        /// </summary>
        public new bool DesignMode
        {
            get
            {
                return base.DesignMode;
            }
        }


        //TODO remove private DataGridViewCell _GeneralCurrentCell;

        private bool checkSelectionForGeneralCurrentCell = true;

        /// <summary>
        /// MSFlexGrid has a different selection behaviour than DataGridView.
        /// In a MSFlexGrid the cell that currently has the focus does not have
        /// the same background color or "FocusRect" than the other selected cells.
        /// To provide an MSFlexGrid compatible behaviour we must provide a way 
        /// to track that cell
        /// </summary>
        public DataGridViewCell GeneralCurrentCell
        {
            get
            {
                if (_generalCurrentCell == null) _generalCurrentCell = CurrentCell;
                //else _generalCurrentCell.Style.SelectionForeColor = DefaultCellStyle.SelectionForeColor;
                if (checkSelectionForGeneralCurrentCell && !SelectedCells.Contains(_generalCurrentCell))
                    _generalCurrentCell = CurrentCell;

                return _generalCurrentCell;

            }
            set
            {
                if (_generalCurrentCell == value) return;
                _generalCurrentCell = value;
            }
        }



        /// <summary>
        /// Avoids Focus Rectangle to be displayed. FocusRectangle is Managed in the Control
        /// </summary>
        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }







        /// <summary>
        /// Current Internal Cell pointer
        /// </summary>
        public DataGridViewCell _generalCurrentCell;
        /// <summary>
        /// Fill Style Settings
        /// </summary>
        public FillStyleSettings _fillStyle;

        /// <summary>
        /// Determines whether setting the Text property or one of the Cell formatting properties of a FlexGrid applies the change to all selected cells
        /// </summary>
        [Description("Determines whether setting the Text property or one of the Cell formatting properties of a FlexGrid applies the change to all selected cells"), 
        Browsable(true), DefaultValue(FillStyleSettings.FillSingle), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public FillStyleSettings FillStyle
        {
            get 
            {
                if (isInitializing)
                {
                    return GetValueIfFound<FillStyleSettings>("FillStyle", FillStyleSettings.FillSingle);
                }
                else
                    return _fillStyle;
            }
            set {
                if (isInitializing)
                    myValues["FillStyle"] = value;
                else
                _fillStyle = value; 
            
            }
        }

        
        string _toolTipText = String.Empty;

        /// <summary>
        /// Gets/Sets the tool tip for the complete grid control
        /// </summary>
        public string ToolTipText
        {
            get
            {
                return _toolTipText;
            }
            set
            {
                _toolTipText = value;
            }
        }


		private static DataGridViewCell[] GridCellCollectionToArray(DataGridViewSelectedCellCollection collection)
		{
			DataGridViewCell[] cells = new DataGridViewCell[collection.Count];
			int x = 0;
			foreach (DataGridViewCell cell in collection)
			{
				cells[x] = cell;
				x++;
			}
			return cells;
		}





        /// <summary>
        /// Returns/sets the color used to draw the lines between FlexGrid cells.
        /// </summary>
        [Description("Returns/sets the color used to draw the lines between FlexGrid cells."),
        Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), 
        Obsolete("Use Grid Color Instead")]
        public Color GridColorFixed
        {
            get { return GridColor; }
            set { GridColor = value; }
        }







 




     



     private FocusRectSettings _focusRect = FocusRectSettings.FocusLight;

        /// <summary>
        /// Determines whether the FlexGrid control should draw a focus rectangle around the current cell.
        /// </summary>
        [Description("Determines whether the FlexGrid control should draw a focus rectangle around the current cell."), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(FocusRectSettings.FocusLight)]
        public FocusRectSettings FocusRect
        {
            get { return _focusRect; }
            set
            {
                _focusRect = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Removes a row from a FlexGrid control at run time
        /// </summary>
        /// <param name="index">The index of the Row</param>
        public void RemoveItem(int index)
        {
            if (index == 0)
            {
                if (ColumnHeadersVisible)
                    throw new InvalidOperationException("It's not possible to remove a Fixed Row");
            }
            index = ColumnHeadersVisible ? index - 1 : index;
            base.Rows.RemoveAt(index);
        }
        /// <summary>
        /// Get Base Cell at position
        /// </summary>
        /// <param name="columnindex">column position</param>
        /// <param name="rowindex">row position</param>
        /// <returns></returns>
        public DataGridViewCell BaseGetCell(int columnindex, int rowindex)
        {
            return base[columnindex, rowindex];
        }
        /// <summary>
        /// Base Set Cell
        /// </summary>
        /// <param name="columnindex">column index</param>
        /// <param name="rowindex">row index</param>
        /// <param name="cell">new value</param>
        public void BaseSetCell(int columnindex, int rowindex,DataGridViewCell cell)
        {
            base[columnindex, rowindex] = cell;
        }


        /*TODO!!!
        /// <summary>
        /// Gets/sets the current cell.
        /// </summary>
        public DataGridViewCell CurrentCell
        {
            get
            {
                return grid.GeneralCurrentCell;
            }
            set
            {
                grid.GeneralCurrentCell = value;
            }
        }*/


        /// <summary>
        /// Returns/sets whether a grid should allow regular cell selection, selection by rows, or selection by columns.
        /// </summary>
        public new DataGridViewSelectionMode SelectionMode
        {
            get
            {
                return base.SelectionMode;
            }
            set
            {
                base.SelectionMode = value;
                if (SelectionStarted)
                {
                    if (value == DataGridViewSelectionMode.FullColumnSelect)
                    {
                        foreach (DataGridViewColumn column in Columns)
                        {
                            column.SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                    }
                    //When the SelectionMode changes, The Cell is not selected. 
                    //In flex grid this doesn't happen, so to correct this
                    //Cell Is selected manually.
                    if (CurrentCell != null)
                        CurrentCell.Selected = true;
                }
            }
            //TODO _selectionMode = value;
        }

        private object ColumnHeadersTag;
        private object RowHeadersTag;

        /// <summary>
        /// Gets Array of long integer values with one item for each row (RowData) of the FlexGrid. Not available at design time.
        /// </summary>
        /// <param name="index">The index of the Row</param>
        /// <returns>The Data Stored on the Row</returns>
        public int get_RowData(int index)
        {
            if (index == 0)
            {
                if (ColumnHeadersVisible)
                    return ColumnHeadersTag != null ? (int)ColumnHeadersTag : 0;
            }

            int realindex = ColumnHeadersVisible ? index - 1 : index;
            return base.Rows[realindex].Tag != null ? (int)base.Rows[realindex].Tag : 0;
        }

        /// <summary>
        /// Sets Array of long integer values with one item for each row (RowData) of the FlexGrid. Not available at design time.
        /// </summary>
        /// <param name="index">The index of the Row</param>
        /// <param name="value">The Data to be Stored on the Row</param>
        public void set_RowData(int index, int value)
        {
            if (index == 0)
            {
                if (ColumnHeadersVisible)
                {
                    ColumnHeadersTag = value;
                    return;
                }
            }
            int realindex = ColumnHeadersVisible ? index - 1 : index;
            base.Rows[realindex].Tag = value;
        }

		/// <summary>
		/// This class manages the Column Data of a specified grid.
		/// </summary>
        public class ColDataProperty
        {
            /// <summary>
            /// Parent control.
            /// </summary>
            public DataGridViewFlex parent;
			/// <summary>
			/// Creates a ColDataProperty class for a specified grid.
			/// </summary>
			/// <param name="parent">The grid for which this class should be created.</param>
            public ColDataProperty(DataGridViewFlex parent) { this.parent = parent; }
			/// <summary>
			/// Gets/sets the Column Data property for the specified column.
			/// </summary>
			/// <param name="index">The index of the column.</param>
			/// <returns>The column data of the selected column.</returns>
            public int this[int index]
            {
                get
                {
                    if (index == 0)
                    {
                        if (parent.RowHeadersVisible)
                            return parent.RowHeadersTag != null ? (int)parent.RowHeadersTag : 0;
                    }
                    int realindex = parent.RowHeadersVisible ? index - 1 : index;
                    return parent.Columns[realindex].Tag != null ? (int)parent.Columns[realindex].Tag : 0;
                }
                set
                {
                    if (index == 0)
                    {
                        if (parent.RowHeadersVisible)
                        {
                            parent.RowHeadersTag = value;
                            return;
                        }
                    }
                    int realindex = parent.RowHeadersVisible ? index - 1 : index;
                    parent.Columns[realindex].Tag = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets Array of long integer values with one item for each column (ColData) of the FlexGrid.
        /// Not available at design time.
        /// </summary>
        [Browsable(false)]
        public ColDataProperty ColData
        {
            get
            {
                return new ColDataProperty(this);
            }
        }


        /// <summary>
        /// Returns True if the specified column is visible.
        /// </summary>
        /// <param name="index">The index of the Column</param>
        /// <returns></returns>
        public bool get_ColIsVisible(int index)
        {
            if (index == 0)
            {
                return RowHeadersVisible;
            }
            int realindex = index - 1;
            return base.Columns[realindex].Visible;
        }

        /// <summary>
        /// Returns True if the specified row is visible.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RowIsVisible(int index)
        {
            if (index == 0)
            {
                return ColumnHeadersVisible;
            }
            int realindex = index - 1;
            return base.Rows[realindex].Visible;
        }




        /// <summary>
        /// Sets/Gets the alignment of data in the fixed cells of a column.
        /// </summary>
        [Browsable(false)]
        public FixedAlignmentProperty FixedAlignment
        {
            get
            {
                return new FixedAlignmentProperty(this);
            }

        }

        /// <summary>
        /// Gets/Sets the alignment of data in a column. 
        /// Not available at design time (except indirectly through the FormatString property).
        /// </summary>
        [Browsable(false)]
        public ColAlignmentProperty ColAlignment
        {
            get
            {
                return new ColAlignmentProperty(this);
            }
        }


        /// <summary>
        /// Gets the distance in Pixels between the upper-left corner of the control and the upper-left corner of a specified column.
        /// </summary>
        [Browsable(false)]
        public ColPosProperty ColPos
        {
            get
            {
                return new ColPosProperty(this);
            }
        }

        /// <summary>
        /// Gets the distance in Pixels between the upper-left corner of the control and the upper-left corner of a specified row.
        /// </summary>
        [Browsable(false)]
		public RowPosProperty RowPos
		{
			get
			{
				return new RowPosProperty(this);
			}
		}



        /// <summary>
        /// Clears the contents of the FlexGrid. This includes all text, pictures, and cell formatting.
        /// </summary>
        public void Clear()
        {
            foreach (DataGridViewColumn col in base.Columns)
            {
                ClearCell(col.HeaderCell);
            } // foreach
            foreach (DataGridViewRow row in base.Rows)
            {
                ClearCell(row.HeaderCell);
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell!=null)
                    {
                        ClearCell(cell);
                    } // if (cell!=null
                }
            }           
        }

        private static void ClearCell(DataGridViewCell cell)
        {
            cell.ErrorText = String.Empty;
            cell.Value = null;
            cell.ToolTipText = String.Empty;
            cell.Style = new DataGridViewCellStyle();
        }

        /// <summary>
        /// Adds a new row to a FlexGrid control at run time.
        /// </summary>
        /// <param name="vsItem">String for elements to add</param>
        public void AddItem(string vsItem)
        {
            string[] items = vsItem.Split(new char[] { '\t' });
            DataGridViewRow row = new DataGridViewRow();
            row.Height = RowTemplate.Height;
            row.MinimumHeight = RowTemplate.MinimumHeight;
            Rows.Add(row);
            for (int i = 0; i < ColumnsCount && i < items.Length; i++)
            {
                GetCell(RowsCount - 1, i).Value = items[i];
            }

        }

        /// <summary>
        /// Adds a new row to a FlexGrid control at run time.
        /// </summary>
        /// <param name="vsItem">String for elements to add</param>
        /// <param name="viIndex">New Row Position</param>
        public void AddItem(string vsItem, int viIndex)
        {
            string[] items = vsItem.Split(new char[] { '\t' });
            DataGridViewRow row = new DataGridViewRow();
            row.Height = RowTemplate.Height;
            row.MinimumHeight = RowTemplate.MinimumHeight;

            if (viIndex < FixedRows)
                throw new InvalidOperationException("Cannot use AddItem on a fixed row");
            int realIndex = ColumnHeadersVisible ? viIndex - 1 : viIndex;
            Rows.Insert(realIndex, row);
            for (int i = 0; i < ColumnsCount && i < items.Length; i++)
            {
                GetCell(viIndex, i).Value = items[i];
            }
        }

        /// <summary>
        /// Changes the value, if the setted value contains \0 chars, then those are removed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event Arguments</param>
        void ExtendedDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewCell cell;

            if (e.ColumnIndex < 0 && e.RowIndex < 0)
            {

                cell = TopLeftHeaderCell;

            }

            else if (e.ColumnIndex < 0)
            {

                cell = Rows[e.RowIndex].HeaderCell;

            }

            else if (e.RowIndex < 0)
            {

                cell = Columns[e.ColumnIndex].HeaderCell;

            }

            else
            {

                cell = this[e.ColumnIndex, e.RowIndex];

            }





            string value = Convert.ToString(cell.Value);

            if (value.IndexOf('\0') != -1)
            {

                cell.Value = value.Substring(0, value.IndexOf('\0'));

            }

        }

        /// <summary>
        /// Set a Column Width, if pos equals zero, the RowHeader is set to the passed value
        /// </summary>
        /// <param name="pos">Column position</param>
        /// <param name="value">New width value</param>
        public void SetColumnWidth(int pos, double value)
        {
            if (pos == 0 && this.FixedRows > 0)
            {
                this.RowHeadersWidth = (int)value;
            }
            else
            {
                if (this.FixedRows > 0)
                {
                    this.Columns[pos-1].Width = (int)value;
                }
                else
                {
                    this.Columns[pos].Width = (int)value;
                }
            }
        }

        /// <summary>
        /// Set a Row Height, if pos equals zero and Fixed Columns are greater than zero, ColumnHeight is changed to param value.
        /// </summary>
        /// <param name="pos">Row Position</param>
        /// <param name="value">New Height value</param>
        public void SetRowHeight(int pos, double value)
        {
            if (pos == 0 && this.FixedColumns > 0)
            {
                this.ColumnHeadersHeight = (int)value;
            }
            else
            {
                if (this.FixedColumns > 0)
                {
                    this.Rows[pos - 1].Height = (int)value;
                }
                else
                {
                    this.Rows[pos].Height = (int)value;
                }
            }
        }

        #endregion
        
        #region Support Classes

		/// <summary>
		/// Class used to manage the Alignment properties of the columns of a grid.
		/// </summary>
        public class ColAlignmentProperty
        {
            internal DataGridViewFlex parent;
			/// <summary>
			/// Creates a ColumnAlignmentProperty class for a specified grid.
			/// </summary>
			/// <param name="parent">The grid for which to create this class.</param>
            public ColAlignmentProperty(DataGridViewFlex parent)
            { this.parent = parent; }
			/// <summary>
			/// Gets/sets the Alignment property for a specified column.
			/// </summary>
			/// <param name="index">The index of the column.</param>
			/// <returns>The Alignment property of the selected column.</returns>
            public DataGridViewContentAlignment this[int index]
            {
                get
                {
                    if (index == 0)
                    {
                        if (parent.RowHeadersVisible)
                        {
                            return parent.RowHeadersDefaultCellStyle.Alignment;
                        }
                    }
                    int realindex = parent.RowHeadersVisible ? index - 1 : index;
                    DataGridViewColumn column = parent.Columns[realindex];
                    return column.DefaultCellStyle.Alignment;

                }
                set
                {
                    if (index == 0)
                    {
                        if (parent.RowHeadersVisible)
                        {
                            parent.RowHeadersDefaultCellStyle.Alignment = value;
                            return;
                        }
                    }
                    int realindex = parent.RowHeadersVisible ? index - 1 : index;
                    DataGridViewColumn column = parent.Columns[realindex];
                    if (column.CellTemplate.Style == parent.DefaultCellStyle)
                    {
                        DataGridViewCellStyle style = new DataGridViewCellStyle(column.CellTemplate.Style);
                        style.Alignment = value;
                        column.CellTemplate.Style = style;
                        column.DefaultCellStyle = style;
                        if (column.HeaderCell.HasStyle)
                        {
                            if (column.HeaderCell.Style.Alignment == DataGridViewContentAlignment.NotSet)
                            {
                                column.HeaderCell.Style.Alignment = value;
                            }
                        }
                        else
                        {
                            column.HeaderCell.Style.Alignment = value;
                        }

                        foreach (DataGridViewRow row in parent.Rows)
                        {
                            if (row.Cells[realindex].Style == parent.DefaultCellStyle)
                            {
                                row.Cells[realindex].Style = style;
                            }
                            else
                            {
                                row.Cells[realindex].Style.Alignment = value;
                            }
                        }
                    }
                    else
                    {
                        column.CellTemplate.Style.Alignment = value;
                        column.DefaultCellStyle.Alignment = value;
                        if (column.HeaderCell.HasStyle)
                        {
                            if (column.HeaderCell.Style.Alignment == DataGridViewContentAlignment.NotSet)
                            {
                                column.HeaderCell.Style.Alignment = value;
                            }
                        }
                        else
                        {
                            column.HeaderCell.Style.Alignment = value;
                        }

                    }
                }
            }
        }

		/// <summary>
		/// Class used to access the indexed ColPos property.
		/// </summary>
		public class ColPosProperty
        {
            /// <summary>
            /// Parent control.
            /// </summary>
            public DataGridViewFlex parent;
			/// <summary>
			/// Creates a ColPosProperty class for the specified grid.
			/// </summary>
			/// <param name="parent">The grid to use to create the ColPosProperty class.</param>
            public ColPosProperty(DataGridViewFlex parent)
            {
                this.parent = parent;
            }
			/// <summary>
			/// Enumerate the columns of the grid.
			/// </summary>
			/// <param name="index">The index of the column</param>
			/// <returns>The ColPos value for the specified column.</returns>
            public int this[int index]
            {
                get
                {
                    if (index == 0)
                        return 0;
                    else
                    {
						int result = parent.RowHeadersVisible ? parent.RowHeadersWidth : 0;
						if (parent.FirstDisplayedCell.ColumnIndex <= index)
						{
							for (int i = parent.FirstDisplayedCell.ColumnIndex; i < index - 1; i++)
							{
								result += parent.Columns[i].Width;
							}
						}
						else
						{
							for (int i = parent.FirstDisplayedCell.ColumnIndex; i > index - 1; i--)
							{
								result -= parent.Columns[i - 1].Width;
							}
						}
						return result;
                    }
                }

            }

        }

		/// <summary>
		/// Enumeration class used to access the indexed ColPos property.
		/// </summary>
		public class RowPosProperty
		{
            /// <summary>
            /// Parent control.
            /// </summary>
			public DataGridViewFlex parent;
			/// <summary>
			/// Creates a RowPosProperty class for the specified grid.
			/// </summary>
			/// <param name="parent">The grid to use to create the RowPosProperty class.</param>
			public RowPosProperty(DataGridViewFlex parent)
			{
				this.parent = parent;
			}
			/// <summary>
			/// Enumerate the rows of the grid.
			/// </summary>
			/// <param name="index">The index of the rows</param>
			/// <returns>The RowPos value for the specified row.</returns>
			public int this[int index]
			{
				get
				{
					if (index == 0)
						return 0;
					else
					{
						int result = parent.ColumnHeadersVisible ? parent.ColumnHeadersHeight : 0;
						if (parent.FirstDisplayedCell.RowIndex <= index)
						{
							for (int i = parent.FirstDisplayedCell.RowIndex; i < index - 1; i++)
							{
								result += parent.Rows[i].Height;
							}
						}
						else
						{
							for (int i = parent.FirstDisplayedCell.RowIndex; i > index - 1; i--)
							{
								result -= parent.Rows[i - 1].Height;
							}
						}
						return result;
					}
				}

			}

		}

		/// <summary>
		/// Enumeration class used to access the indexed FixedAlignment property.
		/// </summary>
        public class FixedAlignmentProperty
        {
            /// <summary>
            /// Parent control.
            /// </summary>
            public DataGridViewFlex parent;
			/// <summary>
			/// Creates a FixedAlignmentProperty class for a specified grid.
			/// </summary>
			/// <param name="parent">The grid for which to create the class.</param>
            public FixedAlignmentProperty(DataGridViewFlex parent) { this.parent = parent; }
			/// <summary>
			/// Obtains the FixedAlignment property for the specified column.
			/// </summary>
			/// <param name="index">The index of the column.</param>
			/// <returns>The Fixed Alignment of the column.</returns>
            public DataGridViewContentAlignment this[int index]
            {
                get
                {
                    if (index == 0)
                    {
                        if (parent.RowHeadersVisible)
                        {
                            return parent.RowHeadersDefaultCellStyle.Alignment;
                        }
                    }
                    int realindex = parent.RowHeadersVisible ? index - 1 : index;
                    DataGridViewColumn column = parent.Columns[realindex];
                    if (column.Frozen)
                        return column.DefaultCellStyle.Alignment;
                    else
                        return DataGridViewContentAlignment.NotSet;
                }
                set
                {
                    if (index == 0)
                    {
                        if (parent.RowHeadersVisible)
                        {
                            parent.RowHeadersDefaultCellStyle.Alignment = value;
                            return;
                        }
                    }
                    int realindex = parent.RowHeadersVisible ? index - 1 : index;
                    DataGridViewContentAlignment align = value;
                    align = align == DataGridViewContentAlignment.NotSet ? DataGridViewContentAlignment.MiddleLeft : align;
                    DataGridViewColumn column = parent.Columns[realindex];
                    if (column.Frozen)
                        column.DefaultCellStyle.Alignment = align;
                    else
                        column.HeaderCell.Style.Alignment = align;
                }
            }
        }


        private class ClipClass
        {
            private int minrow, mincol, maxrow, maxcol, rowcount, colcount;

            private void InitValues(DataGridViewCell[] cells)
            {
                minrow = int.MaxValue;
                mincol = int.MaxValue;
                maxrow = int.MinValue;
                maxcol = int.MinValue;
                rowcount = 0;
                colcount = 0;
                List<int> rows = new List<int>();
                List<int> cols = new List<int>();

                foreach (DataGridViewCell cell in cells)
                {
                    if (!rows.Contains(cell.RowIndex))
                    {
                        rows.Add(cell.RowIndex);
                        if (cell.RowIndex < minrow)
                            minrow = cell.RowIndex;

                        if (cell.RowIndex > maxrow)
                            maxrow = cell.RowIndex;
                    }
                    if (!cols.Contains(cell.ColumnIndex))
                    {
                        cols.Add(cell.ColumnIndex);
                        if (cell.ColumnIndex < mincol)
                            mincol = cell.ColumnIndex;

                        if (cell.ColumnIndex > maxcol)
                            maxcol = cell.ColumnIndex;
                    }
                }
                rowcount = rows.Count;
                colcount = cols.Count;
            }

            public string GetClip(DataGridViewCell[] cells)
            {
                if (cells.Length == 0)
                    return "";

                string[][] Content = GetContent(cells);
                return FormatContent(Content);
            }

            public string[][] GetContent(DataGridViewCell[] cells)
            {
                //Calculates the min, max and the count of rows and cols
                InitValues(cells);

                string[][] Content = new string[rowcount][];
                for (int i = 0; i < rowcount; i++)
                {
                    Content[i] = new string[colcount];
                }

                foreach (DataGridViewCell cell in cells)
                {
                    int rowpos = cell.RowIndex - minrow;
                    int colpos = cell.ColumnIndex - mincol;

                    Content[rowpos][colpos] = cell.Value + "";
                }
                return Content;
            }

            public string FormatContent(string[][] Content)
            {
                //Pass the content to a String
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < rowcount; i++)
                {
                    for (int j = 0; j < colcount; j++)
                    {
                        stringBuilder.Append(Content[i][j]);
                        if (j + 1 != colcount)
                            stringBuilder.Append("\t");
                    }
                    if (i + 1 != rowcount)
                        stringBuilder.AppendLine();
                }
                return stringBuilder.ToString();
            }

            public void SetClip(DataGridViewCell[] cells, string value)
            {
                if (cells.Length == 0)
                    return;

                //Calculates the min, max and the count of rows and cols
                InitValues(cells);
                string[][] Content = new string[rowcount][];
                for (int i = 0; i < rowcount; i++)
                {
                    Content[i] = new string[colcount];
                }

                string[] rowValues = value.Split('\n', '\r');
                for (int i = 0; i < rowValues.Length && i + minrow <= maxrow; i++)
                {
                    string[] colValues = rowValues[i].Split('\t');
                    for (int j = 0; j < colValues.Length && j + mincol <= maxcol; j++)
                    {
                        Content[i][j] = colValues[j].Trim();
                    }
                }
                SetContent(Content, cells);
            }

            public void SetContent(string[][] Content, DataGridViewCell[] cells)
            {
                //Pass the value to the selected cells
                foreach (DataGridViewCell cell in cells)
                {
                    int rowpos = cell.RowIndex - minrow;
                    int colpos = cell.ColumnIndex - mincol;
                    if (Content[rowpos][colpos] != null)
                        cell.Value = Content[rowpos][colpos];
                }
            }
        }

		/// <summary>
		/// This class represents a custom column to be used with the grid.
		/// </summary>
        public class CustomColumn : DataGridViewColumn
        {
			/// <summary>
			/// Creates a new CustomColumn with CustomCell template.
			/// </summary>
            public CustomColumn() : base(new CustomCell())
            {

            }


			/// <summary>
			/// Gets a copy of the CustomColumn.
			/// </summary>
			/// <returns>A copy of the column.</returns>
            public override object Clone()
            {
                CustomColumn col = base.Clone() as CustomColumn;
                col.CellTemplate = this.CellTemplate;
                return col;
            }

        }
        /// <summary>
        /// Custom Cell Class
        /// </summary>
        public class CustomCell : DataGridViewTextBoxCell
        {
            #region "Properties"
            private Image _cellPicture;
            /// <summary>
            /// Gets/Sets the Image of the Cell
            /// </summary>
            [DefaultValue(null)]
            public Image CellPicture
            {
                get { return _cellPicture; }
                set { _cellPicture = value; }
            }

            private DataGridViewContentAlignment _cellPictureAlignment = DataGridViewContentAlignment.NotSet;
            /// <summary>
            /// Gets/Sets the Alignement of the CellPicture
            /// </summary>
            [DefaultValue(DataGridViewContentAlignment.NotSet)]
            public DataGridViewContentAlignment CellPictureAlignment
            {
                get { return _cellPictureAlignment; }
                set { _cellPictureAlignment = value; }
            }

            private DataGridViewImageCellLayout _imageLayout = DataGridViewImageCellLayout.NotSet;
            /// <summary>
            /// Gets/Sets the ImageLayout
            /// </summary>
            [DefaultValue(0)]
            public DataGridViewImageCellLayout ImageLayout
            {
                get { return _imageLayout; }
                set { _imageLayout = value; }
            }

            /// <summary>
            /// Indicates if the Cell has the focus
            /// </summary>
            public bool Focused
            {
                get
                {
                    DataGridViewFlex grid = ParentGrid;
                    return grid != null && grid.GeneralCurrentCell == this && grid.FocusRect != FocusRectSettings.FocusNone;
                }
            }

            /// <summary>
            /// Indicates if the Cell must be HighLighted or not
            /// </summary>
            public bool HighLighted
            {
                get
                {
                    DataGridViewFlex grid = ParentGrid;
                    return grid.HighLight != HighLightSettings.HighlightNever;
                }
            }

            /// <summary>
            /// Returns the XDataGridView that contains this cell
            /// </summary>
            public DataGridViewFlex ParentGrid
            {
                get { return this.DataGridView as DataGridViewFlex; }
            }

            /// <summary>
            /// Indicates if the Parent has the focus
            /// </summary>
            public bool ParentIsFocused
            {
                get
                {
                    DataGridViewFlex grid = ParentGrid;
                    return grid != null && grid.Focused;
                }
            }

            /// <summary>
            /// Returns the type of the Focus
            /// </summary>
            public FocusRectSettings FocusRect
            {
                get
                {
                    DataGridViewFlex grid = ParentGrid;
                    return grid != null ? grid.FocusRect : FocusRectSettings.FocusNone;
                }
            }
            #endregion

            #region "Methods"


            private static Rectangle ImgBounds(Rectangle bounds, int imgWidth, int imgHeight, DataGridViewImageCellLayout imageLayout, DataGridViewContentAlignment Alignment)
            {
                Rectangle empty = Rectangle.Empty;
                switch (imageLayout)
                {
                    case DataGridViewImageCellLayout.NotSet:
                    case DataGridViewImageCellLayout.Normal:
                        empty = new Rectangle(bounds.X, bounds.Y, imgWidth, imgHeight);
                        break;

                    case DataGridViewImageCellLayout.Zoom:
                        if ((imgWidth * bounds.Height) >= (imgHeight * bounds.Width))
                        {
                            empty = new Rectangle(bounds.X, bounds.Y, bounds.Width, decimal.ToInt32((imgHeight * bounds.Width) / imgWidth));
                            break;
                        }
                        empty = new Rectangle(bounds.X, bounds.Y, decimal.ToInt32((imgWidth * bounds.Height) / imgHeight), bounds.Height);
                        break;
                }
                switch (Alignment)
                {
                    case DataGridViewContentAlignment.MiddleRight:
                        empty.X = bounds.Right - empty.Width;
                        goto Label_025B;

                    case DataGridViewContentAlignment.BottomLeft:
                        empty.X = bounds.X;
                        goto Label_025B;

                    case DataGridViewContentAlignment.BottomRight:
                        empty.X = bounds.Right - empty.Width;
                        goto Label_025B;

                    case DataGridViewContentAlignment.TopLeft:
                        empty.X = bounds.X;
                        goto Label_025B;

                    case DataGridViewContentAlignment.TopRight:
                        empty.X = bounds.Right - empty.Width;
                        goto Label_025B;

                    case DataGridViewContentAlignment.MiddleLeft:
                        empty.X = bounds.X;
                        goto Label_025B;
                }
            Label_025B:
                switch (Alignment)
                {
                    case DataGridViewContentAlignment.TopCenter:
                    case DataGridViewContentAlignment.MiddleCenter:
                    case DataGridViewContentAlignment.BottomCenter:
                        empty.X = bounds.X + ((bounds.Width - empty.Width) / 2);
                        break;
                }
                DataGridViewContentAlignment alignment = Alignment;
                if (alignment <= DataGridViewContentAlignment.MiddleCenter)
                {
                    switch (alignment)
                    {
                        case DataGridViewContentAlignment.TopLeft:
                        case DataGridViewContentAlignment.TopCenter:
                        case DataGridViewContentAlignment.TopRight:
                            empty.Y = bounds.Y;
                            return empty;

                        case (DataGridViewContentAlignment.TopCenter | DataGridViewContentAlignment.TopLeft):
                            return empty;

                        case DataGridViewContentAlignment.MiddleLeft:
                        case DataGridViewContentAlignment.MiddleCenter:
                            goto Label_030C;
                    }
                    return empty;
                }
                if (alignment <= DataGridViewContentAlignment.BottomLeft)
                {
                    switch (alignment)
                    {
                        case DataGridViewContentAlignment.MiddleRight:
                            goto Label_030C;

                        case DataGridViewContentAlignment.BottomLeft:
                            goto Label_032E;
                    }
                    return empty;
                }
                switch (alignment)
                {
                    case DataGridViewContentAlignment.BottomCenter:
                    case DataGridViewContentAlignment.BottomRight:
                        goto Label_032E;

                    default:
                        return empty;
                }
            Label_030C:
                empty.Y = bounds.Y + ((bounds.Height - empty.Height) / 2);
                return empty;
            Label_032E:
                empty.Y = bounds.Bottom - empty.Height;
                return empty;
            }

            private void PaintImage(Graphics g, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts, bool computeContentBounds, bool computeErrorIconBounds, bool paint, Color backcolor)
            {
                Rectangle cellValueBounds = cellBounds;
                Rectangle rectangle3 = this.BorderWidths(advancedBorderStyle);
                cellValueBounds.Offset(rectangle3.X, rectangle3.Y);
                cellValueBounds.Width -= rectangle3.Right;
                cellValueBounds.Height -= rectangle3.Bottom;

                Rectangle destRect = cellValueBounds;
                if (cellStyle.Padding != Padding.Empty)
                {
                    destRect.Offset(cellStyle.Padding.Left, cellStyle.Padding.Top);
                    destRect.Width -= cellStyle.Padding.Horizontal;
                    destRect.Height -= cellStyle.Padding.Vertical;
                }

                if ((destRect.Width > 0) && (destRect.Height > 0))
                {
                    Image image = formattedValue as Image;
                    Icon icon = null;
                    if (image == null)
                    {
                        icon = formattedValue as Icon;
                    }
                    if ((icon != null) || (image != null))
                    {
                        if (paint)
                        {
                            switch (ImageLayout)
                            {
                                case DataGridViewImageCellLayout.NotSet:

                                    break;
                                case DataGridViewImageCellLayout.Normal:
                                    break;
                                case DataGridViewImageCellLayout.Stretch:
                                    if (paint)
                                    {
                                        if (image != null)
                                        {
                                            ImageAttributes imageAttr = new ImageAttributes();
                                            imageAttr.SetWrapMode(WrapMode.TileFlipXY);
                                            g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttr);
                                            imageAttr.Dispose();
                                        }
                                        else
                                        {
                                            g.DrawIcon(icon, destRect);
                                        }
                                    }
                                    break;
                            }
                        }
                    }

                    if (image != null)
                    {
                        g.FillRectangle(new SolidBrush(backcolor), destRect);
                    }
                    Rectangle empty = ImgBounds(destRect, (image == null) ? icon.Width : image.Width, (image == null) ? icon.Height : image.Height, ImageLayout, CellPictureAlignment);
                    if (paint)
                    {
                        Region clip = g.Clip;
                        g.SetClip(Rectangle.Intersect(Rectangle.Intersect(empty, destRect), Rectangle.Truncate(g.VisibleClipBounds)));
                        if (image != null)
                        {
                            g.DrawImage(image, empty);
                        }
                        else
                        {
                            g.DrawIconUnstretched(icon, empty);
                        }
                        g.Clip = clip;
                    }
                }
            }
            #endregion
        }

        #endregion

        
        /// <summary>
        /// Returns the Cell in the given Row and Column
        /// </summary>
        /// <param name="rowindex">The Row</param>
        /// <param name="colIndex">The Column</param>
        /// <returns>The Cell specified</returns>
        public DataGridViewCell GetCell(int rowindex, int colIndex)
        {
            if (colIndex < 0 || rowindex < 0)
                return null;
            else if (colIndex == 0 && rowindex == 0)
            {
                if (RowHeadersVisible && ColumnHeadersVisible)
                    return TopLeftHeaderCell;
                else if (RowHeadersVisible)
                    return base.Rows[0].HeaderCell;
                else if (ColumnHeadersVisible)
                    return Columns[0].HeaderCell;
            }
            else if (colIndex == 0)
            {
                if (RowHeadersVisible && ColumnHeadersVisible)
                    return base.Rows[rowindex - 1].HeaderCell;
                else if (RowHeadersVisible)
                {
                    return base.Rows[rowindex].HeaderCell;
                }
                else if (ColumnHeadersVisible)
                {
                    int realRow = rowindex - 1;
                    return base[0, realRow];
                }
            }
            else if (rowindex == 0)
            {
                if (RowHeadersVisible && ColumnHeadersVisible)
                    return Columns[colIndex - 1].HeaderCell;
                else if (RowHeadersVisible)
                    return base[colIndex - 1, 0];
                else if (ColumnHeadersVisible)
                    return Columns[colIndex].HeaderCell;
            }

            if (ColumnHeadersVisible && RowHeadersVisible)
            {
                int realCol = ColumnHeadersVisible ? colIndex -1 : colIndex;
                int realRow = RowHeadersVisible ? rowindex-1 : rowindex;
                return base[realCol, realRow];
            }
            else if (RowHeadersVisible)
            {
                int realCol = colIndex - 1;
                int realRow = rowindex;
                return base[realCol, realRow];
            }
            else if (ColumnHeadersVisible)
            {
                int realCol = colIndex;
                int realRow = rowindex - 1;
                return base[realCol, realRow];
            }
            else
            {
                int realRow = rowindex;
                int realCol = colIndex;
                return base[realCol, realRow];
            }
        }

        #region FlexGrid


        void changeService_ComponentChanging(object sender, ComponentChangingEventArgs e)
        {
            Control c = e.Component as Control;
            DataGridViewFlex grid = c as DataGridViewFlex;
            if (c != null && grid != null)
            {
               // throw new InvalidOperationException(Resources.MSFlexGridColumnEditor);
            }
        }

        /// <summary>
        /// Returns or sets a value that determines whether clicking on a column or row header should cause the entire column or row to be selected.
        /// </summary>
        [Description("Returns or sets a value that determines whether clicking on a column or row header should cause the entire column or row to be selected."), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(true)]
        bool _allowBigSelection = true;
        /// <summary>
        /// Returns/sets flag to activate BigSelection
        /// </summary>
        public bool AllowBigSelection
        {
            get
            {
                return _allowBigSelection;
            }
            set
            {
                _allowBigSelection = value;
            }
        }

        #endregion

        /// <summary>
        /// Flag to indicate Select All
        /// </summary>
        public bool selectAllFlag;
        /// <summary>
        /// Flag to indicate Row Selection
        /// </summary>
        public bool rowSelection;
        /// <summary>
        /// Flag to indicate Column Selection
        /// </summary>
        public bool colSelection;


        private DataGridViewSelectionMode? originalSelectionMode;


        /// <summary>
        /// Overrides the method of the base parent
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if(originalSelectionMode == null)
                originalSelectionMode = this.SelectionMode;
            else
                this.SelectionMode = originalSelectionMode.Value;

            if (e.ColumnIndex == -1 && e.RowIndex == -1 && AllowBigSelection)
            {
                selectAllFlag = true;
            }
            else if (e.ColumnIndex == -1)
            {
                
                if (SelectionMode == DataGridViewSelectionMode.CellSelect && AllowBigSelection)
                {
                    SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                    rowSelection = true;
                    colSelection = false;
                    SelectRowHeader(e.RowIndex);
                }
            }
            else if (e.RowIndex == -1)
            {
                if (SelectionMode == DataGridViewSelectionMode.CellSelect && AllowBigSelection)
                {
                    SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
                    rowSelection = false;
                    colSelection = true;
                    SelectColumnHeader(e.ColumnIndex);
                }
            }
            else
            {
                if (SelectionMode == DataGridViewSelectionMode.CellSelect)
                {
                    SelectionMode = DataGridViewSelectionMode.CellSelect;
                    rowSelection = false;
                    colSelection = false;
                }
            }
            base.OnCellMouseDown(e);
            // Return to the original value after selection.
        }

		/// <summary>
		/// Called when a Column Header is clicked.
		/// </summary>
		/// <param name="e">The Event arguments.</param>
		protected override void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs e)
		{
			base.OnColumnHeaderMouseClick(e);
			foreach (DataGridViewColumn column in Columns)
			{
				column.Selected = (column.Index == e.ColumnIndex);
			}
		}
        /// <summary>
        /// Select Column Header
        /// </summary>
        /// <param name="index">column position</param>
        public void SelectColumnHeader(int index)
        {
            foreach (DataGridViewRow row in Rows)
            {
                row.Cells[index].Selected = true;
                row.Cells[index].Style.SelectionForeColor = Color.White;
            }
        }
        /// <summary>
        /// Selects Row Header
        /// </summary>
        /// <param name="index">column position</param>
        public void SelectRowHeader(int index)
        {
            foreach (DataGridViewCell cell in Rows[index].Cells)
            {
                cell.Selected = true;
                cell.Style.SelectionForeColor = Color.White;
            }
        }


        /// <summary>
        /// Indicates if a value is numeric
        /// </summary>
        /// <param name="value">The object to be evaluated</param>
        /// <returns>True if it is numeric</returns>
        public static bool IsNumeric(object value)
        {
            double d;
            if (value is int || value is Double)
                return true;
            else
            {
                string strValue = value as string;
                if (!string.IsNullOrEmpty(strValue) && Double.TryParse(strValue, out d))
                    return true;
            }

            return false;
        }


    }


	/// <summary>
	/// This structure stores settings for resizing grids.
	/// </summary>
    public struct GridResizeSettings
    {
		/// <summary>
		/// Indicates if rows should be allowed to be resized.
		/// </summary>
        public bool Rows;
		/// <summary>
		/// Indicates if columns should be allowed to be resized.
		/// </summary>
		public bool Columns;
		/// <summary>
		/// Does not allow either rows nor columns to be resized.
		/// </summary>
		/// <returns>The GridResizeSettings that do not allow the user to do any resizing.</returns>
        public static GridResizeSettings ResizeNone()
        {
            GridResizeSettings newVal = new GridResizeSettings();
            newVal.Rows = false;
            newVal.Columns = false;
            return newVal;
        }
		/// <summary>
		/// Allows both rows and columns to be resized.
		/// </summary>
		/// <returns>The GridResizeSettings that allow the user to resize both rows and columns.</returns>
        public static GridResizeSettings ResizeBoth()
        {
            GridResizeSettings newVal = new GridResizeSettings();
            newVal.Rows = true;
            newVal.Columns = false;
            return newVal;
        }
		/// <summary>
		/// Allows columns to be resized, but not rows.
		/// </summary>
		/// <returns>The GridResizeSettings that allow the user to resize columns, but not rows</returns>
        public static GridResizeSettings ResizeColumns()
        {
            GridResizeSettings newVal = new GridResizeSettings();
            newVal.Rows = false;
            newVal.Columns = true;
            return newVal;
        }
		/// <summary>
		/// Allows rows to be resized, but not columns.
		/// </summary>
		/// <returns>The GridResizeSettings that allow the user to resize rows, but not columns</returns>
		public static GridResizeSettings ResizeRows()
        {
            GridResizeSettings newVal = new GridResizeSettings();
            newVal.Rows = true;
            newVal.Columns = false;
            return newVal;
        }

		/// <summary>
		/// Apply the grid resize settings to the specified grid.
		/// </summary>
		/// <param name="grid">The grid to which the settings should be applied.</param>
        public void Apply(DataGridViewFlex grid)
        {
            grid.AllowUserToResizeColumns = Columns;
            grid.AllowUserToResizeRows = Rows;
        }

		/// <summary>
		/// Obtains the GridResizeSettings of a specified grid.
		/// </summary>
		/// <param name="grid">The grid from which to obtain the GridResizeSettings.</param>
		/// <returns>The GridResizeSettings of the specified grid.</returns>
        public static GridResizeSettings GetCurrent(DataGridViewFlex grid)
        {
            GridResizeSettings newVal = new GridResizeSettings();
            newVal.Rows = grid.AllowUserToResizeRows;
            newVal.Columns = grid.AllowUserToResizeColumns;
            return newVal;
        }

        /// <summary>
        /// Operator that compares if two values of type GridResizeSettings are equal
        /// </summary>
        /// <param name="g1">First value of type GridResizeSettings</param>
        /// <param name="g2">Second value of type GridResizeSettings</param>
        /// <returns>True if the values are equal</returns>
        public static bool operator ==(GridResizeSettings g1, GridResizeSettings g2)
        {
            return (g1.Columns == g2.Columns) && (g1.Rows == g2.Rows);
        }

        /// <summary>
        /// Operator that compares if two values of type GridResizeSettings are different
        /// </summary>
        /// <param name="g1">First value of type GridResizeSettings</param>
        /// <param name="g2">Second value of type GridResizeSettings</param>
        /// <returns>True if the values are different</returns>
        public static bool operator !=(GridResizeSettings g1, GridResizeSettings g2)
        {
            return !(g1 == g2);
        }

        /// <summary>
        /// Returns the hash code of this instance
        /// </summary>
        /// <returns>Hash code of this instance</returns>
        public override int GetHashCode()
        {
            return Rows.GetHashCode() ^ Columns.GetHashCode();
        }

        /// <summary>
        /// Indicates if this instance and specified object are equal 
        /// </summary>
        /// <param name="obj">Another object to compare to</param>
        /// <returns>True if the instances are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            GridResizeSettings g = (GridResizeSettings)obj;
            return Rows.Equals(g.Rows) && Columns.Equals(g.Columns);
        }


    }   
}