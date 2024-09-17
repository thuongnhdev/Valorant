using Mirror;
using MultiFPS.Gameplay.Gamemodes;
using UnityEngine;
using UnityEngine.UI;

namespace MultiFPS.UI.Gamemodes
{
    public class UINormalMode : UITeamBasedGamemode
    {
        [SerializeField] Image _bombPlantedIndicator;

        public override void SetupUI(Gamemode gamemode, NetworkIdentity player)
        {
            base.SetupUI(gamemode, player);
            gamemode.GetComponent<NormalMode>().Defuse_OnBombPlanted += OnBombPlanted;
            gamemode.GetComponent<NormalMode>().Defuse_OnBombDefused += OnBombDefused;


            _bombPlantedIndicator.enabled = false;
        }

        void OnBombPlanted()
        {
            _bombPlantedIndicator.color = Color.red;
            _bombPlantedIndicator.enabled = true;
        }

        protected override void OnNewRoundStarted()
        {
            _bombPlantedIndicator.enabled = false;
        }

        void OnBombDefused()
        {
            _bombPlantedIndicator.color = Color.green;
            _bombPlantedIndicator.enabled = true;
        }
    }
}
