using DataBaseConnection.Core.Domain.Interfaces;
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
    public class AddObjectService : IAddObjectService
    {
        private VideohostingDbContext _context;
        public AddObjectService(VideohostingDbContext context)
        {
            _context = context;
        }

        public AddRezult Add<T>(T table) where T : class, IAddble
        {
            try
            {
                var dbSet = _context.Set<T>();
                dbSet.Add(table);
                int affectedRows = _context.SaveChanges();
                return new AddRezult(affectedRows > 0, AffectedTableNames());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении объекта: {ex.Message}");
                return new AddRezult(false, null);
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
    }//Класс - сервис отвечающий за добавление сущностей в БД.
     //Реализует интерфейс IAddObjectService, который определяет методы для добавления записей в любые таблицы, кроме таблицы ServerLog.
     //Generic метод реализует добавление сущности и возвращает результат в виде структуры AddRezult,
     //которая содержит информацию об успешности операции и затронутых таблицах.
}
