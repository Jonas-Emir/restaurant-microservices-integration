using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RestauranteService.Data;
using RestauranteService.ItemServiceHttpClient;
using RestauranteService.RabbitMqClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("RestauranteConnection");

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connectionString));

builder.Services.AddScoped<IRestauranteRepository, RestauranteRepository>();

builder.Services.AddSingleton<IRabbitMqClient, RabbitMqClient>();

builder.Services.AddHttpClient<IItemServiceHttpClient, ItemSeviceHttpClient>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RestauranteService", Version = "v1" });
});


builder.Services.AddExceptionHandler<RestauranteService.Middlewares.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
