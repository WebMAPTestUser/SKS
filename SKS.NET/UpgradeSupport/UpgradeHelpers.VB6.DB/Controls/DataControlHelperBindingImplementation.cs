using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data.Common;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace UpgradeHelpers.VB6.DB.Controls
{
    public partial class DataControlHelper<T> :  IBindingList, IList, ICollection, IEnumerable, ITypedList, ICancelAddNew, ISupportInitializeNotification, ICurrencyManagerProvider
    {

		/// <summary>
		/// Gets the DataControlHelper's BindingSource.
		/// </summary>
        public BindingSource Source
        {
            get
            {
                return source;
            }
        }
		
		/// <summary>
		/// Gets the DataControlHelper's initialized BindingSource when in Design Mode.  Otherwise,
		/// returns the BindingSource without initializing it.
		/// </summary>
		internal void UpdateBindingSource()
		{
			if (source != null)
			{
				 //First disconnect handlers 
				//rvasquez testing UnBindDataSet(); //This is to remove handlers and any other settings
				source.DataSource =  Recordset;
                if (Recordset.Tables.Count > 0)
                {
				source.DataMember = Recordset.Tables[0].TableName;
			}
			}
			else
				source = new BindingSource(Recordset, Recordset.Tables[0].TableName);
		}
		
		/// <summary>
		/// Gets the DataControlHelper's initialized BindingSource when in Design Mode.  Otherwise,
		/// returns the BindingSource without initializing it.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BindingSource InitializedSource
        {
            get
            {
				if (DesignMode)
				{
					UpdateConnectionInfo();
					RefreshResultSet();
					source = new BindingSource(Recordset, Recordset.Tables[0].TableName);
				}
				return source;
            }
        }

        #region IBindingListView Members

        /// <summary>
        /// Invokes underlying BindingSource's ApplySort method if the source is 
        /// connected to a DataSource, does nothing otherwise.
        /// </summary>
        /// <param name="sorts"></param>
        public void ApplySort(ListSortDescriptionCollection sorts)
        {

            if (IsConnectionAvailable())
                ((IBindingListView)source).ApplySort(sorts);
        }

        /// <summary>
        /// Returns underlying BindingSource's Filter value if the source is 
        /// connected to a DataSource, returns empty string "" otherwise.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Filter
        {
            get
            {

                if (IsConnectionAvailable())
                {
                    return source.Filter;
                }
                else
                {
                    return "";
                }
            }
            set
            {

                if (IsConnectionAvailable())
                    ((IBindingListView)source).Filter = value;
            }
        }

        /// <summary>
        /// Calls underlying BindingSource's RemoveFilter method if the source is 
        /// connected to a DataSource, does nothing otherwise.
        /// </summary>
        public void RemoveFilter()
        {

            if (IsConnectionAvailable())
                ((IBindingListView)source).RemoveFilter();
        }

        /// <summary>
        /// Returns underlying BindingSource's SortDescriptions value if the source is 
        /// connected to a DataSource, returns null otherwise.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ListSortDescriptionCollection SortDescriptions
        {

            get
            {

                if (IsConnectionAvailable())
                    return ((IBindingListView)source).SortDescriptions;
                else
                    return null;

            }
        }

        /// <summary>
        /// Returns underlying BindingSource's SupportsAdvancedSorting value if the source is 
        /// connected to a DataSource, returns false by default.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SupportsAdvancedSorting
        {

            get
            {

                if (IsConnectionAvailable())
                    return ((IBindingListView)source).SupportsAdvancedSorting;
                else
                    return false;

            }
        }

        /// <summary>
        /// Returns underlying BindingSource's SupportsFiltering value if the source is 
        /// connected to a DataSource, returns true by default.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SupportsFiltering
        {

            get
            {

                if (IsConnectionAvailable())
                    return ((IBindingListView)source).SupportsFiltering;
                else
                    return true;
            }
        }

        #endregion

        #region IBindingList Members

        /// <summary>
        /// Calls underlying BindingSource's RemoveFilter method if the source is 
        /// connected to a DataSource, does nothing otherwise.
        /// </summary>
        /// <param name="property"></param>
        public void AddIndex(PropertyDescriptor property)
        {

            if (IsConnectionAvailable())
                ((IBindingList)source).AddIndex(property);
        }

        /// <summary>
        /// Calls underlying BindingSource's AddNew method if the source is 
        /// connected to a DataSource, returns null otherwise.
        /// </summary>
        /// <returns></returns>
        public object AddNew()
        {

            if (IsConnectionAvailable())
                return ((IBindingList)source).AddNew();
            else
                return null;
        }

        /// <summary>
        /// Returns underlying BindingSource's AllowEdit value if the source is 
        /// connected to a DataSource, returns true otherwise.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowEdit
        {
            get
            {

                if (IsConnectionAvailable())
                    return ((IBindingList)source).AllowEdit;
                else
                    return true;
            }
        }

        /// <summary>
        /// Returns underlying BindingSource's AllowNew value if the source is 
        /// connected to a DataSource, returns true otherwise.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowNew
        {
            get
            {

                if (IsConnectionAvailable())
                    return ((IBindingList)source).AllowNew;
                else
                    return true;
            }
        }

        /// <summary>
        /// Returns underlying BindingSource's AllowRemove value if the source is 
        /// connected to a DataSource, returns true otherwise.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowRemove
        {
            get
            {

                if (IsConnectionAvailable())
                    return ((IBindingList)source).AllowRemove;
                else
                    return false;
            }
        }

        /// <summary>
        /// Calls underlying BindingSource's ApplySort method if the source is 
        /// connected to a DataSource, does nothing otherwise.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="direction"></param>
        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {

            if (IsConnectionAvailable())
                ((IBindingList)source).ApplySort(property, direction);
        }

        /// <summary>
        /// Calls underlying BindingSource's Find method if the source is 
        /// connected to a DataSource, returns -1 otherwise.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public int Find(PropertyDescriptor property, object key)
        {

            if (IsConnectionAvailable())
                return ((IBindingList)source).Find(property, key);
            else
                return -1;
        }

        /// <summary>
        /// Returns underlying BindingSource's IsSorted value if the source is 
        /// connected to a DataSource, returns false otherwise.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSorted
        {
            get
            {

                if (IsConnectionAvailable())
                    return ((IBindingList)source).IsSorted;
                else
                    return false;
            }
        }

#pragma warning disable 0067
        /// <summary>
        /// ListChanged EventHandler is not supported for this component
        /// </summary>
        public event ListChangedEventHandler ListChanged;
#pragma warning restore 0067

        /// <summary>
        /// Calls underlying BindingSource's RemoveIndex method if the source is 
        /// connected to a DataSource, does nothing otherwise.
        /// </summary>
        /// <param name="property"></param>
        public void RemoveIndex(PropertyDescriptor property)
        {

            if (IsConnectionAvailable())
                ((IBindingList)source).RemoveIndex(property);
        }

        /// <summary>
        /// Calls underlying BindingSource's RemoveSort method if the source is 
        /// connected to a DataSource, does nothing otherwise.
        /// </summary>
        public void RemoveSort()
        {

            if (IsConnectionAvailable())
                ((IBindingList)source).RemoveSort();
        }

        /// <summary>
        /// Returns underlying BindingSource's SortDirection value if the source is 
        /// connected to a DataSource, returns ListSortDirection.Ascending otherwise.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ListSortDirection SortDirection
        {
            get
            {

                if (IsConnectionAvailable())
                    return ((IBindingList)source).SortDirection;
                else
                    return ListSortDirection.Ascending;
            }
        }

        /// <summary>
        /// Returns underlying BindingSource's SortProperty value if the source is 
        /// connected to a DataSource, returns null otherwise.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PropertyDescriptor SortProperty
        {
            get
            {

                if (IsConnectionAvailable())
                    return ((IBindingList)source).SortProperty;
                else
                    return null;
            }
        }

        /// <summary>
        /// Returns underlying BindingSource's SupportsChangeNotification value if the source is 
        /// connected to a DataSource, returns true otherwise.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SupportsChangeNotification
        {
            get
            {

                if (IsConnectionAvailable())
                    return ((IBindingList)source).SupportsChangeNotification;
                else
                    return true;
            }
        }

        /// <summary>
        /// Returns underlying BindingSource's SupportsSearching value if the source is 
        /// connected to a DataSource, returns true otherwise.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SupportsSearching
        {
            get
            {

                if (IsConnectionAvailable())
                    return ((IBindingList)source).SupportsSearching;
                else
                    return true;
            }
        }

        /// <summary>
        /// Returns underlying BindingSource's SupportsSorting value if the source is 
        /// connected to a DataSource, returns true otherwise.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SupportsSorting
        {
            get
            {

                if (IsConnectionAvailable())
                    return ((IBindingList)source).SupportsSorting;
                else
                    return true;
            }
        }

        #endregion

        #region IList Members

        /// <summary>
        /// Calls underlying BindingSource's Add method if the source is 
        /// connected to a DataSource, returns -1 otherwise.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Add(object value)
        {

            if (IsConnectionAvailable())
                return ((IList)source).Add(value);
            else
                return -1;
        }

        /// <summary>
        /// Calls underlying BindingSource's Add method if the source is 
        /// connected to a DataSource, does nothing otherwise.
        /// </summary>
        public void Clear()
        {

            if (IsConnectionAvailable())
                ((IList)source).Clear();
        }

        /// <summary>
        /// Calls underlying BindingSource's Contains method if the source is 
        /// connected to a DataSource, returns false otherwise.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(object value)
        {

            if (IsConnectionAvailable())
                return ((IList)source).Contains(value);
            else
                return false;
        }

        /// <summary>
        /// Calls underlying BindingSource's IndexOf method if the source is 
        /// connected to a DataSource, returns -1 otherwise.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(object value)
        {

            if (IsConnectionAvailable())
                return ((IList)source).IndexOf(value);
            else
                return -1;
        }

        /// <summary>
        /// Calls underlying BindingSource's Insert method if the source is 
        /// connected to a DataSource, does nothing otherwise.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Insert(int index, object value)
        {

            if (IsConnectionAvailable())
                ((IList)source).Insert(index, value);
        }

        /// <summary>
        /// Returns underlying BindingSource's IsFixedSize value if the source is 
        /// connected to a DataSource, returns true otherwise.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsFixedSize
        {
            get
            {

                if (IsConnectionAvailable())
                    return ((IList)source).IsFixedSize;
                else
                    return true;
            }
        }

        /// <summary>
        /// Returns underlying BindingSource's IsReadOnly value if the source is 
        /// connected to a DataSource, returns false otherwise.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsReadOnly
        {
            get
            {

                if (IsConnectionAvailable())
                    return ((IList)source).IsReadOnly;
                else
                    return false;
            }
        }

        /// <summary>
        /// Calls underlying BindingSource's Remove method if the source is 
        /// connected to a DataSource, does nothing otherwise.
        /// </summary>
        /// <param name="value"></param>
        public void Remove(object value)
        {

            if (IsConnectionAvailable())
                ((IList)source).Remove(value);
        }

        /// <summary>
        /// Calls underlying BindingSource's RemoveAt method if the source is 
        /// connected to a DataSource, does nothing otherwise.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {

            if (IsConnectionAvailable())
                ((IList)source).RemoveAt(index);
        }

        /// <summary>
        /// Gets and sets underlying BindingSource's this[] property if the source is 
        /// connected to a DataSource.  Returns null otherwise.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object this[int index]
        {
            get
            {

                if (IsConnectionAvailable())
                    return ((IList)source)[index];
                else
                    return null;
            }
            set
            {

                if (IsConnectionAvailable())
                    ((IList)source)[index] = value;
            }
        }

        #endregion

        #region ICollection Members

        /// <summary>
        /// Calls underlying BindingSource's CopyTo method if the source is 
        /// connected to a DataSource, does nothing otherwise.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {

            if (IsConnectionAvailable())
                ((ICollection)source).CopyTo(array, index);
        }

        /// <summary>
        /// Returns underlying BindingSource's Count value if the source is 
        /// connected to a DataSource, returns 0 otherwise.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Count
        {
            get
            {

                if (IsConnectionAvailable())
                    return ((IList)source).Count;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Returns underlying BindingSource's IsSynchronized value if the source is 
        /// connected to a DataSource, returns false otherwise.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSynchronized
        {
            get
            {

                if (IsConnectionAvailable())
                    return ((IList)source).IsSynchronized;
                else
                    return false;
            }
        }

        /// <summary>
        /// Returns underlying BindingSource's SyncRoot value if the source is 
        /// connected to a DataSource, returns null otherwise.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SyncRoot
        {
            get
            {

                if (IsConnectionAvailable())
                    return ((IList)source).SyncRoot;
                else
                    return null;
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Calls underlying BindingSource's GetEnumerator method if the source is 
        /// connected to a DataSource, returns null otherwise.
        /// </summary>
        /// <returns></returns>
        public System.Collections.IEnumerator GetEnumerator()
        {

            if (IsConnectionAvailable())
                return ((IList)source).GetEnumerator();
            else
                return null;
        }

        #endregion

        #region ITypedList Members

        /// <summary>
        /// Calls underlying BindingSource's GetItemProperties method if the source is 
        /// connected to a DataSource, returns null otherwise.
        /// </summary>
        /// <param name="listAccessors"></param>
        /// <returns></returns>
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {

            if (IsConnectionAvailable())
                return ((ITypedList)InitializedSource).GetItemProperties(listAccessors);
            else
                return null;
        }

        /// <summary>
        /// Calls underlying BindingSource's GetListName method if the source is 
        /// connected to a DataSource, returns null otherwise.
        /// </summary>
        /// <param name="listAccessors"></param>
        /// <returns></returns>
        public string GetListName(PropertyDescriptor[] listAccessors)
        {

            if (IsConnectionAvailable())
                return ((ITypedList)source).GetListName(listAccessors);
            else
                return null;
        }

        #endregion

        #region ISupportInitializeNotification Members

#pragma warning disable 0067
        /// <summary>
        /// Initialized EventHandler is not supported for this component
        /// </summary>
        public event EventHandler Initialized;
#pragma warning restore 0067

        /// <summary>
        /// Returns underlying BindingSource's IsInitialized value if the source is 
        /// connected to a DataSource, returns false otherwise.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsInitialized
        {
            get
            {

                if (IsConnectionAvailable())
                    return ((ISupportInitializeNotification)source).IsInitialized;
                else
                    return false;
            }
        }

        #endregion

        #region ICurrencyManagerProvider Members

        /// <summary>
        /// Returns underlying BindingSource's CurrencyManager value if the source is 
        /// connected to a DataSource, returns null otherwise.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CurrencyManager CurrencyManager
        {
            get
            {

                if (IsConnectionAvailable())
                    return source.CurrencyManager;
                else
                    return null;
            }
        }

        /// <summary>
        /// Calls underlying BindingSource's GetRelatedCurrencyManager method if the source is 
        /// connected to a DataSource, returns null otherwise.
        /// </summary>
        /// <param name="dataMember"></param>
        /// <returns></returns>
        public CurrencyManager GetRelatedCurrencyManager(string dataMember)
        {

            if (IsConnectionAvailable())
                return ((ICurrencyManagerProvider)source).GetRelatedCurrencyManager(dataMember);
            else

                return null;
        }

        #endregion


        #region ICancelAddNew Members

        /// <summary>
        /// Calls underlying BindingSource's CancelNew method if the source is 
        /// connected to a DataSource, does nothing otherwise.
        /// </summary>
        /// <param name="itemIndex"></param>
        public void CancelNew(int itemIndex)
        {

            if (IsConnectionAvailable())
                ((ICancelAddNew)source).CancelNew(itemIndex);
        }

        /// <summary>
        /// Calls underlying BindingSource's EndNew method if the source is 
        /// connected to a DataSource, does nothing otherwise.
        /// </summary>
        /// <param name="itemIndex"></param>
        public void EndNew(int itemIndex)
        {

            if (IsConnectionAvailable())
                ((ICancelAddNew)source).EndNew(itemIndex);
        }

        #endregion

        private bool _isConnectionAvailable = false;

        /// <summary>
        /// Checks whether the properties required to establish a connection properly
        /// have already been set.
        /// </summary>
        /// <returns></returns>
        protected bool IsConnectionAvailable()
        {

            bool result = true;
            if (string.IsNullOrEmpty(ConnectionString) || string.IsNullOrEmpty(RecordSource) || string.IsNullOrEmpty(FactoryName))
            {
                _isConnectionAvailable = false;
                result = false;
            }
            if (!_isConnectionAvailable)
            {
                if (result)
                {
                    if (!Recordset.Opened)
                    {
                        InitRecordset();
                        Recordset.Open();
                    }
                    if (!Recordset.Opened || Recordset.Tables.Count <= 0)
                        result = false;
                }
                _isConnectionAvailable = result;
            }
            return _isConnectionAvailable;

        }

        /// <summary>
        /// Gets and sets underlying BindingSource's Sort property if the source is 
        /// connected to a DataSource.  Returns empty string "" otherwise.
        /// </summary>
        [Browsable(true),Category("General Configuration"),Description("The Sort property is a case-sensitive string that specifies the column names used to sort the rows, along with the sort direction. Columns are sorted ascending by default. Multiple columns can be separated by commas, such as \"State, ZipCode DESC\"")]
        public string Sort
        {
            get
            {
                if (IsConnectionAvailable())
                    return source.Sort;
                else
                {
                    return "";
                }
            }
            set
            {
                if (IsConnectionAvailable())
                {
                    source.Sort = value;
                }
            }
        }
    }
}
