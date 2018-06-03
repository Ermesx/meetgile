namespace IntranetCalendar.Provider.Test
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging.Abstractions;
    using Xunit;
    using Xunit.Abstractions;

    public class IntranetClientTests
    {
        private readonly ITestOutputHelper _output;
        private readonly HttpClient _client;

        public IntranetClientTests(ITestOutputHelper output)
        {
            var config = new ConfigurationBuilder().AddUserSecrets<IntranetClientTests>().Build();
            output.WriteLine($"Intranet Url: {config["IntranetConfig:Url"]}");

            _output = output;
            var handler = new HttpClientHandler
            {
                Credentials = new CredentialCache { { new Uri($"{config["IntranetConfig:Url"]}"), "NTLM", CredentialCache.DefaultNetworkCredentials } },

                // For Fiddler tracing
                ////Proxy = new WebProxy(new Uri("http://127.0.0.1:8888"), true),
                ////UseProxy = true,
                ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
            };

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri($"{config["IntranetConfig:Url"]}/Default.aspx?PageId=884")
            };
        }   

        [Fact]
        public async Task CanGetCalendarData()
        {
            // Arrange
            var now = DateTime.Now.AddDays(3);
            var client = new IntranetClient(_client, NullLogger<IntranetClient>.Instance);

            // Act 
            var data = await client.GetCalendarBodyAsync(now, "Zasoby ogólne IV pietro cz.2");

            // Assert
            _output.WriteLine(data);

            Assert.NotNull(data);
            Assert.Contains($"value=\"{now:d}\"", data);
        }

        [Fact]
        public async Task ThrowExceptionWhenFloorIsIncorrect()
        {
            // Arrange
            var client = new IntranetClient(_client, NullLogger<IntranetClient>.Instance);

            // Act & Assert
            await Assert.ThrowsAsync<FloorDoesntExistException>(() => client.GetCalendarBodyAsync(DateTime.Now, "incorrect"));
        }

        [Theory] 
        [InlineData("2018-05-03", "Zasoby ogólne II piętro cz.1")]
        [InlineData("2018-05-04", "Zasoby ogólne II piętro cz.2")]
        [InlineData("2018-05-07", "Zasoby ogólne III piętro")]
        [InlineData("2018-05-08", "Zasoby ogólne IV piętro")]
        [InlineData("2018-05-09", "Zasoby ogólne IV pietro cz.2")]
        [InlineData("2018-05-09", "Zasoby ogólne VI")]
        public async Task RecognizeDateAndFloor(string dateString, string floor)
        {
            // Arrange 
            var date = DateTime.Parse(dateString);
            var client = new IntranetClient(_client, NullLogger<IntranetClient>.Instance);

            // Act 
            var data = await client.GetCalendarBodyAsync(date, floor);

            // Assert
            Assert.Contains($"||{floor}", data);
            Assert.Contains($"value=\"{date:d}\"", data);
        }
    }
}