using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace UpgradeHelpers.VB6.Help
{
    /// <summary>
    /// This class provides support for Help feature.
    /// </summary>
    public class HelpSupportClass
    {
        /// <summary>
        /// Help Ignore Restrictions Enum
        /// </summary>
        public enum HelpIgnoreResctrictionsEnum
        {
            /// <summary>
            /// MDI Container Restriction
            /// </summary>
            MDIContainerRestriction = 1
        }

        private static Dictionary<String, String> HelpFilesByProject = new Dictionary<String,String>();
        private string _project = "";
        private static readonly string HelpRequestedEvent = "HelpRequested";
        private static OpenFileDialog openFileDialog = new OpenFileDialog();

        /// <summary>
        /// Class Constructor
        /// </summary>
        public HelpSupportClass()
        {
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = UpgradeHelpers.VB6.Resources.UpgradeHelpers_VB6.UpgradeHelpers_VB6_Help_HelpSupportClass_ValidateHelpFile_OpenDialog_Filter;
            _project = Application.ProductName;
            //helpProvider.HelpNamespace = HelpFilesByProject[_project];
        }

        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="project">The current project name</param>
        public HelpSupportClass(String project)
        {
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = UpgradeHelpers.VB6.Resources.UpgradeHelpers_VB6.UpgradeHelpers_VB6_Help_HelpSupportClass_ValidateHelpFile_OpenDialog_Filter;
            _project = project;
            helpProvider.HelpNamespace = HelpFilesByProject[project];
        }

        /// <summary>
        /// List of the HelpRequested event handlers patched by control.
        /// </summary>
        private Dictionary<Control, List<Delegate>> PatchedHelpRequested = new Dictionary<Control, List<Delegate>>();
        private Dictionary<Control, List<HelpIgnoreResctrictionsEnum>> restrictionsToIgnore = new Dictionary<Control, List<HelpIgnoreResctrictionsEnum>>();
        private HelpProvider helpProvider = new HelpProvider();
        /// <summary>
        /// The help file name according to the respective project
        /// </summary>
        public string HelpFile
        {
            get { return HelpFilesByProject[_project];}
            set { 
                HelpFilesByProject[_project] = value;
                helpProvider.HelpNamespace = value;
            }
		}
        
        /// <summary>
        /// Sets the Help Context Id to the control
        /// </summary>
        /// <param name="ctrl">Control to set the help id</param>
        /// <param name="HelpId">Help Id index</param>
        public void SetHelpContextId(Control ctrl, int HelpId)
        {
            SetHelpContextId(ctrl, HelpId, HelpNavigator.TopicId);
        }

        /// <summary>
        /// Sets the Help Context Id to the control
        /// </summary>
        /// <param name="ctrl">Control to set the help id</param>
        /// <param name="HelpId">Help Id index</param>
        /// <param name="hNavigator">One of the HelpNavigator values to set</param>
        public void SetHelpContextId(Control ctrl, int HelpId, HelpNavigator hNavigator)
        {
            //Fix bug 245
            helpProvider.SetHelpKeyword(ctrl, HelpId.ToString());
            SetHelpNavigator(ctrl, hNavigator);
        }
        /// <summary>
        /// Returns the Help Id key from the control
        /// </summary>
        /// <param name="ctrl">Control to search the help id</param>
        /// <returns></returns>
        public int GetHelpContextId(Control ctrl)
        {
            return int.Parse(helpProvider.GetHelpKeyword(ctrl));
        }

        /// <summary>
        /// Sets the Help Navigator value to the control
        /// </summary>
        /// <param name="ctrl">Control to set the help navigator</param>
        /// <param name="hNavigator">One of the HelpNavigator values</param>
        public void SetHelpNavigator(Control ctrl, HelpNavigator hNavigator)
        {
            RestoreHelpEventHandler(ctrl);
            helpProvider.SetHelpNavigator(ctrl, hNavigator);
            PatchHelpEventHandler(ctrl);
        }

        /// <summary>
        /// It will clean the internal dictionaries from old references of controls alreay disposed.
        /// </summary>
        private void CleanDeadReferences()
        {
            try
            {
                List<Control> toClean = new List<Control>();
                foreach (Control ctrl in PatchedHelpRequested.Keys)
                {
                    if (ctrl.IsDisposed)
                        toClean.Add(ctrl);
                }
                foreach (Control ctrl in toClean)
                {
                    PatchedHelpRequested.Remove(ctrl);
                }
            }
            catch { }
        }


        /// <summary>
        /// Replace the HelpRequested event handler placed by the helpProvider with 
        /// a custom event handler so we can catch when the user is requesting help.
        /// </summary>
        /// <param name="ctrl">The source control.</param>
        private void PatchHelpEventHandler(Control ctrl)
        {
            CleanDeadReferences();

            if (PatchedHelpRequested.ContainsKey(ctrl))
                throw new InvalidOperationException(HelpRequestedEvent + " event for this control has been previously patched: '" + ctrl.Name + "'");

            Delegate[] EventDelegates = UpgradeHelpers.VB6.Gui.ContainerHelper.GetEventSubscribers(ctrl, HelpRequestedEvent);

            if (EventDelegates != null)
            {
                EventInfo eInfo = typeof(Control).GetEvent(HelpRequestedEvent);
                if (eInfo == null)
                    throw new InvalidOperationException("Event info for event '" + HelpRequestedEvent + "' could not be found");

                PatchedHelpRequested.Add(ctrl, new List<Delegate>());

                foreach (Delegate del in EventDelegates)
                {
                    PatchedHelpRequested[ctrl].Add(del);
                    eInfo.RemoveEventHandler(ctrl, del);
                }

                ctrl.HelpRequested += new HelpEventHandler(Control_HelpRequested);
            }
        }

        /// <summary>
        /// Restore the HelpRequested event handler that was originally added by 
        /// the helpProvider if one was previously patched.
        /// </summary>
        /// <param name="ctrl">The source control.</param>
        private void RestoreHelpEventHandler(Control ctrl)
        {
            if (PatchedHelpRequested.ContainsKey(ctrl))
            {
                ctrl.HelpRequested -= new HelpEventHandler(Control_HelpRequested);

                EventInfo eInfo = typeof(Control).GetEvent(HelpRequestedEvent);
                if (eInfo == null)
                    throw new InvalidOperationException("Event info for event '" + HelpRequestedEvent + "' could not be found");

                foreach (Delegate del in PatchedHelpRequested[ctrl])
                {
                    eInfo.AddEventHandler(ctrl, del);
                }

                PatchedHelpRequested.Remove(ctrl);
            }
        }

        /// <summary>
        /// Custom event handler used to patch the HelpRequested event of 
        /// the controls controlled by  the help provider.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="hlpevent"></param>
        private void Control_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            Control ctrl = (Control)sender;

            //The MDIForms do not show help in VB6
            if ((ctrl is Form) && (((Form)ctrl).IsMdiContainer) 
                && ((!restrictionsToIgnore.ContainsKey(ctrl)) || (restrictionsToIgnore.ContainsKey(ctrl) && !restrictionsToIgnore[ctrl].Contains(HelpIgnoreResctrictionsEnum.MDIContainerRestriction))))
                return;

            if (PatchedHelpRequested.ContainsKey(ctrl) && ValidateHelpFile())
            {
                foreach (Delegate del in PatchedHelpRequested[ctrl])
                {
                    del.DynamicInvoke(new object[] { sender, hlpevent });
                }
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
