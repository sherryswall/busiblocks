using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusiBlocks.Attachment
{
    [Serializable]
    public class FileInfo
    {
        private string _name;

        protected FileInfo()
        {
        }

        public FileInfo(string name, string contentType, byte[] contentData)
        {
            Name = name;
            ContentType = contentType;
            ContentData = contentData;
        }

        public virtual string Name
        {
            get { return _name; }
            protected set
            {
                EntityHelper.ValidateCode("Name", value);
                _name = value;
            }
        }

        public virtual byte[] ContentData { get; protected set; }

        public virtual string Description { get; set; }

        public virtual string ContentType { get; protected set; }
    }
}