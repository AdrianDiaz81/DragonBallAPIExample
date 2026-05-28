using Application.Characters.CreateCharacter;
using FluentAssertions;

namespace DragonBall.Tests.Characters;

public sealed class CreateCharacterValidatorTests
{
    private readonly CreateCharacterValidator _validator = new();

    private static CreateCharacterCommand ValidCommand() =>
        new("Goku", "Saiyan", 9000, "Z Fighters");

    [Fact]
    public async Task Valid_command_passes()
    {
        var result = await _validator.ValidateAsync(ValidCommand(), TestContext.Current.CancellationToken);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Empty_name_fails(string name)
    {
        var result = await _validator.ValidateAsync(ValidCommand() with { Name = name }, TestContext.Current.CancellationToken);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Name_exceeding_100_chars_fails()
    {
        var result = await _validator.ValidateAsync(ValidCommand() with { Name = new string('A', 101) }, TestContext.Current.CancellationToken);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Empty_race_fails(string race)
    {
        var result = await _validator.ValidateAsync(ValidCommand() with { Race = race }, TestContext.Current.CancellationToken);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Race");
    }

    [Fact]
    public async Task Race_exceeding_50_chars_fails()
    {
        var result = await _validator.ValidateAsync(ValidCommand() with { Race = new string('A', 51) }, TestContext.Current.CancellationToken);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Race");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task PowerLevel_zero_or_negative_fails(int powerLevel)
    {
        var result = await _validator.ValidateAsync(ValidCommand() with { PowerLevel = powerLevel }, TestContext.Current.CancellationToken);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PowerLevel");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Empty_affiliation_fails(string affiliation)
    {
        var result = await _validator.ValidateAsync(ValidCommand() with { Affiliation = affiliation }, TestContext.Current.CancellationToken);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Affiliation");
    }

    [Fact]
    public async Task Affiliation_exceeding_100_chars_fails()
    {
        var result = await _validator.ValidateAsync(ValidCommand() with { Affiliation = new string('A', 101) }, TestContext.Current.CancellationToken);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Affiliation");
    }
}
