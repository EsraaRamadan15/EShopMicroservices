var builder = WebApplication.CreateBuilder(args);

//---- add services to container
var app = builder.Build();

//--- Configure HTTP request pipline
app.Run();
