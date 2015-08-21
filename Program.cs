using System;
using System.IO;
using NConsoler;

namespace Musuk.AddSite
{
    class Program
    {
        public static void Main(params string[] args)
        {
            Consolery.Run(typeof(Program), args);
        }

        [Action]
        public static void Method(
            [Required(Description = "Host name for site binding. Site will have the same name too.")] 
            string hostName,
            [Optional(null, "path", Description = "Site home directory path. Current directory by default")] 
            string physicalPath,
            [Optional("127.0.0.1", Description = "Site binding IP. [127.0.0.1] by default")] 
            string ip,
            [Optional(80, Description = "Site binding port. [80] by default.")]
            int port,
            [Optional(".NET v4.5", "pool", Description = "Application pool for site. [.NET v4.5] by default")]
            string appPool
        )
        {
            try
            {
                ProcessBuildSite(ip, physicalPath, hostName, appPool, port);
                Console.WriteLine("Done.");
            }
            catch(FileNotFoundException e)
            {
                WriteError("{0} file {1}", e.Message, e.FileName);
            }
            catch (Exception e)
            {
                WriteError(e.Message);
            }
        }

        private static void WriteError(string message, params string[] param)
        {
            Console.WriteLine("Site creating failed: {0}", string.Format(message, param));
        }

        private static void ProcessBuildSite(string ip, string physicalPath, string hostName, string appPool, int port)
        {
            using (var siteBuilder = new SiteBuilder())
            {
                if (!siteBuilder.IsValidIP(ip))
                {
                    Console.WriteLine("IP address [{0}] has invalid format.", ip);
                    return;
                }

                if (string.IsNullOrEmpty(physicalPath))
                    physicalPath = Directory.GetCurrentDirectory();
                if (siteBuilder.SiteExists(hostName))
                {
                    Console.WriteLine("Site with name {0} already exists.", hostName);
                    return;
                }
                if (!siteBuilder.AppPoolExists(appPool))
                {
                    Console.WriteLine("Application pool {0} does not exists.", appPool);
                    Console.WriteLine("Available application pool names are:");
                    foreach (var pool in siteBuilder.ServerManager.ApplicationPools)
                    {
                        Console.WriteLine(pool.Name);
                    }
                    return;
                }

                siteBuilder.CreateSite(ip, port, hostName, physicalPath, appPool);
                Console.WriteLine("Site was created.");

                if (!siteBuilder.HostFileRecordExists(ip, hostName))
                {
                    siteBuilder.AppendHostsFile(ip, hostName);
                    Console.WriteLine("Hosts file was recored appended.");
                }
                else
                    Console.WriteLine("Hosts file recored for this ip/host already exists.");
            }
        }
    }
}
