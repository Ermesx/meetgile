namespace IntranetCalendarReader.Console
{
    using System.Net;
    using System.Net.Http;
    using Microsoft.Extensions.Options;
    using Options;

    public class
        NltmHttpClientHandler : HttpClientHandler
    {
        public NltmHttpClientHandler(IOptions<IntranetConfig> options)
        {
            var config = options.Value;

            var credentials = string.IsNullOrWhiteSpace(config.User)
                ? CredentialCache.DefaultNetworkCredentials
                : new NetworkCredential(config.User, config.Password);

            Credentials = new CredentialCache
            {
                { config.Url, "NTLM", credentials }
            };
        }
    }
}