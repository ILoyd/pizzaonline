using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PizzaOnline.Bll.Dtos;
using PizzaOnline.Bll.Interfaces;
using PizzaOnline.Dal;
using System.Text;

namespace PizzaOnline.Bll
{
    public class PizzaService : IPizzaService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PizzaService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PizzaDto> GetPizzaAsync(int pizzaId)
        {
            return await _context.Pizzas
                .ProjectTo<PizzaDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id == pizzaId)
                ?? throw new EntityNotFoundException("Pizza Not Found");
        }

        public async Task<IEnumerable<PizzaDto>> GetPizzasAsync()
        {
            return await _context.Pizzas
                .ProjectTo<PizzaDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<PizzaDto> InsertPizzaAsync(PizzaDto newPizza)
        {
            var entity = _mapper.Map<Dal.Entities.Pizza>(newPizza);
            _context.Pizzas.Add(entity);
            try
            {
                await _context.SaveChangesAsync();
                return await GetPizzaAsync(entity.Id);
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }

        public async Task UpdatePizzaAsync(int pizzaId, PizzaDto updatedPizza, bool forceUpdate)
        {
            var efPizza = _mapper.Map<Dal.Entities.Pizza>(updatedPizza);
            efPizza.Id = pizzaId;
            var entry = _context.Attach(efPizza);
            entry.State = EntityState.Modified;

            try
            {
                await _context.CustomSaveChangesAsync(forceUpdate);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Pizzas.AnyAsync(p => p.Id == pizzaId))
                    throw new EntityNotFoundException("Pizza Not Found");
                else
                    throw;
            }

        }

        public async Task SoftDeleteAsync(int pizzaId, bool forceDelete, long lastVerison)
        {

            if (!await _context.Pizzas.AnyAsync(p => p.Id == pizzaId))
                throw new EntityNotFoundException("Pizza Not Found");

            var efPizza = _context.Pizzas.AsNoTracking().FirstOrDefault(p => p.Id == pizzaId);

            if(efPizza != null)
            {
                byte[] last = BitConverter.GetBytes(lastVerison);
                if (!efPizza.RowVersion.SequenceEqual(last))
                    efPizza.RowVersion = BitConverter.GetBytes(DateTime.Now.Ticks);
                _context.Pizzas.Remove(efPizza);

                try
                {
                    await _context.CustomSaveChangesAsync(forceDelete);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
        }
    }
}