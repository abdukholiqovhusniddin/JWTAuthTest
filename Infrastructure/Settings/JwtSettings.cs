namespace Infrastructure.Settings;
public class JwtSettings
{
    public string SecretKey { get; init; } = null!;
    public int AccessTokenMinutes { get; init; }
    public int RefreshTokenDays { get; init; }
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
}
