using Sirenix.OdinInspector;
using System;
using Unity.Services.Authentication;
using Unity.Services.Friends;
using Unity.Services.Friends.Models;
using Unity.Services.Friends.Notifications;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FriendsManagerqqwq : MonoBehaviour
{
    [Button]
    public async void InitializeFriends()
    {
        await FriendsService.Instance.InitializeAsync();

        FriendsService.Instance.RelationshipAdded += OnRelationshipAdded;
        FriendsService.Instance.RelationshipDeleted += OnRelationshipDeleted;
        FriendsService.Instance.MessageReceived += OnMessageRecived;
        FriendsService.Instance.PresenceUpdated += OnPresenceUpdated;
    }


    //
    #region Helpers

    [Button]
    public void ShowFriends()
    {
        foreach (Relationship rel in FriendsService.Instance.Friends)

            Debug.Log(rel.Member.Id + " ||" + rel.Member.Presence.Availability);
    }

    [Button]
    public void ShowIncoming()
    {
        foreach (Relationship rel in FriendsService.Instance.IncomingFriendRequests)

            Debug.Log(rel.Member.Id + " || " + rel.Member.Presence.Availability);
        // SendFriendRequest(rel.Member.Id);
    }

    [Button]
    public void ShowOutgoing()
    {
        foreach (Relationship rel in FriendsService.Instance.OutgoingFriendRequests)

            Debug.Log(rel.Member.Id + " || " + rel.Member.Presence.Availability);
        // SendFriendRequest(rel.Member.Id);
    }
    [Button]
    public void ShowBlocks()
    {
        foreach (Relationship rel in FriendsService.Instance.Blocks)

            Debug.Log(rel.Member.Id + " ||" + rel.Member.Presence.Availability);

        // UnblockUser(rel.Member.Id);
    }

    #endregion
    //

    #region Events

    [Button]

    public async void DeleteFriend(string playerId)
    {
        await FriendsService.Instance.DeleteFriendAsync(playerId);
    }
    [Button]
    public async void DeleteIncomingRequest(string playerId)
    {
        await FriendsService.Instance.DeleteIncomingFriendRequestAsync(playerId);
    }
    [Button]
    public async void DeleteOutgoingRequest(string playerId)
    {
        await FriendsService.Instance.DeleteOutgoingFriendRequestAsync(playerId);
    }
    private void OnPresenceUpdated(IPresenceUpdatedEvent e)
    {
        Debug.Log("Se a actualizado es estado de" + e.ID + e.Presence.Availability);
    }

    private void OnMessageRecived(IMessageReceivedEvent e)
    {
        //e.GetAs<>
        Debug.Log("he recibido un mensaje de " + e.UserId + "mensaje : " + e);
    }

    private void OnRelationshipDeleted(IRelationshipDeletedEvent e)
    {

        Debug.Log("Relationship added : " + e.Relationship.Id);

    }
    private void OnRelationshipAdded(IRelationshipAddedEvent e) {

        Debug.Log("Relationship added : " + e.Relationship.Id + e.Relationship.Type);
    }
    #endregion


    /*private void OnPresenceUpdated(IPresenceUpdatedEvent @event)
{
    throw new NotImplementedException();
}

private void OnMessageRecived(IMessageReceivedEvent @event)
{

    throw new NotImplementedException();

}
private void OnRelationshipDeleted(IRelationshipDeletedEvent @event)
{
    throw new NotImplementedException();
}

private void OnRelationshipAdded(IRelationshipAddedEvent @event)
{
    throw new NotImplementedException();

}*/

    #region Message
    public class LobbyInviteMessage
    {
        public string lobbyId;
        public string fromPlayerID;
        public LobbyInviteMessage() { }
    }
    public class SimpleMessage
    {
        public string content;


        public SimpleMessage() { }
    }
    #endregion*

}
/* public async Task SendMessage(string targetID, string text)
 {
     var msg = new SimpleMessage();
     msg.content = text;

     await FriendsService.Instance.MessageAsync(targetID, msg);

     Debug.Log("Mensaje enviado con exito");
 }
 [Button]

 public async Task SendLobbyInvite(string targetID, string lobbyId)
 {

     var lobbyInvite = new LobbyInviteMessage();
     lobbyInvite.lobbyId = lobbyId;
     lobbyInvite.fromPlayerID = AuthenticationService.Instance.PlayerId;

     await FriendsService.Instance.MessageAsync(targetID, lobbyInvite);

     Debug.Log("Invitacion a lobby enviada");



 }
*/
