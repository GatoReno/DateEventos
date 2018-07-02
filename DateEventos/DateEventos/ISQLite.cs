using System;
using System.Collections.Generic;
using System.Text;

namespace DateEventos
{
    public interface ISQLite
    {
        SQLite.SQLiteConnection GetConnection();

    }
}
