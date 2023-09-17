using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace STCore.Utiles
{
    public class Ftp : IDisposable
    {
        public static string host = null;
        public static string user = null;
        public static string pass = null;
        public static string host_ftp = null;////like ftp:/test.com
        private FtpWebRequest ftpRequest = null;
        private FtpWebResponse ftpResponse = null;
        private Stream ftpStream = null;
        private int bufferSize = 2048;
        public Ftp()
        {

        }
        public Ftp(string hostIP, string userName, string password)
        {
            host = hostIP;
            user = userName;
            pass = password;
        }
        public void download(string remoteFile, string localFile)
        {
            try
            {
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host_ftp + "/" + remoteFile);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                FileStream localFileStream = new FileStream(localFile, FileMode.Create);
                byte[] byteBuffer = new byte[bufferSize];
                int bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                try
                {
                    while (bytesRead > 0)
                    {
                        localFileStream.Write(byteBuffer, 0, bytesRead);
                        bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                localFileStream.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return;
        }

        public void upload(string remoteFile, string localFile)
        {
            try
            {
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host_ftp + "/" + remoteFile);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                ftpStream = ftpRequest.GetRequestStream();
                FileStream localFileStream = new FileStream(localFile, FileMode.Create);
                byte[] byteBuffer = new byte[bufferSize];
                int bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                try
                {
                    while (bytesSent != 0)
                    {
                        ftpStream.Write(byteBuffer, 0, bytesSent);
                        bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                localFileStream.Close();
                ftpStream.Close();
                ftpRequest = null;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return;
        }
        public void upload_File(string remoteFile, Stream LocalFile, string withoutfilename)
        {
            try
            {

                withoutfilename = withoutfilename.Remove(withoutfilename.Length - 1);

                createDirectory(withoutfilename);

                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host_ftp + "/" + remoteFile);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                ftpStream = ftpRequest.GetRequestStream();
                byte[] byteBuffer = new byte[bufferSize];
                int bytesSent = LocalFile.Read(byteBuffer, 0, bufferSize);
                try
                {
                    while (bytesSent != 0)
                    {
                        ftpStream.Write(byteBuffer, 0, bytesSent);
                        bytesSent = LocalFile.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                LocalFile.Close();
                ftpStream.Close();
                ftpRequest = null;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return;
        }
        public void upload_Img_As_Png(string remoteFileName, Stream LocalFile, string withoutfilename)
        {
            try
            {
                BinaryReader b = new BinaryReader(LocalFile);
                byte[] binData = b.ReadBytes(Convert.ToInt32(LocalFile.Length));

                Image imageObject = new Bitmap(new MemoryStream(binData));

                MemoryStream mstream = new MemoryStream();
                imageObject.Save(mstream, ImageFormat.Png);
                mstream.Position = 0;
                remoteFileName = Path.GetFileNameWithoutExtension(remoteFileName) + ".png";

                withoutfilename = withoutfilename.Remove(withoutfilename.Length - 1);

                createDirectory(withoutfilename);

                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host_ftp + "/" + withoutfilename + "/" + remoteFileName);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                ftpStream = ftpRequest.GetRequestStream();
                byte[] byteBuffer = new byte[bufferSize];
                int bytesSent = mstream.Read(byteBuffer, 0, bufferSize);
                try
                {
                    while (bytesSent != 0)
                    {
                        ftpStream.Write(byteBuffer, 0, bytesSent);
                        bytesSent = mstream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                mstream.Close();
                ftpStream.Close();
                ftpRequest = null;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return;
        }
        public void delete(string deleteFile)
        {
            try
            {
                ftpRequest = (FtpWebRequest)WebRequest.Create(host_ftp + "/" + deleteFile);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = false;
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                ftpRequest.Timeout = -1;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (WebException e) { Console.WriteLine(e.ToString()); }
            return;
        }

        public void rename(string currentFileNameAndPath, string newFileName)
        {
            try
            {
                ftpRequest = (FtpWebRequest)WebRequest.Create(host_ftp + "/" + currentFileNameAndPath);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                ftpRequest.RenameTo = newFileName;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return;
        }

        public void createDirectory(string newDirectory)
        {
            try
            {
                ftpRequest = (FtpWebRequest)WebRequest.Create(host_ftp + "/" + newDirectory);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return;
        }

        public string getFileCreatedDateTime(string fileName)
        {
            try
            {
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host_ftp + "/" + fileName);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                StreamReader ftpReader = new StreamReader(ftpStream);
                string fileInfo = null;
                try { fileInfo = ftpReader.ReadToEnd(); }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                return fileInfo;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return "";
        }

        public string getFileSize(string fileName)
        {
            try
            {
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host_ftp + "/" + fileName);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                StreamReader ftpReader = new StreamReader(ftpStream);
                string fileInfo = null;
                try { while (ftpReader.Peek() != -1) { fileInfo = ftpReader.ReadToEnd(); } }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                return fileInfo;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return "";
        }

        public string[] directoryListFileNames(string directory)
        {
            try
            {
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host_ftp + "/" + directory);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                StreamReader ftpReader = new StreamReader(ftpStream);
                string directoryRaw = null;
                try { while (ftpReader.Peek() != -1) { directoryRaw += ftpReader.ReadLine() + "|"; } }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                try
                {
                    string[] directoryList = directoryRaw.Split("|".ToCharArray());
                    string[] temp = new string[directoryList.Length - 2];
                    for (int i = 2; i < directoryList.Length; i++)
                    {
                        temp[i - 2] = directoryList[i];
                    }
                    return temp;
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return new string[] { "" };
        }

        public string[] directoryListDetailed(string directory)
        {
            try
            {
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host_ftp + "/" + directory);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                StreamReader ftpReader = new StreamReader(ftpStream);
                string directoryRaw = null;
                try { while (ftpReader.Peek() != -1) { directoryRaw += ftpReader.ReadLine() + "|"; } }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                try { string[] directoryList = directoryRaw.Split("|".ToCharArray()); return directoryList; }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return new string[] { "" };
        }

        public void Dispose()
        {

        }
    }
    public class file_windows
    {
        /// <summary>
        /// Saves to path.
        /// فقط با فرمت PNG
        /// عکس ارسال شده را در مسیر مشخص ذخیره میکند
        /// </summary>
        /// <param name="path_f">آدرسی که فایل در ان می خواهد ذخیره شود</param>
        /// <param name="File1">فایل</param>
        /// <param name="new_filename">نام فایل</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool save_to_path(string path_f, IFormFile File1, string new_filename)
        {
            try
            {
                
                BinaryReader b = new BinaryReader(File1.OpenReadStream());
                byte[] binData = b.ReadBytes(Convert.ToInt32(File1.OpenReadStream().Length));///فایل را به صورت باینری می خواند

                Image imageObject = new Bitmap(new MemoryStream(binData));///عکس را تشکیل می دهد

                MemoryStream stream = new MemoryStream();
                imageObject.Save(stream, ImageFormat.Png);///فرمت فایل را به پی ان جی تغییر می دهد


                string directory = path_f;
                if (!Directory.Exists(directory))
                {
                    DirectoryInfo di = Directory.CreateDirectory(directory);
                }
                string temp = path_f + Path.GetFileNameWithoutExtension(new_filename) + ".PNG";
                if (System.IO.File.Exists(temp))
                    System.IO.File.Delete(temp);
                imageObject.Save(temp);///فایل ره در فولدر ذخیره میکند
                return true;
            }
            catch (Exception x)
            {
                return false;
            }
        }
        /// <summary>
        /// فقط با فرمت PNG
        /// عکس ارسال شده را در مسیر مشخص ذخیره میکند
        /// </summary>
        /// <param name="path_f">آدرسی که فایل در ان می خواهد ذخیره شود</param>
        /// <param name="File1">فایل</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool save_to_path(string path_f, IFormFile File1)
        {
            try
            {
                BinaryReader b = new BinaryReader(File1.OpenReadStream());
                byte[] binData = b.ReadBytes(Convert.ToInt32(File1.OpenReadStream().Length));///فایل را به صورت باینری می خواند

                Image imageObject = new Bitmap(new MemoryStream(binData));

                MemoryStream stream = new MemoryStream();
                imageObject.Save(stream, ImageFormat.Png);///تشکیل عکس با فرمت جدید


                string directory = path_f;
                if (!Directory.Exists(directory))
                {
                    DirectoryInfo di = Directory.CreateDirectory(directory);
                }
                string temp = path_f + Path.GetFileNameWithoutExtension(File1.FileName) + ".PNG";
                if (System.IO.File.Exists(temp))
                    System.IO.File.Delete(temp);
                imageObject.Save(temp);///عکس زا ذخیره میکند
                //var fileName = Path.GetFileName(File1.FileName);

                //File1.SaveAs(Path.Combine(directory, fileName));
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Creates the file.
        /// فایلی با نام ورودی و ادرس ورودی می سازد
        /// </summary>
        /// <param name="_Directory">The directory.</param>
        /// <param name="FileName">Name of the file.</param>
        public void Create_File(string _Directory, string FileName)
        {
            string fullpath = _Directory + "/" + FileName;
            if (!Directory.Exists(_Directory))
            {
                Directory.CreateDirectory(_Directory);
            }
            if (!File.Exists(fullpath))
            {
                File.Create(fullpath);
            }
        }
        /// <summary>
        /// برای حذف یک فایل از جایی مشخص استفاده می شود
        /// </summary>
        /// <param name="path">محلی که فایل قرار دارد</param>
        /// <returns>اگر درست برگردد یعنی فایل حذف شده است</returns>
        public bool delete_file(string path)
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private Random generator;
        private Random Generator
        {
            get
            {
                if (this.generator == null)
                {
                    this.generator = new Random();
                }
                return this.generator;
            }
        }
        /// <summary>
        /// از جایی مشخص یک فایل با فرمت مشخص را به صورت رندم انتخاب می کند
        /// </summary>
        /// <param name="path">جایی که فایل ها قرار دارند</param>
        /// <param name="formats">فرمت فایل ها به صورت ارایه مثل : { ".png", ".jpg" }</param>
        /// <returns>نام فایل را برمی گرداند</returns>
        public string getrandomfile(string path, string[] formats)
        {
            string file = null;
            if (!string.IsNullOrEmpty(path))
            {
                var extensions = formats;
                try
                {
                    var di = new DirectoryInfo(path);
                    var rgFiles = di.GetFiles("*.*")
                                    .Where(f => extensions.Contains(f.Extension.ToLower()));
                    int fileCount = rgFiles.Count();
                    if (fileCount > 0)
                    {
                        int x = this.Generator.Next(0, fileCount);
                        file = rgFiles.ElementAt(x).Name;

                    }
                }
                catch { }
            }
            return file;
        }
        /// <summary>
        /// متنی را در فایل می نویسد
        /// </summary>
        /// <param name="path_with_name">ادرس فایل به همراه نام ان</param>
        /// <param name="text">متن برای نوشتن در فایل</param>
        public async void write_text_to_file(string path_with_name, string text)
        {
            try
            {
                using (StreamWriter writer = File.AppendText(path_with_name))
                {
                    await writer.WriteLineAsync(text);
                    writer.Close();
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// از فایل متنی متن ها را می خواند
        /// </summary>
        /// <param name="path_with_filename">ارس فایل به همراه اسم فایل</param>
        /// <returns>متن های خوانده شده</returns>
        public string read_from_file(string path_with_filename)
        {
            string tozihat = (System.IO.File.ReadAllText(path_with_filename));
            return tozihat;
        }
        /// <summary>
        /// به فایل موجود خط به خط لیست را اضافه میکند
        /// </summary>
        /// <param name="path"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task write_line_tofile(string path, string[] list)
        {
            try
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(path, true);
                for (int i = 0; i < list.Length; i++)
                {
                    string temp = list[i];
                    await file.WriteLineAsync(temp);
                }
                file.Close();
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// همه خط ها را می خواند و به صورت لیست بر می گرداند
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<string> read_all_lines(string path)
        {
            string[] lines = File.ReadAllLines(path);
            return lines.ToList<string>();
        }
        /// <summary>
        /// اگر در فایلی که به صورت لاینی در ان ذخیره کردی ام کلمه ی مورد نظر موجود باشد جواب درست است
        /// </summary>
        /// <param name="path"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool IsMatch(string path, string text)
        {
            using (StreamReader sr = File.OpenText(path))
            {
                string[] lines = File.ReadAllLines(path);
                bool isMatch = false;
                for (int x = 0; x < lines.Length - 1; x++)
                {
                    if (text == lines[x])
                    {
                        sr.Close();
                        isMatch = true;
                    }
                }
                return isMatch;
            }
        }
        /// <summary>
        /// خط مشخصی را از فایل می خواند
        /// </summary>
        /// <param name="path"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public string read_spec_line(string path, int number)
        {
            StreamReader sr = File.OpenText(path);
            string[] lines = File.ReadAllLines(path);
            if (number > lines.Length)
            {
                sr.Close();
                return "";
            }
            else
            {
                sr.Close();
                return lines[number - 1];
            }
        }
        /// <summary>
        /// از خط مشخصی تا اخر می خواند و بر می گرداند
        /// </summary>
        /// <param name="path"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<string> read_speecline_toEnd(string path, int number)
        {
            StreamReader sr = File.OpenText(path);
            string[] lines = File.ReadAllLines(path);
            if (number > lines.Length)
            {
                sr.Close();
                return new List<string>();
            }
            else
            {
                List<string> l = new List<string>();
                for (int i = number - 1; i < lines.Length - 1; i++)
                {
                    l.Add(lines[i]);
                }
                sr.Close();
                return l;
            }
        }
    }
    /// <summary>
    /// Class File_api.
    /// این کلاس برای هندل کردن فایل ها در وب سرور مورد استفاده قرار می گیرد
    /// </summary>
    /// <seealso cref="Ntaban.Section.file_windows" />
    public class File_api : file_windows, IDisposable
    {
        /// <summary>
        /// The file name
        /// اسم فایل را به صورت استاتیک نگه داری میکنیم برای دسترسی سریع به ان
        /// </summary>
        public static string File_name;
        /// <summary>
        /// Uploads the file.
        /// فایل آپلود شده را ذخیره سازی میکند
        /// </summary>
        /// <param name="httpC">The HTTP c.</param>
        /// <param name="size">The size.
        /// سایز مجاز برای ذخیره</param>
        /// <param name="types">The types.
        /// فزمت مجاز برای ذخیره</param>
        /// <param name="path_withoutnamefile">The path withoutnamefile.</param>
        /// <param name="New_filename">The new filename.
        /// نام فایل جدید که می تواند خالی و یا پر باشد</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Upload_File(List<IFormFile> files, long size, string[] types, string path_withoutnamefile, string New_filename)
        {
            try
            {
                if (files.Count > 0)///اگر در درخواست فایلهایی موجود باشد
                {
                    foreach (IFormFile file in files)///در ازاء تمام فایل ها
                    {
                        string a = file.FileName;
                        string[] suport = types;
                        if (Total_Validate.validate_file(file, size, suport, "حجم فایل بیشتر از حد مجاز", "فرمت ارسالی درست نیست") == false)///اگر سایز یا فرمت فایل مجاز نباشد
                        {
                            return false;
                        }
                        var filePath = "";
                        if (New_filename == null && New_filename == string.Empty)
                        {
                            filePath = path_withoutnamefile + file.FileName;
                            save_to_path(path_withoutnamefile, file);///فایل را ذخیره میکند
                        }
                        else
                        {
                            filePath = path_withoutnamefile + New_filename;
                            save_to_path(path_withoutnamefile, file, New_filename);///فایل را ذخیره میکند
                        }

                        File_name = filePath.ToString();
                    }
                    return true;
                }
                return false;
            }
            catch { return false; }
        }
        public List<string> GetDirectoryFilesName(string path)
        {
            List<string> res = new List<string>();
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
                res.Add(Path.GetFileName(file));
            return res;
        }
        /// <summary>
        /// Downloads the file todirectory.
        /// یک فایل را در یک فولدر دانلد میکند
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="path">The path.</param>
        public static void download_file_todirectory(string host, string path)
        {
            try
            {
                var client = new WebClient();
                client.DownloadFile(host, path);
            }
            catch
            {

            }
        }

        public void Dispose()
        {
        }
    }
}
