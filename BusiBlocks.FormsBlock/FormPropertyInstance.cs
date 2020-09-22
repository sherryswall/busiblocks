using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusiBlocks.FormsBlock
{
    public class FormPropertyInstance
    {
        protected FormPropertyInstance()
        {
        }

        public FormPropertyInstance(FormInstance formInstance, FormProperty formProperty, string value)
        {
            this.FormInstance = formInstance;
            this.FormProperty = formProperty;
            this.Value = value;
        }

        public virtual string Id
        {
            get;
            protected set;
        }

        public virtual FormInstance FormInstance
        {
            get;
            set;
        }

        public virtual FormProperty FormProperty
        {
            get;
            set;
        }

        public virtual string Value
        {
            get;
            set;
        }

        public virtual bool Deleted { get; set; }
    }
}
