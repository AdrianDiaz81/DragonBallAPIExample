using Application.Characters;
using Application.Characters.CreateCharacter;
using Application.Characters.GetCharacterById;
using Application.Characters.GetCharacters;
using Domain.Characters;
using FluentValidation;
using MinApiLib.Endpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddOpenApi();
builder.Services.AddSingleton<ICharacterRepository, InMemoryCharacterRepository>();
builder.Services.AddValidatorsFromAssembly(typeof(GetCharactersQuery).Assembly);
builder.Services.AddScoped<GetCharactersHandler>();
builder.Services.AddScoped<GetCharacterByIdHandler>();
builder.Services.AddScoped<CreateCharacterHandler>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapEndpoints();

app.Run();

public partial class Program { }
