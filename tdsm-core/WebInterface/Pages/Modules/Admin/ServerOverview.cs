
using System;
using System.Timers;
using Terraria;
namespace tdsm.core.WebInterface.Pages.Admin
{
    /*
     * Note, Modules are not to send HTML, rather they are to send API information
     */
    [WebModule(Url = "/api/server/overview", Nodes = new string[]
    {
        "tdsm.web.server.overview",
        "*" //Temp
    })]
    public class ServerOverview : WebPage
    {
        static readonly ResourceDependancy[] _dependencies = new ResourceDependancy[]
        {
            new ResourceDependancy()
            {
                Type = ResourceType.Javascript,
                Url = "modules/serveroverview.js"
            }
        };
        public override ResourceDependancy[] GetDependencies()
        {
            CheckTimer();
            return _dependencies;
        }

        static ProcessInfo? _info;
        static void Watch(object sender, ElapsedEventArgs e)
        {
            _info = ResourceMonitor.GetProcessInformation();
            //_memCounter = new System.Diagnostics.PerformanceCounter("Mono Memory", "Total Physical Memory", String.Empty, Environment.MachineName);
        }

        static void CheckTimer()
        {
            if (_tmr == null)
            {
                //Get the information immediately for the current caller
                _info = ResourceMonitor.GetProcessInformation();

                _tmr = new Timer(3000);
                _tmr.Elapsed += Watch;
                _tmr.Start();
            }
        }

        static Timer _tmr;
        //static System.Diagnostics.PerformanceCounter _memCounter;

        public override void ProcessRequest(WebRequest request)
        {
            CheckTimer();
            request.StatusCode = 200;

            if (_info != null)
            {
                request.Writer.Buffer(true);

                var it = _info.Value;

                //Max Memory
                //request.Writer.Buffer(_memCounter.RawValue / 1024.0 / 1024.0);

                request.Writer.Buffer(it.CPU);
                request.Writer.Buffer(it.CPUTimeMs);
                request.Writer.Buffer(ResourceMonitor.CPUAverage);
                request.Writer.Buffer(ResourceMonitor.CPUMax);

                request.Writer.Buffer(8.0 * 1024.0 * 1024.0 * 1024.0);

                request.Writer.Buffer(it.Virtual);
                request.Writer.Buffer(it.VirtualMax);
                request.Writer.Buffer(ResourceMonitor.VirtualAverage);

                request.Writer.Buffer(it.Working);
                request.Writer.Buffer(it.WorkingMax);
                request.Writer.Buffer(ResourceMonitor.WorkingAverage);

                request.Writer.Buffer(tdsm.api.Tools.ActivePlayerCount);
                request.Writer.Buffer(Main.maxNetPlayers);

                //Server is running
                request.Writer.Buffer(Netplay.ServerUp);
                request.Writer.Buffer(ServerCore.Server.AcceptNewConnections);

                //System.Diagnostics.Debug.WriteLine("it.CPU: " + it.CPU);
                //System.Diagnostics.Debug.WriteLine("it.Virtual: " + it.Virtual);
                //System.Diagnostics.Debug.WriteLine("it.Working: " + it.Working);
            }
            else
            {
                request.Writer.Buffer(false);
            }

            request.WriteOut();
        }
    }

    static class ResourceMonitor
    {
        //TODO removing of old values (e.g. older than 10 minutes)

        static int _cpuTally = 0;
        static double _cpuTotal = 0;
        static double _cpuLargest = 0;

        static int _virtTally = 0;
        static long _virtTotal = 0;

        static int _workTally = 0;
        static long _workTotal = 0;

        public static double CPUMax
        {
            get
            { return _cpuLargest; }
        }

        public static double CPUAverage
        {
            get
            { return _cpuTotal / _cpuTally; }
        }

        public static long VirtualAverage
        {
            get
            { return _virtTotal / _virtTally; }
        }

        public static long WorkingAverage
        {
            get
            { return _workTotal / _workTally; }
        }

        public static ProcessInfo GetProcessInformation()
        {
            using (var process = System.Diagnostics.Process.GetCurrentProcess())
            {
                var time = process.TotalProcessorTime;
                var inf = new ProcessInfo()
                {
                    CPU = 100.0 * time.TotalMilliseconds / (DateTime.Now - process.StartTime).TotalMilliseconds,
                    CPUTimeMs = time.TotalMilliseconds,

                    Virtual = process.VirtualMemorySize64,
                    VirtualMax = process.PeakVirtualMemorySize64,

                    Working = process.WorkingSet64,
                    WorkingMax = process.PeakWorkingSet64
                };

                _cpuTotal += inf.CPU;
                _cpuTally++;

                _virtTotal += inf.Virtual;
                _virtTally++;

                _workTotal += inf.Working;
                _workTally++;

                if (_cpuLargest < inf.CPU) _cpuLargest = inf.CPU;

                return inf;
            }
        }
    }

    public struct ProcessInfo
    {
        public double CPU { get; set; }
        public double CPUTimeMs { get; set; }

        public long Virtual { get; set; }
        public long VirtualMax { get; set; }

        public long Working { get; set; }
        public long WorkingMax { get; set; }
    }
}
