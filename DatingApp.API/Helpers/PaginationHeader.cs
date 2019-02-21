namespace DatingApp.API.Helpers
{
    public class PaginationHeader
    {
        public int CurrentPage { get; set; }
        public int ItemPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public PaginationHeader(int currentPage, int itemPerPagem, int totalItems, int totalPages)
        {
            this.CurrentPage= currentPage;
            this.ItemPerPage= itemPerPagem;
            this.TotalItems = totalItems;
            this.TotalPages =totalPages;
        }
    }
}