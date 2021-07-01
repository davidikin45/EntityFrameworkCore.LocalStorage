using Blazored.LocalStorage;
using EntityFrameworkCore.LocalStorage.FileManager;
using EntityFrameworkCore.LocalStorage.Serializer;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;

namespace EntityFrameworkCore.LocalStorage.StoreManager
{
    public class DefaultStoreManager<T> : IStoreManager
    {
        private readonly ISerializer _serializer;
        private readonly IFileManager _fileManager;

        public DefaultStoreManager(LocalStorageOptions options, IEntityType entityType, ISyncLocalStorageService localStorage)
        {
            if (options.Serializer == "xml")
            {
                _serializer = new XMLSerializer<T>(entityType, entityType.FindPrimaryKey().GetPrincipalKeyValueFactory<T>());
            }
            else if (options.Serializer == "bson")
            {
                _serializer = new BSONSerializer<T>(entityType, entityType.FindPrimaryKey().GetPrincipalKeyValueFactory<T>());
            }
            else if (options.Serializer == "csv")
            {
                _serializer = new CSVSerializer<T>(entityType, entityType.FindPrimaryKey().GetPrincipalKeyValueFactory<T>());
            }
            else
            {
                _serializer = new JSONSerializer<T>(entityType, entityType.FindPrimaryKey().GetPrincipalKeyValueFactory<T>());
            }

            string fmgr = options.FileManager ?? "default";
            string filetype = options.Serializer ?? "json";

            if (fmgr.Length >= 9 && fmgr.Substring(0, 9) == "encrypted")
            {
                string password = "";

                if (fmgr.Length > 9)
                {
                    password = fmgr.Substring(10);
                }

                _fileManager = new EncryptedLocalStorageFileManager(entityType, filetype, password, options.DatabaseName, localStorage);
            }
            else
            {
                _fileManager = new LocalStorageFileManager(entityType, filetype, options.DatabaseName, localStorage);
            }       
        }

        public Dictionary<TKey, object[]> Deserialize<TKey>(Dictionary<TKey, object[]> newList)
        {
            string content = _fileManager.LoadContent();
            return _serializer.Deserialize(content, newList);
        }

        public void Serialize<TKey>(Dictionary<TKey, object[]> list)
        {
            string cnt = _serializer.Serialize(list);
            _fileManager.SaveContent(cnt);
        }
    }
}
