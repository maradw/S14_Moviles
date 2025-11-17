using UnityEngine;
using Unity.Services.Friends;
using Sirenix.OdinInspector;
using Unity.Services.Friends.Notifications;
using System;
using Unity.Services.Friends.Models;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine.UI;
using TMPro;

public class FriendsManager : MonoBehaviour
{
    [SerializeField] TMP_InputField playerIDRequest;
    [SerializeField]Button sendFriendRequest;
    
    [SerializeField] Button showFriends;
    [SerializeField] TextMeshProUGUI friends;

    [SerializeField] Button showIncomingFriends;
    [SerializeField] TextMeshProUGUI incoming;

    [SerializeField] Button State;
    [SerializeField] TextMeshProUGUI StatePlayer;
    void Start()
    {
       // InitializeFriends();
        sendFriendRequest.onClick.AddListener(() => AddFriendByName());
        showFriends.onClick.AddListener(() => ShowFriends());
        showIncomingFriends.onClick.AddListener(() => ShowIncoming());
        State.onClick.AddListener(() => SetAvailability(Availability.Busy));
    }


    [Button]
    public async void InitializeFriends()
    {
        await FriendsService.Instance.InitializeAsync();

        FriendsService.Instance.RelationshipAdded += OnRelationshipAdded;
        FriendsService.Instance.RelationshipDeleted += OnRelationshipDeleted;
        FriendsService.Instance.MessageReceived += OnMessageRecived;
        FriendsService.Instance.PresenceUpdated += OnPresenceUpdated;
        Debug.Log("initialized");

    }
    #region Methods

    [Button]
    public async void SendFriendRequest()
    {
        string playerId = playerIDRequest.text;
        await FriendsService.Instance.AddFriendAsync(playerId);
        Debug.Log("Sending friend request");    }
    [Button]
    public async void AddFriendByName()
    {
        string username = playerIDRequest.text;
        await FriendsService.Instance.AddFriendByNameAsync(username);
    }

    [Button]
    public async void BlockUser(string playerId)
    {
        await FriendsService.Instance.AddBlockAsync(playerId);  
    }
    [Button]
    public async void UnblockUser(string playerId)
    {
        await FriendsService.Instance.DeleteBlockAsync(playerId);
    }

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
    #endregion
    #region Helpers

    [Button]
    public void ShowFriends()
    {
        foreach (Relationship rel in FriendsService.Instance.Friends)
        {
            friends.text = " friends:  " + "\n " + rel.Member.Id + " ||" + rel.Member.Presence.Availability.ToString();
            Debug.Log(" friends:  " + "\n "+ rel.Member.Id + " ||" + rel.Member.Presence.Availability );
          
        }
    }

    [Button]
    public void ShowIncoming()
    {
        if (FriendsService.Instance.IncomingFriendRequests.Count == 0)
        {
            Debug.Log("NO HAY solicitudes entrantes");
            incoming.text = "No incoming requests";
            return;
        }

        foreach (Relationship rel in FriendsService.Instance.IncomingFriendRequests)
        {
            Debug.Log(rel.Member.Id + " ||" + rel.Member.Presence.Availability);
            incoming.text = $"Incoming: {rel.Member.Id}";
        }


    }
    [Button]
    public void ShowOutgoing()
    {
        foreach (Relationship rel in FriendsService.Instance.OutgoingFriendRequests)
        {
            Debug.Log(rel.Member.Id + " ||" + rel.Member.Presence.Availability);

            // SendFriendRequest(rel.Member.Id);
        }
    }
    [Button]
    public void ShowBlocks()
    {
        foreach (Relationship rel in FriendsService.Instance.Blocks)
        {
            Debug.Log(rel.Member.Id + " ||" + rel.Member.Presence.Availability);

           // UnblockUser(rel.Member.Id);
        }
    }

    #endregion
    #region Status

    [Button]
    public async void SetAvailability(Availability availability)
    {
        await FriendsService.Instance.SetPresenceAvailabilityAsync(availability);
        StatePlayer.text = availability.ToString();   
    }
    #region Example
    public class ActivitySample
    {
        public string state;
        public string rounds;

        public ActivitySample() { }
    }

    public void TestActivity()
    {
        ActivitySample activity = new();
        activity.state = "Gibraltar";
        activity.rounds = "8/13";
        SetActivity(activity);
    }
    #endregion

    public async void SetActivity<T>(T activity)where T : new()
    {
        await FriendsService.Instance.SetPresenceActivityAsync(activity);
    }

    public async void SetPrecense<T>(Availability availability , T activity) where T : new()
    {
        await FriendsService.Instance.SetPresenceAsync(availability, activity);
    }

    #endregion
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
    [Button]
    public async Task SendMessage(string targetID , string text)
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


    #endregion
    #region Events
    private void OnPresenceUpdated(IPresenceUpdatedEvent e)
    {
        Debug.Log("Se a actualizado es estado de" + e.ID + e.Presence.Availability);
    }

    private void OnMessageRecived(IMessageReceivedEvent e)
    {

        //e.GetAs<>
        

        LobbyInviteMessage invite = null;


        try
        {
            invite = e.GetAs<LobbyInviteMessage>();
        }
        catch 
        { 
            var msg = e.GetAs<SimpleMessage>();
            Debug.Log("he recibido un mensaje de " + e.UserId + "mensaje : " + msg.content);

        }
        if (invite == null) return;

        //-> LobbyJoinByCode(invite.JoinCode)

    }

    private void OnRelationshipDeleted(IRelationshipDeletedEvent e)
    {
        Debug.Log("Relationship added : " + e.Relationship.Id);
    }

    private void OnRelationshipAdded(IRelationshipAddedEvent e)
    {
        Debug.Log("Relationship added : " + e.Relationship.Id + e.Relationship.Type);
    }
    #endregion
}
