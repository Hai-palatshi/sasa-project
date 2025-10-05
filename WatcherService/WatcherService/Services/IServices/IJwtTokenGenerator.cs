namespace WatcherService.Services.IServices
{
    public interface IJwtTokenGenerator
    {
        string CreateToken(string iss, int exp, string sharedSecret);
    }
}
