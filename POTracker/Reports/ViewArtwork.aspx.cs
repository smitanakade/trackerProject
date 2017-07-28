using POTracker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace POTracker.Reports
{
    public partial class ViewArtwork : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var tempPath = Server.MapPath("~/Temp"); ;
            var parentPath = @"\\192.1.1.15\a r c h i v e\ZFTP_Final Art";

            var sortBy = Request.QueryString["sortby"];

            var product = Request.QueryString["p"];

            if (!string.IsNullOrEmpty(product))
            {
                lblProductCode.Text = product;
                lblArtworkheader.Visible = true;
                var artworkFolderName = string.Format(@"\{0} Final Art", product);

                var serverPath = string.Format("{0}{1}", parentPath, artworkFolderName);

                var tempDirectories = Directory.GetDirectories(tempPath);
                foreach (var tempdir in tempDirectories)
                {
                    try
                    {
                        Directory.Delete(tempdir,true);
                    }
                    catch(Exception ex)
                    {

                    }
                }
                var currentTempPath = string.Format(@"{0}\{1}", tempPath, UtcNowTicks);
                var tempDinfo = Directory.CreateDirectory(currentTempPath);
                //
                // Databind to the directory listing
                //
                var listing = new List<FileSystemInfo>();
                try
                {

                    var serverFiles = Directory.GetFiles(serverPath);
                    foreach (var serverFile in serverFiles)
                    {
                        File.Copy(serverFile, string.Format(@"{0}\{1}", tempDinfo.FullName, Path.GetFileName(serverFile)));
                    }
                    var dir = new DirectoryInfo(tempDinfo.FullName);
                    listing = dir.GetFileSystemInfos().ToList();
                }
                catch
                {
                    using (UNCAccessWithCredentials unc = new UNCAccessWithCredentials())
                    {
                        if (unc.NetUseWithCredentials(serverPath, "design", "arlec", "apple"))
                        {
                            var serverFiles = Directory.GetFiles(serverPath);
                            foreach (var serverFile in serverFiles)
                            {
                                File.Copy(serverFile, string.Format(@"{0}\{1}", tempDinfo.FullName, Path.GetFileName(serverFile)));
                            }
                            var dir = new DirectoryInfo(tempDinfo.FullName);
                            listing = dir.GetFileSystemInfos().ToList();
                        }
                    }
                }
                //
                // Handle sorting
                //
                if (!string.IsNullOrEmpty(sortBy))
                {
                    if (sortBy.Equals("name"))
                    {
                        listing = listing.OrderBy(f => f.Name).ToList();
                    }
                    else if (sortBy.Equals("namerev"))
                    {
                        listing = listing.OrderByDescending(f => f.Name).ToList();
                    }
                    else if (sortBy.Equals("date"))
                    {
                        listing = listing.OrderBy(f => f.LastAccessTime).ToList();
                    }
                    else if (sortBy.Equals("daterev"))
                    {
                        listing = listing.OrderByDescending(f => f.LastAccessTime).ToList();
                    }
                    else if (sortBy.Equals("size"))
                    {
                        listing = listing.OrderBy(f => ((int)(((FileInfo)f).Length * 10 / (double)1024) / (double)10)).ToList();
                    }
                    else if (sortBy.Equals("sizerev"))
                    {
                        listing = listing.OrderByDescending(f => ((int)(((FileInfo)f).Length * 10 / (double)1024) / (double)10)).ToList();
                    }
                }

                DirectoryListing.DataSource = listing;
                DirectoryListing.DataBind();

                ////
                ////  Prepare the file counter label
                ////
                FileCount.Text = listing.Count + " items.";

                ////
                ////
                ////  Parepare the parent path label
                //path = VirtualPathUtility.AppendTrailingSlash(Context.Request.Path);
                //if (path.Equals("/") || path.Equals(VirtualPathUtility.AppendTrailingSlash(HttpRuntime.AppDomainAppVirtualPath)))
                //{
                //    // cannot exit above the site root or application root
                //    parentPath = null;
                //}
                //else
                //{
                //    parentPath = VirtualPathUtility.Combine(path, "..");
                //}

                //if (string.IsNullOrEmpty(parentPath))
                //{
                //    NavigateUpLink.Visible = false;
                //    NavigateUpLink.Enabled = false;
                //}
                //else
                //{
                //    NavigateUpLink.NavigateUrl = parentPath;
                //}
            }
        }
        protected string GetFileSizeString(FileSystemInfo info)
        {
            if (info is FileInfo)
            {
                return string.Format("- {0}K", ((int)(((FileInfo)info).Length * 10 / (double)1024) / (double)10));
            }
            else
            {
                return string.Empty;
            }
        }
        protected string DisplayFileIcon(FileSystemInfo fsi)
        {
            var result = "/images/PDFIcon.png";
            //  Assume that this entry is a file.
            var entryType = "File";

            // Determine if entry is really a directory
            if ((fsi.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                entryType = "Directory";
            }
            if (entryType == "File")
            {
                if(!fsi.Extension.Equals(".pdf"))
                {
                    result = "/images/FileIcon.png";
                }
            }
            else if (entryType == "Directory")
            {
                result = "/images/FolderIcon.png";
            }
            return result;
        }
        private static long lastTimeStamp = DateTime.UtcNow.Ticks;
        public static long UtcNowTicks
        {
            get
            {
                long original, newValue;
                do
                {
                    original = lastTimeStamp;
                    long now = DateTime.UtcNow.Ticks;
                    newValue = Math.Max(now, original + 1);
                } while (Interlocked.CompareExchange
                             (ref lastTimeStamp, newValue, original) != original);

                return newValue;
            }
        }
    }

}