namespace TS.Shared.Jwt
{
    public interface IJwtRefreshUtil
    {
        public JwtRefreshToken CreateRefreshToken(long userId);

        public Task<bool> SaveAsync(JwtRefreshToken refreshToken);

        public Task<bool> RemoveAsync(string token);

        public Task<bool> RefreshAsync(string token);
    }
}
