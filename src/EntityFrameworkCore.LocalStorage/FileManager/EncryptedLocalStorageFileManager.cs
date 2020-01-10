using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Blazored.LocalStorage;

namespace EntityFrameworkCore.LocalStorage.FileManager
{
    class EncryptedLocalStorageFileManager : IFileManager
    {
        private readonly object thisLock = new object();

        IEntityType type;
        private readonly string filetype;
        private readonly string key;
        private readonly string databasename;
        private readonly string _location;
        private readonly ISyncLocalStorageService localStorage;

        public EncryptedLocalStorageFileManager(IEntityType _type, string _filetype, string _key, string _databasename, ISyncLocalStorageService localStorage)
        {
            type = _type;
            filetype = _filetype;
            key = _key;
            databasename = string.IsNullOrEmpty(_databasename) ? "" : _databasename + ".";
            this.localStorage = localStorage;
        }

        public string GetFileName()
        {
            string name = GetValidFileName(type.GetTableName());

            return databasename + name + "." + filetype + ".encrypted";
        }

        private static string GetValidFileName(string input)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                input = input.Replace(c, '_');
            }

            return input;
        }

        public string LoadContent()
        {
            lock (thisLock)
            {
                string path = GetFileName();

                var data = localStorage.GetItem<string>(path);
                if (!string.IsNullOrEmpty(data))
                    return Decrypt(data);

                return "";
            }
        }

        public void SaveContent(string content)
        {
            lock (thisLock)
            {
                string path = GetFileName();
                localStorage.SetItem(path, Encrypt(content));
            }
        }

        public bool Clear()
        {
            lock (thisLock)
            {
                if (localStorage.ContainKey(GetFileName()))
                {
                    localStorage.RemoveItem(GetFileName());
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool FileExists()
        {
            lock (thisLock)
            {
                return localStorage.ContainKey(GetFileName());
            }
        }

        private string Decrypt(string str)
        {
            try
            {
                str = str.Replace(" ", "+");
                return UseAesDecryptor(str);
            }
            catch
            {
                return "";
            }
        }

        private string UseAesDecryptor(string str)
        {
            byte[] cipherBytes = Convert.FromBase64String(str);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Dispose();
                    }
                    str = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return str;
        }

        private string Encrypt(string str)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(str);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Dispose();
                    }
                    str = Convert.ToBase64String(ms.ToArray());
                }
            }
            return str;
        }
    }
}
