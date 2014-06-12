using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace UpgradeHelpers.VB6.Gui
{
    /// <summary>
    /// DataHelperDesignerModalView inherits Form.
    /// </summary>
    internal partial class DataHelperDesignerModalView : Form
    {
        /// <summary>
        /// Instance to store the information on how to bind the control to the datahelper control.
        /// </summary>
        private DataHelperBindingInfo _dataHelperBindingInfo = null;
        public DataHelperBindingInfo DataHelperBindingInfo
        {
            get
            {
                return _dataHelperBindingInfo;
            }
        }

        private ITypeDescriptorContext context = null;

        /// <summary>
        /// Constructor of the Modal View window to edit the value of the property. 
        /// It receives the current instance (if one is set) and 
        /// the context so it can obtain extra information.
        /// </summary>
        /// <param name="dataHelperBindingInfo"></param>
        /// <param name="context"></param>
        public DataHelperDesignerModalView(DataHelperBindingInfo dataHelperBindingInfo, ITypeDescriptorContext context)
        {
            InitializeComponent();

            this._dataHelperBindingInfo = dataHelperBindingInfo;
            this.context = context;

            LoadListOfDataHelpers();
            InitPropertyGrid();
        }

        /// <summary>
        /// Loads the list of DataHelpers found in the form.
        /// </summary>
        private void LoadListOfDataHelpers()
        {
            int index = 0;
            try
            {
                DHComboBox.Items.Clear();
                Control ctrl = (Control)context.Instance;

                foreach (Control childCtrl in ContainerHelper.Controls(ctrl.FindForm()))
                {
                    if (childCtrl is DataHelper)
                    {
                        index = DHComboBox.Items.Add(childCtrl);
                        if ((_dataHelperBindingInfo != null) && (_dataHelperBindingInfo.BindingControl != null)
                            && (_dataHelperBindingInfo.BindingControl.Equals(childCtrl)))
                        {
                            DHComboBox.SelectedIndex = index;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Loads the Grid with the list of properties that should be set based on the control.
        /// </summary>
        private void InitPropertyGrid()
        {
            try
            {
                if (_dataHelperBindingInfo != null)
                {
                    foreach (KeyValuePair<string, string> props in _dataHelperBindingInfo.BindingParameters)
                    {
                        dbGridProperties.Rows.Add(props.Key, props.Value);
                    }
                }
            }
            catch { };
        }

        /// <summary>
        /// Check if the Ok button can be enabled.
        /// </summary>
        private void DHComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateRequiredInfo();
        }

        /// <summary>
        /// Check if the Ok button can be enabled.
        /// </summary>
        private void dbGridProperties_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            ValidateRequiredInfo();
        }

        /// <summary>
        /// Check if the Ok button can be enabled.
        /// </summary>
        private void dbGridProperties_KeyPress(object sender, KeyPressEventArgs e)
        {
            ValidateRequiredInfo();
        }

        /// <summary>
        /// Validate if all information required is available.
        /// </summary>
        private void ValidateRequiredInfo()
        {
            bool res = false;
            int i = 0;
            try
            {
                res = DHComboBox.SelectedIndex != -1;
                if (res)
                {
                    for (i = 0; i < dbGridProperties.Rows.Count; i++)
                    {
                        if ((dbGridProperties[1, i].Value == null) || (string.IsNullOrEmpty(dbGridProperties[1, i].Value.ToString().Trim())))
                        {
                            res = false;
                            break;
                        }
                    }
                }

                cmdOk.Enabled = res;
            }
            catch { }
        }

        /// <summary>
        /// To update the information edited so far if the Ok button has been set.
        /// </summary>
        private void DataHelperDesignerModalView_FormClosing(object sender, FormClosingEventArgs e)
        {
            List<KeyValuePair<string, string>> props = new List<KeyValuePair<string, string>>();

            if (DialogResult == DialogResult.OK)
            {
                for (int i = 0; i < dbGridProperties.Rows.Count; i++)
                {
                    props.Add(new KeyValuePair<string, string>(dbGridProperties[0, i].Value.ToString(), dbGridProperties[1, i].Value.ToString()));
                }

                if (DHComboBox.SelectedItem != null)
                    _dataHelperBindingInfo = new DataHelperBindingInfo((DataHelper)DHComboBox.SelectedItem, props.ToArray());
                else
                    _dataHelperBindingInfo = new DataHelperBindingInfo(null, props.ToArray());
            }
        }
        /// <summary>
        /// cmdClean Click, Cleans the values in ComboBox and Grid.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">EventArgs.</param>
        private void cmdClean_Click(object sender, EventArgs e)
        {
            DHComboBox.SelectedIndex = -1;
            for (int i = 0; i < dbGridProperties.Rows.Count; i++)
            {
                dbGridProperties[1, i].Value = string.Empty;
            }
        }
    }
}