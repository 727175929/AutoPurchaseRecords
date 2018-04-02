using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AutoPurchaseRecords.DAL
{
    /// <summary>
    /// AES对称加密
    /// 
    /// 可能的异常：
    /// >>密钥长度不正确
    /// >>内容字符串太长
    /// </summary>
    public class AES
    {
        /// <summary>
        /// 密钥
        /// AES的加密、解密使用的密钥是同一个
        /// </summary>
        byte[] keyArray;
        public AES()
        {
            keyArray = UTF8Encoding.UTF8.GetBytes("12345678901234567890123456789012");
        }
        public AES(string key)
        {
            keyArray = UTF8Encoding.UTF8.GetBytes(key);
        }
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="toEncrypt">待加密字符串</param>
        /// <returns></returns>
        public string Encrypt(string toEncrypt)
        {
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="toDecrypt">待解密字符串</param>
        /// <returns></returns>
        public string Decrypt(string toDecrypt)
        {
            try
            {
                byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception)
            {
                return toDecrypt;
            }
        }
    }
}
