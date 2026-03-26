using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VideohostingPanel.Classes
{
    public enum Tables
    {
        Video,
        User,
        ServerLog,
        Comment,
        Like,
        DisLike,
        View,
        Emploee
    } // НЕ МЕНЯТЬ ПОРЯДОК!
    // Порядок этого перечисления должен совпадать с порядком таблиц в базе данных, так как он используется для получения данных из БД и отображения их в интерфейсе

    public enum Pages
    {
        EditComment,
        EditUser,
        EditVideo,
        EditEmloyee,
        Verify
    }

    public enum Windows
    {
        MainWindow,
        Login, 
        ViewLog
    }

    public enum ForeignKeys
    {
        User,
        Video
    }

    public enum Roles
    {
        Admin,
        Verifier
    }

}
