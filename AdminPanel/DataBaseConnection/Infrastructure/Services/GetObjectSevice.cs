using DataBaseConnection.Core.Domain.Interfaces;
using DataBaseConnection.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataBaseConnection.Infrastructure.Services
{
    public class GetObjectSevice : IGetObjectService
    {
        private VideohostingDbContext _context;
        public GetObjectSevice(VideohostingDbContext context)
        {
            _context = context;
        }
        public List<T> GetObjects<T>(int from = 0, int to = 10, bool ascending = true) where T : class, IModel
        {
            IQueryable<T> list = null;
            try
            {
                list = _context.Set<T>().AsNoTracking();

                // Применяем сортировку по Id
                list = ascending ? list.OrderBy(e => EF.Property<object>(e, "Id")) : list.OrderByDescending(e => EF.Property<object>(e, "Id"));

                return list.Skip(from).Take(to).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public int GetCountOfRecords<T>() where T : class, IModel
        {
            try
            {
                return _context.Set<T>().Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public IModel GetObject<T>(int id) where T : class, IModel
        {
            return _context.Set<T>().Find(id);
        }
    }
}
