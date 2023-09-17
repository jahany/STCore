using Newtonsoft.Json;
using STCore.Attributes;
using STCore.CLS;
using STCore.connect;
using STCore.Exceptions;
using STCore.Interfaces;
using STCore.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static STCore.CLS.QueryValues;

namespace STCore.SQL
{

    public class Queries : IQueries
    {
        public IDAL dataaccesslayer => _dal;
        private IDAL _dal;
        IQuerySetValues IQSV;
        ILogger ILog;
        CRUD crud;
        public Queries(IDAL _Idal, ILogger _Ilog)
        {
            IQSV = new QueryValues();
            _dal = _Idal;
            ILog = _Ilog;
            crud = new CRUD(dataaccesslayer);
        }
        private int _rowsaffected { get; set; }

        public int RowsAffected
        {
            get => _rowsaffected;
            set => _rowsaffected = value;
        }

        public async virtual void Delete(string table_name, string condition, Cmd_Parameters p)
        {
            dataaccesslayer.cmd.CommandType = CommandType.Text;
            dataaccesslayer.cmd.Parameters.Clear();
            IQSV.Fill_Cmd(dataaccesslayer, p);
            string sql = "delete From " + table_name + " where " + condition + "";
            try
            {
                var res = await dataaccesslayer.executenonquery(sql);
                dataaccesslayer.cmd.Parameters.Clear();
                if (res < 0)
                    throw new SQLDeleteException("Item Not Deleted", new SQLDeleteException("error"));
            }
            catch
            {
                throw new SQLDeleteException("Item Not Deleted", new SQLDeleteException("error"));
            }
        }
        public virtual async void Insert(string table_name, List<string> values, object o)
        {
            dataaccesslayer.cmd.CommandType = CommandType.Text;
            System.Type type = o.GetType();
            List<string> columns = type.GetProperty("Columns", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(o) as List<string>;
            List<int> data_type = type.GetProperty("Types", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(o) as List<int>;
            dataaccesslayer.cmd.Parameters.Clear();
            string sql = "insert into " + table_name + " (";
            for (int i = 0; i < columns.Count; i++)
            {
                if (IQSV.Is_Identy_Param(type, columns[i].ToString()))
                {
                    continue;
                }
                if (i + 1 != columns.Count)
                {
                    sql += columns[i] + ",";
                }
                else
                {
                    sql += columns[i];
                }
            }
            sql += ") values (";
            for (int i = 0; i < columns.Count; i++)
            {
                if (IQSV.Is_Identy_Param(type, columns[i].ToString()))
                {
                    continue;
                }
                if (i + 1 != columns.Count)
                {
                    sql += "@" + columns[i] + ",";
                }
                else
                {
                    sql += "@" + columns[i];
                }
            }
            sql += ")";
            try
            {
                IQSV.Fill_Cmd(dataaccesslayer, new Cmd_Parameters(columns, data_type, values));
                await dataaccesslayer.executenonquery(sql);
                dataaccesslayer.cmd.Parameters.Clear();
            }
            catch
            {
                throw new SQLInsertException("NoItemInserted", new SQLInsertException("-1"));
            }
        }
        public virtual void Bulk_Insert<T>(string table_name, T o) where T : new()
        {
            dataaccesslayer.cmd.CommandType = CommandType.StoredProcedure;
            DataTable dt = CreateDataTable<T>(new List<T>() { o });
            dataaccesslayer.executenonquery(dt, table_name);
        }
        public virtual async void Insert<T>(string table_name, T o) where T : new()
        {
            dataaccesslayer.cmd.CommandType = CommandType.Text;
            Type type = typeof(T);
            var properties = GetTrueColumnsPropertiesForInsert(type);

            string sql = "insert into " + table_name + " (";
            for (int i = 0; i < properties.Count; i++)
            {
                if (i + 1 != properties.Count)
                {
                    sql += properties[i].Name + ",";
                }
                else
                {
                    sql += properties[i].Name;
                }
            }
            sql += ") values (";
            for (int i = 0; i < properties.Count; i++)
            {
                if (i + 1 != properties.Count)
                {
                    sql += "@" + properties[i].Name + ",";
                }
                else
                {
                    sql += "@" + properties[i].Name;
                }
            }
            sql += ")";
            for (int i = 0; i < properties.Count; i++)
            {
                dataaccesslayer.cmd.Parameters.Add("@" + properties[i].Name + "", QueryHelper.GetDbType(properties[i].PropertyType)).Value = properties[i].GetValue(o);
            }
            try
            {
                dataaccesslayer.executenonqueryNoAsync(sql);
                dataaccesslayer.cmd.Parameters.Clear();
            }
            catch
            {
                throw new SQLInsertException("Item Not Insterted", new SQLInsertException("-1"));
            }
        }
        public virtual long Insert_retLastId<T>(string table_name, T o) where T : new()
        {
            dataaccesslayer.cmd.CommandType = CommandType.Text;
            Type type = typeof(T);
            var properties = GetTrueColumnsPropertiesForInsert(type);
            string sql = "insert into " + table_name + " (";
            for (int i = 0; i < properties.Count; i++)
            {
                if (i + 1 != properties.Count)
                {
                    sql += properties[i].Name + ",";
                }
                else
                {
                    sql += properties[i].Name;
                }
            }
            sql += ") values (";
            for (int i = 0; i < properties.Count; i++)
            {
                if (i + 1 != properties.Count)
                {
                    sql += "@" + properties[i].Name + ",";
                }
                else
                {
                    sql += "@" + properties[i].Name;
                }
            }
            sql += ") select Max(id) from " + table_name + "";
            dataaccesslayer.cmd.Parameters.Clear();
            for (int i = 0; i < properties.Count; i++)
            {
                var temp = properties[i].GetValue(o);
                if (temp != null)
                    dataaccesslayer.cmd.Parameters.Add("@" + properties[i].Name + "", QueryHelper.GetDbType(properties[i].PropertyType)).Value = properties[i].GetValue(o);
                else
                    dataaccesslayer.cmd.Parameters.Add("@" + properties[i].Name + "", QueryHelper.GetDbType(properties[i].PropertyType)).Value = DBNull.Value;
            }
            var res = (dataaccesslayer.executescaler(sql).Result.ToString());
            if (long.Parse(res) > 0)
            {
                return long.Parse(res);
            }
            else
            {
                throw new SQLInsertException("Item Not Insterted", new SQLInsertException("-1"));
            }
        }
        public static DataTable CreateDataTable<T>(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = GetTrueColumnsPropertiesForInsert(type);

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Count()];
                for (int i = 0; i < properties.Count(); i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
        public async virtual void Insert(IUD_Parameters p)
        {
            dataaccesslayer.cmd.CommandType = CommandType.Text;
            string sql = "insert into " + p.table_name + " (";
            for (int i = 0; i < p.Columns.Count; i++)
            {
                if (i + 1 != p.Columns.Count)
                {
                    sql += p.Columns[i] + ",";
                }
                else
                {
                    sql += p.Columns[i];
                }
            }
            sql += ") values (";
            for (int i = 0; i < p.Columns.Count; i++)
            {
                if (i + 1 != p.Columns.Count)
                {
                    sql += "@" + p.Columns[i] + ",";
                }
                else
                {
                    sql += "@" + p.Columns[i];
                }
            }
            sql += ")";
            try
            {
                IQSV.Fill_Cmd(dataaccesslayer, new Cmd_Parameters(p.Columns, p.Types, p.Values));
                await dataaccesslayer.executenonquery(sql);
            }
            catch
            {
                throw new SQLInsertException("Item Not Insterted", new SQLInsertException("-1"));
            }

        }
        public async virtual void Update(string table_name, List<string> values, string condition, object o, Cmd_Parameters p)
        {
            dataaccesslayer.cmd.CommandType = CommandType.Text;
            System.Type type = o.GetType();
            List<string> columns = type.GetProperty("Columns", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(o) as List<string>;
            List<int> data_type = type.GetProperty("Types", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(o) as List<int>;
            dataaccesslayer.cmd.Parameters.Clear();
            string sql = "update " + table_name + " SET ";
            for (int i = 0; i < columns.Count; i++)
            {
                if (IQSV.Is_Identy_Param(type, columns[i].ToString()))
                {
                    continue;
                }
                if (i + 1 != columns.Count)
                {
                    sql += columns[i] + "=@" + columns[i] + ",";
                }
                else
                {
                    sql += columns[i] + "=@" + columns[i];
                }
            }
            sql += "where " + condition + "";
            try
            {
                IQSV.Fill_Cmd(dataaccesslayer, new Cmd_Parameters(columns, data_type, values));
                await dataaccesslayer.executenonquery(sql);
                dataaccesslayer.cmd.Parameters.Clear();
            }
            catch
            {
                throw new SQLUpdateException("Item Not Updated", new SQLUpdateException("error"));
            }

        }
        public async virtual void Update<T>(string table_name, T o, string condition, Cmd_Parameters p) where T : new()
        {
            dataaccesslayer.cmd.CommandType = CommandType.Text;
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }

            Type type = typeof(T);
            var properties = GetTrueColumnsPropertiesForUpdate(type);

            string sql = "update " + table_name + " SET ";
            bool flag = false;
            for (int i = 0; i < properties.Count; i++)
            {
                if (properties[i].GetValue(o) as object != null)
                {
                    if (properties[i].GetValue(o) as long? != -10)
                    {
                        if (!sql.EndsWith(',') && flag == true)
                        {
                            sql += ",";
                        }
                        flag = true;
                        bool temp = false;
                        object[] attrs = properties[i].GetCustomAttributes(true);
                        //temp = attrs.Count() == 0 ? false : true;
                        //foreach (object attr in attrs)
                        //{
                        //    ColumnName colattr = attr as ColumnName;
                        //    if (colattr != null)
                        //    {
                        //        if (i + 1 != properties.Count && properties[i + 1].GetValue(o) as object != null)
                        //        {
                        //            sql += colattr.columnName + "=@" + properties[i].Name + ",";
                        //        }
                        //        else
                        //        {
                        //            sql += colattr.columnName + "=@" + properties[i].Name;
                        //        }
                        //    }
                        //}
                        //if (temp == false)
                        //{
                        if (i + 1 != properties.Count && properties[i + 1].GetValue(o) as object != null)
                        {
                            sql += properties[i].Name + "=@" + properties[i].Name + ",";
                        }
                        else
                        {
                            sql += properties[i].Name + "=@" + properties[i].Name;
                        }
                        //}
                    }
                }

            }
            sql += " where " + condition + "";
            for (int i = 0; i < properties.Count; i++)
            {
                var temp = properties[i].GetValue(o);
                if (temp != null && properties[i].GetValue(o) as int? != -10)
                    dataaccesslayer.cmd.Parameters.Add("@" + properties[i].Name + "", QueryHelper.GetDbType(properties[i].PropertyType)).Value = properties[i].GetValue(o);
                else
                    dataaccesslayer.cmd.Parameters.Add("@" + properties[i].Name + "", QueryHelper.GetDbType(properties[i].PropertyType)).Value = DBNull.Value;
            }
            try
            {
                int res = await dataaccesslayer.executenonquery(sql);
                dataaccesslayer.cmd.Parameters.Clear();
                if (res < 0)
                {
                    throw new SQLUpdateException("Item Not Updated", new SQLUpdateException("error"));
                }
            }
            catch
            {
                throw new SQLUpdateException("Item Not Updated", new SQLUpdateException("error"));
            }
        }
        public async virtual void Update(IUD_Parameters p, string condition)
        {
            dataaccesslayer.cmd.CommandType = CommandType.Text;
            dataaccesslayer.cmd.Parameters.Clear();
            string sql = "update " + p.table_name + " SET ";
            for (int i = 0; i < p.Columns.Count; i++)
            {
                if (i + 1 != p.Columns.Count)
                {
                    sql += p.Columns[i] + "=@" + p.Columns[i] + ",";
                }
                else
                {
                    sql += p.Columns[i] + "=@" + p.Columns[i];
                }
            }
            sql += " where " + condition + "";
            try
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
                RowsAffected = await dataaccesslayer.executenonquery(sql);
                if (RowsAffected < 0)
                {
                    throw new SQLUpdateException("Item Not Updated", new SQLUpdateException("error"));
                }
            }
            catch
            {
                throw new SQLUpdateException("Item Not Updated", new SQLUpdateException("error"));
            }
        }


        public async Task<List<T>> Execute_List_Greedy<T>(string sql, T o, Cmd_Parameters p) where T : new()
        {
            dataaccesslayer.cmd.CommandType = CommandType.Text;
            List<T> list = new List<T>();
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            //DataTable dt =awa dataaccesslayer.select(sql);
            SqlDataReader dr = await dataaccesslayer.executereader(sql);
            //System.Type type = o.GetType();
            //while (dr.Read())
            //{
            //    T objo = Activator.CreateInstance<T>();
            //    IQSV.Set_ValtoClass(type, dr.FieldCount - 1, dr, objo);
            //    list.Add(objo);
            //}
            var r = Serialize(dr);
            string json = JsonConvert.SerializeObject(r, Formatting.Indented);
            list = Json_Converter<List<T>>(json);
            dataaccesslayer.disconnect();
            return list;
        }
        public List<T> Execute_List_From_Json<T>(string sql, Cmd_Parameters p) where T : new()
        {
            dataaccesslayer.cmd.CommandType = CommandType.Text;
            dataaccesslayer.disconnect();
            List<T> list = new List<T>();
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            SqlDataReader dr = dataaccesslayer.executereader(sql).Result;///Null Report

            StringBuilder sb = new StringBuilder();
            try
            {
                while (dr.Read())
                    sb.Append(dr.GetSqlString(0).Value);
            }
            catch
            {
                return new List<T>();
            }
            string json = sb.ToString();
            dataaccesslayer.disconnect();
            list = Json_Converter<List<T>>(json);
            if (list == null)
            {
                return new List<T>();
            }
            return list;
        }
        public async Task<List<T>> Execute_List_From_Json_async<T>(string sql, Cmd_Parameters p) where T : new()
        {
            dataaccesslayer.cmd.CommandType = CommandType.Text;
            dataaccesslayer.disconnect();
            List<T> list = new List<T>();
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            SqlDataReader dr =await dataaccesslayer.executereader(sql);///Null Report

            StringBuilder sb = new StringBuilder();
            try
            {
                while (await dr.ReadAsync())
                    sb.Append(dr.GetSqlString(0).Value);
            }
            catch
            {
                return new List<T>();
            }
            string json = sb.ToString();
            dataaccesslayer.disconnect();
            list = Json_Converter<List<T>>(json);
            if (list == null)
            {
                return new List<T>();
            }
            return list;
        }
        public async Task<string> Execute_Json(string sql, Cmd_Parameters p)
        {
            dataaccesslayer.cmd.CommandType = CommandType.Text;
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            //DataTable dt =awa dataaccesslayer.select(sql);
            SqlDataReader dr = await dataaccesslayer.executereader(sql);
            var r = Serialize(dr);
            return JsonConvert.SerializeObject(r, Formatting.Indented);
        }

        public List<T> Execute_List_Greedy<T>(IDataReader dr, T o, Cmd_Parameters p) where T : new()
        {
            dataaccesslayer.cmd.CommandType = CommandType.Text;
            List<T> list = new List<T>();
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            System.Type type = o.GetType();
            while (dr.Read())
            {
                T objo = (T)Activator.CreateInstance(typeof(T));
                IQSV.Set_ValtoClass(type, dr.FieldCount - 1, dr as SqlDataReader, objo);
                list.Add(objo);
            }
            dataaccesslayer.disconnect();
            return list;
        }
        public async Task<T> Execute_List_Greedy_OneObject<T>(string sql, T o, Cmd_Parameters p) where T : new()
        {
            dataaccesslayer.cmd.CommandType = CommandType.Text;
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            //DataTable dt =awa dataaccesslayer.select(sql);
            SqlDataReader dr = await dataaccesslayer.executereader(sql);
            System.Type type = o.GetType();
            while (dr.Read())
            {
                IQSV.Set_ValtoClass(type, dr.FieldCount - 1, dr, o);
            }
            dataaccesslayer.disconnect();
            return o;
        }
        public async Task<Tuple<T, IDataReader>> Execute_Greedy_OneObject_multiSelect<T>(string sql, T o, Cmd_Parameters p) where T : new()
        {
            dataaccesslayer.cmd.CommandType = CommandType.Text;
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            //DataTable dt =awa dataaccesslayer.select(sql);
            SqlDataReader dr = await dataaccesslayer.executereader(sql);
            System.Type type = o.GetType();
            while (dr.Read())
            {
                IQSV.Set_ValtoClass(type, dr.FieldCount - 1, dr, o);
            }
            //dataaccesslayer.disconnect();
            return new Tuple<T, IDataReader>(o, dr);
        }
        public T Json_Converter<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static List<PropertyInfo> GetTrueColumnsPropertiesForInsert(Type type)
        {
            return type.GetProperties(BindingFlags.Public |
                BindingFlags.DeclaredOnly |
                BindingFlags.Instance).Where(prop => !prop.IsDefined(typeof(Identity), false)
                && !prop.IsDefined(typeof(IFileStream), false)
                && !prop.IsDefined(typeof(CantInsert), false)).ToList();
        }
        private static List<PropertyInfo> GetTrueColumnsPropertiesForUpdate(Type type)
        {
            return type.GetProperties(BindingFlags.Public |
                BindingFlags.DeclaredOnly |
                BindingFlags.Instance).Where(prop => !prop.IsDefined(typeof(Identity), false)
                && !prop.IsDefined(typeof(IFileStream), false)
                && !prop.IsDefined(typeof(CantUpdate))).ToList();
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
        public IEnumerable<Dictionary<string, object>> Serialize(SqlDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++)
                cols.Add(reader.GetName(i));

            while (reader.Read())
                results.Add(SerializeRow(cols, reader));

            return results;
        }
        private Dictionary<string, object> SerializeRow(IEnumerable<string> cols,
                                                        SqlDataReader reader)
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
                result.Add(col, reader[col]);
            return result;
        }

        public string ToString(object item)
        {
            if (item == null)
            {
                return "NULL";
            }
            else
                return item.ToString();
        }

        public T Execute_Object_From_Json<T>(string sql, T o, Cmd_Parameters p) where T : new()
        {
            dataaccesslayer.cmd.CommandType = CommandType.Text;
            dataaccesslayer.disconnect();
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            SqlDataReader dr = dataaccesslayer.executereader(sql).Result;///Null Report

            StringBuilder sb = new StringBuilder();
            try
            {
                while (dr.Read())
                    sb.Append(dr.GetSqlString(0).Value);
            }
            catch
            {
                return default(T);
            }
            string json = sb.ToString();
            dataaccesslayer.disconnect();
            o = Json_Converter<T>(json);
            if (o == null)
            {
                return default(T);
            }
            return o;
        }

        public ICRUD CRUD()
        {
            return crud;
        }

        public ILogger Logger()
        {
            return ILog;
        }
    }
    /// <summary>
    /// have object of cmd parameters and its set for params 
    /// It is use 
    /// the class must have definition of all columns in table of sqlserver 
    /// </summary>
    public class Queries_Default : IQueriesDefault
    {
        STCore.Interfaces.IDAL dataaccesslayer;
        IQuerySetValues IQSV;
        public Queries_Default(IDAL _Idal)
        {
            IQSV = new QueryValues();
            dataaccesslayer = _Idal;
        }
        public virtual int selectMax_Id(string table_name)
        {
            string sql = "select MAX(id) from " + table_name + "";
            return dataaccesslayer.executescaler(sql).Result == null ? -1 : int.Parse(dataaccesslayer.executescaler(sql).Result.ToString());
        }
        public virtual bool IsExist(string table_name, string where_p, Cmd_Parameters p)
        {

            IQSV.Fill_Cmd(dataaccesslayer, p);
            string sql = "select count(*) from " + table_name + " where " + where_p + "";
            string temp = dataaccesslayer.executescaler(sql).Result as string;
            dataaccesslayer.cmd.Parameters.Clear();
            if (temp == "0")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public virtual int Get_Back_Number_Of_Pages(int tedad, string table)
        {
            dataaccesslayer.cmd.Parameters.Clear();
            int Counter = 0, All;
            string SQL = "Select COUNT(id) From " + table + "";
            Counter = (dataaccesslayer.executescaler(SQL).Result as int?).Value;
            dataaccesslayer.cmd.Parameters.Clear();
            dataaccesslayer.disconnect();
            if (Counter % tedad == 0)
            {
                All = Counter / tedad;
            }
            else
            {
                All = Counter / tedad;
                All = All + 1;
            }
            return All;
        }
        public virtual bool Delete(string table_name, string condition, Cmd_Parameters p)
        {
            dataaccesslayer.cmd.Parameters.Clear();
            IQSV.Fill_Cmd(dataaccesslayer, p);
            string sql = "delete From " + table_name + " where " + condition + "";
            try
            {
                dataaccesslayer.executenonquery(sql);
                dataaccesslayer.cmd.Parameters.Clear();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public virtual async Task<bool> Insert(string table_name, List<string> values, object o)
        {
            System.Type type = o.GetType();
            List<string> columns = type.GetProperty("Columns", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(o) as List<string>;
            List<int> data_type = type.GetProperty("Types", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(o) as List<int>;
            dataaccesslayer.cmd.Parameters.Clear();
            string sql = "insert into " + table_name + " (";
            for (int i = 0; i < columns.Count; i++)
            {
                if (IQSV.Is_Identy_Param(type, columns[i].ToString()))
                {
                    continue;
                }
                if (i + 1 != columns.Count)
                {
                    sql += columns[i] + ",";
                }
                else
                {
                    sql += columns[i];
                }
            }
            sql += ") values (";
            for (int i = 0; i < columns.Count; i++)
            {
                if (IQSV.Is_Identy_Param(type, columns[i].ToString()))
                {
                    continue;
                }
                if (i + 1 != columns.Count)
                {
                    sql += "@" + columns[i] + ",";
                }
                else
                {
                    sql += "@" + columns[i];
                }
            }
            sql += ")";
            try
            {
                IQSV.Fill_Cmd(dataaccesslayer, new Cmd_Parameters(columns, data_type, values));
                await dataaccesslayer.executenonquery(sql);
                dataaccesslayer.cmd.Parameters.Clear();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async virtual Task<bool> Update(string table_name, List<string> values, string condition, object o, Cmd_Parameters p)
        {
            System.Type type = o.GetType();
            List<string> columns = type.GetProperty("Columns", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(o) as List<string>;
            List<int> data_type = type.GetProperty("Types", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(o) as List<int>;
            dataaccesslayer.cmd.Parameters.Clear();
            string sql = "update " + table_name + " SET ";
            for (int i = 0; i < columns.Count; i++)
            {
                if (IQSV.Is_Identy_Param(type, columns[i].ToString()))
                {
                    continue;
                }
                if (i + 1 != columns.Count)
                {
                    sql += columns[i] + "=@" + columns[i] + ",";
                }
                else
                {
                    sql += columns[i] + "=@" + columns[i];
                }
            }
            sql += "where " + condition + "";
            try
            {
                IQSV.Fill_Cmd(dataaccesslayer, new Cmd_Parameters(columns, data_type, values));
                await dataaccesslayer.executenonquery(sql);
                dataaccesslayer.cmd.Parameters.Clear();
                return true;
            }
            catch
            {
                return false;
            }

        }
        public virtual int Insert_select_last_id(string table_name, List<string> values, object o)
        {
            System.Type type = o.GetType();
            List<string> columns = type.GetProperty("Columns", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(o) as List<string>;
            List<int> data_type = type.GetType().GetProperty("Types", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(o) as List<int>;
            dataaccesslayer.cmd.Parameters.Clear();
            string sql = "insert into " + table_name + " (";
            for (int i = 0; i < columns.Count; i++)
            {
                if (IQSV.Is_Identy_Param(type, columns[i].ToString()))
                {
                    continue;
                }
                if (i + 1 != columns.Count)
                {
                    sql += columns[i] + ",";
                }
                else
                {
                    sql += columns[i];
                }
            }
            sql += ") values (";
            for (int i = 0; i < columns.Count; i++)
            {
                if (IQSV.Is_Identy_Param(type, columns[i].ToString()))
                {
                    continue;
                }
                if (i + 1 != columns.Count)
                {
                    sql += "@" + columns[i] + ",";
                }
                else
                {
                    sql += "@" + columns[i];
                }
            }
            sql += ") select Max(id) from " + table_name + "";
            IQSV.Fill_Cmd(dataaccesslayer, new Cmd_Parameters(columns, data_type, values));
            return (dataaccesslayer.executescaler(sql).Result as int?).Value;
        }
        public async Task<List<T>> Execute_List_All<T>(string sql, T o, Cmd_Parameters p)
        {
            List<T> list = new List<T>();
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            SqlDataReader dr = await dataaccesslayer.executereader(sql);
            System.Type type = o.GetType();
            List<string> columns = type.GetProperty("Columns", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(o) as List<string>;
            while (dr.Read())
            {
                T objo = (T)Activator.CreateInstance(typeof(T));
                IQSV.Set_ValtoClass(type, columns.Count - 1, dr, objo);
                list.Add(objo);
            }
            dataaccesslayer.disconnect();
            return list;
        }
        public async Task<List<T>> Execute_List_Greedy<T>(string sql, T o, Cmd_Parameters p)
        {
            List<T> list = new List<T>();
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            SqlDataReader dr = await dataaccesslayer.executereader(sql);
            System.Type type = o.GetType();
            while (dr.Read())
            {
                T objo = (T)Activator.CreateInstance(typeof(T));
                IQSV.Set_ValtoClass(type, dr.FieldCount - 1, dr, objo);
                list.Add(objo);
            }
            dataaccesslayer.disconnect();
            return list;
        }
        public async Task<List<T>> Execute_List<T>(string sql, T o, Cmd_Parameters p, int count)
        {
            List<T> list = new List<T>();
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            SqlDataReader dr = await dataaccesslayer.executereader(sql);
            System.Type type = o.GetType();
            List<string> columns = type.GetProperty("Columns", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(o) as List<string>;
            // List<int> data_type = type.GetProperty("Types", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(o) as List<int>;
            while (dr.Read())
            {
                T objo = (T)Activator.CreateInstance(typeof(T));
                IQSV.Set_ValtoClass(type, count - 1, dr, objo);
                list.Add(objo);
            }
            dataaccesslayer.disconnect();
            return list;
        }
        public async Task<List<T>> Execute_List_Custom<T>(string sql, T o, Cmd_Parameters p, int count, List<string> Columns)
        {
            List<T> list = new List<T>();
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            SqlDataReader dr = await dataaccesslayer.executereader(sql);
            System.Type type = o.GetType();
            List<string> columns = Columns;
            // List<int> data_type = type.GetProperty("Types", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(o) as List<int>;
            while (dr.Read())
            {
                T objo = (T)Activator.CreateInstance(typeof(T));
                IQSV.Set_ValtoClass(type, count - 1, dr, objo);
                list.Add(objo);
            }
            dataaccesslayer.disconnect();
            return list;
        }
        public async Task<List<dynamic>> Execute_Dynamic_List(string sql, Cmd_Parameters p)
        {
            List<dynamic> list = new List<dynamic>();
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            SqlDataReader dr = await dataaccesslayer.executereader(sql);
            while (dr.Read())
            {
                var dic = new Dictionary<string, object>();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    string name = dr.GetName(i);
                    object m = dr.GetValue(i);
                    dic.Add(dr.GetName(i), dr.GetValue(i));
                }
                var post = new DynamicEntity(dic);
                list.Add(post);
            }
            dataaccesslayer.disconnect();
            return list;
        }
        public async Task<T> Execute_Object<T>(string sql, T o, Cmd_Parameters p) where T : class, new()
        {
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            SqlDataReader dr = await dataaccesslayer.executereader(sql);
            System.Type type = o.GetType();
            List<string> columns = type.GetProperty("Columns", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(o) as List<string>;
            // List<int> data_type = type.GetProperty("Types", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy).GetValue(o) as List<int>;
            if (dr.HasRows == false)
                return null;
            else
            {
                while (dr.Read())
                {
                    T objo = new T();
                    IQSV.Set_ValtoClass(type, dr.FieldCount - 1, dr, objo);
                    o = objo;
                }
                dataaccesslayer.disconnect();
                return o;
            }
        }
    }
    /// <summary>
    /// no object of cmd parameters
    /// </summary>
    public class Queries_Custom : QueryValues
    {
        STCore.Interfaces.IDAL dataaccesslayer;
        public Queries_Custom(IDAL _Idal)
        {
            dataaccesslayer = _Idal;
        }
        public int RowsAffected = 0;
        public virtual async Task<bool> Insert_No_Protect(IUD_Parameters p)
        {
            dataaccesslayer.cmd.Parameters.Clear();
            string sql = "insert into " + p.table_name + " (";
            for (int i = 0; i < p.Columns.Count; i++)
            {
                if (i + 1 != p.Columns.Count)
                {
                    sql += p.Columns[i] + ",";
                }
                else
                {
                    sql += p.Columns[i];
                }
            }
            sql += ") values (";
            for (int i = 0; i < p.Values.Count; i++)
            {
                if (i + 1 != p.Values.Count)
                {
                    sql += "'";
                    sql = string.Join("\x200E", sql, p.Values[i]);
                    sql += "',";
                }
                else
                {
                    sql += "'";
                    sql = string.Join("\x200E", sql, p.Values[i]);
                    sql += "'";
                }
            }
            sql += ")";
            try
            {
                //IQSV.Fill_Cmd(dataaccesslayer, p);
                await dataaccesslayer.executenonquery(sql);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async virtual Task<bool> Insert(IUD_Parameters p)
        {
            string sql = "insert into " + p.table_name + " (";
            for (int i = 0; i < p.Columns.Count; i++)
            {
                if (i + 1 != p.Columns.Count)
                {
                    sql += p.Columns[i] + ",";
                }
                else
                {
                    sql += p.Columns[i];
                }
            }
            sql += ") values (";
            for (int i = 0; i < p.Columns.Count; i++)
            {
                if (i + 1 != p.Columns.Count)
                {
                    sql += "@" + p.Columns[i] + ",";
                }
                else
                {
                    sql += "@" + p.Columns[i];
                }
            }
            sql += ")";
            try
            {
                Fill_Cmd(dataaccesslayer, new Cmd_Parameters(p.Columns, p.Types, p.Values));
                await dataaccesslayer.executenonquery(sql);
                return true;
            }
            catch
            {
                return false;
            }

        }
        public async virtual Task<bool> Update(IUD_Parameters p, string condition)
        {
            dataaccesslayer.cmd.Parameters.Clear();
            string sql = "update " + p.table_name + " SET ";
            for (int i = 0; i < p.Columns.Count; i++)
            {
                if (i + 1 != p.Columns.Count)
                {
                    sql += p.Columns[i] + "=@" + p.Columns[i] + ",";
                }
                else
                {
                    sql += p.Columns[i] + "=@" + p.Columns[i];
                }
            }
            sql += " where " + condition + "";
            try
            {
                Fill_Cmd(dataaccesslayer, p);
                RowsAffected = await dataaccesslayer.executenonquery(sql);
                return true;
            }
            catch (Exception x)
            {
                return false;
            }
        }
        public virtual async Task<bool> Delete(IUD_Parameters p, string condition)
        {
            dataaccesslayer.cmd.Parameters.Clear();
            string sql = "delete From " + p.table_name + " where " + condition + "";
            try
            {
                Fill_Cmd(dataaccesslayer, p.SetParameters);
                await dataaccesslayer.executenonquery(sql);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public virtual async Task<int> Insert_select_last_id(IUD_Parameters p)
        {
            string sql = "insert into " + p.table_name + " (";
            for (int i = 0; i < p.Columns.Count; i++)
            {
                if (i + 1 != p.Columns.Count)
                {
                    sql += p.Columns[i] + ",";
                }
                else
                {
                    sql += p.Columns[i];
                }
            }
            sql += ") values (";
            for (int i = 0; i < p.Columns.Count; i++)
            {
                if (i + 1 != p.Columns.Count)
                {
                    sql += "@" + p.Columns[i] + ",";
                }
                else
                {
                    sql += "@" + p.Columns[i];
                }
            }
            sql += ") select Max(id) from " + p.table_name + "";
            dataaccesslayer.cmd.Parameters.Clear();
            Fill_Cmd(dataaccesslayer, new Cmd_Parameters(p.Columns, p.Types, p.Values));
            return (await dataaccesslayer.executescaler(sql) as int?).Value;
        }
        public virtual async Task<int> Get_Back_Number_Of_Pages(int tedad, string table)
        {
            dataaccesslayer.cmd.Parameters.Clear();
            int Counter = 0, All;
            string SQL = "Select COUNT(id) From " + table + "";
            Counter = (await dataaccesslayer.executescaler(SQL) as int?).Value;
            dataaccesslayer.cmd.Parameters.Clear();
            dataaccesslayer.disconnect();
            if (Counter % tedad == 0)
            {
                All = Counter / tedad;
            }
            else
            {
                All = Counter / tedad;
                All = All + 1;
            }
            return All;
        }
        public virtual async Task<int> Get_Back_Number_Of_Pages(int tedad, string table, string with_where_p, Cmd_Parameters p)
        {
            dataaccesslayer.cmd.Parameters.Clear();
            Fill_Cmd(dataaccesslayer, p);
            int Counter = 0, All;
            string SQL = "Select COUNT(id) From " + table + " " + with_where_p + "";
            Counter = (await dataaccesslayer.executescaler(SQL) as int?).Value;
            dataaccesslayer.cmd.Parameters.Clear();
            dataaccesslayer.disconnect();
            if (Counter % tedad == 0)
            {
                All = Counter / tedad;
            }
            else
            {
                All = Counter / tedad;
                All = All + 1;
            }
            return All;
        }
        /// <summary>
        /// زمانی که تعتداد را از سرور نمی خوانیم
        /// </summary>
        /// <param name="tedad"></param>
        /// <param name="PageNumber"></param>
        /// <returns></returns>
        public virtual int Get_Back_Number_Of_Pages(int tedad, int PageNumber)
        {
            int Counter = 0, All;
            Counter = PageNumber;
            if (Counter % tedad == 0)
            {
                All = Counter / tedad;
            }
            else
            {
                All = Counter / tedad;
                All = All + 1;
            }
            return All;
        }
        //public List<T> Execute_List<T>(string sql, T o, List<string> columns, List<int> data_type)
        //{
        //    List<T> list = new List<T>();
        //    SqlDataReader dr = dataaccesslayer.executereader(sql);
        //    System.Type type = o.GetType();
        //    while (dr.Read())
        //    {
        //        IQSV.Set_ValtoClass(type, columns, dr, o);
        //        list.Add(o);
        //    }
        //    dataaccesslayer.disconnect();
        //    return list;
        //}
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// برای صفحه بندی استفاده میشود
        /// </summary>
        /// <param name="page_number">در کدام صفحه قرار داریم</param>
        /// <param name="tedad">تعداد در هر پست</param>
        /// <param name="columns">نام ستون ها برای انتخاب</param>
        /// <param name="table">نام جدول</param>
        /// <param name="condition_with_where">شرط به همراه where</param>
        /// <returns>کویری را بر می گرداند</returns>
        public virtual string safhe_bandi(int page_number, int tedad, string columns, string table, string condition_with_where)
        {
            int shoro;
            if (page_number == 1)
            {
                shoro = 0;
            }
            else
            {
                shoro = ((page_number - 1) * tedad) + 1;
            }
            //////////////////////////////////////////////////////////////////////////////////////////
            string sql = "DECLARE @shoro INT ; " +
                         "DECLARE @tedad INT ; " +
                         "SET @shoro = " + shoro + "; " +
                         "SET @tedad = " + tedad + " ; " +
                         "select * from (select " + columns + ",ROW_NUMBER() OVER (ORDER BY " + table + ".id  asc) as row " +
                         "from " + table + " " + condition_with_where + ") jadval " +
                         "where jadval.row>=@shoro and jadval.row<=@shoro+@tedad;";
            return sql;
        }
        /// <summary>
        /// صفحه بندی برای چندین جدول که join  شده اند
        /// </summary>
        /// <param name="page_number">شماره صحفه ای که در ان قرار داریم</param>
        /// <param name="tedad">تعداد پست در هر صفحه</param>
        /// <param name="columns">نام ستون ها برای انتخاب</param>
        /// <param name="table">نام جدول</param>
        /// <param name="id_toSort">نام ای دی برای مرتب سازی به همراه نام جدول</param>
        /// <param name="condition_with_where">شرط به همراه where</param>
        /// <returns>کیویری را بر می گرداند</returns>
        public virtual string safhe_bandi_for_innner_join(int page_number, int tedad, string columns, string table, string id_toSort, string condition_with_where)
        {
            int shoro;
            if (page_number == 1)
            {
                shoro = 0;
            }
            else
            {
                shoro = ((page_number - 1) * tedad) + 1;
            }
            //////////////////////////////////////////////////////////////////////////////////////////
            string sql = "DECLARE @shoro INT ; " +
                         "DECLARE @tedad INT ; " +
                         "SET @shoro = " + shoro + "; " +
                         "SET @tedad = " + tedad + " ; " +
                         "select * from (select " + columns + ",ROW_NUMBER() OVER (ORDER BY " + id_toSort + "  desc) as row " +
                         "from " + table + " " + condition_with_where + ") jadval " +
                         "where jadval.row>=@shoro and jadval.row<=@shoro+@tedad;";
            return sql;
        }
        /// <summary>
        /// برای جست و جو استفاده می شود
        /// </summary>
        /// <param name="columns">نام ستون ها برای انتخاب</param>
        /// <param name="table">نام جدول</param>
        /// <param name="name_col">نام ستونی که می خواهیم  در ان سرچ کنیم</param>
        /// <param name="key">کلمه ی جست و جو شده</param>
        /// <returns></returns>
        public virtual string search(string columns, string table, string name_col, string key)
        {
            string sql = "select " + columns + " from " + table + " where " + name_col + " like '%" + key + "%'";
            return sql;
        }
        /// <summary>
        /// برای جست و جو استفاده می شود
        /// </summary>
        /// <param name="columns">نام ستون ها برای انتخاب</param>
        /// <param name="table">نام جدول</param>
        /// <param name="name_col">نام ستونی که می خواهیم  در ان سرچ کنیم</param>
        /// <param name="key">کلمه ی جست و جو شده</param>
        /// <param name="where">شرط بدون where</param>
        /// <returns></returns>
        public virtual string search(string columns, string table, string name_col, string key, string where)
        {
            string sql = "select " + columns + " from " + table + " where " + name_col + " like '%" + key + "%' and " + where + "";
            return sql;
        }
        /// <summary>
        /// دریافت یک لیست بین دو تاریخ
        /// </summary>
        /// <param name="columns">نام ستون ها برای انتخاب</param>
        /// <param name="table">نام جدول</param>
        /// <param name="col_to_select">ستونی که در ان تاریخ ها موجود است برای تبدیل به تاریخ</param>
        /// <param name="first_t">اولین تاریخ</param>
        /// <param name="secend_t">تاریخ دوم</param>
        /// <returns>کیویری را بر می گرداند</returns>
        public virtual string sqlquery_between(string columns, string table, string col_to_select, string first_t, string secend_t)
        {
            string sql = "select " + columns + " from " + table + " where CONVERT(date," + col_to_select + ",111) between '" + first_t + "' and '" + secend_t + "'";
            return sql;
        }
        /// <summary>
        /// دریافت یک لیست بین دو تاریخ
        /// </summary>
        /// <param name="columns">نام ستون ها برای انتخاب</param>
        /// <param name="table">نام جدول</param>
        /// <param name="col_to_select">ستونی که در ان تاریخ ها موجود است برای تبدیل به تاریخ</param>
        /// <param name="first_t">اولین تاریخ</param>
        /// <param name="secend_t">تاریخ دوم</param>
        /// <param name="condition_with_and">شرط به همراه and</param>
        /// <returns>کیویری را بر می گرداند</returns>
        public virtual string sqlquery_between(string columns, string table, string col_to_select, string first_t, string secend_t, string condition_with_and)
        {
            string sql = "select " + columns + " from " + table + " where CONVERT(date," + col_to_select + ",111) between '" + first_t + "' and '" + secend_t + "' " + condition_with_and + "";
            return sql;
        }
        /// <summary>
        /// دریافت لیستی کمتر از تاریخ مشخص
        /// </summary>
        /// <param name="columns">نام ستون ها برای انتخاب</param>
        /// <param name="table">نام جدول</param>
        /// <param name="col_to_select">ستونی که در ان تاریخ ها موجود است برای تبدیل به تاریخ</param>
        /// <param name="tarikh">تاریخ مشخص</param>
        /// <returns></returns>
        public virtual string sqlquery_Lessthan(string columns, string table, string col_to_select, string tarikh)
        {
            string sql = "select " + columns + " from " + table + " where CONVERT(date," + col_to_select + ",111) < '" + tarikh + "'";
            return sql;
        }
        /// <summary>
        /// دریافت لیستی کمتر از تاریخ مشخص
        /// </summary>
        /// <param name="columns">نام ستون ها برای انتخاب</param>
        /// <param name="table">نام جدول</param>
        /// <param name="col_to_select">ستونی که در ان تاریخ ها موجود است برای تبدیل به تاریخ</param>
        /// <param name="tarikh">تاریخ مشخص</param>
        /// <param name="condition_with_and">شرط به همراه and</param>
        /// <returns></returns>
        public virtual string sqlquery_Lessthan(string columns, string table, string col_to_select, string tarikh, string condition_with_and)
        {
            string sql = "select " + columns + " from " + table + " where CONVERT(date," + col_to_select + ",111) < '" + tarikh + "' " + condition_with_and + "";
            return sql;
        }
        /// <summary>
        /// دریافت لیستی بیشتر از تاریخ مشخص
        /// </summary>
        /// <param name="columns">نام ستون ها برای انتخاب</param>
        /// <param name="table">نام جدول</param>
        /// <param name="col_to_select">ستونی که در ان تاریخ ها موجود است برای تبدیل به تاریخ</param>
        /// <param name="tarikh">تاریخ مشخص</param>
        /// <returns></returns>
        public virtual string sqlquery_Morethan(string columns, string table, string col_to_select, string tarikh)
        {
            string sql = "select " + columns + " from " + table + " where CONVERT(date," + col_to_select + ",111) > '" + tarikh + "'";
            return sql;
        }        /// <summary>
                 /// دریافت لیستی بیشتر از تاریخ مشخص
                 /// </summary>
                 /// <param name="columns">نام ستون ها برای انتخاب</param>
                 /// <param name="table">نام جدول</param>
                 /// <param name="col_to_select">ستونی که در ان تاریخ ها موجود است برای تبدیل به تاریخ</param>
                 /// <param name="tarikh">تاریخ مشخص</param>
                 /// <param name="condition_with_and">شرط به همراه and</param>
                 /// <returns></returns>
        public virtual string sqlquery_Morethan(string columns, string table, string col_to_select, string tarikh, string condition_with_and)
        {
            string sql = "select " + columns + " from " + table + " where CONVERT(date," + col_to_select + ",111) > '" + tarikh + "' " + condition_with_and + "";
            return sql;
        }
        public async Task<List<dynamic>> Execute_Dynamic_List(string sql, Cmd_Parameters p)
        {
            List<dynamic> list = new List<dynamic>();
            if (p.Columns != null)
            {
                Fill_Cmd(dataaccesslayer, p);
            }
            SqlDataReader dr = await dataaccesslayer.executereader(sql);
            while (dr.Read())
            {
                var dic = new Dictionary<string, object>();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    string name = dr.GetName(i);
                    object m = dr.GetValue(i);
                    if (m != System.DBNull.Value)
                    {
                        dic.Add(dr.GetName(i), dr.GetValue(i));
                    }
                    else
                    {
                        dic.Add(dr.GetName(i), "");
                    }
                }
                var post = new DynamicEntity(dic);
                list.Add(post);
            }
            dataaccesslayer.disconnect();
            return list;
        }
    }

    public static class QueryHelper
    {
        private static Dictionary<Type, SqlDbType> typeMap;

        // Create and populate the dictionary in the static constructor
        static QueryHelper()
        {
            typeMap = new Dictionary<Type, SqlDbType>();

            typeMap[typeof(string)] = SqlDbType.NVarChar;
            typeMap[typeof(char[])] = SqlDbType.NVarChar;
            typeMap[typeof(byte)] = SqlDbType.TinyInt;
            typeMap[typeof(short)] = SqlDbType.SmallInt;
            typeMap[typeof(int)] = SqlDbType.Int;
            typeMap[typeof(long)] = SqlDbType.BigInt;
            typeMap[typeof(byte[])] = SqlDbType.Image;
            typeMap[typeof(bool)] = SqlDbType.Bit;
            typeMap[typeof(DateTime)] = SqlDbType.DateTime2;
            typeMap[typeof(DateTimeOffset)] = SqlDbType.DateTimeOffset;
            typeMap[typeof(decimal)] = SqlDbType.Money;
            typeMap[typeof(float)] = SqlDbType.Real;
            typeMap[typeof(double)] = SqlDbType.Float;
            typeMap[typeof(TimeSpan)] = SqlDbType.Time;
            typeMap[typeof(Guid)] = SqlDbType.UniqueIdentifier;
            /* ... and so on ... */
        }

        public static SqlDbType GetDbType(Type giveType)
        {
            // Allow nullable types to be handled
            giveType = Nullable.GetUnderlyingType(giveType) ?? giveType;

            if (typeMap.ContainsKey(giveType))
            {
                return typeMap[giveType];
            }

            throw new ArgumentException($"{giveType.FullName} is not a supported .NET class");
        }

        public static SqlDbType GetDbType<T>()
        {
            return GetDbType(typeof(T));
        }
    }

    public class CRUD : ICRUD
    {
        public IDAL dataaccesslayer;
        IQuerySetValues IQSV;

        IDAL ICRUD.dataaccesslayer => dataaccesslayer;

        public CRUD(IDAL dal)
        {
            dataaccesslayer = dal;
            IQSV = new QueryValues();
        }
        public void Delete(string CrudProcedureName, Cmd_Parameters p)
        {
            dataaccesslayer.cmd.Parameters.Clear();

            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            dataaccesslayer.cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                dataaccesslayer.executenonqueryNoAsync(CrudProcedureName);
            }
            catch
            {
                throw new SQLDeleteException("Item Not Deleted", new SQLInsertException("-1"));
            }
        }

        public void Execute(string CrudProcedureName, Cmd_Parameters p)
        {
            dataaccesslayer.cmd.Parameters.Clear();

            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            dataaccesslayer.cmd.CommandType = CommandType.StoredProcedure;
            dataaccesslayer.executenonqueryNoAsync(CrudProcedureName);
        }
        public void Execute(string CrudProcedureName)
        {
            dataaccesslayer.cmd.CommandType = CommandType.StoredProcedure;
            dataaccesslayer.executenonqueryNoAsync(CrudProcedureName);
        }
        public async void ExecuteAsync(string CrudProcedureName, Cmd_Parameters p)
        {
            dataaccesslayer.cmd.Parameters.Clear();

            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            dataaccesslayer.cmd.CommandType = CommandType.StoredProcedure;
            await dataaccesslayer.executenonquery(CrudProcedureName);
        }

        public void Insert<T>(string CrudProcedureName, T o) where T : new()
        {
            dataaccesslayer.cmd.Parameters.Clear();
            Type type = typeof(T);
            var properties = GetTrueColumnsPropertiesForInsert(type);

            string sql = CrudProcedureName;
            dataaccesslayer.cmd.CommandType = CommandType.StoredProcedure;
            for (int i = 0; i < properties.Count; i++)
            {
                dataaccesslayer.cmd.Parameters.Add("@" + properties[i].Name + "", QueryHelper.GetDbType(properties[i].PropertyType)).Value = properties[i].GetValue(o);
            }
            try
            {
                int res = dataaccesslayer.executenonqueryNoAsync(sql);
                if (res == 0)
                {
                    throw new SQLInsertException("Item Not Insterted", new SQLInsertException("-1"));
                }
            }
            catch
            {
                throw new SQLInsertException("Item Not Insterted", new SQLInsertException("-1"));
            }
        }
        public string SelectScaler(string CrudProcedureName, Cmd_Parameters p)
        {
            dataaccesslayer.disconnect();
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            dataaccesslayer.cmd.CommandType = CommandType.StoredProcedure;
            return dataaccesslayer.executescaler(CrudProcedureName).Result != null ? dataaccesslayer.executescaler(CrudProcedureName).Result.ToString() : "0";
        }
        public List<T> Select<T>(string CrudProcedureName, Cmd_Parameters p) where T : new()
        {
            dataaccesslayer.cmd.Parameters.Clear();
            dataaccesslayer.disconnect();
            List<T> list = new List<T>();
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            dataaccesslayer.cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = dataaccesslayer.executereader(CrudProcedureName).Result;///Null Report

            StringBuilder sb = new StringBuilder();
            try
            {
                while (dr.Read())
                    sb.Append(dr.GetSqlString(0).Value);
            }
            catch
            {
                return new List<T>();
            }

            string json = sb.ToString();
            dataaccesslayer.disconnect();
            list = Json_Converter<List<T>>(json);
            if (list == null)
            {
                return new List<T>();
            }
            return list;
        }
        public List<T> Select_Paging<T>(string CrudProcedureName, Cmd_Parameters p, int PageNumber, int Tedad) where T : new()
        {
            p.Columns.Add("PageNumber");
            p.Types.Add(4);
            p.Values.Add(PageNumber.ToString());

            p.Columns.Add("howmany");
            p.Types.Add(11);
            p.Values.Add(Tedad.ToString());

            dataaccesslayer.disconnect();
            List<T> list = new List<T>();
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            dataaccesslayer.cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = dataaccesslayer.executereader(CrudProcedureName).Result;///Null Report

            StringBuilder sb = new StringBuilder();
            try
            {
                while (dr.Read())
                    sb.Append(dr.GetSqlString(0).Value);
            }
            catch
            {
                return new List<T>();
            }

            string json = sb.ToString();
            dataaccesslayer.disconnect();
            list = Json_Converter<List<T>>(json);
            if (list == null)
            {
                return new List<T>();
            }
            return list;
        }
        public T SelectOneObject<T>(string CrudProcedureName, Cmd_Parameters p) where T : new()
        {
            dataaccesslayer.cmd.Parameters.Clear();
            dataaccesslayer.disconnect();
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            dataaccesslayer.cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = dataaccesslayer.executereader(CrudProcedureName).Result;///Null Report
            T o = new T();
            StringBuilder sb = new StringBuilder();
            try
            {
                while (dr.Read())
                    sb.Append(dr.GetSqlString(0).Value);
            }
            catch
            {
                return default(T);
            }

            string json = sb.ToString();
            dataaccesslayer.disconnect();
            o = Json_Converter<T>(json);
            if (o == null)
            {
                return default(T);
            }
            return o;
        }

        public void Update<T>(string CrudProcedureName, T o) where T : new()
        {
            dataaccesslayer.cmd.Parameters.Clear();

            Type type = typeof(T);
            var properties = GetTrueColumnsPropertiesForUpdate(type);

            string sql = CrudProcedureName;
            dataaccesslayer.cmd.CommandType = CommandType.StoredProcedure;
            for (int i = 0; i < properties.Count; i++)
            {
                dataaccesslayer.cmd.Parameters.Add("@" + properties[i].Name + "", QueryHelper.GetDbType(properties[i].PropertyType)).Value = properties[i].GetValue(o);
            }
            try
            {
                int res = dataaccesslayer.executenonqueryNoAsync(sql);
                if (res == -1)
                {
                    throw new SQLUpdateException("Item Not Updated", new SQLInsertException("-1"));
                }
            }
            catch
            {
                throw new SQLUpdateException("Item Not Updated", new SQLInsertException("-1"));
            }
        }


        private static List<PropertyInfo> GetTrueColumnsPropertiesForInsert(Type type)
        {
            return type.GetProperties(BindingFlags.Public |
                BindingFlags.DeclaredOnly |
                BindingFlags.Instance).Where(prop => !prop.IsDefined(typeof(Identity), false)
                && !prop.IsDefined(typeof(IFileStream), false)
                && !prop.IsDefined(typeof(CantInsert), false)).ToList();
        }
        private static List<PropertyInfo> GetTrueColumnsPropertiesForUpdate(Type type)
        {
            return type.GetProperties(BindingFlags.Public |
                BindingFlags.DeclaredOnly |
                BindingFlags.Instance).Where(prop => !prop.IsDefined(typeof(IFileStream), false)
                && !prop.IsDefined(typeof(CantUpdate))).ToList();
        }
        private T Json_Converter<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string Select_json(string CrudProcedureName, Cmd_Parameters p)
        {
            dataaccesslayer.cmd.Parameters.Clear();
            dataaccesslayer.disconnect();
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            dataaccesslayer.cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = dataaccesslayer.executereader(CrudProcedureName).Result;///Null Report

            StringBuilder sb = new StringBuilder();
            try
            {
                while (dr.Read())
                    sb.Append(dr.GetSqlString(0).Value);
            }
            catch
            {
                return null;
            }

            dataaccesslayer.disconnect();
            return sb.ToString();
        }

        public async Task<string> Select_json_Async(string CrudProcedureName, Cmd_Parameters p)
        {

            dataaccesslayer.disconnect();
            if (p.Columns != null)
            {
                IQSV.Fill_Cmd(dataaccesslayer, p);
            }
            dataaccesslayer.cmd.CommandType = CommandType.StoredProcedure;
            dataaccesslayer.cmd.Parameters.Clear();
            SqlDataReader dr =await dataaccesslayer.executereader(CrudProcedureName);///Null Report

            StringBuilder sb = new StringBuilder();
            try
            {
                while (dr.Read())
                    sb.Append(dr.GetSqlString(0).Value);
            }
            catch
            {
                return null;
            }

            string json = sb.ToString();
            dataaccesslayer.disconnect();
            return json;
        }


    }
}