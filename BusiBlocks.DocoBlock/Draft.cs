using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusiBlocks.DocoBlock
{
    public class Draft
    {
        private string mId;
        public virtual string Id
        {
            get { return mId; }
            set { mId = value; }
        }
        private string mVersionId;
        public virtual string VersionId
        {
            get { return mVersionId; }
            set { mVersionId = value; }
        }
        private string mContent;
        public virtual string Content
        {
            get { return mContent; }
            set { mContent = value; }
        }
        private DateTime mSaveDate;
        public virtual DateTime SaveDate
        {
            get { return mSaveDate; }
            set { mSaveDate = value; }
        }
        private string mName;
        public virtual string Name
        {
            get { return mName; }
            set { mName = value; }
        }
        private int mSequence;
        public virtual int Sequence
        {
            get { return mSequence; }
            set { mSequence = value; }

        }

        public Draft()
        { }

        public virtual bool Deleted { get; set; }
    }
}
