using Application.Characters.CreateCharacter;
using FluentAssertions;

namespace DragonBall.Tests.Characters;

public sealed class CreateCharacterValidatorTests
{
    private readonly CreateCharacterValidator _validator = new();

    private static CreateCharacterCommand ValidCommand() =>
        new("Goku", "Son", "Saiyan", 9000, null, null, null);

    [Fact]
    public async Task Valid_command_passes()
    {
        var result = await _validator.ValidateAsync(ValidCommand(), TestContext.Current.CancellationToken);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Valid_command_with_zero_power_level_passes()
    {
        var result = await _validator.ValidateAsync(ValidCommand() with { PowerLevel = 0 }, TestContext.Current.CancellationToken);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Valid_command_without_optional_fields_passes()
    {
        var command = new CreateCharacterCommand("Goku", "Son", null, 9000, null, null, null);
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);
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
    public async Task Empty_last_name_fails(string lastName)
    {
        var result = await _validator.ValidateAsync(ValidCommand() with { LastName = lastName }, TestContext.Current.CancellationToken);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LastName");
    }

    [Fact]
    public async Task LastName_exceeding_100_chars_fails()
    {
        var result = await _validator.ValidateAsync(ValidCommand() with { LastName = new string('A', 101) }, TestContext.Current.CancellationToken);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LastName");
    }

    [Fact]
    public async Task Negative_power_level_fails()
    {
        var result = await _validator.ValidateAsync(ValidCommand() with { PowerLevel = -1 }, TestContext.Current.CancellationToken);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PowerLevel");
    }

    [Fact]
    public async Task Race_exceeding_100_chars_fails()
    {
        var result = await _validator.ValidateAsync(ValidCommand() with { Race = new string('A', 101) }, TestContext.Current.CancellationToken);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Race");
    }

    [Fact]
    public async Task Null_race_passes()
    {
        var result = await _validator.ValidateAsync(ValidCommand() with { Race = null }, TestContext.Current.CancellationToken);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Description_exceeding_500_chars_fails()
    {
        var result = await _validator.ValidateAsync(ValidCommand() with { Description = new string('A', 501) }, TestContext.Current.CancellationToken);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }
}
