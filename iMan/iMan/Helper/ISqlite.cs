
using SQLite;

namespace iMan.Helpers
{
    public interface ISqlite
    {
        SQLiteAsyncConnection GetConnection();
    }
}