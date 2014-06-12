using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UpgradeHelpers.VB6.DB.Controls;
using UpgradeHelpers.VB6.DB.DAO;
#if DAOEvents
	UpgradeHelpers.VB6.DB.DAO.Events;
#endif
using System.ComponentModel.Design;

namespace UpgradeHelpers.VB6.DB.DAO
{
	/// <summary>
	/// This class implements functionality for a Data Control for DAO access (usually Jet Engine e.g MS Access)
	/// </summary>
	[ToolboxItem(true)]
	public partial class DAODataControlHelper : InternalDAODataControlHelper
	{
#if DAOEvents
		#region Events declarations
        /// <summary>
        /// Exposes the EndOfRecordsetEvent.
        /// </summary>
        [Browsable(true), Category("Data")]
        public event EndOfRecordsetEventHandler EndOfRecordset;
        /// <summary>
        /// Exposes the FieldChangeEvent.
        /// </summary>
        [Browsable(true), Category("Data")]
        public event FieldChangeEventHandler WillChangeField;
        /// <summary>
        /// Exposes the FieldChangeCompleteEvent.
        /// </summary>
        [Browsable(true), Category("Data")]
        public event FieldChangeCompleteEventHandler FieldChangeComplete;
        /// <summary>
        /// Exposes the RecordChangeEvent.
        /// </summary>
        [Browsable(true), Category("Data")]
        public event RecordChangeEventHandler WillChangeRecord;
        /// <summary>
        /// Exposes the RecordChangeCompleteEvent.
        /// </summary>
        [Browsable(true), Category("Data")]
        public event RecordChangeCompleteEventHandler RecordChangeComplete;
        /// <summary>
        /// Exposes the RecordSetChangeCompleteEvent.
        /// </summary>
        [Browsable(true), Category("Data")]
        public event RecordSetChangeEventHandler WillChangeRecordset;
        /// <summary>
        /// Exposes the RecordSetChangeCompleteEvent.
        /// </summary>
        [Browsable(true), Category("Data")]
        public event RecordSetChangeCompleteEventHandler RecordsetChangeComplete;
        /// <summary>
        /// Exposes the MoveEvent.
        /// </summary>
        [Browsable(true), Category("Data")]
        public event MoveEventHandler WillMove;
        /// <summary>
        /// Exposes the MoveCompleteEvent.
        /// </summary>
        [Browsable(true), Category("Data")]
        public event MoveCompleteEventHandler MoveComplete;
		#endregion
#endif

		/// <summary>
		/// Creates a new Data Control instance.
		/// </summary>
		public DAODataControlHelper()
		{
			InitializeComponent();
		}


		/// <summary>
		/// De initializes the underlying recordset object.
		/// </summary>
		protected override void DeInitRecordset()
		{
			if (OnInitialization || !IsConnectionAvailable())
			{
				return;
			}
			base.DeInitRecordset();
			if (Recordset != null)
			{
				Recordset.Validating -= new ValidatingEventHandler(Recordset_Validating);
				Recordset.AfterUpdate -= new EventHandler<DAORecordSetHelper.UpdateInfo>(Recordset_AfterUpdate);
			}
			// Future support of events 
#if DAOEvents
		    if (Recordset != null)
            {
                Recordset.WillChangeField -= new FieldChangeEventHandler(Recordset_WillChangeField);
                Recordset.WillChangeRecord -= new RecordChangeEventHandler(Recordset_WillChangeRecord);
                Recordset.WillChangeRecordset -= new RecordSetChangeEventHandler(Recordset_WillChangeRecordset);
                Recordset.WillMove -= new MoveEventHandler(Recordset_WillMove);
                Recordset.EndOfRecordset -= new EndOfRecordsetEventHandler(Recordset_EndOfRecordset);
                Recordset.FieldChangeComplete -= new FieldChangeCompleteEventHandler(Recordset_FieldChangeComplete);
                Recordset.MoveComplete -= new MoveCompleteEventHandler(Recordset_MoveComplete);
                Recordset.RecordChangeComplete -= new RecordChangeCompleteEventHandler(Recordset_RecordChangeComplete);
                Recordset.RecordsetChangeComplete -= new RecordSetChangeCompleteEventHandler(Recordset_RecordsetChangeComplete);    
            }
#endif
		}
		/// <summary>
		/// Initializes the underlying recordset object.
		/// </summary>
		protected override void InitRecordset()
		{
			base.InitRecordset();
			if (Recordset != null)
			{
				Recordset.Validating += new ValidatingEventHandler(Recordset_Validating);
				Recordset.AfterUpdate += new EventHandler<DAORecordSetHelper.UpdateInfo>(Recordset_AfterUpdate);
			}
			// Future support of events 
#if DAOEvents
            Recordset.LockType = LockType;
            Recordset.CursorLocation = CursorLocation;
            Recordset.WillChangeField += new FieldChangeEventHandler(Recordset_WillChangeField);
            Recordset.WillChangeRecord += new RecordChangeEventHandler(Recordset_WillChangeRecord);
            Recordset.WillChangeRecordset += new RecordSetChangeEventHandler(Recordset_WillChangeRecordset);
            Recordset.WillMove += new MoveEventHandler(Recordset_WillMove);
            Recordset.EndOfRecordset += new EndOfRecordsetEventHandler(Recordset_EndOfRecordset);
            Recordset.FieldChangeComplete += new FieldChangeCompleteEventHandler(Recordset_FieldChangeComplete);
            Recordset.MoveComplete += new MoveCompleteEventHandler(Recordset_MoveComplete);
            Recordset.RecordChangeComplete += new RecordChangeCompleteEventHandler(Recordset_RecordChangeComplete);
            Recordset.RecordsetChangeComplete += new RecordSetChangeCompleteEventHandler(Recordset_RecordsetChangeComplete);
#endif
		}
		void Recordset_AfterUpdate(object sender, DAORecordSetHelper.UpdateInfo e)
		{
			//Called with reflection because method is protected
			Source.GetType().GetMethod("OnListChanged", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(Source, new object[] { new ListChangedEventArgs(MatchChangeType(e.UpdateType), e.index) });

		}

		private ListChangedType MatchChangeType(DAORecordSetHelper.UpdateType updateType)
		{
			switch (updateType)
			{
				case DAORecordSetHelper.UpdateType.Added:
					return ListChangedType.ItemAdded;
				case DAORecordSetHelper.UpdateType.Deleted:
					return ListChangedType.ItemDeleted;
				case DAORecordSetHelper.UpdateType.Modified:
					return ListChangedType.ItemChanged;
			}
			throw new NotSupportedException();
		}
		/// <summary>
		/// Fires DAOControlHelper Validating event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="vArgs"></param>
		void Recordset_Validating(object sender, ValidatingEventArgs vArgs)
		{
			int Action = vArgs.Action;
			int Save = vArgs.Save;
			OnValidating(ref Action, ref Save);
			vArgs.Action = Action;
			vArgs.Save = Save;
		}
		/// <summary>
		/// Ends the initialization process.
		/// </summary>
		public override void EndInit()
		{
			OnInitialization = false;
			base.EndInit();
		}
		/// <summary>
		/// Public ValidatingEvent.
		/// </summary>
		public new event ValidatingEventHandler Validating = null;
		/// <summary>
		/// Raises the validating event.
		/// </summary>
		/// <param name="Action">The action that raises the event.</param>
		/// <param name="Save">Determines if needs to save.</param>
		protected override void OnValidating(ref int Action, ref int Save)
		{
			if (Validating != null)
			{
				ValidatingEventArgs vArgs = new ValidatingEventArgs(Action, Save);
				Validating(this, vArgs);
				Action = vArgs.Action;
				Save = vArgs.Save;
			}
		}
		/// <summary>
		/// Delegate to wire the ValidatingEvent.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public new delegate void ValidatingEventHandler(object sender, ValidatingEventArgs e);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="txtNotes"></param>
		/// <param name="p"></param>
		public void BindControl(TextBox txtNotes, string p)
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// 
		/// </summary>
		public int EditMode
		{
			get
			{
				return (int)Recordset.EditMode;
			}
		}
#if DAOEvents
		#region events
        /// <summary>
        /// Handles the MoveComplete event of the underlying Recordset, triggering the control's  MoveComplete event.
        /// </summary>
        /// <param name="eventSender">The object which rises the event.</param>
        /// <param name="e">The arguments of the event.</param>
        void Recordset_MoveComplete(object eventSender, MoveCompleteEventArgs e)
        {
            EventStatusEnum status = e.Status;
            OnMoveComplete(e.Reason, ref status, e.Errors);
            e.Status = status;
        }
        /// <summary>
        /// Handles the WillMove event of the underlying Recordset, triggering the control's  WillMove event.
        /// </summary>
        /// <param name="eventSender">The object which rises the event.</param>
        /// <param name="e">The arguments of the event.</param>
        void Recordset_WillMove(object eventSender, MoveEventArgs e)
        {
            EventStatusEnum status = e.Status;
            OnWillMove(e.Reason, ref status);
            e.Status = status;            
        }
        /// <summary>
        /// Handles the WillChangeRecordset event of the underlying Recordset, triggering the control's  WillChangeRecordset event.
        /// </summary>
        /// <param name="eventSender">The object which rises the event.</param>
        /// <param name="e">The arguments of the event.</param>
        void Recordset_WillChangeRecordset(object eventSender, RecordSetChangeEventArgs e)
        {
            EventStatusEnum status = e.Status;
            OnWillChangeRecordset(e.Reason, ref status);
            e.Status = status;
        }
        /// <summary>
        /// Handles the RecordsetChangeComplete event of the underlying Recordset, triggering the control's RecordsetChangeComplete event.
        /// </summary>
        /// <param name="eventSender">The object which rises the event.</param>
        /// <param name="e">The arguments of the event.</param>
        void Recordset_RecordsetChangeComplete(object eventSender, RecordSetChangeCompleteEventArgs e)
        {
            EventStatusEnum status = e.Status;
            OnRecordsetChangeComplete(e.Reason, ref status, e.Errors);
            e.Status = status;
        }
        /// <summary>
        /// Handles the RecordChangeComplete event of the underlying Recordset, triggering the control's RecordChangeComplete event.
        /// </summary>
        /// <param name="eventSender">The object which rises the event.</param>
        /// <param name="e">The arguments of the event.</param>
        void Recordset_RecordChangeComplete(object eventSender, RecordChangeCompleteEventArgs e)
        {
            EventStatusEnum status = e.Status;
            OnRecordChangeComplete(e.Reason, ref status, e.NumberOfRecords, e.Errors);
            e.Status = status;
        }
        /// <summary>
        /// Handles the FieldChangeComplete event of the underlying Recordset, triggering the control's FieldChangeComplete event.
        /// </summary>
        /// <param name="eventSender">The object which rises the event.</param>
        /// <param name="e">The arguments of the event.</param>
        void Recordset_FieldChangeComplete(object eventSender, FieldChangeCompleteEventArgs e)
        {
            EventStatusEnum status = e.Status;
            OnFieldChangeComplete(ref status, e.NumberOfFields, e.FieldValues, e.Errors);
            e.Status = status;
        }
        /// <summary>
        /// Handles the EndOfRecordset event of the underlying Recordset, triggering the control's EndOfRecordset event.
        /// </summary>
        /// <param name="eventSender">The object which rises the event.</param>
        /// <param name="e">The arguments of the event.</param>
        void Recordset_EndOfRecordset(object eventSender, EndOfRecordsetEventArgs e)
        {
            bool moreData = e.MoreData;
            OnEndOfRecordset(ref moreData, e.Status);
            e.MoreData = moreData;
        }
        /// <summary>
        /// Handles the WillChangeRecord event of the underlying Recordset, triggering the control's WillChangeRecord event.
        /// </summary>
        /// <param name="eventSender">The object which rises the event.</param>
        /// <param name="e">The arguments of the event.</param>
        void Recordset_WillChangeRecord(object eventSender, RecordChangeEventArgs e)
        {
            EventStatusEnum status = e.Status;
            OnWillChangeRecord(e.Reason, ref status, e.NumberOfRecords);
            e.Status = status;
        }
        /// <summary>
        /// Handles the WillChangeField event of the underlying Recordset, triggering the control's WillChangeField event.
        /// </summary>
        /// <param name="eventSender">The object which rises the event.</param>
        /// <param name="e">The arguments of the event.</param>
        void Recordset_WillChangeField(object eventSender, FieldChangeEventArgs e)
        {
            EventStatusEnum status = e.Status;
            OnWillChangeField(ref status, e.NumberOfFields, e.FieldValues); 
            e.Status = status;            
        }
        /// <summary>
        /// The EndOfRecordset event is called when there is an attempt to move to a row past the end of the Recordset.
        /// </summary>
        /// <param name="moredata">Bool value that indicates if more data have been added to the ADORecordsetHelper.</param>
        /// <param name="status">A EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        protected void OnEndOfRecordset(ref bool moredata, EventStatusEnum status)
        {
            if (EndOfRecordset != null)
            {
                EndOfRecordsetEventArgs eor = new EndOfRecordsetEventArgs(moredata, status);
                EndOfRecordset(this, eor);
                moredata = eor.MoreData;
            }
        }
        /// <summary>
        /// The WillChangeField event is called before a pending operation changes the value of one or more Field objects in the ADORecordsetHelper.
        /// </summary>
        /// <param name="status">A EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        /// <param name="numfields">Indicates the number of fields objects contained in the “fieldvalues” array.</param>
        /// <param name="fieldvalues">Array with the new values of the modified fields.</param>
        protected void OnWillChangeField(ref EventStatusEnum status, int numfields, object[] fieldvalues)
        {
            if (WillChangeField != null)
            {
                FieldChangeEventArgs args = new FieldChangeEventArgs(numfields, fieldvalues, status);
                WillChangeField(this, args);
                status = args.Status;
            }
        }
        /// <summary>
        /// The FieldChangeComplete event is called after the value of one or more Field objects has changed.
        /// </summary>
        /// <param name="status">A EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        /// <param name="numfields">Indicates the number of fields objects contained in the “fieldvalues” array.</param>
        /// <param name="fieldvalues">Array with the new values of the modified fields.</param>
        /// <param name="errors">Array containing all the errors occurred during the field change.</param>
        protected void OnFieldChangeComplete(ref EventStatusEnum status, int numfields, object[] fieldvalues, string[] errors)
        {
            if (FieldChangeComplete != null)
            {
                FieldChangeCompleteEventArgs args = new FieldChangeCompleteEventArgs(numfields, fieldvalues, errors, status);
                FieldChangeComplete(this, args);
                status = args.Status;
            }
        }
        /// <summary>
        /// The OnWillChangeRecord event is called before one or more records (rows) in the Recordset change.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="status">A EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        /// <param name="numRecords">Value indicating the number of records changed (affected).</param>
        protected void OnWillChangeRecord(EventReasonEnum reason, ref EventStatusEnum status, int numRecords)
        {
            if (WillChangeRecord != null)
            {
                RecordChangeEventArgs args = new RecordChangeEventArgs(reason, numRecords, status);
                WillChangeRecord(this, args);
                status = args.Status;
            }
        }
        /// <summary>
        /// OnRecordChangeComplete event is called after one or more records change.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="status">An EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        /// <param name="numRecords">Value indicating the number of records changed (affected).</param>
        /// <param name="errors">Array containing all the errors occurred during the field change.</param>
        protected void OnRecordChangeComplete(EventReasonEnum reason, ref EventStatusEnum status, int numRecords, string[] errors)
        {
            if (RecordChangeComplete != null)
            {
                RecordChangeCompleteEventArgs args = new RecordChangeCompleteEventArgs(reason, numRecords, errors, status);
                RecordChangeComplete(this, args);
                status = args.Status;
            }
        }
        /// <summary>
        /// OnWillChangeRecordset event is called before a pending operation changes the ADORecordsetHelper.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="status">A EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        protected void OnWillChangeRecordset(EventReasonEnum reason, ref EventStatusEnum status)
        {
            if (WillChangeRecordset != null)
            {
                RecordSetChangeEventArgs args = new RecordSetChangeEventArgs(reason, status);
                WillChangeRecordset(this, args);
                status = args.Status;
            }
        }
        /// <summary>
        /// OnRecordsetChangeComplete event is called after the ADORecordsetHelper has changed.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="status">A EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        /// <param name="errors">Array containing all the errors occurred during the field change.</param>
        protected void OnRecordsetChangeComplete(EventReasonEnum reason, ref EventStatusEnum status, string[] errors)
        {
            if (RecordsetChangeComplete != null)
            {
                RecordSetChangeCompleteEventArgs args = new RecordSetChangeCompleteEventArgs(reason, errors, status);
                RecordsetChangeComplete(this, args);
                status = args.Status;
            }
        }
        /// <summary>
        /// OnWillMove event is called before a pending operation changes the current position in the ADORecordsetHelper.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="status">A EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        protected void OnWillMove(EventReasonEnum reason, ref EventStatusEnum status)
        {
            if (WillMove != null)
            {
                MoveEventArgs args = new MoveEventArgs(reason, status);
                WillMove(this, args);
                status = args.Status;
            }
        }
        /// <summary>
        /// OnMoveComplete event is called after the current position in the ADORecordsetHelper changes.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="status">A EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        /// <param name="errors">Array containing all the errors occurred during the field change.</param>
        protected void OnMoveComplete(EventReasonEnum reason, ref EventStatusEnum status, string[] errors)
        {
            if (MoveComplete != null)
            {
                MoveCompleteEventArgs args = new MoveCompleteEventArgs(reason, errors, status);
                MoveComplete(this, args);
                status = args.Status;
            }
        }
        
		#endregion
#endif
	}
}

