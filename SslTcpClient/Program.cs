

namespace SslTcpClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string serverCertificateName = null;
            string machineName = null;
            if (args == null ||args.Length <1 )
            {
                SSL_TCP_CLient.DisplayUsage();
            }

            machineName = args[0];
            if (args.Length <2 )
            {
                serverCertificateName = machineName;
            }
            else
            {
                serverCertificateName = args[1];
            }
            SSL_TCP_CLient.RunClient (machineName, serverCertificateName);
        }
    }
}