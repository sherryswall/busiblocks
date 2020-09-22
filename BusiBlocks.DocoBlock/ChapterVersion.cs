using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusiBlocks.DocoBlock
{
    public class ChapterVersion
    {
        private string mId;
        public virtual string Id
        {
            get { return mId; }
            set { mId = value; }
        }
        private string mChapterId;
        public virtual string ChapterId
        {
            get { return mChapterId; }
            set { mChapterId = value; }
        }
        private string mVersionNo;
        public virtual string Version
        {
            get { return mVersionNo; }
            set { mVersionNo = value; }
        }
        private string mName;
        public virtual string Name
        {
            get { return mName; }
            set { mName = value; }
        }
        private string mContent;
        public virtual string Content
        {
            get { return mContent; }
            set { mContent = value; }
        }
        private int mActive;
        public virtual int Active
        {
            get { return mActive; }
            set { mActive = value; }
        }
        private int mSequence;
        public virtual int Sequence
        {
            get { return mSequence; }
            set { mSequence = value; }
        }
        private int mVersOrder;
        public virtual int VersionOrder
        {
            get { return mVersOrder; }
            set { mVersOrder = value; }
        }

        public ChapterVersion()
        { }
        public ChapterVersion(string docId,string chapterID,string name,string content,string versionNumber,int versionOrder,int sequence)
        {
            ChapterId = chapterID;
            Name = name;
            Content = content;
            Version = versionNumber;
            Sequence = sequence;
            VersionOrder = versionOrder;
        }

        public virtual bool Deleted { get; set; }

    }
}
