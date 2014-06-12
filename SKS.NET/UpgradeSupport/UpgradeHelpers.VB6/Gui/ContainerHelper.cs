using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;
using UpgradeHelpers.VB6.Utils;

namespace UpgradeHelpers.VB6.Gui
{
    /// <summary>
    /// It is very common in VB6 to implement algorithms that tranverse the Controls collection
    /// for a Form or UserControl.
    /// During migration some issues appear in the target platform because the controls collection
    /// in .NET is not flat but hierarquical.
    /// This issue will break a lot of the original logic, that assumes that all controls present in the 
    /// Form or UserControl will be present in one big collection.
    /// This Helper Class provides several methods that allows to traverse the .NET components collections
    /// in an easy and direct way, just as it was possible in VB6.
    /// </summary>
    public class ContainerHelper
    {

        /// <summary>
        /// .NET does not implement the VB6 concept of controls arrays.
        /// During migration these arrays are generated as Arrays of Controls, but
        /// these arrays are not Controls. So they are not present in the Controls collections of a Form or UserControl.
        /// So to perform things like: <br></br>
        /// <code>
        /// Form1.Controls("MyTextBoxControlArray")
        /// </code>
        /// A class that will be able to "wrap" the array of controls and make it "behave" like a Control.
        /// and gives access to those elements.
        /// </summary>
        public class ControlArray : Control
        {
            /// <summary>
            /// A reference to the actual array.
            /// </summary>
            private Control[] controlArray = null;

            /// <summary>
            /// Builds a ControlArray object that will "wrap" the specified array, to make it be "seen" as a Control.
            /// </summary>
            /// <param name="controlArray">The control array that will be wrapped.</param>
            public ControlArray(Control[] controlArray)
            {
                this.controlArray = controlArray;
            }

            /// <summary>
            /// Builds a ControlArray object that will "wrap" the specified array, to make it be "seen" as a Control.
            /// This overload is used when the Control Array has a different type. 
            /// Supported array types are:
            /// * Control
            /// * ToolStripItem
            /// * MenuStrip
            /// </summary>
            /// <exception cref="System.InvalidCastException">Thrown when an array with unsupported type is used.</exception>
            /// <param name="ctrlArray">The control array that will be wrapped.</param>
            public ControlArray(Array ctrlArray)
            {
                MenuStrip mnuStrip = null;
                ToolStripItem toolStrip = null;
                object itemArray = 0;
                List<Control> lstControls = new List<Control>();
                Type elemType = ctrlArray.GetType().GetElementType();

                if (ctrlArray.Length == 0)
                {
                    this.controlArray = new Control[] { };
                }
                else
                {
                    itemArray = ctrlArray.GetValue(ctrlArray.GetLowerBound(0));

                    if (itemArray is Control)
                    {
                        this.controlArray = (Control[])ctrlArray;
                    }
                    else if (itemArray is ToolStripItem)
                    {
                        foreach (object item in ctrlArray)
                        {
                            toolStrip = item as ToolStripItem;
                            lstControls.Add(new MenuItemControl(toolStrip));
                        }
                        this.controlArray = lstControls.ToArray();
                    }
                    else if (itemArray is MenuStrip)
                    {
                        foreach (object item in ctrlArray)
                        {
                            mnuStrip = item as MenuStrip;
                            lstControls.Add(new MenuItemControl(mnuStrip));
                        }
                        this.controlArray = lstControls.ToArray();
                    }
                    else
                        throw new InvalidCastException("Invalid element type for control array: " + elemType.Name);
                }
            }

            /// <summary>
            /// Returns the Control at the specified index position.
            /// </summary>
            /// <exception cref="System.IndexOutOfRangeException"></exception>
            /// <param name="index"></param>
            /// <returns></returns>
            public Control this[int index]
            {
                get
                {
                    return controlArray[index];
                }
            }

            /// <summary>
            /// Returns the Control at the specified index position.
            /// </summary>
            /// <exception cref="System.IndexOutOfRangeException"></exception>
            /// <param name="index"></param>
            /// <returns></returns>
            public Control this[double index]
            {
                get
                {
                    return controlArray[(int)index];
                }
            }

            /// <summary>
            /// Returns the Length of the subyacent array.
            /// </summary>
            public int Length
            {
                get
                {
                    return controlArray.Length;
                }
            }

        }

        /// <summary>
        /// VB6 Menus are migrated to classes of the ToolStripItems objects.
        /// .NET menus are not Controls like in VB6.
        /// To iterate thru the collection of menu items we must "wrap" all the items.
        /// </summary>
        public class MenuItemControl : Control
        {
            /// <summary>
            /// Internal reference to the menuItems or mainMenu.
            /// </summary>
            private ToolStripItem menuItem = null;

            /// <summary>
            /// Returns the internal reference to the "wrapped" instance of a menuItem.
            /// </summary>
            public ToolStripItem ToolStripItemInstance
            {
                get
                {
                    return menuItem;
                }
            }

            /// <summary>
            /// Variable to hold the reference to main menu element.
            /// </summary>
            private MenuStrip mainMenu = null;

            /// <summary>
            /// Returns a reference to the MenuStrip that represent the main menu element.
            /// </summary>
            public MenuStrip MenuStrip
            {
                get
                {
                    return mainMenu;
                }
            }

            /// <summary>
            /// Overriding of casting operations.
            /// </summary>
            /// <param name="item"></param>
            /// <exception cref="System.Exception">Throw if the MenuStrip property is NULL.</exception>
            /// <returns></returns>
            public static explicit operator MenuStrip(MenuItemControl item)
            {
                if (item.mainMenu != null)
                    return item.mainMenu;
                throw new Exception("AIS-Exception. Item does not contains a reference to a MenuStrip type");
            }

            /// <summary>
            /// Implements a casting operator to unwrap the ToolStripItem.
            /// </summary>
            /// <exception cref="System.Exception">Throw if contained menuitem is null.</exception>
            /// <param name="item"></param>
            /// <returns></returns>
            public static explicit operator ToolStripItem(MenuItemControl item)
            {
                if (item.menuItem != null)
                    return item.menuItem;
                else
                    throw new Exception("AIS-Exception. Item does not contains a reference to a ToolStripItem type");
            }

            /// <summary>
            /// Implements a casting operator to wrap MenuStrip inside MenuItemControl instance.
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public static explicit operator MenuItemControl(MenuStrip item)
            {
                return new MenuItemControl(item);
            }

            /// <summary>
            /// Implements a casting operator to wrap ToolStripItem inside a MenuItemControl instance.
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public static explicit operator MenuItemControl(ToolStripItem item)
            {
                return new MenuItemControl(item);
            }


            /// <summary>
            /// Constructs a new instance wrapping a MenuStrip item inside of it.
            /// </summary>
            /// <param name="mainMenu">The MenuStrip item to wrap.</param>
            public MenuItemControl(MenuStrip mainMenu)
            {
                this.mainMenu = mainMenu;
                InitializeProperties();
                AddEventHandlers();
            }

            /// <summary>
            /// Constructs a new instance wrapping a ToolStripItem inside of it.
            /// </summary>
            /// <param name="menuItem">The ToolStripItem to wrap.</param>
            public MenuItemControl(ToolStripItem menuItem)
            {
                this.menuItem = menuItem;
                InitializeProperties();
                AddEventHandlers();
            }

            /// <summary>
            /// Returns true if this wrapper contains a reference that is not null.
            /// </summary>
            public bool IsToolStripItem
            {
                get
                {
                    return this.menuItem != null;
                }
            }

            /// <summary>
            /// Takes care of initialize some of the properties of the base control class with 
            /// the contained instance.
            /// </summary>
            private void InitializeProperties()
            {
                try { base.Enabled = Enabled; }
                catch { }
                try { base.Name = Name; }
                catch { }
                try { base.Tag = Tag; }
                catch { }
                try { base.Visible = Visible; }
                catch { }
            }

            /// <summary>
            /// Adds some eventHandlers of the base control class to track when some properties values 
            /// have been changed, specifically the Visible and Enable properties.
            /// </summary>
            private void AddEventHandlers()
            {
                base.VisibleChanged += new EventHandler(MenuItemControl_VisibleChanged);
                base.EnabledChanged += new EventHandler(MenuItemControl_EnabledChanged);
            }

            /// <summary>
            /// Event Handler to handle changes to the visible property.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void MenuItemControl_VisibleChanged(object sender, EventArgs e)
            {
                this.Visible = base.Enabled;
            }

            /// <summary>
            /// Event Handler to handle changes to the enable property.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void MenuItemControl_EnabledChanged(object sender, EventArgs e)
            {
                this.Enabled = base.Enabled;
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem AccessibilityObject.
            /// </summary>
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new AccessibleObject AccessibilityObject
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.AccessibilityObject;

                    if (this.menuItem != null)
                        return this.menuItem.AccessibilityObject;

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem AccessibleDefaultActionDescription.
            /// <seealso cref="System.Windows.Forms.Control.AccessibleDefaultActionDescription"/>
            /// </summary>
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new string AccessibleDefaultActionDescription
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.AccessibleDefaultActionDescription;

                    if (this.menuItem != null)
                        return this.menuItem.AccessibleDefaultActionDescription;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.AccessibleDefaultActionDescription = value;
                    else if (this.menuItem != null)
                        this.menuItem.AccessibleDefaultActionDescription = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem AccessibleDescription.
            /// <seealso cref="System.Windows.Forms.Control.AccessibleDescription"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new string AccessibleDescription
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.AccessibleDescription;

                    if (this.menuItem != null)
                        return this.menuItem.AccessibleDescription;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.AccessibleDescription = value;
                    else if (this.menuItem != null)
                        this.menuItem.AccessibleDescription = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem AccessibleName.
            /// <seealso cref="System.Windows.Forms.Control.AccessibleName"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new string AccessibleName
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.AccessibleName;

                    if (this.menuItem != null)
                        return this.menuItem.AccessibleName;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.AccessibleName = value;
                    else if (this.menuItem != null)
                        this.menuItem.AccessibleName = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem AccessibleRole.
            /// <seealso cref="System.Windows.Forms.Control.AccessibleRole"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new AccessibleRole AccessibleRole
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.AccessibleRole;

                    if (this.menuItem != null)
                        return this.menuItem.AccessibleRole;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.AccessibleRole = value;
                    else if (this.menuItem != null)
                        this.menuItem.AccessibleRole = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem AllowDrop.
            /// <seealso cref="System.Windows.Forms.Control.AllowDrop"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            public override bool AllowDrop
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.AllowDrop;

                    if (this.menuItem != null)
                        return this.menuItem.AllowDrop;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.AllowDrop = value;
                    else if (this.menuItem != null)
                        this.menuItem.AllowDrop = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Anchor.
            /// <seealso cref="System.Windows.Forms.Control.Anchor"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            public override AnchorStyles Anchor
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Anchor;

                    if (this.menuItem != null)
                        return this.menuItem.Anchor;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Anchor = value;
                    else if (this.menuItem != null)
                        this.menuItem.Anchor = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem AutoScrollOffset.
            /// <seealso cref="System.Windows.Forms.Control.AutoScrollOffset"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            public override Point AutoScrollOffset
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.AutoScrollOffset;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.AutoScrollOffset = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem AutoSize.
            /// <seealso cref="System.Windows.Forms.Control.AutoSize"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            public override bool AutoSize
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.AutoSize;

                    if (this.menuItem != null)
                        return this.menuItem.AutoSize;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.AutoSize = value;
                    else if (this.menuItem != null)
                        this.menuItem.AutoSize = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem BackColor.
            /// <seealso cref="System.Windows.Forms.Control.BackColor"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            public override Color BackColor
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.BackColor;

                    if (this.menuItem != null)
                        return this.menuItem.BackColor;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.BackColor = value;
                    else if (this.menuItem != null)
                        this.menuItem.BackColor = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem BackgroundImage.
            /// <seealso cref="System.Windows.Forms.Control.BackgroundImage"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            public override Image BackgroundImage
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.BackgroundImage;

                    if (this.menuItem != null)
                        return this.menuItem.BackgroundImage;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.BackgroundImage = value;
                    else if (this.menuItem != null)
                        this.menuItem.BackgroundImage = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem BackgroundImageLayout.
            /// <seealso cref="System.Windows.Forms.Control.BackgroundImageLayout"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            public override ImageLayout BackgroundImageLayout
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.BackgroundImageLayout;

                    if (this.menuItem != null)
                        return this.menuItem.BackgroundImageLayout;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.BackgroundImageLayout = value;
                    else if (this.menuItem != null)
                        this.menuItem.BackgroundImageLayout = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem BindingContext.
            /// <seealso cref="System.Windows.Forms.Control.BindingContext"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            public override BindingContext BindingContext
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.BindingContext;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.BindingContext = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Bottom.
            /// <seealso cref="System.Windows.Forms.Control.Bottom"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new int Bottom
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Bottom;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Bounds.
            /// <seealso cref="System.Windows.Forms.Control.Bounds"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Rectangle Bounds
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Bounds;

                    if (this.menuItem != null)
                        return this.menuItem.Bounds;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Bounds = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem CanFocus.
            /// <seealso cref="System.Windows.Forms.Control.CanFocus"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool CanFocus
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.CanFocus;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem CanSelect.
            /// <seealso cref="System.Windows.Forms.Control.CanSelect"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool CanSelect
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.CanSelect;

                    if (this.menuItem != null)
                        return this.menuItem.CanSelect;

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Capture.
            /// <seealso cref="System.Windows.Forms.Control.Capture"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Capture
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Capture;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Capture = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem CausesValidation.
            /// <seealso cref="System.Windows.Forms.Control.CausesValidation"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool CausesValidation
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.CausesValidation;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.CausesValidation = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem CheckForIllegalCrossThreadCalls.
            /// <seealso cref="System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static bool CheckForIllegalCrossThreadCalls
            {
                get
                {
                    return Control.CheckForIllegalCrossThreadCalls;
                }
                set
                {
                    Control.CheckForIllegalCrossThreadCalls = true;
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem ClientRectangle.
            /// <seealso cref="System.Windows.Forms.Control.ClientRectangle"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Rectangle ClientRectangle
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.ClientRectangle;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem ClientSize.
            /// <seealso cref="System.Windows.Forms.Control.ClientSize"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Size ClientSize
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.ClientSize;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.ClientSize = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip CompanyName property.
            /// <seealso cref="System.Windows.Forms.Control.CompanyName"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new string CompanyName
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.CompanyName;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip ContainsFocus property.
            /// <seealso cref="System.Windows.Forms.Control.ContainsFocus"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool ContainsFocus
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.ContainsFocus;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip ContextMenu property.
            /// <seealso cref="System.Windows.Forms.Control.ContextMenu"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            public override ContextMenu ContextMenu
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.ContextMenu;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.ContextMenu = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip ContextMenuStrip property.
            /// <seealso cref="System.Windows.Forms.Control.ContextMenuStrip"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            public override ContextMenuStrip ContextMenuStrip
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.ContextMenuStrip;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.ContextMenuStrip = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Returns a flat collection of Controls for a MenuStrip or a ToolStripItem or 
            /// null if the internal object is not set.
            /// </summary>
            public new ControlCollection Controls
            {
                get
                {
                    if (this.mainMenu != null)
                        return new MenuItemsCollection(this.mainMenu);

                    if (this.menuItem != null)
                        return new MenuItemsCollection(this.menuItem);

                    return null;
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip Created property.
            /// <seealso cref="System.Windows.Forms.Control.ContextMenuStrip"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Created
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Created;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip Cursor property.
            /// <seealso cref="System.Windows.Forms.Control.ContextMenuStrip"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            public override Cursor Cursor
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Cursor;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Cursor = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip DataBindings property.
            /// <seealso cref="System.Windows.Forms.Control.DataBindings"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new ControlBindingsCollection DataBindings
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.DataBindings;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Returns the DefaultBackColor for Controls.
            /// <seealso cref="System.Windows.Forms.Control.DefaultBackColor"/>
            /// </summary>            
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static Color DefaultBackColor
            {
                get
                {
                    return Control.DefaultBackColor;
                }
            }

            /// <summary>
            /// Returns the DefaultBackColor for Controls.
            /// <seealso cref="System.Windows.Forms.Control.DefaultFont"/>
            /// </summary>            
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static Font DefaultFont
            {
                get
                {
                    return Control.DefaultFont;
                }
            }

            /// <summary>
            /// Returns the DefaultBackColor for Controls.
            /// <seealso cref="System.Windows.Forms.Control.DefaultForeColor"/>
            /// </summary>            
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static Color DefaultForeColor
            {
                get
                {
                    return Control.DefaultForeColor;
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip DisplayRectangle property.
            /// <seealso cref="System.Windows.Forms.Control.DisplayRectangle"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            public override Rectangle DisplayRectangle
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.DisplayRectangle;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip Disposing property.
            /// <seealso cref="System.Windows.Forms.Control.Disposing"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Disposing
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Disposing;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Dock property.
            /// <seealso cref="System.Windows.Forms.Control.Dock"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            public override DockStyle Dock
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Dock;

                    if (this.menuItem != null)
                        return this.menuItem.Dock;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Dock = value;
                    else if (this.menuItem != null)
                        this.menuItem.Dock = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Enabled property.
            /// <seealso cref="System.Windows.Forms.Control.Dock"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Enabled
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Enabled;

                    if (this.menuItem != null)
                        return this.menuItem.Enabled;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Enabled = value;
                    else if (this.menuItem != null)
                        this.menuItem.Enabled = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Focused property.
            /// <seealso cref="System.Windows.Forms.Control.Focused"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object does not support 
            /// the property or it is not set.</exception>
            public override bool Focused
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Focused;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Font property.
            /// <seealso cref="System.Windows.Forms.Control.Font"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            public override Font Font
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Font;

                    if (this.menuItem != null)
                        return this.menuItem.Font;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Font = value;
                    else if (this.menuItem != null)
                        this.menuItem.Font = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem ForeColor property.
            /// <seealso cref="System.Windows.Forms.Control.ForeColor"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            public override Color ForeColor
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.ForeColor;

                    if (this.menuItem != null)
                        return this.menuItem.ForeColor;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.ForeColor = value;
                    else if (this.menuItem != null)
                        this.menuItem.ForeColor = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gets Window Handle
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new IntPtr Handle
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Handle;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <exception cref="System.Exception">Throws an exception indicating that it is not supported.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool HasChildren
            {
                get
                {
                    throw new Exception("AIS-Exception, Object does not support the property");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Height property.
            /// <seealso cref="System.Windows.Forms.Control.Height"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new int Height
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Height;

                    if (this.menuItem != null)
                        return this.menuItem.Height;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Height = value;
                    else if (this.menuItem != null)
                        this.menuItem.Height = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip ImeMode property.
            /// <seealso cref="System.Windows.Forms.Control.ImeMode"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new ImeMode ImeMode
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.ImeMode;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.ImeMode = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip InvokeRequired property.
            /// <seealso cref="System.Windows.Forms.Control.InvokeRequired"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool InvokeRequired
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.InvokeRequired;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip IsAccessible property.
            /// <seealso cref="System.Windows.Forms.Control.IsAccessible"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool IsAccessible
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.IsAccessible;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.IsAccessible = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem IsDisposed property.
            /// <seealso cref="System.Windows.Forms.Control.IsDisposed"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool IsDisposed
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.IsDisposed;

                    if (this.menuItem != null)
                        return this.menuItem.IsDisposed;

                    throw new Exception("AIS-Exception, Object not set");
                }
            }



            /// <summary>
            /// Gives access to the internal MenuStrip IsHandleCreated property.
            /// <seealso cref="System.Windows.Forms.Control.Height"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool IsHandleCreated
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.IsHandleCreated;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip IsMirrored property.
            /// <seealso cref="System.Windows.Forms.Control.Height"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>            
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool IsMirrored
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.IsMirrored;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip LayoutEngine property.
            /// <seealso cref="System.Windows.Forms.Control.Height"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>            
            public override LayoutEngine LayoutEngine
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.LayoutEngine;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip Left property.
            /// <seealso cref="System.Windows.Forms.Control.Left"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new int Left
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Left;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Left = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip Location property.
            /// <seealso cref="System.Windows.Forms.Control.Location"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Point Location
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Location;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Location = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Margin property.
            /// <seealso cref="System.Windows.Forms.Control.Margin"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Padding Margin
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Margin;

                    if (this.menuItem != null)
                        return this.menuItem.Margin;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Margin = value;
                    else if (this.menuItem != null)
                        this.menuItem.Margin = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip MaximunSize property.
            /// <seealso cref="System.Windows.Forms.Control.MaximumSize"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            public override Size MaximumSize
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.MaximumSize;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.MaximumSize = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip MinimumSize property.
            /// <seealso cref="System.Windows.Forms.Control.MinimumSize"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>

            public override Size MinimumSize
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.MinimumSize;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.MinimumSize = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gets the value for modifier key (Ctrl, Shift and Alt)
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static Keys ModifierKeys
            {
                get
                {
                    return Control.ModifierKeys;
                }
            }

            /// <summary>
            /// Gets which Mouse button is pressed
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static MouseButtons MouseButtons
            {
                get
                {
                    return Control.MouseButtons;
                }
            }

            /// <summary>
            /// Gets the Point position of the mouse
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static Point MousePosition
            {
                get
                {
                    return Control.MousePosition;
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Name property.
            /// <seealso cref="System.Windows.Forms.Control.Name"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new string Name
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Name;

                    if (this.menuItem != null)
                        return this.menuItem.Name;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Name = value;
                    else if (this.menuItem != null)
                        this.menuItem.Name = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Padding property.
            /// <seealso cref="System.Windows.Forms.Control.Padding"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Padding Padding
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Padding;

                    if (this.menuItem != null)
                        return this.menuItem.Padding;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Padding = value;
                    else if (this.menuItem != null)
                        this.menuItem.Padding = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip Parent property.
            /// <seealso cref="System.Windows.Forms.Control.Parent"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Control Parent
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Parent;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Parent = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip PreferredSize property.
            /// <seealso cref="System.Windows.Forms.Control.PreferredSize"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Size PreferredSize
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.PreferredSize;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip ProductName property.
            /// <seealso cref="System.Windows.Forms.Control.ProductName"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new string ProductName
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.ProductName;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip ProductVersion property.
            /// <seealso cref="System.Windows.Forms.Control.ProductVersion"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new string ProductVersion
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.ProductVersion;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip RecreatingHandle property.
            /// <seealso cref="System.Windows.Forms.Control.RecreatingHandle"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool RecreatingHandle
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.RecreatingHandle;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip Region property.
            /// <seealso cref="System.Windows.Forms.Control.Region"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Region Region
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Region;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Region = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip Right property.
            /// <seealso cref="System.Windows.Forms.Control.Right"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new int Right
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Right;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem RightToLeft property.
            /// <seealso cref="System.Windows.Forms.Control.RightToLeft"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            public override RightToLeft RightToLeft
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.RightToLeft;

                    if (this.menuItem != null)
                        return this.menuItem.RightToLeft;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.RightToLeft = value;
                    else if (this.menuItem != null)
                        this.menuItem.RightToLeft = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Site property.
            /// <seealso cref="System.Windows.Forms.Control.Site"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            public override ISite Site
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Site;

                    if (this.menuItem != null)
                        return this.menuItem.Site;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Site = value;
                    else if (this.menuItem != null)
                        this.menuItem.Site = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Size property.
            /// <seealso cref="System.Windows.Forms.Control.Size"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Size Size
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Size;

                    if (this.menuItem != null)
                        return this.menuItem.Size;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Size = value;
                    else if (this.menuItem != null)
                        this.menuItem.Size = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip TabIndex property.
            /// <seealso cref="System.Windows.Forms.Control.TabIndex"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new int TabIndex
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.TabIndex;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.TabIndex = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip TabStop property.
            /// <seealso cref="System.Windows.Forms.Control.TabStop"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool TabStop
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.TabStop;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.TabStop = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Tag property.
            /// <seealso cref="System.Windows.Forms.Control.Tag"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new object Tag
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Tag;

                    if (this.menuItem != null)
                        return this.menuItem.Tag;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Tag = value;
                    else if (this.menuItem != null)
                        this.menuItem.Tag = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Text property.
            /// <seealso cref="System.Windows.Forms.Control.Text"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            public override string Text
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Text;

                    if (this.menuItem != null)
                        return this.menuItem.Text;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Text = value;
                    else if (this.menuItem != null)
                        this.menuItem.Text = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip Top property.
            /// <seealso cref="System.Windows.Forms.Control.Top"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new int Top
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Top;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Top = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip TopLevelControl property.
            /// <seealso cref="System.Windows.Forms.Control.TopLevelControl"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Control TopLevelControl
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.TopLevelControl;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip UseWaitCursor property.
            /// <seealso cref="System.Windows.Forms.Control.Left"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set or 
            /// if the internal object does not support the property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool UseWaitCursor
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.UseWaitCursor;

                    if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.UseWaitCursor = value;
                    else if (this.menuItem != null)
                        throw new Exception("AIS-Exception, Object does not support the property");
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }


            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Visible property.
            /// <seealso cref="System.Windows.Forms.Control.Visible"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Visible
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Visible;

                    if (this.menuItem != null)
                        return this.menuItem.Available;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Visible = value;
                    else if (this.menuItem != null)
                        this.menuItem.Available = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Gives access to the internal MenuStrip or ToolStripItem Width property.
            /// <seealso cref="System.Windows.Forms.Control.Width"/>
            /// </summary>            
            /// <exception cref="System.Exception">Thrown if the internal object is not set.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new int Width
            {
                get
                {
                    if (this.mainMenu != null)
                        return this.mainMenu.Width;

                    if (this.menuItem != null)
                        return this.menuItem.Width;

                    throw new Exception("AIS-Exception, Object not set");
                }
                set
                {
                    if (this.mainMenu != null)
                        this.mainMenu.Width = value;
                    else if (this.menuItem != null)
                        this.menuItem.Width = value;
                    else
                        throw new Exception("AIS-Exception, Object not set");
                }
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <exception cref="System.Exception">Throws an exception indicating that 
            /// it is not a supported property.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new IWindowTarget WindowTarget
            {
                get
                {
                    throw new Exception("AIS-Exception, Object does not support the property");
                }
                set
                {
                    throw new Exception("AIS-Exception, Object does not support the property");
                }
            }


            /// <summary>
            /// sets the Begin Invoke method
            /// </summary>
            /// <param name="method">pointer to method to call</param>
            /// <returns></returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new IAsyncResult BeginInvoke(Delegate method)
            {
                if (this.mainMenu != null)
                    return this.mainMenu.BeginInvoke(method);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }
            /// <summary>
            /// sets the Begin Invoke method
            /// </summary>
            /// <param name="method">pointer to method to call</param>
            /// <param name="args">array of parameters to use</param>
            /// <returns></returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new IAsyncResult BeginInvoke(Delegate method, params object[] args)
            {
                if (this.mainMenu != null)
                    return this.mainMenu.BeginInvoke(method, args);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// If the internal object is a MenuStrip, it calls the BringToFront method.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void BringToFront()
            {
                if (this.mainMenu != null)
                    this.mainMenu.BringToFront();

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Not suppported.
            /// </summary>
            /// <param name="ctl"></param>
            /// <returns></returns>
            /// <exception cref="System.Exception">Throws an exception indicating that it is not supported.</exception>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Contains(Control ctl)
            {
                throw new Exception("AIS-Exception, Object does not support the method");
            }

            /// <summary>
            /// Creates Control
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void CreateControl()
            {
                if (this.mainMenu != null)
                    this.mainMenu.CreateControl();

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Creates Graphics
            /// </summary>
            /// <returns></returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Graphics CreateGraphics()
            {
                if (this.mainMenu != null)
                    return this.mainMenu.CreateGraphics();

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Do Drag and Drop use data object and effects
            /// </summary>
            /// <param name="data">data object</param>
            /// <param name="allowedEffects">DragDropEffects enum</param>
            /// <returns>Exception in case is not set the main menu or menu item</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects)
            {
                if (this.mainMenu != null)
                    return this.mainMenu.DoDragDrop(data, allowedEffects);

                if (this.menuItem != null)
                    return this.menuItem.DoDragDrop(data, allowedEffects);

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Draws a bitmap in the Rectagle target position
            /// </summary>
            /// <param name="bitmap">pointer to bitmap</param>
            /// <param name="targetBounds">Rectangle position values</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
            {
                if (this.mainMenu != null)
                    this.mainMenu.DrawToBitmap(bitmap, targetBounds);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Call End Invoke method
            /// </summary>
            /// <param name="asyncResult">use the IAsyncResult parameter</param>
            /// <returns>returns object</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new object EndInvoke(IAsyncResult asyncResult)
            {
                if (this.mainMenu != null)
                    return this.mainMenu.EndInvoke(asyncResult);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Finds the Form
            /// </summary>
            /// <returns>Form found</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Form FindForm()
            {
                if (this.mainMenu != null)
                    return this.mainMenu.FindForm();

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Sets the Focus
            /// </summary>
            /// <returns></returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Focus()
            {
                if (this.mainMenu != null)
                    return this.mainMenu.Focus();

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Retrieves the control that has the specific handle
            /// </summary>
            /// <param name="handle">The window handle (HWND) to search for</param>
            /// <returns>Control</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static Control FromChildHandle(IntPtr handle)
            {
                return Control.FromChildHandle(handle);
            }

            /// <summary>
            /// Returns the control that is associated to the specified handle
            /// </summary>
            /// <param name="handle">The window handle (HWND) to search for</param>
            /// <returns>Control</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static Control FromHandle(IntPtr handle)
            {
                return Control.FromHandle(handle);
            }
            /// <summary>
            /// It's not supported.
            /// </summary>
            /// <param name="pt"></param>
            /// <returns></returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Control GetChildAtPoint(Point pt)
            {
                throw new Exception("AIS-Exception, Object does not support the method");
            }

            /// <summary>
            /// It's not supported
            /// </summary>
            /// <param name="pt"></param>
            /// <param name="skipValue"></param>
            /// <returns></returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue)
            {
                throw new Exception("AIS-Exception, Object does not support the method");
            }

            /// <summary>
            /// Gets Parent control
            /// </summary>
            /// <returns>Control</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new IContainerControl GetContainerControl()
            {
                if (this.mainMenu != null)
                    return this.mainMenu.GetContainerControl();

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Gets Next or Back Control in the tab order
            /// </summary>
            /// <param name="ctl">Control to start the search</param>
            /// <param name="forward">Next or Back</param>
            /// <returns></returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Control GetNextControl(Control ctl, bool forward)
            {
                if (this.mainMenu != null)
                    return this.mainMenu.GetNextControl(ctl, forward);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Retrieves the size of a rectangular area into which a control can be fitted
            /// </summary>
            /// <param name="proposedSize">custom size area</param>
            /// <returns>Returns size used</returns>
            public override Size GetPreferredSize(Size proposedSize)
            {
                if (this.mainMenu != null)
                    return this.mainMenu.GetPreferredSize(proposedSize);

                if (this.menuItem != null)
                    return this.menuItem.GetPreferredSize(proposedSize);

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Hides the control
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Hide()
            {
                if (this.mainMenu != null)
                    this.mainMenu.Hide();

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Invalidates the specified region of the control
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Invalidate()
            {
                if (this.mainMenu != null)
                    this.mainMenu.Invalidate();

                if (this.menuItem != null)
                    this.menuItem.Invalidate();

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Invalidates the specified region of the control
            /// </summary>
            /// <param name="invalidateChildren">bool</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Invalidate(bool invalidateChildren)
            {
                if (this.mainMenu != null)
                    this.mainMenu.Invalidate(invalidateChildren);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }
            /// <summary>
            /// Invalidates the specified region of the control
            /// </summary>
            /// <param name="rc">Use Rectangle area</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Invalidate(Rectangle rc)
            {
                if (this.mainMenu != null)
                    this.mainMenu.Invalidate(rc);

                if (this.menuItem != null)
                    this.menuItem.Invalidate(rc);

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Invalidates the specified region of the control
            /// </summary>
            /// <param name="region">use the Region area</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Invalidate(Region region)
            {
                if (this.mainMenu != null)
                    this.mainMenu.Invalidate(region);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Invalidates the specified region of the control
            /// </summary>
            /// <param name="rc">use the Rectangle Area</param>
            /// <param name="invalidateChildren">invalidates children too?</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Invalidate(Rectangle rc, bool invalidateChildren)
            {
                if (this.mainMenu != null)
                    this.mainMenu.Invalidate(rc, invalidateChildren);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Invalidates the specified region of the control
            /// </summary>
            /// <param name="region">use the Region area</param>
            /// <param name="invalidateChildren">invalidates children too?</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Invalidate(Region region, bool invalidateChildren)
            {
                if (this.mainMenu != null)
                    this.mainMenu.Invalidate(region, invalidateChildren);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Execute the specified delegate method
            /// </summary>
            /// <param name="method">a delegate method to call in the control context</param>
            /// <returns></returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new object Invoke(Delegate method)
            {
                if (this.mainMenu != null)
                    return this.mainMenu.Invoke(method);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Execute the specified method with the parameters
            /// </summary>
            /// <param name="method">a delegate method to call in the control context</param>
            /// <param name="args">use the array of arguments</param>
            /// <returns></returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new object Invoke(Delegate method, params object[] args)
            {
                if (this.mainMenu != null)
                    return this.mainMenu.Invoke(method, args);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="keyVal">Determines whether the CAPS LOCK, NUM LOCK or SCROLL LOCK key is in effect</param>
            /// <returns>true if is in effect</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static bool IsKeyLocked(Keys keyVal)
            {
                return Control.IsKeyLocked(keyVal);
            }

            /// <summary>
            /// Is mnemonic the charcode for the control in the specified text
            /// </summary>
            /// <param name="charCode">char code to look up</param>
            /// <param name="text">specified text</param>
            /// <returns>true if is mnemonic</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new static bool IsMnemonic(char charCode, string text)
            {
                return Control.IsMnemonic(charCode, text);
            }

            /// <summary>
            /// Force to perform layout logic and it's children too.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void PerformLayout()
            {
                if (this.mainMenu != null)
                    this.mainMenu.PerformLayout();

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Force to perform layout logic and it's children too.
            /// </summary>
            /// <param name="affectedControl">Control recently changed</param>
            /// <param name="affectedProperty">Name of the control</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void PerformLayout(Control affectedControl, string affectedProperty)
            {
                if (this.mainMenu != null)
                    this.mainMenu.PerformLayout(affectedControl, affectedProperty);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Computes the location of the specified screen
            /// </summary>
            /// <param name="p">Screen coordinate to convert</param>
            /// <returns>the new Point</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Point PointToClient(Point p)
            {
                if (this.mainMenu != null)
                    return this.mainMenu.PointToClient(p);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Computes the location of the specified screen
            /// </summary>
            /// <param name="p">Point to convert</param>
            /// <returns>new Point</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Point PointToScreen(Point p)
            {
                if (this.mainMenu != null)
                    return this.mainMenu.PointToScreen(p);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Preprocess the keyboard or input messages
            /// </summary>
            /// <param name="msg">string message to process</param>
            /// <returns></returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new PreProcessControlState PreProcessControlMessage(ref Message msg)
            {
                if (this.mainMenu != null)
                    return this.mainMenu.PreProcessControlMessage(ref msg);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Preprocess the keyboard or input messages
            /// </summary>
            /// <param name="msg">Message to process</param>
            /// <returns></returns>
            public override bool PreProcessMessage(ref Message msg)
            {
                if (this.mainMenu != null)
                    return this.mainMenu.PreProcessMessage(ref msg);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the property");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Computes the Rectagle to Client of the specific location
            /// </summary>
            /// <param name="r">Rectangle location to convert</param>
            /// <returns>Converted Rectangle</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Rectangle RectangleToClient(Rectangle r)
            {
                if (this.mainMenu != null)
                    return this.mainMenu.RectangleToClient(r);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Computes the Rectagle to Client of the specific location
            /// </summary>
            /// <param name="r">Rectangle area to convert</param>
            /// <returns>Resulted Rectangle </returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Rectangle RectangleToScreen(Rectangle r)
            {
                if (this.mainMenu != null)
                    return this.mainMenu.RectangleToScreen(r);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Invalidates and Redraw the control
            /// </summary>
            public override void Refresh()
            {
                if (this.mainMenu != null)
                    this.mainMenu.Refresh();

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the property");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// It's not supported
            /// </summary>
            public override void ResetBackColor()
            {
                throw new Exception("AIS-Exception, Object does not support the property");
            }

            /// <summary>
            /// It's not supported
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void ResetBindings()
            {
                throw new Exception("AIS-Exception, Object does not support the method");
            }

            /// <summary>
            /// Is not supported
            /// </summary>
            public override void ResetCursor()
            {
                throw new Exception("AIS-Exception, Object does not support the property");
            }

            /// <summary>
            /// It's not supported
            /// </summary>
            public override void ResetFont()
            {
                throw new Exception("AIS-Exception, Object does not support the property");
            }

            /// <summary>
            /// Is not supported
            /// </summary>
            public override void ResetForeColor()
            {
                throw new Exception("AIS-Exception, Object does not support the property");
            }

            /// <summary>
            /// Is not supported
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void ResetImeMode()
            {
                throw new Exception("AIS-Exception, Object does not support the method");
            }

            /// <summary>
            /// It's not supported
            /// </summary>
            public override void ResetRightToLeft()
            {
                throw new Exception("AIS-Exception, Object does not support the property");
            }

            /// <summary>
            /// Sets default text to Text property
            /// </summary>
            public override void ResetText()
            {
                if (this.mainMenu != null)
                    this.mainMenu.ResetText();

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the property");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Resume usual layout logic
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void ResumeLayout()
            {
                if (this.mainMenu != null)
                    this.mainMenu.ResumeLayout();

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Resume usual layout logic and performs pending request
            /// </summary>
            /// <param name="performLayout"></param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void ResumeLayout(bool performLayout)
            {
                if (this.mainMenu != null)
                    this.mainMenu.ResumeLayout(performLayout);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Scale the controls to ratio
            /// </summary>
            /// <param name="ratio"></param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Scale(float ratio)
            {
                if (this.mainMenu != null)
                    this.mainMenu.Scale(ratio);
                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Scale the controls to size factor
            /// </summary>
            /// <param name="factor"></param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Scale(SizeF factor)
            {
                if (this.mainMenu != null)
                    this.mainMenu.Scale(factor);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Scale the controls to dx and dy size
            /// </summary>
            /// <param name="dx">x size</param>
            /// <param name="dy">y size</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Scale(float dx, float dy)
            {
                if (this.mainMenu != null)
                    this.mainMenu.Scale(dx, dy);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Activate the control
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Select()
            {
                if (this.mainMenu != null)
                    this.mainMenu.Select();

                if (this.menuItem != null)
                    this.menuItem.Select();

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Activates the next control
            /// </summary>
            /// <param name="ctl">the starting control to search</param>
            /// <param name="forward">is Forward or Backward?</param>
            /// <param name="tabStopOnly">use the Tab Stop?</param>
            /// <param name="nested">search in children?</param>
            /// <param name="wrap">go to first control and continue search?</param>
            /// <returns></returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool SelectNextControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap)
            {
                if (this.mainMenu != null)
                    return this.mainMenu.SelectNextControl(ctl, forward, tabStopOnly, nested, wrap);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Send the control back to zorder
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void SendToBack()
            {
                if (this.mainMenu != null)
                    this.mainMenu.SendToBack();

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Specific bounds for location and size
            /// </summary>
            /// <param name="x">x position</param>
            /// <param name="y">y position</param>
            /// <param name="width">width size</param>
            /// <param name="height">height size</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void SetBounds(int x, int y, int width, int height)
            {
                if (this.mainMenu != null)
                    this.mainMenu.SetBounds(x, y, width, height);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="x">x position</param>
            /// <param name="y">y position</param>
            /// <param name="width">width size</param>
            /// <param name="height">height size</param>
            /// <param name="specified">Bounds specified, do a bitwise between specified and parameters</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void SetBounds(int x, int y, int width, int height, BoundsSpecified specified)
            {
                if (this.mainMenu != null)
                    this.mainMenu.SetBounds(x, y, width, height, specified);

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Displays the control to user
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Show()
            {
                if (this.mainMenu != null)
                    this.mainMenu.Show();

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Temporary suspend the layout logic to user
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void SuspendLayout()
            {
                if (this.mainMenu != null)
                    this.mainMenu.SuspendLayout();

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

            /// <summary>
            /// Redraw the control to the invalidated areas
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void Update()
            {
                if (this.mainMenu != null)
                    this.mainMenu.Update();

                if (this.menuItem != null)
                    throw new Exception("AIS-Exception, Object does not support the method");

                throw new Exception("AIS-Exception, Object not set");
            }

        }

        /// <summary>
        /// Menu Items Collection
        /// </summary>
        public class MenuItemsCollection : System.Windows.Forms.Control.ControlCollection, IEnumerator<Control>, IEnumerable<Control>, IDisposable
        {
            private IEnumerator _controlEnumerator = null;

            /// <summary>
            /// Constructor Menu Items Collection
            /// </summary>
            /// <param name="owner">Use the menu system for a form</param>
            public MenuItemsCollection(MenuStrip owner)
                : base(owner)
            {
                this._controlEnumerator = owner.Items.GetEnumerator();
            }

            /// <summary>
            /// Constructor Menu Items Collection
            /// </summary>
            /// <param name="owner">adds the ToolstripItem</param>
            public MenuItemsCollection(ToolStripItem owner)
                : base(new MenuItemControl(owner))
            {
                if (owner is ToolStripMenuItem)
                    this._controlEnumerator = ((ToolStripMenuItem)owner).DropDownItems.GetEnumerator();
            }

            /// <summary>
            /// Overwriting for inherited members ControlCollection
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Control Owner
            {
                get
                {
                    throw new Exception("AIS-Exception, Implementation of member not supported");
                }
            }

            /// <summary>
            /// Array access using an index
            /// </summary>
            /// <param name="index">index position</param>
            /// <returns>Control at index position</returns>
            public override Control this[int index]
            {
                get
                {
                    Control result = null;
                    if (base.Owner is MenuStrip)
                        result = new MenuItemControl(((MenuStrip)base.Owner).Items[index]);

                    if (base.Owner is MenuItemControl)
                        result = ((MenuItemControl)base.Owner).Controls[index];

                    return result;
                }
            }

            /// <summary>
            /// Array access using key string
            /// </summary>
            /// <param name="key">string name of the control</param>
            /// <returns>Control indexed with key name</returns>
            public override Control this[string key]
            {
                get
                {
                    Control result = null;
                    if (base.Owner is MenuStrip)
                        result = new MenuItemControl(((MenuStrip)base.Owner).Items[key]);

                    if (base.Owner is MenuItemControl)
                        result = ((MenuItemControl)base.Owner).Controls[key];

                    return result;
                }
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="value"></param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override void Add(Control value)
            {
                throw new Exception("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="controls"></param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override void AddRange(Control[] controls)
            {
                throw new Exception("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override void Clear()
            {
                throw new Exception("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Contains(Control control)
            {
                throw new Exception("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override bool ContainsKey(string key)
            {
                throw new Exception("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="searchAllChildren"></param>
            /// <returns></returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Control[] Find(string key, bool searchAllChildren)
            {
                throw new Exception("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="child"></param>
            /// <returns></returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new int GetChildIndex(Control child)
            {
                throw new Exception("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="child"></param>
            /// <param name="throwException"></param>
            /// <returns></returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override int GetChildIndex(Control child, bool throwException)
            {
                throw new Exception("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Returns the instance collection
            /// </summary>
            /// <returns></returns>
            IEnumerator<Control> IEnumerable<Control>.GetEnumerator()
            {
                return this;
            }

            /// <summary>
            /// Retruns the Enumerator instance.
            /// </summary>
            /// <returns></returns>
            public override IEnumerator GetEnumerator()
            {
                return this;
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="control"></param>
            /// <returns></returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new int IndexOf(Control control)
            {
                throw new Exception("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override int IndexOfKey(string key)
            {
                throw new Exception("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// It's Not supported.
            /// </summary>
            /// <param name="value"></param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override void Remove(Control value)
            {
                throw new Exception("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// It's Not supported.
            /// </summary>
            /// <param name="index"></param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new void RemoveAt(int index)
            {
                throw new Exception("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// It's Not supported.
            /// </summary>
            /// <param name="key"></param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override void RemoveByKey(string key)
            {
                throw new Exception("AIS-Exception, Implementation of member not supported");
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            /// <param name="child"></param>
            /// <param name="newIndex"></param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override void SetChildIndex(Control child, int newIndex)
            {
                throw new Exception("AIS-Exception, Implementation of member not supported");
            }


            /// <summary>
            /// Current element in the collection as a control
            /// </summary>
            Control IEnumerator<Control>.Current
            {
                get
                {
                    if (_controlEnumerator != null)
                    {
                        if (_controlEnumerator.Current != null)
                            return new MenuItemControl((ToolStripItem)_controlEnumerator.Current);
                    }
                    return null;
                }
            }

            /// <summary>
            /// Current element in the collection as an object
            /// </summary>
            public object Current
            {
                get
                {
                    if (_controlEnumerator != null)
                    {
                        if (_controlEnumerator.Current != null)
                            return new MenuItemControl((ToolStripItem)_controlEnumerator.Current);
                    }
                    return null;
                }
            }

            /// <summary>
            /// Finalizer
            /// </summary>
            ~MenuItemsCollection()
            {
                Dispose(false);
            }

            /// <summary>
            /// Disposes the intance
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private bool disposed;

            /// <summary>
            /// Disposes the instance
            /// </summary>
            /// <param name="disposing"></param>
            protected virtual void Dispose(bool disposing)
            {
                if (!disposed)
                {
                    if (disposing)
                    {
                        //No managed resources to dispose
                    }
                }
                //No unmanaged resources to dispose
                disposed = true;
            }

            /// <summary>
            /// Advance to next control in the collection
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                bool hasNext = false;

                if (_controlEnumerator != null)
                    hasNext = _controlEnumerator.MoveNext();

                return hasNext;
            }

            /// <summary>
            /// internal enumerator is set to empty
            /// </summary>
            public void Reset()
            {
                _controlEnumerator = null;
            }
        }
        /// <summary>
        /// Returns the NestedControlEnumerator for the control
        /// </summary>
        /// <param name="control">used to get the NestedControlEnumerator</param>
        /// <returns></returns>
        public static NestedControlEnumerator Controls(Control control)
        {
            return new NestedControlEnumerator(control);
        }

        /// <summary>
        /// A structure to store the list of events for an object.
        /// </summary>
        private static WeakDictionary<object, Dictionary<string, Delegate[]>> EventSubscribersCache = new WeakDictionary<object, Dictionary<string, Delegate[]>>();

        /// <summary>
        /// Gets the delegates bound to an event in an object.
        /// </summary>
        /// <param name="Target">The object.</param>
        /// <param name="EventName">The event name.</param>
        /// <returns>Null if no delegates or event were found.</returns>
        protected internal static Delegate[] GetEventSubscribers(object Target, string EventName)
        {
            if ((EventSubscribersCache.ContainsKey(Target)) && (EventSubscribersCache[Target].ContainsKey(EventName)))
                return EventSubscribersCache[Target][EventName];

            Delegate del = null;
            string[] WinFormsEventsName = new string[] { "Event" + EventName, "Event_" + EventName
                , "EVENT" + EventName.ToUpper(), "EVENT_" + EventName.ToUpper()};
            Type TargetType = Target.GetType();
            FieldInfo fInfo = null;

            while (TargetType != null)
            {
                //Look for a field in the Target with the name of the event
                fInfo = TargetType.GetField(EventName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                if (fInfo != null)
                {
                    //Gets the current value in the Target instance
                    del = (Delegate)fInfo.GetValue(Target);
                    if (del != null)
                    {
                        AddListOfEventsToChache(Target, EventName, del.GetInvocationList());
                        return EventSubscribersCache[Target][EventName];
                    }
                }
                else
                {
                    foreach (string winEventName in WinFormsEventsName)
                    {
                        //Look for a field in the Target with the name of the event as defined in some cases
                        fInfo = TargetType.GetField(winEventName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                        if (fInfo != null)
                        {
                            EventHandlerList eHandlerList = (EventHandlerList)Target.GetType().GetProperty("Events", (BindingFlags.FlattenHierarchy | (BindingFlags.NonPublic | BindingFlags.Instance))).GetValue(Target, null);

                            del = eHandlerList[fInfo.GetValue(Target)];
                            if (del != null)
                            {
                                AddListOfEventsToChache(Target, EventName, del.GetInvocationList());
                                return EventSubscribersCache[Target][EventName];
                            }
                        }
                    }
                }

                //Repeats the process in the base types if nothing has been found so far
                TargetType = TargetType.BaseType;
            }

            AddListOfEventsToChache(Target, EventName, null);
            return null;
        }

        /// <summary>
        /// Method to add a list of events to the cache.
        /// </summary>
        /// <param name="target">The object target to use as key.</param>
        /// <param name="EventName">The name of the event.</param>
        /// <param name="delList">The list of event handlers.</param>
        private static void AddListOfEventsToChache(object target, string EventName, Delegate[] delList)
        {
            if (!EventSubscribersCache.ContainsKey(target))
            {
                EventSubscribersCache.Add(target, new Dictionary<string, Delegate[]>());
                Component cmp = target as Component;
                if (cmp != null)
                    cmp.Disposed += new EventHandler(Component_Disposed);
            }

            if (!EventSubscribersCache[target].ContainsKey(EventName))
                EventSubscribersCache[target].Add(EventName, delList);
            else
                EventSubscribersCache[target][EventName] = delList;
        }

        /// <summary>
        /// Event handler release resources when a component is disposed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Component_Disposed(object sender, EventArgs e)
        {
            try
            {
                if (EventSubscribersCache.ContainsKey(sender))
                    EventSubscribersCache.Remove(sender);
            }
            catch { }
        }

        ///<summary>
        /// This method is obsolete, use ControlArrayHelper instead.
        ///</summary>
        ///<param name="controlObject"></param>
        ///<returns></returns>
        /// <see cref="ControlArrayHelper"/>
        [Obsolete]
        public static int GetControlIndex(Object controlObject)
        {
            return ControlArrayHelper.GetControlIndex(controlObject);
        }

        ///<summary>
        /// This method is obsolete, use ControlArrayHelper instead.
        ///</summary>
        ///<param name="formContainer"></param>
        ///<param name="controlName"></param>
        ///<param name="index"></param>
        /// <see cref="ControlArrayHelper"/>
        [Obsolete]
        public static void LoadControl(Form formContainer, String controlName, int index)
        {
            ControlArrayHelper.LoadControl(formContainer, controlName, index);
        }

        ///<summary>
        /// This method is obsolete, use ControlArrayHelper instead.
        ///</summary>
        ///<param name="objectToUnload"></param>
        /// <see cref="ControlArrayHelper"/>
        [Obsolete]
        public static void UnloadControl(Object objectToUnload)
        {
            ControlArrayHelper.UnloadControl(objectToUnload);
        }

        ///<summary>
        /// This method is obsolete, use ControlArrayHelper instead.
        ///</summary>
        ///<param name="formContainer"></param>
        ///<param name="controlName"></param>
        ///<param name="index"></param>
        /// <see cref="ControlArrayHelper"/>
        [Obsolete]
        public static void UnloadControl(Form formContainer, String controlName, int index)
        {
            ControlArrayHelper.UnloadControl(formContainer, controlName, index);
        }
    }

    /// <summary>
    /// To flatten the .NET controls collection and expose all controls in a 1-dimensional array, this 
    /// IEnumerator implementation is provided that enumerates the controls contained
    /// by the given control and all their children too.
    /// </summary>
    public class NestedControlEnumerator : IEnumerator<Control>, IEnumerable<Control>
    {

        /// <summary>
        /// Fields to use with the IEnumerator.
        /// </summary>
        private NestedControlEnumerator currentNestedEnumerator = null;
        private IEnumerator controlEnumerator = null;
        private Boolean currentIsValid = false;

        /// <summary>
        /// Reference to the control at was used to create this enumerator.
        /// </summary>
        private Control control;
        private bool mustDisposeControl = false;


        /// <summary>
        /// Creates an enumerator to transverse thru the control and all its children components.
        /// </summary>
        /// <param name="control">The root component to start the iteration.</param>
        public NestedControlEnumerator(Control control)
        {
            this.control = control;
            this.controlEnumerator = control.Controls.GetEnumerator();
        }

        /// <summary>
        /// Creates an enumerator to transverse thru a MenuStrip and all its children components.
        /// </summary>
        /// <param name="menu">The menu strip component where the control enumeration will start.</param>
        public NestedControlEnumerator(MenuStrip menu)
        {
            mustDisposeControl = true;
            this.control = new ContainerHelper.MenuItemControl(menu);
            this.controlEnumerator = ((ContainerHelper.MenuItemControl)this.control).Controls.GetEnumerator();
        }

        /// <summary>
        /// Creates an enumerator to transverse thru a ToolStripItem and all its children components.
        /// </summary>
        /// <param name="menuItem">The ToolStripItem component where the control enumeration will start.</param>
        public NestedControlEnumerator(ToolStripItem menuItem)
        {
            mustDisposeControl = true;
            this.control = new ContainerHelper.MenuItemControl(menuItem);
            this.controlEnumerator = ((ContainerHelper.MenuItemControl)this.control).Controls.GetEnumerator();
        }

        #region IEnumerator, IEnumerable Implementation
        /// <summary>
        /// Properties and methods related with IEnumerator and IEnumerable.
        /// </summary>
        Control IEnumerator<Control>.Current
        {
            get
            {
                if (currentNestedEnumerator != null)
                    return (Control)currentNestedEnumerator.Current;
                else
                    return (Control)controlEnumerator.Current;
            }
        }

        /// <summary>
        /// Returns the current control in the enumeration.
        /// </summary>
        public object Current
        {
            get
            {
                if (currentNestedEnumerator != null)
                    return (Control)currentNestedEnumerator.Current;
                else
                    return (Control)controlEnumerator.Current;
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~NestedControlEnumerator()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the intance
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed;
        /// <summary>
        /// Disposes the instance
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (currentNestedEnumerator != null)
                        currentNestedEnumerator.Dispose();
                    if (mustDisposeControl && control != null)
                        control.Dispose();
                }
            }
            //No unmanaged resources to dispose
            disposed = true;
        }

        /// <summary>
        /// Move to next Control in the collection.
        /// </summary>
        /// <returns>False if it is at the end of the collection, True otherwise.</returns>
        public bool MoveNext()
        {
            bool hasNext = false;
            if (currentNestedEnumerator != null)
            {
                if (!(hasNext = currentNestedEnumerator.MoveNext()))
                {
                    currentNestedEnumerator.Dispose();
                    currentNestedEnumerator = null;
                }
            }
            else if (currentIsValid)
            {
                if (controlEnumerator.Current is GroupBox || controlEnumerator.Current is Panel || controlEnumerator.Current is TabControl
                    || controlEnumerator.Current is MenuStrip || controlEnumerator.Current is PictureBox)
                {
                    currentNestedEnumerator =
                        new NestedControlEnumerator(((Control)controlEnumerator.Current));
                    hasNext = MoveNext();
                }
                else if (currentIsValid && (controlEnumerator.Current is ContainerHelper.MenuItemControl)
                         && (((ContainerHelper.MenuItemControl)controlEnumerator.Current).IsToolStripItem))
                {
                    currentNestedEnumerator = new NestedControlEnumerator((ToolStripItem)((ContainerHelper.MenuItemControl)controlEnumerator.Current));
                    hasNext = MoveNext();
                }
            }



            if (!hasNext)
                hasNext = controlEnumerator.MoveNext();

            if (hasNext && (Current is MdiClient))
                hasNext = false;

            currentIsValid = hasNext;

            //TabPages must be ommited but the constrols inside not
            if (hasNext && (Current is TabPage))
                hasNext = MoveNext();


            return hasNext;
        }

        /// <summary>
        /// Clears all internal structures, reset the enumerator to the initial state.
        /// </summary>
        public void Reset()
        {
            if (currentNestedEnumerator != null)
                currentNestedEnumerator.Dispose();
            currentNestedEnumerator = null;
            controlEnumerator.Reset();
            currentIsValid = false;
        }

        /// <summary>
        /// Generics implementation to return an IEnumerator for Control.
        /// </summary>
        /// <returns></returns>
        IEnumerator<Control> IEnumerable<Control>.GetEnumerator()
        {
            return this;
        }

        /// <summary>
        /// Provides an IEnumerator implementation.
        /// </summary>
        /// <returns>A collection reference that can be use to enumerate.</returns>
        public IEnumerator GetEnumerator()
        {
            return this;
        }


        /// <summary>
        /// Returns the control in the collection with the specified name.
        /// </summary>
        public Control this[String name]
        {
            get
            {
                Control result = control.Controls[name];
                if (result == null)
                {
                    IDictionary<string, Control> nc = GetNestedControls();
                    if (nc.ContainsKey(name))
                        result = nc[name];
                    if (result == null)
                    {
                        //We need to look for a control array
                        Type type = control.GetType();
                        FieldInfo finfo = type.GetField(name);
                        if (finfo != null)
                        {
                            object field_value = finfo.GetValue(control);
                            if (field_value is Array)
                            {
                                return new Gui.ContainerHelper.ControlArray((Array)field_value);
                            }
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Returns the control in the collection at the specified index.
        /// </summary>
        public Control this[int index]
        {
            get
            {
                IList<Control> nc = GetIndexedNestedControls();
                return nc[index];
            }
        }

        /// <summary>
        /// Returns the number of Controls in the colletion.
        /// </summary>
        public int Count
        {
            get
            {
                return GetIndexedNestedControls().Count;
            }
        }

        /// <summary>
        /// Removes the Control element at the specified position.
        /// </summary>
        /// <param name="i"></param>
        public void RemoveAt(int i)
        {
            if (currentNestedEnumerator != null)
            {
                currentNestedEnumerator.RemoveAt(i);
            }
            else
            {
                control.Controls.RemoveAt(i);
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A HashTable mapping controls name to the control reference.</returns>
        private IDictionary<string, Control> GetNestedControls()
        {
            IDictionary<String, Control> nc = new SortedDictionary<String, Control>();
            this.Reset();
            foreach (Control ctl in this)
                nc[ctl.Name] = ctl;

            return nc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Provides a list of all Controls in the collection.</returns>
        private IList<Control> GetIndexedNestedControls()
        {
            IList<Control> nc = new List<Control>();
            WeakReference wr = new WeakReference(nc);
            this.Reset();
            foreach (Control ctl in this)
                nc.Add(ctl);

            return nc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Provides a list of all Controls in the collection.</returns>
        public IList GetControls()
        {
            IList nc = new List<Control>();
            WeakReference wr = new WeakReference(nc);
            this.Reset();
            foreach (Control ctl in this)
                nc.Add(ctl);

            return nc;
        }
    }
}
