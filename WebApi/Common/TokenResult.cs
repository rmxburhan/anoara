namespace WebApi.Common;

public record TokenResult(string Token, DateTime ExpiredAt);