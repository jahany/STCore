using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace STCore.Utiles
{
    public class Securiry : IDisposable
    {
        private const string cryptoKey = "cryptoKey";
        private static readonly byte[] IV =
            new byte[8] { 240, 3, 45, 29, 0, 76, 173, 59 };
        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string Encrypt(string s)
        {
            if (s == null || s.Length == 0) return string.Empty;

            string result = string.Empty;

            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(s);

                TripleDESCryptoServiceProvider des =
                    new TripleDESCryptoServiceProvider();

                MD5CryptoServiceProvider MD5 =
                    new MD5CryptoServiceProvider();

                des.Key =
                    MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(cryptoKey));

                des.IV = IV;
                result = Convert.ToBase64String(
                    des.CreateEncryptor().TransformFinalBlock(
                        buffer, 0, buffer.Length));
            }
            catch
            {
                throw;
            }

            return result;
        }
        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string Decrypt(string s)
        {
            s = s.Replace(" ", "+");
            if (s == null || s.Length == 0) return string.Empty;

            string result = string.Empty;

            try
            {
                byte[] buffer = Convert.FromBase64String(s);

                TripleDESCryptoServiceProvider des =
                    new TripleDESCryptoServiceProvider();

                MD5CryptoServiceProvider MD5 =
                    new MD5CryptoServiceProvider();

                des.Key =
                    MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(cryptoKey));

                des.IV = IV;

                result = Encoding.ASCII.GetString(
                    des.CreateDecryptor().TransformFinalBlock(
                    buffer, 0, buffer.Length));
            }
            catch
            {
                throw;
            }

            return result;
        }
        /// <summary>
        /// برای کد کردن از این تابع استفاه میشود
        /// </summary>
        /// <param name="s">ورودی از هر نوعی می تواند باشد</param>
        /// <returns>کد شده</returns>
        public string Encode(string s)//استرینگ ها کد می شوند
        {
            string resualt = Encrypt(s);
            return resualt;
        }
        /// <summary>
        /// برای دیکد کردن از این تابع استفاه میشود
        /// </summary>
        /// <param name="s">ورودی قفط از نوع کد شده وارد شود</param>
        /// <returns>دیکد شده را تحویل می دهد</returns>
        public string Decode(string s)
        {
            string resualt = Decrypt(s);
            return resualt;
        }
        /// <summary>
        /// عددی را به صورت رندم انتخاب میکند
        /// </summary>
        /// <param name="maxNumber">عددی انتخاب می شود که از صفر تا این عدد می باشد</param>
        /// <returns></returns>
        public static int GetRandomNumber(int maxNumber)
        {
            if (maxNumber < 1)
                throw new System.Exception("ماکس باید بیشتر از یک باشد");
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            int seed = (b[0] & 0x7f) << 24 | b[1] << 16 | b[2] << 8 | b[3];
            System.Random r = new System.Random(seed);
            return r.Next(1, maxNumber);
        }
        /// <summary>
        /// حروفی را به صورت رندم انتخاب می کند
        /// </summary>
        /// <param name="length">تعداد حروف در کلمه رندم</param>
        /// <returns></returns>
        public static string GetRandomString(int length)
        {
            string[] array = new string[54]
            {
                "0","2","3","4","5","6","8","9",
                "a","b","c","d","e","f","g","h","j","k","m","n","p","q","r","s","t","u","v","w","x","y","z",
                "A","B","C","D","E","F","G","H","J","K","L","M","N","P","R","S","T","U","V","W","X","Y","Z"
            };
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < length; i++) sb.Append(array[GetRandomNumber(53)]);
            return sb.ToString();
        }
        private string sec_intstring(string content)
        {
            List<string> list1 = new List<string>();
            string[] array = content.Select(f => f.ToString()).ToArray();
            string res = "";
            for (int i = 0; i < array.Length; i++)
            {
                res += int_string(array[i]);
            }
            return res;
        }
        private string sec_stringint(string content)
        {
            List<string> list1 = new List<string>();
            string[] array = content.Select(f => false.ToString()).ToArray();
            string res = "";
            for (int i = 0; i < array.Length; i++)
            {
                res += string_int(array[i]);
            }
            return res;
        }
        public static string Id_Encrypt(long value)
        {
            return Convert.ToBase64String(BitConverter.GetBytes(value)).Replace("=", "").Replace("+", "").Replace("/", "-");
        }
        public static string Id_Encrypt_Imagename(long value)
        {
            return Convert.ToBase64String(BitConverter.GetBytes(value)).Replace("=", "").Replace("+", "").Replace("/", "-") + ".png";
        }
        private string int_string(string content)
        {
            switch (content)
            {
                case "0":
                    return "a";
                case "1":
                    return "b";
                case "2":
                    return "c";
                case "3":
                    return "d";
                case "4":
                    return "e";
                case "5":
                    return "f";
                case "6":
                    return "g";
                case "7":
                    return "h";
                case "8":
                    return "i";
                case "9":
                    return "j";
                default:
                    break;
            }
            return content;
        }
        private string string_int(string content)
        {
            switch (content)
            {
                case "a":
                    return "0";
                case "b":
                    return "1";
                case "c":
                    return "2";
                case "d":
                    return "3";
                case "e":
                    return "4";
                case "f":
                    return "5";
                case "g":
                    return "6";
                case "h":
                    return "7";
                case "i":
                    return "8";
                case "j":
                    return "9";
                default:
                    break;
            }
            return content;
        }
        /// <summary>
        /// query string را کد می کند
        /// </summary>
        /// <param name="content">ای دی query string</param>
        /// <returns></returns>
        public string encode_Qstring(string content)
        {
            return int_string(content);
        }
        /// <summary>
        /// query string کد شده را دیکد می کند
        /// </summary>
        /// <param name="content">ای دی query string به صورت کد شده</param>
        /// <returns></returns>
        public string decode_Qstring(string content)
        {
            return string_int(content);
        }
        private string MakeExpiryHash(DateTime expiry)
        {
            const string salt = "some random bytes";
            byte[] bytes = Encoding.UTF8.GetBytes(salt + expiry.ToString("s"));
            using (var sha = System.Security.Cryptography.SHA1.Create())
                return string.Concat(sha.ComputeHash(bytes).Select(b => b.ToString("x2"))).Substring(8);
        }
        /// <summary>
        /// لینک دانلد به صورت query string می سازد
        /// </summary>
        /// <returns>Download?exp={0}&k={1}", expires.ToString("s"), hash</returns>
        public string make_download_link()
        {
            DateTime expires = DateTime.Now + TimeSpan.FromDays(1);
            string hash = MakeExpiryHash(expires);
            string link = string.Format("Download?exp={0}&k={1}", expires.ToString("s"), hash);
            return link;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Implements a multi-stage byte array. Uses less memory than a byte
    /// array large enough to hold an offset for each Unicode character.
    /// </summary>
    class UnicodeSkipArray
    {
        // Pattern length used for default byte value
        private byte _patternLength;
        // Default byte array (filled with default value)
        private byte[] _default;
        // Array to hold byte arrays
        private byte[][] _skipTable;
        // Size of each block
        private const int BlockSize = 0x100;

        /// <summary>
        /// Initializes this UnicodeSkipTable instance
        /// </summary>
        /// <param name="patternLength">Length of BM pattern</param>
        public UnicodeSkipArray(int patternLength)
        {
            // Default value (length of pattern being searched)
            _patternLength = (byte)patternLength;
            // Default table (filled with default value)
            _default = new byte[BlockSize];
            InitializeBlock(_default);
            // Master table (array of arrays)
            _skipTable = new byte[BlockSize][];
            for (int i = 0; i < BlockSize; i++)
                _skipTable[i] = _default;
        }

        /// <summary>
        /// Sets/gets a value in the multi-stage tables.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte this[int index]
        {
            get
            {
                // Return value
                return _skipTable[index / BlockSize][index % BlockSize];
            }
            set
            {
                // Get array that contains value to set
                int i = (index / BlockSize);
                // Does it reference the default table?
                if (_skipTable[i] == _default)
                {
                    // Yes, value goes in a new table
                    _skipTable[i] = new byte[BlockSize];
                    InitializeBlock(_skipTable[i]);
                }
                // Set value
                _skipTable[i][index % BlockSize] = value;
            }
        }

        /// <summary>
        /// Initializes a block to hold the current "nomatch value.
        /// </summary>
        /// <param name="block">Block to be initialized</param>
        private void InitializeBlock(byte[] block)
        {
            for (int i = 0; i < BlockSize; i++)
                block[i] = _patternLength;
        }
    }

    /// <summary>
    /// Implements Boyer-Moore search algorithm
    /// </summary>
    class BoyerMoore
    {
        private string _pattern;
        private bool _ignoreCase;
        private UnicodeSkipArray _skipArray;

        // Returned index when no match found
        public const int InvalidIndex = -1;

        public BoyerMoore(string pattern)
        {
            Initialize(pattern, false);
        }

        public BoyerMoore(string pattern, bool ignoreCase)
        {
            Initialize(pattern, ignoreCase);
        }

        /// <summary>
        /// Initializes this instance to search a new pattern.
        /// </summary>
        /// <param name="pattern">Pattern to search for</param>
        public void Initialize(string pattern)
        {
            Initialize(pattern, false);
        }

        /// <summary>
        /// Initializes this instance to search a new pattern.
        /// </summary>
        /// <param name="pattern">Pattern to search for</param>
        /// <param name="ignoreCase">If true, search is case-insensitive</param>
        public void Initialize(string pattern, bool ignoreCase)
        {
            _pattern = pattern;
            _ignoreCase = ignoreCase;

            // Create multi-stage skip table
            _skipArray = new UnicodeSkipArray(_pattern.Length);
            // Initialize skip table for this pattern
            if (_ignoreCase)
            {
                for (int i = 0; i < _pattern.Length - 1; i++)
                {
                    _skipArray[Char.ToLower(_pattern[i])] = (byte)(_pattern.Length - i - 1);
                    _skipArray[Char.ToUpper(_pattern[i])] = (byte)(_pattern.Length - i - 1);
                }
            }
            else
            {
                for (int i = 0; i < _pattern.Length - 1; i++)
                    _skipArray[_pattern[i]] = (byte)(_pattern.Length - i - 1);
            }
        }

        /// <summary>
        /// Searches for the current pattern within the given text
        /// starting at the beginning.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public int Search(string text)
        {
            return Search(text, 0);
        }

        /// <summary>
        /// Searches for the current pattern within the given text
        /// starting at the specified index.
        /// </summary>
        /// <param name="text">Text to search</param>
        /// <param name="startIndex">Offset to begin search</param>
        /// <returns></returns>
        public int Search(string text, int startIndex)
        {
            int i = startIndex;

            // Loop while there's still room for search term
            while (i <= (text.Length - _pattern.Length))
            {
                // Look if we have a match at this position
                int j = _pattern.Length - 1;
                if (_ignoreCase)
                {
                    while (j >= 0 && Char.ToUpper(_pattern[j]) == Char.ToUpper(text[i + j]))
                        j--;
                }
                else
                {
                    while (j >= 0 && _pattern[j] == text[i + j])
                        j--;
                }

                if (j < 0)
                {
                    // Match found
                    return i;
                }

                // Advance to next comparision
                i += Math.Max(_skipArray[text[i + j]] - _pattern.Length + 1 + j, 1);
            }
            // No match found
            return InvalidIndex;
        }
    }
}
