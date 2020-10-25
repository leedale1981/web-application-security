using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace MutationalFuzzer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //FuzzGetParams(args);
            FuzzPostParams(args);
        }

        private static void FuzzPostParams(string[] args)
        {
            string[] requestLines = File.ReadAllLines(args[0]);
            string[] parameters = requestLines[requestLines.Length - 1].Split('&');
            string host = string.Empty;
            StringBuilder requestBuilder = new StringBuilder();

            foreach (string line in requestLines)
            {
                if (line.StartsWith("Host:"))
                {
                    host = line.Split(' ')[1].Replace("\r", string.Empty);
                }

                requestBuilder.Append(line + "\n");
            }

            string request = requestBuilder.ToString() + "\r\n";
            Console.WriteLine(request);

            IPEndPoint rhost = new IPEndPoint(IPAddress.Parse(host), 80);

            foreach (string parameter in parameters)
            {
                using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    sock.Connect(rhost);

                    string val = parameter.Split('=')[1];
                    string req = request.Replace("=" + val, "=" + val + "'");

                    byte[] reqBytes = Encoding.ASCII.GetBytes(req);
                    sock.Send(reqBytes);

                    byte[] buf = new byte[sock.ReceiveBufferSize];
                    sock.Receive(buf);

                    string response = Encoding.ASCII.GetString(buf);

                    if (response.Contains("error in your SQL syntax"))
                    {
                        Console.WriteLine("Parameter " + parameter + " seems vulnerable");
                        Console.Write(" to SQL injection with value: " + val + "'");
                    }
                }
            }
        }

        private static void FuzzGetParams(string[] args)
        {
            string url = args[0];
            int index = url.IndexOf("?");
            string[] parameters = url.Remove(0, index + 1).Split('&');
            
            foreach(string param in parameters)
            {
                string xssUrl = url.Replace(param, $"{param}fd<xss>sa");
                string sqlUrl = url.Replace(param, $"{param}fd'sa");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sqlUrl);
                request.Method = "GET";

                string sqlResponse = string.Empty;
                using (StreamReader sqlReader = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    sqlResponse = sqlReader.ReadToEnd();
                    request = (HttpWebRequest)WebRequest.Create(xssUrl);
                    request.Method = "GET";
                    string xssResponse = string.Empty;

                    using (StreamReader xssReader = new StreamReader(request.GetResponse().GetResponseStream()))
                    {
                        xssResponse = xssReader.ReadToEnd();

                        if (xssResponse.Contains("<xss>"))
                        {
                            Console.WriteLine($"Possible XSS found in parameter {param}");
                        }

                        if (sqlResponse.ToLower().Contains("error in your sql syntax"))
                        {
                            Console.WriteLine($"SQL Injection found in parameter {param}");
                        }
                    }
                }            
            }
        }
    }
}