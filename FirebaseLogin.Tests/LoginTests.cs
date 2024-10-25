using Moq;
using Newtonsoft.Json;
using FirebaseLogin.Network;
using Moq.Protected;

namespace FirebaseLogin.Tests {
    public class FirebaseServiceTests {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly FirebaseService _firebaseService;

        public FirebaseServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _firebaseService = new FirebaseService(httpClient);
        }

        [Fact]
        public async Task LoginWithEmailAndPassword_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var email = "test@example.com";
            var password = "testpassword";
            var expectedToken = "testToken";

            var jsonResponse = JsonConvert.SerializeObject(new FirebaseService.FirebaseAuthResponse
            {
                idToken = expectedToken
            });

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            // Act
            var token = await _firebaseService.LoginWithEmailAndPassword(email, password);

            // Assert
            Assert.Equal(expectedToken, token);
        }

        [Fact]
        public async Task LoginWithEmailAndPassword_InvalidCredentials_ThrowsException()
        {
            // Arrange
            var email = "wrong@example.com";
            var password = "wrongpassword";

            var jsonResponse = "{\"error\": {\"message\": \"EMAIL_NOT_FOUND\"}}";
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Content = new StringContent(jsonResponse)
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _firebaseService.LoginWithEmailAndPassword(email, password));

            Assert.Equal("Não autenticado", exception.Message);
        }
    }
}
