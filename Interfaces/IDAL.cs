using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace STCore.Interfaces
{
    public interface IDAL
    {
        IDAL DirectInstance();
        SqlCommand cmd { get; set; }
        DataTable dt { get; set; }
        string Err { get; }
        void disconnect();
        Task<object> executescaler(string sql);
        Task<int> executenonquery(string sql);
        int executenonqueryNoAsync(string sql);
        Task<DataTable> select(string sql);
        Task<SqlDataReader> executereader(string sql);
        Task<IEnumerable<Dictionary<string, object>>> executeReader_Json(string sql);
        void executenonquery(DataTable dt, string tablename);
    }
}
