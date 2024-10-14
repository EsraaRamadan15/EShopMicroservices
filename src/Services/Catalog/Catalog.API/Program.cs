using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

//---- add services to container
var app = builder.Build();

//--- Configure HTTP request pipline

app.MapCarter();
app.Run();
