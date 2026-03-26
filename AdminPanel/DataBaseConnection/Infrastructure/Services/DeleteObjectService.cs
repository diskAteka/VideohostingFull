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
    public class DeleteObjectService : IDeleteObjectService
    {
        private readonly VideohostingDbContext _context;
        public DeleteObjectService(VideohostingDbContext context)
        {
            _context = context;
        }

        public DeleteResult Delete<T>(int id) where T : class, IDeleteble
        {
            try
            {
                var dbSet = _context.Set<T>(); //Определяети тип таблицы на лету
                var entity = dbSet.Find(id);
                if (entity == null)
                    return new DeleteResult(false, null); //Если сущность не найдена, возвращаем неудачный результат с пустым массивом
                dbSet.Remove(entity);
                int affectedRows = _context.SaveChanges();
                return new DeleteResult(affectedRows > 0, AffectedTableNames()); //Если удаление прошло успешно, возвращаем результат с информацией об измененных таблицах
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении объекта типа {typeof(T)}, {ex.Message}");
                return new DeleteResult(false, null); //При возникновении ошибки возвращаем неудачный результат с пустым массивом
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
    }//Класс - сервис отвечающий за удаление сущностей из БД.
     //Реализует интерфейс IDeleteObjectService, который определяет методы для удаления пользователей, видео и комментариев.
     //Каждый метод обрабатывает удаление соответствующей сущности и возвращает результат в виде структуры DeleteResult,
     //которая содержит информацию об успешности операции и затронутых таблицах.
}
