using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseConnection.Core.Domain.ResultModels
{
    public struct UpdateRezult(bool success, List<string> affectedTables)
    {
        public bool Success { get; } = success;
        public List<string> AffectedTables { get; } = affectedTables;
    }
}
