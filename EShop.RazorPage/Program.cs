﻿using EShop.RazorPage.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.RegisterApiServices();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();

//For Using Redis Instead Caching in Memory (both are Distributed caching)
//Just Install Microsoft.Extensions.Caching.Redis package From Nuget
//and used below Config instead above (Other settings should not be affected)
//builder.Services.AddDistributedRedisCache(option => option.Configuration="localhost:6379");
//********** Or Better Used Below Setup **************
//First install package Microsoft.Extensions.Caching.StackExchangeRedis then use below config
//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = "localhost:6379";
//});

builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("Account", builder =>
    {
        builder.RequireAuthenticatedUser();
    });

    option.AddPolicy("SellerPanel", builder =>
    {
        builder.RequireAuthenticatedUser();
        builder.RequireAssertion(f => f.User.Claims
            .Any(c => c.Type == ClaimTypes.Role && (c.Value.Contains("Seller") ||
                                                     c.Value.Contains("seller") ||
                                                     c.Value.Contains("فروشنده"))));
    });
});

builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.AuthorizeFolder("/Profile", "Account");
        options.Conventions.AuthorizeFolder("/SellerPanel", "SellerPanel");
    });

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidAudience = builder.Configuration["JwtConfig:Audience"],
        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:SignInKey"])),
        ValidateLifetime = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//Take Token From Cookie Request & Append It To Headers For Sending To The Server
app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["token"]?.ToString();
    if (string.IsNullOrWhiteSpace(token) == false)
    {
        context.Request.Headers.Append("Authorization", $"Bearer {token}");
    }
    await next();
});


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.Use(async (context, next) =>
{
    await next();
    var status = context.Response.StatusCode;
    if (status == 401)
    {
        var path = context.Request.Path;
        context.Response.Redirect($"/auth/login?redirectTo={path}");
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
