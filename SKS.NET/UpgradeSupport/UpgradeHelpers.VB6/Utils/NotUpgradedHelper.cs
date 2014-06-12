using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace UpgradeHelpers.VB6.Utils
{

    /// <summary>
    /// The NotUpgradedHelper is a miscellaneous class to handle notifications for the not-upgraded members or statements.
    /// Each not-upgraded member generates a stub declaration in the target application.
    /// When one of these stub declarations is invoked the NotUpgradedHelper notifies the missing functionality to the application user.
    /// </summary>
    public class NotUpgradedHelper
    {

        private static bool performNotifications = true;
        private static bool reporting = false;

        private static string title = "Not-Upgraded Element";

        private static string message1 = 
            "The not-upgraded element: '";

        private static string message2 = 
            "' is being invoked.\n" +
            "The application behavior might be affected depending on how critical this element is.\n\n" + 
            "Do you want to ignore this issue and continue running the application?\n\n" +
            "[Yes]    = Ignore this occurrence and continue the program execution\n" +
            "[No]     = Stop the execution and debug\n" +
            "[Cancel] = Cancel the not-upgraded element notifications and ignore any potential behavior difference\n";

        /// <summary>
        /// Notifies the usage of a not-upgraded element to the user.
        /// <param name="NotUpgradedElementName">The name of the not-upgraded VB6 member.</param>
        /// </summary>
        public static void NotifyNotUpgradedElement(string NotUpgradedElementName)
        {
            if (performNotifications && !reporting)
            {
                reporting = true;
                DialogResult res = MessageBox.Show(message1 + NotUpgradedElementName + message2, title + NotUpgradedElementName, MessageBoxButtons.YesNoCancel);
                reporting = false;
                switch (res)
                {
                    case DialogResult.Yes:
                        // Do nothing
                        break;
                    case DialogResult.No:
                        System.Diagnostics.Debugger.Break();
                        break;
                    case DialogResult.Cancel:
                        performNotifications = false;
                        break;
                }
            }
        }
    }
}
