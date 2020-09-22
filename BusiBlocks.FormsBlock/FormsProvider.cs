using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Provider;

namespace BusiBlocks.FormsBlock
{
    /// <summary>
    /// FormsProvider abstract class. 
    /// A FormsProvider can be used to store forms information.
    /// 
    /// </summary>
    public abstract class FormsProvider : ProviderBase
    {
        #region Form Definition
        public abstract FormDefinition CreateFormDefinition(string name);

        public abstract void UpdateFormDefinition(FormDefinition formDefinition);

        public abstract void DeleteFormDefinition(FormDefinition formDefinition);

        public abstract FormDefinition GetFormDefinition(string id);

        public abstract FormDefinition GetFormDefinitionByName(string name, bool throwIfNotFound);

        public abstract IList<FormDefinition> GetAllFormDefinitions();
        #endregion

        #region Form Property
        public abstract FormProperty CreateFormProperty(FormDefinition formDefinition, string name, string datatype, int sequenceNo);

        public abstract void UpdateFormProperty(FormProperty formProperty);

        public abstract void DeleteFormProperty(FormProperty formProperty);

        public abstract FormProperty GetFormProperty(string id);

        public abstract FormProperty GetFormPropertyByName(string name, bool throwIfNotFound);

        public abstract IList<FormProperty> GetAllFormProperties(FormDefinition formDefinition);
        #endregion

        #region Form Instance
        public abstract FormInstance CreateFormInstance(FormDefinition formDefinition, string createdBy);

        public abstract void UpdateFormInstance(FormInstance formInstance);

        public abstract void DeleteFormInstance(FormInstance formInstance);

        public abstract FormInstance GetFormInstance(string id);

        public abstract FormInstance GetFormInstanceByName(string name, bool throwIfNotFound);

        public abstract IList<FormInstance> GetAllFormInstances();
        #endregion

        #region Form Property Instance
        public abstract FormPropertyInstance CreateFormPropertyInstance(FormInstance formInstance, FormProperty formProperty, string value);

        public abstract void UpdateFormPropertyInstance(FormPropertyInstance formPropertyInstance);

        public abstract void DeleteFormPropertyInstance(FormPropertyInstance formPropertyInstance);

        public abstract FormPropertyInstance GetFormPropertyInstance(string id);

        public abstract FormPropertyInstance GetFormPropertyInstanceByKeys(FormInstance formInstance, FormProperty formProperty, bool throwIfNotFound);
        #endregion
    }
}
