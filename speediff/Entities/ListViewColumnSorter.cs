using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CoyneSolutions.SpeeDiff
{
    public class ListViewColumnSorter : IComparer
    {
        public List<ListViewColumnSortOptions> ColumnSortOptions { get; set; }
        private int sortColumn = -1;
        private bool sortAscending;
        public ListViewColumnSorter(ListView listView)
        {
            ColumnSortOptions = new List<ListViewColumnSortOptions>();
            for (var i = 0; i < listView.Columns.Count; i++)
            {
                ColumnSortOptions.Add(new ListViewColumnSortOptions());
            }
            listView.ListViewItemSorter = this;
            listView.ColumnClick += listView_ColumnClick;
        }


        void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == sortColumn)
            {
                sortAscending = !sortAscending;
            }
            else
            {
                sortColumn = e.Column;
                if (ColumnSortOptions.Count > sortColumn)
                {
                    // Use the configured default sort for this column
                    sortAscending = ColumnSortOptions[sortColumn].Ascending;
                }
                else
                {
                    sortAscending = true;
                }
            }
            ((ListView)sender).Sort();
        }

        public int Compare(object x, object y)
        {
            if (sortColumn == -1)
            {
                return 0;
            }
            var itemX = x as ListViewItem;
            var itemY = y as ListViewItem;
            if (itemX == null || itemY == null) return 0;

            var numeric = false;
            if (ColumnSortOptions.Count > sortColumn)
            {
                // Use the configured sort type for this column
                numeric = ColumnSortOptions[sortColumn].Numeric;
            }

            var textX = itemX.SubItems[sortColumn].Text;
            var textY = itemY.SubItems[sortColumn].Text;

            int compareResult;

            if (numeric)
            {
                double numX, numY;
                var parsedX = double.TryParse(textX, out numX);
                var parsedY = double.TryParse(textY, out numY);

                if (parsedX && parsedY)
                {
                    compareResult = numX.CompareTo(numY);
                }
                else
                {
                    compareResult = parsedX.CompareTo(parsedY);
                }
            }
            else
            {
                compareResult = string.Compare(textX, textY, StringComparison.Ordinal);
            }
            if (!sortAscending)
            {
                compareResult = -compareResult;
            }
            return compareResult;
        }
    }
}
