using DataBaseConnection.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseConnection.Infrastructure.Interfaces
{
    public interface IGetObjectService
    {
        List<T> GetObjects<T>(int from, int to, bool ascending = true) where T : class , IModel;

        int GetCountOfRecords<T>() where T : class, IModel;

        public IModel GetObject<T>(int id) where T : class, IModel;
    }
}
