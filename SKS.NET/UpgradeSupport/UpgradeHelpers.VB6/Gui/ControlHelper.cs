using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using UpgradeHelpers.VB6.Utils;


namespace UpgradeHelpers.VB6.Gui
{

    /// <summary>
    /// Implements several contol-related functionalities which were present in VB6 and are not in .NET.
    /// </summary>
    public class ControlHelper
    {
        /// <summary>
        /// External API to Get Window Rect from user32.dll
        /// </summary>
        /// <param name="hWnd">handler pointer</param>
        /// <param name="rect">RECT structure output</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        protected internal static extern bool GetWindowRect(HandleRef hWnd, [In, Out] ref RECT rect);
        
        /// <summary>
        /// External API to Get Window from user32.dll
        /// </summary>
        /// <param name="hWnd">handler to get</param>
        /// <param name="uCmd">int cmd</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        protected internal static extern IntPtr GetWindow(HandleRef hWnd, int uCmd);

        /// <summary>
        /// External API to get if Is Window Visible from user32.dll
        /// </summary>
        /// <param name="hWnd">window handler</param>
        /// <returns>returs true if is visible</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        protected internal static extern bool IsWindowVisible(HandleRef hWnd);

        /// <summary>
        /// Internal RECT structure.
        /// </summary>
        protected internal struct RECT
        {
            /// <summary>
            /// left value
            /// </summary>
            public int left;
            /// <summary>
            /// top value
            /// </summary>
            public int top;
            /// <summary>
            /// right value
            /// </summary>
            public int right;
            /// <summary>
            /// bottom value
            /// </summary>
            public int bottom;
            /// <summary>
            /// Constructor, set the left,top,right,bottom values
            /// </summary>
            /// <param name="left">left position value</param>
            /// <param name="top">top position value</param>
            /// <param name="right">right position value</param>
            /// <param name="bottom">bottom position value</param>
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
            /// <summary>
            /// Constructor using the a Rectangle values
            /// </summary>
            /// <param name="r">Rectangle variable to get the position values</param>
            public RECT(Rectangle r)
            {
                this.left = r.Left;
                this.top = r.Top;
                this.right = r.Right;
                this.bottom = r.Bottom;
            }
            /// <summary>
            /// Returns a RECT structure from a x, y position and width, height values
            /// </summary>
            /// <param name="x">x position</param>
            /// <param name="y">y position</param>
            /// <param name="width">width value</param>
            /// <param name="height">height value</param>
            /// <returns></returns>
            public static RECT FromXYWH(int x, int y, int width, int height)
            {
                return new RECT(x, y, x + width, y + height);
            }
            /// <summary>
            /// Gets the Size structure from internal values
            /// </summary>
            public Size Size
            {
                get
                {
                    return new Size(this.right - this.left, this.bottom - this.top);
                }
            }
        }



        /// <summary>
        /// This hash has a map of control to ControlGraphics structures.
        /// </summary>
        private static WeakDictionary<Control, ControlGraphics> printHash = new WeakDictionary<Control, ControlGraphics>();

        /// <summary>
        /// Sets DrawWidth extended property.
        /// </summary>
        /// <param name="mControl">The control whose DrawWidth will be set.</param>
        /// <param name="val">The new DrawWidth value.</param>
        public static void setDrawWidth(Control mControl, int val)
        {
            ControlGraphics fg;
            fg = addToHash(mControl);
            fg.DrawWidth = val;
        }

        /// <summary>
        /// Sets DrawWidth extended property.
        /// </summary>
        /// <param name="mControl">The control whose DrawWidth will be set.</param>
        /// <param name="val">The new DrawWidth value.</param>
        public static void setDrawWidth(Control mControl, double val)
        {
            setDrawWidth(mControl, (int)val);
        }


        /// <summary>
        /// Obtains the DrawWidth value for a given control.
        /// </summary>
        /// <param name="mControl">The control whose DrawWidth value will be obtained.</param>
        /// <returns>The DrawWidth value for the given control.</returns>
        public static int getDrawWidth(Control mControl)
        {
            ControlGraphics fg;
            fg = addToHash(mControl);
            return fg.DrawWidth;
        }

        /// <summary>
        /// Sets CurrentX extended property.
        /// </summary>
        /// <param name="mControl">The control whose CurrentX will be set.</param>
        /// <param name="val">The new CurrentX value.</param>
        public static void setCurrentX(Control mControl, int val)
        {
            ControlGraphics fg;
            fg = addToHash(mControl);
            fg.CurrentX = val;
        }

        /// <summary>
        /// Sets CurrentX extended property.
        /// </summary>
        /// <param name="mControl">The control whose CurrentX will be set.</param>
        /// <param name="val">The new CurrentX value.</param>
        public static void setCurrentX(Control mControl, double val)
        {
            setCurrentX(mControl, (int)val);
        }

        /// <summary>
        /// Obtains the CurrentX value for a given control.
        /// </summary>
        /// <param name="mControl">The control whose CurrentX value will be obtained.</param>
        /// <returns>The CurrentX value for the given control.</returns>
        public static int getCurrentX(Control mControl)
        {
            ControlGraphics fg;
            fg = addToHash(mControl);
            return fg.CurrentX;
        }

        /// <summary>
        /// Sets CurrentY extended property.
        /// </summary>
        /// <param name="mControl">The control whose CurrentY will be set.</param>
        /// <param name="val">The new CurrentY value.</param>
        public static void setCurrentY(Control mControl, double val)
        {
            setCurrentY(mControl, (int)val);
        }

        /// <summary>
        /// Sets CurrentY extended property.
        /// </summary>
        /// <param name="mControl">The control whose CurrentY will be set.</param>
        /// <param name="val">The new CurrentY value.</param>
        public static void setCurrentY(Control mControl, int val)
        {
            ControlGraphics fg;
            fg = addToHash(mControl);
            fg.CurrentY = val;
        }

        /// <summary>
        /// Obtains the CurrentY value for a given control.
        /// </summary>
        /// <param name="mControl">The control whose CurrentY value will be obtained.</param>
        /// <returns>The CurrentY value for the given control.</returns>
        public static int getCurrentY(Control mControl)
        {
            ControlGraphics fg;
            fg = addToHash(mControl);
            return fg.CurrentY;
        }

        /// <summary>
        /// Prints the given parameters inside the specified control.
        /// </summary>
        /// <param name="mControl">The control to print in.</param>
        /// <param name="parameters">The elements to be printed.</param>
        public static void Print(Control mControl, params object[] parameters)
        {
            ControlGraphics fg;
            fg = addToHash(mControl);
            fg.Print(parameters);
        }

        /// <summary>
        /// Draws a line inside the given control with the specified parameters.
        /// </summary>
        /// <param name="mControl">The control to print in.</param>
        /// <param name="x1">The x value for the starting point.</param>
        /// <param name="y1">The y value for the starting point.</param>
        /// <param name="x2">The x value for the ending point.</param>
        /// <param name="y2">The y value for the ending point.</param>
        /// <param name="olecolor">The desired line color</param>
        public static void Line(Control mControl, int x1, int y1, int x2, int y2, int olecolor)
        {
            Line(mControl, x1, y1, x2, y2, ColorTranslator.FromOle(olecolor));
        }

        /// <summary>
        /// Draws a line inside the given control with the specified parameters.
        /// </summary>
        /// <param name="mControl">The control to print in.</param>
        /// <param name="x1">The x value for the starting point.</param>
        /// <param name="y1">The y value for the starting point.</param>
        /// <param name="x2">The x value for the ending point.</param>
        /// <param name="y2">The y value for the ending point.</param>
        /// <param name="olecolor">The desired line color.</param>
        public static void Line(Control mControl, double x1, double y1, double x2, double y2, int olecolor)
        {
            Line(mControl, (int)x1, (int)y1, (int)x2, (int)y2, ColorTranslator.FromOle(olecolor));
        }

        /// <summary>
        /// Draws a line inside the given control with the specified parameters.
        /// </summary>
        /// <param name="mControl">The control to print in.</param>
        /// <param name="x1">The x value for the starting point.</param>
        /// <param name="y1">The y value for the starting point.</param>
        /// <param name="x2">The x value for the ending point.</param>
        /// <param name="y2">The y value for the ending point.</param>
        /// <param name="color">The desired line color.</param>
        public static void Line(Control mControl, int x1, int y1, int x2, int y2, Color color)
        {
            ControlGraphics fg;
            fg = addToHash(mControl);
            fg.Line(x1, y1, x2, y2, color);
        }

        /// <summary>
        /// Draws a circle inside the given control with the specified parameters.
        /// </summary>
        /// <param name="mControl">The control to print in.</param>
        /// <param name="x">The x value for the center point.</param>
        /// <param name="y">The y value for the center point.</param>
        /// <param name="radius">The circle radius value.</param>
        /// <param name="olecolor">The desired circle color.</param>
        public static void Circle(Control mControl, int x, int y, double radius, int olecolor)
        {
            Circle(mControl, x, y, radius, ColorTranslator.FromOle(olecolor));
        }

        /// <summary>
        /// Draws a circle inside the given control with the specified parameters.
        /// </summary>
        /// <param name="mControl">The control to print in.</param>
        /// <param name="x">The x value for the center point.</param>
        /// <param name="y">The y value for the center point.</param>
        /// <param name="radius">The circle radius value.</param>
        /// <param name="olecolor">The desired circle color.</param>
        public static void Circle(Control mControl, double x, double y, double radius, int olecolor)
        {
            Circle(mControl, (int)x, (int)y, radius, olecolor);
        }

        /// <summary>
        /// Draws a circle inside the given control with the specified parameters.
        /// </summary>
        /// <param name="mControl">The control to print in.</param>
        /// <param name="x">The x value for the center point.</param>
        /// <param name="y">The y value for the center point.</param>
        /// <param name="radius">The circle radius value.</param>
        /// <param name="color">The desired circle color.</param>
        public static void Circle(Control mControl, int x, int y, double radius, Color color)
        {
            ControlGraphics fg;
            fg = addToHash(mControl);
            fg.Circle(x, y, radius, color);
        }

        /// <summary>
        /// Clears the graphics for the given control.
        /// </summary>
        /// <param name="mControl">The control to be cleared.</param>
        public static void Cls(Control mControl)
        {
            ControlGraphics fg;
            fg = addToHash(mControl);
            fg.Cls();
        }

        /// <summary>
        /// Function created to return the TextHeight of a control.
        /// Use this function for controls, when TextHeight applies to the print object use 
        /// PrinterHelper.TextHeight instead.
        /// </summary>
        /// <param name="con">The control.</param>
        /// <param name="str">The string to use in the calculus.</param>
        /// <returns>The text height required to print the str in the control.</returns>
        public static float TextHeight(Control con, string str)
        {
            System.Drawing.Graphics mesur = null;
            System.Drawing.SizeF size = new System.Drawing.SizeF();
            mesur = con.CreateGraphics();
            size = mesur.MeasureString(str, con.Font);
            return size.Height;
        }

        /// <summary>
        /// Function created to return the TextWidth of a control.
        /// Use this function for controls, when TextWidth applies to the print object use 
        /// PrinterHelper.TextWidth instead.
        /// </summary>
        /// <param name="con">The control.</param>
        /// <param name="str">The string to use in the calculus.</param>
        /// <returns>The text width required to print the str in the control.</returns>
        public static float TextWidth(Control con, string str)
        {
            System.Drawing.Graphics mesur = null;
            System.Drawing.SizeF size = new System.Drawing.SizeF();
            mesur = con.CreateGraphics();
            size = mesur.MeasureString(str, con.Font);
            return size.Width;
        }

        /// <summary>
        /// Support method to return the Enabled state of a control for special cases like 
        /// when a "ForEach control in Form.Control" is used.
        /// </summary>
        /// <param name="ctrl">The source control.</param>
        /// <returns>The state of the control.</returns>
        public static bool GetEnabled(Control ctrl)
        {
            if (ctrl is AxHost)
                return ((AxHost)ctrl).Enabled;

            if (ctrl is ContainerHelper.MenuItemControl)
                return ((ContainerHelper.MenuItemControl)ctrl).Enabled;

            //fsaborio. Correccion para invocar el metodo enable correcto (casos donde se sobreescribio como new Enabled)
            return Convert.ToBoolean(UpgradeHelpers.VB6.Utils.ReflectionHelper.GetMember(ctrl, "Enabled"));
        }

        /// <summary>
        /// Support method to set the Enabled state of a control for special cases like 
        /// when a "ForEach control in Form.Control" is used.
        /// </summary>
        /// <param name="ctrl">The source control.</param>
        /// <param name="value">set the bool value to Enabled property</param>
        /// <returns>The state of the control.</returns>
        public static void SetEnabled(Control ctrl, bool value)
        {
            if (ctrl is AxHost)
                ((AxHost)ctrl).Enabled = value;

            if (ctrl is ContainerHelper.MenuItemControl)
                ((ContainerHelper.MenuItemControl)ctrl).Enabled = value;

            //fsaborio. Correccion para invocar el metodo enable correcto (casos donde se sobreescribio como new Enabled)
            UpgradeHelpers.VB6.Utils.ReflectionHelper.SetMember(ctrl, "Enabled", value);
        }

        /// <summary>
        /// Support method to return the Visible state of a control for special cases like 
        /// when a "ForEach control in Form.Control" is used.
        /// </summary>
        /// <param name="ctrl">The source control.</param>
        /// <returns>The state of the control.</returns>
        public static bool GetVisible(Control ctrl)
        {
            if (ctrl is AxHost)
                return ((AxHost)ctrl).Visible;

            if (ctrl is ContainerHelper.MenuItemControl)
                return ((ContainerHelper.MenuItemControl)ctrl).Visible;

            return ctrl.Visible;
        }
        /// <summary>
        /// Support method to set the Visible state of a control for special cases like 
        /// when a "ForEach control in Form.Control" is used.
        /// </summary>
        /// <param name="ctrl">The source control.</param>
        /// <param name="value">set the Visible property to the control</param>
        /// <returns>The state of the control.</returns>
        public static void SetVisible(Control ctrl, bool value)
        {
            if (ctrl is AxHost)
                ((AxHost)ctrl).Visible = value;
            else if (ctrl is ContainerHelper.MenuItemControl)
                ((ContainerHelper.MenuItemControl)ctrl).Visible = value;
            else
                ctrl.Visible = value;
        }

        /// <summary>
        /// Support method to return the Tag state of a control for special cases like 
        /// when a "ForEach control in Form.Control" is used.
        /// </summary>
        /// <param name="ctrl">The source control.</param>
        /// <returns>The state of the control.</returns>
        public static string GetTag(Control ctrl)
        {
            if (ctrl is AxHost)
                return Convert.ToString(((AxHost)ctrl).Tag);

            if (ctrl is ContainerHelper.MenuItemControl)
                return Convert.ToString(((ContainerHelper.MenuItemControl)ctrl).Tag);

            return Convert.ToString(ctrl.Tag);
        }
        /// <summary>
        /// Support method to set the Tag state of a control for special cases like 
        /// when a "ForEach control in Form.Control" is used.
        /// </summary>
        /// <param name="ctrl">The source control.</param>
        /// <param name="value">set the Tag value to the control</param>
        /// <returns>The state of the control.</returns>
        public static void SetTag(Control ctrl, string value)
        {
            if (ctrl is AxHost)
                ((AxHost)ctrl).Tag = value;

            if (ctrl is ContainerHelper.MenuItemControl)
                ((ContainerHelper.MenuItemControl)ctrl).Tag = value;

            ctrl.Tag = value;
        }

        /// <summary>
        /// Returns true if the control is not completely visible given a window that 
        /// its partially or completely hiding it.
        /// </summary>
        /// <param name="ctrl">The source control.</param>
        /// <returns>True if the control is partially or completely hidden by a window.</returns>
        public static bool IsControlPartiallyObscured(Control ctrl)
        {
            Graphics g = ctrl.CreateGraphics();
            Region controlRegion = null;
            Region notObscuredControlRegion = null;

            GetVisibilityRegionsForControl(ctrl, out controlRegion, out notObscuredControlRegion);
            return !controlRegion.IsEmpty(g) && !controlRegion.Equals(notObscuredControlRegion, g);
        }

        /// <summary>
        /// Returns true if another window is completely hidding this control.
        /// </summary>
        /// <param name="ctrl">The source control.</param>
        /// <returns>True if the control is hidden by a window.</returns>
        public static bool IsControlObscured(Control ctrl)
        {
            Graphics g = ctrl.CreateGraphics();
            Region controlRegion = null;
            Region notObscuredControlRegion = null;

            GetVisibilityRegionsForControl(ctrl, out controlRegion, out notObscuredControlRegion);
            return notObscuredControlRegion.IsEmpty(g);
        }

        /// <summary>
        /// Given a control returns the region of the control, also it returs the region of the control 
        /// that is not obscured by another window.
        /// </summary>
        /// <param name="ctrl">The sources control.</param>
        /// <param name="controlRegion">The region of the control.</param>
        /// <param name="notObscuredControlRegion">The region not obscured by another windows.</param>
        private static void GetVisibilityRegionsForControl(Control ctrl, out Region controlRegion, out Region notObscuredControlRegion)
        {
            Control parentInternal = null;
            Control parentInternalParent = null;

            if (!ctrl.IsHandleCreated || !ctrl.Visible)
            {
                controlRegion = new Region();
                controlRegion.MakeEmpty();
                notObscuredControlRegion = new Region();
                notObscuredControlRegion.MakeEmpty();
                return;
            }

            RECT rect = new RECT();
            parentInternal = ctrl.GetType().GetProperty("ParentInternal", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(ctrl, null) as Control;
            if (parentInternal != null)
            {
                parentInternalParent = parentInternal.GetType().GetProperty("ParentInternal", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(parentInternal, null) as Control;
                while (parentInternalParent != null)
                {
                    parentInternal = parentInternalParent;
                    parentInternalParent = parentInternal.GetType().GetProperty("ParentInternal", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(parentInternal, null) as Control;
                }
            }
            GetWindowRect(new HandleRef(ctrl, ctrl.Handle), ref rect);
            controlRegion = new Region(Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom));
            notObscuredControlRegion = new Region(Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom));

            IntPtr ptr2;
            IntPtr handle;
            if (parentInternal != null)
                handle = parentInternal.Handle;
            else
                handle = ctrl.Handle;

            for (IntPtr ptr = handle; (ptr2 = GetWindow(new HandleRef(null, ptr), 3)) != IntPtr.Zero; ptr = ptr2)
            {
                GetWindowRect(new HandleRef(null, ptr2), ref rect);
                Rectangle rectangle = Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
                if (IsWindowVisible(new HandleRef(null, ptr2)))
                {
                    notObscuredControlRegion.Exclude(rectangle);
                }
            }
        }

        private static ControlGraphics addToHash(Control mControl)
        {
            ControlGraphics fg;
            if (printHash.ContainsKey(mControl))
            {
                fg = printHash[mControl];
            }
            else
            {
                fg = new ControlGraphics(mControl);
                mControl.Disposed += new EventHandler(fg.mControl_Disposed);
                printHash.Add(mControl, fg);
            }
            return fg;
        }

        /// <summary>
        /// To store temporarely removed events from controls.
        /// </summary>
        private static WeakDictionary<Control, Dictionary<string, List<Delegate>>> EventsDisabled = new WeakDictionary<Control, Dictionary<string, List<Delegate>>>();

        /// <summary>
        /// Remove the event handlers for a control (Disable).
        /// </summary>
        /// <param name="ctrl">The control.</param>
        /// <param name="eventName">The event name.</param>
        protected static internal void DisableControlEvents(Control ctrl, string eventName)
        {
            Delegate[] EventDelegates = ContainerHelper.GetEventSubscribers(ctrl, eventName);

            if (EventDelegates != null)
            {
                EventInfo eInfo = ctrl.GetType().GetEvent(eventName);
                if (eInfo != null)
                {
                    if (!EventsDisabled.ContainsKey(ctrl))
                        EventsDisabled.Add(ctrl, new Dictionary<string, List<Delegate>>());

                    if (!EventsDisabled[ctrl].ContainsKey(eventName))
                        EventsDisabled[ctrl].Add(eventName, new List<Delegate>());

                    foreach (Delegate del in EventDelegates)
                    {
                        EventsDisabled[ctrl][eventName].Add(del);
                        eInfo.RemoveEventHandler(ctrl, del);
                    }
                }
            }
        }

        /// <summary>
        /// Append the event handlers previously removed for a control (Enable).
        /// </summary>
        /// <param name="ctrl">The control.</param>
        /// <param name="eventName">The event name.</param>
        protected static internal void EnableControlEvents(Control ctrl, string eventName)
        {
            if ((EventsDisabled.ContainsKey(ctrl)) && (EventsDisabled[ctrl].ContainsKey(eventName)))
            {
                EventInfo eInfo = ctrl.GetType().GetEvent(eventName);
                if (eInfo != null)
                {
                    foreach (Delegate del in EventsDisabled[ctrl][eventName])
                    {
                        eInfo.AddEventHandler(ctrl, del);
                    }

                    EventsDisabled[ctrl].Remove(eventName);

                    if (EventsDisabled[ctrl].Count == 0)
                        EventsDisabled.Remove(ctrl);
                }
            }
        }

        /// <summary>
        /// Print, Cls, Line operations work using some values like
        /// CurrentX, CurrentY, and DrawWidth.
        /// For that reason this values must be keep associated with the control.
        /// </summary>
        private class ControlGraphics
        {
            Control control;

            public ControlGraphics(Control controlToDraw)
            {
                this.control = controlToDraw;
            }

            private int drawWidth = 1;
            public int DrawWidth
            {
                get
                {
                    return this.drawWidth;
                }
                set
                {
                    this.drawWidth = value;
                }
            }

            private int currentX = 0;
            public int CurrentX
            {
                get
                {
                    return this.currentX;
                }
                set
                {
                    this.currentX = value;
                }
            }

            private int currentY = 0;
            public int CurrentY
            {
                get
                {
                    return this.currentY;
                }
                set
                {
                    this.currentY = value;
                }
            }

            /// <summary>
            /// Clears the control from any previous drawings.
            /// </summary>
            public void Cls()
            {
                using (Graphics g = control.CreateGraphics())
                {
                    g.Clear(control.BackColor);
                    CurrentX = 0;
                    CurrentY = 0;
                }
            }

            /// <summary>
            /// Draws an image with its actual size.
            /// </summary>
            /// <param name="x">X coordinate position.</param>
            /// <param name="y">Y coordinate position.</param>
            /// <param name="filename">Filename for the image to draw.</param>
            public void DrawImage(int x, int y, string filename)
            {
                using (Graphics g = control.CreateGraphics())
                {
                    Bitmap imagen = new Bitmap(filename);
                    g.DrawImage(imagen, x, y);
                }
            }

            /// <summary>
            /// Draws an image with the specified size.
            /// </summary>
            /// <param name="x">X coordinate position.</param>
            /// <param name="y">Y coordinate position.</param>
            /// <param name="width">Width for the image.</param>
            /// <param name="height">Height for the image.</param>
            /// <param name="filename">Filename for the image to draw.</param>
            public void DrawImage(int x, int y, int width, int height, string filename)
            {
                using (Graphics g = control.CreateGraphics())
                {
                    Bitmap imagen = new Bitmap(filename);
                    g.DrawImage(imagen, x, y, width, height);
                }
            }

            /// <summary>
            /// Draws a circle with the specified color.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="radius"></param>
            /// <param name="color"></param>
            public void Circle(int x, int y, double radius, Color color)
            {
                Graphics g = control.CreateGraphics();
                SolidBrush brush = new SolidBrush(color);
                Pen pen = new Pen(brush);
                try
                {
                    radius = radius * 1.108;
                    x -= (int)radius / 2;
                    y -= (int)radius / 2;
                    g.DrawEllipse(pen, x, y, (float)radius, (float)radius);
                    CurrentX = x;
                    CurrentY = y;
                }
                finally
                {
                    g.Dispose();
                    brush.Dispose();
                    pen.Dispose();
                }
            }

            /// <summary>
            /// Draws a line with the specified points and color.
            /// </summary>
            /// <param name="x1"></param>
            /// <param name="y1"></param>
            /// <param name="x2"></param>
            /// <param name="y2"></param>
            /// <param name="color"></param>
            public void Line(int x1, int y1, int x2, int y2, Color color)
            {
                Graphics g = control.CreateGraphics();
                Pen pen = new Pen(color, DrawWidth);
                try
                {
                    g.DrawLine(pen, x1, y1, x2, y2);
                    CurrentX = x2;
                    CurrentY = y2;
                }
                finally
                {
                    g.Dispose();
                    pen.Dispose();
                }
            }

            /// <summary>
            /// Prints the specified parameters in the control.
            /// </summary>
            /// <param name="parameters"></param>
            public void Print(object[] parameters)
            {
                Graphics g = control.CreateGraphics();
                SolidBrush brush = new SolidBrush(control.ForeColor);
                try
                {
                    Font font = control.Font;
                    foreach (object o in parameters)
                    {
                        if (o == null)
                        {
                            //In VB6 this causes an exception
                            //TODO:  should we throw that same exception?
                        }
                        else
                            g.DrawString(o.ToString(), font, brush, CurrentX, CurrentY);

                        CurrentX += (((int)font.Size) * 12);
                    }
                    CurrentY += font.Height;
                    CurrentX = 0;
                }
                finally
                {
                    g.Dispose();
                    brush.Dispose();
                }
            }

            /// <summary>
            /// This is used to handle the dispose event of the associated control
            /// to make sure that the hash table is removed.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            internal void mControl_Disposed(object sender, EventArgs e)
            {
                Dispose();
            }

            public void Dispose()
            {
                printHash.Remove(control);
            }
        }
    }
}
