using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using QRCoder;

namespace LabManager
{
    public static class PortalLink
    {
        public const int DefaultPort = 8080;

        public static string FindUrl(int port = DefaultPort)
        {
            string ip = FindLanIp();
            if (string.IsNullOrEmpty(ip))
                return "";
            return "http://" + ip + ":" + port + "/";
        }

        public static Bitmap CreateQr(string url, int pixelsPerModule)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            using (var generator = new QRCodeGenerator())
            using (QRCodeData data = generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.M))
            using (var qr = new QRCode(data))
            {
                return qr.GetGraphic(pixelsPerModule, Color.Black, Color.White, false);
            }
        }

        private static string FindLanIp()
        {
            string fallback = null;
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus != OperationalStatus.Up)
                    continue;
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    continue;

                foreach (UnicastIPAddressInformation address in nic.GetIPProperties().UnicastAddresses)
                {
                    IPAddress ip = address.Address;
                    if (ip.AddressFamily != AddressFamily.InterNetwork || IPAddress.IsLoopback(ip))
                        continue;

                    byte[] b = ip.GetAddressBytes();
                    string text = ip.ToString();
                    if (b[0] == 192 && b[1] == 168)
                        return text;
                    if (b[0] == 10 || (b[0] == 172 && b[1] >= 16 && b[1] <= 31))
                    {
                        if (fallback == null)
                            fallback = text;
                    }
                }
            }

            return fallback;
        }
    }
}
