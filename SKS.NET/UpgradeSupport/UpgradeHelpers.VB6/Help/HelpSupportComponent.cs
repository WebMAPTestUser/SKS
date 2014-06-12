using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace UpgradeHelpers.VB6.Help
{
    /// <summary>
    /// This class provides support for Help feature.
    /// </summary>
    public partial class HelpSupportComponent : Component
    {
        private static OpenFileDialog openFileDialog = new OpenFileDialog();

        static HelpSupportComponent()
        {
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = UpgradeHelpers.VB6.Resources.UpgradeHelpers_VB6.UpgradeHelpers_VB6_Help_HelpSupportClass_ValidateHelpFile_OpenDialog_Filter;
        }
        /// <summary>
        /// Help Constants
        /// </summary>
        public enum HelpConstants
        {
            /// <summary>
            /// Help Context
            /// </summary>
            cdlHelpContext = 1,
            /// <summary>
            /// Quit Help
            /// </summary>
            cdlHelpQuit = 2,
            /// <summary>
            /// HelpContents
            /// </summary>
            cdlHelpContents = 3,
            /// <summary>
            /// Help on Help
            /// </summary>
            cdlHelpHelpOnHelp = 4,
            /// <summary>
            /// Set contents
            /// </summary>
            cdlHelpSetContents = 5,
            /// <summary>
            /// Help Context Popup
            /// </summary>
            cdlHelpContextPopup = 8,
            /// <summary>
            /// Force File
            /// </summary>
            cdlHelpForceFile = 9,
            /// <summary>
            /// Keyboard to show help
            /// </summary>
            cdlHelpKey = 257,
            /// <summary>
            /// Command to show help
            /// </summary>
            cdlHelpCommandHelp = 258,
            /// <summary>
            /// Partial key
            /// </summary>
            cdlHelpPartialKey = 261,
        }
        /// <summary>
        /// Help Support Component Constructor
        /// </summary>
        public HelpSupportComponent()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Help Support Component Constructor, set the internal container
        /// </summary>
        /// <param name="container">add the instance to the container</param>
        public HelpSupportComponent(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private string _HelpFile = string.Empty;

        /// <summary>
        /// Get/Set the HelpFile path
        /// </summary>
        public string HelpFile
        {
            get { return _HelpFile; }
            set { _HelpFile = value; }
        }

        private int _HelpCommand = 0;
        /// <summary>
        /// Get/Set Help Command
        /// </summary>
        public int HelpCommand
        {
            get { return _HelpCommand; }
            set { _HelpCommand = value; }
        }

        private object _HelpContext = null;
        /// <summary>
        /// Get/Set Help Context object
        /// </summary>
        public object HelpContext
        {
            get { return _HelpContext; }
            set
            {
                if (value != null)
                    value = value.ToString();
                _HelpContext = value;
            }
        }
        /// <summary>
        /// Opens the Help Window
        /// </summary>
        public void ShowHelp()
        {
            try
            {
                if (ValidateHelpFile())
                {
                    if (_HelpCommand == (int)HelpConstants.cdlHelpContext)
                    {
                       System.Windows.Forms.Help.ShowHelp(null, _HelpFile, HelpNavigator.TopicId, _HelpContext);
                      
                    }
                    else if (_HelpCommand == (int)HelpConstants.cdlHelpContents)
                    {
                        System.Windows.Forms.Help.ShowHelp(null, _HelpFile, HelpNavigator.TableOfContents);
                    }
                    else if (_HelpCommand == (int)HelpConstants.cdlHelpForceFile)
                    {
                        System.Windows.Forms.Help.ShowHelp(null, _HelpFile, HelpNavigator.TableOfContents);
                    }
                    else if (_HelpCommand == (int)HelpConstants.cdlHelpKey)
                    {
                        System.Windows.Forms.Help.ShowHelp(null, _HelpFile, HelpNavigator.KeywordIndex, _HelpContext);
                    }
                    else if (_HelpCommand == (int)HelpConstants.cdlHelpPartialKey)
                    {
                        System.Windows.Forms.Help.ShowHelp(null, _HelpFile, HelpNavigator.KeywordIndex, _HelpContext);
                    }
                    else {
                        throw new Exception("Option not supported");
                    }
                }
               
            }
            catch (Exception e)
            {
                MessageBox.Show("Error displaying help: " + e.Message, "Displaying help", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Validates that HelpFile exists.
        /// </summary>
        /// <returns></returns>
        private bool ValidateHelpFile()
        {
            string helpPath = string.Empty;
            if (!string.IsNullOrEmpty(HelpFile))
            {
                helpPath = System.IO.Path.GetFullPath(HelpFile);

                if (!System.IO.Path.HasExtension(helpPath))
                    helpPath = helpPath + ".chm";

                if (!System.IO.File.Exists(helpPath))
                {
                    if ((MessageBox.Show(string.Format(UpgradeHelpers.VB6.Resources.UpgradeHelpers_VB6.UpgradeHelpers_VB6_Help_HelpSupportClass_ValidateHelpFile_Question, helpPath),
                        UpgradeHelpers.VB6.Resources.UpgradeHelpers_VB6.UpgradeHelpers_VB6_Help_HelpSupportClass_ValidateHelpFile_Question_Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        && (openFileDialog.ShowDialog() == DialogResult.OK))
                    {
                        HelpFile = openFileDialog.FileName;

                        return true;
                    }

                    MessageBox.Show(string.Format(UpgradeHelpers.VB6.Resources.UpgradeHelpers_VB6.UpgradeHelpers_VB6_Help_HelpSupportClass_ValidateHelpFile_ValidationFailure, helpPath),
                        UpgradeHelpers.VB6.Resources.UpgradeHelpers_VB6.UpgradeHelpers_VB6_Help_HelpSupportClass_ValidateHelpFile_Question_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return false;
                }
                else
                    return true;
            }
            else
                return false;
        }
    }
}
