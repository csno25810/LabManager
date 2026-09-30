using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace LabPortal
{
    public static class LanAccess
    {
        public static bool IsGoogleHostAllowed(string hostHeader)
        {
            string host = HostName(hostHeader);
            if (string.IsNullOrEmpty(host))
                return false;
            if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase))
                return true;

            IPAddress ip;
            if (!IPAddress.TryParse(host, out ip))
                return false;
            ip = Normalize(ip);
            return ip != null && IPAddress.IsLoopback(ip);
        }

        private static string HostName(string hostHeader)
        {
            if (string.IsNullOrEmpty(hostHeader))
                return "";

            hostHeader = hostHeader.Trim();
            if (hostHeader.StartsWith("["))
            {
                int end = hostHeader.IndexOf(']');
                return end > 1 ? hostHeader.Substring(1, end - 1) : hostHeader;
            }

            int colon = hostHeader.LastIndexOf(':');
            if (colon > 0 && hostHeader.IndexOf(':') == colon)
                return hostHeader.Substring(0, colon);
            return hostHeader;
        }

        public static bool IsSameLan(IPAddress client)
        {
            client = Normalize(client);
            if (client == null)
                return false;
            if (IPAddress.IsLoopback(client))
                return true;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus != OperationalStatus.Up)
                    continue;

                foreach (UnicastIPAddressInformation address in nic.GetIPProperties().UnicastAddresses)
                {
                    IPAddress local = Normalize(address.Address);
                    if (local == null || local.AddressFamily != AddressFamily.InterNetwork)
                        continue;
                    if (IPAddress.IsLoopback(local))
                        continue;
                    if (InSameSubnet(client, local, address.IPv4Mask))
                        return true;
                }
            }

            return false;
        }

        private static IPAddress Normalize(IPAddress address)
        {
            if (address == null)
                return null;
            if (address.IsIPv4MappedToIPv6)
                return address.MapToIPv4();
            return address;
        }

        private static bool InSameSubnet(IPAddress client, IPAddress local, IPAddress mask)
        {
            if (client.AddressFamily != AddressFamily.InterNetwork ||
                local.AddressFamily != AddressFamily.InterNetwork ||
                mask == null || mask.AddressFamily != AddressFamily.InterNetwork)
            {
                return false;
            }

            byte[] clientBytes = client.GetAddressBytes();
            byte[] localBytes = local.GetAddressBytes();
            byte[] maskBytes = mask.GetAddressBytes();
            for (int i = 0; i < 4; i++)
            {
                if ((clientBytes[i] & maskBytes[i]) != (localBytes[i] & maskBytes[i]))
                    return false;
            }

            return true;
        }
    }
}
