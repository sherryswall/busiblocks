using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusiBlocks.FormsBlock
{
    public class FormInstance
    {
        protected FormInstance()
        {
        }

        public FormInstance(FormDefinition formDefinition, string createdBy)
        {
            this.FormDefinition = formDefinition;
            this.CreatedBy = createdBy;
        }

        public virtual string Id
        {
            get;
            protected set;
        }

        public virtual FormDefinition FormDefinition
        {
            get;
            set;
        }

        public virtual string CreatedBy
        {
            get;
            set;
        }

        public virtual bool Deleted { get; set; }
    }
}
