using Mirror;
using UnityEngine;

namespace MultiFPS.Gameplay.Gamemodes
{
    [AddComponentMenu("MultiFPS/Gamemodes/TeamDeathmatchMode")]
    public class TeamDeathmatchMode : TeamBasedGamemode
    {
        public static TeamDeathmatchMode Instance;

        protected override void Awake()
        {
            Instance = this;
        }

        public TeamDeathmatchMode()
        {
            Indicator = Gamemodes.TeamDeathmatchMode;
            LetPlayersSpawnOnTheirOwn = false;
            LetPlayersTakeControlOverBots = true;
        }

        protected override void RoundEvent_Setup()
        {
            base.RoundEvent_Setup();

            LetPlayersSpawnOnTheirOwn = false;
            FriendyFire = false;
        }

        public override void SetupGamemode(RoomProperties roomProperties)
        {
            base.SetupGamemode(roomProperties);
            LetPlayersSpawnOnTheirOwn = false;
        }

        public override void Server_OnPlayerKilled(Health victimID, Health killerID)
        {
            //count score only when game runs, not for example during warmup
            if (State != GamemodeState.Inprogress) return;

            OnPlayerKilled(new int[] { GetAliveTeamAbundance(0), GetAliveTeamAbundance(1) });

            if (RoundState != GamemodeRoundState.InProgress) return; //if round is ended, don't end it another time

            CheckAliveTeamStates();
        }

        protected override void RoundEvent_TimerEnded()
        {
            base.RoundEvent_TimerEnded();
            SwitchRoundState(GamemodeRoundState.RoundEnded);
        }

        protected override void MatchEvent_StartMatch()
        {
            base.MatchEvent_StartMatch();
            LetPlayersSpawnOnTheirOwn = true;
            RPC_TBG_UpdateTeamScores(_teamScores);
        }
    }
}