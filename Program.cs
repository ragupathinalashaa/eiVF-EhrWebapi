using AutoMapper;
using Data;
using Data.Repositories;
using eIVF;
using eIVF.Filters;
using Microsoft.EntityFrameworkCore;
using Services.Manager;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<EhrDataContext>(options =>
    options.UseSqlServer(connectionString)
);
//AutoMapper Registration
var mapperConfig = new MapperConfiguration(mc => {
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<IUpdataDataSyncRespository, UpdateDataSyncRepository>();
builder.Services.AddTransient<IDataSyncService, DataSyncService>();
builder.Services.AddTransient<IFhirService, FhirService>();
builder.Services.AddSwaggerGen(c =>
{
    // Set the comments path for the Swagger JSON and UI.
    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
//builder.Services.AddMvcCore(options =>
//{
//    options.Filters.Add(
//          new ApiResourceFilter()
//    );
//    options.Filters.Add(
//          new ApiResultFilter()
//    );
//});

var app = builder.Build();


var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
//app.UseSwaggerUI();
//}
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "eIVF API V1");
    //c.RoutePrefix = string.Empty;
});
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
