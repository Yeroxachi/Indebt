using Application.Context;
using Application.DTOs;
using Application.Helpers;
using Application.Responses;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services;

public class AuthService : BaseService, IAuthService
{
    private readonly IOptions<EnvironmentOptions> _environmentOptions;
    private readonly IEmailService _emailService;
    private readonly IOptions<AuthOptions> _authOptions;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly byte[] _tokenKey;
    private readonly Random _random;
    private const int LenghtOfConfirmationCode = 6;
    private readonly TokenValidationParameters _validationParameters;

    private const string ConfirmationMessageText =
        "Good day! We are the InDebt team and welcome you to our ranks.\n To complete your registration, please enter the code below. Thank you and have a nice day!\n Code :";
    private const string EmailConfirmationSubject = "Email Confirmation";

    public AuthService(IInDebtContext context, IMapper mapper, IHttpContextAccessor accessor, IOptions<EnvironmentOptions> environmentOptions, IEmailService emailService, IOptions<AuthOptions> authOptions) : base(context, mapper, accessor)
    {
        _environmentOptions = environmentOptions;
        _emailService = emailService;
        _authOptions = authOptions;
        _tokenHandler = new JwtSecurityTokenHandler();
        _tokenKey = Encoding.UTF8.GetBytes(_authOptions.Value.Key);
        _random = new Random();
        _validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(_tokenKey),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    }

    public async Task<BaseResponse> AuthenticateAsync(LoginDto dto)
    {
        var passwordHash = UserHelper.ComputeSha256Hash(dto.Password);
        var user = await Context.Users
            .Include(x=>x.Groups)
            .FirstOrDefaultAsync(x => x.Username == dto.Username && x.PasswordHash == passwordHash);

        if (user is null)
        {
            return BadRequest("Username or password is incorrect");
        }

        if (!user.IsConfirmedEmail)
        {
            return BadRequest("Email address is not confirmed.");
        }

        var response = new TokenResponse
        {
            AccessToken = CreateAccessToken(user),
            RefreshToken = await CreateRefreshToken(user)
        };

        return Ok(response);
    }
    
    public async Task<BaseResponse> RefreshTokenAsync(string refreshToken)
    {
        var userId = GetUserIdFromToken(refreshToken);
        if (userId is null)
        {
            return BadRequest("InvalidToken");
        }
            
        var user = await Context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null)
        {
            return BadRequest("Invalid Token");
        }

        var currentToken = await Context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == refreshToken && x.UserId == userId && x.IsValid);

        if (currentToken is null)
        {
            return BadRequest("Invalid Token");
        }

        currentToken.IsValid = false;
        var newAccessToken = CreateAccessToken(user);
        var newRefreshToken = await CreateRefreshToken(user);
            
        var response = new TokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
        await Context.SaveChangesAsync();
            
        return Ok(response);
    }

    public async Task<BaseResponse> ResendEmailAsync(string email)
    {
        var user = await Context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
        {
            return NotFound();
        }

        if (user.IsConfirmedEmail)
        {
            return BadRequest("Email is already confirmed");
        }

        var confirmationCode = new ConfirmationCode
        {
            Type = ConfirmationType.EmailConfirmation,
            User = user,
            Code = CreateConfirmationCode(),
        };
        await Context.ConfirmationCodes.AddAsync(confirmationCode);

        var emailDto = new EmailDto
        {
            ReceiverEmail = user.Email,
            ReceiverName = user.Name,
            Subject = EmailConfirmationSubject,
            Message = $"{ConfirmationMessageText} {confirmationCode.Code}"
        };

        await _emailService.SendEmailAsync(emailDto);
        await Context.SaveChangesAsync();
        return Ok();
    }

    public async Task<BaseResponse> RegistrationAsync(RegistrationDto dto)
    {
        //Creating user and adding to Context
        var user = new User
        {
            Email = dto.Email,
            Name = dto.Name,
            Surname = dto.Surname,
            Username = dto.Username,
            IsConfirmedEmail = false
        };
        var existUser = await Context.Users.FirstOrDefaultAsync(x => x.Email == user.Email || x.Username == user.Username);
        if (existUser is not null)
        {
            return BadRequest("User with similar email or username exist in system");
        }

        var passwordHash = UserHelper.ComputeSha256Hash(dto.Password);
        user.PasswordHash = passwordHash;
        await Context.Users.AddAsync(user);
        //Creating confirmation andAdding to context
        var confirmationCode = new ConfirmationCode
        {
            Type = ConfirmationType.EmailConfirmation,
            User = user,
            Code = CreateConfirmationCode(),
        };
        
        await Context.ConfirmationCodes.AddAsync(confirmationCode);
        await Context.SaveChangesAsync();
        
        //Create a message to send to the mail for verification
        var emailDto = new EmailDto
        {
            ReceiverEmail = user.Email,
            ReceiverName = dto.Name,
            Subject = EmailConfirmationSubject,
            Message = $"{ConfirmationMessageText} {confirmationCode.Code}"
        };
        
        await _emailService.SendEmailAsync(emailDto);
        var response = Mapper.Map<UserResponse>(user);
        return Created(response);
    }
    
    public async Task<BaseResponse> ConfirmAccountAsync(EmailConfirmationDto dto)
    {
        var user = await Context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
        if (user == null)
        {
            return NotFound();
        }

        var confirmation = await Context.ConfirmationCodes
            .FirstOrDefaultAsync(x => x.Code == dto.Code && x.User.Id == user.Id && x.Type == ConfirmationType.EmailConfirmation);
        if (confirmation is null)
        {
            return NotFound();
        }
        
        user.IsConfirmedEmail = true;
        var confirmations = await Context.ConfirmationCodes
            .Where(x => x.User.Id == user.Id && x.Type == ConfirmationType.EmailConfirmation)
            .ToListAsync();
        Context.ConfirmationCodes.RemoveRange(confirmations);
        Context.Users.Update(user);

        var response = new TokenResponse
        {
            AccessToken = CreateAccessToken(user),
            RefreshToken = await CreateRefreshToken(user)
        };

        await Context.SaveChangesAsync();
        return Ok(response);
    }
    
    private string CreateAccessToken(User user)
    {
        var idsString = string.Join(",", user.Groups.Select(x => x.GroupId));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new (ClaimTypes.Name, user.Username),
                new (Constants.UserIdClaimName, user.Id.ToString()),
                new (Constants.UserGroupsIdsClaimName, idsString)
            }),
            Expires = DateTime.UtcNow.AddSeconds(_authOptions.Value.AccessTokenLifeTime),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_tokenKey), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }
    
    private async Task<string> CreateRefreshToken(User user)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new (ClaimTypes.Name, user.Username),
                new (Constants.UserIdClaimName, user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddSeconds(_authOptions.Value.RefreshTokenLifeTime),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_tokenKey), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        var refreshToken = _tokenHandler.WriteToken(token);

        var oldTokens = await Context.RefreshTokens
            .Where(x => x.UserId == user.Id && x.IsValid)
            .ToListAsync();
        foreach (var oldToken in oldTokens)
        {
            oldToken.IsValid = false;
        }

        await Context.RefreshTokens.AddAsync(new RefreshToken
        {
            Token = refreshToken,
            User = user,
            IsValid = true
        });
        await Context.SaveChangesAsync();
        return refreshToken;
    }
    
    private string CreateConfirmationCode()
    {
        var result = new StringBuilder();
        for (var i = 0; i < LenghtOfConfirmationCode; i++)
        {
            result.Append(_random.Next(0, 9));
        }
        return result.ToString();
    }
    
    private string GenerateRedirectLinkForConfirmation(string email, string code)
    {
        var redirectUrl = $"{_environmentOptions.Value.HostUrl}/api/Auths/ConfirmEmail/{email}/{code}";
        var link = $"<a href='{redirectUrl}'>Click here</a>";
        return link;
    }

    private Guid? GetUserIdFromToken(string token)
    {
        try
        {
            var principal = _tokenHandler.ValidateToken(token, _validationParameters, out var validatedToken);
            if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            var userIdClaim = principal.FindFirst(Constants.UserIdClaimName);
            if (userIdClaim is null)
            {
                return null;
            }
                    
            return Guid.Parse(userIdClaim.Value);
        }
        catch (SecurityTokenSignatureKeyNotFoundException e)
        {
            return null;
        }
    }
}
