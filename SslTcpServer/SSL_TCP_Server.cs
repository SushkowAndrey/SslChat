using System;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace SslTcpServer
{
    public class SSL_TCP_Server
    {
        static X509Certificate _serverCertificate = null;
        
        public static void RunServer(string certificate)
        {
            _serverCertificate = X509Certificate.CreateFromCertFile(certificate);
            
            var listener = new TcpListener(IPAddress.Any, 8080);
            listener.Start();
            
            while (true)
            {
                Console.WriteLine("Waiting for a client to connect...");
                
                var client = listener.AcceptTcpClient();
                ProcessClient(client);
            }
        }
        
        private static void ProcessClient (TcpClient client)
        {
            var sslStream = new SslStream(client.GetStream(), false);
            
            try
            {
                sslStream.AuthenticateAsServer(_serverCertificate, clientCertificateRequired: false, checkCertificateRevocation: true);

                sslStream.ReadTimeout = 5000;
                sslStream.WriteTimeout = 5000;
                
                Console.WriteLine("Waiting for client message...");
                var messageData = ReadMessage(sslStream);
                Console.WriteLine($"Received: {messageData}");

                
                var message = Encoding.UTF8.GetBytes("Hello from the server.<EOF>");
                Console.WriteLine("Sending hello message.");
                sslStream.Write(message);
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                if (e.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {e.InnerException.Message}");
                }
                Console.WriteLine ("Authentication failed - closing the connection.");
                sslStream.Close();
                client.Close();
            }
            finally
            {
                sslStream.Close();
                client.Close();
            }
        }

        private static string ReadMessage(SslStream sslStream)
        {
            var buffer = new byte[2048];
            var messageData = new StringBuilder();
            var bytes = -1;
            
            do
            {
                bytes = sslStream.Read(buffer, 0, buffer.Length);
                
                messageData.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
                
                if (messageData.ToString().IndexOf("<EOF>") != -1)
                {
                    break;
                }
            } while (bytes != 0);

            return messageData.ToString();
        }

        public static void DisplayUsage()
        {
            Console.WriteLine("To start the server specify:");
            Console.WriteLine("serverSync certificateFile.cer");
            Environment.Exit(1);
        }
    }
}