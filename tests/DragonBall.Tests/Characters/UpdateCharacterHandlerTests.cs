using Application.Characters.UpdateCharacter;
using Domain.Characters;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;

namespace DragonBall.Tests.Characters;

public sealed class UpdateCharacterHandlerTests
{
    private readonly InMemoryCharacterRepositoryStub _repository = new();
    private readonly UpdateCharacterHandler _handler;

    public UpdateCharacterHandlerTests()
    {
        _handler = new UpdateCharacterHandler(
            _repository,
            new UpdateCharacterValidator(),
            NullLogger<UpdateCharacterHandler>.Instance);
    }

    [Fact]
    public async Task Updating_only_power_level_leaves_other_fields_unchanged()
    {
        var command = new UpdateCharacterCommand(1, PowerLevel: 99);

        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result!.PowerLevel.Should().Be(99);
        result.Name.Should().Be("Goku");
        result.LastName.Should().Be("Son");
    }

    [Fact]
    public async Task Empty_name_sent_throws_ValidationException()
    {
        var command = new UpdateCharacterCommand(1, Name: "");

        var act = () => _handler.Handle(command, TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Negative_power_level_sent_throws_ValidationException()
    {
        var command = new UpdateCharacterCommand(1, PowerLevel: -1);

        var act = () => _handler.Handle(command, TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Non_existent_id_returns_null()
    {
        var command = new UpdateCharacterCommand(9999, Name: "Test");

        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Empty_payload_is_a_valid_no_op()
    {
        var command = new UpdateCharacterCommand(1);

        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Goku");
        result.PowerLevel.Should().Be(9000);
    }

    // Stub mínimo en memoria para los tests del handler
    private sealed class InMemoryCharacterRepositoryStub : ICharacterRepository
    {
        private readonly List<Character> _data =
        [
            new() { Id = 1, Name = "Goku", LastName = "Son", Race = "Saiyan", PowerLevel = 9000 }
        ];

        public IReadOnlyList<Character> GetAll() => _data.AsReadOnly();
        public Character? GetById(int id) => _data.FirstOrDefault(c => c.Id == id);
        public Character Add(Character character) => character;

        public Character? Update(int id, CharacterPatch patch)
        {
            var index = _data.FindIndex(c => c.Id == id);
            if (index < 0) return null;
            var e = _data[index];
            var updated = new Character
            {
                Id = e.Id,
                Name = patch.Name ?? e.Name,
                LastName = patch.LastName ?? e.LastName,
                Race = patch.Race ?? e.Race,
                PowerLevel = patch.PowerLevel ?? e.PowerLevel,
                Description = patch.Description ?? e.Description,
                Affiliation = patch.Affiliation ?? e.Affiliation,
                ImageUrl = patch.ImageUrl ?? e.ImageUrl
            };
            _data[index] = updated;
            return updated;
        }

        public bool Delete(int id) => _data.RemoveAll(c => c.Id == id) > 0;
    }
}
