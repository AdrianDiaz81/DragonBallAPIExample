using Application.Characters;
using Application.Characters.CreateCharacter;
using Application.Characters.DeleteCharacter;
using Application.Characters.GetCharacterById;
using Application.Characters.GetCharacters;
using Application.Characters.UpdateCharacter;
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
builder.Services.AddScoped<UpdateCharacterHandler>();
builder.Services.AddScoped<DeleteCharacterHandler>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();

app.MapOpenApi();
app.MapScalarApiReference(); // default route: /scalar/{documentName} → /scalar/v1

app.MapEndpoints();

app.Run();

public partial class Program { }
