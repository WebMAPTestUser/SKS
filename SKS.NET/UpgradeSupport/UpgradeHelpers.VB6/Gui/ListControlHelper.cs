using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace UpgradeHelpers.VB6.Gui
{
    /// <summary>
    /// Extender that adds support to special functionality in ComboBoxes and ListBoxes, 
    /// mainly related to ItemData.
    /// </summary>
    [ProvideProperty("ItemData", typeof(System.Windows.Forms.ListControl))]
    public partial class ListControlHelper : Component, IExtenderProvider, ISupportInitialize
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ListControlHelper()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="container">The container where to add the controls.</param>
        public ListControlHelper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// Indicates if EndInit hasn't been executed yet after a BeginInit.
        /// </summary>
        private bool OnInitialization = false;

        /// <summary>
        /// Implements BeginInit Method from ISupportInitialize. 
        /// Sets ListControl status to OnInitialization.
        /// </summary>
        public void BeginInit()
        {
            OnInitialization = true;
        }

        /// <summary>
        /// Implements EndInit Method from ISupportInitialize. 
        /// Sets ListControl status to Not OnInitialization.
        /// </summary>
        public void EndInit()
        {
            OnInitialization = false;
            RefreshItemsData();
        }

        /// <summary>
        /// Updates the list of items data of the controls in runtime after the EndInit has been invoked.
        /// </summary>
        private void RefreshItemsData()
        {
            if (!DesignMode)
            {
                foreach (System.Windows.Forms.ListControl lstControl in ItemsData.Keys)
                {
                    if (lstControl is System.Windows.Forms.ComboBox)
                    {
                        for (int i = 0; (i < ((System.Windows.Forms.ComboBox)lstControl).Items.Count) && (i < ItemsData[lstControl].Length); i++)
                        {
                            //((System.Windows.Forms.ComboBox)lstControl).Items[i] = ItemsData[lstControl][i];
                        }
                    }
                    else
                    {
                        for (int i = 0; (i < ((System.Windows.Forms.ListBox)lstControl).Items.Count) && (i < ItemsData[lstControl].Length); i++)
                        {
                            //((System.Windows.Forms.ListBox)lstControl).Items[i] = ItemsData[lstControl][i];
                        }
                    }
                }
                ItemsData.Clear();
            }
        }

        /// <summary>
        /// Stores the ItemsData for each control temporarely during design time.
        /// </summary>
        private Dictionary<System.Windows.Forms.ListControl, int[]> ItemsData = new Dictionary<System.Windows.Forms.ListControl, int[]>();

        /// <summary>
        /// Determinates which controls can use these extra properties.
        /// </summary>
        /// <param name="extender">The object to test.</param>
        /// <returns>True if the object can extend the properties.</returns>
        public bool CanExtend(object extender)
        {
            return (extender is System.Windows.Forms.ListControl);
        }

        /// <summary>
        /// Gets the ItemData property of a specific list control.
        /// </summary>
        /// <param name="lstControl">The list control to test.</param>
        /// <returns>Returns an int array with the item data list of the control.</returns>
        public int[] GetItemData(System.Windows.Forms.ListControl lstControl)
        {
            int[] res = new int[0];

            if (lstControl is System.Windows.Forms.ComboBox)
                res = GetItemData((System.Windows.Forms.ComboBox)lstControl);
            else
                res = GetItemData((System.Windows.Forms.ListBox)lstControl);

            return res;
        }

        /// <summary>
        /// Gets the ItemData property of a specific list control. 
        /// This specific function applies just for a ComboBox control.
        /// </summary>
        /// <param name="lstControl">The list control to test.</param>
        /// <returns>Returns an int array with the item data list of the control.</returns>
        private int[] GetItemData(System.Windows.Forms.ComboBox lstControl)
        {
            int[] res = new int[lstControl.Items.Count];

            //In design time we will keep the list of itemsData in a separate list 
            //so we don't mess with the VS.NET Designer to display the Items property
            if (DesignMode)
            {
                if (!ItemsData.ContainsKey(lstControl))
                {
                    for (int i = 0; i < lstControl.Items.Count; i++)
                    {
                        //res[i] = Microsoft.VisualBasic.Compatibility.VB6.Support.GetItemData(lstControl, i);
                    }

                    ItemsData.Add(lstControl, res);
                }
                else
                {
                    if (lstControl.Items.Count != ItemsData[lstControl].Length)
                    {
                        for (int i = 0; (i < lstControl.Items.Count) && (i < ItemsData[lstControl].Length); i++)
                            res[i] = ItemsData[lstControl][i];

                        ItemsData[lstControl] = res;
                    }
                    else
                        res = ItemsData[lstControl];
                }
            }
            else
            {
                for (int i = 0; i < lstControl.Items.Count; i++)
                {
                    //res[i] = Microsoft.VisualBasic.Compatibility.VB6.Support.GetItemData(lstControl, i);
                }
            }

            return res;
        }

        /// <summary>
        /// Gets the ItemData property of a specific list control. 
        /// This specific function applies just for a ListBox control.
        /// </summary>
        /// <param name="lstControl">The list control to test.</param>
        /// <returns>Returns an int array with the item data list of the control.</returns>
        private int[] GetItemData(System.Windows.Forms.ListBox lstControl)
        {
            int[] res = new int[lstControl.Items.Count];

            //In design time we will keep the list of itemsData in a separate list so 
            //we don't mess with the VS.NET Designer to display the Items property
            if (DesignMode)
            {
                if (!ItemsData.ContainsKey(lstControl))
                {
                    for (int i = 0; i < lstControl.Items.Count; i++)
                    {
                        //res[i] = Microsoft.VisualBasic.Compatibility.VB6.Support.GetItemData(lstControl, i);
                    }

                    ItemsData.Add(lstControl, res);
                }
                else
                {
                    if (lstControl.Items.Count != ItemsData[lstControl].Length)
                    {
                        for (int i = 0; (i < lstControl.Items.Count) && (i < ItemsData[lstControl].Length); i++)
                            res[i] = ItemsData[lstControl][i];

                        ItemsData[lstControl] = res;
                    }
                    else
                        res = ItemsData[lstControl];
                }
            }
            else
            {
                for (int i = 0; i < lstControl.Items.Count; i++)
                {
                    //res[i] = Microsoft.VisualBasic.Compatibility.VB6.Support.GetItemData(lstControl, i);
                }
            }

            return res;
        }

        /// <summary>
        /// Sets the ItemData property of a specific list control.
        /// </summary>
        /// <param name="lstControl">The list control.</param>
        /// <param name="itemsData">The Item data list to set.</param>
        public void SetItemData(System.Windows.Forms.ListControl lstControl, int[] itemsData)
        {
            if (lstControl is System.Windows.Forms.ComboBox)
                SetItemData((System.Windows.Forms.ComboBox)lstControl, itemsData);
            else
                SetItemData((System.Windows.Forms.ListBox)lstControl, itemsData);
        }

        /// <summary>
        /// Sets the ItemData property of a specific list control.
        /// This specific function applies just for a ComboBox control.
        /// </summary>
        /// <param name="lstControl">The list control.</param>
        /// <param name="itemsData">The Item data list to set.</param>
        private void SetItemData(System.Windows.Forms.ComboBox lstControl, int[] itemsData)
        {
            int[] items = new int[lstControl.Items.Count];
            if (itemsData != null)
            {
                if (DesignMode || OnInitialization)
                {
                    if (!OnInitialization)
                    {
                        for (int i = 0; (i < lstControl.Items.Count) && (i < itemsData.Length); i++)
                            items[i] = itemsData[i];
                    }
                    else
                        items = itemsData;

                    if (!ItemsData.ContainsKey(lstControl))
                        ItemsData.Add(lstControl, items);
                    else
                        ItemsData[lstControl] = items;
                }
                else
                {
                    for (int i = 0; (i < lstControl.Items.Count) && (i < itemsData.Length); i++)
                    {
                        //Microsoft.VisualBasic.Compatibility.VB6.Support.SetItemData(lstControl, i, itemsData[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the ItemData property of a specific list control.
        /// This specific function applies just for a ListBox control.
        /// </summary>
        /// <param name="lstControl">The list control.</param>
        /// <param name="itemsData">The Item data list to set.</param>
        private void SetItemData(System.Windows.Forms.ListBox lstControl, int[] itemsData)
        {
            int[] items = new int[lstControl.Items.Count];
            if (itemsData != null)
            {
                if (DesignMode || OnInitialization)
                {
                    if (!OnInitialization)
                    {
                        for (int i = 0; (i < lstControl.Items.Count) && (i < itemsData.Length); i++)
                            items[i] = itemsData[i];
                    }
                    else
                        items = itemsData;

                    if (!ItemsData.ContainsKey(lstControl))
                        ItemsData.Add(lstControl, items);
                    else
                        ItemsData[lstControl] = items;
                }
                else
                {

                    //for (int i = 0; (i < lstControl.Items.Count) && (i < itemsData.Length); i++)
                    //    Microsoft.VisualBasic.Compatibility.VB6.Support.SetItemData(lstControl, i, itemsData[i]);
                    Dictionary<int,int> lstItemData = new Dictionary<int,int>();
                    for (int i = 0; (i < lstControl.Items.Count) && (i < itemsData.Length); i++)
                    {
                        lstItemData.Add(i, itemsData[i]);
                    }
                    lstControl.Tag = lstItemData;
                }
            }
        }
    }


    /// <summary>
    /// Static class that contains a List control extender methods
    /// </summary>
    public static class ListControl_Extenders
    {
        private class Item
        {
            public int itemdata;
            public string item;

            public Item()
            {
                itemdata = 0;
                item = "";
            }

            public override string ToString()
            {
                return item;
            }
        }

        /// <summary>
        /// Gets the list item.
        /// </summary>
        /// <param name="lstControl">The List control instance.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
#if TargetF2
        public static string GetListItem(System.Windows.Forms.ListControl lstControl, int index)
#else
        public static string GetListItem(this System.Windows.Forms.ListControl lstControl, int index)
#endif
        {
            if (lstControl is System.Windows.Forms.ComboBox)
                return GetListItem((System.Windows.Forms.ComboBox)lstControl, index);
            else
                return GetListItem((System.Windows.Forms.ListBox)lstControl, index);
        }

        private static string GetListItem(System.Windows.Forms.ComboBox lstControl, int index)
        {
            if (index >= 0 && lstControl.Items.Count > index)
            {
                string item = "";
                if (lstControl.Items[index] is Item)
                {
                    item = ((Item)lstControl.Items[index]).item;
                }
                else
                {
                    item = lstControl.Items[index] as string;
                }
                return item;
            }
            else
            {
                return "";
            }
        }
        private static string GetListItem(System.Windows.Forms.ListBox lstControl, int index)
        {
            if (index >= 0 && lstControl.Items.Count > index)
            {
                string item = "";
                if (lstControl.Items[index] is Item)
                {
                    item = ((Item)lstControl.Items[index]).item;
                }
                else
                {
                    item = lstControl.Items[index] as string;
                }
                return item;
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// Sets the list item.
        /// </summary>
        /// <param name="lstControl">The list control instance.</param>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
#if TargetF2
        public static void SetListItem(System.Windows.Forms.ListControl lstControl, int index, string value)
#else
        public static void SetListItem(this System.Windows.Forms.ListControl lstControl, int index, string value)
#endif
        {
            if (lstControl is System.Windows.Forms.ComboBox)
                SetListItem((System.Windows.Forms.ComboBox)lstControl, index, value);
            else
                SetListItem((System.Windows.Forms.ListBox)lstControl, index, value);
        }

        private static void SetListItem(System.Windows.Forms.ComboBox lstControl, int index, string value)
        {
            if (lstControl.Items.Count >= index)
            {
                Item item = new Item();
                item.item = value;
                if (lstControl.Items.Count == index)
                {
                    lstControl.Items.Add(item);
                }
                else
                {
                    lstControl.Items[index] = item;
                }
            }
            else
            {
                Microsoft.VisualBasic.Information.Err().Number = 381;
            }
        }
        private static void SetListItem(System.Windows.Forms.ListBox lstControl, int index, string value)
        {
            if (lstControl.Items.Count >= index)
            {
                Item item = new Item();
                item.item = value;
                if (lstControl.Items.Count == index)
                {
                    lstControl.Items.Add(item);
                }
                else
                {
                    if (lstControl.Items[index] is Item)
                    {
                        item.itemdata = ((Item)lstControl.Items[index]).itemdata;
                    }
                    lstControl.Items[index] = item;
                }
            }
            else
            {
                Microsoft.VisualBasic.Information.Err().Number = 381;
            }
        }

        /// <summary>
        /// Gets the item data.
        /// </summary>
        /// <param name="lstControl">The list control instance.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
#if TargetF2
        public static int GetItemData(System.Windows.Forms.ListControl lstControl, int index)
#else
        public static int GetItemData(this System.Windows.Forms.ListControl lstControl, int index)
#endif
        {
            if (lstControl is System.Windows.Forms.ComboBox)
                return GetItemData((System.Windows.Forms.ComboBox)lstControl, index);
            else
                return GetItemData((System.Windows.Forms.ListBox)lstControl, index);
        }

#if TargetF2
        private static int GetItemData(System.Windows.Forms.ListBox lstControl, int index)
#else
        private static int GetItemData(this System.Windows.Forms.ListBox lstControl, int index)
#endif
        {
            if (lstControl.Items.Count > 0 && lstControl.Items.Count >= index && lstControl.Items[index] is Item)
            {
                return ((Item)lstControl.Items[index]).itemdata;
            }
            return 0;
        }

#if TargetF2
        private static int GetItemData(System.Windows.Forms.ComboBox lstControl, int index)
#else
        private static int GetItemData(this System.Windows.Forms.ComboBox lstControl, int index)
#endif
        {
            if (lstControl.Items.Count > 0 && lstControl.Items.Count >= index && lstControl.Items[index] is Item)
            {
                return ((Item)lstControl.Items[index]).itemdata;
            }
            return 0;
        }
        /// <summary>
        /// Sets the item data.
        /// </summary>
        /// <param name="lstControl">The list control instance.</param>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
#if TargetF2
        public static void SetItemData(System.Windows.Forms.ListControl lstControl, int index, int value)
#else
        public static void SetItemData(this System.Windows.Forms.ListControl lstControl, int index, int value)
#endif
        {
            if (lstControl is System.Windows.Forms.ListBox)
            {
                SetItemData((System.Windows.Forms.ListBox)lstControl, index, value);
            }
            else if (lstControl is System.Windows.Forms.ComboBox)
            {
                SetItemData((System.Windows.Forms.ComboBox)lstControl, index, value);
            }
        }

#if TargetF2
        private static void SetItemData(System.Windows.Forms.ListBox lstControl, int index, int value)
#else
        private static void SetItemData(this System.Windows.Forms.ListBox lstControl, int index, int value)
#endif
        {
            if (lstControl.Items.Count > 0 && lstControl.Items.Count >= index)
            {
                Item item;
                if (lstControl.Items[index] is Item)
                {
                    item = (Item)lstControl.Items[index];
                    item.itemdata = value;
                }
                else
                {
                    item = new Item();
                    item.item = lstControl.Items[index].ToString();
                    item.itemdata = value;
                }
                lstControl.Items[index] = item;
            }
            else
            {
                Microsoft.VisualBasic.Information.Err().Number = 381;
            }
        }
#if TargetF2
        private static void SetItemData(System.Windows.Forms.ComboBox lstControl, int index, int value)
#else
        private static void SetItemData(this System.Windows.Forms.ComboBox lstControl, int index, int value)
#endif
        {
            if (lstControl.Items.Count > 0 && lstControl.Items.Count >= index)
            {
                Item item;
                if (lstControl.Items[index] is Item)
                {
                    item = (Item)lstControl.Items[index];
                    item.itemdata = value;
                }
                else
                {
                    item = new Item();
                    item.item = lstControl.Items[index].ToString();
                    item.itemdata = value;
                }
                lstControl.Items[index] = item;
            }
            else
            {
                Microsoft.VisualBasic.Information.Err().Number = 381;
            }
        }
		
#if TargetF2

/// <summary>
/// Adds the item.
/// </summary>
/// <param name="lstControl">The list control instance.</param>		
/// <param name="value">The value.</param>			
        public static void Clear(System.Windows.Forms.ListControl lstControl, string value)
#else
		/// <summary>
		/// Adds the item.
		/// </summary>
		/// <param name="lstControl">The list control instance.</param>		
		public static void Clear(this System.Windows.Forms.ListControl lstControl)
#endif
		{
			if (lstControl is System.Windows.Forms.ListBox)
			{
				((System.Windows.Forms.ListBox)lstControl).Items.Clear();
			}
			else if (lstControl is System.Windows.Forms.ComboBox)
			{
				((System.Windows.Forms.ComboBox)lstControl).Items.Clear();
			}
			_dictionaryOfNewIndexes[lstControl.GetHashCode()] = -1;
		}

		/// <summary>
		/// Adds the item
		/// </summary>
		/// <param name="lstControl">The list control instance.</param>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
#if TargetF2
        public static void AddItem(System.Windows.Forms.ListControl lstControl, string value, int index)
#else
        public static void AddItem(this System.Windows.Forms.ListControl lstControl, string value, int index)
#endif
        {
            if (lstControl is System.Windows.Forms.ListBox)
            {
                AddItem((System.Windows.Forms.ListBox)lstControl, value, index);
            }
            else if (lstControl is System.Windows.Forms.ComboBox)
            {
                AddItem((System.Windows.Forms.ComboBox)lstControl, value, index);
            }
        }

#if TargetF2
        private static void AddItem(System.Windows.Forms.ListBox lstControl, string value, int index)
#else
        private static void AddItem(this System.Windows.Forms.ListBox lstControl, string value, int index)
#endif
        {
            try
            {
                lstControl.Items.Insert(index, value);
				_dictionaryOfNewIndexes[lstControl.GetHashCode()] = index;
            }
            catch
            {
            }
        }

#if TargetF2
        private static void AddItem(System.Windows.Forms.ComboBox lstControl, string value, int index)
#else
        private static void AddItem(this System.Windows.Forms.ComboBox lstControl, string value, int index)
#endif
        {
            try
            {
                lstControl.Items.Insert(index, value);
				_dictionaryOfNewIndexes[lstControl.GetHashCode()] = index;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="lstControl">The list control instance.</param>
        /// <param name="value">The value.</param>
#if TargetF2
        public static void AddItem(System.Windows.Forms.ListControl lstControl, string value)
#else
        public static void AddItem(this System.Windows.Forms.ListControl lstControl, string value)
#endif
        {
            if (lstControl is System.Windows.Forms.ListBox)
            {
                AddItem((System.Windows.Forms.ListBox)lstControl, value);
            }
            else if (lstControl is System.Windows.Forms.ComboBox)
            {
                AddItem((System.Windows.Forms.ComboBox)lstControl, value);
            }
        }

#if TargetF2
        private static void AddItem(System.Windows.Forms.ListBox lstControl, string value)
#else
        private static void AddItem(this System.Windows.Forms.ListBox lstControl, string value)
#endif
        {
            int newIndex = lstControl.Items.Add(value);
			_dictionaryOfNewIndexes[lstControl.GetHashCode()] = newIndex;
        }

#if TargetF2
        private static void AddItem(System.Windows.Forms.ComboBox lstControl, string value)
#else
        private static void AddItem(this System.Windows.Forms.ComboBox lstControl, string value)
#endif
        {
            int newIndex = lstControl.Items.Add(value);
			_dictionaryOfNewIndexes[lstControl.GetHashCode()] = newIndex;
        }

        static Dictionary<int, int> _dictionaryOfNewIndexes = new Dictionary<int, int>();

        /// <summary>
        /// Gets the new index.
        /// </summary>
        /// <param name="lstControl">The list control instance.</param>
        /// <returns></returns>
#if TargetF2
        public static int GetNewIndex(System.Windows.Forms.ListControl lstControl)
#else
        public static int GetNewIndex(this System.Windows.Forms.ListControl lstControl)
#endif
        {
			if (_dictionaryOfNewIndexes.ContainsKey(lstControl.GetHashCode()))
            {
				return _dictionaryOfNewIndexes[lstControl.GetHashCode()];
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="lstControl">The list control instance.</param>
        /// <param name="index">The index.</param>
#if TargetF2
        public static void RemoveItem(System.Windows.Forms.ListControl lstControl, int index)
#else
        public static void RemoveItem(this System.Windows.Forms.ListControl lstControl, int index)
#endif
        {
            if (lstControl is System.Windows.Forms.ListBox)
            {
                RemoveItem((System.Windows.Forms.ListBox)lstControl, index);
            }
            else if (lstControl is System.Windows.Forms.ComboBox)
            {
                RemoveItem((System.Windows.Forms.ComboBox)lstControl, index);
            }
        }

#if TargetF2
        private static void RemoveItem(System.Windows.Forms.ListBox lstControl, int index)
#else
        private static void RemoveItem(this System.Windows.Forms.ListBox lstControl, int index)
#endif
        {
            try
            {
                lstControl.Items.RemoveAt(index);
				_dictionaryOfNewIndexes[lstControl.GetHashCode()] = -1;
            }
            catch
            {
            }
        }

#if TargetF2
        private static void RemoveItem(System.Windows.Forms.ComboBox lstControl, int index)
#else
        private static void RemoveItem(this System.Windows.Forms.ComboBox lstControl, int index)
#endif
        {
            try
            {
				if (lstControl.Text == lstControl.Items[index].ToString())
				{
					lstControl.Text = string.Empty;
				}
                lstControl.Items.RemoveAt(index);
				_dictionaryOfNewIndexes[lstControl.GetHashCode()] = -1;
            }
            catch
            {
            }
        }
    }
}
