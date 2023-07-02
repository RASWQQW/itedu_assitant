using Newtonsoft.Json.Bson;
using StackExchange.Redis;
using System.Diagnostics;

namespace itedu_assitant.DB
{
    public class rediscontext
    {
        public ConnectionMultiplexer connectionmpx;
        public rediscontext()
        {
            var hostname = "localhost";
            var port = 6379;
            var password = "";
            connectionmpx = ConnectionMultiplexer.Connect($"{hostname}:{port}, password={password}, abortConnect=false");

            RunServer(); // to run redis server
        }

        public IDatabase GetBase()
        {
            return connectionmpx.GetDatabase();
        }

        public void RunServer()
        {
            using (var one = new Process())
            {
                ProcessStartInfo info = new ProcessStartInfo
                {
                    FileName = @"cmd.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                };

                string redisruncommamnd = "sudo service redis-server start";
                one.StartInfo = info;
                one.Start();

                one.StandardInput.WriteLine("cd C://Users/rasul");
                //one.StandardInput.WriteLine("wsl.exe--shutdown"); optional
                one.StandardInput.WriteLine($"wsl.exe {redisruncommamnd}");
                one.StandardInput.WriteLine("exit");
                one.StandardInput.Close();
                one.WaitForExit();
                Console.WriteLine(one.StandardOutput.ReadToEnd());
            }
        }

        public void RedisCleaner(object? state)
        {
            while (true)
            {
                Thread.Sleep(TimeSpan.FromMinutes(15).Milliseconds);
                var currentTime = DateTime.Now.Date.Hour;
                if (currentTime >= 0 && currentTime <= 4)
                {
                    var keys = connectionmpx.GetServer("localhost", 6279).Keys();
                    foreach (var key in keys)
                    {
                        if (key.ToString().Contains("userIp"))
                        {
                            GetBase().KeyDelete(key);
                        }
                    }
                }
            }
        }
        public void Check()
        {
            Debug.WriteLine(this.GetBase().Ping());
        }

        public void Check2()
        {
            GetBase().HashSet("entry", new HashEntry[] { new HashEntry("name", "jhon"), new HashEntry("sname", "Doe") });
            Debug.WriteLine("Val get -+" + GetBase().HashGetAsync("entry", "sname").Result);
        }

    }
}
