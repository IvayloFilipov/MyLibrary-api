namespace Common.Models.InputDTOs
{
    public class PaginatorInputDto
    {
        const int maxPageSize = 30;
        public int Page { get; set; }

        private int pageSize;
        public int PageSize
        {
            get { return this.pageSize; }
            set
            {
                this.pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}
