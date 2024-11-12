namespace ASP.NETCore_WebAPI.Pagination
{
    public abstract class QueryStringParameter
    {
        const int maxpagesize = 50;
        public int PageNumber { get; set; } = 1;

        private int _pageSize  = maxpagesize;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value > maxpagesize ? maxpagesize : value;
            }
        }
    }
}
