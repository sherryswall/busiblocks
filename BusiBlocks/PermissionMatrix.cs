using System.Collections.Generic;

namespace BusiBlocks
{
    public class PermissionMatrix
    {
        public PermissionMatrix(List<object> rows, List<object> columns, List<PermissionMatrixDataItem> data)
        {
            this.rows = rows;
            this.columns = columns;
            this.data = data;
        }

        public List<object> rows { get; set; }
        public List<object> columns { get; set; }
        public List<PermissionMatrixDataItem> data { get; set; }
    }

    public class PermissionMatrixDataItem
    {
        public PermissionMatrixDataItem(string id, object columnData, object rowData)
        {
            this.id = id;
            this.rowData = rowData;
            this.columnData = columnData;
        }

        public string id { get; set; }
        public object columnData { get; set; }
        public object rowData { get; set; }
    }
}