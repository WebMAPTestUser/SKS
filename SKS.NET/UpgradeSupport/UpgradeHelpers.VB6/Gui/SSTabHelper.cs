using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace UpgradeHelpers.VB6.Gui
{
    /// <summary>
    /// Class created to add support to functionality lost in TabControls.
    /// </summary>
    [ProvideProperty("UseMnemonic", typeof(TabControl))]
    [ProvideProperty("ActiveTabFontStyle", typeof(TabControl))]
    public partial class SSTabHelper : Component, ISupportInitialize, IExtenderProvider
    {
        /// <summary>
        /// Enum values for the custom property ActiveTabFont.
        /// </summary>
        public enum ActiveTabFontStyleEnum
        {
            /// <summary>
            /// Default
            /// </summary>
            Default,
            /// <summary>
            /// Regular
            /// </summary>
            Regular,
            /// <summary>
            /// Italic
            /// </summary>
            Italic,
            /// <summary>
            /// Bold
            /// </summary>
            Bold,
            /// <summary>
            /// Strikeout
            /// </summary>
            Strikeout,
            /// <summary>
            /// Underline
            /// </summary>
            Underline,
            /// <summary>
            /// BoldItalic
            /// </summary>
            Bold_Italic
        }

        /// <summary>
        /// Enum to handle the different properties and custom behaviors supplied by this Helper.
        /// </summary>
        private enum newPropertiesEnum
        {
            ActiveFontStyle = 1,
            UseMnemonic = 2,
            TabEnabled = 3
        }

        /// <summary>
        /// Only the list of disabled tabs will be contained here.
        /// </summary>
        private static readonly IDictionary<int, IList<int>> TabsDisabled = new Dictionary<int, IList<int>>();

        /// <summary>
        /// Stores the visible status of each of the tabs of a tabcontrol.
        /// </summary>
        private static readonly IDictionary<int, List<KeyValuePair<TabPage, bool>>> TabsVisible = new Dictionary<int, List<KeyValuePair<TabPage, bool>>>();
        private static readonly IDictionary<int, Dictionary<newPropertiesEnum, object>> newProperties = new Dictionary<int, Dictionary<newPropertiesEnum, object>>();

        /// <summary>
        /// Controls when a tabControl should have its UseMnemonic property set.
        /// </summary>
        private static readonly IDictionary<int, List<TabControl>> FormsWithTabsControlsUsingMnemonic = new Dictionary<int, List<TabControl>>();

        /// <summary>
        /// Controls which tabControls are drawing its text using custom drawing mode.
        /// </summary>
        private static readonly IDictionary<int, List<newPropertiesEnum>> ControlOfCustomDrawingMode = new Dictionary<int, List<newPropertiesEnum>>();

        /// <summary>
        /// Delays the set of the UseMnemonic property after the control has been properly initialized.
        /// </summary>
        private static readonly List<TabControl> DelayedSetUseMnemonic = new List<TabControl>();
        /// <summary>
        /// Indicates if EndInit hasn't been executed yet after a BeginInit.
        /// </summary>
        private bool OnInitialization;

        /// <summary>
        /// Determinates which controls can use these extra properties.
        /// </summary>
        /// <param name="extender">The object to test.</param>
        /// <returns>True if the object can extend the properties.</returns>
        public bool CanExtend(object extender)
        {
            return (extender is TabControl);
        }

        /// <summary>
        /// Method BeginInit to implement inherited from ISupportInitialize.
        /// </summary>
        public void BeginInit()
        {
            OnInitialization = true;
        }

        /// <summary>
        /// Method EndInit to implement inherited from ISupportInitialize.
        /// </summary>
        public void EndInit()
        {
            OnInitialization = false;
            if (DesignMode)
                return;

            foreach (TabControl tabCtrl in DelayedSetUseMnemonic)
            {
                ProcessDelayedUseMnemonic(tabCtrl);
            }
            DelayedSetUseMnemonic.Clear();
        }


        /// <summary>
        /// Class constructor.
        /// </summary>
        public SSTabHelper()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="container"></param>
        public SSTabHelper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// Gets the value for the property ActiveTabFontStyle.
        /// </summary>
        /// <param name="tabControl">The tab control to test.</param>
        /// <returns>The current value for ActiveTabFontStyle property.</returns>
        [Description("Sets the font style to use for the active tabs"), Category("Custom Properties")]
        public ActiveTabFontStyleEnum GetActiveTabFontStyle(TabControl tabControl)
        {
            return Static_GetActiveTabFontStyle(tabControl);
        }
        /// <summary>
        /// Sets the value for the property ActiveTabFontStyle.
        /// </summary>
        /// <param name="tabControl">The tab control to set.</param>
        /// <param name="value">The value to be set.</param>
        public void SetActiveTabFontStyle(TabControl tabControl, ActiveTabFontStyleEnum value)
        {
            Static_SetActiveTabFontStyle(tabControl, value);
        }

        /// <summary>
        /// Gets the value for the property ActiveTabFontStyle.
        /// </summary>
        /// <param name="tabControl">The tab control to test.</param>
        /// <returns>The current value for ActiveTabFontStyle property.</returns>
        private static ActiveTabFontStyleEnum Static_GetActiveTabFontStyle(TabControl tabControl)
        {
            if (CheckForProperty(tabControl, newPropertiesEnum.ActiveFontStyle))
                return (ActiveTabFontStyleEnum)newProperties[tabControl.GetHashCode()][newPropertiesEnum.ActiveFontStyle];

            return ActiveTabFontStyleEnum.Default;
        }

        /// <summary>
        /// Sets the value for the property ActiveTabFontStyle.
        /// </summary>
        /// <param name="tabControl">The tab control to set.</param>
        /// <param name="value">The value to be set.</param>
        private static void Static_SetActiveTabFontStyle(TabControl tabControl, ActiveTabFontStyleEnum value)
        {
            if (CheckForProperty(tabControl, newPropertiesEnum.ActiveFontStyle))
            {
                newProperties[tabControl.GetHashCode()][newPropertiesEnum.ActiveFontStyle] = value;
                if (value != ActiveTabFontStyleEnum.Default)
                    SetCustomDrawingMode(tabControl, newPropertiesEnum.ActiveFontStyle, true);
            }
        }

        // USEMNEMONIC PROPERTY
        // AIS BUG-1644

        /// <summary>
        /// Adds the property UseMnemonic to the TabControls.
        /// </summary>
        /// <param name="tabControl">The TabControl to enable the property.</param>
        /// <returns>True if the TabControls have to set the UseMnemonic property set.</returns>
        [Description("If true, the first character preceded by an ampersand will be used as the button's mnemonic key.")]
        public bool GetUseMnemonic(TabControl tabControl)
        {
            return Static_GetUseMnemonic(tabControl);
        }

        /// <summary>
        /// The static implmentation of GetUseMnemonic for internal use.
        /// </summary>
        /// <param name="tabControl">The TabControl to enable the property.</param>
        /// <returns>True if the TabControls have to set the UseMnemonic property set.</returns>
        private static bool Static_GetUseMnemonic(TabControl tabControl)
        {
            bool needsUpdateForced = ((!newProperties.ContainsKey(tabControl.GetHashCode())) || (!newProperties[tabControl.GetHashCode()].ContainsKey(newPropertiesEnum.UseMnemonic)));
            bool res = false;

            if (CheckForProperty(tabControl, newPropertiesEnum.UseMnemonic))
                res = (bool)newProperties[tabControl.GetHashCode()][newPropertiesEnum.UseMnemonic];

            if (needsUpdateForced)
                SetCustomDrawingMode(tabControl, newPropertiesEnum.UseMnemonic, res);

            return res;
        }

        /// <summary>
        /// Adds the property UseMnemonic to the TabControls.
        /// </summary>
        /// <param name="tabControl">The TabControl to enable the property.</param>
        /// <param name="value">The value to be set.</param>
        public void SetUseMnemonic(TabControl tabControl, bool value)
        {
            Static_SetUseMnemonic(tabControl, value, OnInitialization);
        }

        /// <summary>
        /// The static implementation of SetUseMnemonic, for internal use.
        /// </summary>
        /// <param name="tabControl">The TabControl to enable the property.</param>
        /// <param name="value">The value to be set.</param>
        /// <param name="OnInitialization"></param>
        private static void Static_SetUseMnemonic(TabControl tabControl, bool value, bool OnInitialization)
        {
            int key = tabControl.GetHashCode();
            bool needsUpdateForced = (!newProperties.ContainsKey(key) || !newProperties[key].ContainsKey(newPropertiesEnum.UseMnemonic));

            if (CheckForProperty(tabControl, newPropertiesEnum.UseMnemonic))
            {
                if (needsUpdateForced || (((bool)newProperties[key][newPropertiesEnum.UseMnemonic]) != value))
                {
                    newProperties[key][newPropertiesEnum.UseMnemonic] = value;
                    if (value)
                        SetCustomDrawingMode(tabControl, newPropertiesEnum.UseMnemonic, true);
                    else
                        SetCustomDrawingMode(tabControl, newPropertiesEnum.UseMnemonic, false);

                    if (OnInitialization)
                        DelayedSetUseMnemonic.Add(tabControl);
                    else
                        ProcessDelayedUseMnemonic(tabControl);
                }
            }
        }

        /// <summary>
        /// Process a UseMnemonic property for a tabControl when this was delayed.
        /// </summary>
        /// <param name="tabControl">The TabControl to process.</param>
        private static void ProcessDelayedUseMnemonic(TabControl tabControl)
        {
            bool value = (bool)newProperties[tabControl.GetHashCode()][newPropertiesEnum.UseMnemonic];

            Form parentForm = tabControl.FindForm();
            int code = parentForm.GetHashCode();
            if (value)
            {
                parentForm.KeyPreview = true;
                parentForm.KeyDown += TabControl_ParentForm_KeyDown;

                if (!FormsWithTabsControlsUsingMnemonic.ContainsKey(code))
				{
                    FormsWithTabsControlsUsingMnemonic.Add(code, new List<TabControl>());
					FormClosedEventHandler handler = new FormClosedEventHandler(delegate(object sender, FormClosedEventArgs e)
						{
							FormsWithTabsControlsUsingMnemonic.Remove(code);
							parentForm.KeyDown -= TabControl_ParentForm_KeyDown;
						});
					parentForm.FormClosed += handler;
				}
                FormsWithTabsControlsUsingMnemonic[code].Add(tabControl);
            }
            else
            {
                parentForm.KeyPreview = false;
                parentForm.KeyDown -= TabControl_ParentForm_KeyDown;

                if (FormsWithTabsControlsUsingMnemonic.ContainsKey(code))
                {
                    if (FormsWithTabsControlsUsingMnemonic[code].Contains(tabControl))
                        FormsWithTabsControlsUsingMnemonic[code].Remove(tabControl);

                    if (FormsWithTabsControlsUsingMnemonic[code].Count == 0)
                        FormsWithTabsControlsUsingMnemonic.Remove(code);
                }
            }
        }

        /// <summary>
        /// Handles the KeyDown event in the parent form so we can emulate the UseMnemonic property.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The Key event arguments.</param>
        private static void TabControl_ParentForm_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char character = Convert.ToChar(e.KeyCode);

                if ((e.Alt) && !Char.IsControl(character) && FormsWithTabsControlsUsingMnemonic.ContainsKey(sender.GetHashCode()))
                {
                    foreach (TabControl tabCtrl in FormsWithTabsControlsUsingMnemonic[sender.GetHashCode()])
                    {
                        TabPage target = FindTabPageToHandleMnemonic(tabCtrl, character);
                        if (target != null)
                        {
                            tabCtrl.SelectedTab = target;
                            e.Handled = true;
                            //AIS-Bug 7527 FSABORIO
                            e.SuppressKeyPress = true;
                            return;
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Finds the TabPage that can respond to the mnemonic character 'character' within TabControl.
        /// </summary>
        /// <param name="tabCtrl">The parent TabControl.</param>
        /// <param name="character">The character to test.</param>
        /// <returns>Null if no tab page is found, otherwise the tab page is returned.</returns>
        private static TabPage FindTabPageToHandleMnemonic(TabControl tabCtrl, char character)
        {
            int index = tabCtrl.SelectedIndex + 1;

            for (int i = index; i < tabCtrl.TabCount; i++)
            {
                if (Control.IsMnemonic(character, tabCtrl.TabPages[i].Text) && GetTabEnabled(tabCtrl, i) && GetTabVisible(tabCtrl, i))
                    return tabCtrl.TabPages[i];
            }

			for (int i = 0; i <= index; i++)
            {
                if (Control.IsMnemonic(character, tabCtrl.TabPages[i].Text) && GetTabEnabled(tabCtrl, i) && GetTabVisible(tabCtrl, i))
                    return tabCtrl.TabPages[i];
            }

            return null;
        }

        /// <summary>
        /// Returns a list of the tabpages for the tabControl with a Mnemonic set.
        /// </summary>
        /// <param name="tabCtrl">The TabControl parent.</param>
        /// <returns>A list, only valid tabPages are returned.</returns>
        private Dictionary<int, TabPage> ListOfTabPagesWithMnemonics(TabControl tabCtrl)
        {
            Dictionary<int, TabPage> res = ListOfTabPagesWithMnemonics(tabCtrl);
            for (int i = 0; i < tabCtrl.TabCount; i++)
            {
                //tabCtrl.TabPages[i].Text[1].
            }

            return res;
        }

        /// <summary>
        /// Checks if the control can process Mnemonics.
        /// </summary>
        /// <param name="ctrl">The control to test.</param>
        /// <returns>True if the control can process Mnemonics.</returns>
        private bool CanProcessMnemonic(Control ctrl)
        {
            if (!ctrl.Enabled || !ctrl.Visible)
            {
                return false;
            }
            if (ctrl.Parent != null)
            {
                return CanProcessMnemonic(ctrl.Parent);
            }
            return true;
        }

        //////////////////////////////
        //// USEMNEMONIC PROPERTY ////
        //////////////////////////////

        /// <summary>
        /// Enables/Disables a Tab.
        /// </summary>
        /// <param name="TabCtrl">The TabCtrl to be enabled.</param>
        /// <param name="index">The TabControl index.</param>
        /// <param name="value">Indicates if enable/disable TabControl.</param>
        public static void SetTabEnabled(TabControl TabCtrl, int index, bool value)
        {
            IList<int> lstDisabled;
            if (TabsDisabled.ContainsKey(TabCtrl.GetHashCode()))
                lstDisabled = TabsDisabled[TabCtrl.GetHashCode()];
            else
            {
                lstDisabled = new List<int>();
                TabsDisabled.Add(TabCtrl.GetHashCode(), lstDisabled);
                TabCtrl.Selecting += TabControl_Selecting;
                SetCustomDrawingMode(TabCtrl, newPropertiesEnum.TabEnabled, true);
                TabCtrl.EnabledChanged += TabCtrl_EnabledChanged;
            }

            //Tab is being enabled so it must be eliminated from the list
            if ((value) && (lstDisabled.Contains(index)))
                lstDisabled.Remove(index);

            //Tab is being disabled so it must be added if necessary
            if ((!value) && (!lstDisabled.Contains(index)))
            {
                lstDisabled.Add(index);
                GetTabControlPages(TabCtrl)[index].ForeColor = Color.Green;
            }

            TabsDisabled[TabCtrl.GetHashCode()] = lstDisabled;
            TabCtrl.Refresh();
        }

        /// <summary>
        /// Sets the tabControl to use the custom drawing mode, 
        /// if the mode or the eventhandler have already been set then
        /// they are not set again.
        /// </summary>
        /// <param name="tabControl">The tabControl to set its custom value.</param>
        /// <param name="property">A custom behaviour value.</param>
        /// <param name="value">True if the custom drawing mode has to be set.</param>
        private static void SetCustomDrawingMode(TabControl tabControl, newPropertiesEnum property, bool value)
        {
            if (tabControl != null)
            {
                int key = tabControl.GetHashCode();
                if (value)
                {
                    if (!ControlOfCustomDrawingMode.ContainsKey(key))
                    {
                        tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
                        tabControl.DrawItem += TabControl_DrawItem;
                        ControlOfCustomDrawingMode.Add(key, new List<newPropertiesEnum>());
                    }

                    if (!ControlOfCustomDrawingMode[key].Contains(property))
                        ControlOfCustomDrawingMode[key].Add(property);
                }
                else
                {
                    if (ControlOfCustomDrawingMode.ContainsKey(key))
                    {
                        if (ControlOfCustomDrawingMode[key].Contains(property))
                            ControlOfCustomDrawingMode[key].Remove(property);

                        if (ControlOfCustomDrawingMode[key].Count == 0)
                        {
                            tabControl.DrawMode = TabDrawMode.Normal;
                            tabControl.DrawItem -= TabControl_DrawItem;
                            ControlOfCustomDrawingMode.Remove(key);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Forces to redraw the tabs when the tab control has been enabled/disabled.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void TabCtrl_EnabledChanged(object sender, EventArgs e)
        {
            ((TabControl)sender).Invalidate();
        }

        /// <summary>
        /// Gets the current status of a tab into a tabcontrol.
        /// </summary>
        /// <param name="TabCtrl">The tab control to test.</param>
        /// <param name="index">The tab index.</param>
        public static bool GetTabEnabled(TabControl TabCtrl, int index)
        {
            if (TabsDisabled.ContainsKey(TabCtrl.GetHashCode()))
            {
                return !TabsDisabled[TabCtrl.GetHashCode()].Contains(index);
            }
            return true;
        }

        /// <summary>
        /// Indicates whether the given TabPage is currently enabled.
        /// </summary>
        /// <param name="tabControl">The <c>TabControl</c> object owning the given TabPage.</param>
        /// <param name="page">The <c>TabPage</c> object to test for.</param>
        /// <returns><c>true</c> if the tab is currently enabled, <c>false</c> otherwise.</returns>
        private static bool GetTabEnabled(TabControl tabControl, TabPage page)
        {
            int index = GetPageIndex(tabControl, page);
            return GetTabEnabled(tabControl, index);

        }

        
        /// <summary>
        /// Gets the index of <c>page</c> in <c>tabControl</c> but considering already hidden tabpages.
        /// This method is different from calling <c>tabControl.TabPages.IndexOf</c> 
        /// due to the <c>SetVisible</c> implementation which removes
        /// tabs instead of hide them.
        /// </summary>
        /// <param name="tabControl">The TabControl object owning the page to test for.</param>
        /// <param name="page">The TabPage object to get index for.</param>
        /// <returns>The index page.</returns>
        private static int GetPageIndex(TabControl tabControl, TabPage page)
        {
            return VisibleAffected(tabControl) ? GetTabControlPages(tabControl).IndexOf(page) : tabControl.TabPages.IndexOf(page);
        }

        /// <summary>
        /// Avoids to select tabs disabled.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The TabControlCancel event arguments.</param>
        private static void TabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.Action == TabControlAction.Selecting && !GetTabEnabled((TabControl)sender, GetPageIndex((TabControl)sender, e.TabPage)))
                e.Cancel = true;
        }

        /// <summary>
        /// Paints the gray disable font.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The DrawItem event arguments.</param>
        private static void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl cTabControl = ((TabControl)sender);
            // painting in middle of SetVisible operation
            if (e.Index >= cTabControl.TabCount) 
                return;
            Graphics g = e.Graphics;
			using (StringFormat strFrmt = new StringFormat())
			{
                strFrmt.Trimming = StringTrimming.Character;
                strFrmt.Alignment = StringAlignment.Center;
                strFrmt.LineAlignment = StringAlignment.Center;
                Rectangle tmpRec;
                Color col;
                bool UsesMnemonic = Static_GetUseMnemonic(cTabControl);
                //AIS Bug-1644
                if (UsesMnemonic)
                    strFrmt.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                //AIS Bug-1644

                ActiveTabFontStyleEnum activeTabEnum = Static_GetActiveTabFontStyle(cTabControl);
                Font tabFont;
                Color bgColor;

                tabFont = cTabControl.Font;
                bgColor = cTabControl.TabPages[e.Index].BackColor;
                if (bgColor == Color.Transparent)
                    bgColor = cTabControl.BackColor;

                if (cTabControl.Enabled && GetTabEnabled(cTabControl, cTabControl.TabPages[e.Index]))
                    col = cTabControl.ForeColor;
                else
                    col = Color.Gray;

                Rectangle tabRect = cTabControl.GetTabRect(e.Index);
				using (SolidBrush drawBrush = new SolidBrush(col), bgBrush = new SolidBrush(bgColor))
				{
                    if ((cTabControl.Alignment == TabAlignment.Top) || (cTabControl.Alignment == TabAlignment.Bottom))
                    {
                        if (cTabControl.SelectedIndex == e.Index)
                        {
                            if (activeTabEnum != ActiveTabFontStyleEnum.Default)
                                tabFont = GetModifiedFont(tabFont, activeTabEnum);

                            if (cTabControl.Alignment == TabAlignment.Top)
                                tmpRec = new Rectangle(tabRect.X, tabRect.Y + tabRect.Height - 2, tabRect.Width, 4);
                            else
                                tmpRec = new Rectangle(tabRect.X, tabRect.Y - 2, tabRect.Width, 4);
                        }
                        else
                            tmpRec = e.Bounds;

						g.FillRectangle(bgBrush, tmpRec);

                        strFrmt.FormatFlags = 0;
                        g.DrawString(cTabControl.TabPages[e.Index].Text, tabFont, drawBrush, tabRect, strFrmt);
                    }
                    else
                    {
                        if (cTabControl.SelectedIndex == e.Index)
                        {
                            if (activeTabEnum != ActiveTabFontStyleEnum.Default)
                                tabFont = GetModifiedFont(tabFont, activeTabEnum);

                            if (cTabControl.Alignment == TabAlignment.Left)
                                tmpRec = new Rectangle(tabRect.X + tabRect.Width - 2, tabRect.Y, 4, tabRect.Height);
                            else
                                tmpRec = new Rectangle(tabRect.X - 2, tabRect.Y, 4, tabRect.Height);
                        }
                        else
                            tmpRec = e.Bounds;

						g.FillRectangle(bgBrush, tmpRec);

                        strFrmt.FormatFlags = StringFormatFlags.DirectionVertical;
                        g.DrawString(cTabControl.TabPages[e.Index].Text, tabFont, drawBrush, tabRect, strFrmt);
                    }
				}
			}
            cTabControl.Update();
        }

        /// <summary>
        /// Returns a modified font based on the property value.
        /// </summary>
        /// <param name="tabFont">The base font.</param>
        /// <param name="activeTabEnum">The enum specifying how to change it.</param>
        /// <returns>A new font modified.</returns>
        private static Font GetModifiedFont(Font tabFont, ActiveTabFontStyleEnum activeTabEnum)
        {
            Font res = new Font(tabFont, FontStyle.Regular);
            switch (activeTabEnum)
            {
                case ActiveTabFontStyleEnum.Bold:
                    res = FontChangeBold(res, true);
                    break;
                case ActiveTabFontStyleEnum.Bold_Italic:
                    res = FontChangeBold(res, true);
                    res = FontChangeItalic(res, true);
                    break;
                case ActiveTabFontStyleEnum.Italic:
                    res = FontChangeItalic(res, true);
                    break;
                case ActiveTabFontStyleEnum.Strikeout:
                    res = FontChangeStrikeout(res, true);
                    break;
                case ActiveTabFontStyleEnum.Underline:
                    res = FontChangeUnderline(res, true);
                    break;
            }

            return res;
        }

        /// <summary>
        /// Used instead of SelectedIndex when SetTabVisible has been used previously in the TabCtrl.
        /// Using SetTabVisible in a TabCtrl may return incorrect values in TabCtrl.SelectedIndex.
        /// </summary>
        /// <param name="TabCtrl">The Tab Control to test.</param>
        public static int GetSelectedTabIndex(TabControl TabCtrl)
        {
            int index = TabCtrl.SelectedIndex;
            UpdateTabsVisible(TabCtrl);
            if (index != -1)
            {
                index = TabsVisible[TabCtrl.GetHashCode()].IndexOf(new KeyValuePair<TabPage, bool>(TabCtrl.SelectedTab, true));
                if (index == -1)
                    index = TabsVisible[TabCtrl.GetHashCode()].IndexOf(new KeyValuePair<TabPage, bool>(TabCtrl.SelectedTab, false));
            }
            return index;
        }

        /// <summary>
        /// Used instead of SelectedIndex when SetTabVisible has been used previously in the TabCtrl.
        /// Using SetTabVisible in a TabCtrl may return incorrect values in TabCtrl.SelectedIndex.
        /// </summary>
        /// <param name="TabCtrl">The Tab Control to test.</param>
        /// <param name="index">The Tab index.</param>
        public static void SetSelectedTabIndex(TabControl TabCtrl, int index)
        {
            UpdateTabsVisible(TabCtrl);

            if (!GetTabVisible(TabCtrl, index))
                throw new InvalidOperationException("Invalid property value");

            TabCtrl.SelectedTab = TabsVisible[TabCtrl.GetHashCode()][index].Key;
        }

        /// <summary>
        /// Sets the current visible status of a tab into a tabcontrol.
        /// </summary>
        /// <param name="TabCtrl">The Tab Control to set.</param>
        /// <param name="index">The Tab index.</param>
        /// <param name="value">The Visible value being set.</param>
        public static void SetTabVisible(TabControl TabCtrl, int index, bool value)
        {
            UpdateTabsVisible(TabCtrl);
            List<KeyValuePair<TabPage, bool>> visibleTabs = TabsVisible[TabCtrl.GetHashCode()];
            if ((index < 0) || (index >= visibleTabs.Count))
                throw new Exception("Invalid property array index");

            KeyValuePair<TabPage, bool> tabPageToChange = visibleTabs[index];
            if (tabPageToChange.Value != value)
            {
                //Set invisible by removing it from the TabControl
                if (!value)
                {
                    visibleTabs[index] = new KeyValuePair<TabPage, bool>(tabPageToChange.Key, value);
                    TabCtrl.TabPages.Remove(tabPageToChange.Key);

                }
                else
                {
                    //Set visible by checking its position and inserting it if necessary
                    int expectedPosition = 0;
                    
                    visibleTabs[index] = new KeyValuePair<TabPage, bool>(tabPageToChange.Key, value);
                    for (int i = 0; i < index; i++)
                    {
                        if (visibleTabs[i].Value)
                        {
                            expectedPosition++;
                        }
                    }
                    TabCtrl.TabPages.Insert(expectedPosition, tabPageToChange.Key);
                }
                TabCtrl.Visible = (TabCtrl.TabPages.Count > 0);
            }
        }

        /// <summary>
        /// When the form dies the TabControls should be released
        /// </summary>
        /// <param name="form">The Form</param>
        public static void ReleaseResources(Form form)
        {
            int code = form.GetHashCode();
            List<TabControl> tabs = FormsWithTabsControlsUsingMnemonic[code];
            foreach (TabControl tab in tabs)
            {
                int tabcode = tab.GetHashCode();
                TabsDisabled.Remove(tabcode);
                TabsVisible.Remove(tabcode);
                ControlOfCustomDrawingMode.Remove(tabcode);
                newProperties.Remove(tabcode);
            }
            FormsWithTabsControlsUsingMnemonic.Remove(code);
        }

        /// <summary>
        /// Internal function to update the TabsVisible for the TabCtrl.
        /// </summary>
        /// <param name="TabCtrl">The Tab Control to update.</param>
        private static void UpdateTabsVisible(TabControl TabCtrl)
        {
            //The tabControl is not in the list of control yet
            if (!TabsVisible.ContainsKey(TabCtrl.GetHashCode()))
            {
                TabsVisible.Add(TabCtrl.GetHashCode(), new List<KeyValuePair<TabPage, bool>>());
                for (int i = 0; i < TabCtrl.TabPages.Count; i++)
                {
                    TabsVisible[TabCtrl.GetHashCode()].Add(new KeyValuePair<TabPage, bool>(TabCtrl.TabPages[i], true));
                }
            }
        }

        /// <summary>
        /// Sets the current visible status of a tab into a tabcontrol.
        /// </summary>
        /// <param name="TabCtrl">The Tab Control to test.</param>
        /// <param name="index">The Tab index.</param>
        public static bool GetTabVisible(TabControl TabCtrl, int index)
        {
            UpdateTabsVisible(TabCtrl);

            if ((index < 0) || (index >= TabsVisible[TabCtrl.GetHashCode()].Count))
                throw new Exception("Invalid property array index");

            return TabsVisible[TabCtrl.GetHashCode()][index].Value;
        }

        /// <summary>
        /// Check if the property 'newPropertiesEnum' is already defined for this tabcontrol.
        /// </summary>
        /// <param name="tabControl">The tab control to test.</param>
        /// <param name="prop">The property to check.</param>
        /// <returns>True if property could be checked.</returns>
        private static bool CheckForProperty(TabControl tabControl, newPropertiesEnum prop)
        {
            if (tabControl == null)
                return false;

            CheckNewProperties(tabControl);
            if (!newProperties[tabControl.GetHashCode()].ContainsKey(prop))
                newProperties[tabControl.GetHashCode()][prop] = GetDefaultValueForProperty(prop);

            return true;
        }

        /// <summary>
        /// Checks if the tabControl is controlled by the newProperties Dictionary.
        /// </summary>
        /// <param name="tabControl">The tab control to test.</param>
        private static void CheckNewProperties(TabControl tabControl)
        {
            if (!newProperties.ContainsKey(tabControl.GetHashCode()))
            {
                newProperties[tabControl.GetHashCode()] = new Dictionary<newPropertiesEnum, object>();
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
                case newPropertiesEnum.ActiveFontStyle:
                    return ActiveTabFontStyleEnum.Default;
                case newPropertiesEnum.UseMnemonic:
                    return true;
            }

            return null;
        }

        /// <summary>
        /// Indicates whether the given TabControl object has been affected by a 
        /// SetTabVisible operation.
        /// </summary>
        /// <param name="tabControl">The TabControl object to test for.</param>
        /// <returns><c>true</c> if at least one tab page of the control has been made invisible.</returns>
        private static Boolean VisibleAffected(TabControl tabControl)
        {
            return TabsVisible.ContainsKey(tabControl.GetHashCode());
        }


        /// <summary>
        /// Keeps a cache containing Tab pages being searched.
        /// </summary>
        private static readonly IDictionary<int, WeakReference> pagesCache = new System.Collections.Generic.Dictionary<int, WeakReference>();

        /// <summary>
        /// Gets the TabPages of the Tab Control.
        /// </summary>
        /// <param name="tabControl">The Tab Control to test.</param>
        /// <returns>The visible Tab pages in the Tab Control.</returns>
        private static IList<TabPage> GetTabControlPages(TabControl tabControl)
        {
            int key = tabControl.GetHashCode();
            WeakReference reference = null;
            if (pagesCache.ContainsKey(key))
                reference = pagesCache[key];
            if (reference == null)
                pagesCache[key] = (reference = new WeakReference(null));
            IList<TabPage> result = (IList<TabPage>)reference.Target;
            if (result == null)
            {
                reference.Target = result = new List<TabPage>();
                if (VisibleAffected(tabControl))
                    foreach (KeyValuePair<TabPage, bool> pageEntry in TabsVisible[tabControl.GetHashCode()])
                        result.Add(pageEntry.Key);
                else
                    foreach (TabPage page in tabControl.TabPages)
                        result.Add(page);
            }
            return result;
        }

		/// <summary>
		/// Sets the caption of a tab page.
		/// </summary>
		/// <param name="TabCtrl">The TabControl to use.</param>
		/// <param name="index">The index of the tab.</param>
		/// <param name="caption">The caption to set.</param>
		public static void SetTabCaption(TabControl TabCtrl, int index, string caption)
		{
			List<KeyValuePair<TabPage, bool>> list;
			TabsVisible.TryGetValue(TabCtrl.GetHashCode(), out list);
			if (list == null)
			{
				TabCtrl.TabPages[index].Text = caption;
			}
			else
			{
				list[index].Key.Text = caption;
			}
		}

		/// <summary>
		/// Gets the caption of a tab in a TabControl.
		/// </summary>
		/// <param name="TabCtrl">The TabControl to use.</param>
		/// <param name="index">The index of the tab.</param>
		/// <returns>The caption of the specified tab.</returns>
		public static string GetTabCaption(TabControl TabCtrl, int index)
		{
			List<KeyValuePair<TabPage, bool>> list;
			TabsVisible.TryGetValue(TabCtrl.GetHashCode(), out list);
			if (list == null)
			{
				return TabCtrl.TabPages[index].Text;
			}
			else
			{
				return list[index].Key.Text;
			}
		}
		
		/// <summary>
		/// Gets the number of tab pages in a TabControl.
		/// </summary>
		/// <param name="TabCtrl">The TabControl to test.</param>
		/// <returns>The number of tabs in the TabControl.</returns>
		public static int GetTabCount(TabControl TabCtrl)
		{
			List<KeyValuePair<TabPage, bool>> list;
			TabsVisible.TryGetValue(TabCtrl.GetHashCode(), out list);
			if (list == null)
			{
				return TabCtrl.TabPages.Count;
			}
			else
			{
				return list.Count;
			}
		}

		/// <summary>
		/// Selects the specified tab in the TabControl.
		/// </summary>
		/// <param name="TabCtrl">The tab control to use.</param>
		/// <param name="index">The index of the tab to select.</param>
		public static void SetSelectedIndex(TabControl TabCtrl, int index)
		{
			List<KeyValuePair<TabPage, bool>> list;
			TabsVisible.TryGetValue(TabCtrl.GetHashCode(), out list);
			if (list == null)
			{
				TabCtrl.SelectedIndex = index;
			}
			else
			{
				// If the tab is invisible, throw an exception.
				if (!list[index].Value)
				{
					throw new Exception("Run-time error '380':\r\n\r\nInvalid property value");
				}
				TabCtrl.SelectedTab = list[index].Key;
			}
		}

		/// <summary>
		/// Gets the index of the selected tab.  Unlike in VB6, if
		/// no tabs are visible, it will return -1 instead of the
		/// index of the last visible tab.
		/// </summary>
		/// <param name="TabCtrl">The TabControl to use.</param>
		/// <returns>The index of the selected tab.</returns>
		public static int GetSelectedIndex(TabControl TabCtrl)
		{
			List<KeyValuePair<TabPage, bool>> list;
			TabsVisible.TryGetValue(TabCtrl.GetHashCode(), out list);
			if (list == null)
			{
				return TabCtrl.SelectedIndex;
			}
			else
			{
				return GetPageIndex(TabCtrl, TabCtrl.SelectedTab);
			}
		}
		private static System.Drawing.Font FontChangeBold(System.Drawing.Font CurrentFont, bool Bold)
		{
			return FontChangeStyle(CurrentFont, FontStyle.Bold, Bold);
		}
		private static System.Drawing.Font FontChangeItalic(System.Drawing.Font CurrentFont, bool Italic)
		{
			return FontChangeStyle(CurrentFont, FontStyle.Italic, Italic);
		}
		private static System.Drawing.Font FontChangeStrikeout(System.Drawing.Font CurrentFont, bool Strikeout)
		{
			return FontChangeStyle(CurrentFont, FontStyle.Strikeout, Strikeout);
		}
		private static System.Drawing.Font FontChangeUnderline(System.Drawing.Font CurrentFont, bool Underline)
		{
			return FontChangeStyle(CurrentFont, FontStyle.Underline, Underline);
		}

		private static System.Drawing.Font FontChangeStyle(System.Drawing.Font CurrentFont, FontStyle StyleBit, bool NewValue)
		{
			bool flag = (CurrentFont.Style & StyleBit) != FontStyle.Regular;
			if (flag == NewValue)
			{
				return CurrentFont;
			}
			FontStyle newStyle = CurrentFont.Style & ~StyleBit;
			if (NewValue)
			{
				newStyle |= StyleBit;
			}
			return new System.Drawing.Font(CurrentFont, newStyle);
		}


    }
}
