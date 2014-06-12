using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace UpgradeHelpers.VB6.DB.RDO.Events
{
    /// <summary>
    /// Delegate to handle the RowStatusChangedEvent.
    /// </summary>
    /// <param name="eventSender">The object whic rises the event</param>
    /// <param name="e">The arguments of the event</param>
    public delegate void RowStatusChangedEventHandler(Object eventSender, EventArgs e);
    /// <summary>
    /// Delegate to handle the WillUpdateRowsEvent.
    /// </summary>
    /// <param name="eventSender">The object whic rises the event</param>
    /// <param name="e">The arguments of the event</param>
    public delegate void WillUpdateRowsEventHandler(Object eventSender, WillUpdateRowsEventArgs e);
    /// <summary>
    /// Delegate to handle the RowCurrencyChangeEvent.
    /// </summary>
    /// <param name="eventSender">The object which rises the event</param>
    /// <param name="e">The arguments of the event</param>
    public delegate void RowCurrencyChangeEventHandler(Object eventSender, EventArgs e);
    /// <summary>
    /// Delegate to handle the WillDissociateEvent.
    /// </summary>
    /// <param name="eventSender">The object whic rises the event</param>
    /// <param name="e">The arguments of the event</param>
    public delegate void WillDissociateEventHandler(Object eventSender, WillDissociateEventArgs e);
    /// <summary>
    /// Delegate to handle the WillAssociateEvent.
    /// </summary>
    /// <param name="eventSender">The object whic rises the event</param>
    /// <param name="e">The arguments of the event</param>
    public delegate void WillAssociateEventHandler(Object eventSender, WillAssociateEventArgs e);
    /// <summary>
    /// Delegate to handle the DissociateEvent.
    /// </summary>
    /// <param name="eventSender">The object whic rises the event</param>
    /// <param name="e">The arguments of the event</param>
    public delegate void DissociateEventHandler(Object eventSender, EventArgs e);
    /// <summary>
    /// Delegate to handle the AssociateEvent.
    /// </summary>
    /// <param name="eventSender">The object whic rises the event</param>
    /// <param name="e">The arguments of the event</param>
    public delegate void AssociateEventHandler(Object eventSender, EventArgs e);

    /// <summary>
    /// Arguments for the WillDissociateEvent.
    /// </summary>
    public class WillDissociateEventArgs : EventArgs
    {
        private bool cancel;
        /// <summary>
        /// Creates a new WillDissociateEventArgs instance.
        /// </summary>
        /// <param name="cancel">Determines if the event is cancelled</param>
        public WillDissociateEventArgs(bool cancel)
            : base()
        {
            this.cancel = cancel;
        }
        /// <summary>
        /// Gets and set the cancel flag.
        /// </summary>
        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }
    }
    /// <summary>
    /// Arguments for the WillAssociateEvent.
    /// </summary>
    public class WillAssociateEventArgs : WillDissociateEventArgs
    {
        private DbConnection connection;
        /// <summary>
        /// Creates a new WillDissociateEventArgs instance.
        /// </summary>
        /// <param name="connection">The connection to be associated</param>
        /// <param name="cancel">Determines if the event is cancelled</param>
        public WillAssociateEventArgs(DbConnection connection, bool cancel)
            : base(cancel)
        {
            this.connection = connection;
        }
        /// <summary>
        /// Gets the connection instance.
        /// </summary>
        public DbConnection Connection
        {
            get { return connection; }
        }
    }
    /// <summary>
    /// Arguments for the WillUpdateRowsEvent.
    /// </summary>
    public class WillUpdateRowsEventArgs : EventArgs
    {
        private int returncode;
        /// <summary>
        /// Gets the return code.
        /// </summary>
        public int Returncode
        {
            get { return returncode; }
        }
        /// <summary>
        /// Creates a new WillUpdateRowsEventArgs instance.
        /// </summary>
        /// <param name="returncode">The return code of the event</param>
        public WillUpdateRowsEventArgs(int returncode)
            : base()
        {
            this.returncode = returncode;
        }
    }
}
