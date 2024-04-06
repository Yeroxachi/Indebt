using Application.DTOs;
using Application.Mappers;
using Application.Responses;
using Application.Services;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Microsoft.Extensions.Options;
using Tests.Helpers;
using Xunit;

namespace Tests.UnitTests;

public class AuthServiceTests
{
    private readonly IMapper _mapper;
    private readonly IOptions<AuthOptions> _options;
    private readonly IOptions<EnvironmentOptions> _envOptions;
    private readonly IEmailService _emailService;

    public AuthServiceTests()
    {
        _mapper = TestHelper.GetMapper(new UserMapperProfile());
        _options = TestHelper.CreateAuthOptions();
        _envOptions = TestHelper.CreateEnvironmentOptions();
        _emailService = TestHelper.CreateEmailService();;
    }

    [Fact]
    public async void Registration_ReturnsCreatedStatusCode_WhenValidRegistrationDtoProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Created;
        using var context = new TestDbContextBuilder(nameof(AuthServiceTests))
            .GetContext();
        var sut = new AuthService(context,_mapper,null,_envOptions, _emailService,_options);
        var dto = new RegistrationDto()
        {
            Username = "username1",
            Email = "email112@test.com",
            Password = "password",
            Name = "name",
            Surname = "surname"
        };

        // Act
        var response = await sut.RegistrationAsync(dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Authenticate_ReturnsOkStatusCode_WhenValidLoginDtoProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        await using var context = new TestDbContextBuilder(nameof(AuthServiceTests)).WithUsers()
            .GetContext();
        var sut = new AuthService(context, _mapper,  null, _envOptions, _emailService, _options);
        var dto = new LoginDto
        {
            Username = "username",
            Password = "password"
        };

        // Act
        var response = await sut.AuthenticateAsync(dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Authenticate_ReturnsBadRequestStatusCode_WhenInValidLoginDtoProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.BadRequest;
        using var context = new TestDbContextBuilder(nameof(AuthServiceTests)).WithUsers()
            .GetContext();
        var sut = new AuthService(context, _mapper, null, _envOptions, _emailService, _options);
        var loginDto = new LoginDto()
        {
            Username = "invalidusername",
            Password = "password"
        };

        // Act
        var response = await sut.AuthenticateAsync(loginDto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Authenticate_ReturnsBadRequestStatusCode_WhenUserNotConfirmedDtoProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.BadRequest;
        using var context = new TestDbContextBuilder(nameof(AuthServiceTests)).WithUsers()
            .GetContext();
        var sut = new AuthService(context, _mapper, null, _envOptions, _emailService,_options);
        var loginDto = new LoginDto()
        {
            Username = $"{TestDataConstants.NotConfirmedUsername}",
            Password = "password"
        };

        // Act
        var response = await sut.AuthenticateAsync(loginDto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void ConfirmUserAccount_ReturnOkResponseStatus_WhenAllDataIsProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        using var context = new TestDbContextBuilder(nameof(AuthServiceTests))
            .WithUsers()
            .WithConfirmationCodes()
            .GetContext();
        var sut = new AuthService(context, _mapper,null, _envOptions, _emailService, _options);
        var emailConfirmationDto = new EmailConfirmationDto
        {
            Email = TestDataConstants.DefaultEmail,
            Code = TestDataConstants.CorrectConfirmationCode
        };

        // Act
        var response = await sut.ConfirmAccountAsync(emailConfirmationDto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Theory]
    [InlineData(TestDataConstants.DefaultEmail, TestDataConstants.IncorrectConfirmationCode)]
    [InlineData(TestDataConstants.IncorrectEmail, TestDataConstants.CorrectConfirmationCode)]
    public async void ConfirmUserAccount_ReturnsNotFoundStatusCode_WhenInvalidDataProvided(string inputEmail,
        string inputConfirmationCode)
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        using var context = new TestDbContextBuilder(nameof(AuthServiceTests))
            .WithUsers()
            .WithConfirmationCodes()
            .GetContext();
        var sut = new AuthService(context, _mapper, null, _envOptions, _emailService, _options);
        var emailConfirmationDto = new EmailConfirmationDto
        {
            Email = inputEmail,
            Code = inputConfirmationCode
        };

        // Act
        var response = await sut.ConfirmAccountAsync(emailConfirmationDto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async Task RefreshTokenAsync_GivenCorrectRefreshToken_ResultShouldBeOkResponseCode()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        await using var context = new TestDbContextBuilder(nameof(AuthServiceTests)).WithUsers()
            .GetContext();
        var sut = new AuthService(context, _mapper,  null, _envOptions, _emailService, _options);
        var dto = new LoginDto
        {
            Username = "username",
            Password = "password"
        };

        // Act
        var response = await sut.AuthenticateAsync(dto);
        var refreshToken = response as BaseResponse<TokenResponse>;
        var newResponse = await sut.RefreshTokenAsync(refreshToken!.Data.RefreshToken);

        // Assert
        Assert.Equal(expectedCode, newResponse.Code);
    }

    [Fact]
    public async Task ResendEmailAsync_Returns_ReturnOkResponseCode_WhenCorrectEmailProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        await using var context = new TestDbContextBuilder(nameof(AuthServiceTests)).WithUsers()
            .GetContext();
        var sut = new AuthService(context, _mapper, null, _envOptions, _emailService, _options);
        const string email = TestDataConstants.NotConfirmedEmail;

        // Act
        var response = await sut.ResendEmailAsync(email);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
}