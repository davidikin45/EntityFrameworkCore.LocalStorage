using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Blazored.LocalStorage;

namespace EntityFrameworkCore.LocalStorage
{
    class LocalStorageFileManager
    {
        private readonly object thisLock = new object();

        IEntityType type;
        private readonly string filetype;
        private readonly string databasename;
        private readonly string _location;
        private readonly ISyncLocalStorageService localStorage;

        public LocalStorageFileManager(IEntityType _type, string _filetype, string _databasename, ISyncLocalStorageService localStorage)
        {
            type = _type;
            filetype = _filetype;
            databasename = string.IsNullOrEmpty(_databasename) ? "" : _databasename + ".";
            this.localStorage = localStorage;
        }

        public string GetFileName()
        {
            string name = GetValidFileName(type.GetTableName());

            return databasename + name + "." + filetype;
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

                return localStorage.GetItem<string>(path) ?? "";
            }
        }

        public void SaveContent(string content)
        {
            lock (thisLock)
            {
                string path = GetFileName();
                localStorage.SetItem(path, content);
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
    }
}
