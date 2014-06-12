using System;
using System.Collections.Generic;
using System.Text;

namespace UpgradeHelpers.VB6.DB.ADO.Events
{
    /// <summary>
    /// Delegate to handle the EndOfRecordset event.
    /// </summary>
    /// <param name="eventSender">The object which rises the event.</param>
    /// <param name="e">The arguments of the event.</param>
    public delegate void EndOfRecordsetEventHandler(Object eventSender, EndOfRecordsetEventArgs e);
    /// <summary>
    /// Delegate to handle the FieldChangeEvent event.
    /// </summary>
    /// <param name="eventSender">The object which rises the event</param>
    /// <param name="e">The arguments of the event</param>
    public delegate void FieldChangeEventHandler(Object eventSender, FieldChangeEventArgs e);
    /// <summary>
    /// Delegate to handle the FieldChangeCompleteEvent event.
    /// </summary>
    /// <param name="eventSender">The object which rises the event.</param>
    /// <param name="e">The arguments of the event</param>
    public delegate void FieldChangeCompleteEventHandler(Object eventSender, FieldChangeCompleteEventArgs e);
    /// <summary>
    /// Delegate to handle the RecordChangeEvent event.
    /// </summary>
    /// <param name="eventSender">The object which rises the event.</param>
    /// <param name="e">The arguments of the event</param>
    public delegate void RecordChangeEventHandler(Object eventSender, RecordChangeEventArgs e);
    /// <summary>
    /// Delegate to handle the RecordChangeCompleteEvent event.
    /// </summary>
    /// <param name="eventSender">The object which rises the event.</param>
    /// <param name="e">The arguments of the event</param>
    public delegate void RecordChangeCompleteEventHandler(Object eventSender, RecordChangeCompleteEventArgs e);
    /// <summary>
    /// Delegate to handle the RecordSetChangeEvent event.
    /// </summary>
    /// <param name="eventSender">The object which rises the event.</param>
    /// <param name="e">The arguments of the event.</param>
    public delegate void RecordSetChangeEventHandler(Object eventSender, RecordSetChangeEventArgs e);
    /// <summary>
    /// Delegate to handle the RecordSetChangeCompleteEvent event.
    /// </summary>
    /// <param name="eventSender">The object which rises the event.</param>
    /// <param name="e">The arguments of the event.</param>
    public delegate void RecordSetChangeCompleteEventHandler(Object eventSender, RecordSetChangeCompleteEventArgs e);
    /// <summary>
    /// Delegate to handle the MoveEvent event.
    /// </summary>
    /// <param name="eventSender">The object which rises the event.</param>
    /// <param name="e">The arguments of the event.</param>
    public delegate void MoveEventHandler(Object eventSender, MoveEventArgs e);
    /// <summary>
    /// Delegate to handle the MoveCompleteEvent event.
    /// </summary>
    /// <param name="eventSender">The object which rises the event.</param>
    /// <param name="e">The arguments of the event.</param>
    public delegate void MoveCompleteEventHandler(Object eventSender, MoveCompleteEventArgs e);

    /// <summary>
    /// The EventReasonEnum enumeration classifies the reason why an event is being raised.
    /// </summary>
    public enum EventReasonEnum
    {
        /// <summary>
        /// Add New
        /// </summary>
        adRsnAddNew = 1, 
        /// <summary>
        /// Delete
        /// </summary>
        adRsnDelete, 
        /// <summary>
        /// Update
        /// </summary>
        adRsnUpdate, 
        /// <summary>
        /// Undo update
        /// </summary>
        adRsnUndoUpdate, 
        /// <summary>
        /// Undo add new
        /// </summary>
        adRsnUndoAddNew,
        /// <summary>
        /// Undo delete
        /// </summary>
        adRsnUndoDelete, 
        /// <summary>
        /// Requery
        /// </summary>
        adRsnRequery,
        /// <summary>
        /// Resynch
        /// </summary>
        adRsnResynch, 
        /// <summary>
        /// Close
        /// </summary>
        adRsnClose, 
        /// <summary>
        /// Move
        /// </summary>
        adRsnMove, 
        /// <summary>
        /// First Change
        /// </summary>
        adRsnFirstChange, 
        /// <summary>
        /// Move first
        /// </summary>
        adRsnMoveFirst, 
        /// <summary>
        /// Move next
        /// </summary>
        adRsnMoveNext, 
        /// <summary>
        /// Move previous
        /// </summary>
        adRsnMovePrevious, 
        /// <summary>
        /// Move Last
        /// </summary>
        adRsnMoveLast
    }

    /// <summary>
    /// The EventStatusEnum enumeration classifies the status of an event.
    /// </summary>
    public enum EventStatusEnum 
    { 
        /// <summary>
        /// Status Ok
        /// </summary>
        adStatusOK = 1, 
        /// <summary>
        /// Errors Ocurred
        /// </summary>
        adStatusErrorsOccurred, 
        /// <summary>
        /// Can't Deny
        /// </summary>
        adStatusCantDeny, 
        /// <summary>
        /// Cancel
        /// </summary>
        adStatusCancel, 
        /// <summary>
        /// Unwanted Event
        /// </summary>
        adStatusUnwantedEvent 
    }

    /// <summary>
    /// Base class for the ADODB events helpers.
    /// </summary>
    public class BaseAdoEventArgs : EventArgs
    {
        /// <summary>
        /// private member which declares an EventStatusEnum instance.
        /// </summary>
        private EventStatusEnum adStatus;

        /// <summary>
        /// Gets and sets the event status (EventStatusEnum).
        /// </summary>
        public EventStatusEnum Status
        {
            get
            {
                return adStatus;
            }
            set
            {
                adStatus = value;
            }
        }

        /// <summary>
        /// private member to collect error information.
        /// </summary>
        private string[] errors;

        /// <summary>
        /// Gets the errors array
        /// </summary>
        public string[] Errors
        {
            get { return errors; }
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="status">The status of the event.</param>
        protected BaseAdoEventArgs(EventStatusEnum status)
            : this(status, new string[] { })
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="status">The status of the event.</param>
        /// <param name="errors">The errors on the operation.</param>
        protected BaseAdoEventArgs(EventStatusEnum status, string[] errors)
        {
            this.adStatus = status;
            this.errors = errors;
        }
    }

    /// <summary>
    /// Arguments class for the EndOfRecordsetEvent.
    /// </summary>
    public class EndOfRecordsetEventArgs : BaseAdoEventArgs
    {
        /// <summary>
        /// private member to store moreDataFlag
        /// </summary>
        private bool moreData;

        /// <summary>
        /// Gets and Sets the more data flag
        /// </summary>
        public bool MoreData
        {
            get { return moreData; }
            set { moreData = value; }
        }

        /// <summary>
        /// Creates a new EndOfRecordsetEventArgs instance.
        /// </summary>
        /// <param name="moreData">Indicates if there are more data to get.</param>
        /// <param name="status">The status of the event</param>
        public EndOfRecordsetEventArgs(bool moreData, EventStatusEnum status)
            : base(status)
        {
            this.moreData = moreData;
        }
    }

    /// <summary>
    /// Arguments class for the FieldChangeEvent.
    /// </summary>
    public class FieldChangeEventArgs : BaseAdoEventArgs
    {
        /// <summary>
        /// private member to store number of fields affected value.
        /// </summary>
        private int numberOfFields;
        /// <summary>
        /// private member to store field's values before applying a change.
        /// </summary>
        private Object[] fieldValues;
        /// <summary>
        /// Creates a new FieldChangeEventArgs instance.
        /// </summary>
        /// <param name="numberOfFields">The number of fields affected.</param>
        /// <param name="fieldValues">The field's values before the change.</param>
        /// <param name="errors">The errors ocurred during the operation.</param>
        /// <param name="status">The status of the event</param>
        protected FieldChangeEventArgs(int numberOfFields, object[] fieldValues, string[] errors, EventStatusEnum status)
            : base(status, errors)
        {
            this.numberOfFields = numberOfFields;
            this.fieldValues = fieldValues;
        }
        /// <summary>
        /// Creates a new FieldChangeEventArgs instance.
        /// </summary>
        /// <param name="numberOfFields">The number of fields affected.</param>
        /// <param name="fieldValues">The field's values before the change.</param>
        /// <param name="status">The status of the event.</param>
        public FieldChangeEventArgs(int numberOfFields, object[] fieldValues, EventStatusEnum status)
            : this(numberOfFields, fieldValues, null, status)
        {
        }
        /// <summary>
        /// Gets the number of fields affected.
        /// </summary>
        public int NumberOfFields
        {
            get { return numberOfFields; }
        }
        /// <summary>
        /// The values of the fields affected.
        /// </summary>
        public object[] FieldValues
        {
            get { return fieldValues; }
        }
    }
    /// <summary>
    /// Arguments class for the FieldChangeCompleteEvent.
    /// </summary>
    public class FieldChangeCompleteEventArgs : FieldChangeEventArgs
    {
        /// <summary>
        /// Creates a new FieldChangeCompleteEventArgs instance.
        /// </summary>
        /// <param name="numberOfFields">The number of fields affected.</param>
        /// <param name="fieldValues">The field's values after the change.</param>
        /// <param name="errors">The errors ocurred during the operation.</param>
        /// <param name="status">The status of the event</param>
        public FieldChangeCompleteEventArgs(int numberOfFields, object[] fieldValues, string[] errors, EventStatusEnum status)
            : base(numberOfFields, fieldValues, errors, status)
        {
        }
    }
    /// <summary>
    /// Arguments class for the RecordChangeEvent.
    /// </summary>
    public class RecordChangeEventArgs : BaseAdoEventArgs
    {
        private EventReasonEnum reason;
        private int numberOfRecords;
        /// <summary>
        /// Creates a new RecordChangeEventArgs instance.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="numberOfRecords">The number of fields affected.</param>
        /// <param name="errors">The errors ocurred during the operation.</param>
        /// <param name="status">The status of the event.</param>
        protected RecordChangeEventArgs(EventReasonEnum reason, int numberOfRecords, string[] errors, EventStatusEnum status)
            : base(status, errors)
        {
            this.reason = reason;
            this.numberOfRecords = numberOfRecords;
        }
        /// <summary>
        /// Creates a new RecordChangeEventArgs instance.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="numberOfRecords">The number of fields affected.</param>
        /// <param name="status">The status of the event.</param>
        public RecordChangeEventArgs(EventReasonEnum reason, int numberOfRecords, EventStatusEnum status)
            : this(reason, numberOfRecords, null, status)
        {
        }
        /// <summary>
        /// Gets the reason of the event.
        /// </summary>
        public EventReasonEnum Reason
        {
            get { return reason; }
        }
        /// <summary>
        /// Gets the number of records affected.
        /// </summary>
        public int NumberOfRecords
        {
            get { return numberOfRecords; }
        }
    }
    /// <summary>
    /// Arguments class for the RecordChangeCompleteEvent.
    /// </summary>
    public class RecordChangeCompleteEventArgs : RecordChangeEventArgs
    {
        /// <summary>
        /// Creates a new RecordChangeCompleteEventArgs instance.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="numberOfRecords">The number of fields affected.</param>
        /// <param name="errors">The errors ocurred during the operation.</param>
        /// <param name="status">The status of the event.</param>
        public RecordChangeCompleteEventArgs(EventReasonEnum reason, int numberOfRecords, string[] errors, EventStatusEnum status)
            : base(reason, numberOfRecords, errors, status)
        {
        }
    }
    /// <summary>
    /// Arguments class for the RecordSetChangeEvent.
    /// </summary>
    public class RecordSetChangeEventArgs : BaseAdoEventArgs
    {
        private EventReasonEnum reason;
        /// <summary>
        /// Creates a new RecordSetChangeEventArgs instance.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="errors">The errors ocurred during the operation.</param>
        /// <param name="status">The status of the event.</param>
        protected RecordSetChangeEventArgs(EventReasonEnum reason, string[] errors, EventStatusEnum status)
            : base(status, errors)
        {
            this.reason = reason;
        }
        /// <summary>
        /// Creates a new RecordSetChangeEventArgs instance.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="status">The status of the event.</param>
        public RecordSetChangeEventArgs(EventReasonEnum reason, EventStatusEnum status)
            : this(reason, null, status)
        {
        }
        /// <summary>
        /// Gets the reason of the event.
        /// </summary>
        public EventReasonEnum Reason
        {
            get { return reason; }
        }
    }
    /// <summary>
    /// Arguments class for the RecordSetChangeCompleteEvent.
    /// </summary>
    public class RecordSetChangeCompleteEventArgs : RecordSetChangeEventArgs
    {
        /// <summary>
        /// Creates a new RecordSetChangeCompleteEventArgs instance.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="errors">The errors ocurred during the operation.</param>
        /// <param name="status">The status of the event.</param>
        public RecordSetChangeCompleteEventArgs(EventReasonEnum reason, string[] errors, EventStatusEnum status)
            : base(reason, errors, status)
        {
        }
    }
    /// <summary>
    /// Arguments class for the MoveEvent.
    /// </summary>
    public class MoveEventArgs : BaseAdoEventArgs
    {
        private EventReasonEnum reason;
        /// <summary>
        /// Creates a new MoveEventArgs instance.
        /// </summary>
        /// <param name="reason">The reason of the move.</param>
        /// <param name="errors">The errors ocurred during the operation.</param>
        /// <param name="status">The status of the event.</param>
        protected MoveEventArgs(EventReasonEnum reason, string[] errors, EventStatusEnum status)
            : base(status, errors)
        {
            this.reason = reason;
        }
        /// <summary>
        /// Creates a new MoveEventArgs instance.
        /// </summary>
        /// <param name="reason">The reason of the move.</param>
        /// <param name="status">The status of the event.</param>
        public MoveEventArgs(EventReasonEnum reason, EventStatusEnum status)
            : this(reason, null, status)
        {
        }
        /// <summary>
        /// Gets the reason of the event.
        /// </summary>
        public EventReasonEnum Reason
        {
            get { return reason; }
        }
    }
    /// <summary>
    /// Arguments class for the MoveCompleteEvent.
    /// </summary>
    public class MoveCompleteEventArgs : MoveEventArgs
    {
        /// <summary>
        /// Creates a new MoveCompleteEventArgs instance.
        /// </summary>
        /// <param name="reason">The reason of the move.</param>
        /// <param name="errors">The errors ocurred during the operation.</param>
        /// <param name="status">The status of the event.</param>
        public MoveCompleteEventArgs(EventReasonEnum reason, string[] errors, EventStatusEnum status)
            : base(reason, errors, status)
        {
        }
    }
}


