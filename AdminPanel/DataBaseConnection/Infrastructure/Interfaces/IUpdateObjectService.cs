using DataBaseConnection.Core.Domain.Interfaces;
using DataBaseConnection.Core.Domain.Models;
using DataBaseConnection.Core.Domain.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseConnection.Infrastructure.Interfaces
{
    public interface IUpdateObjectService
    {
        public UpdateRezult Update<T>(T table) where T : class, IUpdateble;
    }
}
