//#define ENABLE_NAT

namespace tdsm.api.Callbacks
{
    //https://github.com/mono/Mono.Nat/releases
    public static class NAT
    {
        public static void OpenPort()
        {
#if ENABLE_NAT
            Netplay.portForwardIP = Netplay.LocalIPAddress();
            Netplay.portForwardPort = Netplay.serverPort;

            Mono.Nat.NatUtility.DeviceFound += NatUtility_DeviceFound;
            Mono.Nat.NatUtility.StartDiscovery();
#endif

            //if (Netplay.mappings != null)
            //{
            //    foreach (IStaticPortMapping staticPortMapping in Netplay.mappings)
            //    {
            //        if (staticPortMapping.InternalPort == Netplay.portForwardPort && staticPortMapping.InternalClient == Netplay.portForwardIP && staticPortMapping.Protocol == "TCP")
            //        {
            //            Netplay.portForwardOpen = true;
            //        }
            //    }
            //    if (!Netplay.portForwardOpen)
            //    {
            //        Netplay.mappings.Add(Netplay.portForwardPort, "TCP", Netplay.portForwardPort, Netplay.portForwardIP, true, "Terraria Server");
            //        Netplay.portForwardOpen = true;
            //    }
            //}
        }

        static void NatUtility_DeviceFound(object sender, Mono.Nat.DeviceEventArgs e)
        {
#if ENABLE_NAT
            var map = new Mono.Nat.Mapping(Mono.Nat.Protocol.Tcp, Netplay.portForwardPort, Netplay.portForwardPort)
            {
                Description = "Terraria Server"
            };
            e.Device.CreatePortMap(map);
#endif
        }

        public static void ClosePort()
        {
            //if (Netplay.portForwardOpen)
            //{
            //    Netplay.mappings.Remove(Netplay.portForwardPort, "TCP");
            //}
        }
    }
}
