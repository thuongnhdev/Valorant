using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MultiFPS.UI.HUD;
using MultiFPS.UI;

namespace MultiFPS.Gameplay.Gamemodes
{
    [AddComponentMenu("MultiFPS/Gamemodes/NormalMode")]
    public class NormalMode : TeamBasedGamemode
    {
        public static NormalMode Instance;

        [SerializeField] private GameObject _bombItemPrefab;
        [SerializeField] private Transform _bombSpawnPoint;
        [SerializeField] GameObject _explosion;
        private GameObject _spawnedBombInstance;

        [SerializeField] BoxCollider[] _bombSites;


        //for keeping state of bomb
        private bool _bombPlanted;
        private bool _bombExploded;

        public Gamemode_GenericEvent Defuse_OnBombPlanted;
        public Gamemode_GenericEvent Defuse_OnBombDefused;
        public int RoundChange;

        [HideInInspector]
        public int DefendingTeamIndex;    
        [HideInInspector]
        public int AttackingTeamIndex;


        protected override void Awake()
        {
            Instance = this;

            _currentRound = 0;
            _isAttackingTeamSwitched = false;
            _attackingTeamIndex = 0; // Assuming team 0 is the attacking team initially
            _defendingTeamIndex = 1; // Assuming team 1 is the defending team initially

            if (_explosion != null)
            {
                _explosion.transform.position = Vector3.zero;
                _explosion.SetActive(false);
            }

            base.Awake();
            // We need only those bombsite parameters, we don't need them to be active
            for (int i = 0; i < _bombSites.Length; i++)
            {
                _bombSites[i].gameObject.SetActive(false);
            }
        }


        public NormalMode()
        {
            Indicator = Gamemodes.NormalMode;
            LetPlayersSpawnOnTheirOwn = false;
            LetPlayersTakeControlOverBots = true;
            FriendyFire = false;
        }

        protected override void RoundEvent_Setup()
        {
            OnPlayerKilled(new int[] { GetAliveTeamAbundance(0), GetAliveTeamAbundance(1) });
            CountTimer(TimeBuy);
            _currentRound++;

            if (_currentRound % RoundChange == 0 && !_isAttackingTeamSwitched)
            {
                SwitchTeams();
                _isAttackingTeamSwitched = true;

            }

            if (_isAttackingTeamSwitched == true)
            {
                RespawnAllPlayers(_teamSpawnpoints[0], 1);
                RespawnAllPlayers(_teamSpawnpoints[1], 0);
            }
            else
            {
                RespawnAllPlayers(_teamSpawnpoints[0], 0);
                RespawnAllPlayers(_teamSpawnpoints.Length > 1 ? _teamSpawnpoints[1] : _teamSpawnpoints[0], 1);
            }

            _bombPlanted = false;
            _bombExploded = false;

            if (_spawnedBombInstance)
                NetworkServer.Destroy(_spawnedBombInstance);

            _spawnedBombInstance = Instantiate(_bombItemPrefab, _bombSpawnPoint.position, _bombSpawnPoint.rotation);
            NetworkServer.Spawn(_spawnedBombInstance);
            LetPlayersSpawnOnTheirOwn = false;
            BlockAllPlayers(false);
            NormalModeMessage("BUY PHASE", _currentRound + 1, NotifyTeamRoles(), 3f, false);
            DefendingTeamIndex = _defendingTeamIndex;
            AttackingTeamIndex = _attackingTeamIndex;
        }

        protected override void RoundEvent_Start()
        {
            base.RoundEvent_Start(); 
        }

        private void SwitchTeams()
        {
            int temp = _attackingTeamIndex;
            _attackingTeamIndex = _defendingTeamIndex;
            _defendingTeamIndex = temp;
        }

        public override void SetupGamemode(RoomProperties roomProperties)
        {
            base.SetupGamemode(roomProperties);
            LetPlayersSpawnOnTheirOwn = false;
        }

        public override void Server_OnPlayerKilled(Health victimID, Health killerID)
        {
            // Count score only when game runs, not for example during warmup
            if (State != GamemodeState.Inprogress) return;

            OnPlayerKilled(new int[] { GetAliveTeamAbundance(_attackingTeamIndex), GetAliveTeamAbundance(_defendingTeamIndex) });

            if (RoundState != GamemodeRoundState.InProgress) return; // If round is ended, don't end it another time

            if (!_bombPlanted)
            {
                // If bomb is not planted, just end the game when one of the teams is completely eliminated
                CheckAliveTeamStates();
            }
            else
            {
                if (_bombExploded) return; // If bomb exploded and killed someone gamemode already changes state so we can't make it here
                                           // Case when attackers win by killing defenders while bomb was planted
                int winnerTeam = GetAliveTeamAbundance(_defendingTeamIndex) <= 0 ? _attackingTeamIndex : -1;

                if (winnerTeam == -1) return;

                _teamScores[winnerTeam]++;

                RPC_TBG_UpdateTeamScores(_teamScores);

                FinalizeTeamScores(winnerTeam);

                SwitchRoundState(GamemodeRoundState.RoundEnded);
            }
        }

        protected override void RoundEvent_TimerEnded()
        {
            base.RoundEvent_TimerEnded();

            if (!_bombPlanted)
            {
                // The attacking team loses if the bomb is not planted when the timer ends
                _teamScores[_defendingTeamIndex] += 1;
                RPC_TBG_UpdateTeamScores(_teamScores);

                RPC_TBG_RoundEnd(_defendingTeamIndex);
                SwitchRoundState(GamemodeRoundState.RoundEnded);
                return;
            }

            if (_bombPlanted)
            {
                _bombExploded = true;
                if (_spawnedBombInstance != null)
                {
                    _explosion.transform.position = _spawnedBombInstance.transform.position;

                    Bomb bomb = _spawnedBombInstance.GetComponent<Bomb>();

                    // Deal explosion damage to nearby players
                    Collider[] col = Physics.OverlapSphere(_spawnedBombInstance.transform.position, bomb.ExplosionRange, GameManager.characterLayer);

                    for (int i = 0; i < col.Length; i++)
                    {
                        Health health = col[i].gameObject.GetComponent<Health>();

                        if (!health) continue;

                        float distance = Vector3.Distance(_spawnedBombInstance.transform.position, health.transform.position);
                        float percentOfDamage;

                        if (distance <= bomb.MinimumExplosionRange)
                            percentOfDamage = 1;
                        else
                        {
                            percentOfDamage = 1f - ((distance - bomb.MinimumExplosionRange) / (bomb.ExplosionRange - bomb.MinimumExplosionRange));
                        }

                        health.Server_ChangeHealthStateRaw(Mathf.FloorToInt(bomb.MaxExplosionDamage * percentOfDamage), 0, AttackType.explosion, health, 1000);
                    }

                    StartCoroutine(PushRigidbodies(bomb.transform.position, bomb.ExplosionRange));
                    IEnumerator PushRigidbodies(Vector3 explosionOrigin, float explosionRange)
                    {
                        // We need to wait till the end of frame to make sure ragdolls are already spawned
                        // So we can push them
                        yield return new WaitForFixedUpdate();

                        Collider[] collidersRigidbody = Physics.OverlapSphere(explosionOrigin, explosionRange, GameManager.rigidbodyLayer);

                        foreach (Collider c in collidersRigidbody)
                        {
                            Rigidbody rg = c.GetComponent<Rigidbody>();
                            if (rg)
                            {
                                rg.AddExplosionForce(1000, explosionOrigin, explosionRange);
                            }
                        }
                    }

                    NetworkServer.Destroy(_spawnedBombInstance);
                    RpcDetonateBomb(_spawnedBombInstance.transform.position);
                }
            }

            _teamScores[_bombPlanted ? _attackingTeamIndex : _defendingTeamIndex] += 1;
            RPC_TBG_UpdateTeamScores(_teamScores);

            RPC_TBG_RoundEnd(_bombPlanted ? _attackingTeamIndex : _defendingTeamIndex);
            SwitchRoundState(GamemodeRoundState.RoundEnded);
        }

        [ClientRpc]
        void RpcDetonateBomb(Vector3 detonationPosition)
        {
            _explosion.transform.position = detonationPosition;

            _explosion.SetActive(true);
            _explosion.GetComponent<ParticleSystem>().Play();
            _explosion.GetComponent<AudioSource>().Play();
        }

        protected override void MatchEvent_StartMatch()
        {
            base.MatchEvent_StartMatch();
            RPC_TBG_UpdateTeamScores(_teamScores);
        }

        public bool BombCanBePlanted(CharacterInstance bombPlanter)
        {
            if (RoundState != GamemodeRoundState.InProgress) return false;

            for (int i = 0; i < _bombSites.Length; i++)
            {
                BoxCollider bombSite = _bombSites[i];

                Collider[] playersInBombSite = Physics.OverlapBox(bombSite.transform.position + bombSite.center, bombSite.size / 2, bombSite.transform.rotation, GameManager.characterLayer);

                for (int playerID = 0; playerID < playersInBombSite.Length; playerID++)
                {
                    CharacterInstance planter = playersInBombSite[playerID].GetComponent<CharacterInstance>();
                    if (planter && planter == bombPlanter) return true;
                }
            }
            return false;
        }

        public void BombPlanted()
        {
            _bombPlanted = true;
            GamemodeMessage("SPIKE PLANTED", 6f);
            CountTimer(10);

            // Notify clients
            RpcBombPlanted();
        }

        public void BombDefused()
        {
            _bombPlanted = false;
            GamemodeMessage("SPIKE DEFUSED", 5f);
            CountTimer(10);
            RpcBombDefused();
        }


        [ClientRpc]
        void RpcBombPlanted()
        {
            Defuse_OnBombPlanted?.Invoke();
        }

        [ClientRpc]
        void RpcBombDefused()
        {
            Defuse_OnBombDefused?.Invoke();
        }

        public override void Relay_NewClientJoined(NetworkConnection conn, NetworkIdentity player)
        {
            base.Relay_NewClientJoined(conn, player);
        }

    }
}
