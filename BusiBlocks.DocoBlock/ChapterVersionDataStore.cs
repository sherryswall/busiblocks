using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Expression;

namespace BusiBlocks.DocoBlock
{
    public class ChapterVersionDataStore : EntityDataStoreBase<ChapterVersion, string>
    {
        public ChapterVersionDataStore(TransactionScope transaction)
            : base(transaction)
        { }

        //Get all Chapter Versions.
        public IList<ChapterVersion> FindAllItems()
        {
            ICriteria criteria = CreateCriteria();
            criteria.AddOrder(Order.Asc("Sequence"));

            return base.Find(criteria, false);
        }

        //Find all Chapter Version using the ChapterId
        public IList<ChapterVersion> FindAllItems(string chapterId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("ChapterId", chapterId));
            criteria.AddOrder(Order.Asc("Version"));

            return base.Find(criteria, false);
        }

        public IList<ChapterVersion> FindAllItemsByArticleId(string articleId)
        {
            IList<Chapter> chapters = DocoManager.GetAllChapters(articleId);

            //Load the chapter from the docoID
            var chaps = from c in chapters
                        where c.DocId == articleId
                        select c;

            ICriteria criteria = CreateCriteria();

            List<string> ids = new List<string>();

            foreach (Chapter item in chaps.ToList())
            {
                ChapterVersion maxChapVersion = FindMaxVersion(item.Id).FirstOrDefault<ChapterVersion>();

                if (maxChapVersion != null)
                    ids.Add(maxChapVersion.Id);
            }

            criteria.Add(Expression.In("Id", ids));
            criteria.AddOrder(Order.Asc("Sequence"));

            return base.Find(criteria, false);
        }

        //Finds the latest version available for the chapter. Used when displaying latest chapter.
        public IList<ChapterVersion> FindMaxVersion(string Id)
        {
            ICriteria criteria = CreateCriteria();

            criteria.Add(Expression.Eq("ChapterId", Id));
            criteria.AddOrder(Order.Desc("VersionOrder"));

            return base.Find(criteria, false);
        }

        //Finds the lowest sequence/order number for the article.
        public int LowestSeqNumber(string articleId)
        {
            IList<Chapter> chapters = DocoManager.GetAllChapters(articleId);
            int tempMin = 999999;
            //Load the chapter from the docoID
            var chaps = from c in chapters
                        where c.DocId == articleId
                        select c;

            ICriteria criteria = CreateCriteria();

            List<string> ids = new List<string>();

            if (chaps.Count<Chapter>() > 0)
            {
                foreach (Chapter item in chaps.ToList())
                {
                    ChapterVersion minChapSeq = FindLowest(item.Id).FirstOrDefault<ChapterVersion>();
                    if (minChapSeq != null)
                    {
                        if (minChapSeq.Sequence < tempMin)
                            tempMin = minChapSeq.Sequence;
                    }
                }
                return tempMin;
            }
            else return 0;
        }

        public IList<ChapterVersion> FindLowest(string id)
        {
            ICriteria criteria = CreateCriteria();        

            criteria.Add(Expression.Eq("ChapterId", id));
            criteria.AddOrder(Order.Desc("Version"));

            return base.Find(criteria, false);
        }

    }
}
