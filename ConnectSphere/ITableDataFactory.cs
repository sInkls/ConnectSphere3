using System.Data;

namespace SocialAppViewer
{
    public interface ITableDataFactory
    {
        DataTable LoadTable(string tableName);
        void SaveTable(string tableName, DataTable table);
    }
}