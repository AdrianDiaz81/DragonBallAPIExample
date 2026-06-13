using Application.Characters.DeleteCharacter;
using Domain.Characters;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace DragonBall.Tests.Characters;

public sealed class DeleteCharacterHandlerTests
{
    private readonly ICharacterRepository _repository = Substitute.For<ICharacterRepository>();
    private readonly DeleteCharacterHandler _handler;

    public DeleteCharacterHandlerTests()
    {
        _handler = new DeleteCharacterHandler(_repository, NullLogger<DeleteCharacterHandler>.Instance);
    }

    [Fact]
    public async Task Deleting_existing_character_returns_true()
    {
        _repository.Delete(1).Returns(true);

        var result = await _handler.Handle(new DeleteCharacterCommand(1), TestContext.Current.CancellationToken);

        result.Should().BeTrue();
        _repository.Received(1).Delete(1);
    }

    [Fact]
    public async Task Deleting_non_existent_id_returns_false()
    {
        _repository.Delete(9999).Returns(false);

        var result = await _handler.Handle(new DeleteCharacterCommand(9999), TestContext.Current.CancellationToken);

        result.Should().BeFalse();
    }
}
