using FitTrack.Models;
using FitTrack.Data;
using FitTrack.Models.Resources;
using System.Text.RegularExpressions;
using System.Security.Claims;
using System.Globalization;

namespace FitTrack.Utils;

public class Util
{

    public string ErrorMessage(Exception ex)
    {
        var message = $"Erro: {ex.Message}";
        if (ex.InnerException != null) message += $"\nInner Exception: {ex.InnerException}";
        if (ex.StackTrace != null) message += $"\nStack Trace:\n{ex.StackTrace}";

        return message;
    }


    // public RetornoGenerico<Usuario> CreateUserHelper(string user = "arthur.cruz", string fullname = "Arthur Cruz", string password = "123")
    // {
    //     try
    //     {

    //         var usuario = new Usuario { name = user, fullName = fullname };

    //         var passwordHasher = new PasswordHasher<Usuario>();
    //         usuario.password = passwordHasher.HashPassword(usuario, password);


    //         return new RetornoGenerico<Usuario> { success = true, message = "Usuário criado com hash corretamente.", data = usuario };
    //     }
    //     catch (Exception ex)
    //     {
    //         return new RetornoGenerico<Usuario> { success = false, message = this.ErrorMessage(ex) };
    //     }
    // }
    
    public int GetIdLoggedUser(ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

        if (userIdClaim != null)
        {
            int userId = int.Parse(userIdClaim);
            return userId;
        }
        return 0;
    }

    public string GetEmailLoggedUser(ClaimsPrincipal user)
    {
        var userEmail = user.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;

        return userEmail ?? "";
    }


    public string FormatCreationDate(DateTime creationDate)
    {
        var culture = new CultureInfo("pt-BR");
        var text = creationDate.ToString("MMM yyyy", culture).Replace(".", "");

        return char.ToUpper(text[0]) + text.Substring(1);
    }

    public string FormatPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return phoneNumber;

        phoneNumber = Regex.Replace(phoneNumber, @"\D", "");

        if (phoneNumber.Length == 11)
        {
            return $"({phoneNumber[..2]}) {phoneNumber[2..7]}-{phoneNumber[7..]}";
        }

        if (phoneNumber.Length == 10)
        {
            return $"({phoneNumber[..2]}) {phoneNumber[2..6]}-{phoneNumber[6..]}";
        }

        return phoneNumber;
    }



}
