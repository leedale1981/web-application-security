using System.Text;
using System.Net.Sockets;
using System.Net;

if (args.Length < 1)
{
    Console.WriteLine("Usage <hostname>");
}
else
{
    HostLookup();
    WebServerId();
}

void WebServerId()
{
    IPAddress[] addresses = Dns.GetHostAddresses(args[0]);

    if (addresses == null || addresses.Length == 0)
    {
        Console.WriteLine($"Couldn't lookup {args[0]}");
    }
    else
    {
        foreach (IPAddress address in addresses)
        {
            IPEndPoint rhost = new IPEndPoint(address.Address, 80);
            using Socket socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.IP);
            socket.Connect(rhost);

            string stringToSend = "HEAD / HTTP/1.1\r\n\r\n";
            Console.WriteLine($"Sending string {stringToSend} to host {address.ToString()}");
            byte[] requestBytes = Encoding.ASCII.GetBytes(stringToSend);
            socket.Send(requestBytes);

            byte[] buffer = new byte[socket.ReceiveBufferSize];
            socket.Receive(buffer);

            string response = Encoding.ASCII.GetString(buffer);

            Console.WriteLine($"Response from {address.ToString()} = {response}");
        }
    }

}

void HostLookup()
{
    IPAddress[] addresses = Dns.GetHostAddresses(args[0]);

    if (addresses == null || addresses.Length == 0)
    {
        Console.WriteLine($"Couldn't lookup {args[0]}");
    }
    else
    {
        foreach (IPAddress address in addresses)
        {
            Console.WriteLine($"{address.ToString()} - {address.AddressFamily}");
        }
    }
}

