namespace Alpha.BetLists.API.Models
{
    public class PagedData<T> where T : class
    {
        public IEnumerable<T> Tickets { get; }

        public int CurrentPageIndex { get; }

        public int PageSize { get; }

        public int TotalRecords { get; }

        public PagedData(IEnumerable<T> tickets, int currentPageIndex, int pageSize, int totalRecords)
        {
            Tickets = tickets;
            CurrentPageIndex = currentPageIndex;
            PageSize = pageSize;
            TotalRecords = totalRecords;
        }

        public bool Any()
        {
            return TotalRecords > 0;
        }

        public bool MoreThanOne()
        {
            return TotalRecords > 1;
        }
    }
}