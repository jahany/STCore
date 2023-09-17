using System;
using System.Data;
using System.Data.SqlClient;
using static STCore.CLS.QueryValues;

namespace STCore.Services
{
    interface IQuerySetValues : IDisposable
    {
        // 0 BigInt
        // 1 NVarChar
        // 2 Int
        // 3 Text
        // 4 TinyInt
        // 5 Date
        // 6 DateTime
        // 7 bit
        // 8 float
        // 10 NULL
        // 11 SmallInt
        // 12 uniqIdenifire
        void Fill_Cmd(STCore.Interfaces.IDAL dataaccesslayer, IUD_Parameters p);
        void Fill_Cmd(STCore.Interfaces.IDAL dataaccesslayer, Cmd_Parameters p);
        void Fill_Cmd(STCore.Interfaces.IDAL dataaccesslayer, DataTable dt, int index);

        /////برای فهمیدن انکه
        /////property indentity 
        /////است یا نه
        bool Is_Identy_Param(Type type, string prop_Name);
        object Set_ValtoClass(System.Type type, int counttoselect, SqlDataReader dr, object o);
    }
}
