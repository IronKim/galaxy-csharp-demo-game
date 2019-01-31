﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Galaxy.Api;

public class Matchmaking : MonoBehaviour
{

    #region Variables

    public List<GalaxyID> lobbyList = new List<GalaxyID>();

    // Variables for storing lobbies data
    private GalaxyID currentLobbyID = null;
    public GalaxyID CurrentLobbyID { get { return currentLobbyID; } set { currentLobbyID = value; } }

    // Variables for storing lobby members data
    public string lobbyName = "";
    static GalaxyID lobbyOwnerID;
    public GalaxyID LobbyOwnerID { get { return lobbyOwnerID; } set { lobbyOwnerID = value; } }

    #endregion

    #region Behaviours

    void OnEnable()
    {
        // Make sure that all listeners are properly disposed when Matchmaking is opened.
        LobbyBrowsingListenersDispose();
        LobbyCreationListenersDispose();
        LobbyChatListenersDispose();
        LobbyManagmentMainMenuListenersDispose();
        LobbyManagmentInGameDispose();
    }

    void OnDisable()
    {
        // Make sure that all listeners are properly disposed when Matchmaking is closed.
        LobbyBrowsingListenersDispose();
        LobbyCreationListenersDispose();
        LobbyChatListenersDispose();
        LobbyManagmentMainMenuListenersDispose();
        LobbyManagmentInGameDispose();
    }

    #endregion

    #region Methods

    /* Requests list of available lobbies
    Note: Private lobbies will not be retrieved */
    public void RequestLobbyList(bool allowFull = false)
    {
        Debug.Log("Requesting lobby list");
        try
        {
            Debug.Assert(lobbyListListenerBrowsing != null);
            GalaxyInstance.Matchmaking().RequestLobbyList(allowFull, lobbyListListenerBrowsing);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not retrieve lobby list for reason: " + e);
        }
    }

    /* Requests lobby data for provided lobbyID 
    Note: Must be called before getting any lobby data, 
    unless you are a member of lobby your trying to get data for (you joined said lobby) */
    public void RequestLobbyData(GalaxyID lobbyID)
    {
        Debug.Log("Requesting data for lobby " + lobbyID);
        try
        {
            Debug.Assert(lobbyDataRetrieveListenerBrowsing != null);
            GalaxyInstance.Matchmaking().RequestLobbyData(lobbyID, lobbyDataRetrieveListenerBrowsing);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not retrieve lobby " + lobbyID + " data for reason: " + e);
        }
    }

    // Joins a specified lobby
    public void JoinLobby(GalaxyID lobbyID)
    {
        Debug.Log("Joining lobby " + lobbyID);
        try
        {
            Debug.Assert(lobbyEnteredListenerBrowsing != null);
            GalaxyInstance.Matchmaking().JoinLobby(lobbyID, lobbyEnteredListenerBrowsing);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not join lobby " + lobbyID + " for reason: " + e);
        }
    }

    // Creates a lobby with specified parameters
    public void CreateLobby(string gameName, LobbyType lobbyType, uint maxMembers, bool joinable, LobbyTopologyType lobbyTopologyType)
    {
        Debug.Log("Creating a lobby");
        try
        {
            lobbyName = gameName;
            Debug.Assert(lobbyCreatedListenerCreation != null);
            GalaxyInstance.Matchmaking().CreateLobby(lobbyType, maxMembers, joinable, lobbyTopologyType, lobbyCreatedListenerCreation);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not create lobby for reason: " + e);
        }
    }

    // Leaves currently entered lobby
    public void LeaveLobby()
    {
        Debug.Log("Leaving lobby " + currentLobbyID);
        try
        {
            Debug.Assert(currentLobbyID != null);
            GalaxyInstance.Matchmaking().LeaveLobby(currentLobbyID);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not leave lobby " + currentLobbyID + " for reason: " + e);
        }
    }

    // Sets lobby 'lobbyID' data 'key' to 'value'
    public void SetLobbyData(GalaxyID lobbyID, string key, string value)
    {
        Debug.Log("Trying to set lobby " + lobbyID + " data " + key + " to " + value);
        try
        {
            GalaxyInstance.Matchmaking().SetLobbyData(lobbyID, key, value);
            Debug.Log("Lobby " + lobbyID + " data " + key + " set to " + value);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not set lobby " + lobbyID + " data " + key + " to " + value + " for reason: " + e);
        }
    }

    // Gets the GalaxyID of the lobby owner
    public GalaxyID GetLobbyOwner(GalaxyID lobbyID)
    {
        GalaxyID lobbyOwnerID = null;
        Debug.Log("Trying to get lobby owner id " + lobbyOwnerID);
        try
        {
            lobbyOwnerID = GalaxyInstance.Matchmaking().GetLobbyOwner(lobbyID);
            Debug.Log("Lobby onwer ID received " + lobbyOwnerID);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not retrieve lobby " + lobbyID + " owner ID for reason: " + e);
        }
        return lobbyOwnerID;
    }

    // Gets lobby 'lobbyID' data 'key'
    public string GetLobbyData(GalaxyID lobbyID, string key)
    {
        string lobbyData = null;
        try
        {
            lobbyData = GalaxyInstance.Matchmaking().GetLobbyData(lobbyID, key);
            Debug.Log("Lobby " + lobbyID + " data " + key + " read " + lobbyData);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not retrieve lobby " + lobbyID + " data " + key + " for reason: " + e);
        }
        return lobbyData;
    }

    // Gets lobby 'lobbyID' member GalaxyID by 'index'
    public GalaxyID GetLobbyMemberByIndex(GalaxyID lobbyID, uint index)
    {
        Debug.Log("Getting lobby member " + index + " GalaxyID");
        GalaxyID memberID = null;
        try
        {
            memberID = GalaxyInstance.Matchmaking().GetLobbyMemberByIndex(lobbyID, index);
            Debug.Log("Lobby member " + index + " GalaxyID " + memberID);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not get lobby member " + index + " GalaxyID for reason: " + e);
        }
        return memberID;
    }

    // Sets current user lobby member data 'key' to 'value' in currently entered lobby
    public void SetLobbyMemberData(string key, string value)
    {
        Debug.Log("Trying to set lobby " + currentLobbyID + " member data " + key + " to " + value);
        try
        {
            GalaxyInstance.Matchmaking().SetLobbyMemberData(currentLobbyID, key, value);
            Debug.Log("Lobby " + currentLobbyID + " member data " + key + " set to " + value);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not set lobby " + currentLobbyID + " member data " + key + " : " + value + " for reason " + e);
        }
    }

    // Gets data 'key' of specified member 'memberID' in currently entered lobby
    public string GetLobbyMemberData(GalaxyID memberID, string key)
    {
        Debug.Log("Trying to get lobby " + currentLobbyID + " member " + memberID + " data " + key);
        string memberData = "";
        try
        {
            memberData = GalaxyInstance.Matchmaking().GetLobbyMemberData(currentLobbyID, memberID, key);
            Debug.Log("Lobby " + currentLobbyID + " member " + memberID + " data " + key + " read " + memberData);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not read lobby " + currentLobbyID + " member " + memberID + " data " + key + " for reason " + e);
        }
        return memberData;
    }

    // Gets GalaxyID of the second player in currently entered lobby
    public GalaxyID GetSecondPlayerID()
    {
        Debug.Log("Trying to get second player ID");
        GalaxyID secondPlayerID = null;
        List<GalaxyID> membersList = GetAllLobbyMembers();
        Debug.Assert(membersList.Count == 2);
        if (membersList[0] != GalaxyInstance.User().GetGalaxyID())
        {
            secondPlayerID = membersList[0];
        }
        else secondPlayerID = membersList[1];
        Debug.Log("Second player ID " + secondPlayerID);
        return secondPlayerID;
    }

    /* Gets ping of a user or lobby 'galaxyID'
    Note: To get a ping of a lobby or its member you first have to either
    join it or request said lobby data */
    public int GetPingWith(GalaxyID galaxyID)
    {
        int ping = -1;
        try
        {
            ping = GalaxyInstance.Networking().GetPingWith(galaxyID);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.Log(e);
        }
        return ping;
    }

    // Gets a number of lobby members currently in the lobby 'lobbyID'
    public uint GetNumLobbyMembers(GalaxyID lobbyID)
    {
        uint lobbyMembersCount = 0;
        try
        {
            lobbyMembersCount = GalaxyInstance.Matchmaking().GetNumLobbyMembers(lobbyID);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not get lobby member count for reason " + e);
        }
        return lobbyMembersCount;
    }

    // Gets a list of all lobby members GalaxyIDs
    private List<GalaxyID> GetAllLobbyMembers()
    {
        Debug.Log("Trying to get all lobby members for lobby: " + currentLobbyID);
        List<GalaxyID> membersList = new List<GalaxyID>();
        uint maxMembers = GetNumLobbyMembers(currentLobbyID);
        for (uint i = 0; i < maxMembers; i++)
        {
            membersList.Add(GetLobbyMemberByIndex(currentLobbyID, i));
        }
        return membersList;
    }

    // Shows overlay dialogs for game invites to the currently entered lobby
    public void ShowOverlayInviteDialog()
    {
        string connectionString = "--JoinLobby=" + currentLobbyID.ToString();
        Debug.Log("Trying to open overlay invite dialog");
        try
        {
            GalaxyInstance.Friends().ShowOverlayInviteDialog(connectionString);
            Debug.Log("Showing Galaxy overlay invite dialog");
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not show Galaxy overlay invite dialog for reason: " + e);
        }
    }

    // Sends game invitation to user for the currently entered lobby 
    public void SendInvitation(GalaxyID userID)
    {
        string connectionString = "--JoinLobby=" + currentLobbyID.ToString();
        Debug.Log("Trying to send invitation to " + userID + " Connection string: " + connectionString);
        try
        {
            GalaxyInstance.Friends().SendInvitation(userID, connectionString);
            Debug.Log("Sent invitation to: " + userID + " Connection string: " + connectionString);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not send invitation to: " + userID + " Connection string: " + connectionString + " for reason: " + e);
        }
    }

    // Sends lobby message
    public void SendLobbyMessage(GalaxyID lobbyID, string message)
    {
        bool messageScheduled;
        try
        {
            messageScheduled = GalaxyInstance.Matchmaking().SendLobbyMessage(lobbyID, message);
            if (!messageScheduled) Debug.LogWarning("Message not scheduled for sending");
            else Debug.Log("Message '" + message + "' was scheduled for sending");
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning(e);
        }
    }
    // Gets a lobby message
    public string GetLobbyMessage(GalaxyID lobbyID, ref GalaxyID senderID, uint messageID)
    {
        string message = null;
        GalaxyInstance.Matchmaking().GetLobbyMessage(lobbyID, messageID, ref senderID, out message);
        return message;
    }

    #endregion

    #region Lobby browsing specific listeners

    public void LobbyBrowsingListenersInit()
    {
        if (lobbyListListenerBrowsing == null) lobbyListListenerBrowsing = new LobbyListListenerBrowsing();
        if (lobbyDataRetrieveListenerBrowsing == null) lobbyDataRetrieveListenerBrowsing = new LobbyDataRetrieveListenerBrowsing();
        if (lobbyEnteredListenerBrowsing == null) lobbyEnteredListenerBrowsing = new LobbyEnteredListenerBrowsing();
    }

    public void LobbyBrowsingListenersDispose()
    {
        if (lobbyListListenerBrowsing != null) lobbyListListenerBrowsing.Dispose();
        if (lobbyDataRetrieveListenerBrowsing != null) lobbyDataRetrieveListenerBrowsing.Dispose();
        if (lobbyEnteredListenerBrowsing != null) lobbyEnteredListenerBrowsing.Dispose();
    }

    /* Informs about the event of retrieving the list of available lobbies
    Callback to methods:
    Matchmaking.RequestLobbyList(bool allowFull = false) */
    private LobbyListListenerBrowsing lobbyListListenerBrowsing;
    private class LobbyListListenerBrowsing : ILobbyListListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;

        public override void OnLobbyList(uint count, LobbyListResult result)
        {
            if (result == LobbyListResult.LOBBY_LIST_RESULT_SUCCESS)
            {
                LobbyListSuccess(count);
            }
            else {
                LobbyListFailure(result);
            }
        }

        private void LobbyListSuccess(uint count)
        {
            Debug.Log(count + " lobbies OnLobbyList");
            if (count == 0)
            {
                GameObject.Find("OnlineBrowserScreen").GetComponent<OnlineBrowserController>().DisplayLobbyList(matchmaking.lobbyList);
            }
            else
            {
                for (uint i = 0; i < count; i++)
                {
                    GalaxyID lobbyID = GalaxyInstance.Matchmaking().GetLobbyByIndex(i);
                    matchmaking.lobbyList.Add(lobbyID);
                    Debug.Log("Requesting lobby data for lobby " + i + " with lobbyID " + lobbyID.ToString());
                    matchmaking.RequestLobbyData(lobbyID);
                }
            }
        }

        private void LobbyListFailure(LobbyListResult result)
        {
            Debug.LogWarning("OnLobbyList failure reason: " + result);
        }

    }

    /* Informs about the event of receiving specified lobby or lobby member data
    Callback to methods:
    Matchmaking.RequestLobbyData(GalaxyID lobbyID)
    Matchmaking.SetLobbyData(GalaxyID lobbyID, string key, string value)
    Matchmaking.SetLobbyMemberData(string key, string value) */
    private LobbyDataRetrieveListenerBrowsing lobbyDataRetrieveListenerBrowsing;
    private class LobbyDataRetrieveListenerBrowsing : ILobbyDataRetrieveListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        public int lobbiesWithDataRetrievedCount = 0;

        public override void OnLobbyDataRetrieveSuccess(GalaxyID lobbyID)
        {
            LobbyDataRetrieveSuccess();
        }

        public override void OnLobbyDataRetrieveFailure(GalaxyID lobbyID, FailureReason failureReason)
        {
            LobbyDataRetrieveFailure(failureReason);
        }

        private void LobbyDataRetrieveSuccess()
        {
            Debug.Log("Data retrieved for " + lobbiesWithDataRetrievedCount + " lobbies out of " + matchmaking.lobbyList.Count);
            lobbiesWithDataRetrievedCount ++;
            
            if (lobbiesWithDataRetrievedCount >= matchmaking.lobbyList.Count)
            {
                GameObject.Find("OnlineBrowserScreen").GetComponent<OnlineBrowserController>().DisplayLobbyList(matchmaking.lobbyList);
                lobbiesWithDataRetrievedCount = 0;
                matchmaking.lobbyList.Clear();
                matchmaking.lobbyList.TrimExcess();
            }
        }

        private void LobbyDataRetrieveFailure(FailureReason failureReason)
        {
            Debug.LogWarning("LobbyDataRetrieveListenerBrowsing failure reason: " + failureReason);
        }

    }

    /* Informs about the event of entering a lobby
    Callback for methods:
    Matchmaking.JoinLobby(GalaxyID lobbyID) */
    private LobbyEnteredListenerBrowsing lobbyEnteredListenerBrowsing;
    private class LobbyEnteredListenerBrowsing : ILobbyEnteredListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;

        public override void OnLobbyEntered(GalaxyID lobbyID, LobbyEnterResult _result)
        {
            switch (_result)
            {
                case LobbyEnterResult.LOBBY_ENTER_RESULT_SUCCESS:
                    LobbyEnteredSuccess(lobbyID);
                break;
                case LobbyEnterResult.LOBBY_ENTER_RESULT_LOBBY_DOES_NOT_EXIST:
                    LobbyEnteredError("Lobby does not exist");
                break;
                case LobbyEnterResult.LOBBY_ENTER_RESULT_LOBBY_IS_FULL:
                    LobbyEnteredError("Lobby is full");
                break;
                case LobbyEnterResult.LOBBY_ENTER_RESULT_ERROR:
                    LobbyEnteredError("Unspecified error");
                break;
            }
        }

        private void LobbyEnteredSuccess(GalaxyID lobbyID)
        {
            matchmaking.CurrentLobbyID = lobbyID;
            matchmaking.LobbyOwnerID = matchmaking.GetLobbyOwner(lobbyID);
            matchmaking.SetLobbyMemberData("state", "notReady");
            GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.OnlineWait);
        }

        private void LobbyEnteredError(string reason)
        {
            GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.OnlineBrowser);
            GameObject.Find("PopUps").GetComponent<PopUps>().MenuCouldNotJoin(reason);
        }

    }

    #endregion

    #region Lobby creation specific listeners

    public void LobbyCreationListenersInit()
    {
        if (lobbyCreatedListenerCreation == null) lobbyCreatedListenerCreation = new LobbyCreatedListenerCreation();
    }

    public void LobbyCreationListenersDispose()
    {
        if (lobbyCreatedListenerCreation != null) lobbyCreatedListenerCreation.Dispose();
    }

    /* Informs about the event of creating a lobby
    Callback to methods:
    Matchmaking.CreateLobby(LobbyType lobbyType, uint maxMembers, bool joinable, LobbyTopologyType lobbyTopologyType) */
    private LobbyCreatedListenerCreation lobbyCreatedListenerCreation;
    private class LobbyCreatedListenerCreation : ILobbyCreatedListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;

        public override void OnLobbyCreated(GalaxyID lobbyID, LobbyCreateResult _result)
        {
            switch (_result)
            {
                case LobbyCreateResult.LOBBY_CREATE_RESULT_SUCCESS:
                    LobbyCreated(lobbyID);
                break;
                case LobbyCreateResult.LOBBY_CREATE_RESULT_ERROR:
                    Timeout();
                break;
            }
        }

        private void LobbyCreated(GalaxyID lobbyID)
        {
            matchmaking.SetLobbyData(lobbyID, "name", matchmaking.lobbyName);
            matchmaking.SetLobbyData(lobbyID, "state", "notReady");
            matchmaking.CurrentLobbyID = lobbyID;
            matchmaking.LobbyOwnerID = GalaxyInstance.Matchmaking().GetLobbyOwner(lobbyID);
            matchmaking.SetLobbyMemberData("state", "notReady");
            GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.OnlineWait);
            Debug.Log("LobbyCreatedListenerCreation finished");
        }

        private void Timeout()
        {
            GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.OnlineCreate);
            GameObject.Find("PopUps").GetComponent<PopUps>().MenuCouldNotCreate();
            Debug.Log("LobbyCreatedListenerCreation finished");
        }

    }

    #endregion

    #region Lobby managment in game global listeners

    public void LobbyManagmentInGameListenersInit() 
    {
        if (lobbyDataListenerInGame == null) lobbyDataListenerInGame = new LobbyDataListenerInGame();
        if (lobbyLeftListenerInGame == null) lobbyLeftListenerInGame = new LobbyLeftListenerInGame();
        if (lobbyMemberStateListenerInGame == null) lobbyMemberStateListenerInGame = new LobbyMemberStateListenerInGame();
    }

    public void LobbyManagmentInGameDispose()
    {
        if (lobbyDataListenerInGame != null) lobbyDataListenerInGame.Dispose();
        if (lobbyLeftListenerInGame != null) lobbyLeftListenerInGame.Dispose();
        if (lobbyMemberStateListenerInGame != null) lobbyMemberStateListenerInGame.Dispose();
    }

    /* Informs about the event of receiving specified lobby or lobby member data
    Callback to methods:
    Matchmaking.RequestLobbyData(GalaxyID lobbyID)
    Matchmaking.SetLobbyData(GalaxyID lobbyID, string key, string value)
    Matchmaking.SetLobbyMemberData(string key, string value) */
    private LobbyDataListenerInGame lobbyDataListenerInGame;
    private class LobbyDataListenerInGame : GlobalLobbyDataListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;

        public override void OnLobbyDataUpdated(GalaxyID lobbyID, GalaxyID memberID)
        {
            Debug.Log("LobbyID: " + lobbyID + "\nMemberID: " + memberID);
            if (memberID != new GalaxyID(0)) LobbyMemberDataUpdated(lobbyID, memberID);
        }

        void LobbyMemberDataUpdated(GalaxyID lobbyID, GalaxyID memberID)
        {
            if (AllMembersGo(lobbyID, memberID))
            {
                matchmaking.SetLobbyData(lobbyID, "state", "go");
                GameObject.Find("GameManager").GetComponent<Online2PlayerGameManager>().enabled = true;
                GameObject.Find("PopUps").GetComponent<PopUps>().ClosePopUps();
            }
        }

        bool AllMembersGo(GalaxyID lobbyID, GalaxyID memberID)
        {
            uint go = 0;

            // Check how many players are in game
            for (uint i = 0; i < 2; i++)
            {
                if (matchmaking.GetLobbyMemberData(matchmaking.GetLobbyMemberByIndex(lobbyID, i), "state") == "go")
                {
                    go++;
                }
            }

            return (go == 2) ? true : false;

        }

    }

    /* Informs about the event of leaving a lobby 
    Callback for methods: 
    Matchmaking.LeaveLobby() */
    private LobbyLeftListenerInGame lobbyLeftListenerInGame;
    private class LobbyLeftListenerInGame : GlobalLobbyLeftListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        public override void OnLobbyLeft(GalaxyID lobbyID, LobbyLeaveReason leaveReason)
        {
            switch(leaveReason)
            {
                case LobbyLeaveReason.LOBBY_LEAVE_REASON_USER_LEFT:
                    LobbyLeft(lobbyID);
                break;
                case LobbyLeaveReason.LOBBY_LEAVE_REASON_LOBBY_CLOSED:
                    if (!GameObject.Find("Online2PlayerGameEnd")) OtherPlayerLeftLobby();
                break;
                case LobbyLeaveReason.LOBBY_LEAVE_REASON_CONNECTION_LOST:
                    if (!GameObject.Find("Online2PlayerGameEnd")) OtherPlayerLeftLobby();
                break;
                default:
                    Debug.LogWarning("OnLobbyLeft failure");
                break;
            }
        }

        void OtherPlayerLeftLobby()
        {
            Debug.Log("Other player left the lobby");
            GameObject.Find("PopUps").GetComponent<PopUps>().ClosePopUps();
            GameObject.Find("PopUps").GetComponent<PopUps>().GameHostLeftLobby();
            GameManager.Instance.GameFinished = true;
        }

        void LobbyLeft(GalaxyID lobbyID)
        {
            matchmaking.CurrentLobbyID = null;
            matchmaking.LobbyOwnerID = null;
            GalaxyManager.Instance.ShutdownNetworking();
            matchmaking.LobbyManagmentInGameDispose();
            Debug.Log("Lobby " + lobbyID + " left");
        }

    }

    /* Informs about the event of lobby member state change */
    private LobbyMemberStateListenerInGame lobbyMemberStateListenerInGame;
    private class LobbyMemberStateListenerInGame : GlobalLobbyMemberStateListener
    {
        public override void OnLobbyMemberStateChanged(GalaxyID lobbyID, GalaxyID memberID, LobbyMemberStateChange memberStateChange)
        {
            Debug.Log(string.Format("OnLobbyMemberStateChanged lobbyID: {0} memberID: {1} change: {2}", lobbyID, memberID, memberStateChange));
            if (memberStateChange != LobbyMemberStateChange.LOBBY_MEMBER_STATE_CHANGED_ENTERED)
            {
                if (!GameObject.Find("Online2PlayerGameEnd")) ClientLeftLobby();
            }
        }

        private void ClientLeftLobby()
        {
            Debug.Log("Client left the lobby.");
            GameObject.Find("PopUps").GetComponent<PopUps>().ClosePopUps();
            GameObject.Find("PopUps").GetComponent<PopUps>().GameClientLeftLobby();
            GameManager.Instance.GameFinished = true;
        }

    }

    #endregion

    #region Lobby managment main menu global listeners

    public void LobbyManagmentMainMenuListenersInit() 
    {
        if (lobbyLeftListenerMainMenu == null) lobbyLeftListenerMainMenu = new LobbyLeftListenerMainMenu();
        if (lobbyDataListenerMainMenu == null) lobbyDataListenerMainMenu = new LobbyDataListenerMainMenu();
    }

    public void LobbyManagmentMainMenuListenersDispose()
    {
        if (lobbyLeftListenerMainMenu != null) lobbyLeftListenerMainMenu.Dispose();
        if (lobbyDataListenerMainMenu != null) lobbyDataListenerMainMenu.Dispose();
    }

    /* Informs about the event of receiving specified lobby or lobby member data
    Callback to methods:
    Matchmaking.RequestLobbyData(GalaxyID lobbyID)
    Matchmaking.SetLobbyData(GalaxyID lobbyID, string key, string value)
    Matchmaking.SetLobbyMemberData(string key, string value) */
    private LobbyDataListenerMainMenu lobbyDataListenerMainMenu;
    private class LobbyDataListenerMainMenu : GlobalLobbyDataListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;

        public override void OnLobbyDataUpdated(GalaxyID lobbyID, GalaxyID memberID)
        {
            Debug.Log("LobbyID: " + lobbyID + "\nMemberID: " + memberID);
            if (memberID == new GalaxyID(0))
            {
                LobbyDataUpdatedReady(lobbyID);
                LobbyDataUpdatedSteady(lobbyID);
            }
            else
            {
                LobbyMemberDataUpdatedReady(lobbyID);
            }
        }

        void LobbyDataUpdatedReady(GalaxyID lobbyID)
        {
            if (matchmaking.GetLobbyData(lobbyID, "state") == "ready" && GalaxyManager.Instance.MyGalaxyID == matchmaking.LobbyOwnerID)
            {
                GameObject.Find("OnlineWaitScreen").GetComponent<OnlineWaitController>().startGameButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                GameObject.Find("OnlineWaitScreen").GetComponent<OnlineWaitController>().startGameButton.GetComponent<Button>().interactable = false;
            }
        }

        void LobbyDataUpdatedSteady(GalaxyID lobbyID)
        {
            if (matchmaking.GetLobbyData(lobbyID, "state") == "steady")
            {
                Debug.Assert(matchmaking.GetLobbyMemberData(GalaxyManager.Instance.MyGalaxyID, "state") == "ready");
                matchmaking.SetLobbyMemberData("state", "steady");
                SceneController.Instance.LoadScene(SceneController.SceneName.Online2PlayerGame, true);
            }
        }

        void LobbyMemberDataUpdatedReady(GalaxyID lobbyID)
        {
            if (GalaxyManager.Instance.MyGalaxyID == matchmaking.LobbyOwnerID)
            {
                matchmaking.SetLobbyData(lobbyID, "state", AllMembersReady(lobbyID) ? "ready" : "notReady");
            }
        }

        bool AllMembersReady(GalaxyID lobbyID)
        {
            uint ready = 0;
            uint lobbyMembersCount = matchmaking.GetNumLobbyMembers(lobbyID);
            // Checks how many players are ready
            for (uint i = 0; i < lobbyMembersCount; i++)
            {
                if (matchmaking.GetLobbyMemberData(matchmaking.GetLobbyMemberByIndex(lobbyID, i), "state") == "ready")
                {
                    ready++;
                }
            }
            return (ready == 2) ? true : false;
        }
    }

    /* Informs about the event of leaving a lobby 
    Callback for methods: 
    Matchmaking.LeaveLobby() */
    private LobbyLeftListenerMainMenu lobbyLeftListenerMainMenu;
    private class LobbyLeftListenerMainMenu : GlobalLobbyLeftListener
    {
        Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        public override void OnLobbyLeft(GalaxyID lobbyID, LobbyLeaveReason leaveReason)
        {
            switch(leaveReason)
            {
                case LobbyLeaveReason.LOBBY_LEAVE_REASON_USER_LEFT:
                    LobbyLeft(lobbyID);
                break;
                case LobbyLeaveReason.LOBBY_LEAVE_REASON_LOBBY_CLOSED:
                    HostLeftLobby();
                break;
                case LobbyLeaveReason.LOBBY_LEAVE_REASON_CONNECTION_LOST:
                    HostLeftLobby();
                break;
                default:
                    Debug.LogWarning("OnLobbyLeft failure");
                break;
            }
        }

        void HostLeftLobby()
        {
            Debug.Log("Host left the lobby");
            GameObject.Find("PopUps").GetComponent<PopUps>().ClosePopUps();
            GameObject.Find("PopUps").GetComponent<PopUps>().MenuHostLeftLobby();
            GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.Online);
        }

        void LobbyLeft(GalaxyID lobbyID)
        {
            matchmaking.CurrentLobbyID = null;
            matchmaking.LobbyOwnerID = null;
            Debug.Log("Lobby " + lobbyID + " left");
            GameObject.Find("MainMenu").GetComponent<MainMenuController>().SwitchMenu(MainMenuController.MenuEnum.Online);
        }

    }

    #endregion

    #region Lobby chat global listeners

    public void LobbyChatListenersInit() {
        if (chatLobbyMessageListener == null) chatLobbyMessageListener = new LobbyMessageListenerChat();
    }

    public void LobbyChatListenersDispose() {
        if (chatLobbyMessageListener != null) chatLobbyMessageListener.Dispose();
    }

	// Lobby message listener
    public LobbyMessageListenerChat chatLobbyMessageListener;
    public class LobbyMessageListenerChat : GlobalLobbyMessageListener
    {
        public List<Dictionary<string, string>> chatLobbyMessageHistory = new List<Dictionary<string, string>>();
        private Matchmaking matchmaking = GalaxyManager.Instance.Matchmaking;
        string message = null;

        public override void OnLobbyMessageReceived(GalaxyID lobbyID, GalaxyID senderID, uint messageID, uint messageLength)
        {
            Dictionary<string, string> messageAndSenderDict = new Dictionary<string, string>();
            try
            {
                Debug.Log("Lobby " + lobbyID + " Sender " + senderID + " message " + message);
                message = matchmaking.GetLobbyMessage(GalaxyManager.Instance.Matchmaking.CurrentLobbyID, ref senderID, messageID);
                messageAndSenderDict.Add("sender", GalaxyManager.Instance.Friends.GetFriendPersonaName(senderID));
                messageAndSenderDict.Add("message", message);
                chatLobbyMessageHistory.Add(messageAndSenderDict);
                Debug.Log("New message from " + GalaxyManager.Instance.Friends.GetFriendPersonaName(senderID) + " to lobbyID " + lobbyID + ": " + message);
                if (GameManager.Instance != null) ((Online2PlayerGameManager)GameManager.Instance).PopChatPrompt();
            }
            catch (GalaxyInstance.Error e)
            {
                Debug.LogWarning(e);
            }
        }
    }

    #endregion

}