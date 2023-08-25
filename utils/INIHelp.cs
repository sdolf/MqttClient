using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MqttClient
{
    internal class INIHelp
    {
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string sesstion, string key, string def, byte[] retval, int size, string filePath);

        [DllImport("kernel32.dll")]
        private static extern int WritePrivateProfileString(string sesstion, byte[] key, byte[] val, string filePath);

        // init filename
        private static string filename = "config.ini";
        private static string filepath = Directory.GetCurrentDirectory() + "\\" + filename;

        public static bool SetValue(string type, string key, string value)
        {
            try
            {
                WritePrivateProfileString(type, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(value), filepath);
                return true;
            }
            catch
            {
                Logger.Warn($"set value failed: type={type}, key={key}, value={value}");
                return false;
            }
        }

        public static string GetString(string type, string key)
        {
            byte[] buffer = new byte[1024];
            int bufLen = GetPrivateProfileString(type, key, "", buffer, buffer.GetUpperBound(0), filepath);
            string s = Encoding.UTF8.GetString(buffer, 0, bufLen);
            return s;
        }

        public static double GetDouble(string type, string key)
        {
            byte[] buffer = new byte[1024];
            int bufLen = GetPrivateProfileString(type, key, "", buffer, buffer.GetUpperBound(0), filepath);
            string s = Encoding.UTF8.GetString(buffer, 0, bufLen);
            try
            {
                return Convert.ToDouble(s);
            }
            catch
            {
                Logger.Warn($"get double failed: type={type}, key={key}, string={s}, return 0 instead");
                return 0;
            }
        }

        public static int GetInt(string type, string key)
        {
            byte[] buffer = new byte[1024];
            int bufLen = GetPrivateProfileString(type, key, "", buffer, buffer.GetUpperBound(0), filepath);
            string s = Encoding.UTF8.GetString(buffer, 0, bufLen);
            try
            {
                return Convert.ToInt16(s);
            }
            catch
            {
                Logger.Warn($"get int failed: type={type}, key={key}, string={s}, return 0 instead");
                return 0;
            }
        }
    }
}
