using System;
using System.Collections.Generic;
using System.Text;

namespace BusiBlocks.DocoBlock
{
    [Serializable]
    public class ArticleNotFoundException : BusiBlocksException
    {
        public ArticleNotFoundException(string id)
            : base("Article " + id + " not found")
        {

        }
    }

    [Serializable]
    public class ArticleNameAlreadyExistsException : BusiBlocksException
    {
        public ArticleNameAlreadyExistsException(string name)
            : base("Name " + name + " already used. The article name must be unique.")
        {

        }
    }

    [Serializable]
    public class ArticleStatusNotValidException : BusiBlocksException
    {
        public ArticleStatusNotValidException(DocoBlock.ArticleStatus status)
            : base("Article status " + status.ToString() + " not valid")
        {

        }
    }

    [Serializable]
    public class DocoCategoryNotFoundException : BusiBlocksException
    {
        public DocoCategoryNotFoundException(string id)
            : base("Doco category " + id + " not found")
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
