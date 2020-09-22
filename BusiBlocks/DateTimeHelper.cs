// -----------------------------------------------------------------------
// <copyright file="DateTimeHelper.cs" company="BusiBlocks">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace BusiBlocks
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DateTimeHelper
    {
        /// <summary>
        /// Returns a string representation of the current time.
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentTimestamp()
        {
            return DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        }
    }
}