using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace PPyrekBackend15043.RestApi.UnitTests.Helpers
{
    public static class TestConfiguration
    {
        public const string JwtKey = "0123456789ABCDEF0123456789ABCDEF";  
        public const string JwtIssuer = "https://unit.test";
        public const string JwtAudience = "https://unit.test.audience";

        public static IConfiguration CreateJwtConfig()
        {
            var dict = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = JwtKey,
                ["Jwt:Issuer"] = JwtIssuer,
                ["Jwt:Audience"] = JwtAudience
            };
            return new ConfigurationBuilder()
                   .AddInMemoryCollection(dict)
                   .Build();
        }
    }

}
