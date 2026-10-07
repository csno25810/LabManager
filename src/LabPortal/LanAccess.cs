using System;
using System.Net;
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

        public static bool CanCheckIn(IPAddress client)
        {
            client = Normalize(client);
            if (client == null)
                return false;
            if (IPAddress.IsLoopback(client))
                return true;
            return IsPrivate(client);
        }

        public static bool IsSameLan(IPAddress client)
        {
            return CanCheckIn(client);
        }

        private static bool IsPrivate(IPAddress client)
        {
            if (client.AddressFamily != AddressFamily.InterNetwork)
                return false;

            byte[] b = client.GetAddressBytes();
            if (b[0] == 10)
                return true;
            if (b[0] == 172 && b[1] >= 16 && b[1] <= 31)
                return true;
            if (b[0] == 192 && b[1] == 168)
                return true;
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
    }
}
