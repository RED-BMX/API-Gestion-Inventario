using API_Gestion_Inventario.Data;
using API_Gestion_Inventario.DTOs.Auth;
using API_Gestion_Inventario.Models;
using API_Gestion_Inventario.Services;
using API_Gestion_Inventario.Services.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace API_Gestion_Inventario.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_ShouldRegisterUser_WhenDataIsValid()
    {
        // Arrange
        await using var connection = CreateConnection();
        await using var context = CreateContext(connection);

        context.Roles.Add(new Role
        {
            Id = 1,
            Name = "User"
        });

        await context.SaveChangesAsync();

        var request = new RegisterRequest
        {
            Username = "nuevo_usuario",
            Email = "nuevo@inventario.com",
            Password = "Password123"
        };

        var passwordService = new Mock<IPasswordService>();
        var jwtService = new Mock<IJwtService>();

        passwordService
            .Setup(service => service.HashPassword(request.Password))
            .Returns("HASH_GENERADO");

        jwtService
            .Setup(service => service.GenerateToken(It.IsAny<User>()))
            .Returns("TOKEN_GENERADO");

        var service = new AuthService(
            context,
            passwordService.Object,
            jwtService.Object);

        // Act
        var result = await service.RegisterAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TOKEN_GENERADO", result.Token);
        Assert.Equal("nuevo_usuario", result.Username);
        Assert.Equal("nuevo@inventario.com", result.Email);
        Assert.Equal("User", result.Role);
        Assert.True(result.UserId > 0);

        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Username == "nuevo_usuario");

        Assert.NotNull(user);
        Assert.Equal("HASH_GENERADO", user.PasswordHash);
        Assert.Equal(1, user.RoleId);

        passwordService.Verify(
            service => service.HashPassword(request.Password),
            Times.Once);

        jwtService.Verify(
            service => service.GenerateToken(
                It.Is<User>(u =>
                    u.Username == "nuevo_usuario" &&
                    u.RoleId == 1)),
            Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrow_WhenUsernameAlreadyExists()
    {
        // Arrange
        await using var connection = CreateConnection();
        await using var context = CreateContext(connection);

        context.Roles.Add(new Role
        {
            Id = 1,
            Name = "User"
        });

        context.Users.Add(new User
        {
            Id = 1,
            Username = "usuario_existente",
            Email = "existente@inventario.com",
            PasswordHash = "HASH",
            RoleId = 1
        });

        await context.SaveChangesAsync();

        var request = new RegisterRequest
        {
            Username = "usuario_existente",
            Email = "nuevo@inventario.com",
            Password = "Password123"
        };

        var passwordService = new Mock<IPasswordService>();
        var jwtService = new Mock<IJwtService>();

        var service = new AuthService(
            context,
            passwordService.Object,
            jwtService.Object);

        // Act
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.RegisterAsync(request));

        // Assert
        Assert.Equal(
            "El nombre de usuario ya está registrado.",
            exception.Message);

        passwordService.Verify(
            service => service.HashPassword(It.IsAny<string>()),
            Times.Never);

        jwtService.Verify(
            service => service.GenerateToken(It.IsAny<User>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrow_WhenEmailAlreadyExists()
    {
        // Arrange
        await using var connection = CreateConnection();
        await using var context = CreateContext(connection);

        context.Roles.Add(new Role
        {
            Id = 1,
            Name = "User"
        });

        context.Users.Add(new User
        {
            Id = 1,
            Username = "usuario_existente",
            Email = "existente@inventario.com",
            PasswordHash = "HASH",
            RoleId = 1
        });

        await context.SaveChangesAsync();

        var request = new RegisterRequest
        {
            Username = "nuevo_usuario",
            Email = "existente@inventario.com",
            Password = "Password123"
        };

        var passwordService = new Mock<IPasswordService>();
        var jwtService = new Mock<IJwtService>();

        var service = new AuthService(
            context,
            passwordService.Object,
            jwtService.Object);

        // Act
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.RegisterAsync(request));

        // Assert
        Assert.Equal(
            "El correo electrónico ya está registrado.",
            exception.Message);

        passwordService.Verify(
            service => service.HashPassword(It.IsAny<string>()),
            Times.Never);

        jwtService.Verify(
            service => service.GenerateToken(It.IsAny<User>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrow_WhenUserRoleDoesNotExist()
    {
        // Arrange
        await using var connection = CreateConnection();
        await using var context = CreateContext(connection);

        var request = new RegisterRequest
        {
            Username = "nuevo_usuario",
            Email = "nuevo@inventario.com",
            Password = "Password123"
        };

        var passwordService = new Mock<IPasswordService>();
        var jwtService = new Mock<IJwtService>();

        var service = new AuthService(
            context,
            passwordService.Object,
            jwtService.Object);

        // Act
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.RegisterAsync(request));

        // Assert
        Assert.Equal(
            "El rol User no está configurado.",
            exception.Message);

        passwordService.Verify(
            service => service.HashPassword(It.IsAny<string>()),
            Times.Never);

        jwtService.Verify(
            service => service.GenerateToken(It.IsAny<User>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnAuthResponse_WhenCredentialsAreValid()
    {
        // Arrange
        await using var connection = CreateConnection();
        await using var context = CreateContext(connection);

        var role = new Role
        {
            Id = 1,
            Name = "User"
        };

        var user = new User
        {
            Id = 1,
            Username = "usuario",
            Email = "usuario@inventario.com",
            PasswordHash = "HASH_GUARDADO",
            RoleId = 1,
            Role = role
        };

        context.Roles.Add(role);
        context.Users.Add(user);

        await context.SaveChangesAsync();

        var request = new LoginRequest
        {
            Username = "usuario",
            Password = "Password123"
        };

        var passwordService = new Mock<IPasswordService>();
        var jwtService = new Mock<IJwtService>();

        passwordService
            .Setup(service => service.VerifyPassword(
                request.Password,
                "HASH_GUARDADO"))
            .Returns(true);

        jwtService
            .Setup(service => service.GenerateToken(
                It.IsAny<User>()))
            .Returns("TOKEN_LOGIN");

        var service = new AuthService(
            context,
            passwordService.Object,
            jwtService.Object);

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TOKEN_LOGIN", result.Token);
        Assert.Equal(1, result.UserId);
        Assert.Equal("usuario", result.Username);
        Assert.Equal("usuario@inventario.com", result.Email);
        Assert.Equal("User", result.Role);

        passwordService.Verify(
            service => service.VerifyPassword(
                request.Password,
                "HASH_GUARDADO"),
            Times.Once);

        jwtService.Verify(
            service => service.GenerateToken(
                It.Is<User>(u =>
                    u.Username == "usuario")),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenUserDoesNotExist()
    {
        // Arrange
        await using var connection = CreateConnection();
        await using var context = CreateContext(connection);

        var request = new LoginRequest
        {
            Username = "usuario_inexistente",
            Password = "Password123"
        };

        var passwordService = new Mock<IPasswordService>();
        var jwtService = new Mock<IJwtService>();

        var service = new AuthService(
            context,
            passwordService.Object,
            jwtService.Object);

        // Act
        var exception =
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => service.LoginAsync(request));

        // Assert
        Assert.Equal(
            "Credenciales inválidas.",
            exception.Message);

        passwordService.Verify(
            service => service.VerifyPassword(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        jwtService.Verify(
            service => service.GenerateToken(
                It.IsAny<User>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenPasswordIsIncorrect()
    {
        // Arrange
        await using var connection = CreateConnection();
        await using var context = CreateContext(connection);

        var role = new Role
        {
            Id = 1,
            Name = "User"
        };

        var user = new User
        {
            Id = 1,
            Username = "usuario",
            Email = "usuario@inventario.com",
            PasswordHash = "HASH_GUARDADO",
            RoleId = 1
        };

        context.Roles.Add(role);
        context.Users.Add(user);

        await context.SaveChangesAsync();

        var request = new LoginRequest
        {
            Username = "usuario",
            Password = "PasswordIncorrecta"
        };

        var passwordService = new Mock<IPasswordService>();
        var jwtService = new Mock<IJwtService>();

        passwordService
            .Setup(service => service.VerifyPassword(
                request.Password,
                "HASH_GUARDADO"))
            .Returns(false);

        var service = new AuthService(
            context,
            passwordService.Object,
            jwtService.Object);

        // Act
        var exception =
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => service.LoginAsync(request));

        // Assert
        Assert.Equal(
            "Credenciales inválidas.",
            exception.Message);

        jwtService.Verify(
            service => service.GenerateToken(
                It.IsAny<User>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldAlwaysAssignUserRole()
    {
        // Arrange
        await using var connection = CreateConnection();
        await using var context = CreateContext(connection);

        context.Roles.AddRange(
            new Role
            {
                Id = 1,
                Name = "Admin"
            },
            new Role
            {
                Id = 2,
                Name = "User"
            });

        await context.SaveChangesAsync();

        var request = new RegisterRequest
        {
            Username = "usuario_nuevo",
            Email = "usuario_nuevo@inventario.com",
            Password = "Password123"
        };

        var passwordService = new Mock<IPasswordService>();
        var jwtService = new Mock<IJwtService>();

        passwordService
            .Setup(service => service.HashPassword(
                request.Password))
            .Returns("HASH");

        jwtService
            .Setup(service => service.GenerateToken(
                It.IsAny<User>()))
            .Returns("TOKEN");

        var service = new AuthService(
            context,
            passwordService.Object,
            jwtService.Object);

        // Act
        var result = await service.RegisterAsync(request);

        // Assert
        Assert.Equal("User", result.Role);

        var user = await context.Users
            .FirstAsync(u => u.Username == "usuario_nuevo");

        Assert.Equal(2, user.RoleId);
    }

    private static SqliteConnection CreateConnection()
    {
        var connection = new SqliteConnection(
            "DataSource=:memory:");

        connection.Open();

        return connection;
    }

    private static ApplicationDbContext CreateContext(
        SqliteConnection connection)
    {
        var options =
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;

        var context = new ApplicationDbContext(options);

        context.Database.EnsureCreated();

        return context;
    }
}