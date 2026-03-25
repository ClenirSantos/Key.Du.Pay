namespace Key.Du.Pay.CrossCutting.Filters
{
    public class PagedItems<T>
    {
        public IList<T> Items { get; set; } = new List<T>();
        public long? Total { get; set; }

    }
}
