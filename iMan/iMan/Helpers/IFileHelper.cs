using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Plugin.FilePicker.Abstractions;

namespace iMan.Helpers
{
    public interface IFileHelper
    {
        string GetFile(string fileName);
        bool DeleteFile(string fileName);
        bool RenameFile(string oldFilename, string newFileName);
        Task<bool> SaveZipToFolder();
        Task<bool> UnzipDb(Stream zipFile);
        //Task AskPermission();
    }
}
