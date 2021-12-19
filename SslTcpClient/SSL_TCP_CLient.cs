using System;
using System.Collections;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace SslTcpClient
{
    public class SSL_TCP_CLient
    {
        private static Hashtable _certificateErrors = new Hashtable();
        
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
           if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

           Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            
           return false;
        }
        public static void RunClient(string machineName, string serverName)
        {
            var client = new TcpClient(machineName,443);
            Console.WriteLine("Client connected.");
            
            var sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback (ValidateServerCertificate), null);
            
            try
            {
                sslStream.AuthenticateAsClient(serverName);
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine ("Authentication failed - closing the connection.");
                client.Close();
                return;
            }
            
            var message = Encoding.UTF8.GetBytes("Hello from the client.<EOF>");
            
            sslStream.Write(message);
            sslStream.Flush();
            
            var serverMessage = ReadMessage(sslStream);
            Console.WriteLine("Server says: {0}", serverMessage);
            
            client.Close();
            Console.WriteLine("Client closed.");
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
            Console.WriteLine("To start the client specify:");
            Console.WriteLine("clientSync machineName [serverName]");
            Environment.Exit(1);
        }
    }
}