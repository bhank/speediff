namespace CoyneSolutions.SpeeDiff
{
    public class ListViewColumnSortOptions
    {
        public bool Numeric { get; set; }
        public bool Ascending { get; set; }

        public ListViewColumnSortOptions()
        {
            Ascending = true;
        }
    }
}