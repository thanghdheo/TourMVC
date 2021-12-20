using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Tour_MVC.Models
{
    public class sortOrder
    {
        private string UpIcon = "fa fa-arrow-up";
        private string DownIcon = "fa fa-arrow-down";
        public string sortedStr { get; set; }
        public SortOrder sortedOrder { get; set; }

        private List<SortTableColumn> sortTableColumns = new List<SortTableColumn>();


        public void AddColum(string column, bool isDefaultColumn=false)
        {
            SortTableColumn tmp = this.sortTableColumns.Where(c => c.columnName.ToLower() == column.ToLower())
                                    .SingleOrDefault();
            if (tmp == null)
            {
                sortTableColumns.Add(new SortTableColumn()
                {
                    columnName = column,
                });
            }
            if (isDefaultColumn==true||tmp == null)
            {
                sortedStr = column;
                sortedOrder = SortOrder.Ascending;
            }
        }

        public SortTableColumn getColumn(string column)
        {
            SortTableColumn tmp = this.sortTableColumns.Where(c => c.columnName.ToLower() == column.ToLower())
                                    .SingleOrDefault();
            if (tmp == null)
            {
                sortTableColumns.Add(new SortTableColumn()
                {
                    columnName = column,
                });
            }
            return tmp;
        }
        public void ApplySort(string sort)
        {

            //this.getColumn("stt").sortIcon = "";
            //this.getColumn("stt").sort = "stt";

            //this.getColumn("tên tour").sortIcon = "";
            //this.getColumn("tên tour").sort = "tentour";

            //this.getColumn("đặc điểm").sortIcon = "";
            //this.getColumn("đặc điểm").sort = "desc";

            //this.getColumn("tên loại hình").sortIcon = "";
            //this.getColumn("tên loại hình").sort = "namelh";


            if(sort == "")
            {
                sort = this.sortedStr;
            }

            sort = sort.ToLower();

            foreach(SortTableColumn sortTableColumn in this.sortTableColumns)
            {
                sortTableColumn.sortIcon = "";
                sortTableColumn.sort = sortTableColumn.columnName;

                if(sort == sortTableColumn.columnName.ToLower())
                {
                    this.sortedOrder = SortOrder.Descending;
                    this.sortedStr = sortTableColumn.columnName;
                    sortTableColumn.sortIcon = DownIcon;
                    sortTableColumn.sort = sortTableColumn.columnName + "_desc";
                }

                if(sort == sortTableColumn.columnName.ToLower() + "_desc")
                {
                    this.sortedOrder = SortOrder.Ascending;
                    this.sortedStr = sortTableColumn.columnName;
                    sortTableColumn.sortIcon = UpIcon;
                    sortTableColumn.sort = sortTableColumn.columnName;
                }
            }
        }
    }

    public class SortTableColumn
    {
        public string columnName { get; set; }
        public string sort { get; set; }

        public string sortIcon { get; set; }
    }
}
