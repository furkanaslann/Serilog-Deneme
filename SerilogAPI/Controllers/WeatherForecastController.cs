using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SerilogAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("GetWeatherForecast finished!");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                
            })
            .ToArray();
        }
        public void OnGet(ILogger<WeatherForecastController> logger)
        {
            const int ServerPortNmb = 3838;

            Console.WriteLine("Enter 'S' for server, 'C' for client");
            string input = Console.ReadLine();
            logger.LogInformation("This is first console message!");
            //input.ToUpper();
            if (input == "S")
            {
                Server();
                logger.LogInformation($"{DateTime.Now} -User logged into the server! ");
            }
            else if (input == "C")
            {
                Client();
                logger.LogInformation($"{DateTime.Now} -User logged into the client! ");
            }
            else
            {
                Console.WriteLine("Unexpected input!");
            }

            void Client()
            {
                // Create a Socket
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Loopback, 0); // IpEndPoint tanýmladýk. 0 portu verirken sistem kendisi otomatik boþ ilk porta atayacaktýr.
                Socket clientsocket = new Socket(SocketType.Stream, ProtocolType.Tcp); // Soket tipini tanýmladýk.
                clientsocket.Bind(clientEndPoint); // EndPoint'i socket'e baðladýk.

                // Create connection
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, ServerPortNmb); // IpEndPoint tanýmladýk. 
                                                                                               // Baðlantýyý baþlatan Client kýsým olduðu için end point olarak 'serverendpoint'i tanýmlamamýz gerekiyor.
                clientsocket.Connect(serverEndPoint); // End point'i socket'e baðladýk.
                logger.LogInformation("{Time} User is connected to the socket! ", DateTime.Now);

                // Send message
                string messageToSend = "Hi Robot! How are you?"; // Göndereceðimiz mesajý belirledik.
                byte[] byteToSend = Encoding.Default.GetBytes(messageToSend); // Mesajý byte türüne çevirdik.
                clientsocket.Send(byteToSend); // Byte mesajý socket'ten gönderdik.
                Console.WriteLine($"Server send message: {messageToSend}");  // Kullanýcýya mesajý log'ladýk.

                // Display received message
                byte[] buffer = new byte[1024]; // Kaynak dizi tanýmladýk.
                int nmberOfBytesReceived = clientsocket.Receive(buffer); // Array kadar socket'ten bilgiyi çektik.
                byte[] receivedBytes = new byte[nmberOfBytesReceived]; // Bilgiyi anlanmý hale getirmek için bilgiyi kopyalayacaðýmýz 2. array(hedef) belirledik.
                Array.Copy(buffer, receivedBytes, nmberOfBytesReceived); // 1. Parametre(kaynak), 2. Parametre(hedef), 3.Kopyalanacak Adet/Boyut
                string receivedMessage = Encoding.Default.GetString(receivedBytes); // Mesajý string deðere çevirdik.
                Console.WriteLine($"Server received message: {receivedMessage}"); // Kullanýcýya mesajý log'ladýk.
                Console.ReadLine();
            }




            void Server()
            {
                // Create a Socket
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, ServerPortNmb); // IpEndPoint tanýmladýk.
                Socket welcomingSocket = new Socket(SocketType.Stream, ProtocolType.Tcp); // Soket tipini tanýmladýk.
                welcomingSocket.Bind(serverEndPoint); // EndPoint'i socket'e baðladýk.

                // Wait for connection
                welcomingSocket.Listen(ServerPortNmb); // Port numarasýný dinliyoruz.
                Socket connectionSocket = welcomingSocket.Accept(); // Gelen baðlantý isteklerini kabul ediyoruz(Accept), baðlantý soketimize aktarýyoruz.

                // Display received message
                byte[] buffer = new byte[1024]; // Kaynak dizi tanýmladýk.
                int nmberOfBytesReceived = connectionSocket.Receive(buffer); // Array kadar socket'ten bilgiyi çektik.
                byte[] receivedBytes = new byte[nmberOfBytesReceived]; // Bilgiyi anlanmý hale getirmek için bilgiyi kopyalayacaðýmýz 2. array(hedef) belirledik.
                Array.Copy(buffer, receivedBytes, nmberOfBytesReceived); // 1. Parametre(kaynak), 2. Parametre(hedef), 3.Kopyalanacak Adet/Boyut
                string receivedMessage = Encoding.Default.GetString(receivedBytes);
                Console.WriteLine($"Server received message: {receivedMessage}");

                // Send received message to the client
                connectionSocket.Send(receivedBytes);
                Console.ReadLine();
            }
        }
    }
}