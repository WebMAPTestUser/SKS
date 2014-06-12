using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using UpgradeHelpers.VB6.Utils;

namespace UpgradeHelpers.VB6.Gui
{
    /// <summary>
    /// Class provided to add lost functionality to the TreeViews .NET.
    /// </summary>
    public class TreeViewHelper
    {
        /// <summary>
        /// Enum to handle the different properties and custom behaviors supplied by this Helper.
        /// </summary>
        private enum newPropertiesEnum
        {
            ExpandedImage = 0,
            NormalImage = 1
        }

        /// <summary>
        /// List of events to be corrected for this provider.
        /// </summary>
        private static Dictionary<string, Delegate> EventsToCorrect = new Dictionary<string, Delegate>();
        /// <summary>
        /// List of events to be patched for this provider.
        /// </summary>
        private static WeakDictionary<TreeView, Dictionary<String, List<Delegate>>> EventsPatched = new WeakDictionary<TreeView, Dictionary<string, List<Delegate>>>();
        /// <summary>
        /// List of properties and values that are supplied by this Helper.
        /// </summary>
        private static WeakDictionary<TreeView, Dictionary<newPropertiesEnum, object>> newProperties = new WeakDictionary<TreeView, Dictionary<newPropertiesEnum, object>>();

        private static readonly string AfterCollapseEventName = "AfterCollapse";
        private static readonly string AfterExpandEventName = "AfterExpand";

        /// <summary>
        /// Constructor.
        /// </summary>
        static TreeViewHelper()
        {
            //Initializes the list of events that should be patched
            EventsToCorrect.Add(AfterCollapseEventName, new TreeViewEventHandler(TreeView_AfterCollapse));
            EventsToCorrect.Add(AfterExpandEventName, new TreeViewEventHandler(TreeView_AfterExpand));
        }


        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////// STATIC PROPERTIES DEFINITION ////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the ExpandedImage property for a TreeNode (Key|Index).
        /// </summary>
        /// <param name="nItem">The TreeNode to get.</param>
        /// <returns>The current value.</returns>
        public static object GetTreeNodeExpandedImageProperty(TreeNode nItem)
        {
            Dictionary<TreeNode, object> treeNodeExpandedImageList = null;
            TreeView tView = nItem.TreeView;

            if (CheckForProperty(tView, newPropertiesEnum.ExpandedImage))
            {
                treeNodeExpandedImageList = (Dictionary<TreeNode, object>)newProperties[tView][newPropertiesEnum.ExpandedImage];
                if (treeNodeExpandedImageList.ContainsKey(nItem))
                    return treeNodeExpandedImageList[nItem];
                else
                    return -1;
            }
            else
                return -1;

        }

        /// <summary>
        /// Sets the ExpandedImage property for a TreeNode (Key|Index).
        /// </summary>
        /// <param name="nItem">The TreeNode to set.</param>
        /// <param name="value">The new value to set.</param>
        public static void SetTreeNodeExpandedImageProperty(TreeNode nItem, object value)
        {
            Dictionary<TreeNode, object> treeNodeExpandedImageList = null;
            TreeView tView = nItem.TreeView;

            if ((value is string) || (value is Int32))
            {
                if (CheckForProperty(tView, newPropertiesEnum.ExpandedImage))
                {
                    ValidateImageIndex(tView, value);

                    treeNodeExpandedImageList = (Dictionary<TreeNode, object>)newProperties[tView][newPropertiesEnum.ExpandedImage];

                    if (!treeNodeExpandedImageList.ContainsKey(nItem))
                        treeNodeExpandedImageList.Add(nItem, value);
                    else
                        treeNodeExpandedImageList[nItem] = value;

                    if (!EventsPatched.ContainsKey(tView))
                        CorrectEventsForTreeView(tView);

                    CheckExpandedImageForTreeNode(nItem);
                }

                return;
            }

            throw new InvalidCastException("Invalid property value");
        }

        /// <summary>
        /// Creates a drag image using the associated image to a treeNode. 
        /// This image is typically used in drag-and-drop operations
        /// </summary>
        /// <param name="tNode">The base node.</param>
        /// <returns>An image that can be used for Drag and Drop operations.</returns>
        public static Image CreateDragImage(TreeNode tNode)
        {
            Image res = UpgradeHelpers.VB6.Resources.UpgradeHelpers_VB6.DefaultDragImage.ToBitmap();
            if ((tNode.TreeView != null) && (tNode.TreeView.ImageList != null))
            {
                if (!string.IsNullOrEmpty(tNode.ImageKey) && tNode.TreeView.ImageList.Images.ContainsKey(tNode.ImageKey))
                    res = tNode.TreeView.ImageList.Images[tNode.ImageKey];

                if ((tNode.ImageIndex >= 0) && (tNode.ImageIndex < tNode.TreeView.ImageList.Images.Count))
                    res = tNode.TreeView.ImageList.Images[tNode.ImageIndex];
            }

            return res;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////// STATIC PROPERTIES DEFINITION ////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Check if the property 'newPropertiesEnum' is already defined for this TreeView.
        /// </summary>
        /// <param name="tView">The TreeView.</param>
        /// <param name="prop">The newPropertiesEnum to search.</param>
        private static bool CheckForProperty(TreeView tView, newPropertiesEnum prop)
        {
            if (tView == null)
                return false;

            CheckNewProperties(tView);
            if (!newProperties[tView].ContainsKey(prop))
                newProperties[tView][prop] = GetDefaultValueForProperty(prop);

            return true;
        }

        /// <summary>
        /// Checks if a TreeView is controlled by the newProperties Dictionary.
        /// </summary>
        /// <param name="tView">The TreeView.</param>
        private static void CheckNewProperties(TreeView tView)
        {
            if (!newProperties.ContainsKey(tView))
            {
                newProperties[tView] = new Dictionary<newPropertiesEnum, object>();
                tView.Disposed += new EventHandler(TreeView_Disposed);
            }
        }

        /// <summary>
        /// Event handler for the Disposed event of a TreeView so it can be cleaned it.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void TreeView_Disposed(object sender, EventArgs e)
        {
            TreeView tView = (TreeView)sender;
            if (newProperties.ContainsKey(tView))
                newProperties.Remove(tView);

            if (EventsPatched.ContainsKey(tView))
                EventsPatched.Remove(tView);
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
                case newPropertiesEnum.ExpandedImage:
                case newPropertiesEnum.NormalImage:
                    return new Dictionary<TreeNode, object>();
            }

            return null;
        }

        /// <summary>
        /// Patches the events for a specific treeview.
        /// </summary>
        /// <param name="tView">The TreeView.</param>
        private static void CorrectEventsForTreeView(TreeView tView)
        {
            Delegate[] EventDelegates = null;

            if (EventsPatched.ContainsKey(tView))
                throw new InvalidOperationException("Events for this TreeView has been previously patched: '" + tView.Name + "'");

            EventsPatched.Add(tView, new Dictionary<string, List<Delegate>>());
            foreach (string eventName in EventsToCorrect.Keys)
            {
                EventInfo eInfo = tView.GetType().GetEvent(eventName);
                if (eInfo == null)
                    throw new InvalidOperationException("Event info for event '" + eventName + "' could not be found");

                EventsPatched[tView].Add(eventName, new List<Delegate>());
                EventDelegates = ContainerHelper.GetEventSubscribers(tView, eventName);
                if (EventDelegates != null)
                {

                    foreach (Delegate del in EventDelegates)
                    {
                        EventsPatched[tView][eventName].Add(del);
                        eInfo.RemoveEventHandler(tView, del);
                    }
                }
                eInfo.AddEventHandler(tView, EventsToCorrect[eventName]);
            }
        }

        /// <summary>
        /// Event handler for the event AfterCollapse.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The TreeView event arguments.</param>
        private static void TreeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            TreeView source = (TreeView)sender;
            try
            {
                CheckExpandedImageForTreeNode(e.Node);

            }
            catch { }
            finally
            {
                try
                {
                    InvokeEvents(source, AfterCollapseEventName, new object[] { sender, e });
                }
                catch { }
            }
        }

        /// <summary>
        /// Event handler for the event AfterExpand.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The TreeView event arguments.</param>
        private static void TreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            TreeView source = (TreeView)sender;
            try
            {
                CheckExpandedImageForTreeNode(e.Node);

            }
            catch { }
            finally
            {
                try
                {
                    InvokeEvents(source, AfterExpandEventName, new object[] { sender, e });
                }
                catch { }
            }
        }

        /// <summary>
        /// Checks if the image for a treenode should be changed based on 
        /// if an ExpandedImage has been defined for the TreeNode.
        /// </summary>
        /// <param name="nItem">The TreeNode.</param>
        private static void CheckExpandedImageForTreeNode(TreeNode nItem)
        {
            Dictionary<TreeNode, object> treeNodeExpandedImageList = null;
            Dictionary<TreeNode, object> treeNodeNormalImageList = null;
            TreeView tView = nItem.TreeView;
            object expandedImageID = null;
            object normalImageID = null;

            if (CheckForProperty(tView, newPropertiesEnum.ExpandedImage))
            {
                treeNodeExpandedImageList = (Dictionary<TreeNode, object>)newProperties[tView][newPropertiesEnum.ExpandedImage];
                if (treeNodeExpandedImageList.ContainsKey(nItem))
                {
                    if (CheckForProperty(tView, newPropertiesEnum.NormalImage))
                    {
                        expandedImageID = treeNodeExpandedImageList[nItem];
                        ValidateImageIndex(tView, expandedImageID);

                        treeNodeNormalImageList = (Dictionary<TreeNode, object>)newProperties[tView][newPropertiesEnum.NormalImage];

                        if (nItem.IsExpanded)
                        {
                            if (!treeNodeNormalImageList.ContainsKey(nItem))
                            {
                                if (!string.IsNullOrEmpty(nItem.ImageKey))
                                    treeNodeNormalImageList.Add(nItem, nItem.ImageKey);
                                else
                                    treeNodeNormalImageList.Add(nItem, nItem.ImageIndex);

                            }

                            if (expandedImageID is string)
                            {
                                nItem.ImageIndex = nItem.SelectedImageIndex = -1;
                                nItem.ImageKey = nItem.SelectedImageKey = expandedImageID as string;
                            }
                            else
                            {
                                nItem.ImageKey = nItem.SelectedImageKey = string.Empty;
                                nItem.ImageIndex = nItem.SelectedImageIndex = Convert.ToInt32(expandedImageID);
                            }
                        }
                        else
                        {
                            if (treeNodeNormalImageList.ContainsKey(nItem))
                            {
                                normalImageID = treeNodeNormalImageList[nItem];
                                if (normalImageID is string)
                                {
                                    nItem.ImageIndex = nItem.StateImageIndex = -1;
                                    nItem.ImageKey = nItem.SelectedImageKey = normalImageID as string;
                                }
                                else
                                {
                                    nItem.ImageKey = nItem.SelectedImageKey = string.Empty;
                                    nItem.ImageIndex = nItem.StateImageIndex = Convert.ToInt32(normalImageID);
                                }

                                treeNodeNormalImageList.Remove(nItem);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates if the ImageIndex can be used with the images of the TreeView.
        /// </summary>
        /// <param name="tView">The TreeView source.</param>
        /// <param name="ImageID">The Image ID.</param>
        private static void ValidateImageIndex(TreeView tView, object ImageID)
        {
            if (!(ImageID is string) && !(ImageID is int))
                throw new InvalidCastException("Invalid type for an image index");

            if (tView.ImageList == null)
                throw new InvalidOperationException("ImageList must be initialized before it can be used");

            if (ImageID is string)
            {
                string ImageKey = ImageID as string;
                if (!tView.ImageList.Images.ContainsKey(ImageKey))
                    throw new KeyNotFoundException("Element not found");
            }

            if (ImageID is int)
            {
                int ImageIndex = Convert.ToInt32(ImageID);
                if ((ImageIndex < 0) || (ImageIndex >= tView.ImageList.Images.Count))
                    throw new IndexOutOfRangeException("Index out of bounds");
            }
        }

        /// <summary>
        /// Allows to invoke the patched events for a TreeView.
        /// </summary>
        /// <param name="source">The TreeView to invoke the event.</param>
        /// <param name="eventName">The event name to be invoked.</param>
        /// <param name="args">The arguments used to invoke the event.</param>
        private static void InvokeEvents(TreeView source, string eventName, object[] args)
        {
            if ((EventsPatched.ContainsKey(source)) && (EventsPatched[source].ContainsKey(eventName)))
            {
                foreach (Delegate del in EventsPatched[source][eventName])
                {
                    del.DynamicInvoke(args);
                }
            }
        }
    }
}
