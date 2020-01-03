using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityServer4;

namespace BankingService.IdentityServer
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }


        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "Kajol",
                    Password = "password"
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "Jeetu",
                    Password = "password"
                }
            };
        }



        public static IEnumerable<ApiResource> GetAllApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("bankingService", "Customer Api for bankingService")
            };
        }


        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                //Client Credentials Password Grant Types
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "bankingService" }
                },

                //Resource Owner Password Grant Types
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "bankingService" }
                },

                //Implicit Flow Grant Type
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.Implicit,

                    RedirectUris = {"http://localhost:5002/signin-oidc"},
                    PostLogoutRedirectUris = {"http://localhost:5002/signout-callback-oidc"},
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },

                //SwaggerClient
                new Client
                {
                    ClientId = "swaggerapiui",
                    ClientName = "Swagger API UI",
                    AllowedGrantTypes = GrantTypes.Implicit,

                    RedirectUris = {"http://localhost:51466/swagger/oauth2-redirect.html"},
                    PostLogoutRedirectUris = {"http://localhost:51466/swagger"},

                    AllowedScopes = { "bankingService" },
                    AllowAccessTokensViaBrowser = true   
                    
                }
            };
        }
    }
}
