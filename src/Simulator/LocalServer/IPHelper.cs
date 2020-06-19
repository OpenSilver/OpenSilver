using System.Net;
using System.Net.Sockets;

// Credits: http://stackoverflow.com/questions/6803073/get-local-ip-address

internal static class IPHelper
{
    internal static string GetLocalIPAddress()
    {
        //IPHostEntry host;
        //string localIP = "";
        //host = Dns.GetHostEntry(Dns.GetHostName());
        //foreach (IPAddress ip in host.AddressList)
        //{
        //    if (ip.AddressFamily == AddressFamily.InterNetwork)
        //    {
        //        localIP = ip.ToString();
        //        break;
        //    }
        //}
        //return localIP;

        string localIP;
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("10.0.2.4", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            localIP = endPoint.Address.ToString();
            return localIP;
        }
    }
}
