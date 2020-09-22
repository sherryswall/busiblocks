using System;

namespace BusiBlocks.SchemaGenerator
{
    /// <summary>
    /// An assembly attribute that can be used to specify the mapping class that can be generated by the schema generator (Setup.aspx page).
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class SetupMappingAttribute : Attribute
    {
        /// <summary>
        /// Constructor of the assembly used to specify the mapping classes.
        /// </summary>
        /// <param name="category">A category description</param>
        /// <param name="mappingClasses">The classes of the specified category.
        /// Must contains a list of class names in the form of Namespace.ClassName separated by a semicolon (;).
        /// Note that the order of the mapping classes is important if the classes are related</param>
        public SetupMappingAttribute(string category, string mappingClasses)
        {
            Category = category;
            MappingClasses = mappingClasses;

            string[] mapList = MappingClasses.Split(';');
            MappingTypes = new Type[mapList.Length];
            for (int i = 0; i < mapList.Length; i++)
            {
                string mapStr = mapList[i].Trim();
                if (mapStr.Length > 0)
                    MappingTypes[i] = Type.GetType(mapStr, true);
            }
        }

        public string MappingClasses { get; set; }


        public string Category { get; set; }


        public Type[] MappingTypes { get; set; }
    }
}