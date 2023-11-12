
using Microsoft.AspNetCore.Mvc;


namespace BitByByte.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HandleErrorCode(int statusCode)
        {
            // You can perform additional error handling if needed
            Response.StatusCode = statusCode;
            return View("404"); // Render the custom 404 page
        }

        [Route("Error")]
        public IActionResult Error()
        {
            // Handle other errors here
            return View("Error"); // Render a general error page
        }
    }
}
