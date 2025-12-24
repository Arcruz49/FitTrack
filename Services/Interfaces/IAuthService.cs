using FitTrack.Models;
using FitTrack.Models.Resources;

public interface IAuthService
{
    RetornoGenerico<Users> Authenticate(string email, string password);
    RetornoGenerico<Users> Register(string name, string email, string password, string confirmPassword);
    RetornoGenerico<Profiles> GetProfile(int profileId = 0);
}
