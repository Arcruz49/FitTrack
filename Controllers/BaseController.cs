using Microsoft.AspNetCore.Mvc;
public abstract class BaseController : Controller
{
    protected int UserId
    {
        get
        {
            var claim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(claim))
                throw new UnauthorizedAccessException();

            return int.Parse(claim);
        }
    }
}
