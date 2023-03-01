using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

namespace AlphaLobby
{
    public class AlphaLobby : MonoBehaviour
    {
        #region Lobby State
        
        [SerializeField]
        [Tooltip("Armazena o nome de tela deste usuário")]
        private string _username;
        public string Username
        {
            get { return _username; }
        }

        #endregion

        public void UpdateUsername(string s)
        {
            _username = s;
        }

        protected async void Start()
        {
            // Instance = this;
            await UnityServices.InitializeAsync();
        }
    }
}
