namespace API.Helpers
{
    // Recieve pagination parameters from the clientapp/user
    public class UserParams
    {
        // setting the maximum page size. It can't be over 50
        private const int MaxPageSize = 50;

        public int PageNumber { get; set; } = 1;

        // setting a default pagesize
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

    }
}