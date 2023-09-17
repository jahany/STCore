using STCore.Attributes;
using STCore.Services;
using STCore.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace STCore.CLS
{
    public class QueryValues : IQuerySetValues
    {
        public struct TIUD_Parameters
        {
            public List<IUD_Parameters> iud;
            public List<int> Type;////if 1 Insert if 2 Update if 3 Delete
            public List<string> condition;
        }
        public struct Cmd_Parameters
        {
            public List<string> Columns;
            public List<int> Types;
            public List<string> Values;
            public Cmd_Parameters(string[] _Columns, int[] _Types, string[] _Values)
            {
                IEnumerable<string> Col = _Columns;
                IEnumerable<int> Ty = _Types;
                IEnumerable<string> Val = _Values;
                Columns = new List<string>(Col);
                Types = new List<int>(Ty);
                Values = new List<string>(Val);
            }
            public Cmd_Parameters(List<string> _Columns, List<int> _Types, List<string> _Values)
            {
                Columns = _Columns;
                Types = _Types;
                Values = _Values;
            }
        }
        public struct IUD_Parameters
        {
            public string table_name;
            public List<string> Columns;
            public List<int> Types;
            public List<string> Values;
            public Cmd_Parameters SetParameters;
            public IUD_Parameters(string _table_name, string[] _Columns, int[] _Types, string[] _Values, Cmd_Parameters p)
            {
                IEnumerable<string> Col = _Columns;
                IEnumerable<int> Ty = _Types;
                IEnumerable<string> Val = _Values;
                Columns = new List<string>(Col);
                Types = new List<int>(Ty);
                Values = new List<string>(Val);
                table_name = _table_name;
                SetParameters = p;
            }
            public IUD_Parameters(string _table_name, List<string> _Columns, List<int> _Types, List<string> _Values, Cmd_Parameters p)
            {
                table_name = _table_name;
                SetParameters = p;
                Columns = _Columns;
                Types = _Types;
                Values = _Values;
            }

            /// <summary>
            /// Use For Delete One Row From sql server
            /// </summary>
            /// <param name="_table_name"></param>
            /// <param name="p"></param>
            public IUD_Parameters(string _table_name, Cmd_Parameters p)
            {
                table_name = _table_name;
                SetParameters = p;
                Columns = new List<string> { };
                Types = new List<int> { };
                Values = new List<string> { };
            }
        }
        public void Fill_Cmd(STCore.Interfaces.IDAL dataaccesslayer, IUD_Parameters p)
        {
            dataaccesslayer.cmd.Parameters.Clear();
            for (int i = 0; i < p.Types.Count; i++)
            {
                if (p.Types[i] == 0)
                {
                    dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.BigInt).Value = p.Values[i];
                }
                else if (p.Types[i] == 1)
                {
                    dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.NVarChar).Value = p.Values[i];
                }
                else if (p.Types[i] == 2)
                {
                    dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.Int).Value = p.Values[i];
                }
                else if (p.Types[i] == 3)
                {
                    dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.Text).Value = p.Values[i];
                }
                else if (p.Types[i] == 4)
                {
                    dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.TinyInt).Value = p.Values[i];
                }
                else if (p.Types[i] == 5)
                {
                    dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.Date).Value = p.Values[i];
                }
                else if (p.Types[i] == 6)
                {
                    dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.DateTime).Value = p.Values[i];
                }
                else if (p.Types[i] == 7)
                {
                    dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.Bit).Value = p.Values[i];
                }
                else if (p.Types[i] == 8)
                {
                    dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.Float).Value = p.Values[i];
                }
            }
            if (p.Columns != null || p.Types != null || p.Values != null)
            {
                //Fill_Cmd(dataaccesslayer, p.SetParameters);
            }
        }
        public void Fill_Cmd(STCore.Interfaces.IDAL dataaccesslayer, DataTable dt, int index)
        {
            dataaccesslayer.cmd.Parameters.Clear();
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                dataaccesslayer.cmd.Parameters.Add("@" + dt.Columns[j].ColumnName + "", QueryHelper.GetDbType(dt.Columns[j].DataType)).Value = dt.Rows[index][j];
            }
        }

        public void Fill_Cmd(STCore.Interfaces.IDAL dataaccesslayer, Cmd_Parameters p)
        {
            dataaccesslayer.cmd.Parameters.Clear();
            if (p.Types != null)
            {

                for (int i = 0; i < p.Types.Count; i++)
                {
                    if (p.Values[i] == null)
                    {
                        dataaccesslayer.cmd.Parameters.AddWithValue("@" + p.Columns[i] + "", DBNull.Value);
                        continue;
                    }

                    if (p.Types[i] == 0)
                    {
                        if (p.Values[i] == null)
                            dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.BigInt).Value = DBNull.Value;
                        else
                            dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.BigInt).Value = p.Values[i];
                    }
                    else if (p.Types[i] == 1)
                    {
                        dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.NVarChar).Value = p.Values[i];
                    }
                    else if (p.Types[i] == 2)
                    {
                        dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.Int).Value = p.Values[i];
                    }
                    else if (p.Types[i] == 3)
                    {
                        dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.Text).Value = p.Values[i];
                    }
                    else if (p.Types[i] == 4)
                    {
                        dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.TinyInt).Value = p.Values[i];
                    }
                    else if (p.Types[i] == 5)
                    {
                        dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.Date).Value = p.Values[i];
                    }
                    else if (p.Types[i] == 6)
                    {
                        dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.DateTime).Value = p.Values[i];
                    }
                    else if (p.Types[i] == 7)
                    {
                        dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.Bit).Value = p.Values[i];
                    }
                    else if (p.Types[i] == 8)
                    {
                        dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.Float).Value = p.Values[i];
                    }
                    else if (p.Types[i] == 10)
                    {
                        dataaccesslayer.cmd.Parameters.AddWithValue("@" + p.Columns[i] + "", DBNull.Value);
                    }
                    else if (p.Types[i] == 11)
                    {
                        dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.SmallInt).Value = Int16.Parse(p.Values[i]);
                    }
                    else if (p.Types[i] == 12)
                    {
                        dataaccesslayer.cmd.Parameters.Add("@" + p.Columns[i] + "", System.Data.SqlDbType.UniqueIdentifier).Value = Guid.Parse(p.Values[i]);
                    }
                }
            }
        }
        /////برای فهمیدن انکه
        /////property indentity 
        /////است یا نه
        public bool Is_Identy_Param(Type type, string prop_Name)
        {
            System.Type type1 = typeof(Identity);
            var attr = type.GetProperty(prop_Name.ToString(), BindingFlags.Public | BindingFlags.Instance).GetCustomAttribute(type1);
            Identity res = attr as Identity;
            if (res == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public object Set_ValtoClass(System.Type type, int counttoselect, SqlDataReader dr, object o)
        {
            try
            {
                int i;
                for (i = 0; i <= counttoselect; i++)
                {
                    var res = dr.GetName(i).ToString();
                    MethodInfo setMethod = type.GetProperty(res, BindingFlags.Public | BindingFlags.Instance).GetSetMethod();
                    if (setMethod == null)
                    {
                        continue;
                    }
                    var item = type.GetProperty(res, BindingFlags.Public | BindingFlags.Instance).PropertyType.Name;

                    if (item == typeof(Int64).Name)
                    {
                        type.GetProperty(res, BindingFlags.Public | BindingFlags.Instance).SetValue(o, dr.GetInt64(i));
                    }
                    else if (item == typeof(Int32).Name)
                    {
                        type.GetProperty(res, BindingFlags.Public | BindingFlags.Instance).SetValue(o, dr.GetInt32(i));
                    }
                    else if (item == typeof(int).Name)
                    {
                        type.GetProperty(res).SetValue(o, dr.GetInt16(i));
                    }
                    else if (item == typeof(string).Name)
                    {
                        type.GetProperty(res).SetValue(o, dr.GetValue(i).ToString());
                    }
                    else if (item == typeof(bool).Name)
                    {
                        type.GetProperty(res).SetValue(o, dr.GetBoolean(i));
                    }
                    else if (item == typeof(float).Name)
                    {
                        type.GetProperty(res).SetValue(o, dr.GetFloat(i));
                    }
                    //else if (item == typeof(long?).Name)
                    //{
                    //    try
                    //    {
                    //        type.GetProperty(res, BindingFlags.Public | BindingFlags.Instance).SetValue(o, dr.GetInt64(i));
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        type.GetProperty(res, BindingFlags.Public | BindingFlags.Instance).SetValue(o, null);
                    //    }
                    //}
                }
                return o;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
                return o;
            }

            #region
            //for (int i = 0; i < columns.Count; i++)
            //{
            //    try
            //    {
            //        if (dr.GetName(i) != columns[i])
            //        {
            //            if (type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).PropertyType.Name == typeof(Int64).Name)
            //            {
            //                type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).SetValue(o, null);
            //            }
            //            else if (type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).PropertyType.Name == typeof(Int32).Name)
            //            {
            //                type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).SetValue(o, null);
            //            }
            //            else if (type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).PropertyType.Name == typeof(int).Name)
            //            {
            //                type.GetProperty(columns[i].ToString()).SetValue(o, null);
            //            }
            //            else if (type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).PropertyType.Name == typeof(string).Name)
            //            {
            //                type.GetProperty(columns[i].ToString()).SetValue(o, "");
            //            }
            //            else if (type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).PropertyType.Name == typeof(bool).Name)
            //            {
            //                type.GetProperty(columns[i].ToString()).SetValue(o, false);
            //            }
            //        }

            //        if (type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).PropertyType.Name == typeof(Int64).Name)
            //        {
            //            type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).SetValue(o, dr.GetInt64(i));
            //        }
            //        else if (type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).PropertyType.Name == typeof(Int32).Name)
            //        {
            //            type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).SetValue(o, dr.GetInt32(i));
            //        }
            //        else if (type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).PropertyType.Name == typeof(int).Name)
            //        {
            //            type.GetProperty(columns[i].ToString()).SetValue(o, dr.GetInt16(i));
            //        }
            //        else if (type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).PropertyType.Name == typeof(string).Name)
            //        {
            //            type.GetProperty(columns[i].ToString()).SetValue(o, dr.GetValue(i));
            //        }
            //        else if (type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).PropertyType.Name == typeof(bool).Name)
            //        {
            //            type.GetProperty(columns[i].ToString()).SetValue(o, dr.GetBoolean(i));
            //        }
            //    }
            //    catch
            //    {
            //        if (type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).PropertyType.Name == typeof(Int64).Name)
            //        {
            //            type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).SetValue(o, null);
            //        }
            //        else if (type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).PropertyType.Name == typeof(Int32).Name)
            //        {
            //            type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).SetValue(o, null);
            //        }
            //        else if (type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).PropertyType.Name == typeof(int).Name)
            //        {
            //            type.GetProperty(columns[i].ToString()).SetValue(o, null);
            //        }
            //        else if (type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).PropertyType.Name == typeof(string).Name)
            //        {
            //            type.GetProperty(columns[i].ToString()).SetValue(o, "");
            //        }
            //        else if (type.GetProperty(columns[i].ToString(), BindingFlags.Public | BindingFlags.Instance).PropertyType.Name == typeof(bool).Name)
            //        {
            //            type.GetProperty(columns[i].ToString()).SetValue(o, false);
            //        }
            //    }
            //}
            //return o;
            #endregion
        }
        public void Dispose()
        {

        }
    }
}
