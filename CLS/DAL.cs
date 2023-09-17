using Microsoft.Extensions.Configuration;
using STCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace STCore.connect
{
    /// <summary>
    /// Class DAL.
    /// کلاس پایه ارتباط با دیتابیس mssql
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class DAL : IDAL, IDisposable///(Data Access Layer)
    {
        public IDAL DirectInstance()
        {
            cmd.CommandType = CommandType.Text;
            return this;
        }
        /// <summary>
        /// The con string
        /// رشته ای است که در ابتدای برنامه مقدار دهی میشود
        /// </summary>
        private static string con_string;
        SqlConnection con;
        private SqlCommand _cmd;
        public SqlCommand cmd { get => _cmd; set => _cmd = value; }
        SqlDataAdapter da;
        private DataTable _datatable;
        /// <summary>
        /// The trans
        /// برای هندل کردن RollBack
        /// </summary>
        private SqlTransaction Trans;
        /// <summary>
        /// The error
        /// زمانی که خطا رخ دهد این رشته پر می شود
        /// </summary>
        private string _Err = "";
        /// <summary>
        /// The req net
        /// اگر دیتا بیس لوکال نباشد این متغییر باید true شود
        /// </summary>
        public static bool Req_Net = false;
        public DataTable dt { get => _datatable; set => _datatable = value; }
        public string Err => _Err;

        /// <summary>
        /// Initializes a new instance of the <see cref="DAL"/> class.
        /// کانستراکتور کلاس است که متغییر ها را مقدار دهی میکند
        /// </summary>
        public DAL(IConfiguration conf)
        {
            con_string = conf.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            try
            {
                if (Req_Net == true)///اگر دیتابیس لوکال نباشد
                {
                    con = new SqlConnection();
                    con.ConnectionString = con_string;
                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandTimeout = 0;
                    da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    _datatable = new DataTable();

                }
                else
                {
                    con = new SqlConnection();
                    con.ConnectionString = con_string;
                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    _datatable = new DataTable();
                }
            }
            catch (Exception ex)
            {
                _Err = "خطا در اتصال به بانک لطفا از متصل بودن به اینترنت خود اطمینان پیدا کنید" + " \n " + ex.Message;
                Mediator_Dal.Get_Instance().OnError(this, this);///error event raise
            }
        }
        public DAL(string constr)
        {
            con_string = constr;
            try
            {
                if (Req_Net == true)///اگر دیتابیس لوکال نباشد
                {
                    con = new SqlConnection();
                    con.ConnectionString = con_string;
                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandTimeout = 0;
                    da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    _datatable = new DataTable();

                }
                else
                {
                    con = new SqlConnection();
                    con.ConnectionString = con_string;
                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    _datatable = new DataTable();
                }
            }
            catch (Exception ex)
            {
                _Err = "خطا در اتصال به بانک لطفا از متصل بودن به اینترنت خود اطمینان پیدا کنید" + " \n " + ex.Message;
                Mediator_Dal.Get_Instance().OnError(this, this);///error event raise
            }
        }
        /// <summary>
        /// Connects this instance.
        /// اتصال به دیتابیس را انجام میدهد
        /// </summary>
        void connect()
        {
            if (con.State == ConnectionState.Closed)///اگر کانکشن بسته شده باشد
            {
                con.Open();
            }
            else
            {
                //con.Close();
                //con.Open();
            }
        }
        /// <summary>
        /// Disconnects this instance.
        /// کانکشن جاری را از بین می برد
        /// </summary>
        public void disconnect()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        /// <summary>
        /// Executescalers the specified SQL.
        /// اسکریپت را در دیتابیس ران میگیرد و نتیجه واحد را بر میگرداند
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>String</returns>
        public async Task<object> executescaler(string sql)
        {
            object result;
            cmd.CommandText = sql;
            connect();
            try
            {
                cmd.CommandTimeout = 0;
                //cmd.Prepare();
                result = cmd.ExecuteScalar();
            }
            catch (Exception x)
            {
                result = null;
            }
            disconnect();
            return result;
        }
        ////////////////////////////////////////////////////method baraye insert,delete,update        
        /// <summary>
        /// Executenonqueries the specified SQL.
        /// برای 3 عمل اصلی در دیتابیس به کار می رود
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>System.String.</returns>
        public async Task<int> executenonquery(string sql)
        {
            int rowsaffeccted;
            cmd.CommandText = sql;
            connect();///اتصال به دیتابیس
            //Trans = con.BeginTransaction();
            cmd.Transaction = Trans;
            try
            {
                //cmd.Prepare();
                rowsaffeccted = await cmd.ExecuteNonQueryAsync();///execute script
                cmd.Dispose();///free Command
                //Trans.Commit();///کد بارگزاری شده در دیتابیس ران می شود
            }
            catch (FormatException ex)
            {
                throw;
            }
            catch (Exception e)///در صورت اررور در دیتابیس
            {
                //Trans.Rollback();///اعمال انجام شده به حالت قبلی در دیتابیس باز میگردد
                rowsaffeccted = -1;
            }
            disconnect();
            return rowsaffeccted;
        }
        public int executenonqueryNoAsync(string sql)
        {
            int rowsaffeccted;
            cmd.CommandText = sql;
            connect();///اتصال به دیتابیس
            try
            {
                //cmd.Prepare();
                rowsaffeccted = cmd.ExecuteNonQuery();///execute script
                cmd.Dispose();///free Command
            }
            catch (FormatException ex)
            {
                throw;
            }
            catch (Exception e)///در صورت اررور در دیتابیس
            {
                rowsaffeccted = -1;
            }
            disconnect();
            return rowsaffeccted;
        }
        public async void executenonquery(DataTable dt, string tablename)
        {
            connect();
            Trans = con.BeginTransaction();
            using (var sqlBulk = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, Trans))
            {
                sqlBulk.DestinationTableName = tablename;
                await sqlBulk.WriteToServerAsync(dt);
            }
        }
        //////////////////////////////////////////////////////////method baraye select//////////        
        /// <summary>
        /// Selects the specified SQL.
        /// برای انتخاب از دیتابیس استفاده می شود
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>DataTable.</returns>
        public async Task<DataTable> select(string sql)
        {
            dt = new DataTable();
            connect();
            da.SelectCommand.Connection = con;
            da.SelectCommand.CommandText = sql;
            try
            {
                // cmd.Prepare();
                await da.SelectCommand.ExecuteReaderAsync();
            }
            catch
            {
                da = null;
            }
            disconnect();
            da.Fill(dt);
            return dt;
        }
        /// <summary>
        /// Executereaders the specified SQL.
        /// برای انتخاب از سرور استفاده میشود
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>SqlDataReader.</returns>
        public async Task<SqlDataReader> executereader(string sql)
        {
            SqlDataReader DATAREADER = null;
            _cmd.CommandText = sql;
            connect();
            try
            {
                // cmd.Prepare();
                DATAREADER =await cmd.ExecuteReaderAsync();
            }
            catch (Exception x)
            {

            }
            return DATAREADER;
        }
        /// <summary>
        /// Executes the reader json.
        /// برای سرعت بخشدین به بخش کلاینت و سرور زمانی که داده ها را به فرمت 
        /// json
        /// تبدیل می کنند از این تابع استفاده شد که در ان برای به جای تبدیل به فرمت ذکر شده ان را به 
        /// یک دیکشنری تبدیل میکند
        /// که می توان به راحتی از نخ بندی و پردازش موازی استفاده کرد
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>دیکشنری شامل اسم و تایپ مرود استفاده</returns>
        public async Task<IEnumerable<Dictionary<string, object>>> executeReader_Json(string sql)
        {
            SqlDataReader DATAREADER = null;
            _cmd.CommandText = sql;
            connect();
            try
            {
                DATAREADER = await cmd.ExecuteReaderAsync();
            }
            catch (Exception ex)
            {

            }
            return Serialize(DATAREADER);///serialize datareader
        }
        /// <summary>
        /// Serializes the specified reader.(DataReader)
        /// برای تبدیل دیتا های خوانده شده از بافر دیتابیس ئ تبدیل انها به دیکشنری استفاده می شود
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>دیکشنری</returns>
        private IEnumerable<Dictionary<string, object>> Serialize(SqlDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            Task t = Task.Factory.StartNew(delegate///یک نخ را ایجاد میکند
            {
                Parallel.For(0, reader.FieldCount,
                    index =>
                    {
                        cols.Add(reader.GetName(index));///نام ستون ها را واکشی می کند و در متغییر می ریزد
                    });
            });
            Task.WaitAll(t);///صبر میکند تا نخ تمام شود

            while (reader.Read())
                results.Add(SerializeRow(cols, reader));

            return results;
        }
        private Dictionary<string, object> SerializeRow(IEnumerable<string> cols,
                                                        SqlDataReader reader)
        {
            var result = new Dictionary<string, object>();

            foreach (var col in cols)
            {
                if (reader[col] != null || reader[col].ToString() != "")
                    result.Add(col, reader[col]);
            }

            return result;
        }
        /// <summary>
        /// Class Mediator_Dal.
        /// این کلاس برای هندل کردن رخداد هایی که در کلاس 
        /// DAL
        /// رخ می دهد مورد استفاده قرار می گیرد
        /// </summary>
        public class Mediator_Dal
        {
            private static readonly Mediator_Dal _Instance = new Mediator_Dal();
            /// <summary>
            /// Gets the instance.
            /// یک نمونه شیء استاتیک از کلاس را بر میگرداند
            /// </summary>
            /// <returns>Mediator_Dal.</returns>
            public static Mediator_Dal Get_Instance()
            {
                return _Instance;
            }
            /// <summary>
            /// Occurs when [error].   
            /// رویداد اررور است که زمانی که
            /// delegate error raise
            /// و یا فعال شود رویداد اررور رخ می دهد
            /// </summary>
            public event EventHandler<DalEvent> Error;
            /// <summary>
            /// Called when [error].
            /// اگر در جایی از کد ارروری رخ دهد
            /// delegate error
            /// را فعال می کند
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="d">The d.</param>
            public void OnError(object sender, DAL d)
            {
                var DalDelegate = Error as EventHandler<DalEvent>;///رخداد اررور را به delegate متصل میکند
                if (DalDelegate != null)///اگر delegate فعال باشد
                {
                    DalDelegate.BeginInvoke(sender, new DalEvent { da = d }, null, null);///رویداد اررور فعال می شود
                }
            }
            /// <summary>
            /// Class DalEvent.
            /// زمانی که رویداد اررور رخ دهد از این کلاس برای پاس دادن اطلاعات مربوط به اررور استفاده می شود
            /// </summary>
            public class DalEvent
            {
                public connect.DAL da { get; set; }
            }
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// زمانی که نرم افزار در حال 
        /// release
        /// کردن فضای رک است این متر اجرا می شود
        /// </summary>
        public void Dispose()
        {
            cmd.Parameters.Clear();
            if (con.State == ConnectionState.Open)///اگر ارتباط با دیتا بیس برقرار باشد آن را از بین می برد
            {
                con.Close();
            }
        }
    }

    /// <summary>
    /// Dynamic class for serialize at RunTime
    /// زمانی که از دیتابیس جدول ره به صورت داینامیک می خواهیم بخوانیم از ابجکت های کلاس مرجع
    /// داینامیک برای اضافه کردن در لیست استفده می کنیم
    /// در اینجا این کلاس را دوباره ساختیم تا توابع آن را  
    /// override کنیم
    /// </summary>
    [Serializable]
    public class DynamicEntity : DynamicObject, ISerializable///کلاس توانایی سریالایز شدن را دارد و نیم تواند پدر کلاس درگری باشد
    {
        /// <summary>
        /// The values
        /// دیکشنری که از دیتابیس خوانده می شود
        /// شامل نام ستون ها و مقادیر در ان ستون ها است
        /// </summary>
        private IDictionary<string, object> _values;
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicEntity"/> class.
        /// تابع ساختار را تشکیل می دهد که در ان ورودی ها را به متغییر ست می کنیم
        /// </summary>
        /// <param name="values">The values.</param>
        public DynamicEntity(IDictionary<string, object> values)
        {
            _values = values;
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// زمانی که کلاس می خواهد سریال شود از این تابع استفاده میکند
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (var kvp in this._values)
            {
                info.AddValue(kvp.Key, kvp.Value);
            }
        }
        /// <summary>
        /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
        /// متد 
        /// try setmember
        /// در کلاس اصلی 
        /// ovverride
        /// شده است
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, the <paramref name="value" /> is "Test".</param>
        /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this._values[binder.Name] = value;
            return true;
        }
        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        ///  متد 
        /// try getmember
        /// در کلاس اصلی 
        /// ovverride
        /// شده است
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
        /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_values.ContainsKey(binder.Name))///اگر شیئ در خواستی در متغییر موجود باشد
            {
                result = _values[binder.Name];///آن را بر می گرداند
                return true;
            }
            result = null;
            return false;
        }
    }
}
