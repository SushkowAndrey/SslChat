using System;

namespace SslTcpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string certificate = null;
            if (args == null ||args.Length < 1 )
            {
                SSL_TCP_Server.DisplayUsage();
            }
            certificate = args[0];
            SSL_TCP_Server.RunServer(certificate);
        }
    }
}