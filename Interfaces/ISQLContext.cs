using System.Collections.Generic;
using System.Threading.Tasks;
using static STCore.CLS.QueryValues;

namespace STCore.Interfaces
{
    public interface IQueriesDefault
    {
        int selectMax_Id(string table_name);
        bool IsExist(string table_name, string where_p, Cmd_Parameters p);
        int Get_Back_Number_Of_Pages(int tedad, string table);
        bool Delete(string table_name, string condition, Cmd_Parameters p);
        Task<bool> Insert(string table_name, List<string> values, object o);
        Task<bool> Update(string table_name, List<string> values, string condition, object o, Cmd_Parameters p);
        int Insert_select_last_id(string table_name, List<string> values, object o);
        Task<T> Execute_Object<T>(string sql, T o, Cmd_Parameters p) where T : class, new();
        Task<List<T>> Execute_List_All<T>(string sql, T o, Cmd_Parameters p);
        Task<List<T>> Execute_List_Greedy<T>(string sql, T o, Cmd_Parameters p);
        Task<List<T>> Execute_List<T>(string sql, T o, Cmd_Parameters p, int count);
        Task<List<T>> Execute_List_Custom<T>(string sql, T o, Cmd_Parameters p, int count, List<string> Columns);
        Task<List<dynamic>> Execute_Dynamic_List(string sql, Cmd_Parameters p);
    }
}

