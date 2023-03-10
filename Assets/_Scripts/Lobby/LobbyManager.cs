using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using AlphaLobby;

namespace AlphaLobby.Managers
{
    public class LobbyManager : MonoBehaviour
    {
        // #region LobbyManager Singleton Constructor Definition
        // private static LobbyManager _lobbyManagerSingleton;
        // public static LobbyManager Instance
        // {
        //     get
        //     {
        //         if (_lobbyManagerSingleton == null)
        //             _lobbyManagerSingleton = new LobbyManager();
        //         return _lobbyManagerSingleton;
        //     }
        //     private set { _lobbyManagerSingleton = Instance; }
        // }

        // private LobbyManager() { }
        // #endregion

        [SerializeField]
        private Lobby _hostedLobby;

        [SerializeField]
        private Lobby _joinedLobby;

        private List<Lobby> _availableLobbies;

        private float heartbeatTimer = 0f;

        public enum LobbyAvailability
        {
            Public = 0,
            Private = 1
        };

        private void Update()
        {
            HandleLobbyHeartbeat();
        }

        #region LobbyCreation
        public void CreateLobby()
        {
            CreateLobby("default", 4);
        }

        public void CreateLobby(string lobbyName, int maxPlayers)
        {
            CreateLobby(lobbyName, maxPlayers, null);
        }

        public void CreateLobby(
            string lobbyName,
            int maxPlayers,
            Dictionary<string, DataObject> data
        )
        {
            CreateLobby(lobbyName, maxPlayers, LobbyAvailability.Public, data);
        }

        public async void CreateLobby(
            string lobbyName,
            int maxPlayers,
            LobbyAvailability lobbyAvailability,
            Dictionary<string, DataObject> data
        )
        {
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate =
                (lobbyAvailability & LobbyAvailability.Private) == LobbyAvailability.Private;
            options.Player = new Player(AuthenticationService.Instance.PlayerId);
            options.Data = data;
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(
                lobbyName,
                maxPlayers,
                options
            );
            _joinedLobby = lobby;
            _hostedLobby = _joinedLobby;
        }

        public async void ListLobbies()
        {
            List<Lobby> lobbies;
            try
            {
                lobbies = (await LobbyService.Instance.QueryLobbiesAsync()).Results;
                this._availableLobbies = lobbies;
                foreach (Lobby lobby in lobbies)
                {
                    Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }
        }

        private async void HandleLobbyHeartbeat()
        {
            if (heartbeatTimer < 0)
            {
                if (_hostedLobby != null)
                {
                    await LobbyService.Instance.GetLobbyAsync(_hostedLobby.Id);
                }
                float heartbeatTimerMax = 15 * 60f;
                heartbeatTimer = heartbeatTimerMax;
            }
            else
            {
                heartbeatTimer -= Time.deltaTime;
            }
        }
        #endregion

        #region LobbyUpdate

        public async void UpdateLobby()
        {
            UpdateLobbyOptions options = new UpdateLobbyOptions();
            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(_hostedLobby.Id, options);
        }
        #endregion
    };
}
