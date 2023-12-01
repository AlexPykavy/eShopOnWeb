using System;

namespace Microsoft.eShopWeb.ApplicationCore.Constants;

public class AuthorizationConstants
{
    public static readonly string DEFAULT_PASSWORD = Environment.GetEnvironmentVariable("DEFAULT_PASSWORD");

    public static readonly string JWT_SECRET_KEY = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
}
