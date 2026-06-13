using Application.Characters.CreateCharacter;
using Domain.Characters;
using FluentAssertions;
using FluentValidation;
using NSubstitute;

namespace DragonBall.Tests.Characters;

public sealed class CreateCharacterHandlerTests
{
    private readonly ICharacterRepository _repository = Substitute.For<ICharacterRepository>();
    private readonly CreateCharacterHandler _handler;

    public CreateCharacterHandlerTests()
    {
        _handler = new CreateCharacterHandler(_repository, new CreateCharacterValidator());
    }

    [Fact]
    public async Task Valid_command_creates_and_returns_character()
    {
        var command = new CreateCharacterCommand("Bardock", "Bardock", "Saiyan", 10000, null, "None", null);
        var expected = new Character { Id = 41, Name = "Bardock", LastName = "Bardock", Race = "Saiyan", PowerLevel = 10000 };
        _repository.Add(Arg.Any<Character>()).Returns(expected);

        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(expected);
        _repository.Received(1).Add(Arg.Any<Character>());
    }

    [Fact]
    public async Task Empty_name_throws_ValidationException_and_does_not_call_repository()
    {
        var command = new CreateCharacterCommand("", "Son", "Saiyan", 9000, null, null, null);

        var act = () => _handler.Handle(command, TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<ValidationException>();
        _repository.DidNotReceive().Add(Arg.Any<Character>());
    }

    [Fact]
    public async Task Empty_last_name_throws_ValidationException()
    {
        var command = new CreateCharacterCommand("Goku", "", "Saiyan", 9000, null, null, null);

        var act = () => _handler.Handle(command, TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<ValidationException>();
        _repository.DidNotReceive().Add(Arg.Any<Character>());
    }

    [Fact]
    public async Task Negative_power_level_throws_ValidationException()
    {
        var command = new CreateCharacterCommand("Goku", "Son", "Saiyan", -1, null, null, null);

        var act = () => _handler.Handle(command, TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<ValidationException>();
        _repository.DidNotReceive().Add(Arg.Any<Character>());
    }

    [Fact]
    public async Task Zero_power_level_is_valid_and_creates_character()
    {
        var command = new CreateCharacterCommand("Goku", "Son", "Saiyan", 0, null, null, null);
        var expected = new Character { Id = 41, Name = "Goku", LastName = "Son", PowerLevel = 0 };
        _repository.Add(Arg.Any<Character>()).Returns(expected);

        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        result.PowerLevel.Should().Be(0);
        _repository.Received(1).Add(Arg.Any<Character>());
    }
}
