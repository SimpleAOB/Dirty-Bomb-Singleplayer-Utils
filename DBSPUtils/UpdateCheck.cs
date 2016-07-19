using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;

namespace UpdateCheckCS
{
    class UpdateCheck : IDisposable
    {
        string errorString = "0";
        string successString = "0";
        string startURL = "0";
        bool latestBool = true;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v">Version. Write the version number that is latest in software server</param>
        /// <param name="ad">App Id. This number will never change</param>
        /// <param name="bu">Base URL to be used when launching browser to update. If the full URL is set inside the software server, write 'na'</param>
        /// <returns>True if no error is encountered. False if error is encountered. The error() method will return the error.</returns>
        public bool tryCheck (string v, int ad, string bu) {
            string currentVersion = v;
            string softwareID = ad.ToString();
            //Build string that will be used in search query
            string versionStringBase = "https://simpleaob.com/software/dbspu";
            // Make request to software server
            try
            {
                using (var client = new WebClient())
                {
                    string webver = client.DownloadString(versionStringBase);
                    if (currentVersion != webver)
                    {
                        successString = "New version found";
                        latestBool = false;
                        return true;
                    }
                    else
                    {
                        successString = "Latest version";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                errorString = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// Returns the error string if one is encountered
        /// </summary>
        /// <returns></returns>
        public string error()
        {
            return errorString;
        }
        /// <summary>
        /// Returns the success string if no error is encountered
        /// </summary>
        /// <returns></returns>
        public string success()
        {
            return successString;
        }
        /// <summary>
        /// Returns the URL to launch the browser with, or whatever else you want to do with it
        /// </summary>
        /// <returns></returns>
        public string URL()
        {
            return startURL;
        }
        /// <summary>
        /// Returns whether the current version is the latest or not
        /// </summary>
        /// <returns></returns>
        public bool latest()
        {
            return latestBool;
        }
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
    }
}
