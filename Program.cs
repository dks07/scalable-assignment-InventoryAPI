using InventoryAPI.Middleware;
using InventoryAPI.Services;
using InventoryAPI.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

configuration.AddEnvironmentVariables();

// Register InventotyDatabaseSettings

var inventoryDbSettingsSection = configuration.GetSection(nameof(InventotyDatabaseSettings));
services.Configure<InventotyDatabaseSettings>(inventoryDbSettingsSection);

services.AddSingleton<IInventotyDatabaseSettings>(sp =>
  sp.GetRequiredService<IOptions<InventotyDatabaseSettings>>().Value);

// Register MongoClient
services.AddSingleton<IMongoClient>(serviceProvider => {
  var settings = serviceProvider.GetRequiredService<IOptions<InventotyDatabaseSettings>>().Value;
  return new MongoClient(settings.ConnectionString);
});

// Register InventoryService
services.AddScoped<IInventoryService, InventoryService>();

var jwtSettings = new JwtSettings();

// Add authentication configuration

services.AddAuthentication(x =>
  {
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
  })
  .AddJwtBearer(x =>
  {
    x.TokenValidationParameters = new TokenValidationParameters
    {
      IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSettings.SecretKey)),
      ValidateIssuer = false,
      ValidateAudience = false,
      RequireExpirationTime = true,
      ClockSkew = TimeSpan.Zero
    };
  });

services.AddControllers();

services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "InventoryService", Version = "v1" });

  var jwtSecurityScheme = new OpenApiSecurityScheme
  {
    Scheme = "bearer",
    BearerFormat = "JWT",
    Name = "JWT Authentication",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.Http,
    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",
    
    Reference = new OpenApiReference
    {
      Id = JwtBearerDefaults.AuthenticationScheme,
      Type = ReferenceType.SecurityScheme
    }
  };
  c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
  c.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {jwtSecurityScheme, Array.Empty<string>()}
  });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
// Configure middleware and other settings
app.UseSwagger();
app.UseSwaggerUI(c =>
{
  c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory API V1");
});
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseEndpoints(endpoints =>
{
  endpoints.MapControllers();
});
app.Run();
