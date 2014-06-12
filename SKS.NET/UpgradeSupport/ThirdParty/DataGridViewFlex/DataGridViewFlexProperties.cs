// Author: mrojas
// Project: UpgradeHelpers.Windows.Forms
// Path: D:\VbcSPP\src\Helpers\UpgradeHelpers.Windows.Forms\ExtendedDataGridView
// Creation date: 8/6/2009 2:29 PM
// Last modified: 10/8/2009 10:32 AM

#region Using directives
using UpgradeHelpers.Windows.Forms.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections;
using System.Data;
#endregion

namespace UpgradeHelpers.Windows.Forms
{
    /// <summary>
    /// This is class implements a component that extends the
    /// System.Windows.Forms.DataGridView control.  It adds new properties and also
    /// provides &quot;Compatibility&quot; support for some Grid controls commonly used
    /// in  VB6: MSFlexGrid and APEX TrueDBGrid
    /// </summary>
    public partial class DataGridViewFlex
    {

        /// <summary>
        /// Determines the total number of columns or rows in a FlexGrid.
        /// </summary>
        [Description("Set or gets the total number of columns"),
        Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ColumnsCount
        {
            get
            {
                if (isInitializing)
                    return GetValueIfFound("ColumnsCount", UNSETVALUE);
                else
                    return Columns.Count + (RowHeadersVisible ? 1 : 0);
            }
            set
            {
                if (isInitializing || DesignMode)
                {
                    myValues["ColumnsCount"] = value;
                }
                else
                {
                    if (value < 0)
                    {
                        throw new ArgumentException(Resources.ValueZeroOrGreater);
                    }

                    //Validating
                    int _fixedColumns = FixedColumns;
                    if (_fixedColumns >= value)
                    {
                        if (_fixedColumns != 0)
                            if (DesignMode)
                                throw new ArgumentException(Resources.FixedColumnLessThanColumnsCount);
                    }

                    //Remove all Items, Value is 0
                    if (value == 0)
                    {
                        Rows.Clear();
                        Columns.Clear();
                        RowHeadersVisible = false;
                        return;
                    }

                    int realvalue = RowHeadersVisible ? value - 1 : value;
                    if (Columns.Count < realvalue)
                    {
                        int oldStart = Columns.Count;
                        for (int i = Columns.Count; i < realvalue; i++)
                        {
                            CustomColumn newCustomColumn = new CustomColumn();
                            newCustomColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                            newCustomColumn.HeaderText = "";
                            newCustomColumn.Name = "Column" + Columns.Count;
                            newCustomColumn.DividerWidth = GridLineWidth; //TODO _gridLineWidth;
                            newCustomColumn.Width = DEFAULT_NEW_CUSTOM_COLUMN_WIDTH;
                            Columns.Add(newCustomColumn);
                        }

                        DataGridViewCellStyle cellstyleFixed = GetCellStyleFixed();
                        DataGridViewCellStyle cellstyleNormal = GetCellStyleNonFixed();
                        //Now we have to check the fixed rows
                        int fixedRows = FixedRows;
                        foreach (DataGridViewRow row in Rows)
                        {
                            if (row.Index < fixedRows)
                            {
                                foreach (DataGridViewCell cell in row.Cells)
                                    cell.Style = cellstyleFixed;

                            }
                            else break;
                        } // for
                    }
                    else if (Columns.Count > realvalue)
                    {
                        for (int i = Columns.Count - 1; i >= realvalue; i--)
                        {
                            Columns.RemoveAt(i);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets/Sets the current amount of rows based on the current grid's behaviour
        /// </summary>
        /// <summary>
        /// Returns/Sets the total number of rows
        /// </summary>
        public int RowsCount
        {
            get
            {
                if (isInitializing)
                    return GetValueIfFound("RowsCount", UNSETVALUE);
                else
                    return Rows.Count + (ColumnHeadersVisible ? 1 : 0);
            }
            set
            {
                if (isInitializing)
                {
                    myValues["RowsCount"] = value;
                }
                else
                {
                    #region Validating
                    if (value < 0)
                        throw new ArgumentException(Resources.ValueZeroOrGreater);
                    int _fixedRows = FixedRows;
                    if (_fixedRows > value)
                    {
                        if (_fixedRows != 0 && DesignMode)
                            throw new ArgumentException(Resources.FixedRowsLessThanRowsCount);
                    }
                    #endregion
                    //Remove all Items, Value is 0
                    if (value == 0)
                    {
                        Rows.Clear();
                        ColumnHeadersVisible = false;
                        return;
                    }

                    int realvalue = ColumnHeadersVisible ? value - 1 : value;
                    if (Rows.Count < realvalue)
                    {
                        //Add A Column (because an Exception is thrown when no columns are available)
                        if (Columns.Count == 0)
                        {
                            CustomColumn customColumn = new CustomColumn();
                            customColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                            customColumn.HeaderText = "";
                            customColumn.Name = "Column" + Columns.Count;
                            customColumn.DividerWidth = _gridLineWidth;
                            customColumn.Width = DEFAULT_NEW_CUSTOM_COLUMN_WIDTH;
                            Columns.Add(customColumn);
                        }
                        Rows.Add(realvalue - Rows.Count);
                    }
                    else if (Rows.Count > realvalue)
                    {
                        for (int i = Rows.Count - 1; i >= realvalue; i--)
                        {
                            Rows.RemoveAt(i);
                        }
                    }
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Sets/Gets the index for the Column that currently has the focus
        /// Returns the currently selected column. This is not a design time property
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CurrentColumnIndex
        {
            get
            {
                if (GeneralCurrentCell != null)
                    return ColumnHeadersVisible ? GeneralCurrentCell.ColumnIndex + 1 : GeneralCurrentCell.ColumnIndex;
                else if (CurrentCell != null)
                {
                    return ColumnHeadersVisible ? CurrentCell.ColumnIndex + 1 :
                        CurrentCell.ColumnIndex;
                }
                else
                    return 0;
            }
            set
            {
                if ((value < 0) || (value > Columns.Count))
                    throw new IndexOutOfRangeException("Index must be between 0 and the Count of Columns");
                DataGridViewCell cell = GetCell(CurrentRowIndex, value);
                _colSel = value;
                if (cell is DataGridViewHeaderCell || !cell.Visible)
                {
                    GeneralCurrentCell = cell;
                }
                else
                {
                    GeneralCurrentCell = cell;
                }
            }
        }

        /// <summary>
        /// Sets/Gets the index for the Row that currently has the focus
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CurrentRowIndex
        {
            get
            {
                if (GeneralCurrentCell != null)
                    return RowHeadersVisible ? GeneralCurrentCell.RowIndex + 1 : GeneralCurrentCell.RowIndex;
                else if (CurrentCell != null)
                {
                    return RowHeadersVisible ? CurrentCell.RowIndex + 1 : CurrentCell.RowIndex;
                }
                else
                    return 0;
            }
            set
            {
                if ((value < 0) || (value > Rows.Count))
                    throw new IndexOutOfRangeException(Resources.InvalidCurrentRowIndex);

                DataGridViewCell cell = GetCell(value, CurrentColumnIndex);
                _rowSel = value;
                if (cell is DataGridViewHeaderCell || !cell.Visible)
                {
                    GeneralCurrentCell = cell;
                }
                else
                {
                    GeneralCurrentCell = cell;
                }
            }
        }

        /// <summary>
        /// Overrides base FirstDisplayedScrollingColumnIndex and delegates the handling of the 
        /// property down onto the specific grid behaviour implementation.
        /// Gets/sets the index of the first displayed column, not including fixed columns.
        /// </summary>
        public new int FirstDisplayedScrollingColumnIndex
        {
            get
            {
                return RowHeadersVisible ? base.FirstDisplayedScrollingColumnIndex + 1 :
                    base.FirstDisplayedScrollingColumnIndex;
            }
            set
            {
                if (DesignMode && value < 0)
                {
                    throw new ArgumentException(Resources.ValueZeroOrGreater);
                }
                if (value > 0)
                    base.FirstDisplayedScrollingColumnIndex = RowHeadersVisible ? value - 1 : value;
            }
        }

        /// <summary>
        /// Overrides base FirstDisplayedScrollingRowIndex and delegates the handling of the 
        /// property down onto the specific grid behaviour implementation.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int FirstDisplayedScrollingRowIndex
        {
            get { return ColumnHeadersVisible ? base.FirstDisplayedScrollingRowIndex + 1 : base.FirstDisplayedScrollingRowIndex; }
            set
            {
                if (DesignMode && value < 0)
                {
                    throw new ArgumentException(Resources.ValueZeroOrGreater);
                }
                if (value > 0)
                    base.FirstDisplayedScrollingRowIndex = ColumnHeadersVisible ? value - 1 : value;
            }
        }



        /// <summary>
        /// Returns/sets the width in Pixels of the gridlines for the control.
        /// </summary>
        [Description("Returns/sets the width in Pixels of the gridlines for the control."),
        Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int GridLineWidth
        {
            get
            {
                if (isInitializing)
                    return GetValueIfFound("GridLineWidth", UNSETVALUE);
                else
                    return _gridLineWidth + 1;
            }
            set
            {
                if (isInitializing)
                {
                    myValues["GridLineWidth"] = value;

                }
                else
                {
                    if (value <= 0)
                        throw new ArgumentException(Resources.ValueGreaterThanZero);
                    if (BorderStyle == BorderStyle.FixedSingle)
                    {
                        _gridLineWidth = value - 1;
                        RowTemplate.DividerHeight = _gridLineWidth;
                        foreach (DataGridViewRow row in Rows)
                        {
                            row.DividerHeight = _gridLineWidth;
                        }
                        foreach (DataGridViewColumn col in Columns)
                        {
                            col.DividerWidth = _gridLineWidth;
                        }
                    }
                    else
                    {
                        //TODO: value must be ignored?
                    }
                }
            }
        }

        /// <summary>
        /// Obtains the row over which the mouse is currently positioned.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MouseRow
        {
            get
            {
                return mouse_cell_row;
            }
        }


        /// <summary>
        /// Obtains the column over which the mouse is currently positioned.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MouseCol
        {
            get { return mouse_cell_column; }
        }


        /// <summary>
        /// Gets/sets the minimum row height allowed for the grid.
        /// </summary>
        [Description("Returns/sets a minimum row height for the entire control"),
        Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int RowHeightMin
        {
            get
            {
                if (isInitializing)
                    return GetValueIfFound("RowHeightMin", UNSETVALUE);
                else
                    return Rows.Count > 0 ? Rows[0].MinimumHeight : 0;
            }
            set
            {
                if (isInitializing)
                {
                    myValues["RowHeightMin"] = value;

                }
                else
                {
                    if (DesignMode && value < 0)
                    {
                        throw new ArgumentException(Resources.ValueZeroOrGreater);
                    }
                    if (value > 0)
                    {
                        foreach (DataGridViewRow row in Rows)
                        {
                            row.MinimumHeight = value;
                        }
                    }
                }
            }
        }

        object _dataSource;
        /// <summary>
        /// Allows access to the base data source
        /// </summary>
        public new object DataSource
        {
            get
            {
                if (isInitializing)
                    return GetValueIfFound<object>("DataSource", null);
                else
                    return _dataSource;
            }
            set
            {
                if (isInitializing)
                {
                    myValues["DataSource"] = value;

                }
                else
                {
                    _dataSource = value;
                    OnDataSourceChanged(EventArgs.Empty);
                }
            }
        }

        bool allowRowSelection;

        /// <summary>
        /// If set selects the full row when the user clicks the row header
        /// </summary>
        [Browsable(true), DefaultValue(true),
        Description("If set selects the full row when the user clicks the row header")]
        public bool AllowRowSelection
        {
            get
            {
                if (isInitializing)
                    return GetValueIfFound("AllowRowSelection", false);
                else
                    return allowRowSelection;
            }
            set
            {
                if (isInitializing)
                {
                    myValues["AllowRowSelection"] = value;

                }
                else
                {
                    allowRowSelection = value;
                }
            }
        }

        /// <summary>
        /// Returns the left position of the current cell
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CellLeft
        {
            get
            {
                if (GeneralCurrentCell != null)
                {
                    if (GeneralCurrentCell.ColumnIndex != -1 && GeneralCurrentCell.RowIndex != -1)
                        return GetCellDisplayRectangle(GeneralCurrentCell.ColumnIndex, GeneralCurrentCell.RowIndex, false).Left;
                    return 0;
                }
                else
                    return 0;


            }
        }

        /// <summary>
        /// Returns or sets the top position of the current cell
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CellTop
        {
            get
            {
                if (GeneralCurrentCell != null)
                {
                    if (GeneralCurrentCell.ColumnIndex != -1 && GeneralCurrentCell.RowIndex != -1)
                        return GetCellDisplayRectangle(GeneralCurrentCell.ColumnIndex,
                            GeneralCurrentCell.RowIndex, false).Top;
                    return 0;
                }
                else
                    return 0;
            }
        }

        /// <summary>
        /// Returns/sets the total number of fixed (non-scrollable) columns or rows for a FlexGrid.
        /// </summary>
        [Browsable(true), DefaultValue(2)]
        public int FixedRows
        {
            get
            {

                if (isInitializing)
                    return GetValueIfFound("FixedRows", UNSETVALUE);
                else
                {
                    int result = ColumnHeadersVisible ? 1 : 0;
                    foreach (DataGridViewRow row in Rows)
                    {
                        if (row.Frozen)
                        {
                            result++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    return result;
                }
            }
            set
            {
                if (isInitializing || DesignMode)
                    myValues["FixedRows"] = value;
                else
                {


                    int _rows = RowsCount;
                    //Validating
                    if (value < 0)
                    {
                        throw new ArgumentException(Resources.ValueZeroOrGreater);
                    }

                    if (value >= _rows)
                    {
                        throw new ArgumentException(Resources.FixedRowsLessThanRowsCount);
                    }

                    //The value is the same
                    if (FixedRows == value)
                    {
                        return;
                    }

                    if (value == 0)
                    {
                        ColumnHeadersVisible = false;

                        Rows.Insert(0, 1);

                        for (int i = 0; i < ColumnsCount; i++)
                        {
                            DataGridViewCell cell = GetCell(0, i);
                            int realCol = RowHeadersVisible ? i - 1 : i;
                            if (realCol < 0)
                            {
                                cell.Value = TopLeftHeaderCell.Value;
                            }
                            else
                            {
                                cell.Value = Columns[realCol].HeaderCell.Value;
                            }
                        }

                        for (int i = 0; i < Rows.Count; i++)
                        {
                            Rows[i].Frozen = false;
                        }
                    }
                    else
                    {
                        int realvalue = value - 1;
                        if (!ColumnHeadersVisible)
                        {
                            if (Rows.Count > 0)
                            {
                                for (int i = 0; i < ColumnsCount; i++)
                                {
                                    DataGridViewCell cell = GetCell(0, i);
                                    int realCol = RowHeadersVisible ? i - 1 : i;
                                    if (realCol < 0)
                                    {
                                        TopLeftHeaderCell.Value = cell.Value;
                                    }
                                    else
                                    {
                                        Columns[realCol].HeaderCell.Value = cell.Value;
                                    }
                                }
                                Rows.RemoveAt(0);
                            }
                        }
                        ColumnHeadersVisible = true;
                        DataGridViewCellStyle cellStyleFixed = GetCellStyleFixed();
                        DataGridViewCellStyle cellStyleNormal = GetCellStyleNonFixed();
                        foreach (DataGridViewRow row in Rows)
                        {
                            Boolean Frozen = row.Index < realvalue;
                            row.Frozen = Frozen;
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                cell.Style = Frozen ? cellStyleFixed : cellStyleNormal;
                            } // foreach
                        }
                    }
                    Refresh();
                }
            }
        }


        /// <summary>
        /// Returns/sets the total number of fixed (non-scrollable) columns or rows
        /// </summary>
        [Description("Returns/sets the total number of fixed (non-scrollable) columns or rows for a FlexGrid."), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(1)]
        public int FixedColumns
        {
            get
            {
                if (isInitializing)
                    return GetValueIfFound("FixedColumns", UNSETVALUE);
                else
                {
                    int result = RowHeadersVisible ? 1 : 0;
                    foreach (DataGridViewColumn dataGridViewColumn in Columns)
                    {
                        if (dataGridViewColumn.Frozen)
                            result++;
                        else
                            break;
                    } // foreach
                    return result;
                }
            } // get
            set
            {
                if (isInitializing || DesignMode)
                    myValues["FixedColumns"] = value;
                else
                {
                    int _cols = ColumnsCount;
                    //Validating
                    #region Validating
                    if (value < 0)
                        throw new ArgumentException("Value is not valid");
                    if (value >= _cols && DesignMode)
                        throw new ArgumentException(Resources.FixedColumnLessThanColumnsCount);
                    if (_FixedColumns == value) //The value is the same just exit
                        return;
                    #endregion
                    _FixedColumns = value;
                    if (SelectionStarted)
                    {
                        SelectionMode = DataGridViewSelectionMode.CellSelect;
                        SelectionStarted = false;
                    }
                    if (value == 0)
                    {
                        RowHeadersVisible = false;
                        CustomColumn newCustomColumn = new CustomColumn();
                        newCustomColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                        newCustomColumn.HeaderText = "";
                        newCustomColumn.Name = "Column" + Columns.Count;
                        newCustomColumn.DividerWidth = _gridLineWidth;
                        newCustomColumn.Width = DEFAULT_NEW_CUSTOM_COLUMN_WIDTH;
                        Columns.Insert(0, newCustomColumn);
                        for (int i = 0; i < RowsCount; i++)
                        {
                            DataGridViewCell cell = GetCell(i, 0);
                            int realRow = ColumnHeadersVisible ? i - 1 : i;
                            if (realRow < 0)
                            {
                                cell.Value = TopLeftHeaderCell.Value;
                            }
                            else
                            {
                                cell.Value = Rows[realRow].HeaderCell.Value;
                            }
                        }
                        for (int i = 0; i < Columns.Count; i++)
                        {
                            Columns[i].Frozen = false;
                        }
                    }
                    else
                    {
                        int realvalue = value - 1;
                        DataGridViewCellStyle cellStyleFixed = GetCellStyleFixed();
                        DataGridViewCellStyle cellStyleNormal = GetCellStyleNonFixed();
                        if (!RowHeadersVisible)
                        {
                            if (Columns.Count > 0)
                            {
                                for (int i = 0; i < RowsCount; i++)
                                {
                                    DataGridViewCell cell = GetCell(i, 0);
                                    int realRow = ColumnHeadersVisible ? i - 1 : i;
                                    if (realRow < 0)
                                    {
                                        TopLeftHeaderCell.Value = cell.Value;
                                    }
                                    else
                                    {
                                        Rows[realRow].HeaderCell.Value = cell.Value;
                                    }
                                }
                                Columns.RemoveAt(0);
                            }
                        }
                        RowHeadersVisible = true;
                        for (int i = 0; i < Columns.Count; i++)
                        {
                            bool Frozen = i < realvalue;
                            Columns[i].Frozen = Frozen;

                        }
                        foreach (DataGridViewRow row in Rows)
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                if (row.Frozen) break; //It has already been set.
                                bool Frozen = cell.ColumnIndex < realvalue;
                                cell.Style = Frozen ? cellStyleFixed : cellStyleNormal;
                            } // foreach
                        } // foreach
                    }
                    //In case that then number of Rows non fixed is 0
                    if (_FixedRows != RowsCount)
                    {
                        _generalCurrentCell = GetCell(FixedRows, FixedColumns);
                        CurrentCell = _generalCurrentCell;
                    }
                    Refresh();
                }
            }
        }
        private int _FixedColumns = DEFAULT_FIXED_COLUMNS;
        private int _FixedRows = DEFAULT_FIXED_ROWS;

        /// <summary>
        /// Returns/sets the text contents of a cell or range of cells.
        /// </summary>
        [Description("Returns/sets the text contents of a cell or range of cells."), Browsable(true), DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get { return GeneralCurrentCell != null ? GeneralCurrentCell.Value + "" : base.Text; }
            set
            {
                if (GeneralCurrentCell != null)
                    GeneralCurrentCell.Value = value;
                else
                    base.Text = value;
            }
        }


        /// <summary>
        /// Returns/sets the text contents of a cell or range of cells.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object CellText
        {
            get { return GeneralCurrentCell != null ? GeneralCurrentCell.Value : null; }
            set
            {
                if (GeneralCurrentCell != null)
                    GeneralCurrentCell.Value = value;
            }
        }

        /// <summary>
        /// Returns/sets the background and foreground colors of individual cells or ranges of cells.
        /// Provides compatibility with MSFlexGrid CellBackColor
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color CellBackColor
        {
            get { return GeneralCurrentCell != null ? GeneralCurrentCell.Style.BackColor : BackColor; }
            set
            {
                if (_fillStyle == FillStyleSettings.FillSingle)
                {
                    if (GeneralCurrentCell != null)
                    {
                        if (!GeneralCurrentCell.HasStyle)
                        {
                            DataGridViewCellStyle style = new DataGridViewCellStyle(GeneralCurrentCell.Style);
                            style.BackColor = value;
                            GeneralCurrentCell.Style = style;
                        }
                        else
                        {
                            GeneralCurrentCell.Style.BackColor = value;
                        }
                    }
                }
                else
                {
                    foreach (DataGridViewCell dataGridViewCell in SelectedCells)
                    {
                        if (!dataGridViewCell.HasStyle)
                        {
                            DataGridViewCellStyle style = new DataGridViewCellStyle(dataGridViewCell.Style);
                            style.BackColor = value;
                            dataGridViewCell.Style = style;
                        }
                        else
                        {
                            dataGridViewCell.Style.BackColor = value;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Returns/sets the background and foreground colors of individual cells or ranges of cells.
        /// Provides compatibility with the MSFlexGrid CellForeColor
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color CellForeColor
        {
            get { return GeneralCurrentCell != null ? GeneralCurrentCell.Style.ForeColor : ForeColor; }
            set
            {

                if (_fillStyle == FillStyleSettings.FillSingle)
                {
                    if (GeneralCurrentCell != null)
                    {
                        if (!GeneralCurrentCell.HasStyle)
                        {
                            DataGridViewCellStyle style = new DataGridViewCellStyle(GeneralCurrentCell.Style);
                            style.ForeColor = value;
                            GeneralCurrentCell.Style = style;
                        }
                        else
                        {
                            GeneralCurrentCell.Style.ForeColor = value;
                        }
                    }
                }
                else
                {
                    foreach (DataGridViewCell dataGridViewCell in SelectedCells)
                    {
                        if (!dataGridViewCell.HasStyle)
                        {
                            DataGridViewCellStyle style = new DataGridViewCellStyle(dataGridViewCell.Style);
                            style.ForeColor = value;
                            dataGridViewCell.Style = style;
                        }
                        else
                        {
                            dataGridViewCell.Style.ForeColor = value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns/sets the alignment of data in a cell or range of selected cells. Not available at design time
        /// Provides compatibility with the MSFlexGrid CellAligment behaviour. DataGridViewContentAligment contants
        /// are used, but the property behaviour resembles the equivalent in MSFlexGrid
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataGridViewContentAlignment CellAlignment
        {
            get { return GeneralCurrentCell != null ? GeneralCurrentCell.Style.Alignment : DataGridViewContentAlignment.NotSet; }
            set
            {
                if (_fillStyle == FillStyleSettings.FillSingle)
                {
                    if (GeneralCurrentCell != null)
                    {
                        if (!GeneralCurrentCell.HasStyle)
                        {
                            DataGridViewCellStyle style = new DataGridViewCellStyle(GeneralCurrentCell.Style);
                            style.Alignment = value;
                            GeneralCurrentCell.Style = style;
                        }
                        else
                        {
                            GeneralCurrentCell.Style.Alignment = value;
                        }
                    }
                }
                else
                {
                    foreach (DataGridViewCell dataGridViewCell in SelectedCells)
                    {
                        if (!dataGridViewCell.HasStyle)
                        {
                            DataGridViewCellStyle style = new DataGridViewCellStyle(dataGridViewCell.Style);
                            style.Alignment = value;
                            dataGridViewCell.Style = style;
                        }
                        else
                        {
                            dataGridViewCell.Style.Alignment = value;
                        }
                    }
                }
            }
        }


        /// <summary>
        ///  Thsi property is used to change the alignment of picture in a cell
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataGridViewContentAlignment CellPictureAlignment
        {
            get
            {
                if (GeneralCurrentCell != null)
                {
                    CustomCell cell = GeneralCurrentCell as CustomCell;
                    if (cell != null)
                    {
                        return cell.CellPictureAlignment;
                    }
                }
                return DataGridViewContentAlignment.NotSet;
            }
            set
            {
                if (GeneralCurrentCell != null)
                {
                    CustomCell cell = GeneralCurrentCell as CustomCell;
                    if (cell != null)
                        cell.CellPictureAlignment = value;
                }
            }
        }

        /// <summary>
        /// Holds the value of the currently selected cell column
        /// </summary>
        public int _colSel;

        /// Holds the value of the currently selected cell row
        public int _rowSel;


        /// <summary>
        /// Returns or sets the start or end column for a range of cells
        /// You can use these properties (ColSel/RowSel) to select a specific region of the grid programmatically, 
        /// or to read the dimensions of an area that the user selects into code.
        /// The grid cursor is in the cell at Row, Col. 
        /// The grid selection is the region between rows Row and RowSel and columns Col and ColSel. 
        /// Note that RowSel may be above or below Row, and ColSel may be to the left or to the right of Col.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ColSel
        {
            get
            {
                int maxColSel = CurrentColumnIndex;
                foreach (DataGridViewCell cell in SelectedCells)
                {
                    if (cell.Selected)
                    {
                        int realCol = RowHeadersVisible ? cell.ColumnIndex + 1 : cell.ColumnIndex;
                        if (maxColSel < realCol)
                            maxColSel = realCol;
                    }
                }
                return maxColSel;
            }
            set
            {
                _colSel = value;
                SetSelectedCells(RowSel, _colSel);
            }
        }

        /// <summary>
        /// Returns or sets the start or end column for a range of cells
        /// You can use these properties (ColSel/RowSel) to select a specific region of the grid programmatically, 
        /// or to read the dimensions of an area that the user selects into code.
        /// The grid cursor is in the cell at Row, Col. 
        /// The grid selection is the region between rows Row and RowSel and columns Col and ColSel. 
        /// Note that RowSel may be above or below Row, and ColSel may be to the left or to the right of Col.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int RowSel
        {
            get
            {
                int maxRowSel = CurrentRowIndex;
                foreach (DataGridViewCell cell in SelectedCells)
                {
                    if (cell.Selected)
                    {
                        int realRow = ColumnHeadersVisible ? cell.RowIndex + 1 : cell.RowIndex;
                        if (maxRowSel < realRow)
                            maxRowSel = realRow;
                    }
                }
                return maxRowSel;
            }
            set
            {
                _rowSel = value;
                SetSelectedCells(_rowSel, ColSel);
            }
        }










        /// <summary>
        /// Returns/sets the contents of the cells in a FlexGrid's selected region. Not available at design time.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Clip
        {
            get
            {
                ClipClass clipclass = new ClipClass();
                return clipclass.GetClip(GridCellCollectionToArray(SelectedCells));
            }
            set
            {

                ClipClass clipclass = new ClipClass();
                clipclass.SetClip(GridCellCollectionToArray(SelectedCells), value);
            }
        }

        #region Font Settings

        private void SetCellFont(DataGridViewCell cell, Font font)
        {
            DataGridViewCellStyle style;
            if (cell.Style == DefaultCellStyle)
            {
                //Creates a new style for the Cell (Modifiying the DefaultCellStyle would change all the grid's Appereance)
                style = new DataGridViewCellStyle(DefaultCellStyle);
                cell.Style = style;
            }
            else
            {
                style = cell.Style;
            }
            style.Font = font;
        }

        private void SetCellFont(DataGridViewCell cell, FontStyle fontStyle, bool property)
        {
            DataGridViewCellStyle style = cell.Style;
            Font newFont;
            if (style.Font != null)
            {
                FontStyle newFontStyle = property ? style.Font.Style | fontStyle : style.Font.Style & ~fontStyle;
                newFont = new Font(style.Font, newFontStyle);
            }
            else
            {
                newFont = new Font(this.Font, property ? fontStyle : FontStyle.Regular);
            }

            SetCellFont(cell, newFont);
        }

        private void SetFontStyle(FontStyle style, bool property)
        {
            if (GeneralCurrentCell != null)
            {
                if (_fillStyle == FillStyleSettings.FillSingle)
                {
                    SetCellFont(GeneralCurrentCell, style, property);
                }
                else
                {
                    foreach (DataGridViewCell dataGridViewCell in SelectedCells)
                    {
                        SetCellFont(dataGridViewCell, style, property);
                    }
                }
            }
        }

        /// <summary>
        /// Returns or sets the bold style for the current cell text.
        /// Provides compatibility for MSFlexGrid
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CellFontBold
        {
            get { return GeneralCurrentCell != null ? GeneralCurrentCell.Style.Font.Bold : false; }
            set { SetFontStyle(FontStyle.Bold, value); }
        }

        /// <summary>
        /// Returns or sets the bold style for the current cell text.
        /// Provides compatibility for MSFlexGrid
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CellFontStrikeOut
        {
            get { return GeneralCurrentCell != null ? GeneralCurrentCell.Style.Font.Strikeout : false; }
            set { SetFontStyle(FontStyle.Strikeout, value); }
        }

        /// <summary>
        /// Returns or sets the italic style for the current cell text.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CellFontItalic
        {
            get { return GeneralCurrentCell != null ? GeneralCurrentCell.Style.Font.Italic : false; }
            set { SetFontStyle(FontStyle.Italic, value); }
        }

        /// <summary>
        /// Returns or sets the underline style for the current cell text.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CellFontUnderline
        {
            get { return GeneralCurrentCell != null ? GeneralCurrentCell.Style.Font.Underline : false; }
            set { SetFontStyle(FontStyle.Underline, value); }
        }

        /// <summary>
        /// Returns/sets the font to be used for individual cells or ranges of cells.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CellFontName
        {
            get { return GeneralCurrentCell != null ? GeneralCurrentCell.Style.Font.Name : Font.Name; }
            set
            {
                string realName;
                switch (value)
                {
                    case "MS Sans Serif":
                        realName = "Microsoft Sans Serif";
                        break;
                    default:
                        realName = value;
                        break;
                }

                if (GeneralCurrentCell != null)
                {
                    if (_fillStyle == FillStyleSettings.FillSingle)
                    {
                        Font f = GeneralCurrentCell.Style.Font;
                        f = new Font(realName, f.Size, f.Style, f.Unit);
                        SetCellFont(GeneralCurrentCell, f);
                    }
                    else
                    {
                        foreach (DataGridViewCell dataGridViewCell in SelectedCells)
                        {
                            Font f = dataGridViewCell.Style.Font;
                            f = new Font(realName, f.Size, f.Style, f.Unit);
                            SetCellFont(dataGridViewCell, f);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns or sets the size, in points, for the current cell text.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float CellFontSize
        {
            get { return GeneralCurrentCell != null ? GeneralCurrentCell.Style.Font.Size : Font.Size; }
            set
            {
                if (GeneralCurrentCell != null)
                {
                    if (_fillStyle == FillStyleSettings.FillSingle)
                    {
                        Font f = GeneralCurrentCell.Style.Font;
                        f = new Font(f.Name, value, f.Style, f.Unit);
                        SetCellFont(GeneralCurrentCell, f);
                    }
                    else
                    {
                        foreach (DataGridViewCell dataGridViewCell in SelectedCells)
                        {
                            Font f = dataGridViewCell.Style.Font;
                            f = new Font(f.Name, value, f.Style, f.Unit);
                            SetCellFont(dataGridViewCell, f);
                        }
                    }
                }
            }
        }

        #endregion
        /// <summary>
        /// Returns the height of the current cell
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CellHeight
        {
            get
            {
                if (GeneralCurrentCell != null)
                {
                    if (GeneralCurrentCell.RowIndex == -1)
                        return ColumnHeadersHeight;
                    else
                        return Rows[GeneralCurrentCell.RowIndex].Height;
                }
                return 0;
            }
        }


        /// <summary>
        /// Returns the width of the current cell
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CellWidth
        {
            get
            {
                if (GeneralCurrentCell != null)
                {
                    if (GeneralCurrentCell.ColumnIndex == -1)
                        return RowHeadersWidth;
                    else
                        return GeneralCurrentCell != null ? Columns[GeneralCurrentCell.ColumnIndex].Width : 0;
                }
                return 0;
            }
        }

        /// <summary>
        /// Returns/sets an image to be displayed in the currently selected cells
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image CellPicture
        {
            get
            {
                DataGridViewImageCell imageCell = GeneralCurrentCell as DataGridViewImageCell;
                if (imageCell != null)
                {
                    return (Image)imageCell.Value;
                }
                return null;
            }
            set
            {
                if (GeneralCurrentCell != null)
                {
                    if (_fillStyle == FillStyleSettings.FillSingle)
                    {
                        if (value != null)
                        {
                            if (!(GeneralCurrentCell is DataGridViewImageCell))
                            {
                                CustomCell cell = GeneralCurrentCell as CustomCell;
                                if (cell != null)
                                    cell.CellPicture = value;
                            }
                        }
                        else
                        {
                            if (GeneralCurrentCell is DataGridViewImageCell)
                            {
                                int row, col;
                                row = GeneralCurrentCell.RowIndex;
                                col = GeneralCurrentCell.ColumnIndex;
                                if (row > 0 && col > 0)
                                    this[col, row] = new CustomCell();
                            }
                        }
                    }
                    else
                    {
                        foreach (DataGridViewCell dataGridViewCell in SelectedCells)
                        {
                            DataGridViewImageCell imageCell = dataGridViewCell as DataGridViewImageCell;
                            if (value != null)
                            {
                                if (imageCell == null)
                                {
                                    int row, col;
                                    row = dataGridViewCell.RowIndex;
                                    col = dataGridViewCell.ColumnIndex;
                                    if (row > 0 && col > 0)
                                    {
                                        DataGridViewImageCell cell = new DataGridViewImageCell();
                                        cell.Value = value;
                                        this[col, row] = cell;
                                    }
                                }
                            }
                            else if (imageCell != null)
                            {
                                int row, col;
                                row = dataGridViewCell.RowIndex;
                                col = dataGridViewCell.ColumnIndex;
                                if (row > 0 && col > 0)
                                    this[col, row] = new CustomCell();
                            }
                        }
                    }
                }
            }
        }

        private Color? _foreColorFixed;
        /// <summary>
        ///  Determines the color used to draw text on each part of the FlexGrid.
        /// </summary>
        [Description(" Determines the color used to draw text on each part of the FlexGrid."),
        Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ForeColorFixed
        {
            get
            {
                if (isInitializing)
                {
                    return GetValueIfFound<Color>("ForeColorFixed", Color.Empty);
                }
                else
                {
                    if (_foreColorFixed == null)
                    {
                        _foreColorFixed = ColumnHeadersDefaultCellStyle.ForeColor;
                    }
                    return _foreColorFixed.Value;
                }
            }
            set
            {
                if (isInitializing)
                {
                    myValues["ForeColorFixed"] = value;
                }
                else
                {
                    ColumnHeadersDefaultCellStyle.ForeColor = value;
                    ColumnHeadersDefaultCellStyle.SelectionForeColor = value;
                    RowHeadersDefaultCellStyle.ForeColor = value;
                    RowHeadersDefaultCellStyle.SelectionForeColor = value;
                    TopLeftHeaderCell.Style.ForeColor = value;
                    TopLeftHeaderCell.Style.SelectionForeColor = value;

                    foreach (DataGridViewColumn column in Columns)
                    {
                        column.HeaderCell.Style.ForeColor = value;
                    }

                    foreach (DataGridViewRow row in base.Rows)
                    {
                        row.HeaderCell.Style.ForeColor = value;
                    }

                    TopLeftHeaderCell.Style.ForeColor = value;
                    _foreColorFixed = value;
                }
            }
        }


        /// <summary>
        /// Returns/sets the color as the background color for all fixed cells.
        /// </summary>
        [Description("Returns/sets the color as the background color for all fixed cells. Provided for compatibility with MSFlexGrid BackColorFixed property"),
        Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackColorFixed
        {
            get
            {
                if (isInitializing)
                {
                    return GetValueIfFound<Color>("BackColorFixed", Color.Empty);
                }
                else
                    return ColumnHeadersDefaultCellStyle.BackColor;

            }
            set
            {
                if (isInitializing)
                {
                    myValues["BackColorFixed"] = value;
                }
                else
                {
                    ColumnHeadersDefaultCellStyle.BackColor = value;
                    ColumnHeadersDefaultCellStyle.SelectionBackColor = value;
                    RowHeadersDefaultCellStyle.BackColor = value;
                    RowHeadersDefaultCellStyle.SelectionBackColor = value;
                    TopLeftHeaderCell.Style.BackColor = value;
                    TopLeftHeaderCell.Style.SelectionBackColor = value;
                    foreach (DataGridViewColumn column in Columns)
                    {
                        column.HeaderCell.Style.BackColor = value;
                    }
                    DataGridViewCellStyle cellStyleFixed = GetCellStyleFixed();
                    cellStyleFixed.BackColor = value;
                    foreach (DataGridViewRow row in Rows)
                    {
                        row.HeaderCell.Style.BackColor = value;
                        if (row.Frozen)
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                if (cell.HasStyle)
                                {
                                    cell.Style.BackColor = value;
                                }
                                else
                                {
                                    cell.Style = cellStyleFixed;
                                }
                            }
                    }
                    TopLeftHeaderCell.Style.BackColor = value;
                }
            }
        }

        private HighLightSettings _highLight = HighLightSettings.HighlightAlways;

        /// <summary>
        /// Returns/sets whether selected cells appear highlighted.
        /// </summary>
        [Description("Returns/sets whether selected cells appear highlighted."),
        Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public HighLightSettings HighLight
        {
            get
            {
                if (isInitializing)
                {
                    return GetValueIfFound<HighLightSettings>("HighLight", HighLightSettings.HighlightAlways);
                }
                else
                    return _highLight;
            }
            set
            {
                if (isInitializing)
                {
                    myValues["HighLight"] = value;
                }
                else
                {
                    _highLight = value;
                    Refresh();
                }
            }
        }

    }
}