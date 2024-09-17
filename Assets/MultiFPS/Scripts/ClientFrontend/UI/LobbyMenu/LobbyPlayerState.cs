using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiFPS.UI
{
    public class LobbyPlayerState : DNNetworkBehaviour
    {
        public long ClientId;
        public string PlayerName;
        public bool IsLeader;

        public LobbyPlayerState(long clientId, string playerName, bool isLeader)
        {
            ClientId = clientId;
            PlayerName = playerName;
            IsLeader = isLeader;
        }

    }
}