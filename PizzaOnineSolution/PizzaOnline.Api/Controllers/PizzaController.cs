using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaOnline.Bll.Dtos;
using PizzaOnline.Bll.Interfaces;

namespace PizzaOnline.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PizzaController : ControllerBase
    {
        private readonly IPizzaService _pizzaService;

        public PizzaController(IPizzaService pizzaService)
        {
            _pizzaService = pizzaService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PizzaDto>>> GetPizzas()
        {
            var pizzas = (await _pizzaService.GetPizzasAsync()).ToList();
            return pizzas.Count > 0 ? Ok(pizzas) : NotFound(pizzas);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PizzaDto>> GetPizza(int id)
        {
            try
            {
                return Ok(await _pizzaService.GetPizzaAsync(id));
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PizzaDto>> PostPizza([FromBody] PizzaDto pizza)
        {
            try
            {
                var created = await _pizzaService.InsertPizzaAsync(pizza);
                return CreatedAtAction(nameof(GetPizza), new { id = created.Id }, created);
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }
            
        }

        [HttpPut("{id}/{forceUpdate}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PutPizza(int id, [FromBody] PizzaDto value, bool forceUpdate)
        {
            try
            {
                await _pizzaService.UpdatePizzaAsync(id, value, forceUpdate);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex is DbUpdateConcurrencyException)
                    return BadRequest();
                else
                    return NotFound();
            }
        }

        [HttpDelete("{id}/{forceDelete}/{lastVersion}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeletePizza(int id, bool forceDelete, long lastVersion)
        {
            try
            {
                await _pizzaService.SoftDeleteAsync(id, forceDelete, lastVersion);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex is DbUpdateConcurrencyException)
                    return BadRequest();
                else
                    return NotFound();
            }
        }
    }
}
