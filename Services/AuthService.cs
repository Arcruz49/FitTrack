using FitTrack.Data;
using FitTrack.Models;
using FitTrack.Models.Resources;
using Microsoft.AspNetCore.Identity;

public class AuthService : IAuthService
{
    private readonly Context _db;
    private readonly PasswordHasher<Users> _passwordHasher;

    public AuthService(Context db)
    {
        _db = db;
        _passwordHasher = new PasswordHasher<Users>();
    }

    public RetornoGenerico<Users> Authenticate(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
            return new RetornoGenerico<Users>
            {
                success = false,
                message = "Email inválido"
            };

        if (string.IsNullOrWhiteSpace(password))
            return new RetornoGenerico<Users>
            {
                success = false,
                message = "Senha inválida"
            };


        var user = _db.Users.FirstOrDefault(u => u.email == email);
        if (user == null)
            return new RetornoGenerico<Users>
            {
                success = false,
                message = "Usuário não encontrado"
            };

        var result = _passwordHasher.VerifyHashedPassword(
            user, user.password, password);

        if (result != PasswordVerificationResult.Success)
            return new RetornoGenerico<Users>
            {
                success = false,
                message = "Senha incorreta"
            };

        return new RetornoGenerico<Users>
        {
            success = true,
            message = "Login realizado com sucesso",
            data = user
        };
    }


    public RetornoGenerico<Profiles> GetProfile(int profileId)
    {
        var profile = _db.Profiles.FirstOrDefault(p => p.id == profileId);

        if (profile == null)
            return new RetornoGenerico<Profiles>
            {
                success = false,
                message = "Perfil não encontrado"
            };

        return new RetornoGenerico<Profiles>
        {
            success = true,
            data = profile
        };  
    }

    public RetornoGenerico<Users> Register(string name, string email, string password, string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new RetornoGenerico<Users>
            {
                success = false,
                message = "Nome inválido"
            };

        if (string.IsNullOrWhiteSpace(email))
            return new RetornoGenerico<Users>
            {
                success = false,
                message = "Email inválido"
            };

        if (string.IsNullOrWhiteSpace(password))
            return new RetornoGenerico<Users>
            {
                success = false,
                message = "Senha inválida"
            };

        if (!confirmPassword.Equals(password))
            return new RetornoGenerico<Users>
            {
                success = false,
                message = "As senhas não coincidem"
            };

        if (_db.Users.Any(u => u.email == email))
            throw new Exception("Email já registrado");

        var user = new Users
        {
            name = name,
            email = email,
            profileId = 2
        };

        user.password = _passwordHasher.HashPassword(user, password);

        _db.Users.Add(user);
        _db.SaveChanges();

        return new RetornoGenerico<Users>
        {
            success = true,
            message = "Login realizado com sucesso",
            data = user
        };
    }


    
}
