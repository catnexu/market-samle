using System.Security.Claims;
using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authorization;

namespace JavaScriptClient;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
      
        builder.Services
            .AddBff()
            .AddRemoteApis();

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
                options.DefaultSignOutScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = "https://localhost:5001";
                options.ClientId = "bff";
                options.ClientSecret = "secret";
                options.ResponseType = "code";
                options.Scope.Add("api1");
                options.Scope.Add("offline_access");
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.MapInboundClaims = false;
            });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();

        app.UseBff();

        app.UseAuthorization();
        app.MapBffManagementEndpoints();

        // Uncomment this for Controller support
        // app.MapControllers()
        //     .AsBffApiEndpoint();

        app.MapGet("/local/identity", LocalIdentityHandler)
            .AsBffApiEndpoint();

        app.MapRemoteBffApiEndpoint("/remote", "https://localhost:6001")
            .RequireAccessToken(Duende.Bff.TokenType.User);
        
        app.Run();
    }
    
    [Authorize]
    private static IResult LocalIdentityHandler(ClaimsPrincipal user)
    {
        var name = user.FindFirst("name")?.Value ?? user.FindFirst("sub")?.Value;
        return Results.Json(new { message = "Local API Success!", user = name });
    }
}