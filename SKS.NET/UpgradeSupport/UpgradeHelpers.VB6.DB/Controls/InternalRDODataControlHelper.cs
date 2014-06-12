using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using UpgradeHelpers.VB6.DB.RDO;


namespace UpgradeHelpers.VB6.DB.Controls
{
    /// <summary>
    /// Base class for the supported RDO Data Controls, internal purposes.
    /// </summary>
    [ToolboxItem(false)]
    public partial class InternalRDODataControlHelper : DataControlHelper<RDORecordSetHelper>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public InternalRDODataControlHelper()
        {
            InitializeComponent();
        }
    }
}
