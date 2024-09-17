using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MultiFPS.UI;
using MultiFPS.UI.HUD;
using static MultiFPS.Gameplay.Gamemodes.Gamemode;

namespace MultiFPS.Gameplay.Gamemodes
{
    [RequireComponent(typeof(RoomManager))]
    public class Gamemode : DNNetworkBehaviour
    {
        protected GamemodeState State = GamemodeState.None;

        /// <summary>
        /// Gamemode indicator, its needed so game knows which gamemode player selected and which to initialize on ROOMMANAGER object
        /// </summary>
        public Gamemodes Indicator { get; protected set; } = Gamemodes.None;

        /// <summary>
        /// for bots to recognize if they have to also attack their team and for health to know if apply friendly fire damage
        /// </summary>
        public bool FFA = false;
        public bool PeacefulBots = false;

        /// <summary>
        /// Determines if players spawn by simple cooldown or their respawn is completely dependent on gamemode
        /// </summary>
        public bool LetPlayersSpawnOnTheirOwn { protected set; get; } = true;

        /// <summary>
        /// Should be true only for round based gamemodes, like TeamEliminations or Defuse, we certainly
        /// dont want to let players take control over bots in deathmatch where everyone respawns after several seconds
        /// </summary>
        public bool LetPlayersTakeControlOverBots { protected set; get; } = false;

        protected int _maxTeamSize = 4;

        private int _timeToEnd = 0;

        public int GameDuration = 60;

        protected int playerTeam;


        /// <summary>
        /// time between moment when all required players joined the game and match starts
        /// </summary>
        public int WarmupTime = 10;

        public int TimeBuy = 10;

        public bool FriendyFire { protected set; get; } = true;

        Coroutine _timerCounter;
        Coroutine _delaySwithGamemodeState;

        /// <summary>
        /// players sorted by their teams in lists
        /// </summary>
        [HideInInspector] public List<Team> _teams = new List<Team>() { new Team(), new Team()};

        public SpawnpointsContainer defaultSpawnPoints;

        protected GamemodeRoundState RoundState;

        //events
        public delegate void Gamemode_GenericEvent();

        #region round events
        public Gamemode_GenericEvent GamemodeEvent_OnNewRoundSetup;
        public Gamemode_GenericEvent GamemodeEvent_OnNewRoundStarted;
        #endregion

        public delegate void Gamemode_Timer(int seconds);
        public Gamemode_Timer GamemodeEvent_Timer;

        public delegate void PlayerKilledByPlayer(Health victim, CharacterPart hittedPart, AttackType attackType, Health killer, Health assist);
        public PlayerKilledByPlayer Client_PlayerKilledByPlayer { get; set; }

        protected int _currentRound;
        protected bool _isAttackingTeamSwitched; // Flag to check if teams have switched roles
        protected int _attackingTeamIndex; // Index of the attacking team
        protected int _defendingTeamIndex; // Index of the defending team
        [SerializeField] protected GameObject[] blockSites;

        public enum GamemodeRoundState
        {
            None,
            FreezeTime,
            InProgress,
            RoundEnded,
            MatchEnded,
        }

        protected virtual void Awake()
        {
        }
        protected virtual void Start()
        {
        }

        protected void StopTimer() 
        {
            if (_timerCounter != null)
            {
                StopCoroutine(_timerCounter);
                _timerCounter = null;
            }

            _timeToEnd = 0;
            RpcUpdateTimer(0);
        }

        protected void CountTimer(int seconds)
        {
            StopTimer();

            _timerCounter = StartCoroutine(CountTime(seconds));

            IEnumerator CountTime(int seconds)
            {
                _timeToEnd = seconds;
                while (_timeToEnd > 0)
                {
                    _timeToEnd--;
                    RpcUpdateTimer(_timeToEnd);
                    yield return new WaitForSeconds(1f);
                }
                TimerEnded();
            }
        }

        [ClientRpc]
        void RpcUpdateTimer(int seconds)
        {
            GamemodeEvent_Timer?.Invoke(seconds);
        }

        #region player joined/disconnected callbacks
        public virtual void Server_OnPlayerInstanceAdded(PlayerInstance player) 
        {
            //if (!isServer) return;
            if (!isServer) return;

            //in FFA we want all players to be in the same team, so we dont let team choose and we choose default team for them instead
            if(player.Team == -1 && !player.BOT)
                AssignPlayerToTeam(player, 0);

            player.Server_ProcessSpawnRequest();
        }

        public void Server_OnPlayerInstanceRemoved(PlayerInstance player)
        {
            if (!isServer) return;

            //if player disconnects and was in certain team, then remove him from it
            if (player.Team != -1)
                RemovePlayerFromTeam(player);

            //say on chat that someone left the game
            Server_WriteToChat($"<color=#{ColorUtility.ToHtmlStringRGBA(Color.red)}>" + player.PlayerInfo.Username + " left the game</color>");

            //check if game can still run after player disconnected
            CheckTeamStates();
        }
        #endregion

        public void DelaySetGamemodeState(GamemodeState state, float delay)
        {
            CancelDelayedSwitchGamemodeStateIfExist();

            _delaySwithGamemodeState = StartCoroutine(ESwitchGamemodeState());

            IEnumerator ESwitchGamemodeState()
            {
                yield return new WaitForSeconds(delay);
                SwitchGamemodeState(state);
            }
        }

        protected void CancelDelayedSwitchGamemodeStateIfExist() 
        {
            if (_delaySwithGamemodeState != null)
            {
                StopCoroutine(_delaySwithGamemodeState);
                _delaySwithGamemodeState = null;
            }
        }

        protected void SwitchGamemodeState(GamemodeState state)
        {
            //print($"Try to switch to state: {state } from {State}, success: {state != State}");
            if (state == State) return;


            /*there might be a situation when player leaves the game when
             * changing gamemode state was planned, so we have to cancel it
             * to avoid situation when player leaves the game and after that
             * match starts with not enough players */
            CancelDelayedSwitchGamemodeStateIfExist();

            State = state;

            switch (State)
            {
                case GamemodeState.WaitingForPlayers:
                    MatchEvent_StartWaitingForPlayers();
                    break;
                case GamemodeState.Warmup:
                    MatchEvent_StartWarmup();
                    break;
                case GamemodeState.Inprogress:
                    MatchEvent_StartMatch();
                    break;
                case GamemodeState.Finish:
                    MatchEvent_EndMatch();
                    break;
            }
        }
        #region Round Events
        protected virtual void SwitchRoundState(GamemodeRoundState roundState)
        {
            if (roundState == RoundState) return;


            switch (roundState)
            {
                case GamemodeRoundState.FreezeTime:
                    GamemodeEvent_OnNewRoundSetup?.Invoke();
                    RoundEvent_Setup();
                    break;

                case GamemodeRoundState.InProgress:
                    RoundEvent_Start();
                    GamemodeEvent_OnNewRoundStarted?.Invoke();
                    break;
                case GamemodeRoundState.RoundEnded:
                    RoundEvent_End();
                    break;
            }

            RoundState = roundState;

            RpcSwitchRoundState(roundState);
        }

        //basically only usefull for clients UI
        [ClientRpc]
        void RpcSwitchRoundState(GamemodeRoundState roundState) 
        {
            switch (roundState)
            {
                case GamemodeRoundState.FreezeTime:
                    InfoRoundNormalMode(true);
                    GamemodeEvent_OnNewRoundSetup?.Invoke();
                    break;

                case GamemodeRoundState.InProgress:
                    InfoRoundNormalMode(false);
                    GamemodeEvent_OnNewRoundStarted?.Invoke();
                    break;
                case GamemodeRoundState.RoundEnded:
                    break;
            }

            RoundState = roundState;
        }

        protected virtual void RoundEvent_Setup() { }
        protected virtual void RoundEvent_Start() { }
        protected virtual void RoundEvent_End() { }
        protected virtual void RoundEvent_TimerEnded() { }
        #endregion

        #region Match events 
        protected virtual void MatchEvent_StartWaitingForPlayers() 
        {
            GamemodeMessage("Waiting for players...", 999f);
            StopTimer();

            LetPlayersSpawnOnTheirOwn = true;
        }
        protected virtual void MatchEvent_StartWarmup()
        {
            if(Indicator == Gamemodes.TeamDeathmatch || Indicator == Gamemodes.NormalMode)
            {
                GamemodeMessage($"BUY PHASE", TimeBuy);
                CountTimer(WarmupTime);
                LetPlayersSpawnOnTheirOwn = false;
            }
            else
            {
                GamemodeMessage($"Game will start in {WarmupTime} seconds", 4f);
                CountTimer(WarmupTime);
                LetPlayersSpawnOnTheirOwn = true;
            }

        }
        protected virtual void MatchEvent_StartMatch()
        {
        }
        protected virtual void MatchEvent_EndMatch()
        {

        }
        #endregion

        public virtual void PlayerSpawnCharacterRequest(PlayerInstance playerInstance)
        {
            playerInstance.Server_SpawnCharacter(defaultSpawnPoints.GetNextSpawnPoint());
        }

        protected string NotifyTeamRoles()
        {
            string message = playerTeam == _attackingTeamIndex ? "Attacking" : "Defending";
            return message;
        }

        public virtual void Relay_NewClientJoined(NetworkConnection conn, NetworkIdentity player)
        {
            if (!isServer) return;

            if (RoomSetup.Properties.P_FillEmptySlotsWithBots) 
            {
                List<PlayerInstance> players = GameManager.Players;

                //if we let player connect, and there are as much "Players" as MaxSlot, then we have a bot and need to vanish him to make
                //place for new player
                if (players.Count >= RoomSetup.Properties.P_MaxPlayers)
                {
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (players[i].BOT)
                        {
                            players[i].DespawnCharacterIfExist();
                            NetworkServer.Destroy(players[i].gameObject);
                            break;
                        }
                    }
                }
                else 
                {
                    FillEmptySlotsWithBots();
                }
            }

            
            TargeRPC_ClientSetupGamemode(conn, RoomSetup.Properties, player);
        }
        public virtual void Relay_ClientDisconnected(NetworkConnection conn, NetworkIdentity player)
        {
            if (RoomSetup.Properties.P_FillEmptySlotsWithBots)
            {
                List<PlayerInstance> players = GameManager.Players;

                if (players.Count < RoomSetup.Properties.P_MaxPlayers)
                {
                    FillEmptySlotsWithBots();
                }
            }
        }

        private void InfoRoundNormalMode(bool isStart)
        {
            UIShopMenu.Instance.IsBuyPhase = isStart;
            UICharacter._instance.StateButtonShop(isStart);
            for (int i = 0; i < blockSites.Length; i++)
            {
                blockSites[i].SetActive(isStart);
            }
        }

        [TargetRpc]
        void TargeRPC_ClientSetupGamemode(NetworkConnection conn, RoomProperties roomProperties, NetworkIdentity player)
        {
            //backend part
            if (!isServer)
                SetupGamemode(roomProperties);

            //ui part
            ClientFrontend.ClientEvent_OnJoinedToGame?.Invoke(this, player);
        }

        public virtual void SetupGamemode(RoomProperties roomProperties)
        {
            RoomSetup.Properties = roomProperties;

            GameDuration = roomProperties.P_GameDuration;
            GameManager.SetGamemode(this);
        }

        protected virtual void TimerEnded()
        {

        }

        #region listeners

        public virtual void Rpc_OnPlayerKilled(Health victimID, CharacterPart hittedPart, AttackType attackType, Health killer, Health assist)
        {
            Client_PlayerKilledByPlayer?.Invoke(victimID, hittedPart, attackType, killer, assist);
        }
        public virtual void Server_OnPlayerKilled(Health victimID, Health killerID)
        {
        }

        #endregion

        [ClientRpc]
        protected void GamemodeMessage(string msg, float liveTime)
        {
            GameManager.GameEvent_GamemodeEvent_Message?.Invoke(msg, liveTime);
        }

        [ClientRpc]
        protected void NormalModeMessage(string msg,int roleNumber, string roleName, float liveTime, bool isEnd)
        {
            GameManager.GameEvent_NormalMode_Message?.Invoke(msg, roleNumber, roleName, liveTime, isEnd);
        }


        /// <summary>
        /// method assings given player to given team and checks what should be done about it
        /// </summary>
        protected void AssignPlayerToTeam(PlayerInstance player, int teamToAssingTo)
        {
            //if player was already in team then remove him
            if (player.Team != -1)
            {
                if (player.MyCharacter)
                {
                    player.MyCharacter.Health.Server_ChangeHealthState(1000000, 0, AttackType.falldamage, player.MyCharacter.Health, 0);
                    print("killed");
                }
                RemovePlayerFromTeam(player);
            }

            //assign to team
            _teams[teamToAssingTo].PlayerInstances.Add(player);

            player.SetTeam(teamToAssingTo);

            OnPlayerAddedToTeam(player, teamToAssingTo);

            CheckTeamStates();

        }

        protected void RemovePlayerFromTeam(PlayerInstance player) 
        {
            if (player.Team != -1)
            {
                _teams[player.Team].PlayerInstances.Remove(player);
                OnPlayerRemovedFromTeam(player, player.Team);
            }
            else
                Debug.LogError("MultiFPS: TRYING TO REMOVE PLAYER FROM TEAM WHO DOES NOT BELONG TO ANY TEAM");
        }

        /// <summary>
        /// checks if game can start if someone joined some team,
        /// or if game can be still running if someone left some team or
        /// totally disconnected from the game
        /// </summary>
        protected virtual void CheckTeamStates()
        {
        }

        #region player managament

        protected void RespawnAllPlayers(SpawnpointsContainer spawnpoints, int team = 0)
        {
            int index = 0;
            foreach (PlayerInstance pi in GameManager.Players)
            {
                index++;
                if (pi.Team == team)
                    pi.Server_SpawnCharacter(spawnpoints.GetNextSpawnPoint());
            }
        }

        IEnumerator ServerSpawnCharacter(PlayerInstance pi, SpawnpointsContainer spawnpoints,int index)
        {
            PlayerInstance Pi = pi;
            float timeDelay = index * 5f;
            yield return new WaitForSeconds(timeDelay);
            Pi.Server_SpawnCharacter(spawnpoints.GetNextSpawnPoint());
        }

        /// <summary>
        /// block movent for all players
        /// </summary>
        protected void BlockAllPlayers(bool block = true) 
        {
            foreach (PlayerInstance pi in GameManager.Players)
            {
                if(pi.MyCharacter)
                    pi.MyCharacter.BlockCharacter(block);
            }
        }

        /// <summary>
        /// reset kills/deaths stats
        /// </summary>
        protected void ResetPlayersStats()
        {
            foreach (PlayerInstance pi in GameManager.Players)
            {
                pi.Kills = 0;
                pi.Deaths = 0;
                pi.Server_UpdateStatsForAllClients();
            }
        }
        #endregion

        /// <summary>
        /// process player request to join team, because team may be full or something else might be going on so
        /// we don't want to let players always join team that they want
        /// </summary>
        public void PlayerRequestToJoinTeam(PlayerInstance player, int requestedTeam)
        {
            int permission = PlayerRequestToJoinTeamPermission(player, requestedTeam);

            if (permission == 0)
            {
                AssignPlayerToTeam(player, requestedTeam);

                Server_WriteToChat($"{player.PlayerInfo.Username} <color=#{ColorUtility.ToHtmlStringRGBA(ClientInterfaceManager.Instance.UIColorSet.TeamColors[requestedTeam])}> joined team {(requestedTeam == 0? "blue": "orange")} </color>");
            }
            else
            {
                requestedTeam = requestedTeam == 0 ? 1 : 0;
                player.ProcessClientRequestToJoinTeam(requestedTeam);
                return;
            }

            //notify player if his request was accepted, and if not, tell him why, maybe team was full or sth else
            if(player.connectionToClient != null) //do not try send it to bots, no need for that
                player.RpcTeamJoiningResponse(requestedTeam, permission);

            playerTeam = player.Team;
        }

        public void ReMatch()
        {
            if (!isServer|| !ClientFrontend.Rematch)
                return;

            ClientFrontend.Rematch = false;
            SwitchRoundState(GamemodeRoundState.RoundEnded);
            DelaySetGamemodeState(GamemodeState.Warmup, 5f);
        }

        /// <summary>
        /// What happens if given player joined given team
        /// </summary>
        protected virtual void OnPlayerAddedToTeam(PlayerInstance player, int team) 
        {
            
        }
        /// <summary>
        /// What happens if given player was removed from team
        /// </summary>
        protected virtual void OnPlayerRemovedFromTeam(PlayerInstance player, int team)
        {

        }

        /// <summary>
        /// here we exactly specify conditions that have to be met in order to let player join his requested team
        /// return 0 if player has permission to join team
        /// return -1 if team is already full
        /// </summary>
        /// <returns></returns>
        protected virtual int PlayerRequestToJoinTeamPermission(PlayerInstance player, int requestedTeam) 
        {
            return 0;
        }


        [ClientRpc]
        public void Server_WriteToChat(string msg) 
        {
            ChatUI.Instance.WriteMessageToChat(msg);
        }

        #region bots management
        void FillEmptySlotsWithBots()
        {
            List<PlayerInstance> players = new List<PlayerInstance>(GameManager.Players);
            int neededBots = RoomSetup.Properties.P_MaxPlayers - players.Count;
            if (players.Count == 1)
            {
                SpawnBot(1);
            }

            //for (int i = 0; i < neededBots; i++)
            //{
            //    SpawnBot();
            //}
        }

        IEnumerator ServerSpawnBot( int index)
        {
            float timeDelay = index * 5f;
            yield return new WaitForSeconds(timeDelay);
            SpawnBot();
        }


        string[] botName = new string[10] { "Oliver","Olivia", "Harry","Amelia", "George", "Ava", "Jack", "Isla", "Noah","Emily" };
        public PlayerInstance SpawnBot(int team = 0)
        {
            if (!DNNetworkManager.Instance.isNetworkActive) return null;

            PlayerInstance playerInstance = Instantiate(DNNetworkManager.Instance.playerPrefab, transform.position, transform.rotation).GetComponent<PlayerInstance>();
            NetworkServer.Spawn(playerInstance.gameObject);

            if (playerInstance)
            {
                playerInstance.SetAsBot();
                playerInstance.RegisterPlayerInstance(); //this method is launched when server receives client data, but since server 
                //won't receive anything here, because server is the one creating this data, we have to launch it manually after spawning bot

                //make bot join some team
                playerInstance.ProcessRequestToJoinTeam(team);

                //if bot after trying to join team 0 is not in team 0, then try to join team 1
                if (playerInstance.Team == -1)
                {
                    team = 1;
                    playerInstance.ProcessRequestToJoinTeam(team);
                }

                string nameBot = (team == 1) ? botName[Random.Range(0, botName.Length)] : "BOT_" + Random.Range(0, 9999).ToString();
                playerInstance.PlayerInfo.Username = nameBot;
            }

            return playerInstance;
        }
        #endregion

        public enum GamemodeState
        {
            None,
            WaitingForPlayers,
            Warmup,
            Inprogress,
            Finish,
        }
        [System.Serializable]
        public class Team
        {
            public List<PlayerInstance> PlayerInstances = new List<PlayerInstance>();
        }

        #region useful methods
        public int GetAliveTeamAbundance(int teamID) 
        {
            List<PlayerInstance> team = _teams[teamID].PlayerInstances;

            int aliveOnes = 0;

            for (int i = 0; i < team.Count; i++)
            {
                if (team[i].MyCharacter && team[i].MyCharacter.Health.CurrentHealth > 0) aliveOnes++;
            }

            return aliveOnes;
        }
        #endregion
    }

    public enum Gamemodes : byte
    {
        None = 255,
        Deathmatch = 0,
        TeamDeathmatch = 1,
        TeamEliminations = 2,
        Defuse = 3,
        TeamDeathmatchMode = 4,
        NormalMode = 5,
    }
}