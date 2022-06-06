using AutoMapper;
using Data;
using Data.Repositories;
using eIVF;
using Microsoft.EntityFrameworkCore;
using Services.Manager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("EhrDataConnection");
builder.Services.AddDbContext<EhrDataContext>(options =>
    options.UseSqlServer(connectionString)
);
//AutoMapper Registration
var mapperConfig = new MapperConfiguration(mc => {
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientAddressRepository, PatientAddressRepository>();
builder.Services.AddTransient<IPatientService, PatientService>();
var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
