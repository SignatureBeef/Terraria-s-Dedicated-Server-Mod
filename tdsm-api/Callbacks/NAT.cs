#define ENABLE_NAT

#if ENABLE_NAT
using Terraria;
#endif
using System;

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
            try
            {
                if(e.Device is Mono.Nat.Upnp.UpnpNatDevice) //TODO, see if Pmp should work as well
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
            }
            catch(Exception ex)
            {
                Tools.WriteLine(ex);
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

        
//        static class NatUtility
//        {
//            private static bool _discovering;
//            private static Socket _discoverer;
//            
//            public delegate void DiscoveryError(DiscoverError err);
//            public static DiscoveryError OnDiscoveryError;
//            
//            public enum DiscoverResult
//            {
//                Started,
//                InProcess
//            }
//            public enum DiscoverError
//            {
//                NoLocalIP
//            }
//            public static DiscoverResult Discover(int broadcastPort)
//            {
//                if (null == _discoverer && !_discovering)
//                {
//                    _discovering = true;
//                    System.Threading.ThreadPool.QueueUserWorkItem(Discovery, broadcastPort);
//                    return DiscoverResult.Started;
//                } else
//                    return DiscoverResult.InProcess;
//            }
//            
//            private static string GetLocalIP()
//            {
//                
//                var itfs = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
//                foreach (var itf in itfs)
//                {
//                    if (itf.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
//                    {
//                        var props = itf.GetIPProperties();
//                        //                        var old = props.GetIPv4Properties();
//                        //                        var nw = props.GetIPv6Properties();
//                        foreach (var add in props.UnicastAddresses)
//                        {
//                            if (add.DuplicateAddressDetectionState == System.Net.NetworkInformation.DuplicateAddressDetectionState.Preferred)
//                            {
//                                if (add.AddressPreferredLifetime != UInt32.MaxValue)
//                                {
//                                    
//                                }
//                            }
//                        }
//                    }
//                }
//                
//                var entry = Dns.GetHostEntry(Dns.GetHostName());
//                foreach (var ip in entry.AddressList)
//                {
//                    if (ip.AddressFamily == AddressFamily.InterNetwork || ip.AddressFamily == AddressFamily.InterNetworkV6)
//                    {
//                        return ip.ToString();
//                    }
//                }
//                
//                return null;
//            }
//            
//            private static void ClearSock()
//            {
//                if (_discoverer != null)
//                {
//                    if (_discoverer.Connected)
//                        _discoverer.Close();
//                    _discoverer.Dispose();
//                    _discoverer = null;
//                }
//            }
//            
//            private static void Discovery(object broadcastPort)
//            {
//                _discoverer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
//                _discoverer.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
//                
//                var ip = GetLocalIP();
//                if (String.IsNullOrEmpty(ip))
//                {
//                    if(OnDiscoveryError != null) OnDiscoveryError.Invoke(DiscoverError.NoLocalIP);
//                    ClearSock();
//                    return;
//                }
//                
//                var request = new StringBuilder();
//                request.Append("M-SEARCH * HTTP/1.1\r\n");
//                request.Append("HOST: ");
//                request.Append(ip);
//                request.Append(':');
//                request.Append(broadcastPort.ToString());
//                request.Append("\r\n");
//                request.Append("ST:upnp:rootdevice\r\n");
//                request.Append("MAN:\"ssdp:discover\"\r\n");
//                request.Append("MX:3\r\n\r\n");
//            }
//        }
    }
}
