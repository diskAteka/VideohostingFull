using DataBaseConnection.Core.Domain.Interfaces;
using DataBaseConnection.Core.Domain.ResultModels;

namespace DataBaseConnection.Infrastructure.Interfaces
{
    public interface IDeleteObjectService
    {
        //Этот интерфейс должен отвечать за удаление любых сущностей в БД

        public DeleteResult Delete<T>(int id) where T : class, IDeleteble;
    }
}
