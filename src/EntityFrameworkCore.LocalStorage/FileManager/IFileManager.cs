using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.LocalStorage.FileManager
{
    interface IFileManager
    {
        string GetFileName();

        string LoadContent();

        void SaveContent(string content);

        bool Clear();

        bool FileExists();
    }
}
