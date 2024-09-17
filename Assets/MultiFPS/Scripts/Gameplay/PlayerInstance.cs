using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MultiFPS.Gameplay.Gamemodes;
using MultiFPS.UI;
using System;
using System.Threading.Tasks;

namespace MultiFPS.Gameplay
{


    [System.Serializable]
    public struct ClientData
    {
        public string Username;
        public int CharacterSkinID;
        public int[] SelectedItemSkins;
        public int[] Lodout;
        public int CharacterModelId;
    }


    public class PlayerInstance : NetworkBehaviour {
        //this is all player data that is player preference, that he will himself send to us
        //or we would retrieve this data about him from his account if we had such system
        

        //all info that we receive from client himself, his nickname, selected cosmetics etc
        public ClientData PlayerInfo;
        public int Team { private set; get; } = -1; //-1 for no team

        //stats, scoreboard reads them
        public int Kills;
        public int Deaths;
        public int Assists;

        [HideInInspector] public bool SpawnCooldown = true;

        [HideInInspector] public CharacterInstance MyCharacter;
        [HideInInspector] public CharacterInstance CurrentlyControlledCharacter;

        #region bot related variables
        public bool BOT { private set; get; } = false;
        GameObject[] _itemsOnSpawn;
        #endregion

        public delegate void OnReceivedTeamResponse(int team, int permissionCode);
        public OnReceivedTeamResponse PlayerEvent_OnReceivedTeamResponse { set; get; }

        //counting for respawn
        Coroutine _spawnCooldownCoroutine;

        int _connectionToClient = -1;
        bool _registeredPlayerInstance = false; //only server code uses it

        bool _subbedToNetworkManager; //both client and server uses this flag
        bool _receivedPlayerInfo = false; 

        public short Latency;

        public bool AlwaysReinstantiateOnRespawn = false;

        private void Awake()
        {
            DNNetworkManager.Instance.OnNewPlayerConnected += UpdateDataForLatePlayer;
            _subbedToNetworkManager = true;
        }

        private void Start()
        {
            GameManager.AddPlayerInstance(this);
            if (isOwned)
            {
                ClientFrontend.ClientPlayerInstance = this;
                PlayerInfo = new ClientData
                {
                    Username = UserSettings.UserNickname,
                    CharacterSkinID = UserSettings.CharacterSkinID,
                    SelectedItemSkins = UserSettings.SelectedItemSkins,
                    Lodout = UserSettings.PlayerLodout,
                    CharacterModelId = UserSettings.SelectedCharacterModel
                };

                SendClientData(PlayerInfo);
                StartCoroutine(SendLatencyData());
            }

            //this connectionToClient check is to avoid sending TargetRPC to bots
            if (isServer && connectionToClient != null)
            {
                GameManager.Gamemode.Relay_NewClientJoined(connectionToClient, this.netIdentity);
                _connectionToClient = connectionToClient.connectionId;
                GameManager.RegisterConnectedPlayer(this);
            }
        }

        private void Update()
        {
            //if this playerinstance belongs to client then get latency value for him
            if (isOwned)
                Latency = (short)Math.Round(NetworkTime.rtt * 1000);

            if (!MyCharacter || (MyCharacter.Health.CurrentHealth <= 0))
            {
                if (isOwned)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        CmdProcessSpawnRequest();
                    }
                }
                else if (SpawnCooldown && BOT && isServer)
                {
                    Server_ProcessSpawnRequest();
                }
            }
        }


        #region latency
        IEnumerator SendLatencyData() 
        {
            while (true) 
            {
                CmdSendLatencyData(Latency);
                
                yield return new WaitForSeconds(2f);
            }
        }
        [Command]
        void CmdSendLatencyData(short latency) 
        {
            RpcReceiveLatencyData(latency);
        }
        [ClientRpc(includeOwner = false)]
        void RpcReceiveLatencyData(short latency) 
        {
            Latency = latency;
        }
        #endregion

        #region lodout
        [Command]
        public void CmdSendNewLodoutInfo(int[] loadoutInfo) 
        {
            PlayerInfo.Lodout = loadoutInfo;
        }
        #endregion


        public void RegisterPlayerInstance()
        {
            if (_registeredPlayerInstance) return;

            if (!isServer) return;

            GameManager.Gamemode.Server_WriteToChat($"<color=#{ColorUtility.ToHtmlStringRGBA(Color.cyan)}>" + PlayerInfo.Username + " joined the game </color>");

            if (BOT)
            {
                _receivedPlayerInfo = true;
                GameManager.Gamemode.Server_OnPlayerInstanceAdded(this);
            }

            _registeredPlayerInstance = true;
        }

        public void SetAsBot() 
        {
            PlayerInfo.CharacterSkinID = UnityEngine.Random.Range(0, ClientInterfaceManager.Instance.characterSkins.Length);
            BOT = true;
        }

        /// <summary>
        /// Normally character inventory is set based on player setting, but we can also force players to spawn with specific items
        /// This is useful for gamemodes where we want players to use one specyfic weapon, and this method is exactly for setting that
        /// </summary>
        public void SetItemsOnSpawn(GameObject[] itemsOnSpawn) 
        {
            _itemsOnSpawn = itemsOnSpawn;
        }

        #region spawn requests
        [Command]
        void CmdProcessSpawnRequest()
        {
            Server_ProcessSpawnRequest();
        }
        public bool Server_ProcessSpawnRequest()
        {
            if (!_receivedPlayerInfo) return false;

            if (Team != -1 && SpawnCooldown && GameManager.Gamemode.LetPlayersSpawnOnTheirOwn)
            {
                GameManager.Gamemode.PlayerSpawnCharacterRequest(this);
                return true;
            }

            return false;
        }

        public void Server_SpawnCharacter(Transform spawnPoint) 
        {
            if (Team == -1) return; //dont spawn character if we are not assigned to a team

            if (CurrentlyControlledCharacter != null && CurrentlyControlledCharacter != MyCharacter) 
            {
                CurrentlyControlledCharacter.SetAsBOT(true);
            }

            SpawnCooldown = false;
            if (_spawnCooldownCoroutine != null)
            {
                StopCoroutine(_spawnCooldownCoroutine);
                _spawnCooldownCoroutine = null;
            }

           
            if (MyCharacter == null || AlwaysReinstantiateOnRespawn)
            {
                //despawn previous character if it exist
                DespawnCharacterIfExist();
                MyCharacter = Instantiate(NetworkManager.singleton.spawnPrefabs[PlayerInfo.CharacterModelId], spawnPoint.position, Quaternion.identity).GetComponent<CharacterInstance>();

                MyCharacter.Health.Server_OnHealthDepleted += OnCharacterHealthDepleted;
                MyCharacter.Server_KilledCharacter += OnPlayerKilled;
                MyCharacter.Health.Server_OnEarnedAssist += OnEarnedAssist;
                MyCharacter.Health.Server_Resurrect += OnServerResurrect;

                NetworkServer.Spawn(MyCharacter.gameObject, connectionToClient);
            }
            else
            {
                MyCharacter.transform.position = spawnPoint.position;
                MyCharacter.Health.ServerResurrect();
            }

            MyCharacter.ServerTeleport(spawnPoint.position, spawnPoint.eulerAngles.y);

            MyCharacter.BlockCharacter(false);
            CurrentlyControlledCharacter = MyCharacter;

            //Assign player lodout, so appropriate items will be spawned
            CharacterItemManager characterItemManager = MyCharacter.GetComponent<CharacterItemManager>();

            //if this player intance is bot, then randomize his lodout every time he spawns, just for gameplay variety
            if (BOT && (PlayerInfo.Lodout == null || PlayerInfo.Lodout.Length <= 0)) 
            {
                List<int> randomItems = new List<int>();

                for (int i = 0; i < ItemManager.Instance.SlotsLodout.Length; i++)
                {
                    randomItems.Add(UnityEngine.Random.Range(0, ItemManager.Instance.SlotsLodout[i].availableItemsForSlot.Length));
                }
                PlayerInfo.Lodout = randomItems.ToArray();
            }

            //if special eq is require, ignore player preferences and set items
            if (_itemsOnSpawn != null && _itemsOnSpawn.Length > 0)
            {
                for (int i = 0; i < characterItemManager.Slots.Count; i++)
                {
                    if (_itemsOnSpawn.Length <= i)
                    {
                        characterItemManager.Slots[i].ItemOnSpawn = null; //we must also clear all default slots!
                        continue;
                    }
                    characterItemManager.Slots[i].ItemOnSpawn = _itemsOnSpawn[i] ? _itemsOnSpawn[i].gameObject : null;
                }
            }
            else
            {
                //if no special equipment is required just spawn character with items that player wants
                for (int i = 0; i < PlayerInfo.Lodout.Length; i++)
                {
                    if (PlayerInfo.Lodout[i] < 0) continue;

                    if (i >= characterItemManager.Slots.Count) break;

                    if (i >= ItemManager.Instance.SlotsLodout[i].availableItemsForSlot.Length) continue;

                    characterItemManager.Slots[i].ItemOnSpawn = ItemManager.Instance.SlotsLodout[i].availableItemsForSlot[PlayerInfo.Lodout[i]].gameObject;
                }
            }

            MyCharacter.Health.Team = Team;

            if (BOT)
                MyCharacter.SetAsBOT(BOT);

            WritePlayerData(PlayerInfo, Team, MyCharacter.netIdentity);

            RpcReceivePlayerData(PlayerInfo, MyCharacter.netIdentity, Team);

            if(!BOT)
                TargetRpcControlCharacter(connectionToClient, MyCharacter);
        }

        private void OnServerResurrect(int health)
        {
            if (!BOT)
                TargetRpcControlCharacter(connectionToClient, MyCharacter);
        }

        public void DespawnCharacterIfExist() 
        {
            if (MyCharacter) //despawn previous character
            {
                MyCharacter.CharacterItemManager.OnDespawnCharacter(); //when despawning character despawn also its equipment
                MyCharacter.Health.Server_OnHealthDepleted -= OnCharacterHealthDepleted;
                MyCharacter.Server_KilledCharacter -= OnPlayerKilled;
                MyCharacter.Health.Server_OnEarnedAssist -= OnEarnedAssist;
                MyCharacter.Health.Server_Resurrect -= OnServerResurrect;

                NetworkServer.Destroy(MyCharacter.gameObject);
            }
        }

        /// <summary>
        /// wait to be able to respawn
        /// </summary>
        public void Server_CountCooldown()
        {
            if (!GameManager.Gamemode.LetPlayersSpawnOnTheirOwn) return;

            SpawnCooldown = false;

            if (_spawnCooldownCoroutine != null)
            {
                StopCoroutine(_spawnCooldownCoroutine);
                _spawnCooldownCoroutine = null;
            }

            _spawnCooldownCoroutine = StartCoroutine(CountSpawnCooldown());
            IEnumerator CountSpawnCooldown()
            {
                yield return new WaitForSeconds(RoomSetup.Properties.P_RespawnCooldown);
                SpawnCooldown = true;

                if (GameManager.Gamemode.LetPlayersSpawnOnTheirOwn)
                    Server_ProcessSpawnRequest();
            }
        }
        #endregion


        void OnPlayerKilled(Health killedPlayer)
        {
            //If we killed someone, increment for us kill count if certain conditions are met, like killed player was actually our enemy, not ally
            Kills = ((killedPlayer.netId == MyCharacter.netId) || (killedPlayer.Team == MyCharacter.Health.Team && !GameManager.Gamemode.FFA)) ? Kills-1: Kills+1;

            //Send stats to all client so they will be able to see new stats on scoreboard
            Server_UpdateStatsForAllClients();

            //if it is bot, let him boast about kill he made
            if (BOT)
                GetComponent<ChatBehaviour>().RpcHandleChatClientMessage($"I killed {killedPlayer.CharacterName}!");
        }

        //if our character died, start counting respawn cooldown
        void OnCharacterHealthDepleted(CharacterPart damagedPart, AttackType attackType, Health attacker, int attackForce)
        {
            //count death for scoreboard
            Deaths++;
            Server_UpdateStatsForAllClients();
            Server_CountCooldown();
        }

        #region update stats
        public void Server_UpdateStatsForAllClients()
        {
            RpcUpdateStats(Kills, Deaths, Assists);
        }
        [ClientRpc]
        void RpcUpdateStats(int _kills, int _deaths, int assists)
        {
            if (isServer) return;
            Kills = _kills;
            Deaths = _deaths;
            Assists = assists;
        }
        #endregion

        #region SendPlayerData

        //sending client data
        void SendClientData(ClientData clientData)
        {
            CmdReceivePlayerData(clientData);
        }

        [Command]
        void CmdReceivePlayerData(ClientData clientData)
        {
            PlayerInfo = clientData;

            //is player did not set his nickmane, then set his nickname with some random number
            if (string.IsNullOrEmpty(PlayerInfo.Username))
                PlayerInfo.Username = $"Guest {UnityEngine.Random.Range(0, 10000)}";

            RpcReceivePlayerData(PlayerInfo, (MyCharacter? MyCharacter.netIdentity: null), Team);
            RegisterPlayerInstance();

            //tell everyone on chat that player joined
            //GameManager.Gamemode.Server_WriteToChat($"<color=#{ColorUtility.ToHtmlStringRGBA(Color.cyan)}>" + PlayerInfo.Username + " joined the game </color>");

            _receivedPlayerInfo = true;
            GameManager.Gamemode.Server_OnPlayerInstanceAdded(this);
        }

        /// <summary>
        /// After server received specific client data, resend this data to all other clients, so their game will know this
        /// player nickname, which skins to render for him etc
        /// </summary>
        [ClientRpc]
        void RpcReceivePlayerData(ClientData clientData, NetworkIdentity _charNetIdentity, int team)
        {
            if(!isServer)
                WritePlayerData(clientData, team, _charNetIdentity);
        }

        private void OnEarnedAssist(Health victimID)
        {
            Assists++;
            RpcUpdateStats(Kills, Deaths, Assists);
        }


        //update for late players
        void UpdateDataForLatePlayer(NetworkConnection conn)
        {
            TargetRpcReceiveDataForLatePlayer(conn, PlayerInfo, MyCharacter ? MyCharacter.netIdentity : null, Kills, Deaths, Team);
        }

        [TargetRpc]
        void TargetRpcReceiveDataForLatePlayer(NetworkConnection conn, ClientData clientData, NetworkIdentity charNetIdentity, int kills, int deaths, int team)
        {
            if (isServer) return;

            Team = team;
            Kills = kills;
            Deaths = deaths;

            WritePlayerData(clientData, team, charNetIdentity);
        }

        void WritePlayerData(ClientData clientData, int team, NetworkIdentity charNetIdentity)
        {
            PlayerInfo = clientData;
            Team = team;

            if (isOwned)
                ClientFrontend.SetClientTeam(team);

            //for inspector so we can better tell who is who
            gameObject.name = "Player: " + PlayerInfo.Username;

            if (charNetIdentity != null)
            {
                MyCharacter = charNetIdentity.GetComponent<CharacterInstance>();
                MyCharacter.Health.CharacterName = PlayerInfo.Username;
                MyCharacter.Health.Team = team;

                MyCharacter.gameObject.name = "Character: " + PlayerInfo.Username;
                MyCharacter.CharacterAnimator.ApplySkin(PlayerInfo.CharacterSkinID);

                MyCharacter._skinsForItems = PlayerInfo.SelectedItemSkins;

                GameManager.CharacterTeamAssigned(MyCharacter);
            }
        }
        #endregion

        #region Take control over bot
        void Client_TakeControlOverCharacter(CharacterInstance character) 
        {
            ClientFrontend.SetOwnedCharacter(character);
            ClientFrontend.SetObservedCharacter(character);

            PlayerGameplayInput.Instance.AssignCharacterToBeControlledByPlayer(character);
        }

        //launched by UI
        public void RequestControlOverPlayerCharacter(CharacterInstance character)
        {
            CmdRequestControlOverPlayerCharacter(character);
        }
        [Command]
        void CmdRequestControlOverPlayerCharacter(CharacterInstance character) 
        {
            if (!GameManager.Gamemode.LetPlayersTakeControlOverBots) return;

            if (!character.BOT) return;

            if (character.Health.Team != Team) return;

            if (character.Health.CurrentHealth <= 0) return;

            character.SetAsBOT(false);

            character.netIdentity.AssignClientAuthority(connectionToClient);

            for (int i = 0; i < character.CharacterItemManager.Slots.Count; i++)
            {
                if (character.CharacterItemManager.Slots[i].Item)
                    character.CharacterItemManager.Slots[i].Item.netIdentity.AssignClientAuthority(connectionToClient);
            }

            RpcRequestControlOverPlayerCharacter(character);
        }
        [ClientRpc]
        void RpcRequestControlOverPlayerCharacter(CharacterInstance characterToTakeControlOver) 
        {
            if(isOwned)
                Client_TakeControlOverCharacter(characterToTakeControlOver);

            //just set new nickame for character on every client so everyone will see on scoreboard and killfeed:
            //$"{BotName} + ({player name who took control over that bot})";
            characterToTakeControlOver.Health.CharacterName = characterToTakeControlOver.Health.CharacterName + $" ({PlayerInfo.Username})";
        }

        [TargetRpc]
        void TargetRpcControlCharacter(NetworkConnection conn, CharacterInstance charInstance) 
        {
            Client_TakeControlOverCharacter(charInstance);
        }
        #endregion

        #region joining teams
        //method for UI to launch it
        public void ClientRequestJoiningTeam(int team) 
        {
            ProcessClientRequestToJoinTeam(team);
        }

        //here we can check for example if team that client wants to join to is not full 
        [Command]
        public void ProcessClientRequestToJoinTeam(int team) 
        {
            ProcessRequestToJoinTeam(team);
        }
        public void ProcessRequestToJoinTeam(int team)
        {
            GameManager.Gamemode.PlayerRequestToJoinTeam(this, team);
        }

        [ClientRpc]
        public void RpcTeamJoiningResponse(int team, int permissionCode) {
            //for UI to subscribe to
            PlayerEvent_OnReceivedTeamResponse?.Invoke(team, permissionCode);

            if(permissionCode == 0 && isOwned)
                ClientFrontend.SetClientTeam(team);
        }
        public void SetTeam(int team) 
        {
            Team=team;
            RpcSetTeam(team);
        }
        [ClientRpc]
        void RpcSetTeam(int team) 
        {
            Team = team;
        }
        #endregion

        private void OnDestroy()
        {
            GameManager.RemovePlayerInstance(this);

            if (_subbedToNetworkManager)
                DNNetworkManager.Instance.OnNewPlayerConnected -= UpdateDataForLatePlayer;

            if (_registeredPlayerInstance)
            {
                GameManager.Gamemode.Server_OnPlayerInstanceRemoved(this);
            }

            if (_connectionToClient != -1) 
            {
                GameManager.Gamemode.Relay_ClientDisconnected(connectionToClient, this.netIdentity);
                GameManager.RemoveConnectedPlayer(_connectionToClient);
            }
        }
    }

}