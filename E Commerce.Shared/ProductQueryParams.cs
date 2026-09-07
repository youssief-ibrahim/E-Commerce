using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Shared
{
    public class ProductQueryParams
    {
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public string? Search { get; set; }
        public ProductSortingOptions Sort { get; set; }

        private int pageIndex = 1;
        public int PageIndex
        {
            get { return pageIndex; }
            set
            {
                pageIndex = (value <= 0) ? 1 : value;
            }
        }

        private const int DefultPageSize = 5;
        private const int MaxPageSize = 10;
        private int pageSize = DefultPageSize;

        public int PageSize
        {
            get { return pageSize; }
            set
            {
                if (value > MaxPageSize) pageSize = MaxPageSize;
                else if (value <= 0) pageSize = DefultPageSize;
                else pageSize = value;
            }
        }
    }
}
