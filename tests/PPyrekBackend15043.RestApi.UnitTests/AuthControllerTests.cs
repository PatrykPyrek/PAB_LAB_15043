using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PPyrekBackend15043.RestApi.UnitTests.Helpers;

namespace PPyrekBackend15043.RestApi.UnitTests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private AuthController _controller = default!;

        [SetUp]
        public void SetUp()
        {
            var config = TestConfiguration.CreateJwtConfig();
            _controller = new AuthController(config);
        }

        [Test]
        public void GenerateToken_ValidRequest_ReturnsOkWithNonEmptyToken()
        {
            // Arrange
            var request = new AuthController.TokenRequest
            {
                Username = "testuser",
                Permissions = new[] { "read", "write" }
            };

            // Act
            var result = _controller.GenerateToken(request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>(), "Powinno być OkObjectResult");

            var ok = (OkObjectResult)result;
            var tokenProperty = ok.Value!.GetType().GetProperty("token");
            var token = tokenProperty!.GetValue(ok.Value) as string;

            Assert.That(token, Is.Not.Null.And.Not.Empty, "Token nie może być pusty");
        }

        [Test]
        public void GenerateToken_IncludesAllPermissionsClaims()
        {
            // Arrange
            var request = new AuthController.TokenRequest
            {
                Username = "tester",
                Permissions = new[] { "perm1", "perm2", "perm3" }
            };

            // Act
            var result = _controller.GenerateToken(request);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var ok = (OkObjectResult)result;
            var tokenProp = ok.Value!.GetType().GetProperty("token");
            var jwtString = tokenProp!.GetValue(ok.Value) as string;
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(jwtString!);

            // Assert 
            var perms = jwt.Claims
                .Where(c => c.Type == "permissions")
                .Select(c => c.Value)
                .ToArray();

            Assert.That(perms, Is.EquivalentTo(request.Permissions),
                        "Wygenerowany JWT musi zawierać wszystkie claimy permissions");
        }

        [Test]
        public void GenerateToken_NoPermissions_NoPermissionsClaimsPresentOnlyNameClaimCustom()
        {
            // Arrange
            var request = new AuthController.TokenRequest
            {
                Username = "emptyperms",
                Permissions = Array.Empty<string>()
            };

            // Act
            var result = _controller.GenerateToken(request);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var ok = (OkObjectResult)result;
            var jwtString = (string)ok.Value!.GetType()
                                       .GetProperty("token")!
                                       .GetValue(ok.Value)!;

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(jwtString);
            var customClaims = jwt.Claims
                .Where(c => c.Type == ClaimTypes.Name || c.Type == "permissions")
                .ToList();

            // Assert
            Assert.That(customClaims.Count, Is.EqualTo(1),
                        "Powinien być tylko jeden custom claim (Name) gdy brak permissions");
            var nameClaim = customClaims.Single();
            Assert.That(nameClaim.Type, Is.EqualTo(ClaimTypes.Name));
            Assert.That(nameClaim.Value, Is.EqualTo(request.Username));
            Assert.That(jwt.Claims.Where(c => c.Type == "permissions"), Is.Empty,
                        "Nie powinno być żadnych claimów typu permissions");
        }

        [Test]
        public void GenerateToken_HasCorrectIssuerAndAudience()
        {
            // Arrange
            var request = new AuthController.TokenRequest
            {
                Username = "issuerTest",
                Permissions = new[] { "x" }
            };

            // Act
            var result = _controller.GenerateToken(request);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var ok = (OkObjectResult)result;
            var jwtString = (string)ok.Value!.GetType()
                                       .GetProperty("token")!
                                       .GetValue(ok.Value)!;

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(jwtString);
            var expectedIssuer = TestConfiguration.JwtIssuer;
            var expectedAudience = TestConfiguration.JwtAudience;

            // Assert
            Assert.That(jwt.Issuer, Is.EqualTo(expectedIssuer),
                        "Issuer w tokenie powinien zgadzać się z konfiguracją");
            Assert.That(jwt.Audiences, Does.Contain(expectedAudience),
                        "Audience w tokenie powinno zawierać wartość z konfiguracji");
        }
    }
}
