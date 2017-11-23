using AspnetCoreFunctionalTestDemo;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Microsoft.AspNetCore.WebSockets.Internal.Constants;

namespace FunctionalTest
{
    public class ValuesControllerTest
    {
        [Fact]
        public void GetValuesTest()
        {
            var client = new TestServer(
                WebHost.CreateDefaultBuilder()
                       .UseStartup<Startup>()
                       .UseContentRoot(GetProjectPath("AspnetCoreFunctionalTestDemo.sln", "", typeof(Startup).Assembly))
                       ).CreateClient();
            var respone = client.GetAsync("api/values/login").Result;
            SetCookie(client, respone);
            var result = client.GetAsync("api/values").Result;
        }

        private static void SetCookie(HttpClient client, HttpResponseMessage respone)
        {
            string cookieString = respone.Headers.GetValues("Set-Cookie").First();
            string cookieBody = cookieString.Split(';').First();
            client.DefaultRequestHeaders.Add("Cookie", cookieBody);
        }

        /// <summary>
        /// 获取工程路径
        /// </summary>
        /// <param name="slnName">解决方案文件名，例test.sln</param>
        /// <param name="solutionRelativePath">如果项目与解决方案文件不在一个目录，例如src文件夹中，则传src</param>
        /// <param name="startupAssembly">程序集</param>
        /// <returns></returns>
        private static string GetProjectPath(string slnName, string solutionRelativePath, Assembly startupAssembly)
        {
            string projectName = startupAssembly.GetName().Name;
            string applicationBasePath = PlatformServices.Default.Application.ApplicationBasePath;
            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                var solutionFileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, slnName));
                if (solutionFileInfo.Exists)
                {
                    return Path.GetFullPath(Path.Combine(directoryInfo.FullName, solutionRelativePath, projectName));
                }

                directoryInfo = directoryInfo.Parent;
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"Solution root could not be located using application root {applicationBasePath}.");
        }
    }
}
