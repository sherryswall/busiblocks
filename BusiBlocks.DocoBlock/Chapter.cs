using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Expression;
using System.Data;
using System.Web;

namespace BusiBlocks.DocoBlock
{

    public class Chapter
    {
        private string mId;
        public virtual string Id
        {
            get { return mId; }
            set { mId = value; }
        }
        private string mDocId;
        public virtual string DocId
        {
            get { return mDocId; }
            protected set { mDocId = value; }
        }        

        private string mStatus;
        public virtual string Status1
        {
            get { return mStatus; }
            protected set { mStatus = value; }
        }
            
        public Chapter()
        { }

        public Chapter(string docId)
        {
            DocId = docId;
        }

        public virtual IList<Chapter> FindAllItems()
        {
            return DocoManager.GetAllChapters();
        }
        
        public virtual void SaveSequence()
        {
            DocoManager.UpdateChapter(this);
        }
        public virtual void LoadList()
        {
            HttpContext.Current.Session["Chapters"] = FindAllItems();
        }
        public virtual void UpdateAllItems(List<Chapter> chapters)
        {
            //will the updates over here.
            
        }

        public virtual bool Deleted { get; set; }
    }
    public struct SubSectionChap
    {
        public string ChapId { get; set; }
        public string AnchorTag { get; set; }
    }
}
