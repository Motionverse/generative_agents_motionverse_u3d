using System;
using System.Security.Cryptography;
using System.Text;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
namespace MotionverseSDK
{
    public static class Utilities
    {
        public static long GetTimeStampSecond()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }

        public static T StrToObj<T>(string str) where T : class
        {
            var serializer = new fsSerializer();
            var data = fsJsonParser.Parse(str);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(T), ref deserialized).AssertSuccessWithoutWarnings();
            return deserialized as T;
        }
        public static string ObjToStr<T>(T value)
        {
            fsData serializedData;
            var serializer = new fsSerializer();
            serializer.TrySerialize(value, out serializedData).AssertSuccessWithoutWarnings();
            var json = fsJsonPrinter.PrettyJson(serializedData);
            return json;
        }
        public static string EncryptWithMD5(string source)
        {
            byte[] sor = Encoding.UTF8.GetBytes(source);
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

            }
            return strbul.ToString();
        }
        public static Transform FindChildRecursively(Transform parent, string name)
        {
            Transform t = parent.Find(name);
            if (t == null)
            {
                foreach (Transform tran in parent)
                {
                    t = FindChildRecursively(tran, name);
                    if (t != null)
                    {
                        return t;
                    }
                }
            }

            return t;
        }

        /// <summary>
        /// SHA1 加密，返回大写字符串
        /// </summary>
        /// <param name="content">需要加密字符串</param>
        /// <returns>返回40位UTF8 大写</returns>
        public static string SHA1(string content)
        {
            return SHA1(content, Encoding.UTF8);
        }
        /// <summary>
        /// SHA1 加密，返回大写字符串
        /// </summary>
        /// <param name="content">需要加密字符串</param>
        /// <param name="encode">指定加密编码</param>
        /// <returns>返回40位小写字符串</returns>
        public static string SHA1(string content, Encoding encode)
        {
            try
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] bytes_in = encode.GetBytes(content);
                byte[] bytes_out = sha1.ComputeHash(bytes_in);
                sha1.Dispose();
                string result = BitConverter.ToString(bytes_out);
                result = result.Replace("-", "");
                return result.ToLower();
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密出错：" + ex.Message);
            }
        }

    }
}

