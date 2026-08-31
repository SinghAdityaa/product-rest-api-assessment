using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.Data;

namespace Infrastructure.Identity;

public sealed class AuthService(ApplicationDbContext db, IOptions<JwtOptions> options, IPasswordHasher<AppUser> hasher) : IAuthService
{
    private readonly JwtOptions _options = options.Value;

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await db.Users.Include(x => x.RefreshTokens).SingleOrDefaultAsync(x => x.Username == request.Username, ct);
        if (user is null || hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            return null;

        return await IssueTokensAsync(user, null, ct);
    }

    public async Task<AuthResponse?> RefreshAsync(RefreshRequest request, CancellationToken ct)
    {
        var token = await db.RefreshTokens.Include(x => x.AppUser).SingleOrDefaultAsync(x => x.Token == request.RefreshToken, ct);
        if (token is null || !token.IsActive) return null;
        return await IssueTokensAsync(token.AppUser, token, ct);
    }

    private async Task<AuthResponse> IssueTokensAsync(AppUser user, RefreshToken? previous, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_options.AccessTokenMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var jwt = new JwtSecurityToken(_options.Issuer, _options.Audience, claims, now, expires, new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
        var refreshValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refresh = new RefreshToken { Token = refreshValue, AppUserId = user.Id, CreatedOn = now, ExpiresOn = now.AddDays(_options.RefreshTokenDays) };

        if (previous is not null)
        {
            previous.RevokedOn = now;
            previous.ReplacedByToken = refreshValue;
        }

        db.RefreshTokens.Add(refresh);
        await db.SaveChangesAsync(ct);
        return new AuthResponse(accessToken, refreshValue, expires);
    }
}
