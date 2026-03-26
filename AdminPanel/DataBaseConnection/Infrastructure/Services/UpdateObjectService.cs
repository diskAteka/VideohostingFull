using DataBaseConnection.Core.Domain.Interfaces;
using DataBaseConnection.Core.Domain.Models;
using DataBaseConnection.Core.Domain.ResultModels;
using DataBaseConnection.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseConnection.Infrastructure.Services
{
    public class UpdateObjectService : IUpdateObjectService
    {
        private VideohostingDbContext _context;
        public UpdateObjectService(VideohostingDbContext context)
        {
            _context = context;
        }

        public UpdateRezult Update<T>(T table) where T : class, IUpdateble //Наследование от IUpdateble гарантирует, что передаваемый объект будет либо Video, либо User, либо Comment
        {
            try
            {
                var dbSet = _context.Set<T>();
                _context.Entry(table).CurrentValues.SetValues(table);
                int affectedRows = _context.SaveChanges();
                return new UpdateRezult(affectedRows > 0, AffectedTableNames());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении объекта: {ex.Message}");
                return new UpdateRezult(false, null);
            }
        }

        private List<string> AffectedTableNames()
        {
            return _context.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Unchanged)
                .Select(e => e.Metadata.GetTableName())
                .Distinct()
                .ToList();
        }
    }//Класс - сервис отвечающий за обновление сущностей в БД.
}
