using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.DocoBlock
{
    public class DraftDataStore : EntityDataStoreBase<Draft, string>
    {
        public DraftDataStore(TransactionScope transaction)
            : base(transaction)
        { }

        public IList<Draft> FindAllItems()
        {
            ICriteria criteria = CreateCriteria();
            criteria.AddOrder(Order.Asc("Id"));

            return base.Find(criteria, false);
        }
        public Draft FindDraftByArticleId(string articleId)
        {
            int lowestOrderNumber = DocoManager.GetLowestSequenceNumber(articleId);

            IList<ChapterVersion> chapters = DocoManager.GetAllItemsByArticleId(articleId);

            if (chapters.Count > 0)
            {
                var chaps = from c in chapters
                            where c.Sequence == lowestOrderNumber
                            select c;

                List<ChapterVersion> ListchapVersions = chaps.ToList<ChapterVersion>();

                if (ListchapVersions.Count > 0)
                {
                    Draft draft = new Draft();
                    draft = FindItemByChapterId(ListchapVersions.First<ChapterVersion>().Id).FirstOrDefault<Draft>();
                    return draft;
                }                
            }

            return new Draft() { Content = "" };
            
        }
        public IList<Draft> FindItemByChapterId(string chapterVersionId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("VersionId", chapterVersionId));
            criteria.AddOrder(Order.Desc("SaveDate"));

            return base.Find(criteria, false);
        }
        public IList<Draft> FindItemByDraftId(string draftId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Id", draftId));

            return base.Find(criteria, false);
        }
    }
}
