using System;
using Domain.Dto.Friendship;
using Domain.Entities;

namespace Domain.Utils.Mappers.Friends;

public class FriendshipMapper
{
    /// <summary>
    /// Converte uma entidade Friendship para um ListFriendsDto.
    /// </summary>
    /// <param name="friendship">A entidade de amizade a ser convertida.</param>
    /// <returns>O objeto ListFriendsDto mapeado.</returns>
    public static ListFriendsDto ToDto(Friendship friendship)
    {
        return new ListFriendsDto
        {
            FriendshipId = friendship.Id,
            FriendId = friendship.AddresseeId,
            FriendName = friendship.Addressee?.UserName, // Assumindo que a entidade User tem a propriedade 'Name'
            Status = friendship.Status.ToString(), // Convertendo o enum para string
            RequestedAt = friendship.RequestedAt
        };
    }

    public static ListFriendsDto ToDtoFriendRequest(Friendship friendship)
    {
        return new ListFriendsDto
        {
            FriendshipId = friendship.Id,
            FriendId = friendship.RequesterId,
            FriendName = friendship.Requester?.UserName, // Assumindo que a entidade User tem a propriedade 'Name'
            Status = friendship.Status.ToString(), // Convertendo o enum para string
            RequestedAt = friendship.RequestedAt
        };
    }

    /// <summary>
    /// Converte uma coleção de entidades Friendship para uma coleção de ListFriendsDto.
    /// </summary>
    /// <param name="friendships">A coleção de entidades de amizade.</param>
    /// <returns>Uma coleção de objetos ListFriendsDto mapeados.</returns>
    public static IEnumerable<ListFriendsDto> ToDto(IEnumerable<Friendship> friendships)
    {
        return friendships.Select(f => ToDto(f));
    }

    public static IEnumerable<ListFriendsDto> ToDtoFriendRequests(IEnumerable<Friendship> friendships)
    {
        return friendships.Select(f => ToDtoFriendRequest(f));
    }


}
