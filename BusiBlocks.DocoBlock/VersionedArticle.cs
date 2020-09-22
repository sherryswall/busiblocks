using System;
using System.Collections.Generic;
using System.Text;

namespace BusiBlocks.DocoBlock
{
    public class VersionedArticle : ArticleBase
    {
        protected VersionedArticle()
        {

        }

        /// <summary>
        /// Create a new versioned article from the specified article source
        /// </summary>
        /// <param name="source"></param>
        public VersionedArticle(Article source)
            : base(source)
        {
            this.Article = source;
        }

        public virtual Article Article
        {
            get;
            protected set;
        }

        public virtual bool Deleted { get; set; }
    }
}
