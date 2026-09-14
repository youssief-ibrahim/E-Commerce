using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services.Exceptions
{
    public abstract  class NotFoundException(string message) : Exception(message)
    {
        //public NotFoundException(string message):base(message)
        //{
        // we use primary constractor 
        //}
        public sealed  class ProductNotFoundException(int id) : NotFoundException($"Product Not Found with id {id}")
        {
        }
        public sealed class BasketNotFoundException(string id) : NotFoundException($"Basket Not Found with id {id}")
        {
        }
    }
}
