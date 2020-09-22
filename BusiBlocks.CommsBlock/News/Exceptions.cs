using System;
using System.Collections.Generic;
using System.Text;

namespace BusiBlocks.CommsBlock.News
{
    [Serializable]
    public class ItemNotFoundException : BusiBlocksException
    {
        public ItemNotFoundException(string id)
            : base("Item " + id + " not found")
        {

        }
    }

    [Serializable]
    public class NewsCategoryNotFoundException : BusiBlocksException
    {
        public NewsCategoryNotFoundException(string id)
            : base("News category " + id + " not found")
        {

        }
    }

    [Serializable]
    public class CategoryNotSpecifiedException : BusiBlocksException
    {
        public CategoryNotSpecifiedException()
            : base("Categories list not specified")
        {

        }
    }
}
