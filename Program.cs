using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dokan;
using System.IO;
using System.Collections;
using System.Threading;
using System.Diagnostics;

namespace TorrentFS
{
    class Program
    {
        class TFS : DokanOperations
        {
            static void LogMe()
            {
                var frames = new StackTrace(1).GetFrames();
                Console.WriteLine("Entering: " + frames[0].GetMethod().Name);
            }

            public int CreateFile(string filename, FileAccess access, FileShare share, FileMode mode, FileOptions options, DokanFileInfo info)
            {
                
                return 0;
            }

            public int OpenDirectory(string filename, DokanFileInfo info)
            {
                
                return 0;
            }

            public int CreateDirectory(string filename, DokanFileInfo info)
            {
                
                return -1;
            }

            public int Cleanup(string filename, DokanFileInfo info)
            {
                
                return 0;
            }

            public int CloseFile(string filename, DokanFileInfo info)
            {
                
                return 0;
            }

            public int ReadFile(string filename, byte[] buffer, ref uint readBytes, long offset, DokanFileInfo info)
            {
                
                return -1;
            }

            public int WriteFile(string filename, byte[] buffer, ref uint writtenBytes, long offset, DokanFileInfo info)
            {
                
                return -1;
            }

            public int FlushFileBuffers(string filename, DokanFileInfo info)
            {
                
                return -1;
            }

            public int GetFileInformation(string filename, FileInformation fileinfo, DokanFileInfo info)
            {
                
                return -1;
            }

            public int FindFiles(string filename, ArrayList files, DokanFileInfo info)
            {
                
                return 0;
            }

            public int SetFileAttributes(string filename, FileAttributes attr, DokanFileInfo info)
            {
                
                return -1;
            }

            public int SetFileTime(string filename, DateTime ctime, DateTime atime, DateTime mtime, DokanFileInfo info)
            {
                
                return -1;
            }

            public int DeleteFile(string filename, DokanFileInfo info)
            {
                
                return -1;
            }

            public int DeleteDirectory(string filename, DokanFileInfo info)
            {
                
                return -1;
            }

            public int MoveFile(string filename, string newname, bool replace, DokanFileInfo info)
            {
                
                return -1;
            }

            public int SetEndOfFile(string filename, long length, DokanFileInfo info)
            {
                
                return -1;
            }

            public int SetAllocationSize(string filename, long length, DokanFileInfo info)
            {
                
                return -1;
            }

            public int LockFile(string filename, long offset, long length, DokanFileInfo info)
            {
                
                return 0;
            }

            public int UnlockFile(string filename, long offset, long length, DokanFileInfo info)
            {
                
                return 0;
            }

            public int GetDiskFreeSpace(ref ulong freeBytesAvailable, ref ulong totalBytes, ref ulong totalFreeBytes, DokanFileInfo info)
            {
                LogMe();

                freeBytesAvailable = 0;
                totalBytes = 1024;
                totalFreeBytes = 0;
                
                return 0;
            }

            public int Unmount(DokanFileInfo info)
            {
                
                return 0;
            }
        }

        static void PrintStatus(int status)
        {
            switch (status)
            {
                case DokanNet.DOKAN_DRIVE_LETTER_ERROR:
                    Console.WriteLine("Drvie letter error");
                    break;
                case DokanNet.DOKAN_DRIVER_INSTALL_ERROR:
                    Console.WriteLine("Driver install error");
                    break;
                case DokanNet.DOKAN_MOUNT_ERROR:
                    Console.WriteLine("Mount error");
                    break;
                case DokanNet.DOKAN_START_ERROR:
                    Console.WriteLine("Start error");
                    break;
                case DokanNet.DOKAN_ERROR:
                    Console.WriteLine("Unknown error");
                    break;
                case DokanNet.DOKAN_SUCCESS:
                    Console.WriteLine("Success");
                    break;
                default:
                    Console.WriteLine("Unknown status: {0}", status);
                    break;
            }
        }

        public class DokanException : Exception
        {
            public int DokanErrorCode { get; private set; }

            public DokanException(string message, int errorcode)
                : base(message)
            {
                this.DokanErrorCode = errorcode;
            }
        }

        static void CheckStatus(int status)
        {
            switch (status)
            {
                case DokanNet.DOKAN_DRIVE_LETTER_ERROR:
                    throw new DokanException("Drvie letter error", status);
                case DokanNet.DOKAN_DRIVER_INSTALL_ERROR:
                    throw new DokanException("Driver install error", status);
                case DokanNet.DOKAN_MOUNT_ERROR:
                    throw new DokanException("DMount error", status);
                case DokanNet.DOKAN_START_ERROR:
                    throw new DokanException("Start error", status);
                case DokanNet.DOKAN_ERROR:
                    throw new DokanException("Unknown errorr", status);
                case DokanNet.DOKAN_SUCCESS:
                    Console.WriteLine("Success");
                    break;
                default:
                    Console.WriteLine("Unknown status: {0}", status);
                    break;
            }
        }

        public class ThreadableDisposableDokan : IDisposable
        {
            public DokanOperations Operations { get; private set; }
            public DokanOptions Options { get; private set; }

            private int StartStatus = DokanNet.DOKAN_SUCCESS;

            private bool disposed = false;

            public ThreadableDisposableDokan(DokanOptions options, DokanOperations operations)
            {
                this.Operations = operations;
                this.Options = options;

                var thread = new Thread(() =>
                {
                    CheckStatus(this.StartStatus = DokanNet.DokanMain(this.Options, this.Operations));
                });

                thread.Start();

                Thread.Sleep(1000);
            }

            public void Dispose()
            {
                if (!disposed)
                {
                    disposed = true;

                    if (this.StartStatus == DokanNet.DOKAN_SUCCESS)
                    {
                        DokanNet.DokanRemoveMountPoint(this.Options.MountPoint);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            using (new ThreadableDisposableDokan(new DokanOptions()
            {
                MountPoint = "t:\\",
                DebugMode = true,
                UseStdErr = false,
                VolumeLabel = "TorrentFS"
            }, new TFS())) { }
        }
    }
}
