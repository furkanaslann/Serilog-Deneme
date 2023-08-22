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
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Loopback, 0); // IpEndPoint tan�mlad�k. 0 portu verirken sistem kendisi otomatik bo� ilk porta atayacakt�r.
                Socket clientsocket = new Socket(SocketType.Stream, ProtocolType.Tcp); // Soket tipini tan�mlad�k.
                clientsocket.Bind(clientEndPoint); // EndPoint'i socket'e ba�lad�k.

                // Create connection
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, ServerPortNmb); // IpEndPoint tan�mlad�k. 
                                                                                               // Ba�lant�y� ba�latan Client k�s�m oldu�u i�in end point olarak 'serverendpoint'i tan�mlamam�z gerekiyor.
                clientsocket.Connect(serverEndPoint); // End point'i socket'e ba�lad�k.
                logger.LogInformation("{Time} User is connected to the socket! ", DateTime.Now);

                // Send message
                string messageToSend = "Hi Robot! How are you?"; // G�nderece�imiz mesaj� belirledik.
                byte[] byteToSend = Encoding.Default.GetBytes(messageToSend); // Mesaj� byte t�r�ne �evirdik.
                clientsocket.Send(byteToSend); // Byte mesaj� socket'ten g�nderdik.
                Console.WriteLine($"Server send message: {messageToSend}");  // Kullan�c�ya mesaj� log'lad�k.

                // Display received message
                byte[] buffer = new byte[1024]; // Kaynak dizi tan�mlad�k.
                int nmberOfBytesReceived = clientsocket.Receive(buffer); // Array kadar socket'ten bilgiyi �ektik.
                byte[] receivedBytes = new byte[nmberOfBytesReceived]; // Bilgiyi anlanm� hale getirmek i�in bilgiyi kopyalayaca��m�z 2. array(hedef) belirledik.
                Array.Copy(buffer, receivedBytes, nmberOfBytesReceived); // 1. Parametre(kaynak), 2. Parametre(hedef), 3.Kopyalanacak Adet/Boyut
                string receivedMessage = Encoding.Default.GetString(receivedBytes); // Mesaj� string de�ere �evirdik.
                Console.WriteLine($"Server received message: {receivedMessage}"); // Kullan�c�ya mesaj� log'lad�k.
                Console.ReadLine();
            }




            void Server()
            {
                // Create a Socket
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, ServerPortNmb); // IpEndPoint tan�mlad�k.
                Socket welcomingSocket = new Socket(SocketType.Stream, ProtocolType.Tcp); // Soket tipini tan�mlad�k.
                welcomingSocket.Bind(serverEndPoint); // EndPoint'i socket'e ba�lad�k.

                // Wait for connection
                welcomingSocket.Listen(ServerPortNmb); // Port numaras�n� dinliyoruz.
                Socket connectionSocket = welcomingSocket.Accept(); // Gelen ba�lant� isteklerini kabul ediyoruz(Accept), ba�lant� soketimize aktar�yoruz.

                // Display received message
                byte[] buffer = new byte[1024]; // Kaynak dizi tan�mlad�k.
                int nmberOfBytesReceived = connectionSocket.Receive(buffer); // Array kadar socket'ten bilgiyi �ektik.
                byte[] receivedBytes = new byte[nmberOfBytesReceived]; // Bilgiyi anlanm� hale getirmek i�in bilgiyi kopyalayaca��m�z 2. array(hedef) belirledik.
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