using System;
using System.Collections.Generic;
//using System.Web.Mvc;
using System.Globalization;


namespace STCore.Section
{
    public class tarikh
    {
        /// <summary>
        /// تاریخ امروز را بر می گرداند
        /// </summary>
        /// <returns></returns>
        public static string date_now()
        {
            DateTime dt = DateTime.Now;
            PersianCalendar pc = new PersianCalendar();
            string today = pc.GetYear(dt).ToString("0000/") + pc.GetMonth(dt).ToString("00/") + pc.GetDayOfMonth(dt).ToString("00");
            return today;
        }
        public static string date_now_hours()
        {
            DateTime dt = DateTime.Now;
            PersianCalendar pc = new PersianCalendar();
            string today = pc.GetYear(dt).ToString("0000/") + pc.GetMonth(dt).ToString("00/") + pc.GetDayOfMonth(dt).ToString("00") + " " + dt.ToLocalTime().ToString("hh:mm:ss");
            return today;
        }
        public static string JustTime()
        {
            DateTime dt = DateTime.Now;
            string t = dt.ToLocalTime().ToString("hh:mm:ss");
            return t;
        }
        /// <summary>
        /// برای جمع کردن چندین روز به تاریخی معین استفاده میشود
        /// </summary>
        /// <param name="history">تاریخی که می خواهد به ان اضافه شود</param>
        /// <param name="mmodat">تعداد روز هایی که می خواهد اضافه شود</param>
        /// <returns>تاریخ به روز شده را بر می گرداند</returns>
        public static string sum_modat_day_tarikh(string history, string mmodat)
        {
            int modat = Int32.Parse(mmodat);
            int myear = 0;
            int mmonth = modat / 30;
            if (mmonth > 12)
            { myear = 1; }
            int mday = modat % 30;
            string[] t = history.Split('/');
            string expert = (int.Parse(t[0]) + myear) + "/" + (int.Parse(t[1]) + mmonth) + "/" + (int.Parse(t[2]) + mday);
            return expert;
        }
        /// <summary>
        /// برای کم کردن چندین روز به تاریخی معین استفاده میشود
        /// </summary>
        /// <param name="history">تاریخی که می خواهد از ان کم شود</param>
        /// <param name="mmodat">تعداد روز هایی که می خواهد کم شود</param>
        /// <returns>تاریخ به روز شده را بر می گرداند</returns>
        public static string substraction_modat_day_tarikh(string history, string mmodat)
        {
            int modat = Int32.Parse(mmodat);
            int myear = 0;
            int mmonth = modat / 30;
            if (mmonth > 12)
            { myear = 1; }
            int mday = modat % 30;
            string[] t = history.Split('/');
            string m, d;
            m = (int.Parse(t[1]) - mmonth) < 10 ? "0" + (int.Parse(t[1]) - mmonth).ToString() : (int.Parse(t[1]) - mmonth).ToString();
            d = (int.Parse(t[2]) - mday) < 10 ? "0" + (int.Parse(t[2]) - mday).ToString() : (int.Parse(t[2]) - mday).ToString();
            string expert = (int.Parse(t[0]) - myear) + "/" + m + "/" + d;
            return expert;
        }
        /// <summary>
        /// برای مقایسه ی دو تاریخ استفاده می شود
        /// </summary>
        /// <param name="d1_forbigger">تاریخ اول</param>
        /// <param name="d2">تاریخ دوم</param>
        /// <returns>اکر درست برگردد یعنی تاریخ اول بزرگتر بوده است</returns>
        public static bool isbig_between_tow_history(string d1_forbigger, string d2)
        {
            string[] expert = d1_forbigger.Split('/');
            string[] now = d2.Split('/');
            if (int.Parse(now[0]) < int.Parse(expert[0]))
            {
                return true;
            }
            else if (int.Parse(now[2]) < int.Parse(expert[1]))
            {
                return true;
            }
            else if (int.Parse(now[2]) < int.Parse(expert[2]))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// زمان دقیق را بر می گرداند
        /// </summary>
        /// <returns></returns>
        public static string time()
        {
            DateTime dt = DateTime.Now;
            string a = "";
            PersianCalendar pc = new PersianCalendar();
            if (dt.Hour >= 0 && dt.Hour <= 12)
            {
                a = "قبل از ظهر";
            }
            else
            {
                a = "بعد از ظهر";
            }
            string time = dt.Hour.ToString("00:") + dt.Minute.ToString("00  ") + a;
            return time;
        }

        public string mah { get; set; }
        public string sal { get; set; }
        public string adadmah { get; set; }
        /// <summary>
        /// لیست ارشیو ماها نه را بر می گرداند
        /// </summary>
        /// <param name="sss">تعداد ماه های قبل برای نمایش از امروز</param>
        /// <returns></returns>
        public List<tarikh> get_back_arshiv_mahane(int sss)
        {
            List<tarikh> list1 = new List<tarikh>();
            string tarikh = date_now();
            string[] s = date_now().Split('/');
            for (int i = 0; i < sss; i++)
            {
                int last = Int16.Parse(s[1].ToString()) - i;
                int al = Int16.Parse(s[1].ToString()) - i;
                if (last <= 0)
                {
                    last = 12 - (i - Int16.Parse(s[1].ToString()));
                }
                if (al == 0)
                {
                    s[0] = (Int32.Parse(s[0]) - 1).ToString();
                }
                tarikh a = new tarikh();
                a.mah = get_back_month(last.ToString());
                a.sal = s[0];
                a.adadmah = last.ToString();
                list1.Add(a);
            }
            return list1;
        }
        /// <summary>
        /// برای بررسی انکه تاریخی مابین دو تریخ دیگر است یا نه
        /// </summary>
        /// <param name="hmin">تاریخ کمتر</param>
        /// <param name="hmid">تاریخ مابین</param>
        /// <param name="hmax">تاریخ بیشتر</param>
        /// <returns>اگر درست برگردد تاریخ مابین دو تاریخ دیگر است</returns>
        public bool testday(string hmin, string hmid, string hmax)
        {
            int rozMin, mahMin, salMin, rozKey, mahKey, salKey, rozMax, mahMax, salMax = 0;
            string[] smin = hmin.Split('/');
            string[] smid = hmid.Split('/');
            string[] smax = hmax.Split('/');
            rozMin = Int32.Parse(smin[2]);
            mahMin = Int32.Parse(smin[1]);
            salMin = Int32.Parse(smin[0]);
            rozKey = Int32.Parse(smid[2]);
            mahKey = Int32.Parse(smid[1]);
            salKey = Int32.Parse(smid[0]);
            rozMax = Int32.Parse(smax[2]);
            mahMax = Int32.Parse(smax[1]);
            salMax = Int32.Parse(smax[0]);
            if (salKey <= salMax || salKey >= salMin)
            {
                if (salMax == salMin)
                {
                    if (mahKey <= mahMax || mahKey >= mahMin)
                    {
                        return testMonth(rozMin, mahMin, rozKey, mahKey, rozMax, mahMax);
                    }
                }
                else
                {
                    if (salKey == salMax && mahKey <= mahMax)
                    {
                        return testMonth(rozMin, mahMin, rozKey, mahKey, rozMax, mahMax);
                    }
                    else if (salKey == salMin && mahKey >= mahMin)
                    {
                        return testMonth(rozMin, mahMin, rozKey, mahKey, rozMax, mahMax);
                    }
                    else if (salKey != salMax && mahKey != mahMax)
                    {
                        return testMonth(rozMin, mahMin, rozKey, mahKey, rozMax, mahMax);
                    }
                }
            }
            return false;
        }
        private bool testMonth(int rozMin, int mahMin, int rozKey, int mahKey, int rozMax, int mahMax)
        {
            if (mahMax == mahMin)
            {
                if (rozKey <= rozMax || rozKey >= rozMin)
                {
                    return true;
                }
            }
            else
            {
                if (mahKey == mahMax && rozKey <= rozMax)
                {
                    return true;
                }
                else if (mahKey == mahMin && rozKey >= rozMin)
                {
                    return true;
                }
                else if (mahKey != mahMax && mahKey != mahMax)
                {
                    return true;
                }
            }
            return false;
        }
        public string get_back_month(string m)
        {
            string mon = m.ToString();
            string mahh = month(mon);
            return mahh;
        }
        /// <summary>
        /// عدد را می گیرد و ماه را بر می گرداند
        /// </summary>
        /// <param name="m">عدد ماه</param>
        /// <returns></returns>
        public string month(string m)
        {
            switch (m)
            {
                case "01":
                    return "فروردین";
                case "02":
                    return "اردیبهشت";
                case "03":
                    return "خرداد";
                case "04":
                    return "تیر";
                case "05":
                    return "مرداد";
                case "06":
                    return "شهریور";
                case "07":
                    return "مهر";
                case "08":
                    return "ابان";
                case "09":
                    return "اذر";
                case "10":
                    return "دی";
                case "11":
                    return "بهمن";
                case "12":
                    return "اسفند";
                case "1":
                    return "فروردین";
                case "2":
                    return "اردیبهشت";
                case "3":
                    return "خرداد";
                case "4":
                    return "تیر";
                case "5":
                    return "مرداد";
                case "6":
                    return "شهریور";
                case "7":
                    return "مهر";
                case "8":
                    return "ابان";
                case "9":
                    return "اذر";
                default:
                    return "فروردین";
            }
        }
        public string day(int m)
        {
            switch (m)
            {
                
                case 1:
                    return "شنبه";
                case 2:
                    return "یکشنبه";
                case 3:
                    return "دو شنبه";
                case 4:
                    return "سه شنبه";
                case 5:
                    return "چهارشنبه";
                case 6:
                    return "پنج شنبه";
                case 7:
                    return "جمعه";
                default:
                    return "شنبه";
            }
        }
        /// <summary>
        /// تاریخ میلادی را به شمسی تبدیل می کند
        /// </summary>
        /// <param name="date">تاریخ میلادی</param>
        /// <returns></returns>
        public string Get_Date(DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            string today = pc.GetYear(date).ToString("0000/") + pc.GetMonth(date).ToString("00/") + pc.GetDayOfMonth(date).ToString("00");
            return today;
        }
        public string GetDatead(DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            string today = pc.GetYear(date).ToString("0000") + month(pc.GetMonth(date).ToString("00")) + " , " + pc.GetDayOfMonth(date).ToString("00") + " , " + day((int)pc.GetDayOfWeek(date));
            return today;
        }
    }
}
