using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace STCore.Utiles
{
    public class Total_Validate
    {
        public static string[] SupportImageExtention = new string[] { "jpeg", "jpg", "png", "bpm", "JPEG", "JPG", "PNG", "BMP" };
        public static string resault = "";
        /// <summary>
        /// برای بررسی ایمل استفاده می شود
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool Email(string email)
        {
            string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(emailRegex);
            if (email == null || email == "")
            {
                resault = "لطفا ایمیل خود را وارد کنید";
                return false;
            }
            if (!re.IsMatch(email))
            {
                resault = "لطفا ورودی ایمیل را بررسی کنید";
                return false;
            }
            return true;
        }
        /// <summary>
        /// فقط فارسی انگلیسی و عدد قبول می کند
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="error_ifnull"></param>
        /// <param name="error_iffalse"></param>
        /// <returns></returns>
        public static bool farsi_english_number(string str1, string error_ifnull, string error_iffalse)
        {
            if (str1 == null || str1 == "")
            {
                resault = error_ifnull;
                return false;
            }
            if (!Regex.IsMatch(str1, @"^[\u0600-\u06FF\uFB8A\u067E\u0686\u06AFa-zA-Z0-9]+$"))
            {
                resault = error_iffalse;
                return true;
            }
            return true;
        }
        /// <summary>
        /// فقط فارسی قبول می کند
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="error_ifnull"></param>
        /// <param name="error_iffalse"></param>
        /// <returns></returns>
        public static bool farsi(string str1, string error_ifnull, string error_iffalse)
        {
            if (str1 == null || str1 == "")
            {
                resault = error_ifnull;
                return false;
            }
            if (!Regex.IsMatch(str1, @"^[\u0600-\u06FF\uFB8A\u067E\u0686\u06AF\s]+$"))
            {
                resault = error_iffalse;
                return false;
            }
            return true;
        }
        /// <summary>
        /// فقط عدد قبول می کند
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="error_ifnull"></param>
        /// <param name="error_iffalse"></param>
        /// <returns></returns>
        public static bool number(string str1, string error_ifnull, string error_iffalse)
        {
            if (str1 == null || str1 == "")
            {
                resault = error_ifnull;
                return false;
            }
            if (!Regex.IsMatch(str1, @"^[0-9]+$"))
            {
                resault = error_iffalse;
                return false;
            }
            return true;
        }
        /// <summary>
        /// فقط انگلیسی قبول می کند
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="error_ifnull"></param>
        /// <param name="error_iffalse"></param>
        /// <returns></returns>
        public static bool english(string str1, string error_ifnull, string error_iffalse)
        {
            if (str1 == null || str1 == "")
            {
                resault = error_ifnull;
                return false;
            }
            if (!Regex.IsMatch(str1, @"^[a-zA-Z]+$"))
            {
                resault = error_iffalse;
                return false;
            }
            return true;
        }
        /// <summary>
        /// خالی بودن را بررسی می کند
        /// </summary>
        /// <param name="str"></param>
        /// <param name="error_msg"></param>
        /// <returns></returns>
        public static bool Null(string str, string error_msg)
        {
            if (str == null || str == "")
            {
                resault = error_msg;
                return false;
            }
            return true;
        }
        /// <summary>
        /// فایل را بررسی می کند
        /// </summary>
        /// <param name="File1"></param>
        /// <param name="size"></param>
        /// <param name="types"></param>
        /// <param name="error_size"></param>
        /// <param name="error_type"></param>
        /// <returns></returns>
        public static bool validate_file(IFormFile File1, long size, string[] types, string error_size, string error_type)
        {

            if (File1.Length > size)
            {
                resault = error_size;
                return false;
            }

            var supportedTypes = types;

            var fileExt = System.IO.Path.GetExtension(File1.FileName).Substring(1);

            if (!supportedTypes.Contains(fileExt))
            {
                resault = error_type;
                return false;
            }
            return true;
        }
        /// <summary>
        /// در بررسی صفت ها اگز مشکلی پیش امده باشد از را بر می گرداند با نام resault
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool dovalidate(object t)
        {
            ValidationContext context = new ValidationContext(t, null, null);
            IList<ValidationResult> errors = new List<ValidationResult>();

            if (!Validator.TryValidateObject(t, context, errors, true))
            {
                resault = "";
                foreach (ValidationResult goal in errors)
                {
                    resault += goal.ErrorMessage.ToString() + "\n";
                }
                return false;
            }
            else
                return true;
            return true;
        }
        public static bool is_network_available()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        }
        public static bool Chek_Net_Connection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }

    public class Attrib_Validate
    {
        public class Email : ValidationAttribute
        {
            public Email()
                : base("{0} شامل کاراکتز های غلطی است")
            {

            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value != null)
                {
                    string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                    Regex re = new Regex(emailRegex);
                    if (!re.IsMatch(value.ToString()))
                    {
                        var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                        return new ValidationResult(errorMessage);
                    }
                }
                return ValidationResult.Success;
            }
        }
        public class Null : ValidationAttribute
        {
            public Null()
                : base("{0}شامل کاراکتز های غلطی است.")
            {

            }
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value == null || value == "")
                {
                    var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(errorMessage);
                }
                return ValidationResult.Success;
            }
        }
        public class farsi : ValidationAttribute
        {
            public farsi()
                : base("{0} شامل کاراکتز های غلطی است")
            {

            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value != null)
                {
                    if (!Regex.IsMatch(value.ToString(), @"^[\u0600-\u06FF\uFB8A\u067E\u0686\u06AF\s]+$"))
                    {
                        var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                        return new ValidationResult(errorMessage);
                    }
                }
                return ValidationResult.Success;
            }
        }
        public class farsi_english_number : ValidationAttribute
        {
            public farsi_english_number()
                : base("{0} شامل کاراکتز های غلطی است")
            {

            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value != null)
                {
                    if (!Regex.IsMatch(value.ToString(), @"^[\u0600-\u06FF\uFB8A\u067E\u0686\u06AFa-zA-Z0-9]+$"))
                    {
                        var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                        return new ValidationResult(errorMessage);
                    }
                }
                return ValidationResult.Success;
            }
        }
        public class number : ValidationAttribute
        {
            public number()
                : base("{0} شامل کاراکتز های غلطی است")
            {

            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value != null)
                {
                    if (!Regex.IsMatch(value.ToString(), @"^[0-9]+$"))
                    {
                        var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                        return new ValidationResult(errorMessage);
                    }
                }
                return ValidationResult.Success;
            }
        }
        public class english : ValidationAttribute
        {
            public english()
                : base("{0} شامل کاراکتز های غلطی است")
            {

            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value != null)
                {
                    if (!Regex.IsMatch(value.ToString(), @"^[a-zA-Z]+$"))
                    {
                        var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                        return new ValidationResult(errorMessage);
                    }
                }
                return ValidationResult.Success;
            }
        }
    }

    public static class DynamicExtensions
    {
        public static bool isBool(dynamic value, bool defaultValue = false)
        {
            bool.TryParse((string)value, out defaultValue);
            return defaultValue;
        }

        public static long isLong(dynamic value, long defaultValue = -1)
        {
            long.TryParse(value as string, out defaultValue);
            return defaultValue;
        }
    }
}
