using System.IO.IsolatedStorage;
using System.IO;

namespace Dietphone.Tools
{
    public sealed class IsolatedFile
    {
        private string relativeFilePath;
        private static IsolatedStorageFile isolatedStorage = null;
        private static readonly object isolatedStorageLock = new object();

        public IsolatedFile(string relativeFilePath)
        {
            this.relativeFilePath = relativeFilePath;
        }

        public static IsolatedStorageFile IsolatedStorage
        {
            get
            {
                lock (isolatedStorageLock)
                {
                    if (isolatedStorage == null)
                    {
                        isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
                    }
                    return isolatedStorage;
                }
            }
        }

        public bool Exists
        {
            get
            {
                return IsolatedStorage.FileExists(relativeFilePath);
            }
        }

        public IsolatedStorageFileStream GetReadingStream()
        {
            return IsolatedStorage.OpenFile(relativeFilePath, FileMode.Open, FileAccess.Read);
        }

        public IsolatedStorageFileStream GetWritingStream()
        {
            return IsolatedStorage.OpenFile(relativeFilePath, FileMode.Create, FileAccess.Write);
        }
    }
}