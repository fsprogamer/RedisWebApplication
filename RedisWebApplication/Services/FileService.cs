using Newtonsoft.Json;
using RedisWebApplication.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RedisWebApplication.Services
{
    public class FileService
    {
        const string responsePath = @"D:\Temp\http_response.json";
        public void ReadFile()
        {
            WBSReport report;
            JsonSerializer serializer = new JsonSerializer();

            using (StreamReader file = File.OpenText(responsePath))
            {
                report = (WBSReport)serializer.Deserialize(file, typeof(WBSReport));
            }
        }
    }
}
