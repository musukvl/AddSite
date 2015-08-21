using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Web.Administration;

namespace Musuk.AddSite
{
    public class SiteBuilder : IDisposable
    {
        public ServerManager ServerManager
        {
            get; 
            protected set;
        }

        public SiteBuilder()
        {
            this.ServerManager = new ServerManager();
        }
        
        public bool SiteExists(string siteName)
        {
            return ServerManager.Sites.Any(s => s.Name == siteName);
        } 

        public bool AppPoolExists(string appPoolName)
        {
            return ServerManager.ApplicationPools.Any(p => p.Name == appPoolName);
        }

        public string BuildBindingInfo(string ip, string port, string hostName)
        {
            return string.Format("{0}:{1}:{2}", ip, port, hostName);
        }

        public void CreateSite(string siteName, string bindingInfo, string physicalPath, string appPool)
        {
            var site = ServerManager.Sites.Add(siteName, "http", bindingInfo, physicalPath);            
            site.Applications[0].ApplicationPoolName = appPool;            
            site.ServerAutoStart = true;
            ServerManager.CommitChanges();
        }

        public void CreateSite(string ip, int port, string hostName, string physicalPath, string appPool)
        {
            var bindingInfo = BuildBindingInfo(ip, port.ToString(), hostName);
            CreateSite(hostName, bindingInfo, physicalPath, appPool);
        }

        private string GetHostsFilePath()
        {
            return System.Environment.ExpandEnvironmentVariables(@"%WinDir%/System32/drivers/etc/hosts");
        }

        public bool HostFileRecordExists(string ip, string hostname)
        {
            string hostsFile;
            using (var stream = new StreamReader(File.OpenRead(GetHostsFilePath())))
            {
                hostsFile = stream.ReadToEnd();
            }
            var matchRecordRegexPattern = string.Format(@"{0}\s+{1}", Regex.Escape(ip), Regex.Escape(hostname));
            return Regex.Match(hostsFile, matchRecordRegexPattern).Success;
        }

        public void AppendHostsFile(string ip, string hostname)
        {
            var hostsFilePath = GetHostsFilePath();
            if (!File.Exists(hostsFilePath))
                throw new FileNotFoundException("Hosts file not found", hostsFilePath);
            bool addNewLineBeforeRecord = (!File.ReadAllText(hostsFilePath).EndsWith(Environment.NewLine));
            
            using (var appender = File.AppendText(hostsFilePath))
            {
                if (addNewLineBeforeRecord)
                    appender.WriteLine();
                appender.WriteLine("{0} \t{1}", ip, hostname);
            }
        }

        

        public bool IsValidIP(string addr)
        {
            const string pattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
            Regex check = new Regex(pattern, RegexOptions.Compiled);
            return check.IsMatch(addr, 0);
        }

        public void Dispose()
        {
            ServerManager.Dispose();
        }
    }
}
