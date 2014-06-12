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
    public partial class DataGridViewFlex : System.Windows.Forms.DataGridView, System.ComponentModel.ISupportInitialize
    {
        private const int DEFAULT_NEW_CUSTOM_COLUMN_WIDTH = 66;
        private const int DEFAULT_GRIDLINEWIDTH = 1;

        private int _gridLineWidth = DEFAULT_GRIDLINEWIDTH;


        /// <summary>
        /// Initializes a new instance of the UpgradeHelpers.Windows.Forms.ExtendedGridView class 
        /// </summary>
        public DataGridViewFlex() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the UpgradeHelpers.Windows.Forms.ExtendedGridView class with the corresponding container
        /// </summary>
        /// <param name="container">The container where the Grid is going to be hosted</param>
        public DataGridViewFlex(IContainer container)
        {
            InitializeComponent();
            _controlKeyDown = new KeyEventHandler(control_KeyDown);
            _controlKeyUp = new KeyEventHandler(control_KeyUp);
            _controlKeyPress = new KeyPressEventHandler(control_KeyPress);
            this.CellMouseEnter -= new DataGridViewCellEventHandler(ExtendedDataGridView_CellMouseEnter);
            this.CellMouseEnter += new DataGridViewCellEventHandler(ExtendedDataGridView_CellMouseEnter);
            this.RowHeaderMouseClick += ExtendedDataGridView_RowHeaderMouseClick;
            isInitializing = false;
            Reset();
            #region Designer related code

            IServiceContainer serviceContainer = container as IServiceContainer;
            if (serviceContainer != null)
            {
                ExtendedDataGridViewPropertyFilter newMyFilter = new ExtendedDataGridViewPropertyFilter();
                DesignerActionService designerActionService = serviceContainer.GetService(typeof(DesignerActionService)) as DesignerActionService;
                //DesignerActionUIService designerActionUIService = serviceContainer.GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
                newMyFilter.oldService = (ITypeDescriptorFilterService)serviceContainer.GetService(typeof(ITypeDescriptorFilterService));
                newMyFilter.designerActionService = designerActionService;
                //newMyFilter.designerActionUIService = designerActionUIService;
                if (newMyFilter.oldService != null)
                {
                    serviceContainer.RemoveService(typeof(ITypeDescriptorFilterService));
                }

                serviceContainer.AddService(typeof(ITypeDescriptorFilterService), newMyFilter);
            }

            // Acquire a reference to IComponentChangeService
            //This service is used during design to make sure that we do not allow the user
            //to edit Columns properties when the grid is in MSFlexGrid compatibility
            IComponentChangeService changeService = container as IComponentChangeService;
            if (changeService != null)
            {
                changeEventHandler = new ComponentChangingEventHandler(changeService_ComponentChanging);
                changeService.ComponentChanging -= changeEventHandler;
                changeService.ComponentChanging += changeEventHandler;
            }
            #endregion
        }

        private void InitializeComponent()
        {
            
        }

       

       

     


        class ExtendedDataGridViewPropertyFilter : ITypeDescriptorFilterService
        {

            public ITypeDescriptorFilterService oldService;
            public DesignerActionService designerActionService;
            //public DesignerActionUIService designerActionUIService;
            DesignerActionList columnEditing;
            bool columnEditingRemoved;

            #region ITypeDescriptorFilterService Members

            public bool FilterAttributes(IComponent component, System.Collections.IDictionary attributes)
            {
                if (oldService != null)
                    oldService.FilterAttributes(component, attributes);
                return true;
            }

            public bool FilterEvents(IComponent component, System.Collections.IDictionary events)
            {
                if (oldService != null)
                    oldService.FilterEvents(component, events);
                return true;
            }

            public bool FilterProperties(IComponent component, System.Collections.IDictionary properties)
            {
                DataGridViewFlex grid = component as DataGridViewFlex;
                if (grid != null)
                {
                    //Initialize ColumnEditing actions
                    CacheColumnEditingActionList(component);
                    if (!grid.isInitializing)
                    {
                       SetPropertiesForFlexGrid(component, properties);
                    }
                    return false;
                }
                else
                    if (oldService != null)
                        return oldService.FilterProperties(component, properties);
                    else
                        return true;
            }
            #endregion

            //private void SetPropertiesForTrueDBGrid(IComponent component, System.Collections.IDictionary properties)
            //{
            //}

            private void SetPropertiesForFlexGrid(IComponent component, System.Collections.IDictionary properties)
            {
                properties.Remove("Columns");
                if (designerActionService != null && columnEditing != null && !columnEditingRemoved)
                {
                    designerActionService.Remove(component, columnEditing);
                    columnEditingRemoved = true;
                }
            }
            private void SetPropertiesForDataGridView(IComponent component, System.Collections.IDictionary properties)
            {
                //foreach (PropertyInfo propertyInfo in typeof(IFlexGridBehaviour).GetProperties())
                //{
                //    properties.Remove(propertyInfo.Name);
                //}
               if (designerActionService != null && columnEditing != null && columnEditingRemoved)
                {
                    designerActionService.Add(component, columnEditing);
                    columnEditingRemoved = false;

                }
            }
            private void CacheColumnEditingActionList(IComponent component)
            {
                if (designerActionService != null && columnEditing == null)
                {
                    try
                    {
                        DesignerActionListCollection designerActionList = designerActionService.GetComponentActions(component, ComponentActionsType.Component);
                        foreach (System.ComponentModel.Design.DesignerActionList dList in designerActionList)
                        {
                            if (dList.GetType().Name.Equals("DataGridViewColumnEditingActionList"))
                            {
                                this.columnEditing = dList;
                                break;
                            }
                        }
                    }
                    catch { }
                }
            }

        }


        /// <summary>
        /// ExtendedDataGridView destructor.
        /// Removes associations between the ComponentChanging event and its handler
        /// when the control is in design mode.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (DesignMode)
            {
                IComponentChangeService changeService =
                    GetService(typeof(IComponentChangeService))
                    as IComponentChangeService;
                if (changeService != null)
                    changeService.ComponentChanging -= changeEventHandler;

            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Change handler used in the designer to intercept changes in certains properties
        /// </summary>
        private ComponentChangingEventHandler changeEventHandler;
      

        /// <summary>
        /// Gets Cell Style
        /// </summary>
        /// <returns></returns>
        public DataGridViewCellStyle GetCellStyleNonFixed()
        {
            DataGridViewCellStyle cellStyleNormal = new DataGridViewCellStyle();
            cellStyleNormal.BackColor = DefaultCellStyle.BackColor;
            cellStyleNormal.ForeColor = DefaultCellStyle.ForeColor;
            cellStyleNormal.SelectionBackColor = DefaultCellStyle.SelectionBackColor;
            cellStyleNormal.SelectionForeColor = DefaultCellStyle.SelectionForeColor;
            return cellStyleNormal;
        }

        /// <summary>
        /// Gets CellStyle
        /// </summary>
        /// <returns></returns>
        public DataGridViewCellStyle GetCellStyleFixed()
        {
            DataGridViewCellStyle cellStyleFixed = new DataGridViewCellStyle();
            cellStyleFixed.BackColor = BackColorFixed;
            cellStyleFixed.ForeColor = ForeColorFixed;
            cellStyleFixed.SelectionBackColor = BackColorFixed;
            cellStyleFixed.SelectionForeColor = ForeColor;
            return cellStyleFixed;
        }

        /// <summary>
        /// Called when the DataSource changes.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnDataSourceChanged(EventArgs e)
        {
            CopyDataFromDataSource();
        }


        private void CopyDataFromDataSource()
        {
            int numColumnsInDataSource = 0;
            if (DataSource is System.ComponentModel.ITypedList)
            {

                System.ComponentModel.PropertyDescriptorCollection columnInfo = ((System.ComponentModel.ITypedList)DataSource).GetItemProperties(null);
                numColumnsInDataSource = columnInfo.Count;
                ColumnsCount = FixedColumns + columnInfo.Count;
                if (RowHeadersVisible)
                {
                    int colIndex = 0;
                    foreach (System.ComponentModel.PropertyDescriptor columnData in columnInfo)
                    {
                        Columns[colIndex++].HeaderText = columnData.DisplayName;
                    } // foreach
                } // if

            } // if
            if (DataSource is IList)
            {
                IList data = DataSource as IList;
                int _FixedRows = FixedRows;
                RowsCount = _FixedRows + data.Count;
                //First check if there is data
                if (data.Count > 0 && numColumnsInDataSource > 0)
                {
                    int _FixedColumns = FixedColumns;
                    int rowindex = _FixedRows;
                    foreach (object rowObj in data)
                    {
                        DataRowView rowView = rowObj as DataRowView;
                        if (rowObj != null)
                        {
                            for (int i = 0; i < numColumnsInDataSource; i++)
                                this[_FixedColumns + i, rowindex].Value = rowView[i];
                        } // if
                        rowindex++;
                    } // foreach

                } // if

            } // if
        }










        /// <summary>
        /// This overload is maded to provide another indexer that 
        /// will eliminate the need for generating narrowing operator in expressions like
        /// grid(10 / ColumnsCount,10 mod ColumnsCount)
        /// </summary>
        /// <param name="rowindex"></param>
        /// <param name="columnindex"></param>
        /// <returns></returns>
        public DataGridViewCell this[Double rowindex, Double columnindex]
        {
            get
            {
                return this[(int)columnindex, (int)rowindex];
            }
            set
            {
                this[(int)columnindex, (int)rowindex] = value;
            }
        }

        /// <summary>
        /// Obtains a cell from the grid.
        /// </summary>
        /// <param name="columnindex">Index of the desired column.</param>
        /// <param name="rowindex">Index of the desired row.</param>
        /// <returns></returns>
        public new DataGridViewCell this[int rowindex, int columnindex]
        {
            get
            {
                return GetCell(rowindex, columnindex);
            }
            set
            {
                base[columnindex, rowindex] = value;
            }

        }

        


        #region ISupportInitialize Members and Behaviour Switching Management

        private bool isInitializing;



        /// <summary>
        /// Implements the ISupportInitialize.BeginInit method.
        /// It sets up a temporal ValueHolderBehavior during the component initialization to 
        /// hold values until the EndInit is called.
        /// </summary>
        public virtual void BeginInit()
        {
            isInitializing = true;
        }
        /// <summary>
        /// Implements the ISupportInitialize.EndInit method.
        /// It sets the grid behavior according the Compatibility mode and delegates EndInit logic to the behaviour.
        /// </summary>
        public virtual void EndInit()
        {
            SetValuesFromInitializeComponents();
        }

        const int LOWEST_GRIDLINEWIDTH_VALUE = 0;
        private const int DEFAULT_ROWHEIGHTMIN = 0;
        private const int DEFAULT_ROWSCOUNT = 2;
        private const int DEFAULT_COLUMNSCOUNT = 2;
        
        private const DataGridViewSelectionMode DEFAULT_SELECTIONMODE = DataGridViewSelectionMode.CellSelect;
       


        private void SetValuesFromInitializeComponents()
        {
            isInitializing = false;
                if (Convert.ToInt32(myValues["GridLineWidth"]) > LOWEST_GRIDLINEWIDTH_VALUE)
                {
                    GridLineWidth = Convert.ToInt32(myValues["GridLineWidth"]);
                } // if
                if (myValues.ContainsKey("SelectionMode") && ((DataGridViewSelectionMode)myValues["SelectionMode"]) != DataGridViewSelectionMode.CellSelect)
                {
                    SelectionMode = ((DataGridViewSelectionMode)myValues["SelectionMode"]);
                } // if
                int _RowsCount = InitFromTempValues("RowsCount");
                
                int _FixedRows = InitFromTempValues("FixedRows");
                int _ColumnsCount = InitFromTempValues("ColumnsCount");
                int _FixedColumns = InitFromTempValues("FixedColumns");

                if (_ColumnsCount ==  0  &&_FixedColumns == 0)
                {
                    _ColumnsCount = DEFAULT_COLUMNSCOUNT;
                    _FixedColumns = DEFAULT_FIXED_COLUMNS;
                }

                if (_RowsCount == 0 && _FixedRows == 0)
                {
                    _RowsCount = DEFAULT_ROWSCOUNT;
                    _FixedRows = DEFAULT_FIXED_ROWS;
                }

                _RowsCount = (_RowsCount == UNSETVALUE) ? DEFAULT_ROWSCOUNT : _RowsCount;
            

                _FixedRows = (_FixedRows == UNSETVALUE ) ? DEFAULT_FIXED_ROWS : _FixedRows;

                
                _ColumnsCount = (_ColumnsCount == UNSETVALUE ) ? DEFAULT_COLUMNSCOUNT : _ColumnsCount;

                
                _FixedColumns = (_FixedColumns == UNSETVALUE ) ? DataGridViewFlex.DEFAULT_FIXED_COLUMNS : _FixedColumns;

                if (_FixedRows == -1 || _FixedColumns == -1 || _RowsCount < 0 ||
                    _FixedRows > _RowsCount ||
                    _FixedColumns > _ColumnsCount)
                {
                    //If there is any invalid value, then reset to defaults
                    FixedRows    = DEFAULT_FIXED_ROWS;
                    FixedColumns = DEFAULT_FIXED_COLUMNS;
                    RowsCount    = DEFAULT_ROWSCOUNT;
                    ColumnsCount = DEFAULT_COLUMNSCOUNT;
                } // if
                else
                {
                    if (RowsCount < 2) RowsCount = 2;
                    if (ColumnsCount < 2) ColumnsCount = 2;
                    if (_FixedRows >= RowsCount)
                    {
                        RowsCount = _RowsCount;
                        FixedRows = _FixedRows;
                    }
                    else
                    {
                        FixedRows = _FixedRows;
                        RowsCount = _RowsCount;
                    } // else
                    if (_FixedColumns >= ColumnsCount)
                    {
                        ColumnsCount = _ColumnsCount;
                        FixedColumns = _FixedColumns;
                    }
                    else
                    {
                        FixedColumns = _FixedColumns;
                        ColumnsCount = _ColumnsCount;
                    } // else
                } // else
                if (myValues.ContainsKey("RowHeightMin") && Convert.ToInt32(myValues["RowHeightMin"]) > 0)
                {
                    RowHeightMin = Convert.ToInt32(myValues["RowHeightMin"]);
                } // if
                if (myValues.ContainsKey("AllowBigSelection"))
                    AllowBigSelection = Convert.ToBoolean(myValues["AllowBigSelection"]);

                if (myValues.ContainsKey("BackColorFixed") && ((Color)myValues["BackColorFixed"]) != Color.Empty)
                {
                    BackColorFixed = (Color)myValues["BackColorFixed"];
                } // if
                if (myValues.ContainsKey("FocusRect") && ((FocusRectSettings)myValues["FocusRect"]) != FocusRectSettings.FocusNone)
                {
                    FocusRect = ((FocusRectSettings)myValues["FocusRect"]);
                } // if

                if (myValues.ContainsKey("HighLight") && ((HighLightSettings)myValues["HighLight"]) != HighLightSettings.HighlightNever)
                {
                    HighLight = ((HighLightSettings)myValues["HighLight"]);
                } // if
                if (myValues.ContainsKey("DataSource"))
                    DataSource = myValues["DataSource"];
            } // if

        private int InitFromTempValues(string key)
        {
            if (myValues.ContainsKey(key))
                return Convert.ToInt32(myValues[key]);
            return UNSETVALUE;
        }
            
        

        #endregion




        void ExtendedDataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (AllowRowSelection)
            {
                ClearSelection();
                foreach (DataGridViewColumn col in Columns)
                {
                    if (col.Visible)
                    {
                        CurrentCell = this[e.RowIndex, col.Index];
                        break;
                    } 
                    else
                        continue;
                    
                } // foreach
                DataGridViewRow currentRow = Rows[e.RowIndex];
                foreach (DataGridViewCell cell in currentRow.Cells)
                {
                    cell.Selected = true;
                }
            }
        }

 










        /// <summary>
        /// Processes the Up and Down Keys to trigger KeyUp and KeyDown handlers
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x100;
//            const int WM_KEYUP = 0x101;
            KeyEventArgs keyevent;
            if (msg.Msg == WM_KEYDOWN)
            {
                keyevent = GetKeyEventArgs(keyData);
                if (keyevent != null)
                {
                    foreach (KeyEventHandler handler in keydownevents)
                    {
                        handler.Invoke(this, keyevent);
                    } // foreach
                    if (keyevent.Handled)
                        return true;
                    if (keyevent != null)
                    {
                        foreach (KeyEventHandler handler in keyupevents)
                        {
                            handler.Invoke(this, keyevent);
                        } // foreach
                        if (keyevent.Handled)
                            return true;
                    } // if
                } // if
            }
            return base.ProcessCmdKey(ref msg, keyData);

        }

        private static KeyEventArgs GetKeyEventArgs(Keys keyData)
        {
            KeyEventArgs keyevent = null;
            switch (keyData)
            {
                case Keys.Tab:
                    keyevent = new KeyEventArgs(Keys.Tab);
                    break;
                case Keys.Down:
                    keyevent = new KeyEventArgs(Keys.Down);
                    break;
                case Keys.Up:
                    keyevent = new KeyEventArgs(Keys.Up);
                    break;

            }
            return keyevent;
        }


        KeyEventHandler _controlKeyDown;
        KeyEventHandler _controlKeyUp;
        KeyPressEventHandler _controlKeyPress;


        List<KeyEventHandler> keydownevents = new List<KeyEventHandler>();


        /// <summary>
        /// Hides DataGridView KeyDown Implementation to provide a functionality closer to the
        /// KeyDown event in VB6
        /// </summary>
        public new event KeyEventHandler KeyDown
        {
            add
            {
                keydownevents.Add(value);
                base.KeyDown += value;
            }
            remove
            {
                try { keydownevents.Remove(value); }
                catch { } // catch
                base.KeyDown -= value;
            }
        }



        List<KeyEventHandler> keyupevents = new List<KeyEventHandler>();


        /// <summary>
        /// Hides DataGridView KeyUp Implementation to provide a functionality closer to the
        /// KeyUp event in VB6
        /// </summary>
        public new event KeyEventHandler KeyUp
        {
            add
            {
                keyupevents.Add(value);
                base.KeyUp += value;
            }
            remove
            {
                try { keyupevents.Remove(value); }
                catch { } // catch
                base.KeyUp -= value;
            }
        }
 
        /// <summary>
        /// Attaches Key Events to Control
        /// </summary>
        /// <param name="control">Control to be modified</param>
        public void AttachKeyEventsToControl(Control control)
        {
            control.KeyDown -= _controlKeyDown;
            control.KeyPress -= _controlKeyPress;
            control.KeyUp -= _controlKeyUp;


            control.KeyDown += _controlKeyDown;
            control.KeyPress += _controlKeyPress;
            control.KeyUp += _controlKeyUp;
        }

        void control_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (KeyEventHandler handler in keyupevents)
            {
                handler.Invoke(sender, e);
            } // foreach

        }

        void control_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnKeyPress(e);
        }

        void control_KeyDown(object sender, KeyEventArgs e)
        {
            foreach (KeyEventHandler handler in keydownevents)
            {
                handler.Invoke(sender, e);
            } // foreach
        }


        #region Not Implemented

        private bool redraw = true;

        /// <summary>
        /// Redraw
        /// </summary>
        public bool Redraw
        {
            get
            {
                return redraw;
            }
            set
            {
                if (value == redraw) return;
                redraw = value;
                if (value)
                {
                    ResumeLayout();
                }
                else
                {
                    SuspendLayout();
                }
            }
        }

        #endregion
    }
}