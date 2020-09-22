using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Expression;
using NHibernate.Transform;

namespace BusiBlocks.CommsBlock.News
{
    /// <summary>
    /// Class that use NHibernate to save the Item data
    /// </summary>
    public class ItemDataStore : EntityDataStoreBase<Item, string>
    {
        public ItemDataStore(TransactionScope transactionScope)
            : base(transactionScope)
        {
        }

        public IList<Item> FindByCategory(Category category)
        {
            ICriteria criteria = CreateCriteria();
            criteria.CreateCriteria("Category").Add(Expression.Eq("Id", category.Id));
            criteria.AddOrder(Order.Desc("NewsDate"));

            return Find(criteria, false);
        }

        public IList<Item> FindPublishedByCategory(Category category)
        {
            string categoryId = category.Id;
            categoryId = categoryId.Replace("\'", string.Empty).Replace(" AND ", string.Empty).Replace(" OR ", string.Empty).Replace(" - ", string.Empty);

            string sql = "select {ni.*} from NewsItem ni " +
                        "inner join VersionItem vi on ni.Id=vi.ItemId " +
                        "inner join ItemApprovalStatus ias on ni.ApprovalStatusId = ias.Id " +
                        "where vi.GroupId in (select distinct VersionItem.GroupId from VersionItem) " +
                        "and ni.IdCategory= '" + categoryId + "' " +
                        "and ni.Id = (select top 1 NewsItem.Id from NewsItem inner join VersionItem on NewsItem.Id=VersionItem.ItemId where VersionItem.GroupId=vi.GroupId order by NewsItem.NewsDate desc) " +
                        "and ias.Name='Published' " +
                        "and ni.Deleted='false'";

            var newsList = TransactionScope.NHibernateSession.CreateSQLQuery(sql).AddEntity("ni", typeof(Item)).List<Item>();

            return newsList;

            //category is the one which you have read access to


            //var groupIds = TransactionScope.NHibernateSession.CreateCriteria(typeof(Versioning.VersionItem)).SetProjection(Projections.Distinct(Projections.Property("GroupId"))).List();
            //var detachedQuery = DetachedCriteria.For<Item>().CreateCriteria("Version",NHibernate.SqlCommand.JoinType.InnerJoin);
                        
            //var newsItem = TransactionScope.NHibernateSession.CreateCriteria(typeof(Item)).Add(Expression.Eq("Deleted", false));
            //newsItem.CreateCriteria("Version", "vi", NHibernate.SqlCommand.JoinType.InnerJoin);
           
            //newsItem.Add(Subqueries.PropertyIn("Id", detachedQuery));

            //newsItem.CreateCriteria("Category").Add(Expression.Eq("Id", category.Id));
            //newsItem.CreateCriteria("ApprovalStatus").Add(Expression.Eq("Id", ItemApprovalStatusLayer.ItemApprovalStatusManager.GetStatusByName("Published").Id)).List<Item>();

            //return newsItem.List<Item>();
        }

        public IList<Item> FindAllItems()
        {
            ICriteria criteria = CreateCriteria();
            criteria.AddOrder(Order.Desc("NewsDate"));

            return Find(criteria, false);
        }

        public IList<Item> FindByCategory(Category category, PagingInfo paging)
        {
            ICriteria criteria = CreateCriteria();
            criteria.CreateCriteria("Category").Add(Expression.Eq("Id", category.Id));
            criteria.AddOrder(Order.Desc("NewsDate"));

            return Find(criteria, paging, false);
        }

        public IList<Item> FindByGroup(string groupId)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.InsensitiveLike("Groups", "%" + groupId + "%"));
            criteria.SetResultTransformer(new DistinctRootEntityResultTransformer());
            criteria.AddOrder(Order.Desc("NewsDate"));

            return Find(criteria, false);
        }

        public IList<Item> FindByOwner(string owner)
        {
            ICriteria criteria = CreateCriteria();
            criteria.Add(Expression.Eq("Owner", owner));
            criteria.AddOrder(Order.Desc("NewsDate"));

            return Find(criteria, false);
        }

        public IList<Item> FindByFields(Filter<string> categoryName,
                                        Filter<string> tag,
                                        DateTime? fromDate, DateTime? toDate,
                                        PagingInfo paging)
        {
            ICriteria criteria = CreateCriteria();

            if (tag != null)
                criteria.Add(tag.ToCriterion("Tag"));

            if (fromDate != null)
                criteria.Add(Expression.Ge("NewsDate", fromDate));

            if (toDate != null)
                criteria.Add(Expression.Le("NewsDate", toDate));

            if (categoryName != null)
            {
                ICriteria categoryCriteria = criteria.CreateCriteria("Category");
                categoryCriteria.Add(categoryName.ToCriterion("Name"));
            }

            criteria.AddOrder(Order.Desc("NewsDate"));

            return Find(criteria, paging, false);
        }
    }
}
