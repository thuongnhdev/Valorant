using MultiFPS.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiFPS.UI.Gamemodes
{
    public class HudPlayerHealth : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI tmpName;
        [SerializeField] Image iconCharacter;
        [SerializeField] Image healthbar;

        CharacterInstance myCharacter;

        public void SetupNameplate(CharacterInstance _myCharacter)
        {
            _myCharacter.Health.Client_OnHealthStateChanged += OnPlayerHealthStateChanged;
            myCharacter = _myCharacter;

            tmpName.text = myCharacter.Health.CharacterName;

        }

        public void Set(CharacterInstance characterInstance)
        {
            myCharacter = characterInstance;
            SetupNameplate(characterInstance);
        }

        private void OnPlayerHealthStateChanged(int currentHealth, CharacterPart damagedPart, AttackType attackType, byte attackerID)
        {
            healthbar.fillAmount = (float)currentHealth / myCharacter.Health.MaxHealth;
        }

        private void OnDestroy()
        {
            myCharacter.Health.Client_OnHealthStateChanged -= OnPlayerHealthStateChanged;
        }

    }
}