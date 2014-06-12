using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace UpgradeHelpers.VB6.Utils
{
    /// <summary>
    /// Implements a Dictionary containing as the Key a WeakReference. It facilitates that references
    /// inside this Dictionary could be released with any problem.
    /// </summary>
    public class WeakDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private static IEqualityComparer<InternalWeakReference> _comparer = new WeakKeyComparer<InternalWeakReference>();
        private Dictionary<InternalWeakReference, TValue> _dict = new Dictionary<InternalWeakReference, TValue>(_comparer);
        private long _lastGlobalMem;
        private int _lastDictCount;

        #region WeakDictionary<TKey, TValue> Members

        /// <summary>
        /// Adds an entry to the Dictionary.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <param name="value">The entry value.</param>
        private void AddWeakRef(TKey key, TValue value)
        {
            this.RemoveDeletedWeakRefs();
            _dict.Add(new InternalWeakReference(key), value);
        }

        /// <summary>
        /// Review and remove all the WeakReference that were freed by Garbage Collector.
        /// </summary>
        private void RemoveDeletedWeakRefs()
        {
            int count = _dict.Count;
            if (count != 0)
            {
                if (this._lastDictCount == 0)
                {
                    this._lastDictCount = count;
                }
                else
                {
                    long totalMemory = GC.GetTotalMemory(false);
                    if (this._lastGlobalMem == 0L)
                    {
                        this._lastGlobalMem = totalMemory;
                    }
                    else
                    {
                        float num3 = ((float)(totalMemory - this._lastGlobalMem)) / ((float)this._lastGlobalMem);
                        float num4 = ((float)(count - this._lastDictCount)) / ((float)this._lastDictCount);
                        if ((num3 < 0f) && (num4 >= 0f))
                        {
                            ArrayList list = new ArrayList();
                            foreach (InternalWeakReference reference in _dict.Keys)
                            {
                                if ((reference != null) && !reference.IsAlive)
                                {
                                    list.Add(reference);
                                }
                            }
                            if (list != null)
                            {
                                foreach (InternalWeakReference obj3 in list)
                                {
                                    _dict.Remove(obj3);
                                }
                            }
                        }
                        this._lastGlobalMem = totalMemory;
                        this._lastDictCount = count;
                    }
                }
            }
        }

        #endregion

        #region IDictionary<TKey,TValue> Members

        /// <summary>
        /// Adds an entry to the Dictionary.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <param name="value">The entry value.</param>
        public void Add(TKey key, TValue value)
        {
            this.AddWeakRef(key, value);
        }

        /// <summary>
        /// Indicates if a key element is contained in the Dictionary.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <returns>True if key element is contained in the Dictionary.</returns>
        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(new InternalWeakReference(key));
        }

        /// <summary>
        /// Returns the collection of Keys from the Dictionary
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                RemoveDeletedWeakRefs();
                List<TKey> keys = new List<TKey>();
                foreach (InternalWeakReference k in _dict.Keys)
                {
                    keys.Add((TKey)k.Target);
                }
                return keys;
            }
        }

        /// <summary>
        /// Removes a key element from the Dictionary.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <returns>True if key element was removed succesfully.</returns>
        public bool Remove(TKey key)
        {
            return _dict.Remove(new InternalWeakReference(key));
        }

        /// <summary>
        /// Tries to get a value and returned it.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <param name="value">The entry value where result will be returned.</param>
        /// <returns>True if key element was found and returned successfully.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dict.TryGetValue(new InternalWeakReference(key), out value);
        }

        /// <summary>
        /// Returns the collection of Values from the Dictionary
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                return _dict.Values;
            }
        }

        /// <summary>
        /// Gets a value from the Dictionary.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <returns>The value corresponding to the key element.</returns>
        public TValue this[TKey key]
        {
            get
            {
                return _dict[new InternalWeakReference(key)];
            }
            set
            {
                if (!this.ContainsKey(key))
                    this.AddWeakRef(key, value);
                else
                    this._dict[new InternalWeakReference(key)] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        /// <summary>
        /// Adds an entry to the Dictionary.
        /// </summary>
        /// <param name="item">The key value pair to be added.</param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Clears and removes all elements from the Dictionary.
        /// </summary>
        public void Clear()
        {
            _dict.Clear();
        }

        /// <summary>
        /// Indicates if the key value pair is in the Dictionary.
        /// </summary>
        /// <param name="item">The key value pair to be searched.</param>
        /// <returns>True if key element is contained in the Dictionary.</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this.ContainsKey(item.Key);
        }

        /// <summary>
        /// </summary>
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the number of elements from the Dictionary.
        /// </summary>
        /// <returns>The number of elements contained in the Dictionary.</returns>
        public int Count
        {
            get { return _dict.Count; }
        }

        /// <summary>
        /// </summary>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Removes a item from the Dictionary.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        /// <returns>True if key element was removed succesfully.</returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return this.Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        /// <summary>
        /// </summary>
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Gets an enumerator for the Dictionary.
        /// </summary>
        /// <returns>A enumerator.</returns>
        public IEnumerator GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Represents an internal WeakReference that overloads the comparative methods in order
        /// to change the behaviour of equality functions.
        /// </summary>
        private sealed class InternalWeakReference : WeakReference
        {
            // Fields
            private int _hashCode;

            // Methods
            internal InternalWeakReference(object o)
                : base(o)
            {
                this._hashCode = o.GetHashCode();
            }

            /// <summary>
            /// Compares this object with another object.
            /// </summary>
            /// <param name="o">The object to be compared.</param>
            /// <returns>True is they are the same instance.</returns>
            public override bool Equals(object o)
            {
                if (o == null)
                {
                    return false;
                }
                if (o.GetHashCode() != this._hashCode)
                {
                    return false;
                }
                if ((o != this) && (!this.IsAlive || !object.ReferenceEquals(o, this.Target)))
                {
                    return false;
                }
                return true;
            }

            /// <summary>
            /// Gets the hash code for this instance.
            /// </summary>
            /// <returns>The hash code for the instance.</returns>
            public override int GetHashCode()
            {
                return this._hashCode;
            }
        }

        /// <summary>
        /// Comparer to be used in the WeakDictionary to overload the behaviour for comparing
        /// objects for InternalWeakReference.
        /// </summary>
        private class WeakKeyComparer<TComp> : IEqualityComparer<TComp>
        {
            /// <summary>
            /// Compares two object instances.
            /// </summary>
            /// <param name="x">An object to be compared.</param>
            /// <param name="y">An object to be compared.</param>
            /// <returns>True is they are the same instance.</returns>
            public bool Equals(TComp x, TComp y)
            {
                if (x == null)
                {
                    return (y == null);
                }
                if ((y == null) || (x.GetHashCode() != y.GetHashCode()))
                {
                    return false;
                }
                WeakReference reference = x as WeakReference;
                WeakReference reference2 = y as WeakReference;
                object obj1 = null;
                object obj2 = null;
                if (reference != null)
                {
                    if (!reference.IsAlive)
                    {
                        return false;
                    }
                    obj1 = reference.Target;
                }
                if (reference2 != null)
                {
                    if (!reference2.IsAlive)
                    {
                        return false;
                    }
                    obj2 = reference2.Target;
                }
                return object.ReferenceEquals(obj1, obj2);
            }

            /// <summary>
            /// Gets the hash code for an object.
            /// </summary>
            /// <param name="obj">The object to get the hash code.</param>
            /// <returns>The hash code for the object.</returns>
            public int GetHashCode(TComp obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
