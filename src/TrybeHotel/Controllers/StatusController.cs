using Microsoft.AspNetCore.Mvc;


namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("/")]
    public class StatusController : Controller
    {
        return OK(new {message = "online"});
    }
}
