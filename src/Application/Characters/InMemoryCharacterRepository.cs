using Domain.Characters;

namespace Application.Characters;

public sealed class InMemoryCharacterRepository : ICharacterRepository
{
    private readonly List<Character> _characters =
    [
        new() { Id = 1,  Name = "Goku",               Race = "Saiyan",            PowerLevel = 9000,       Affiliation = "Z Fighters",      ImageUrl = "https://dragonball-api.com/characters/goku_normal.webp" },
        new() { Id = 2,  Name = "Vegeta",             Race = "Saiyan",            PowerLevel = 8500,       Affiliation = "Z Fighters",      ImageUrl = "https://dragonball-api.com/characters/vegeta_normal.webp" },
        new() { Id = 3,  Name = "Piccolo",             Race = "Namekian",          PowerLevel = 5000,       Affiliation = "Z Fighters",      ImageUrl = "https://dragonball-api.com/characters/picolo_normal.webp" },
        new() { Id = 4,  Name = "Bulma",               Race = "Human",             PowerLevel = 5,          Affiliation = "Z Fighters",      ImageUrl = "https://dragonball-api.com/characters/bulma.webp" },
        new() { Id = 5,  Name = "Frieza",              Race = "Frieza Race",       PowerLevel = 12000000,   Affiliation = "Frieza Force",    ImageUrl = "https://dragonball-api.com/characters/Freezer.webp" },
        new() { Id = 6,  Name = "Zarbon",              Race = "Alien",             PowerLevel = 23000,      Affiliation = "Frieza Force",    ImageUrl = "https://dragonball-api.com/characters/zarbon.webp" },
        new() { Id = 7,  Name = "Dodoria",             Race = "Alien",             PowerLevel = 22000,      Affiliation = "Frieza Force",    ImageUrl = "https://dragonball-api.com/characters/dodoria.webp" },
        new() { Id = 8,  Name = "Ginyu",               Race = "Alien",             PowerLevel = 120000,     Affiliation = "Frieza Force",    ImageUrl = "https://dragonball-api.com/characters/ginyu.webp" },
        new() { Id = 9,  Name = "Cell",                Race = "Bio-Android",       PowerLevel = 900000000,  Affiliation = "Red Ribbon Army", ImageUrl = "https://dragonball-api.com/characters/celula.webp" },
        new() { Id = 10, Name = "Gohan",               Race = "Half-Saiyan",       PowerLevel = 7000,       Affiliation = "Z Fighters",      ImageUrl = "https://dragonball-api.com/characters/gohan.webp" },
        new() { Id = 11, Name = "Krillin",             Race = "Human",             PowerLevel = 1770,       Affiliation = "Z Fighters",      ImageUrl = "https://dragonball-api.com/characters/Krilin_Universo7.webp" },
        new() { Id = 12, Name = "Tenshinhan",          Race = "Human",             PowerLevel = 1830,       Affiliation = "Z Fighters",      ImageUrl = "https://dragonball-api.com/characters/Tenshinhan_Universo7.webp" },
        new() { Id = 13, Name = "Yamcha",              Race = "Human",             PowerLevel = 1480,       Affiliation = "Z Fighters",      ImageUrl = "https://dragonball-api.com/characters/Final_Yamcha.webp" },
        new() { Id = 14, Name = "Chi-Chi",             Race = "Human",             PowerLevel = 130,        Affiliation = "Z Fighters",      ImageUrl = "https://dragonball-api.com/characters/ChiChi_DBS.webp" },
        new() { Id = 15, Name = "Gotenks",             Race = "Half-Saiyan",       PowerLevel = 90000000,   Affiliation = "Z Fighters",      ImageUrl = "https://dragonball-api.com/characters/Gotenks_Artwork.webp" },
        new() { Id = 16, Name = "Trunks",              Race = "Half-Saiyan",       PowerLevel = 6500,       Affiliation = "Z Fighters",      ImageUrl = "https://dragonball-api.com/characters/Trunks_Buu_Artwork.webp" },
        new() { Id = 17, Name = "Master Roshi",        Race = "Human",             PowerLevel = 139,        Affiliation = "Z Fighters",      ImageUrl = "https://dragonball-api.com/characters/roshi.webp" },
        new() { Id = 18, Name = "Bardock",             Race = "Saiyan",            PowerLevel = 10000,      Affiliation = "None",            ImageUrl = "https://dragonball-api.com/characters/Bardock_Artwork.webp" },
        new() { Id = 19, Name = "Launch",              Race = "Human",             PowerLevel = 18,         Affiliation = "None",            ImageUrl = "https://dragonball-api.com/characters/Lunch_traje_de_sirvienta_en_el_manga.webp" },
        new() { Id = 20, Name = "Mr. Satan",           Race = "Human",             PowerLevel = 10,         Affiliation = "None",            ImageUrl = "https://dragonball-api.com/characters/Mr_Satan_DBSuper.webp" },
        new() { Id = 21, Name = "Dende",               Race = "Namekian",          PowerLevel = 10,         Affiliation = "Z Fighters",      ImageUrl = "https://dragonball-api.com/characters/Dende_Artwork.webp" },
        new() { Id = 22, Name = "Android 17",          Race = "Android",           PowerLevel = 1400000,    Affiliation = "None",            ImageUrl = "https://dragonball-api.com/characters/17_Artwork.webp" },
        new() { Id = 23, Name = "Android 16",          Race = "Android",           PowerLevel = 1400000,    Affiliation = "Red Ribbon Army", ImageUrl = "https://dragonball-api.com/characters/Androide_16.webp" },
        new() { Id = 24, Name = "Android 19",          Race = "Android",           PowerLevel = 800000,     Affiliation = "Red Ribbon Army", ImageUrl = "https://dragonball-api.com/characters/Android19.webp" },
        new() { Id = 25, Name = "Android 13",          Race = "Android",           PowerLevel = 1000000,    Affiliation = "Red Ribbon Army", ImageUrl = "https://dragonball-api.com/characters/Androide13normal.webp" },
        new() { Id = 26, Name = "Android 14",          Race = "Android",           PowerLevel = 500000,     Affiliation = "Red Ribbon Army", ImageUrl = "https://dragonball-api.com/characters/14Dokkan.webp" },
        new() { Id = 27, Name = "Android 15",          Race = "Android",           PowerLevel = 500000,     Affiliation = "Red Ribbon Army", ImageUrl = "https://dragonball-api.com/characters/15Dokkan.webp" },
        new() { Id = 28, Name = "Nail",                Race = "Namekian",          PowerLevel = 42000,      Affiliation = "None",            ImageUrl = "https://dragonball-api.com/characters/Nail_Artwork.webp" },
        new() { Id = 29, Name = "Raditz",              Race = "Saiyan",            PowerLevel = 1500,       Affiliation = "None",            ImageUrl = "https://dragonball-api.com/characters/Raditz_artwork_Dokkan.webp" },
        new() { Id = 30, Name = "Babidi",              Race = "Wizard",            PowerLevel = 200,        Affiliation = "None",            ImageUrl = "https://dragonball-api.com/characters/Babidi_Artwork.webp" },
        new() { Id = 31, Name = "Majin Buu",           Race = "Majin",             PowerLevel = 1000000000, Affiliation = "None",            ImageUrl = "https://dragonball-api.com/characters/BuuGordo_Universo7.webp" },
        new() { Id = 32, Name = "Beerus",              Race = "God of Destruction",PowerLevel = 999999999,  Affiliation = "None",            ImageUrl = "https://dragonball-api.com/characters/Beerus_DBS_Broly_Artwork.webp" },
        new() { Id = 33, Name = "Whis",                Race = "Angel",             PowerLevel = 999999999,  Affiliation = "None",            ImageUrl = "https://dragonball-api.com/characters/Whis_DBS_Broly_Artwork.webp" },
        new() { Id = 34, Name = "Zeno",                Race = "God",               PowerLevel = 999999999,  Affiliation = "None",            ImageUrl = "https://dragonball-api.com/characters/Zeno_Artwork.webp" },
        new() { Id = 35, Name = "Kibito-Shin",         Race = "Supreme Kai",       PowerLevel = 500,        Affiliation = "Z Fighters",      ImageUrl = "https://dragonball-api.com/characters/Kibito_shin_Artwork.webp" },
        new() { Id = 36, Name = "Jiren",               Race = "Unknown",           PowerLevel = 999999998,  Affiliation = "None",            ImageUrl = "https://dragonball-api.com/characters/Jiren.webp" },
        new() { Id = 37, Name = "Toppo",               Race = "Unknown",           PowerLevel = 500000000,  Affiliation = "None",            ImageUrl = "https://dragonball-api.com/characters/Toppo.webp" },
        new() { Id = 38, Name = "Dyspo",               Race = "Unknown",           PowerLevel = 200000000,  Affiliation = "None",            ImageUrl = "https://dragonball-api.com/characters/Dispo_render.webp" },
        new() { Id = 39, Name = "Broly",               Race = "Saiyan",            PowerLevel = 1400000000, Affiliation = "None",            ImageUrl = "https://dragonball-api.com/transformaciones/Broly_DBS_Base.webp" },
        new() { Id = 40, Name = "Gogeta",              Race = "Saiyan",            PowerLevel = 999999999,  Affiliation = "Z Fighters",      ImageUrl = "https://dragonball-api.com/transformaciones/gogeta.webp" },
    ];

    public IReadOnlyList<Character> GetAll() => _characters.AsReadOnly();

    public Character? GetById(int id) => _characters.FirstOrDefault(c => c.Id == id);

    public Character Add(Character character)
    {
        var newId = _characters.Count > 0 ? _characters.Max(c => c.Id) + 1 : 1;
        var newCharacter = new Character
        {
            Id = newId,
            Name = character.Name,
            Race = character.Race,
            PowerLevel = character.PowerLevel,
            Affiliation = character.Affiliation,
            ImageUrl = character.ImageUrl
        };
        _characters.Add(newCharacter);
        return newCharacter;
    }
}
