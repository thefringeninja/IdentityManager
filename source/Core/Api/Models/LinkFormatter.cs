using IdentityManager.Extensions;

namespace IdentityManager.Api.Models
{
    internal static class LinkFormatter
    {
        public static string Users() 
            => Constants.UserRoutePrefix;

        public static string User(string subject) 
            => $"{Constants.UserRoutePrefix}/{subject}";
        
        public static string UserProperty(string subject, string type) 
            => $"{Constants.UserRoutePrefix}/{subject}/properties/{type.ToBase64UrlEncoded()}";

        public static string UserRole(string subject, string role) 
            => $"{Constants.UserRoutePrefix}/{subject}/roles/{role.ToBase64UrlEncoded()}";

        public static string UserClaims(string subject) 
            => $"{Constants.UserRoutePrefix}/{subject}/claims";

        public static string UserClaim(string subject, string type, string value) 
            => $"{Constants.UserRoutePrefix}/{subject}/claims/{type.ToBase64UrlEncoded()}/{value.ToBase64UrlEncoded()}";

        public static string Roles()
            => Constants.RoleRoutePrefix;

        public static string Role(string subject) 
            => $"{Constants.RoleRoutePrefix}/{subject}";

        public static string RoleProperty(string subject, string type) 
            => $"{Constants.RoleRoutePrefix}/{subject}/properties/{type.ToBase64UrlEncoded()}";
    }
}