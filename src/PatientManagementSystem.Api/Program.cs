using PatientManagementSystem.Common.Middleware;
using PatientManagementSystem.Modules.Identity;
using PatientManagementSystem.Modules.Identity.DependencyInjection;
using PatientManagementSystem.Modules.Identity.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(IdentityModuleMarker).Assembly);

builder.Services.AddIdentityModule(builder.Configuration);

var app = builder.Build();

await IdentityRoleSeeder.SeedAsync(app.Services);

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello, this is James! Welcome to my humble Patient Management System API! :)");

app.MapControllers();

app.Run();
