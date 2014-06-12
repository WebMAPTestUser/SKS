using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace UpgradeHelpers.VB6.Gui
{
    /// <summary>
    /// Implements functionality in the MSDataGrid which was available in VB6 and is not in .NET.
    /// </summary>
    [ProvideProperty("GridLayout", typeof(object))]
    public partial class MSDataGridHelper : Component, IExtenderProvider
    {
        private Dictionary<object, MSDataGridHelperLayoutInfo> gridLayout = new Dictionary<object, MSDataGridHelperLayoutInfo>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MSDataGridHelper()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with container.
        /// </summary>
        /// <param name="container">The container where the button is included.</param>
        public MSDataGridHelper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// Determinates which controls can use these extra properties.
        /// </summary>
        /// <param name="extender">The object to test.</param>
        /// <returns>True if the object can extend the properties.</returns>
        public bool CanExtend(object extender)
        {
            return CheckIfIsAMSDataGrid(extender);
        }

        /// <summary>
        /// Checks if the instance represents a MSDataGrid.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool CheckIfIsAMSDataGrid(object obj)
        {
            System.Windows.Forms.AxHost.ClsidAttribute clsidAttr = null;
            foreach (object attr in obj.GetType().GetCustomAttributes(false))
            {
                clsidAttr = attr as System.Windows.Forms.AxHost.ClsidAttribute;
                if (clsidAttr != null)
                    return clsidAttr.Value.Equals("{cde57a43-8b86-11d0-b3c6-00a0c90aea82}", StringComparison.CurrentCultureIgnoreCase);
            }
            return false;
        }

        /// <summary>
        /// Returns the property Splits of the object if it is a MSDataGrid.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static Splits GetSplits(object obj)
        {
            if (CheckIfIsAMSDataGrid(obj))
                return UpgradeHelpers.VB6.Utils.ReflectionHelper.GetMember(obj, "Splits") as Splits;

            return null;
        }

        /// <summary>
        /// Returns the property Columns of the object if it is a MSDataGrid.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static Columns GetColumns(object obj)
        {
            if (CheckIfIsAMSDataGrid(obj))
                return UpgradeHelpers.VB6.Utils.ReflectionHelper.GetMember(obj, "Columns") as Columns;

            return null;
        }

        /// <summary>
        /// Allows to have access to the grid layout in design time.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        [Editor(typeof(MSDataGridHelperDesignerEditor), typeof(System.Drawing.Design.UITypeEditor)), Description("Allows to modify the layout of the grid"), Category("Layout")]
        public MSDataGridHelperLayoutInfo GetGridLayout(object grid)
        {
            if (!gridLayout.ContainsKey(grid))
            {
                Splits splits = GetSplits(grid);
                Columns cols = GetColumns(grid);

                if ((splits == null) || (cols == null))
                    throw new Exception("AIS-Exception. Couldn't retrieve the properties Splits or Columns of the object");

                gridLayout.Add(grid, new MSDataGridHelperLayoutInfo(splits, cols, System.DateTime.Now.ToString()));
            }

            return gridLayout[grid];
        }

        /// <summary>
        /// Sets a grid layout to a given grid.
        /// </summary>
        /// <param name="grid">The grid to set the grid layout to.</param>
        /// <param name="value">The new grid layout.</param>
        public void SetGridLayout(object grid, MSDataGridHelperLayoutInfo value)
        {
            if (!gridLayout.ContainsKey(grid))
                gridLayout.Add(grid, value);
            else
                gridLayout[grid] = value;
        }
    }
}
