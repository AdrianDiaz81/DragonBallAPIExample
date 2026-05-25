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
        var command = new CreateCharacterCommand("Bardock", "Saiyan", 10000, "None");
        var expected = new Character { Id = 11, Name = "Bardock", Race = "Saiyan", PowerLevel = 10000, Affiliation = "None" };
        _repository.Add(Arg.Any<Character>()).Returns(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeEquivalentTo(expected);
        _repository.Received(1).Add(Arg.Any<Character>());
    }

    [Fact]
    public async Task Invalid_command_throws_ValidationException_and_does_not_call_repository()
    {
        var command = new CreateCharacterCommand("", "Saiyan", 9000, "Z Fighters");

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
        _repository.DidNotReceive().Add(Arg.Any<Character>());
    }

    [Fact]
    public async Task PowerLevel_zero_throws_ValidationException()
    {
        var command = new CreateCharacterCommand("Goku", "Saiyan", 0, "Z Fighters");

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*PowerLevel*");
    }
}
