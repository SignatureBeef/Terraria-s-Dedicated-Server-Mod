#define ENABLE_NAT

#if ENABLE_NAT
using Terraria;
#endif
namespace tdsm.api.Callbacks
{
    //https://github.com/mono/Mono.Nat/releases
    public static class NAT
    {
        public static bool EnableNAT = false;

        public static void OpenPort()
        {
#if ENABLE_NAT
            if (EnableNAT)
            {
                Netplay.portForwardIP = Netplay.LocalIPAddress();
                Netplay.portForwardPort = Netplay.serverPort;

                Mono.Nat.NatUtility.DeviceFound += NatUtility_DeviceFound;
                Mono.Nat.NatUtility.StartDiscovery();
            }
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
            if (EnableNAT)
            {
                var current = e.Device.GetAllMappings();
                if (current != null)
                {
                    foreach (var map in current)
                    {
                        if (map.Protocol == Mono.Nat.Protocol.Tcp && map.PrivatePort == Netplay.portForwardPort && map.PublicPort == Netplay.portForwardPort)
                        {
                            Netplay.portForwardOpen = true;
                        }
                    }
                }

                if (!Netplay.portForwardOpen)
                {
                    var terrariaMap = new Mono.Nat.Mapping(Mono.Nat.Protocol.Tcp, Netplay.portForwardPort, Netplay.portForwardPort)
                    {
                        Description = "Terraria Server"
                    };
                    e.Device.CreatePortMap(terrariaMap);
                    Tools.WriteLine("Created a new NAT map record for Terraria Server");
                    Netplay.portForwardOpen = true;
                }
                else
                {
                    Tools.WriteLine("Detected an existing NAT map record for Terraria Server");
                }
            }
#endif
        }

        public static void ClosePort()
        {
            //if (Netplay.portForwardOpen)
            //{
            //    //Netplay.mappings.Remove(Netplay.portForwardPort, "TCP");
            //}
        }
    }
}
