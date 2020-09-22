using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusiBlocks.FormsBlock
{
    public class FormDefinition
    {
        protected FormDefinition()
        {
        }

        public FormDefinition(string name)
        {
            this.Name = name;
        }

        public virtual string Id
        {
            get;
            protected set;
        }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual bool Deleted { get; set; }
    }
}
