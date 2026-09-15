namespace Cochief.Infrastructure.Persistence.Mappers;

using Cochief.Domain.Model;
using Cochief.Infrastructure.Persistence.Entities;

internal sealed class UserMapper(PlayerMapper playerMapper) : IMapper<User, UserEntity>
{
    private readonly PlayerMapper _playerMapper = playerMapper;

    public User ToDomain(UserEntity entity)
    {
        Player? player = entity.Player is null ? null : _playerMapper.ToDomain(entity.Player);

        return User.Restore(entity.Id, entity.Name, entity.Email, entity.PasswordHash, player);
    }

    public UserEntity ToPersistence(User model)
    {
        return new UserEntity
        {
            Id = model.Id,
            Name = model.Name,
            Email = model.Email.Value,
            PasswordHash = model.PasswordHash,
            PlayerId = model.Player?.Id
        };
    }
}
