using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared.DTOS.BasketDTOS;
using E_Commerce.Shared.DTOS.ProductDTOS;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketsController: ControllerBase
    {
        private readonly IBasketService basketService;
        public BasketsController(IBasketService _basketService)
        {
            basketService=_basketService;
        }
        [HttpGet]
        public async Task<ActionResult<ProductDto>> GetBasket(string id)
        {
            var basket=await basketService.GetBasketAsync(id);
            return Ok(basket);
        }
        [HttpPost]
        public async Task<ActionResult<CartDto>> CreateOrUpdateBasket(CartDto basket)
        {
            var updatedBasket = await basketService.CreateOrUpdateBasketAsync(basket);
            return Ok(updatedBasket);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteBasket(string id)
        {
            var result = await basketService.DeleteBasketAsync(id);
            return Ok(result);
        }
    }
}
