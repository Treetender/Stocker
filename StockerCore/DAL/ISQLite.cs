using SQLite;

namespace StockerCore.DAL
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
