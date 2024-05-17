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

        public static IActionResult HandleModelStateErrors(this Controller controller)
        {
            var errorMessages = controller.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return controller.BadRequest(new { Errors = errorMessages });
        }

        public static IActionResult HandleErrorMessages(this Controller controller, IEnumerable<string> errorMessages)
        {
            return controller.BadRequest(new { Errors = errorMessages });
        }

        public static IActionResult HandleErrorMessage(this Controller controller, string errorMessage)
        {
            return controller.BadRequest(new { Errors = new List<string> { errorMessage } });
        }
    }
}