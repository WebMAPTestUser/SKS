using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UpgradeHelpers.VB6.Gui
{
    /// <summary>
    /// A designer form to edit splits and columns for a MSDataGridHelper.
    /// </summary>
    public partial class MSDataGridHelperDesignerModalView : Form
    {
        private int LastSelectedSplit = -1;
        private int LastSelectedColumn = -1;
        private MSDataGridHelperLayoutInfo _gridInfo = null;

        /// <summary>
        /// Returns the grid layout information object.
        /// </summary>
        /// <returns>A MSDataGridHelperLayoutInfo containing the grid layout information for this object.</returns>
        public MSDataGridHelperLayoutInfo gridInfo
        {
            get
            {
                return new MSDataGridHelperLayoutInfo(_gridInfo.GridSplits, _gridInfo.GridColumns, System.DateTime.Now.ToString());
            }
        }

        /// <summary>
        /// Builds an instance of MSDataGridHelperDesignerModalView with 
        /// the specified grid layout information object.
        /// </summary>
        /// <param name="gridInfo">A MSDataGridHelperLayoutInfo containing the grid layout information 
        /// for the new instance.</param>
        public MSDataGridHelperDesignerModalView(MSDataGridHelperLayoutInfo gridInfo)
        {
            InitializeComponent();
            _gridInfo = gridInfo;
        }

        /// <summary>
        /// Handles the Load event by updating the splits and columns contained in the grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MSDataGridHelperDesignerModalView_Load(object sender, EventArgs e)
        {
            UpdatesTreeView();
        }

        /// <summary>
        /// Updates the information contained in the treeview to represent the list of splits and columns.
        /// </summary>
        private void UpdatesTreeView()
        {
            TreeNode trNode = null;
            this.trvLayout.Nodes.Clear();
            CleanInformation();

            if (_gridInfo != null)
            {
                for (int i = 0; i < _gridInfo.GridSplits.Count; i++)
                {
                    trNode = trvLayout.Nodes.Add(GetSplitColumnTextRepresentation(_gridInfo.GridSplits[i]));
                    AddColumnList(_gridInfo.GridSplits[i], trNode);
                }
            }

            TriesToUpdateLastSelectedNode();
        }

        /// <summary>
        /// Tries to select a node that was previously selected before the UpdatesTreeView.
        /// </summary>
        private void TriesToUpdateLastSelectedNode()
        {
            try
            {
                //There was a previous node selected
                if ((LastSelectedSplit > -1) || (LastSelectedColumn > -1))
                {
                    //A split was selected only
                    if (LastSelectedColumn < 0)
                    {
                        //Ensures that one existing node will be selected
                        if (LastSelectedSplit >= trvLayout.Nodes.Count)
                            LastSelectedSplit = (trvLayout.Nodes.Count - 1);

                        trvLayout.SelectedNode = trvLayout.Nodes[LastSelectedSplit];
                        trvLayout.SelectedNode.EnsureVisible();
                    }
                    else
                    {
                        //The selected split no longer exists
                        if (LastSelectedSplit >= trvLayout.Nodes.Count)
                        {
                            LastSelectedSplit = (trvLayout.Nodes.Count - 1);
                            LastSelectedColumn = -1;
                            TriesToUpdateLastSelectedNode();
                        }
                        else
                        {
                            if (LastSelectedColumn >= trvLayout.Nodes[LastSelectedSplit].Nodes.Count)
                                LastSelectedColumn = (trvLayout.Nodes[LastSelectedSplit].Nodes.Count - 1);

                            trvLayout.SelectedNode = trvLayout.Nodes[LastSelectedSplit].Nodes[LastSelectedColumn];
                            trvLayout.SelectedNode.EnsureVisible();
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Adds the columns within a split.
        /// </summary>
        /// <param name="split"></param>
        /// <param name="trNode"></param>
        private void AddColumnList(Split split, TreeNode trNode)
        {
            for (int i = 0; i < split.Columns.Count; i++)
            {
                trNode.Nodes.Add(GetSplitColumnTextRepresentation(split.Columns[i]));
            }
        }

        /// <summary>
        /// Returns a textual representation of the item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetSplitColumnTextRepresentation(object item)
        {
            Split split = item as Split;
            Column col = item as Column;

            if (split != null)
                return "Split " + (split.Index + 1);
            else if (col != null)
                return "Column " + (col.ColIndex + 1) + " [" + col.Caption + "]";

            return "Unknown";
        }

        /// <summary>
        /// When a node has been selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trvLayout_AfterSelect(object sender, TreeViewEventArgs e)
        {

            LastSelectedSplit = LastSelectedColumn = -1;
            if (trvLayout.SelectedNode != null)
            {
                UpdateInformation(trvLayout.SelectedNode);

                if (trvLayout.SelectedNode.Level == 0)
                    LastSelectedSplit = trvLayout.SelectedNode.Index;
                else
                {
                    LastSelectedSplit = trvLayout.SelectedNode.Parent.Index;
                    LastSelectedColumn = trvLayout.SelectedNode.Index;
                }

            }
            else
            {
                CleanInformation();
            }
        }

        /// <summary>
        /// Takes care of updating the information displayed for the current selected split or column, 
        /// also corrects the state of the edition buttons.
        /// </summary>
        /// <param name="treeNode"></param>
        private void UpdateInformation(TreeNode treeNode)
        {
            this.lblProperties.Text = treeNode.Text + " properties:";
            this.propGrid.SelectedObject = GetSelectedSplitColumn(treeNode);
            this.cmdDelete.Enabled = ((treeNode.PrevNode != null) || (treeNode.NextNode != null));
        }

        /// <summary>
        /// Cleans the information displayed as no split or column is selected, 
        /// also corrects the state of the edition buttons.
        /// </summary>
        private void CleanInformation()
        {
            this.lblProperties.Text = "Properties";
            this.propGrid.SelectedObject = null;
            this.cmdDelete.Enabled = false;
        }

        /// <summary>
        /// Obtains a reference to the current split or column selected in the treeview.
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns></returns>
        private object GetSelectedSplitColumn(TreeNode treeNode)
        {
            object res = null;

            if (treeNode != null)
            {
                if (treeNode.Level == 0)
                    res = _gridInfo.GridSplits[treeNode.Index];
                else
                    res = _gridInfo.GridSplits[treeNode.Parent.Index].Columns[treeNode.Index];
            }

            return res;
        }

        /// <summary>
        /// Takes care of adding a new split.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdAddSplit_Click(object sender, EventArgs e)
        {
            try
            {
                _gridInfo.GridSplits.Add((short)_gridInfo.GridSplits.Count);
                UpdatesTreeView();
            }
            catch (Exception e1)
            {
                MessageBox.Show("Error " + e1.Message);
            }
        }

        /// <summary>
        /// Takes care of adding a new column.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdAddColumn_Click(object sender, EventArgs e)
        {
            try
            {
                _gridInfo.GridColumns.Add((short)_gridInfo.GridColumns.Count);
                UpdatesTreeView();
            }
            catch (Exception e1)
            {
                MessageBox.Show("Error " + e1.Message);
            }
        }

        /// <summary>
        /// Takes care of deleting the current selected split or column.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (trvLayout.SelectedNode != null)
                {
                    if (trvLayout.SelectedNode.Level == 0)
                        _gridInfo.GridSplits.Remove(trvLayout.SelectedNode.Index);
                    else
                        _gridInfo.GridSplits[trvLayout.SelectedNode.Parent.Index].Columns.Remove(trvLayout.SelectedNode.Index);

                }
                UpdatesTreeView();
            }
            catch (Exception e1)
            {
                MessageBox.Show("Error " + e1.Message);
            }
        }

        /// <summary>
        /// Updates the text for a node in the treenode when a property is updated.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void propGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (string.Equals(e.ChangedItem.Label, "caption", StringComparison.CurrentCultureIgnoreCase))
            {
                if (trvLayout.SelectedNode != null)
                {
                    string text = GetSplitColumnTextRepresentation(GetSelectedSplitColumn(trvLayout.SelectedNode));
                    if (!string.IsNullOrEmpty(text))
                        trvLayout.SelectedNode.Text = text;
                }
            }
        }

        /// <summary>
        /// To manage the delete keyword.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trvLayout_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                cmdDelete_Click(sender, null);
        }

    }
}