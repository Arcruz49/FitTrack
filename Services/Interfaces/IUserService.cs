using FitTrack.Models;
using FitTrack.Models.Resources;

namespace FitTrack.Services.Interfaces;
public interface IUserService
{
    RetornoGenerico<UsersDTO> GetUserById(int id);
    Retorno SaveProfileInfo(int userId, UsersDTO ProfileInfo);
    Task<Retorno> UploadProfilePictureAsync(int userId, IFormFile profilePic);
 }
