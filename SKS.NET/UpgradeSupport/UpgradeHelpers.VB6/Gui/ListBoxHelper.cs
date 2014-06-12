using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using UpgradeHelpers.VB6.Utils;

namespace UpgradeHelpers.VB6.Gui
{
    /// <summary>
    /// Extender that adds support to special functionality in ListBoxes, 
    /// for example the properties SelectionMode and Selected.
    /// </summary>
    [ProvideProperty("SelectionMode", typeof(System.Windows.Forms.ListBox))]
    public partial class ListBoxHelper : Component, IExtenderProvider
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ListBoxHelper()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="container">The container where to add the controls.</param>
        public ListBoxHelper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// Contains the current selected indexes in the ListBox.
        /// </summary>
        private static WeakDictionary<System.Windows.Forms.ListBox, int> selectedIndexList = new WeakDictionary<ListBox, int>();

        /// <summary>
        /// Determinate which controls can use these extra properties.
        /// </summary>
        /// <param name="extender">The object to test.</param>
        /// <returns>True if the object can extend the properties.</returns>
        public bool CanExtend(object extender)
        {
            return (extender is System.Windows.Forms.ListBox);
        }

        /// <summary>
        /// Returns the current value of SelectionMode provided by this control. 
        /// It happens to be the same value of the ListBox control.
        /// </summary>
        /// <param name="lstBox">The control to get the SelectionMode.</param>
        /// <returns>The current SelectionMode assigned to the control.</returns>
        public System.Windows.Forms.SelectionMode GetSelectionMode(System.Windows.Forms.ListBox lstBox)
        {
            return lstBox.SelectionMode;
        }

        /// <summary>
        /// Sets the SelectionMode for a control.
        /// </summary>
        /// <param name="lstBox">The control to set the SelectionMode.</param>
        /// <param name="mode">The selection mode to set.</param>
        public void SetSelectionMode(System.Windows.Forms.ListBox lstBox, System.Windows.Forms.SelectionMode mode)
        {
            lstBox.SelectionMode = mode;
            if ((mode == System.Windows.Forms.SelectionMode.MultiExtended) ||
                (mode == System.Windows.Forms.SelectionMode.MultiSimple))
            {
                if (!selectedIndexList.ContainsKey(lstBox))
                {
                    selectedIndexList.Add(lstBox, 0);
                    lstBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
                    lstBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(ListBox_DrawItem);
                }
            }
            else
            {
                if (selectedIndexList.ContainsKey(lstBox))
                {
                    selectedIndexList.Remove(lstBox);
                    lstBox.DrawMode = DrawMode.Normal;
                    lstBox.DrawItem -= new System.Windows.Forms.DrawItemEventHandler(ListBox_DrawItem);
                }
            }
        }

        /// <summary>
        /// For MultiExtended and MultiSimple selection modes we will draw the items ourselves 
        /// to keep track of which item has the focus.
        /// </summary>
        /// <param name="sender">The ListBox raising the event.</param>
        /// <param name="e">The DrawItemEventArgs for the current item to draw.</param>
        private void ListBox_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            System.Windows.Forms.ListBox lstBox = (System.Windows.Forms.ListBox)sender;
            e.DrawBackground();
            using (Brush myBrush = new SolidBrush(e.ForeColor))
            {
                if ((e.Index < 0) || (e.Index >= lstBox.Items.Count))
                    return;

                if (selectedIndexList.ContainsKey(lstBox))
                {
                    if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
                        selectedIndexList[lstBox] = e.Index;
                }

                // Draw the current item text based on the current Font and the custom brush settings.
                e.Graphics.DrawString(lstBox.Items[e.Index].ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
                // If the ListBox has focus, draw a focus rectangle around the selected item.
                e.DrawFocusRectangle();
            }
        }

        /// <summary>
        /// Function to get the selected index of a ListBox. It depends on the selection mode property.
        /// </summary>
        /// <param name="lstBox">The listbox to return the SelectedIndex.</param>
        /// <returns>The current selected index for a list box or in the case of 
        /// SelectionMode = [MultiExtended|MultiSimple] it might throw an exception 
        /// if a ListBoxHelper component hasn't been added to the form with 
        /// the ListBox (The ListBoxHelper component will provide an extra 
        /// property to set SelectionMode).
        /// </returns>
        public static int GetSelectedIndex(System.Windows.Forms.ListBox lstBox)
        {
            if ((lstBox.SelectionMode == SelectionMode.MultiExtended) || (lstBox.SelectionMode == SelectionMode.MultiSimple))
            {
                if (selectedIndexList.ContainsKey(lstBox))
                    return selectedIndexList[lstBox];
                else
                    throw new Exception("SelectedIndex property not stored for a MultiSelect ListBox, "
                        + "please add a ListBoxHelper to the form and set the property SelectionMode again");
            }
            else
                return lstBox.SelectedIndex;
        }

        /// <summary>
        /// Function to set the selected index of a ListBox. Its behavior depends on 
        /// the selection mode property.
        /// </summary>
        /// <param name="lstBox">The listbox to set the SelectedIndex.</param>
        /// <param name="SelectedIndex">The value to be set.</param>
        /// <returns>Returns the selectedIndex after the operation.</returns>
        public static int SetSelectedIndex(System.Windows.Forms.ListBox lstBox, int SelectedIndex)
        {
            int currSelectedIndex = 0;
            bool mustBeClean = false;

            if ((lstBox.SelectionMode == SelectionMode.MultiSimple) || (lstBox.SelectionMode == SelectionMode.MultiExtended))
            {
                if (selectedIndexList.ContainsKey(lstBox))
                    selectedIndexList[lstBox] = SelectedIndex;
                else
                    throw new Exception("SelectedIndex property not stored for a MultiSelect ListBox, "
                        + "please add a ListBoxHelper to the form and set the property SelectionMode again");

                currSelectedIndex = lstBox.SelectedIndex;
                if ((SelectedIndex > -1) && (SelectedIndex < lstBox.Items.Count))
                {
                    mustBeClean = !lstBox.SelectedIndices.Contains(SelectedIndex);

                    lstBox.SetSelected(SelectedIndex, true);
                    if (mustBeClean)
                    {
                        ControlHelper.DisableControlEvents(lstBox, "SelectedIndexChanged");
                        lstBox.SetSelected(SelectedIndex, false);
                        ControlHelper.EnableControlEvents(lstBox, "SelectedIndexChanged");
                    }
                }
                else
                {
                    if (lstBox.Items.Count > 0)
                        lstBox.SelectedIndex = 0;
                    lstBox.SelectedIndex = -1;
                }


                lstBox.SelectedIndex = currSelectedIndex;
            }
            else
            {
                if ((SelectedIndex < -1) || (SelectedIndex >= lstBox.Items.Count))
                    throw new Exception("Invalid property value");

                lstBox.SelectedIndex = SelectedIndex;
            }

            return GetSelectedIndex(lstBox);
        }

        /// <summary>
        ///  Returns a value indicating whether the specified item is selected.
        /// </summary>
        /// <param name="lstBox">The listbox to test.</param>
        /// <param name="index">The index of the item to query if it is selected.</param>
        /// <returns>True if the item is selected.</returns>
        public static bool GetSelected(System.Windows.Forms.ListBox lstBox, int index)
        {
            return lstBox.GetSelected(index);
        }

        /// <summary>
        /// Selects or clears the selection for the specified item in a System.Windows.Forms.ListBox.
        /// </summary>
        /// <param name="lstBox">The listbox parent.</param>
        /// <param name="index">The index of the item.</param>
        /// <param name="value">The value to set to selected property.</param>
        public static void SetSelected(System.Windows.Forms.ListBox lstBox, int index, bool value)
        {
            if ((index < -1) || (index >= lstBox.Items.Count))
                throw new Exception("Invalid property value");

            if (lstBox.GetSelected(index) != value)
            {
                if ((value) && ((lstBox.SelectionMode == SelectionMode.MultiSimple) || (lstBox.SelectionMode == SelectionMode.MultiExtended)))
                {
                    if (selectedIndexList.ContainsKey(lstBox))
                        selectedIndexList[lstBox] = index;
                    else
                        throw new Exception("SelectedIndex property not stored for a MultiSelect ListBox, "
                            + "please add a ListBoxHelper to the form and set the property SelectionMode again");
                }

                lstBox.SetSelected(index, value);
            }
        }
    }
}
