using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusiBlocks.FormsBlock
{
    public class FormProperty
    {
        protected FormProperty()
        {
        }

        public FormProperty(FormDefinition formDefinition, string name, string datatype, int sequenceNo)
        {
            this.FormDefinition = formDefinition;
            this.Name = name;
            this.Datatype = datatype;
            this.SequenceNo = sequenceNo;
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

        public virtual string Name
        {
            get;
            set;
        }

        public virtual string Datatype
        {
            get;
            set;
        }

        public virtual int SequenceNo
        {
            get;
            set;
        }

        public virtual bool Deleted { get; set; }
    }
}
