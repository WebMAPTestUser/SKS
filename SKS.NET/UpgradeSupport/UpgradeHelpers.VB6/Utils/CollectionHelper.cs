using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace UpgradeHelpers.VB6.Utils
{
    /// <summary>
    /// The CollectionHelper contains a specific functionality to support VB6.Collection using
    /// System.Collections.Specialized.OrderedDictionary .Net native class.
    /// </summary>
    public static class CollectionHelper
    {
        /// <summary>
        /// Searchs an element in Dictionary via a key and returns the index of the element.
        /// </summary>
        /// <param name="dict">Dictionary where to search the element.
        /// </param>
        /// <param name="key">Key of the element being searched.</param>
        /// <returns>Returns the index of the found element or -1 if element is not found.</returns>
#if TargetF2
        public static int GetIndex(OrderedDictionary dict, object key)
#else
        public static int GetIndex(this OrderedDictionary dict, object key)
#endif
        {
            int index = -1;
            foreach (DictionaryEntry elem in dict)
            {
                index++;
                if (elem.Key == key) return index;
            }
            return -1;
        }
    }
}
