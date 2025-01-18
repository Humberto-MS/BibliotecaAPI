using BibliotecaAPI.Data;
using BibliotecaAPI.Filters;
using BibliotecaAPI.Middlewares;
using BibliotecaAPI.Services;
using BibliotecaAPI.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers ( opciones => opciones.Filters.Add ( typeof ( FiltroDeExcepcion ) ) )
    .AddJsonOptions ( opciones => opciones.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles )
    .AddNewtonsoftJson();

builder.Services.AddDbContext<ApplicationDbContext>(opciones => 
    opciones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddAuthentication ( JwtBearerDefaults.AuthenticationScheme )
    .AddJwtBearer ( opciones => opciones.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey ( Encoding.UTF8.GetBytes ( builder.Configuration [ "llavejwt" ]! ) ),
        ClockSkew = TimeSpan.Zero
    } );

builder.Services.AddAutoMapper ( typeof ( Program ) );

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization ( opciones => {
    opciones.AddPolicy ( "EsAdmin", politica => politica.RequireClaim ( "esAdmin" ) );
} );

builder.Services.AddDataProtection();
builder.Services.AddTransient<HashService>();

builder.Services.AddCors ( opciones => {
    opciones.AddDefaultPolicy ( builder => 
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders ( [ "cantidadTotalRegistros" ] ) );
} );

builder.Services.AddTransient<GeneradorEnlaces>();
builder.Services.AddTransient<HATEOASAutorFilterAttribute>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

var app = builder.Build();

app.UseLoguearRespuestaHttp();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.Run();
