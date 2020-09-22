// -----------------------------------------------------------------------
// <copyright file="KeyComparer.cs" company="BusiBlocks">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace BusiBlocks
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class KeyComparer<T> : IComparer<T>
    {
        private readonly Func<T, object> keyExtractor;

        public KeyComparer(Func<T, object> keyExtractor)
        {
            this.keyExtractor = keyExtractor;
        }

        #region IComparer<T> Members

        public int Compare(T x, T y)
        {
            return keyExtractor(x).ToString().CompareTo(keyExtractor(y).ToString());
        }

        #endregion

        public int GetHashCode(T obj)
        {
            return keyExtractor(obj).GetHashCode();
        }
    }
}