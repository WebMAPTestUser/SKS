using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using UpgradeHelpers.VB6.Utils;

namespace UpgradeHelpers.VB6.Gui
{
    /// <summary>
    /// Extender that adds support to special functionality in ListView controls.
    /// </summary>
    [ProvideProperty("Sorted", typeof(ListView))]
    [ProvideProperty("SortKey", typeof(ListView))]
    [ProvideProperty("SortOrder", typeof(ListView))]
    [ProvideProperty("CorrectEventsBehavior", typeof(ListView))]
    [ProvideProperty("ItemClickMethod", typeof(ListView))]
    [ProvideProperty("LargeIcons", typeof(ListView))]
    [ProvideProperty("SmallIcons", typeof(ListView))]
    [ProvideProperty("ColumnHeaderIcons", typeof(ListView))]
    public partial class ListViewHelper : Component, IExtenderProvider, ISupportInitialize
    {

        /// <summary>
        /// Indicates if EndInit hasn't been executed yet after a BeginInit.
        /// </summary>
        private bool OnInitialization = false;
        /// <summary>
        /// Delegate for ItemClick event.
        /// </summary>
        private delegate void ListView_ItemClickDelegate(ListViewItem Item);
        /// <summary>
        /// Events to be locked during several processes.
        /// </summary>
        private static object objLockEvents = new object();
        /// <summary>
        /// List of events to be corrected for this provider.
        /// </summary>
        private static Dictionary<string, Delegate> EventsToCorrect = new Dictionary<string, Delegate>();
        /// <summary>
        /// List of events to be patched for this provider.
        /// </summary>
        private static WeakDictionary<ListView, Dictionary<String, List<Delegate>>> EventsPatched = new WeakDictionary<ListView, Dictionary<string, List<Delegate>>>();
        /// <summary>
        /// List of properties and values that are supplied by this Helper.
        /// </summary>
        private static WeakDictionary<ListView, Dictionary<newPropertiesEnum, object>> newProperties = new WeakDictionary<ListView, Dictionary<newPropertiesEnum, object>>();
        /// <summary>
        /// Keeps a list of Icons set for different properties.
        /// </summary>
        private static List<ListView> PendingListIconsToProcess = new List<ListView>();

        private static readonly string ItemClickEventName = "ItemClick";
        private static readonly string DrawItemEventName = "DrawItem";
        private static readonly string DrawSubItemEventName = "DrawSubItem";
        private static readonly string DrawColumnHeaderEventName = "DrawColumnHeader";
        private static readonly string KeyUpEventName = "KeyUp";
        private static readonly string ClickEventName = "Click";
        private static readonly string MouseUpEventName = "MouseUp";
        private static readonly string DoubleClickEventName = "DoubleClick";

        /// <summary>
        /// Constructor.
        /// </summary>
        static ListViewHelper()
        {
            //Initializes the list of events that should be patched
            EventsToCorrect.Add(ClickEventName, new EventHandler(ListView_Click));
            EventsToCorrect.Add(MouseUpEventName, new MouseEventHandler(ListView_MouseUp));
            EventsToCorrect.Add(DoubleClickEventName, new EventHandler(ListView_DoubleClick));
            EventsToCorrect.Add(KeyUpEventName, new KeyEventHandler(ListView_KeyUp));

            Application.AddMessageFilter(new IMessageFilterImplementer());
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ListViewHelper()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="container">The container where to add the controls.</param>
        public ListViewHelper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// Enum to handle the different properties and custom behaviors supplied by this Helper.
        /// </summary>
        private enum newPropertiesEnum
        {
            CustomSortingClass = 0,
            SortedProperty = 1,
            SortKeyProperty = 2,
            SortOrderProperty = 3,
            CorrectEventsBehavior = 4,
            ItemClickMethod = 5,
            LargeIcons = 6,
            SmallIcons = 7,
            ColumnHeaderIcons = 8,
            InternalColumnHeaderImageListHelper = 9,
            ListItemIcon = 10,
            ListItemSmallIcon = 11
        }


        /// <summary>
        /// Determinates which controls can use these extra properties.
        /// </summary>
        /// <param name="extender">The object to test.</param>
        /// <returns>True if the object can extend the properties.</returns>
        public bool CanExtend(object extender)
        {
            return (extender is ListView);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////// INSTANCE PROPERTIES DEFINITION //////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the Sorted property for a ListView.
        /// </summary>
        /// <param name="lView">The ListView to be test.</param>
        /// <returns>True elements are ordered, otherwise false.</returns>
        [Description("Indicates whether the elements of a control are automatically sorted alphabetically"), Category("Custom Properties")]
        public bool GetSorted(ListView lView)
        {
            return GetSortedProperty(lView);
        }
        /// <summary>
        /// Sets the Sorted property for a ListView.
        /// </summary>
        /// <param name="lView">The ListView to be set.</param>
        /// <param name="value">Indicates if values in ListView must be ordered or not.</param>
        public void SetSorted(ListView lView, bool value)
        {
            SetSortedProperty(lView, value);
        }

        /// <summary>
        /// Gets SortKey property for a ListView.
        /// </summary>
        /// <param name="lView">The ListView to be test.</param>
        /// <returns>The current SortKey value.</returns>
        [Description("Returns/sets the current sort key"), Category("Custom Properties")]
        public int GetSortKey(ListView lView)
        {
            return GetSortKeyProperty(lView);
        }
        /// <summary>
        /// Sets SortKey property for a ListView.
        /// </summary>
        /// <param name="lView">The ListView to be set.</param>
        /// <param name="value">The new sortkey value.</param>
        public void SetSortKey(ListView lView, int value)
        {
            SetSortKeyProperty(lView, value, DesignMode);
        }

        /// <summary>
        /// Gets SortOrder property for a ListView.
        /// </summary>
        /// <param name="lView">The ListView to be test.</param>
        /// <returns>Indicates if values of ListView are ordered ascending or descending.</returns>
        [Description("Returns/sets whether or not the ListItems will be sorted in ascending or descending order."), Category("Custom Properties")]
        public SortOrder GetSortOrder(ListView lView)
        {
            return GetSortOrderProperty(lView);
        }
        /// <summary>
        /// Sets SortOrder property for a ListView.
        /// </summary>
        /// <param name="lView">The ListView to be set.</param>
        /// <param name="value">The new SortOrder value indicating the kind of ordering for the ListView.</param>
        public void SetSortOrder(ListView lView, SortOrder value)
        {
            SetSortOrderProperty(lView, value, DesignMode);
        }

        /// <summary>
        /// Gets CorrectEventsBehavior property for a ListView.
        /// </summary>
        /// <param name="lView">The ListView to be test.</param>
        /// <returns>If events must be corrected or not.</returns>
        [Description("Indicates if some events should be patched to be fired in the same way that used to be fired in VB6"), Category("Custom Properties")]
        public bool GetCorrectEventsBehavior(ListView lView)
        {
            if (CheckForProperty(lView, newPropertiesEnum.CorrectEventsBehavior))
                return Convert.ToBoolean(newProperties[lView][newPropertiesEnum.CorrectEventsBehavior]);
            else
                return false;
        }
        /// <summary>
        /// Sets CorrectEventsBehavior property for a ListView.
        /// </summary>
        /// <param name="lView">The ListView to be set.</param>
        /// <param name="value">The new value indicating if events must be corrected.</param>
        public void SetCorrectEventsBehavior(ListView lView, bool value)
        {
            if (CheckForProperty(lView, newPropertiesEnum.CorrectEventsBehavior))
                newProperties[lView][newPropertiesEnum.CorrectEventsBehavior] = value;
        }

        /// <summary>
        /// Gets the name of the method to be invoked when the item click is fired, 
        /// this is a custom event that will be handled internally.
        /// </summary>
        /// <param name="lView">The listView to get the property.</param>
        /// <returns>The name of the method which will be invoked when the event should be fired,
        ///  it should receive a ListViewItem item as parameter.</returns>
        [Description("The name of the item click method, it should receive a ListViewItem item as parameter"), Category("Custom Event Handlers")]
        public string GetItemClickMethod(ListView lView)
        {
            if (CheckForProperty(lView, newPropertiesEnum.ItemClickMethod))
                return (string)newProperties[lView][newPropertiesEnum.ItemClickMethod];
            else
                return string.Empty;
        }
        /// <summary>
        /// Sets the name of the method to be invoked when the item click is fired, 
        /// this is a custom event that will be handled internally.
        /// </summary>
        /// <param name="lView">The listView to set the property.</param>
        /// <param name="value">The name of the method which will be invoked when 
        /// the event should be fired, it should receive a ListViewItem item as parameter.</param>
        public void SetItemClickMethod(ListView lView, string value)
        {
            if (CheckForProperty(lView, newPropertiesEnum.ItemClickMethod))
                newProperties[lView][newPropertiesEnum.ItemClickMethod] = value;
        }

        /// <summary>
        /// Gets the name of the VB6 ListView in the form to use for the list of large icons.
        /// </summary>
        /// <param name="lView">The ListView where to find the name.</param>
        /// <returns>The name of the VB6 ListView.</returns>
        [Description("Returns/sets the name of the VB6 ListView control to use when displaying items as large icons. Note: The property LargeImageList will be affected in runtime"), Category("Custom Properties")]
        public string GetLargeIcons(ListView lView)
        {
            object value = GetLargeIconsProperty(lView);
            if (value is string)
                return (string)value;
            else if (ImageListHelper.IsValid(value))
                return (string)Utils.ReflectionHelper.GetMember(value, "Name");
            else
                throw new InvalidCastException("Invalid property value");
        }

        /// <summary>
        /// Sets the name of the VB6 ListView in the form to use for the list of large icons.
        /// </summary>
        /// <param name="lView">The ListView where to set the name.</param>
        /// <param name="value">The new name of the VB6 ListView.</param>
        public void SetLargeIcons(ListView lView, string value)
        {
            SetLargeIconsProperty(lView, value, OnInitialization, DesignMode);
        }

        /// <summary>
        /// Gets the name of the VB6 listview in the form to use for the list of small icons.
        /// </summary>
        /// <param name="lView">The ListView where to find the name.</param>
        /// <returns>The name of the VB6 ListView.</returns>
        [Description("Returns/sets the name of the VB6 ListView control to use when displaying items as small icons. Note: The property SmallImageList will be affected in runtime"), Category("Custom Properties")]
        public string GetSmallIcons(ListView lView)
        {
            object value = GetSmallIconsProperty(lView);
            if (value is string)
                return (string)value;
            else if (ImageListHelper.IsValid(value))
                return (string)Utils.ReflectionHelper.GetMember(value, "Name");
            else
                throw new InvalidCastException("Invalid property value");
        }

        /// <summary>
        /// Sets the name of the VB6 ListView in the form to use for the list of small icons.
        /// </summary>
        /// <param name="lView">The ListView where to set the name.</param>
        /// <param name="value">The new name of the VB6 ListView.</param>
        public void SetSmallIcons(ListView lView, string value)
        {
            SetSmallIconsProperty(lView, value, OnInitialization, DesignMode);
        }

        /// <summary>
        /// Gets the name of the VB6 ListView in the form to use for the column headers icons.
        /// </summary>
        /// <param name="lView">The ListView where to find the name.</param>
        /// <returns>The name of the VB6 ListView.</returns>
        [Description("Returns/sets the name of the VB6 ListView control used to store the images to show in the column headers"), Category("Custom Properties")]
        public string GetColumnHeaderIcons(ListView lView)
        {
            object value = GetColumnHeaderIconsProperty(lView);
            if (value is string)
                return (string)value;
            else if (ImageListHelper.IsValid(value))
                return (string)Utils.ReflectionHelper.GetMember(value, "Name");
            else
                throw new InvalidCastException("Invalid property value");
        }
        /// <summary>
        /// Sets the name of the VB6 ListView in the form to use for the column headers icons.
        /// </summary>
        /// <param name="lView">>The ListView where to set the name.</param>
        /// <param name="value">The name of the VB6 ListView.</param>
        public void SetColumnHeaderIcons(ListView lView, string value)
        {
            SetColumnHeaderIconsProperty(lView, value, OnInitialization, DesignMode);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////// INSTANCE PROPERTIES DEFINITION //////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////


        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////// STATIC PROPERTIES DEFINITION ////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets static property for Sorted property.
        /// </summary>
        /// <param name="lView">The ListView to test.</param>
        /// <returns>The Sorted value in the ListView.</returns>
        public static bool GetSortedProperty(ListView lView)
        {
            if (CheckForProperty(lView, newPropertiesEnum.SortedProperty))
            {
                bool res = Convert.ToBoolean(newProperties[lView][newPropertiesEnum.SortedProperty]);
                SyncSortedProperty(lView, res);

                return res;
            }
            else
                return false;
        }

        /// <summary>
        /// Sets static property for Sorted property.
        /// </summary>
        /// <param name="lView">The ListView to set.</param>
        /// <param name="value">The new Sorted value.</param>
        public static void SetSortedProperty(ListView lView, bool value)
        {
            if (CheckForProperty(lView, newPropertiesEnum.SortedProperty))
            {
                newProperties[lView][newPropertiesEnum.SortedProperty] = value;

                GetCustomListItemComparer(lView).Sorted = value;
                SyncSortedProperty(lView, value);
            }
        }

        /// <summary>
        /// Gets static property for SortKey property.
        /// </summary>
        /// <param name="lView">The ListView to test.</param>
        /// <returns>The SortKey value in the ListView.</returns>
        public static int GetSortKeyProperty(ListView lView)
        {
            if (CheckForProperty(lView, newPropertiesEnum.SortKeyProperty))
                return Convert.ToInt32(newProperties[lView][newPropertiesEnum.SortKeyProperty]);
            else
                return 0;
        }
        /// <summary>
        /// Sets static property for SortKey property.
        /// </summary>
        /// <param name="lView">The ListView to set.</param>
        /// <param name="value">The new SortKey value.</param>
        public static void SetSortKeyProperty(ListView lView, int value)
        {
            SetSortKeyProperty(lView, value, false);
        }
        /// <summary>
        /// Sets static property for SortKey property.
        /// </summary>
        /// <param name="lView">The ListView to set.</param>
        /// <param name="value">The new SortKey value.</param>
        /// <param name="onDesignMode">Indicates if design mode is currently active.</param>
        private static void SetSortKeyProperty(ListView lView, int value, bool onDesignMode)
        {
            if (CheckForProperty(lView, newPropertiesEnum.SortKeyProperty))
            {
                if ((lView.Columns.Count > 0) && ((value < 0) || (value >= lView.Columns.Count)))
                    throw new InvalidOperationException("Invalid property value");

                newProperties[lView][newPropertiesEnum.SortKeyProperty] = value;
                GetCustomListItemComparer(lView).SortKey = value;
                if (!onDesignMode)
                    lView.Sort();
            }
        }

        /// <summary>
        /// Gets static property for SortOrder property.
        /// </summary>
        /// <param name="lView">The ListView to test.</param>
        /// <returns>The SortOrder value in the ListView.</returns>
        public static SortOrder GetSortOrderProperty(ListView lView)
        {
            if (CheckForProperty(lView, newPropertiesEnum.SortOrderProperty))
                return (SortOrder)(newProperties[lView][newPropertiesEnum.SortOrderProperty]);
            else
                return SortOrder.Ascending;
        }
        /// <summary>
        /// Sets static property for SortOrder property.
        /// </summary>
        /// <param name="lView">The ListView to set.</param>
        /// <param name="value">The new SortOrder value.</param>
        public static void SetSortOrderProperty(ListView lView, SortOrder value)
        {
            SetSortOrderProperty(lView, value, false);
        }
        /// <summary>
        /// Sets static property for SortOrder property.
        /// </summary>
        /// <param name="lView">The ListView to set.</param>
        /// <param name="value">The new SortOrder value.</param>
        /// <param name="onDesignMode">Indicates if design mode is currently active.</param>
        private static void SetSortOrderProperty(ListView lView, SortOrder value, bool onDesignMode)
        {
            if (CheckForProperty(lView, newPropertiesEnum.SortOrderProperty))
            {

                if (value != SortOrder.None)
                {
                    newProperties[lView][newPropertiesEnum.SortOrderProperty] = value;
                    GetCustomListItemComparer(lView).SortOrder = value;
                    if (!onDesignMode)
                        lView.Sort();
                }
            }
        }

        /// <summary>
        /// Gets the name of the VB6 ListView in the form to use for the list of large icons.
        /// </summary>
        /// <param name="lView">The ListView to test.</param>
        /// <returns>The name of the VB6 ListView.</returns>
        public static object GetLargeIconsProperty(ListView lView)
        {
            if (CheckForProperty(lView, newPropertiesEnum.LargeIcons))
                return newProperties[lView][newPropertiesEnum.LargeIcons];
            else
                return string.Empty;
        }
        /// <summary>
        /// Sets the name of the VB6 ListView in the form to use for the list of large icons.
        /// </summary>
        /// <param name="lView">The ListView to set.</param>
        /// <param name="value">The new name for the VB6 ListView.</param>
        /// <param name="onDesignMode">Is Design Mode</param>
        public static void SetLargeIconsProperty(ListView lView, object value, bool onDesignMode)
        {
            SetLargeIconsProperty(lView, value, false, onDesignMode);
        }
        /// <summary>
        /// Sets the name of the VB6 ListView in the form to use for the list of large icons.
        /// </summary>
        /// <param name="lView">The ListView to set.</param>
        /// <param name="value">The new name for the VB6 ListView.</param>
        /// <param name="delayProcessing">Delays the processing of the property to after the EndInit.</param>
        /// <param name="onDesignMode">Is Design Mode</param>
        private static void SetLargeIconsProperty(ListView lView, object value, bool delayProcessing, bool onDesignMode)
        {
            if ((value is string) || ImageListHelper.IsValid(value))
            {
                if (CheckForProperty(lView, newPropertiesEnum.LargeIcons))
                {
                    newProperties[lView][newPropertiesEnum.LargeIcons] = value;
                    if (!delayProcessing)
                        ProcessLargeIconsProperty(lView, onDesignMode);
                    else
                    {
                        if (!PendingListIconsToProcess.Contains(lView))
                            PendingListIconsToProcess.Add(lView);
                    }
                }
            }
            else
                throw new InvalidCastException("Invalid property value");
        }

        /// <summary>
        /// Gets the name of the VB6 ListView in the form to use for the list of small icons.
        /// </summary>
        /// <param name="lView">The ListView to test.</param>
        /// <returns>The name of the VB6 ListView.</returns>
        public static object GetSmallIconsProperty(ListView lView)
        {
            if (CheckForProperty(lView, newPropertiesEnum.SmallIcons))
                return newProperties[lView][newPropertiesEnum.SmallIcons];
            else
                return string.Empty;
        }
        /// <summary>
        /// Sets the name of the VB6 ListView in the form to use for the list of small icons.
        /// </summary>
        /// <param name="lView">The ListView to set.</param>
        /// <param name="value">The new name for the VB6 ListView.</param>
        /// <param name="onDesignMode">Is Design Mode</param>
        public static void SetSmallIconsProperty(ListView lView, object value, bool onDesignMode)
        {
            SetSmallIconsProperty(lView, value, false, onDesignMode);
        }
        /// <summary>
        /// Sets the name of the VB6 ListView in the form to use for the list of small icons.
        /// </summary>
        /// <param name="lView">The ListView to set.</param>
        /// <param name="value">The new value for the property.</param>
        /// <param name="delayProcessing">Delays the processing of the property to after the EndInit</param>
        /// <param name="onDesignMode">Is Design Mode</param>
        private static void SetSmallIconsProperty(ListView lView, object value, bool delayProcessing, bool onDesignMode)
        {
            if ((value is string) || ImageListHelper.IsValid(value))
            {
                if (CheckForProperty(lView, newPropertiesEnum.SmallIcons))
                {
                    newProperties[lView][newPropertiesEnum.SmallIcons] = value;
                    if (!delayProcessing)
                        ProcessSmallIconsProperty(lView, onDesignMode);
                    else
                    {
                        if (!PendingListIconsToProcess.Contains(lView))
                            PendingListIconsToProcess.Add(lView);
                    }
                }
            }
            else
                throw new InvalidCastException("Invalid property value");
        }

        /// <summary>
        /// Gets the name of the VB6 ListView in the form to use for the column headers.
        /// </summary>
        /// <param name="lView">The ListView to test.</param>
        /// <returns>The name of the VB6 ListView.</returns>
        public static object GetColumnHeaderIconsProperty(ListView lView)
        {
            if (CheckForProperty(lView, newPropertiesEnum.ColumnHeaderIcons))
                return newProperties[lView][newPropertiesEnum.ColumnHeaderIcons];
            else
                return string.Empty;
        }
        /// <summary>
        /// Sets the name of the VB6 ListView in the form to use for the column headers
        /// </summary>
        /// <param name="lView">The ListView to set</param>
        /// <param name="value">The name for VB6 ListView</param>
        /// <param name="onDesignMode">Is Design Mode</param>
        public static void SetColumnHeaderIconsProperty(ListView lView, object value, bool onDesignMode)
        {
            SetColumnHeaderIconsProperty(lView, value, false, onDesignMode);
        }
        /// <summary>
        /// Sets the name of the VB6 listview in the form to use for the column headers.
        /// </summary>
        /// <param name="lView">The ListView to set</param>
        /// <param name="value">The new value for the property.</param>
        /// <param name="delayProcessing">Delays the processing of the property to after the EndInit.</param>
        /// <param name="onDesignMode">Is Design Mode</param>
        private static void SetColumnHeaderIconsProperty(ListView lView, object value, bool delayProcessing, bool onDesignMode)
        {
            if ((value is string) || ImageListHelper.IsValid(value))
            {
                if (CheckForProperty(lView, newPropertiesEnum.ColumnHeaderIcons))
                {
                    newProperties[lView][newPropertiesEnum.ColumnHeaderIcons] = value;
                    if (!delayProcessing)
                        ProcessColumnHeaderIconsProperty(lView, onDesignMode);
                    else
                    {
                        if (!PendingListIconsToProcess.Contains(lView))
                            PendingListIconsToProcess.Add(lView);
                    }
                }
            }
            else
                throw new InvalidCastException("Invalid property value");
        }

        /// <summary>
        /// Gets the Icon property of a ColumnHeader.
        /// </summary>
        /// <param name="cHeader">The source ColumnHeader.</param>
        /// <returns>The key|index of the Icon for the ColumnHeader.</returns>
        public static object GetColumnHeaderItemIconProperty(ColumnHeader cHeader)
        {
            if (!string.IsNullOrEmpty(cHeader.ImageKey))
                return cHeader.ImageKey;
            else
                return cHeader.ImageIndex;
        }
        /// <summary>
        /// Sets the Icon property of a ColumnHeader.
        /// </summary>
        /// <param name="cHeader">The source ColumnHeader.</param>
        /// <param name="value">The new key|index of the Icon for the ColumnHeader.</param>
        public static void SetColumnHeaderItemIconProperty(ColumnHeader cHeader, object value)
        {
            if (value is string)
            {
                cHeader.ImageIndex = -1;
                cHeader.ImageKey = (string)value;
            }

            if (value is Int32)
            {
                cHeader.ImageKey = string.Empty;
                cHeader.ImageIndex = (int)value;
            }
        }

        /// <summary>
        /// Gets the Icon property of a ListItem (Key|Index to use for LargeIcons).
        /// </summary>
        /// <param name="lItem">The source ListItem.</param>
        /// <returns>The Key|Index of the Icon to use when LargeIcons are shown.</returns>
        public static object GetListItemIconProperty(ListViewItem lItem)
        {
            Dictionary<ListViewItem, object> listViewItemIconLists = null;
            ListView lView = lItem.ListView;

            if (CheckForProperty(lView, newPropertiesEnum.ListItemIcon))
            {
                listViewItemIconLists = (Dictionary<ListViewItem, object>)newProperties[lView][newPropertiesEnum.ListItemIcon];
                if (listViewItemIconLists.ContainsKey(lItem))
                {
                    return listViewItemIconLists[lItem];
                }
                else
                {
                    if (!string.IsNullOrEmpty(lItem.ImageKey))
                        return lItem.ImageKey;
                    else
                        return lItem.ImageIndex;
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// Sets the Icon property of a ListItem (Key|Index to use for LargeIcons).
        /// </summary>
        /// <param name="lItem">The source ListItem.</param>
        /// <param name="value">The new Key|Index of the Icon to use when LargeIcons are shown.</param>
        public static void SetListItemIconProperty(ListViewItem lItem, object value)
        {
            Dictionary<ListViewItem, object> listViewItemIconLists = null;
            ListView lView = lItem.ListView;

            if ((value is string) || (value is Int32))
            {
                if (CheckForProperty(lView, newPropertiesEnum.ListItemIcon))
                {
                    listViewItemIconLists = (Dictionary<ListViewItem, object>)newProperties[lView][newPropertiesEnum.ListItemIcon];

                    if (listViewItemIconLists.ContainsKey(lItem))
                        listViewItemIconLists[lItem] = value;
                    else
                    {
                        if (value is string)
                        {
                            lItem.ImageIndex = -1;
                            lItem.ImageKey = (string)value;
                        }

                        if (value is Int32)
                        {
                            lItem.ImageKey = string.Empty;
                            lItem.ImageIndex = (int)value;
                        }
                    }
                }
            }
            else
                throw new InvalidCastException("Invalid property value");
        }

        /// <summary>
        /// Gets the SmallIcon property of a ListItem (Key|Index to use for SmallIcons).
        /// </summary>
        /// <param name="lItem">The source ListItem.</param>
        /// <returns>The Key|Index of the SmallIcon to use when SmallIcons are shown.</returns>
        public static object GetListItemSmallIconProperty(ListViewItem lItem)
        {
            Dictionary<ListViewItem, object> listViewItemSmallIconLists = null;
            ListView lView = lItem.ListView;

            if (CheckForProperty(lView, newPropertiesEnum.ListItemSmallIcon))
            {
                listViewItemSmallIconLists = (Dictionary<ListViewItem, object>)newProperties[lView][newPropertiesEnum.ListItemSmallIcon];
                if (listViewItemSmallIconLists.ContainsKey(lItem))
                    return listViewItemSmallIconLists[lItem];
                else
                    return -1;
            }
            else
                return -1;
        }
        /// <summary>
        /// Sets the SmallIcon property of a ListItem (Key|Index to use for SmallIcons).
        /// </summary>
        /// <param name="lItem">The source ListItem.</param>
        /// <param name="value">The new Key|Index of the SmallIcon to use when SmallIcons are shown.</param>
        public static void SetListItemSmallIconProperty(ListViewItem lItem, object value)
        {
            Dictionary<ListViewItem, object> listViewItemSmallIconLists = null;
            ListView lView = lItem.ListView;

            if ((value is string) || (value is Int32))
            {
                if (CheckForProperty(lView, newPropertiesEnum.ListItemSmallIcon))
                {
                    listViewItemSmallIconLists = (Dictionary<ListViewItem, object>)newProperties[lView][newPropertiesEnum.ListItemSmallIcon];

                    if (!listViewItemSmallIconLists.ContainsKey(lItem))
                        listViewItemSmallIconLists.Add(lItem, value);
                    else
                        listViewItemSmallIconLists[lItem] = value;

                    PatchDrawItemEvents(lView);
                }
                return;
            }

            throw new InvalidCastException("Invalid property value");
        }

        /// <summary>
        /// Sets the SmallIcon property of a ListItem, it works as SetListItemSmallIconProperty does 
        /// but the ListItem is returned so it can be used in the normal upgrade of the functions 
        /// ListView.Add and ListView.Insert.
        /// </summary>
        /// <param name="lItem">The ListItem source.</param>
        /// <param name="value"></param>
        /// <returns>The resultant ListView item.</returns>
        public static ListViewItem AddListItemSmallIconProperty(ListViewItem lItem, object value)
        {
            SetListItemSmallIconProperty(lItem, value);
            return lItem;
        }

        /// <summary>
        /// In order to handle the property SmallIcons of a ListItem some Draw events must be handled.
        /// </summary>
        /// <param name="lView">The ListView source.</param>
        private static void PatchDrawItemEvents(ListView lView)
        {
            Delegate[] EventDelegates = null;
            bool PatchDrawItem = true;
            bool PatchDrawSubItem = true;
            bool PatchDrawColumnHeader = true;
            EventInfo eInfo = null;

            if (EventsPatched.ContainsKey(lView))
            {
                //The events were previously patched
                if (EventsPatched[lView].ContainsKey(DrawItemEventName))
                    PatchDrawItem = false;

                if (EventsPatched[lView].ContainsKey(DrawColumnHeaderEventName))
                    PatchDrawColumnHeader = false;

                if (EventsPatched[lView].ContainsKey(DrawSubItemEventName))
                    PatchDrawSubItem = false;
            }
            else
                EventsPatched.Add(lView, new Dictionary<string, List<Delegate>>());

            if (PatchDrawItem)
            {
                eInfo = lView.GetType().GetEvent(DrawItemEventName);
                if (eInfo == null)
                    throw new InvalidOperationException("Event info for event '" + DrawItemEventName + "' could not be found");

                EventsPatched[lView].Add(DrawItemEventName, new List<Delegate>());
                EventDelegates = ContainerHelper.GetEventSubscribers(lView, DrawItemEventName);
                if (EventDelegates != null)
                {
                    foreach (Delegate del in EventDelegates)
                    {
                        EventsPatched[lView][DrawItemEventName].Add(del);
                        eInfo.RemoveEventHandler(lView, del);
                    }
                }
                lView.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(ListView_DrawItem);
            }

            if (PatchDrawColumnHeader)
            {
                eInfo = lView.GetType().GetEvent(DrawColumnHeaderEventName);
                if (eInfo == null)
                    throw new InvalidOperationException("Event info for event '" + DrawColumnHeaderEventName + "' could not be found");

                EventsPatched[lView].Add(DrawColumnHeaderEventName, new List<Delegate>());
                EventDelegates = ContainerHelper.GetEventSubscribers(lView, DrawColumnHeaderEventName);
                if (EventDelegates != null)
                {
                    foreach (Delegate del in EventDelegates)
                    {
                        EventsPatched[lView][DrawColumnHeaderEventName].Add(del);
                        eInfo.RemoveEventHandler(lView, del);
                    }
                }
                lView.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(ListView_DrawColumnHeader);
            }

            if (PatchDrawSubItem)
            {
                eInfo = lView.GetType().GetEvent(DrawSubItemEventName);
                if (eInfo == null)
                    throw new InvalidOperationException("Event info for event '" + DrawSubItemEventName + "' could not be found");

                EventsPatched[lView].Add(DrawSubItemEventName, new List<Delegate>());
                EventDelegates = ContainerHelper.GetEventSubscribers(lView, DrawSubItemEventName);
                if (EventDelegates != null)
                {
                    foreach (Delegate del in EventDelegates)
                    {
                        EventsPatched[lView][DrawSubItemEventName].Add(del);
                        eInfo.RemoveEventHandler(lView, del);
                    }
                }
                lView.DrawSubItem += new DrawListViewSubItemEventHandler(ListView_DrawSubItem);
            }

            lView.OwnerDraw = true;
        }

        /// <summary>
        /// Returns a subItem from a ListView item.
        /// </summary>
        /// <param name="lItem">The parent item.</param>
        /// <param name="index">The index of the item that has to be returned.</param>
        /// <returns>The found ListViewSubItem.</returns>
        public static ListViewItem.ListViewSubItem GetListViewSubItem(ListViewItem lItem, int index)
        {
            return GetListViewSubItem(lItem, lItem.ListView, index);
        }

        /// <summary>
        /// Returns a subItem from a ListView item.
        /// </summary>
        /// <param name="lItem">The parent item</param>
        /// <param name="parentListView">The parent ListView that will contain the ListView item.</param>
        /// <param name="index">The index of the item that has to be returned.</param>
        /// <returns>The found ListViewSubItem.</returns>
        public static ListViewItem.ListViewSubItem GetListViewSubItem(ListViewItem lItem, ListView parentListView, int index)
        {
            if ((parentListView.Columns.Count <= index) || (index < 0))
                throw new InvalidOperationException("Invalid property value");

            if (lItem.SubItems.Count <= index)
            {
                //lItem.SubItems.AddRange(Utils.ArraysHelper.InitializeArray<string>(index - lItem.SubItems.Count + 1));
                string[] strings = new string[index - lItem.SubItems.Count + 2];
                for (int i = 0; i < strings.Length; i++)
                    strings[i] = String.Empty;
                lItem.SubItems.AddRange(strings);
            }

            return lItem.SubItems[index];
        }

        /// <summary>
        /// Returns the left property for a column.
        /// </summary>
        /// <param name="lView">The ListView containing the column.</param>
        /// <param name="column">The Column to test.</param>
        /// <returns>The left value of the column.</returns>
        public static int GetListViewColumnLeftProperty(System.Windows.Forms.ListView lView, System.Windows.Forms.ColumnHeader column)
        {
            int Left = 0;
            for (int i = 0; i < column.Index; i++)
                Left += lView.Columns[i].Width;

            return Left;
        }

        /// <summary>
        /// Returns the left property for a column.
        /// </summary>
        /// <param name="columns">The ListView columns</param>
        /// <param name="columnIndex">The Column to test.</param>
        /// <returns>The left value of the column.</returns>
        public static int GetListViewColumnLeftProperty(System.Windows.Forms.ListView.ColumnHeaderCollection columns, int columnIndex)
        {
            int Left = 0;
            for (int i = 0; i < columnIndex; i++)
                Left += columns[i].Width;

            return Left;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////// STATIC PROPERTIES DEFINITION ////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Check if the property 'newPropertiesEnum' is already defined for this list view.
        /// </summary>
        /// <param name="lView">The list view to test.</param>
        /// <param name="prop">The new PropertiesEnum.</param>
        private static bool CheckForProperty(ListView lView, newPropertiesEnum prop)
        {
            if (lView == null)
                return false;

            CheckNewProperties(lView);
            if (!newProperties[lView].ContainsKey(prop))
                newProperties[lView][prop] = GetDefaultValueForProperty(prop);

            //Ensures that a custom class has been set to do the ordering
            if ((prop == newPropertiesEnum.SortedProperty) || (prop == newPropertiesEnum.SortKeyProperty)
                || (prop == newPropertiesEnum.SortOrderProperty))
            {
                if ((lView.ListViewItemSorter == null) || !(lView.ListViewItemSorter is ListViewItemComparer))
                {
                    if (!newProperties[lView].ContainsKey(newPropertiesEnum.CustomSortingClass))
                        newProperties[lView][newPropertiesEnum.CustomSortingClass] = GetDefaultValueForProperty(newPropertiesEnum.CustomSortingClass);

                    lView.ListViewItemSorter = (System.Collections.IComparer)newProperties[lView][newPropertiesEnum.CustomSortingClass];
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the lView is controlled by the newProperties Dictionary.
        /// </summary>
        /// <param name="lView">The ListView to test.</param>
        private static void CheckNewProperties(ListView lView)
        {
            if (!newProperties.ContainsKey(lView))
            {
                newProperties[lView] = new Dictionary<newPropertiesEnum, object>();
                lView.Disposed += new EventHandler(ListView_Disposed);
            }
        }

        /// <summary>
        /// Returns a default value for the specified property.
        /// </summary>
        /// <param name="prop">The property requesting a default value.</param>
        /// <returns>A default value casted as object.</returns>
        private static object GetDefaultValueForProperty(newPropertiesEnum prop)
        {
            switch (prop)
            {
                case newPropertiesEnum.SortedProperty:
                case newPropertiesEnum.CorrectEventsBehavior:
                    return false;
                case newPropertiesEnum.SortKeyProperty:
                    return 0;
                case newPropertiesEnum.SortOrderProperty:
                    return SortOrder.Ascending;
                case newPropertiesEnum.CustomSortingClass:
                    return new ListViewItemComparer();
                case newPropertiesEnum.LargeIcons:
                case newPropertiesEnum.SmallIcons:
                case newPropertiesEnum.ColumnHeaderIcons:
                case newPropertiesEnum.ItemClickMethod:
                    return string.Empty;
                case newPropertiesEnum.ListItemSmallIcon:
                case newPropertiesEnum.ListItemIcon:
                    return new Dictionary<ListViewItem, object>();
                case newPropertiesEnum.InternalColumnHeaderImageListHelper:
                    return new ImageListHelper();
            }

            return null;
        }

        /// <summary>
        /// Gets the Custom List Item Comparer for a ListView.
        /// </summary>
        /// <param name="lView">The ListView to test.</param>
        /// <returns>The custom comparer for the ListView.</returns>
        private static ListViewItemComparer GetCustomListItemComparer(ListView lView)
        {
            return (ListViewItemComparer)newProperties[lView][newPropertiesEnum.CustomSortingClass];
        }

        /// <summary>
        /// The value for Sorted depends in the value of the property Sorting, so 
        /// whenever you set Sorted it syncs to Sorting.
        /// </summary>
        /// <param name="lView">The ListView to sync.</param>
        /// <param name="res">Indicates if sort must be done.</param>
        private static void SyncSortedProperty(ListView lView, bool res)
        {
            if (res)
                lView.Sorting = SortOrder.Ascending;
            else
                lView.Sorting = SortOrder.None;
        }

        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        public void BeginInit()
        {
            OnInitialization = true;
        }

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        public void EndInit()
        {
            if (!DesignMode)
            {
                CleanDeadReferences();
                CorrectEventsBehavior();
                ProcessIconListsProperties();
            }
            OnInitialization = false;
        }

        /// <summary>
        /// Loads the .NET ImageLists to be used for ListViews from the VB6 ImageLists.
        /// </summary>
        private void ProcessIconListsProperties()
        {
            lock (objLockEvents)
            {
                try
                {
                    foreach (ListView lView in PendingListIconsToProcess)
                    {
                        if (newProperties[lView].ContainsKey(newPropertiesEnum.LargeIcons))
                            ProcessLargeIconsProperty(lView, DesignMode);

                        if (newProperties[lView].ContainsKey(newPropertiesEnum.SmallIcons))
                            ProcessSmallIconsProperty(lView, DesignMode);

                        if (newProperties[lView].ContainsKey(newPropertiesEnum.ColumnHeaderIcons))
                            ProcessColumnHeaderIconsProperty(lView, DesignMode);
                    }

                }
                catch { }
                PendingListIconsToProcess.Clear();
            }
        }

        /// <summary>
        /// Process the Column Header Icons property for a ListView.
        /// </summary>
        /// <param name="lView">The ListView to set the property.</param>
        /// <param name="onDesignMode">Is Design Mode</param>
        private static void ProcessColumnHeaderIconsProperty(ListView lView, bool onDesignMode)
        {
            string name = string.Empty;

            ImageListHelper imgHelper = null;
            ImageListHelper currentImgHelper = null;
            object value = newProperties[lView][newPropertiesEnum.ColumnHeaderIcons];
            if (value is string)
            {
                name = (string)value;
                if (!string.IsNullOrEmpty(name))
                {
                    if (CheckForProperty(lView, newPropertiesEnum.InternalColumnHeaderImageListHelper))
                    {
                        currentImgHelper = (ImageListHelper)newProperties[lView][newPropertiesEnum.InternalColumnHeaderImageListHelper];

                        if (!currentImgHelper.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                        {
                            imgHelper = GetImageListHelper(lView, name, onDesignMode);
                            imgHelper.Name = name;
                            newProperties[lView][newPropertiesEnum.InternalColumnHeaderImageListHelper] = imgHelper;
                            CleanColumnHeaderItemIconProperty(lView);
                        }
                    }
                }
                PatchDrawItemEvents(lView);
            }
            else if (ImageListHelper.IsValid(value))
            {
                if (CheckForProperty(lView, newPropertiesEnum.InternalColumnHeaderImageListHelper))
                {
                    currentImgHelper = (ImageListHelper)newProperties[lView][newPropertiesEnum.InternalColumnHeaderImageListHelper];

                    name = (string)Utils.ReflectionHelper.GetMember(value, "Name");
                    if (!currentImgHelper.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        imgHelper = GetImageListHelper(value);
                        imgHelper.Name = name;
                        newProperties[lView][newPropertiesEnum.InternalColumnHeaderImageListHelper] = imgHelper;
                        CleanColumnHeaderItemIconProperty(lView);
                    }
                    PatchDrawItemEvents(lView);
                }
            }
        }

        /// <summary>
        /// Cleans the values for the ColumHeaderItemIcon of each ColumnHeader in the ListView.
        /// </summary>
        /// <param name="lView">The parent ListView.</param>
        private static void CleanColumnHeaderItemIconProperty(ListView lView)
        {
            foreach (ColumnHeader cHeader in lView.Columns)
            {
                SetColumnHeaderItemIconProperty(cHeader, -1);
            }
        }

        /// <summary>
        /// Process the small icons property for a ListView.
        /// </summary>
        /// <param name="lView">The ListView to set the property.</param>
        /// <param name="onDesignMode">Is Design Mode</param>
        private static void ProcessSmallIconsProperty(ListView lView, bool onDesignMode)
        {
            ImageListHelper imgHelper = null;
            object value = newProperties[lView][newPropertiesEnum.SmallIcons];
            if (value is string)
            {
                if (!string.IsNullOrEmpty((string)value))
                {
                    imgHelper = GetImageListHelper(lView, (string)value, onDesignMode);
                    newProperties[lView][newPropertiesEnum.SmallIcons] = imgHelper;
                    lView.SmallImageList = imgHelper.NETImageList;
                }
            }
            else if (ImageListHelper.IsValid(value))
            {
                imgHelper = GetImageListHelper(value);
                lView.SmallImageList = imgHelper.NETImageList;
            }
        }

        /// <summary>
        /// Process the large icons property for a ListView.
        /// </summary>
        /// <param name="lView">The ListView to set the property.</param>
        /// <param name="onDesignMode">Is Design Mode</param>
        private static void ProcessLargeIconsProperty(ListView lView, bool onDesignMode)
        {
            ImageListHelper imgHelper = null;
            object value = newProperties[lView][newPropertiesEnum.LargeIcons];
            if (value is string)
            {
                if (!string.IsNullOrEmpty((string)value))
                {
                    imgHelper = GetImageListHelper(lView, (string)value, onDesignMode);
                    lView.LargeImageList = imgHelper.NETImageList;
                }
            }
            else if (ImageListHelper.IsValid(value))
            {
                imgHelper = GetImageListHelper(value);
                lView.LargeImageList = imgHelper.NETImageList;
            }
        }

        /// <summary>
        /// Returns a ImageListHelper created based on a VB6 ImageList (name).
        /// </summary>
        /// <param name="lView">The ListView is used to get access to 
        /// the original VB6 ImageList based on its name.</param>
        /// <param name="VB6ImageListName">The name of the VB6 Image List.</param>
        /// <param name="onDesignMode">Is Design Mode?</param>
        /// <returns>An instance of a ImageListHelper.</returns>
        private static ImageListHelper GetImageListHelper(ListView lView, string VB6ImageListName, bool onDesignMode)
        {
            ImageListHelper imgHelper = new ImageListHelper();
            Form parentForm = lView.FindForm();
            if (parentForm != null)
            {
                object imlControl = ContainerHelper.Controls(parentForm)[VB6ImageListName];
                if (imlControl != null)
                {
                    imgHelper.LoadVB6ImageList(imlControl);
                }
                else
                {
                    if (!onDesignMode)
                    {
                        Type type = parentForm.GetType();
                        FieldInfo finfo = type.GetField(VB6ImageListName);
                        if (finfo != null)
                        {
                            object field_value = finfo.GetValue(parentForm);
                            imgHelper.NETImageList = field_value as ImageList;
                        }
                    }
                }
            }
            return imgHelper;
        }

        /// <summary>
        /// Returns a ImageListHelper created based on a VB6 ImageList object.
        /// </summary>
        /// <param name="VB6ImageList">The VB6 Imagelist object.</param>
        /// <returns>An instance of a ImageListHelper.</returns>
        private static ImageListHelper GetImageListHelper(object VB6ImageList)
        {
            ImageListHelper imgHelper = new ImageListHelper();
            imgHelper.LoadVB6ImageList(VB6ImageList);
            return imgHelper;
        }

        /// <summary>
        /// Cleans the public dictionaries from old references of ListViews alreay disposed.
        /// </summary>
        private void CleanDeadReferences()
        {
            try
            {
                List<ListView> toClean = new List<ListView>();
                foreach (ListView lView in newProperties.Keys)
                {
                    if (lView.IsDisposed)
                        toClean.Add(lView);
                }
                foreach (ListView lView in toClean)
                {
                    newProperties.Remove(lView);
                }

                toClean.Clear();
                foreach (ListView lView in EventsPatched.Keys)
                {
                    if (lView.IsDisposed)
                        toClean.Add(lView);
                }
                foreach (ListView lView in toClean)
                {
                    EventsPatched.Remove(lView);
                }
            }
            catch { }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////// FUNCTIONS TO PATCH THE EVENTS ///////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////
        /* This is how this path of events is going to work:
         *  When in design code the property "CorrectEventsBehavior" is set to true for a specific 
         *  listview, the following code will be applied at the end of execution of InitializeComponent,
         *  that means at the end of the design code.
         *  This code will:
         *      - Remove the event handlers for certains event as they were specified in design time
         *      - Add a custom event handler for the specific event being patch (defined below)
         *      - The custome events defined here will decide how and under what circunstances the
         *          original events will be called
         * 
         *  This mean that we will remove the events defined by the user and add our owns and we decide
         *  how and when to call the user defined events.
         * 
         *  Restrictions:
         *      This will path the events defined in design time, if the user specify another events in
         *      runtime code they will not be patched.
         */



        /// <summary>
        /// Deattach some events for the ListViews in order to be managed internally. 
        /// It means to replace the current behaviour.
        /// </summary>
        private static void CorrectEventsBehavior()
        {
            List<ListView> lViewToCorrects = new List<ListView>();
            lock (objLockEvents)
            {
                foreach (ListView lView in newProperties.Keys)
                {
                    if ((newProperties[lView].ContainsKey(newPropertiesEnum.CorrectEventsBehavior))
                        && (Convert.ToBoolean(newProperties[lView][newPropertiesEnum.CorrectEventsBehavior])))
                    {
                        lViewToCorrects.Add(lView);
                        CorrectEventsForListView(lView);
                    }

                    //Patch for the ItemClicEvent
                    if (newProperties[lView].ContainsKey(newPropertiesEnum.ItemClickMethod))
                    {
                        PatchItemClickEvent(lView);
                    }
                }

                foreach (ListView lView in lViewToCorrects)
                {
                    newProperties[lView].Remove(newPropertiesEnum.CorrectEventsBehavior);
                }
            }
        }

        /// <summary>
        /// Patchs the custom event ItemClick for a listView.
        /// </summary>
        /// <param name="lView">The source ListView.</param>
        private static void PatchItemClickEvent(ListView lView)
        {
            Delegate del = null;
            try
            {
                string methodName = Convert.ToString(newProperties[lView][newPropertiesEnum.ItemClickMethod]).Trim();
                if (!string.IsNullOrEmpty(methodName))
                {
                    if (!EventsPatched[lView].ContainsKey(ItemClickEventName))
                    {
                        MethodInfo mInfo = lView.FindForm().GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                        if (mInfo.IsStatic)
                            del = Delegate.CreateDelegate(typeof(ListView_ItemClickDelegate), mInfo);
                        else
                            del = Delegate.CreateDelegate(typeof(ListView_ItemClickDelegate), lView.FindForm(), mInfo);

                        if (!EventsPatched.ContainsKey(lView))
                            EventsPatched.Add(lView, new Dictionary<string, List<Delegate>>());

                        EventsPatched[lView].Add(ItemClickEventName, new List<Delegate>());
                        EventsPatched[lView][ItemClickEventName].Add(del);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format(UpgradeHelpers.VB6.Resources.UpgradeHelpers_VB6.UpgradeHelpers_VB6_Help_ListViewHelper_PatchItemClickEvent_Err_Msg, e.Message));
            }
        }

        /// <summary>
        /// Patches the events for a specific ListView.
        /// </summary>
        /// <param name="lView">The source ListView.</param>
        private static void CorrectEventsForListView(ListView lView)
        {
            Delegate[] EventDelegates = null;

            if (EventsPatched.ContainsKey(lView))
                throw new InvalidOperationException("Events for this list view has been previously patched: '" + lView.Name + "'");

            EventsPatched.Add(lView, new Dictionary<string, List<Delegate>>());
            foreach (string eventName in EventsToCorrect.Keys)
            {
                EventInfo eInfo = lView.GetType().GetEvent(eventName);
                if (eInfo == null)
                    throw new InvalidOperationException("Event info for event '" + eventName + "' could not be found");

                EventsPatched[lView].Add(eventName, new List<Delegate>());
                EventDelegates = ContainerHelper.GetEventSubscribers(lView, eventName);
                if (EventDelegates != null)
                {

                    foreach (Delegate del in EventDelegates)
                    {
                        EventsPatched[lView][eventName].Add(del);
                        eInfo.RemoveEventHandler(lView, del);
                    }
                }
                eInfo.AddEventHandler(lView, EventsToCorrect[eventName]);
            }
        }

        /// <summary>
        /// Allows to invoke the patched events for a ListView.
        /// </summary>
        /// <param name="source">The source ListView.</param>
        /// <param name="eventName">The name of the event to be invoked.</param>
        /// <param name="args">The args of the event to be used in the invokation.</param>
        private static void InvokeEvents(ListView source, string eventName, object[] args)
        {
            if ((EventsPatched.ContainsKey(source)) && (EventsPatched[source].ContainsKey(eventName)))
            {
                foreach (Delegate del in EventsPatched[source][eventName])
                {
                    del.DynamicInvoke(args);
                }
            }
        }

        /// <summary>
        /// Event handler for the Disposed event of a ListView so we can clean it from EventsPatched.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void ListView_Disposed(object sender, EventArgs e)
        {
            ListView lView = (ListView)sender;
            if (newProperties.ContainsKey(lView))
                newProperties.Remove(lView);

            if (EventsPatched.ContainsKey(lView))
                EventsPatched.Remove(lView);
        }

        /// <summary>
        /// </summary>
        /// <param name="lView"></param>
        public void ManuallyRemoveFromPatchedEvents(ListView lView)
        {
            if (newProperties.ContainsKey(lView))
                newProperties.Remove(lView);
            if (EventsPatched.ContainsKey(lView))
                EventsPatched.Remove(lView);
        }

        /// <summary>
        /// Event handler for the MouseUp event of a ListView.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void ListView_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                ListView source = (ListView)sender;
                if (source.Focused)
                    InvokeEvents(source, ClickEventName, new object[] { sender, new EventArgs() });

                InvokeEvents(source, MouseUpEventName, new object[] { sender, e });

            }
            catch { }
        }

        /// <summary>
        /// Event handler for the Click event of a ListView.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void ListView_Click(object sender, EventArgs e)
        {
            try
            {
                //This event won't be fired from here, it will be fired by MouseUp event
                ListView source = (ListView)sender;

                //It will fire ItemClick event instead
                if (source.FocusedItem != null)
                    InvokeEvents(source, ItemClickEventName, new object[] { source.FocusedItem });

            }
            catch { }
        }

        /// <summary>
        /// Event handler for the Double Click event of a ListView.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void ListView_DoubleClick(object sender, EventArgs e)
        {
            //Nothing to do with this event
            //It will be fired by IMessageFilterImplementer.PreFilterMessage
        }

        /// <summary>
        /// Event handler for the Key Up event of a ListView.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The key event arguments.</param>
        private static void ListView_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                ListView source = (ListView)sender;

                //It will fire ItemClick event
                if ((source.FocusedItem != null) && (e.KeyCode != Keys.Tab) && (e.KeyCode != Keys.Enter))
                    InvokeEvents(source, ItemClickEventName, new object[] { source.FocusedItem });

                InvokeEvents(source, KeyUpEventName, new object[] { sender, e });

            }
            catch { }
        }

        /// <summary>
        /// Event handler for the DrawItem event of a ListView, 
        /// required to manage the property SmallIcon of the ListItems.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The DrawListView event arguments.</param>
        private static void ListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            object value = null;
            Dictionary<ListViewItem, object> listViewItemIconLists = null;
            Dictionary<ListViewItem, object> listViewItemSmallIconLists = null;
            ListView lView = (ListView)sender;

            if (CheckForProperty(lView, newPropertiesEnum.ListItemIcon))
            {
                listViewItemIconLists = (Dictionary<ListViewItem, object>)newProperties[lView][newPropertiesEnum.ListItemIcon];

                if (CheckForProperty(lView, newPropertiesEnum.ListItemSmallIcon))
                {
                    listViewItemSmallIconLists = (Dictionary<ListViewItem, object>)newProperties[lView][newPropertiesEnum.ListItemSmallIcon];
                    if (lView.View == View.LargeIcon)
                    {
                        if (listViewItemIconLists.ContainsKey(e.Item))
                        {
                            value = listViewItemIconLists[e.Item];
                            if (value is string)
                            {
                                e.Item.ImageIndex = -1;
                                e.Item.ImageKey = (string)value;
                            }
                            else
                            {
                                e.Item.ImageKey = string.Empty;
                                e.Item.ImageIndex = (int)value;
                            }
                            listViewItemIconLists.Remove(e.Item);
                        }
                    }
                    else
                    {
                        if (!listViewItemIconLists.ContainsKey(e.Item))
                        {
                            if (!string.IsNullOrEmpty(e.Item.ImageKey))
                                listViewItemIconLists.Add(e.Item, e.Item.ImageKey);
                            else
                                listViewItemIconLists.Add(e.Item, e.Item.ImageIndex);

                            if (listViewItemSmallIconLists.ContainsKey(e.Item))
                            {
                                value = listViewItemSmallIconLists[e.Item];
                                if (value is string)
                                {
                                    e.Item.ImageIndex = -1;
                                    e.Item.ImageKey = (string)value;
                                }
                                else
                                {
                                    e.Item.ImageKey = string.Empty;
                                    e.Item.ImageIndex = (int)value;
                                }
                            }
                            /*
                            else
                            {
                                e.Item.ImageIndex = -1;
                                e.Item.ImageKey = string.Empty;
                            }
                             */
                        }
                    }
                    InvokeEvents(lView, DrawItemEventName, new object[] { sender, e });
                }
            }
            //e.DrawDefault = true;
        }

        /// <summary>
        /// Event handler for the DrawSubItem event of a ListView, 
        /// required in order to process the property 
        /// listviewSubItem.UseItemStyleForSubItems when it is set to false.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The DrawListViewSubItem event arguments.</param>
        public static void ListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            ListView lView = (ListView)sender;
            if (e.ColumnIndex == 0) //do default drawing for the first column
            {
                e.DrawDefault = true;
            }
            else
            {
                //Draw SubItem
                ListViewItem.ListViewSubItem subitem = e.SubItem;
                if (subitem == null || subitem.Tag == null)
                {
                    e.DrawDefault = true;
                }
                else if (subitem.Tag != null)
                {
                    Image img = null;
                    ImageListHelper imgHelper = (ImageListHelper)newProperties[lView][newPropertiesEnum.SmallIcons];
                    if (imgHelper != null)
                    {
                        if (subitem.Tag is string)
                        {
                            string imgKey = subitem.Tag.ToString();
                            if (imgHelper.NETImageList.Images.ContainsKey(imgKey))
                            {
                                img = imgHelper.NETImageList.Images[imgKey];
                            }
                        }
                        else
                        {
                            if (subitem.Tag is int)
                            {
                                int imgIndex = Convert.ToInt32(subitem.Tag);
                                if (imgHelper.NETImageList.Images.Count > imgIndex)
                                {
                                    img = imgHelper.NETImageList.Images[imgIndex];
                                }
                            }
                        }
                    }
                    if (img != null)
                    {
                        e.DrawBackground();
                        bool focused = e.Item.ListView.Focused;
                        Color back = e.Item.Selected ? (focused ? SystemColors.Highlight : SystemColors.Menu) : e.SubItem.ForeColor;
                        Color fore = e.Item.Selected ? (focused ? SystemColors.HighlightText : e.SubItem.ForeColor) : e.SubItem.ForeColor;
                        int fonty = e.Bounds.Y + ((int)(e.Bounds.Height / 2)) - ((int)(e.SubItem.Font.Height / 2));
                        int x = e.Bounds.X + 2;
                        if (e.Item.Selected)
                        {
                            using (Brush backBrush = new SolidBrush(back))
                                e.Graphics.FillRectangle(backBrush, e.Bounds);
                        }
                        x = DrawSubItemIcon(e, x, img);
                        using (Brush foreBrush = new SolidBrush(fore))
                            e.Graphics.DrawString(e.SubItem.Text, e.SubItem.Font, foreBrush, x, fonty);
                    }
                    else
                        e.DrawDefault = true;
                }
                else
                    e.DrawDefault = true;
            }
            InvokeEvents(lView, DrawSubItemEventName, new object[] { sender, e });
            //e.DrawDefault = true;
        }

        /// <summary>
        /// Method to draw the icon on the subitem 
        /// </summary>
        /// <param name="e">The event arguments</param>
        /// <param name="x">The x coordinate </param>
        /// <param name="image">The image to be drawn</param>
        /// <returns>The x position after the drawing action</returns>
        private static int DrawSubItemIcon(DrawListViewSubItemEventArgs e, int x, Image image)
        {
            Bitmap myBitmap = new Bitmap(image);
            Icon myIcon = Icon.FromHandle(myBitmap.GetHicon());
            int y = e.Bounds.Y + ((e.Bounds.Height / 2) - (myIcon.Height / 2));
            e.Graphics.DrawIcon(myIcon, x, y);
            x += myIcon.Width + 2;
            return x;
        }

        /// <summary>
        /// Event handler for the DrawColumnHeader event of a ListView, 
        /// required to manage the property SmallIcon of the ListItems.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            ListView lView = (ListView)sender;
            ColumnHeader colH = null;
            Image img = null;

            if (lView.View == View.Details)
            {
                if (newProperties.ContainsKey(lView) && newProperties[lView].ContainsKey(newPropertiesEnum.InternalColumnHeaderImageListHelper))
                {
                    colH = e.Header;
                    ImageListHelper imgHelper = (ImageListHelper)newProperties[lView][newPropertiesEnum.InternalColumnHeaderImageListHelper];
                    if (!string.IsNullOrEmpty(colH.ImageKey))
                    {
                        if (imgHelper.NETImageList.Images.ContainsKey(colH.ImageKey))
                        {
                            img = imgHelper.NETImageList.Images[colH.ImageKey];
                            DrawColumnHeader(e, colH, img);
                        }
                        else
                            DrawColumnHeader(e, colH, null);
                    }
                    else if (colH.ImageIndex >= 0)
                    {
                        if (imgHelper.NETImageList.Images.Count > colH.ImageIndex)
                        {
                            img = imgHelper.NETImageList.Images[colH.ImageIndex];
                            DrawColumnHeader(e, colH, img);
                        }
                        else
                            DrawColumnHeader(e, colH, null);
                    }
                    else
                        e.DrawDefault = true;
                }
                else
                    e.DrawDefault = true;
            }
            else
                e.DrawDefault = true;

            InvokeEvents(lView, DrawColumnHeaderEventName, new object[] { sender, e });
        }

        /// <summary>
        /// Takes care of drawing a column header using an image.
        /// </summary>
        /// <param name="e">The DrawListViewColumnHeader event arguments.</param>
        /// <param name="colH">The ColumnHeader to be drawn.</param>
        /// <param name="img">The Image where to draw the column.</param>
        private static void DrawColumnHeader(DrawListViewColumnHeaderEventArgs e, ColumnHeader colH, Image img)
        {
            int width = TextRenderer.MeasureText(" ", e.Font).Width;
            string text = colH.Text;
            HorizontalAlignment textAlign = colH.TextAlign;
            TextFormatFlags flags = (textAlign == HorizontalAlignment.Left) ? TextFormatFlags.GlyphOverhangPadding : ((textAlign == HorizontalAlignment.Center) ? TextFormatFlags.HorizontalCenter : TextFormatFlags.Right);
            flags |= TextFormatFlags.WordEllipsis | TextFormatFlags.VerticalCenter;

            if (img != null)
            {
                int halfWidth = width / 2;

                Rectangle imgBounds = new Rectangle(e.Bounds.Location.X + halfWidth, e.Bounds.Location.Y + 1, e.Bounds.Size.Height, e.Bounds.Size.Height - 3);
                Rectangle txtBounds = new Rectangle(imgBounds.Location.X + imgBounds.Size.Width + halfWidth, e.Bounds.Location.Y, e.Bounds.Size.Width - (imgBounds.Size.Width + 2 * halfWidth), e.Bounds.Size.Height);

                e.DrawBackground();
                e.Graphics.DrawImage(img, imgBounds);
                TextRenderer.DrawText(e.Graphics, text, e.Font, txtBounds, e.ForeColor, flags);
            }
            else
            {
                e.DrawBackground();
                TextRenderer.DrawText(e.Graphics, text, e.Font, e.Bounds, e.ForeColor, flags);
            }
        }




        /// <summary>
        /// Class provided to patch some events that require to catch the messages from windows
        /// like DoubleClick event for a ListView.
        /// </summary>
        private class IMessageFilterImplementer : IMessageFilter
        {
            /// <summary>
            /// Catches the DoubleClick Windows Message.
            /// </summary>
            /// <param name="m">The Windows Message.</param>
            public bool PreFilterMessage(ref Message m)
            {
                try
                {
                    //DoubleClick message
                    if (m.Msg == 0x203)
                    {
                        foreach (ListView lView in ListViewHelper.EventsPatched.Keys)
                        {
                            if ((!lView.IsDisposed) && lView.Handle.Equals(m.HWnd))
                            {
                                //Fire the DoubleClick events
                                InvokeEvents(lView, DoubleClickEventName, new object[] { lView, new EventArgs() });
                                return false;
                            }
                        }
                    }
                }
                catch { }

                return false;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////// FUNCTIONS TO PATCH THE EVENTS ///////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Custom class to do the comparison of columns of a ListView.
        /// </summary>
        public class ListViewItemComparer : System.Collections.IComparer
        {
            /// <summary>
            /// Stores if ListView is sorted.
            /// </summary>
            private bool _Sorted = false;
            /// <summary>
            /// Indicates if ListView is sorted.
            /// </summary>
            public bool Sorted
            {
                get { return _Sorted; }
                set { _Sorted = value; }
            }

            /// <summary>
            /// Stores the SortKey value for the ListView.
            /// </summary>
            private int _SortKey = 0;
            /// <summary>
            /// Indicates the SortKey value for the ListView.
            /// </summary>
            public int SortKey
            {
                get { return _SortKey; }
                set { _SortKey = value; }
            }

            /// <summary>
            /// Stores the SortOrder value for the ListView.
            /// </summary>
            private SortOrder _SortOrder = SortOrder.Ascending;
            /// <summary>
            /// Indicates the SortOrder value for the ListView.
            /// </summary>
            public SortOrder SortOrder
            {
                get { return _SortOrder; }
                set { _SortOrder = value; }
            }

            /// <summary>
            /// Does the comparison between two ListView items.
            /// </summary>
            /// <param name="x">A ListView item to be compared.</param>
            /// <param name="y">A ListView item to be compared.</param>
            /// <returns>The result of the comparison.</returns>
            public int Compare(object x, object y)
            {
                if (_Sorted)
                {
                    if ((((ListViewItem)x).SubItems.Count > SortKey) && (((ListViewItem)y).SubItems.Count > SortKey))
                    {
                        if (_SortOrder == SortOrder.Ascending)
                            return String.Compare(((ListViewItem)x).SubItems[SortKey].Text, ((ListViewItem)y).SubItems[SortKey].Text);
                        else if (_SortOrder == SortOrder.Descending)
                            return String.Compare(((ListViewItem)y).SubItems[SortKey].Text, ((ListViewItem)x).SubItems[SortKey].Text);
                    }
                }

                return 0;
            }
        }
    }

    /// <summary>
    /// Class To Extend ListViewItems
    /// </summary>
    public static class ListViewExtensions
    {
        /// <summary>
        /// Get the actual selected item
        /// </summary>
        /// <param name="list">ListView to check on</param>
        /// <returns>ListViewItem selected</returns>
        public static ListViewItem SelectedItem(ListView list)
        {
            return list.SelectedItems.Count > 0 ? list.SelectedItems[0] : null;
        }
    }
}
