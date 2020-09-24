using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Forge;
using DesignIntentApi.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DesignIntentApi.Controllers.Forge
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgeAuthController : ControllerBase
    {
        // As both internal & public tokens are used for all visitors
        // we don't need to request a new token on every request, so let's
        // cache them using static variables. Note we still need to refresh
        // them after the expires_in time (in seconds)
        private static dynamic InternalToken { get; set; }
        private static dynamic PublicToken { get; set; }

        /// <summary>
        /// Get access token with public (viewables:read) scope
        /// </summary>
        [HttpGet]
        [Route("oauth/token")]
        public async Task<ForgeTokenDto> GetPublicAsync()
        {
            if (PublicToken == null || PublicToken.ExpiresAt < DateTime.UtcNow)
            {
                PublicToken = await Get2LeggedTokenAsync(new Scope[] { Scope.ViewablesRead });
                PublicToken.ExpiresAt = DateTime.UtcNow.AddSeconds(PublicToken.expires_in);
            }


            return new ForgeTokenDto
            {
                AccessToken = PublicToken["access_token"],
                Expires = PublicToken["ExpiresAt"]
            };
        }

        /// <summary>
        /// Get access token with internal (write) scope
        /// </summary>
        public static async Task<dynamic> GetInternalAsync()
        {
            if (InternalToken == null || InternalToken.ExpiresAt < DateTime.UtcNow)
            {
                InternalToken = await Get2LeggedTokenAsync(new Scope[] { Scope.BucketCreate, Scope.BucketRead, Scope.BucketDelete, Scope.DataRead, Scope.DataWrite, Scope.DataCreate, Scope.CodeAll });
                InternalToken.ExpiresAt = DateTime.UtcNow.AddSeconds(InternalToken.expires_in);
            }

            return InternalToken;
        }

        /// <summary>
        /// Get the access token from Autodesk
        /// </summary>
        private static async Task<dynamic> Get2LeggedTokenAsync(Scope[] scopes)
        {
            TwoLeggedApi oauth = new TwoLeggedApi();
            string grantType = "client_credentials";
            dynamic bearer = await oauth.AuthenticateAsync(
              GetAppSetting("FORGE_CLIENT_ID"),
              GetAppSetting("FORGE_CLIENT_SECRET"),
              grantType,
              scopes);
            return bearer;
        }

        /// <summary>
        /// Reads appsettings from web.config
        /// </summary>
        public static string GetAppSetting(string settingKey)
        {
            return Environment.GetEnvironmentVariable(settingKey).Trim();
        }
    }
}
