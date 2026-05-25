using Application.Characters;
using Application.Characters.CreateCharacter;
using Application.Characters.GetCharacters;
using Domain.Characters;
using FluentValidation;
using MinApiLib.Endpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<ICharacterRepository, InMemoryCharacterRepository>();
builder.Services.AddValidatorsFromAssembly(typeof(GetCharactersQuery).Assembly);
builder.Services.AddScoped<GetCharactersHandler>();
builder.Services.AddScoped<CreateCharacterHandler>();

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapEndpoints();

app.Run();

public partial class Program { }
