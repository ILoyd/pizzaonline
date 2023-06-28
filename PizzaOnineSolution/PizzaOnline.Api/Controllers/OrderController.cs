using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PizzaOnline.Bll.Dtos;
using PizzaOnline.Bll.Interfaces;
using System.Xml.Serialization;

namespace PizzaOnline.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            try
            {
                return Ok(await _orderService.GetOrderAsync(id));
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderDto>> PostOrder(OrderDto orderDto)
        {
            try
            {
                if (orderDto != null)
                {
                    orderDto.Pizzas.Clear();
                    _orderService.SaveLastOrderToXml(orderDto);
                }
                var created = await _orderService.InsertOrderAsync(orderDto);

                return CreatedAtAction(nameof(GetOrder), new { id = created.Id }, created);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<OrderDto> GetLastOrder()
        {
            var orderDto = _orderService.LoadLastOrderFromXml();
            if (orderDto == null)
                return NotFound();
            return Ok(orderDto);
        }
    }
}
