using System;

namespace BusiBlocks
{
    public class PagingInfo
    {
        private readonly long mCurrentPage;
        private readonly long mPageSize;

        public PagingInfo(long pPageSize, long pCurrentPage)
        {
            mPageSize = pPageSize;
            mCurrentPage = pCurrentPage;
        }

        public static PagingInfo All
        {
            get { return new PagingInfo(0, 0); }
        }

        public long PageSize
        {
            get { return mPageSize; }
        }

        public long CurrentPage
        {
            get { return mCurrentPage; }
        }

        public long RowCount { get; set; }

        public long PagesCount
        {
            get { return (long) Math.Ceiling(RowCount/(double) PageSize); }
        }
    }
}