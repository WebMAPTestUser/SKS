using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UpgradeHelpers.VB6.DB.Controls;
using System.ComponentModel.Design;
using UpgradeHelpers.VB6.DB.DAO;


namespace UpgradeHelpers.VB6.DB.Controls
{
    /// <summary>
    /// Base class for the supported DAO Data Controls, internal purposes.
	/// This class just exists due to the VS Designer issue with Generic classes it is just a bridge between the Generaric class and
	/// the ToolBox enabled control DAODataControlHelper
    /// </summary>
    [ToolboxItem(false)]
    public partial class InternalDAODataControlHelper : DataControlHelper<DAORecordSetHelper>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public InternalDAODataControlHelper()
            : base()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public InternalDAODataControlHelper(IContainer container)
            : base(container)
        {
        }
    }
}
