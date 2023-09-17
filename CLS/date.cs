using Microsoft.SqlServer.Server;
using System;
using System.Data.SqlTypes;

[Serializable()]
[SqlUserDefinedType(Format.Native)]
public struct TabanDate : INullable, IComparable<TabanDate>
{
    private Int16 m_Year;
    private byte m_Month;
    private byte m_Day;
    private byte m_Hour;
    private byte m_Minute;
    private byte m_Second;
    private bool is_Null;


    public Int16 Year
    {
        get
        {
            return (this.m_Year);
        }
        set
        {
            m_Year = value;
        }
    }

    public byte Month
    {
        get
        {
            return (this.m_Month);
        }
        set
        {
            m_Month = value;
        }
    }

    public byte Day
    {
        get
        {
            return (this.m_Day);
        }
        set
        {
            m_Day = value;
        }
    }

    public byte Hour
    {
        get
        {
            return (this.m_Hour);
        }
        set
        {
            m_Hour = value;
        }
    }

    public byte Minute
    {
        get
        {
            return (this.m_Minute);
        }
        set
        {
            m_Minute = value;
        }
    }

    public byte Second
    {
        get
        {
            return (this.m_Second);
        }
        set
        {
            m_Second = value;
        }
    }

    public bool IsNull
    {
        get
        {
            return is_Null;
        }
    }

    public static TabanDate Null
    {
        get
        {
            TabanDate jl = new TabanDate();
            jl.is_Null = true;
            return (jl);
        }
    }


    public override string ToString()
    {
        if (this.IsNull)
        {
            return "NULL";
        }
        else
        {
            return this.m_Year.ToString("D4") + "/" + this.m_Month.ToString("D2") + "/" + this.m_Day.ToString("D2") + " " + this.Hour.ToString("D2") + ":" + this.Minute.ToString("D2") + ":" + this.Second.ToString("D2");
        }
    }


    public static TabanDate Parse(SqlString s)
    {
        if (s.IsNull || string.IsNullOrEmpty(s.ToString()))
        {
            return Null;
        }

        System.Globalization.PersianCalendar pers = new System.Globalization.PersianCalendar();
        string str = Convert.ToString(s);
        string[] JDate = str.Split(' ')[0].Split('/');

        TabanDate jl = new TabanDate();

        jl.Year = Convert.ToInt16(JDate[0]);
        byte MonthsInYear = (byte)pers.GetMonthsInYear(jl.Year);
        jl.Month = (byte.Parse(JDate[1]) <= MonthsInYear ? (byte.Parse(JDate[1]) > 0 ? byte.Parse(JDate[1]) : (byte)1) : MonthsInYear);
        byte DaysInMonth = (byte)pers.GetDaysInMonth(jl.Year, jl.Month); ;
        jl.Day = (byte.Parse(JDate[2]) <= DaysInMonth ? (byte.Parse(JDate[2]) > 0 ? byte.Parse(JDate[2]) : (byte)1) : DaysInMonth);
        if (str.Split(' ').Length > 1)
        {
            string[] JTime = str.Split(' ')[1].Split(':');
            jl.Hour = (JTime.Length >= 1 ? (byte.Parse(JTime[0]) < 23 && byte.Parse(JTime[0]) >= (byte)0 ? byte.Parse(JTime[0]) : (byte)0) : (byte)0);
            jl.Minute = (JTime.Length >= 2 ? (byte.Parse(JTime[1]) < 59 && byte.Parse(JTime[1]) >= (byte)0 ? byte.Parse(JTime[1]) : (byte)0) : (byte)0);
            jl.Second = (JTime.Length >= 3 ? (byte.Parse(JTime[2]) < 59 && byte.Parse(JTime[2]) >= (byte)0 ? byte.Parse(JTime[2]) : (byte)0) : (byte)0);
        }
        else { jl.Hour = 0; jl.Minute = 0; jl.Second = 0; }

        return (jl);
    }

    public SqlString GetDate()
    {
        return this.m_Year.ToString("D4") + "/" + this.m_Month.ToString("D2") + "/" + this.m_Day.ToString("D2");
    }

    public SqlString GetTime()
    {
        return this.Hour.ToString("D2") + ":" + this.Minute.ToString("D2") + ":" + this.Second.ToString("D2");
    }

    public SqlDateTime toDateTime()///hammon datetime 
    {
        var pers = new System.Globalization.PersianCalendar();
        DateTime dt = new DateTime(this.Year, this.Month, this.Day, this.Hour, this.Minute, this.Second, 0, pers);
        try
        {
            return SqlDateTime.Parse(dt.Year.ToString("0000/") + dt.Month.ToString("00/") + dt.Day.ToString("00") + " " + dt.Hour.ToString("00:") + dt.Minute.ToString("00:") + dt.Second.ToString("00"));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Can't parse " + dt.ToString() + "\n" + this.Year.ToString());
            throw new InvalidOperationException("Can't parse " + this.Year.ToString());
        }
    }

    public SqlString DateAdd(SqlString interval, int increment)
    {
        System.Globalization.PersianCalendar pers = new System.Globalization.PersianCalendar();
        DateTime dt = pers.ToDateTime(this.Year, this.Month, this.Day, this.Hour, this.Minute, this.Second, 0);
        string CInterval = interval.ToString();
        bool isConvert = true;
        switch (CInterval)
        {
            case "Year":
                dt = pers.AddYears(dt, increment);
                break;
            case "Month":
                dt = pers.AddMonths(dt, increment);
                break;
            case "Day":
                dt = pers.AddDays(dt, increment);
                break;
            case "Hour":
                dt = pers.AddHours(dt, increment);
                break;
            case "Minute":
                dt = pers.AddMinutes(dt, increment);
                break;
            case "Second":
                dt = pers.AddSeconds(dt, increment);
                break;
            default:
                isConvert = false;
                break;
        }

        if (isConvert == true)
        {
            this.Year = (Int16)pers.GetYear(dt);
            this.Month = (byte)pers.GetMonth(dt);
            this.Day = (byte)pers.GetDayOfMonth(dt);
            this.Hour = (byte)pers.GetHour(dt);
            this.Minute = (byte)pers.GetMinute(dt);
            this.Second = (byte)pers.GetSecond(dt);
        }


        return this.m_Year.ToString("D4") + "/" + this.m_Month.ToString("D2") + "/" + this.m_Day.ToString("D2") + " " + this.Hour.ToString("D2") + ":" + this.Minute.ToString("D2") + ":" + this.Second.ToString("D2");
    }
    public DateTime ToDateTime()
    {
        if (this != null)
        {
            System.Globalization.PersianCalendar pers = new System.Globalization.PersianCalendar();
            DateTime dt = pers.ToDateTime(this.Year, this.Month, this.Day, this.Hour, this.Minute, this.Second, 0);
            return dt;
        }
        else
        {
            return DateTime.Now;
        }
    }

    public int CompareTo(TabanDate obj)
    {
        if (obj == null)
            return 0;
        if (obj != null)
        {
            return this < obj ? -1 : 1;
        }
        throw new NotImplementedException();
    }

    public static bool operator ==(TabanDate a, TabanDate b)
    {
        if (a.GetDate() == b.GetDate())
            return true;
        else
            return false;
    }
    public static bool operator !=(TabanDate a, TabanDate b)
    {
        if (a.GetDate() != b.GetDate())
            return true;
        else
            return false;
    }
    public static bool operator <(TabanDate a, TabanDate b)
    {
        if (a.ToDateTime() < b.ToDateTime())
            return true;
        else
            return false;
    }
    public static bool operator >(TabanDate a, TabanDate b)
    {
        if (a.ToDateTime() > b.ToDateTime())
            return true;
        else
            return false;
    }
}