using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;

namespace Microsoft.ServiceModel.Samples.BasicWebProgramming
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        [WebGet]
        string EchoWithGet(string s);

        [OperationContract]
        [WebInvoke]
        string EchoWithPost(string s);
    }

    public class Service : IService
    {
        public string EchoWithGet(string s)
        {
            return "You said " + s;
        }

        public string EchoWithPost(string s)
        {
            return "You said " + s;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(Service), new Uri("http://localhost:8000"));
            host.AddServiceEndpoint(typeof(IService), new BasicHttpBinding(), "Soap");//SOAP
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(IService), new WebHttpBinding(), "Web");//BasicWeb
            endpoint.Behaviors.Add(new WebHttpBehavior());

            try
            {
                host.Open();

                using (WebChannelFactory<IService> wcf = new WebChannelFactory<IService>(new Uri("http://localhost:8000/Web")))
                {
                    IService channel = wcf.CreateChannel();

                    string s;

                    Console.WriteLine("Calling EchoWithGet by HTTP GET: ");
                    s = channel.EchoWithGet("Hello, world");
                    Console.WriteLine("   Output: {0}", s);

                    Console.WriteLine("");
                    Console.WriteLine("This can also be accomplished by navigating to");
                    Console.WriteLine("http://localhost:8000/Web/EchoWithGet?s=Hello, world!");
                    Console.WriteLine("in a web browser while this sample is running.");

                    Console.WriteLine("");

                    Console.WriteLine("Calling EchoWithPost by HTTP POST: ");
                    s = channel.EchoWithPost("Hello, world");
                    Console.WriteLine("   Output: {0}", s);
                    Console.WriteLine("");
                }
                
                using (ChannelFactory<IService> scf = new ChannelFactory<IService>(new BasicHttpBinding(), "http://localhost:8000/Soap"))
                {
                    //WithSOAP
                    IService channel = scf.CreateChannel();

                    string s;

                    Console.WriteLine("Calling EchoWithGet on SOAP endpoint: ");
                    s = channel.EchoWithGet("Hello, world");
                    Console.WriteLine("   Output: {0}", s);

                    Console.WriteLine("");

                    Console.WriteLine("Calling EchoWithPost on SOAP endpoint: ");
                    s = channel.EchoWithPost("Hello, world");
                    Console.WriteLine("   Output: {0}", s);
                    Console.WriteLine("");
                }

                Console.WriteLine("Press [Enter] to terminate");
                Console.ReadLine();
                host.Close();
            }
            catch (CommunicationException cex)
            {
                Console.WriteLine("An exception occurred: {0}", cex.Message);
                host.Abort();
            }
        }
    }
}