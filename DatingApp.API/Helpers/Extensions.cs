using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
    // Having the class as static means that we do not need to create a new instance of the class when we need to use on of its methods
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message) {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}