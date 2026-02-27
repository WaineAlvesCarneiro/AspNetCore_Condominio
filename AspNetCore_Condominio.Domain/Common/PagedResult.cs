namespace AspNetCore_Condominio.Domain.Common
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int LinesPerPage { get; set; }

        public int TotalPages
        {
            get
            {
                if (LinesPerPage <= 0) return 0;
                return (int)Math.Ceiling(TotalCount / (double)LinesPerPage);
            }
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public int PageNumber
        {
            get => PageIndex;
            set => PageIndex = value;
        }

        public int PageSize
        {
            get => LinesPerPage;
            set => LinesPerPage = value;
        }

        public int FirstItemOnPage => (PageIndex - 1) * LinesPerPage + 1;
        public int LastItemOnPage => Math.Min(PageIndex * LinesPerPage, TotalCount);
    }
}