using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UpgradeHelpers.VB6.DB.Controls;
using System.ComponentModel.Design;
using UpgradeHelpers.VB6.DB.ADO;


namespace UpgradeHelpers.VB6.DB.Controls
{
    /// <summary>
    /// Base class for the supported ADO Data Controls, internal purposes.
    /// </summary>
    [ToolboxItem(false)]
    public partial class InternalADODataControlHelper : DataControlHelper<ADORecordSetHelper>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public InternalADODataControlHelper()
            : base()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public InternalADODataControlHelper(IContainer container)
            : base(container)
        {
        }
    }
}
