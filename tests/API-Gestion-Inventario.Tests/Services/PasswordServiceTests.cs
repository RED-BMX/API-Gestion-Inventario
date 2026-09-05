using API_Gestion_Inventario.Services;

namespace API_Gestion_Inventario.Tests.Services;

public class PasswordServiceTests
{
    [Fact]
    public void HashPassword_ShouldCreateValidHash()
    {
        // Arrange
        var service = new PasswordService();
        var password = "Prueba123";

        // Act
        var hash = service.HashPassword(password);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(hash));
        Assert.NotEqual(password, hash);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnTrue_WhenPasswordIsCorrect()
    {
        // Arrange
        var service = new PasswordService();
        var password = "Prueba123";
        var hash = service.HashPassword(password);

        // Act
        var result = service.VerifyPassword(password, hash);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalse_WhenPasswordIsIncorrect()
    {
        // Arrange
        var service = new PasswordService();
        var password = "Prueba123";
        var wrongPassword = "PasswordIncorrecta";
        var hash = service.HashPassword(password);

        // Act
        var result = service.VerifyPassword(wrongPassword, hash);

        // Assert
        Assert.False(result);
    }
}