using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseConnection.Core.Domain.ResultModels
{
    public struct DeleteResult(bool succses, List<string> affectedTables)
    {
        public bool Success { get; } = succses;
        public List<string> AffectedTables { get; } = affectedTables;
    }//Результат удаления записей представлен в виде структуру для сохранения семантики кода
}
