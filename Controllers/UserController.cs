using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitTrack.Data;
using FitTrack.Utils;
using FitTrack.Models.Resources;
using FitTrack.Models;

namespace FitTrack.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly Util _util;
    public UserController(IUserService userService, Util util)
    {
        _userService = userService;
        _util = util;
    }

    [Authorize]
    public IActionResult Index()
    {

        var userIdClaim = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userIdClaim)) return Json(new { success = false, message = "Usuário não autenticado" });

        int userId = int.Parse(userIdClaim);

        var userResult = _userService.GetUserById(userId);

        if(!userResult.success) return Json(new { success = false, message = userResult.message });

        userResult.data.profilePic = Url.Content(userResult.data.profilePic);
        return View(userResult.data);
    }
    
    [Authorize]
    [HttpPost]
    public JsonResult SaveProfileInfo([FromBody] UsersDTO ProfileInfo)
    {

        #region validacoes
        
        if(string.IsNullOrEmpty(ProfileInfo.name)) return Json(new {success = false, message = "Nome inválido"});
        if(string.IsNullOrEmpty(ProfileInfo.email)) return Json(new {success = false, message = "Email inválido"});
        
        #endregion

        var userIdClaim = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return Json(new { success = false, message = "Usuário não autenticado" });

        int userId = int.Parse(userIdClaim);

        var saveProfileResult = _userService.SaveProfileInfo(userId, ProfileInfo);
        
        if(!saveProfileResult.success) return Json(new { success = false, message = saveProfileResult.message});

        return Json(new { success = true, message = saveProfileResult.message });
    }

    [Authorize]
    [HttpPost]
    public async Task<JsonResult> UploadProfilePicture(IFormFile profilePic)
    {
        if (profilePic == null || profilePic.Length == 0)
            return Json(new { success = false, message = "Imagem inválida" });

        var userIdClaim = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return Json(new { success = false, message = "Usuário não autenticado" });
        
        int userId = int.Parse(userIdClaim);

        var uploadProfilePicResult = await _userService.UploadProfilePictureAsync(userId, profilePic);

        if(!uploadProfilePicResult.success) return Json(new { success = false, message = uploadProfilePicResult.message});

        return Json(new
        {
            success = true,
            message = "Foto atualizada com sucesso",
        });
    }
}