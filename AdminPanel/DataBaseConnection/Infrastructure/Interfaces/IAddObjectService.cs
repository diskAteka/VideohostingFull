using DataBaseConnection.Core.Domain.Interfaces;
using DataBaseConnection.Core.Domain.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseConnection.Infrastructure.Interfaces
{
    public interface IAddObjectService
    {
        public AddRezult Add<T>(T table) where T : class, IAddble; 
        //Наследование от интерфейса IAdd гарантирует что не будут переданы объекты,
        //которые не предназначены для добавления в базу данных
    }
}
