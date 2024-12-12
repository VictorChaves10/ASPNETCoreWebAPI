using ASP.NETCore_WebAPI.Context;
using ASP.NETCore_WebAPI.Domain;
using ASP.NETCore_WebAPI.DTOs.Mappings;
using ASP.NETCore_WebAPI.Filters;
using ASP.NETCore_WebAPI.Repositories;
using ASP.NETCore_WebAPI.Repositories.Interfaces;
using ASP.NETCore_WebAPI.Services;
using ASP.NETCore_WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ApiExceptionFilter));

})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
}).AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();

#region ConnectionString

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

#endregion

#region JwtIdentity

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

var secretKey = builder.Configuration["JWT:SecretKey"]
                    ?? throw new ArgumentException("Invalid secret key!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))

    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
    .AddPolicy("SuperAdminOnly", policy => policy.RequireRole("Admin")
                                                 .RequireClaim("id", "macoratti"))
    .AddPolicy("UserOnly", policy => policy.RequireRole("User"))
    .AddPolicy("ExclusiveOnly", policy =>
                policy.RequireAssertion(context =>
                context.User.HasClaim(claim =>
                claim.Type == "id" && claim.Value == "macoratti") || context.User.IsInRole("SuperAdmin")));


#endregion

#region Swagger

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "apicatalogo", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer JWT ",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
                });
});

#endregion

#region Cors

var OrigensComAcessoPermitido = "_origensComAcessoPermitido";

builder.Services.AddCors(options => {
    options.AddPolicy(name: OrigensComAcessoPermitido,
    policy =>
    {
        policy.WithOrigins("https://apirequest.io");
    });
 });


#endregion

builder.Services.AddScoped<ApiLogginFilter>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAutoMapper(typeof(DomainToDTOMappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};

if (app.Environment.IsProduction())
    app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors(OrigensComAcessoPermitido);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
