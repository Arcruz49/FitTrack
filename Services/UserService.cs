using FitTrack.Data;
using FitTrack.Models;
using FitTrack.Models.Resources;
using FitTrack.Utils;
public class UserService : IUserService
{

    private readonly Context _db;
    private readonly Util _util;

    public UserService(Context db, Util util)
    {
        _db = db;
        _util = util;
    }
    public RetornoGenerico<UsersDTO> GetUserById(int id)
    {


        var user = _db.Users.Where(a => a.id == id).Select(a => new UsersDTO
        {
            id = a.id,
            name = a.name,
            email = a.email,
            bio = a.bio,
            phoneNumber = _util.FormatPhoneNumber(a.phoneNumber ?? ""),
            profilePic = string.IsNullOrWhiteSpace(a.profilePic) ? "~/images/default-pf.png" : a.profilePic,
            creation_date = a.creation_date,
            creation_date_string = _util.FormatCreationDate(a.creation_date),
        }).FirstOrDefault();

        if(user == null) return new RetornoGenerico<UsersDTO>{success = false, message = "Usuário não identificado"};

        var metrics = _db.UserMetrics.Where(a => a.userId == id).FirstOrDefault();

        if(metrics != null)
        {
            user.weight = metrics.weight;
            user.height = metrics.height;
            user.bodyFat = metrics.bodyFat;
            user.armCircumference = metrics.armCircumference;
            user.chestCircumference = metrics.chestCircumference;
            user.waistCircumference = metrics.waistCircumference;
            user.legCircumference = metrics.legCircumference;
            user.weightGoal = metrics.weightGoal;
        }

        return new RetornoGenerico<UsersDTO>{success = true, message = "", data = user};
    }

    public Retorno SaveProfileInfo(int userId, UsersDTO ProfileInfo)
    {
        var user = _db.Users.Where(a => a.id == userId).FirstOrDefault();
        var userMetrics = _db.UserMetrics.Where(a => a.userId == userId).FirstOrDefault();

        if(user == null) return new Retorno{success = false, message = "Usuário não identificado"};

        user.name = ProfileInfo.name;
        user.email = ProfileInfo.email;
        user.bio = ProfileInfo.bio;
        user.phoneNumber = ProfileInfo.phoneNumber;

        // Metrics table --melhorar isso aqui depois
        if(userMetrics == null)
        {
            userMetrics = new UserMetrics()
            {
                userId = userId,
                weight = ProfileInfo.weight,
                height = ProfileInfo.height,
                bodyFat = ProfileInfo.bodyFat,
                armCircumference = ProfileInfo.armCircumference,
                chestCircumference = ProfileInfo.chestCircumference,
                waistCircumference = ProfileInfo.waistCircumference,
                legCircumference = ProfileInfo.legCircumference,
                weightGoal = ProfileInfo.weightGoal,
                workoutsGoal = ProfileInfo.workoutsGoal,
            };
            
            _db.UserMetrics.Add(userMetrics);
        }
        else
        {
            userMetrics.weight = ProfileInfo.weight;
            userMetrics.height = ProfileInfo.height;
            userMetrics.bodyFat = ProfileInfo.bodyFat;
            userMetrics.armCircumference = ProfileInfo.armCircumference;
            userMetrics.chestCircumference = ProfileInfo.chestCircumference;
            userMetrics.waistCircumference = ProfileInfo.waistCircumference;
            userMetrics.legCircumference = ProfileInfo.legCircumference;
            userMetrics.weightGoal = ProfileInfo.weightGoal;
            userMetrics.workoutsGoal = ProfileInfo.workoutsGoal;
        }

        try
        {
            _db.SaveChanges();
            return new Retorno{success = true, message = "Cadastro atualizado"};
        }
        catch(Exception ex)
        {
            return new Retorno{success = true, message = _util.ErrorMessage(ex)};
        }
    }

    public async Task<Retorno> UploadProfilePictureAsync(int userId, IFormFile profilePic)
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(profilePic.FileName)}";
        var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var path = Path.Combine(folder, fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await profilePic.CopyToAsync(stream);
        }

        var user = _db.Users.First(x => x.id == userId);

        if (!string.IsNullOrWhiteSpace(user.profilePic))
        {
            var oldFilePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                user.profilePic.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
            );

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
        }

        user.profilePic = "/uploads/" + fileName;

        try
        {
            _db.SaveChanges();
            return new Retorno { success = true, message = "Imagem atualizada" };
        }
        catch (Exception ex)
        {
            return new Retorno { success = false, message = _util.ErrorMessage(ex) };
        }
    }

}