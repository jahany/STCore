using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static STCore.CLS.QueryValues;

namespace STCore.Interfaces
{
    public interface IQueries
    {
        IDAL dataaccesslayer { get; }
        int RowsAffected { get; set; }
        void Delete(string table_name, string condition, Cmd_Parameters p);
        void Insert(string table_name, List<string> values, object o);
        void Insert<T>(string table_name, T o) where T : new();
        long Insert_retLastId<T>(string table_name, T o) where T : new();
        void Bulk_Insert<T>(string table_name, T o) where T : new();
        void Update(string table_name, List<string> values, string condition, object o, Cmd_Parameters p);
        void Update<T>(string table_name, T o, string condition, Cmd_Parameters p) where T : new();
        void Update(IUD_Parameters p, string condition);
        Task<List<T>> Execute_List_Greedy<T>(string sql, T o, Cmd_Parameters p) where T : new();
        Task<T> Execute_List_Greedy_OneObject<T>(string sql, T o, Cmd_Parameters p) where T : new();
        T Json_Converter<T>(string json);
        List<T> Execute_List_From_Json<T>(string sql, Cmd_Parameters p) where T : new();
        Task<List<T>> Execute_List_From_Json_async<T>(string sql, Cmd_Parameters p) where T : new();
        T Execute_Object_From_Json<T>(string sql, T o, Cmd_Parameters p) where T : new();
        List<T> Execute_List_Greedy<T>(IDataReader dr, T o, Cmd_Parameters p) where T : new();
        Task<Tuple<T, IDataReader>> Execute_Greedy_OneObject_multiSelect<T>(string sql, T o, Cmd_Parameters p) where T : new();
        Task<string> Execute_Json(string sql, Cmd_Parameters p);
        string ToString(object item);
        ICRUD CRUD();
        ILogger Logger();
    }
    public interface ICRUD
    {
        IDAL dataaccesslayer { get; }
        void Insert<T>(string CrudProcedureName, T o) where T : new();
        void Update<T>(string CrudProcedureName, T o) where T : new();
        string SelectScaler(string CrudProcedureName, Cmd_Parameters p);
        List<T> Select<T>(string CrudProcedureName, Cmd_Parameters p) where T : new();
        List<T> Select_Paging<T>(string CrudProcedureName, Cmd_Parameters p, int PageNumber, int Tedad) where T : new();
        T SelectOneObject<T>(string CrudProcedureName, Cmd_Parameters p) where T : new();
        string Select_json(string CrudProcedureName, Cmd_Parameters p);
        Task<string> Select_json_Async(string CrudProcedureName, Cmd_Parameters p);
        void Execute(string CrudProcedureName, Cmd_Parameters p);
        void Execute(string CrudProcedureName);
        void ExecuteAsync(string CrudProcedureName, Cmd_Parameters p);
        void Delete(string CrudProcedureName, Cmd_Parameters p);
    }
    public interface ILogger
    {
        void SaveLog(ControllerBase context, string Message);
        void SaveAuthLog(ControllerBase context, string message);
    }
}
