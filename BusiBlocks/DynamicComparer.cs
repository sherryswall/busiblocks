using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Web.UI.WebControls;
using Label = System.Reflection.Emit.Label;

namespace BusiBlocks
{
    public class DynamicComparer<T> : IComparer<T>
    {
        #region Private Fields

        protected CompareMethodInvoker Comparer;
        protected DynamicMethod Method;

        #endregion

        #region Nested type: CompareMethodInvoker

        protected delegate int CompareMethodInvoker(T x, T y);

        #endregion

        #region Constructors

        public DynamicComparer(string orderBy, SortDirection sortDirection)
        {
            Initialize(orderBy + " " + (sortDirection == SortDirection.Ascending ? "ASC" : "DESC"));
        }

        public DynamicComparer(SortProperty[] sortProperties)
        {
            Initialize(sortProperties);
        }

        #endregion

        #region Public Methods

        public int Compare(T x, T y)
        {
            if (x == null && y == null)
                return 0;
            else if (x == null)
                return -1;
            else if (y == null)
                return 1;

            return Comparer.Invoke(x, y);
        }

        public void Initialize(string orderBy)
        {
            Initialize(ParseOrderBy(orderBy));
        }

        public void Initialize(SortProperty[] sortProperties)
        {
            CheckSortProperties(sortProperties);
            Method = CreateDynamicCompareMethod(sortProperties);
            Comparer = (CompareMethodInvoker) Method.CreateDelegate(typeof (CompareMethodInvoker));
        }

        #endregion

        #region Private Methods

        protected SortProperty[] ParseOrderBy(string orderBy)
        {
            if (orderBy == null || orderBy.Trim().Length == 0)
                throw new ArgumentException("The \"order by\" string must not be null or empty.");

            string[] props = orderBy.Split(',');
            var sortProps = new SortProperty[props.Length];

            for (int i = 0; i < props.Length; i++)
            {
                bool descending = false;
                string prop = props[i].Trim();

                if (prop.ToUpper().EndsWith(" DESC"))
                {
                    descending = true;
                    prop = prop.Substring(0, prop.ToUpper().LastIndexOf(" DESC", StringComparison.Ordinal));
                }
                else if (prop.ToUpper().EndsWith(" ASC"))
                {
                    prop = prop.Substring(0, prop.ToUpper().LastIndexOf(" ASC", StringComparison.Ordinal));
                }

                prop = prop.Trim();

                sortProps[i] = new SortProperty(prop, descending);
            }

            return sortProps;
        }

        protected DynamicMethod CreateDynamicCompareMethod(SortProperty[] sortProperties)
        {
            var dm = new DynamicMethod("DynamicComparison", typeof (int), new[] {typeof (T), typeof (T)},
                                       typeof (DynamicComparer<T>));

            #region Generate IL for dynamic method

            ILGenerator ilGen = dm.GetILGenerator();

            Label lbl = ilGen.DefineLabel(); // Declare and define a label that we can jump to.
            ilGen.DeclareLocal(typeof (int)); // Declare a local variable for storing result.
            var localVariables = new Dictionary<Type, LocalBuilder>();

            ilGen.Emit(OpCodes.Ldc_I4_0); // Push 0 onto the eval stack.
            ilGen.Emit(OpCodes.Stloc_0); // Store the eval stack item in the local variable @ position 0.

            foreach (SortProperty property in sortProperties)
                // For each of the properties we want to check inject the following il.
            {
                string propertyName = property.Name;
                PropertyInfo propertyInfo = typeof (T).GetProperty(propertyName);

                ilGen.Emit(OpCodes.Ldloc_0); // Load local variable at position 0.
                ilGen.Emit(OpCodes.Brtrue_S, lbl);
                    // Is the local variable in the evaluation stack equal to 0. If not jump to the label we just defined.
                ilGen.Emit(OpCodes.Ldarg_0); // Load argument at position 0.
                ilGen.EmitCall(OpCodes.Callvirt, propertyInfo.GetGetMethod(), null); // Get "Name" property value.

                if (propertyInfo.PropertyType.IsValueType)
                    // If the type is a valuetype then we need to inject the following IL.
                {
                    if (!localVariables.ContainsKey(propertyInfo.PropertyType))
                        // Do we have a local variable for this type? If not, add one.
                        localVariables.Add(propertyInfo.PropertyType, ilGen.DeclareLocal(propertyInfo.PropertyType));
                            // Adds a local variable of type x.

                    int localIndex = localVariables[propertyInfo.PropertyType].LocalIndex;
                        // This local variable is for handling value types of type x.

                    ilGen.Emit(OpCodes.Stloc, localIndex); // Store the value in the local var at position x.
                    ilGen.Emit(OpCodes.Ldloca_S, localIndex); // Load the address of the value into the stack. 
                }

                ilGen.Emit(OpCodes.Ldarg_1); // Load argument at position 0.
                ilGen.EmitCall(OpCodes.Callvirt, propertyInfo.GetGetMethod(), null);
                    // Get "propertyName" property value.
                ilGen.EmitCall(OpCodes.Callvirt,
                               propertyInfo.PropertyType.GetMethod("CompareTo", new[] {propertyInfo.PropertyType}), null);
                    // Compare the top 2 items in the evaluation stack and push the return value into the eval stack.

                if (property.Descending)
                    // If the sort should be descending we need to flip the result of the comparison.
                    ilGen.Emit(OpCodes.Neg); // Negates the item in the eval stack.

                ilGen.Emit(OpCodes.Stloc_0); // Store the result in the local variable.
            }

            ilGen.MarkLabel(lbl); // This is the spot where the label we created earlier should be added.
            ilGen.Emit(OpCodes.Ldloc_0); // Load the local var into the eval stack.
            ilGen.Emit(OpCodes.Ret); // Return the value.

            #endregion

            return dm;
        }

        protected void CheckSortProperties(SortProperty[] sortProperties)
        {
            if (sortProperties == null)
                sortProperties = new SortProperty[0];

            Type instanceType = typeof (T);
            if (!instanceType.IsPublic)
                throw new ArgumentException(string.Format("Type \"{0}\" is not public.", typeof (T).FullName));

            foreach (SortProperty sProp in sortProperties)
            {
                PropertyInfo pInfo = instanceType.GetProperty(sProp.Name);

                if (pInfo == null)
                    throw new ArgumentException(string.Format("No public property named \"{0}\" was found.", sProp.Name));

                if (!pInfo.CanRead)
                    throw new ArgumentException(string.Format("The property \"{0}\" is write-only.", sProp.Name));

                if (!typeof (IComparable).IsAssignableFrom(pInfo.PropertyType))
                    throw new ArgumentException(
                        string.Format("The type \"{1}\" of the property \"{0}\" does not implement IComparable.",
                                      sProp.Name, pInfo.PropertyType.FullName));
            }
        }

        #endregion
    }


    public struct SortProperty
    {
        #region Properties

        private bool _descending;
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null || value.Trim().Length == 0)
                    throw new ArgumentException("A property cannot have an empty name.");

                _name = value.Trim();
            }
        }

        public bool Descending
        {
            get { return _descending; }
            set { _descending = value; }
        }

        #endregion

        #region Constructors

        public SortProperty(string propertyName)
        {
            if (propertyName == null || propertyName.Trim().Length == 0)
                throw new ArgumentException("A property cannot have an empty name.");

            _name = propertyName.Trim();
            _descending = false;
        }

        public SortProperty(string propertyName, bool sortDescending)
        {
            if (propertyName == null || propertyName.Trim().Length == 0)
                throw new ArgumentException("A property cannot have an empty name.");

            _name = propertyName.Trim();
            _descending = sortDescending;
        }

        #endregion
    }
}