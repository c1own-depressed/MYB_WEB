using Application.Utils;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult HandleServiceResult(this Controller controller, ServiceResult serviceResult)
        {
            if (serviceResult.Success)
            {
                return controller.Ok();
            }

            var errorMessages = string.Join("; ", serviceResult.Errors);
            return controller.BadRequest(errorMessages);
        }

    }
}