using Mirror;
using MultiFPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

using MultiFPS.Gameplay.Gamemodes;
using System;

namespace MultiFPS.UI.HUD
{
    /// <summary>
    /// Responsible for displaying player health, ammo for current weapon
    /// </summary>
    public class UICharacter : CharacterHud
    {
        public GameObject Overlay;

        public static UICharacter _instance;

        [Header("Health")]
        [SerializeField] Text _healthText;
        [SerializeField] Slider _healthBar;

        [Header("World Icons")]
        public GameObject UIMarkerPrefab;

        //borders for 3D UI icons, in this project we use it only for player nametags 
        public Canvas WorldIconBorders;

        [Header("Respawn UI")]
        [SerializeField] UIRespawnCooldown _respawnCooldown;

        [Header("countdown UI")]
        [SerializeField] public UICountDown _uiCountDown;

        [Header("Omen countdown UI")]
        [SerializeField] public UICountDownSkillJett _uiCountDownSkillJett;

        [Header("Gamemodes UI")]
        /// <summary>
        /// gamemodes UI prefabs have to be placed in this array in the same order as gamemodes in enum "Gamemodes"
        /// </summary>
        [SerializeField] GameObject[] gamemodesUI;

        [Header("Ammo")]
        [SerializeField] Text _ammo;
        [SerializeField] Text _ammoSupply;

        [Header("KillConfirmation")]
        [SerializeField] bool _showKillConfirmationText;
        [SerializeField] Text _killMessage;
        //[SerializeField] float _killMsgLiveTime = 3f;
        Coroutine _killConfirmationMessageProcedure;

        //animation propeties of "kill confirmed" icon
        public float vanishingSpeed;
        public float maxInclination;
        public float gainingSpeed;
        public float vanishTime = 0.5f; 

        [Header("DamageIndicator")]
        [SerializeField] HUDTakeDamageMarker _takeDamageMarker;
        [SerializeField] Image _damageIndicatorImage; //UI that will flash with red on damage
        [SerializeField] Color _damageIndicatorColor;
        Coroutine _damageIndicatorAnimation;
        [SerializeField] public Image _minimapSkillQImage;
        [SerializeField] float _damageIndicatorVanishTime = 5f;

        [Header("Marker")]
        [SerializeField] HitMarker _hitMarker;
        [SerializeField] HitMarker _killConfirmationMarker;

        [Header("UI")]
        [SerializeField] Button buttonShootLeft;
        [SerializeField] Button buttonShootRight;
        [SerializeField] Button buttonJump;
        [SerializeField] Button buttonReload;
        [SerializeField] Button buttonCrouch;
        [SerializeField] Button buttonAim;
        [SerializeField] Button buttonWalk;
        [SerializeField] Button buttonSwitch1;
        [SerializeField] Button buttonSwitch2;
        [SerializeField] Button buttonUseKnife;
        [SerializeField] Button buttonMeleeLeft;
        [SerializeField] Button buttonMeleeRight;
        [SerializeField] Button buttonKnifeToGun;
        [SerializeField] Button buttonShop;

        [SerializeField] public Scrollbar DarkCoverScrollSkill;

        public JoystickVirtual joystick;

        #region Quick Message
        [SerializeField] private TextMeshProUGUI _msgText;
        [SerializeField] private TextMeshProUGUI _msgRoundRoleText;
        [SerializeField] private TextMeshProUGUI _msgRoundNumberText;
        [SerializeField] private GameObject _msgBackground;
        [SerializeField] private GameObject _objClickBuy;
        [SerializeField] private GameObject _objTeamRoleBot;
        [SerializeField] private TextMeshProUGUI _msgRoundRoleBotText;

        Coroutine _messageLiveTimeCounter;
        #endregion


        protected override void Awake ()   
        {
            base.Awake();

            _instance = this;

            ShowCharacterHUD(false);
            _ammo.text = string.Empty;
            _ammoSupply.text = string.Empty;
        
            _killMessage.color = Color.clear;
            _damageIndicatorImage.color = Color.clear;

            GameManager.GameEvent_GamemodeEvent_Message += GamemodeMessage;
            GameManager.GameEvent_NormalMode_Message += NormalModeMessage;

            //hide message UI on start
            _msgBackground.gameObject.SetActive(false);
            _msgText.text = string.Empty;

            if (buttonJump || buttonSwitch1 || buttonReload || buttonShootRight || buttonShootLeft || buttonAim || buttonCrouch != null)
            {
                buttonSwitch1.onClick.AddListener(PlayerGameplayInput.Instance.TakeItem1);
                buttonSwitch2.onClick.AddListener(PlayerGameplayInput.Instance.TakeItem2);
                buttonUseKnife.onClick.AddListener(PlayerGameplayInput.Instance.TakeKnife);
                buttonKnifeToGun.onClick.AddListener(PlayerGameplayInput.Instance.TakeItem1);
                buttonJump.onClick.AddListener(PlayerGameplayInput.Instance.Jump);
                buttonReload.onClick.AddListener(PlayerGameplayInput.Instance.Reload);
                buttonShootRight.onClick.AddListener(PlayerGameplayInput.Instance.Shoot);
                buttonMeleeLeft.onClick.AddListener(PlayerGameplayInput.Instance.Shoot);
                buttonShootLeft.onClick.AddListener(PlayerGameplayInput.Instance.Shoot);
                buttonMeleeRight.onClick.AddListener(PlayerGameplayInput.Instance.Shoot);
                buttonAim.onClick.AddListener(PlayerGameplayInput.Instance.Aim);
                buttonCrouch.onClick.AddListener(PlayerGameplayInput.Instance.Cround);
            }

        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameManager.GameEvent_GamemodeEvent_Message -= GamemodeMessage;
            GameManager.GameEvent_NormalMode_Message -= NormalModeMessage;
        }

        //register/deregister character
        protected override void AssignCharacterForUI(CharacterInstance _characterInstanceToAssignForUI)
        {
            _myObservedCharacter = _characterInstanceToAssignForUI;
            _myObservedCharacter.Health.Client_OnHealthStateChanged += OnHealthStateChanged;
            _myObservedCharacter.Health.Client_OnHealthAdded += OnHealthAdded;
            _myObservedCharacter.Health.Client_KillConfirmation += PlayKillIcon;
            _characterInstanceToAssignForUI.Health.Client_OnDamageDealt += OnDamageDealt;

            ShowCharacterHUD(true);
            UpdateHealthHUD();

            _takeDamageMarker.Initialize(_characterInstanceToAssignForUI);
            _respawnCooldown.HideUI();

            GetComponent<HudInventory>().ObserveCharacter(_characterInstanceToAssignForUI);
        }
        protected override void DeassignCurrentCharacterFromUI(CharacterInstance _characterToDeassign)
        {
            _myObservedCharacter.Health.Client_OnHealthStateChanged -= OnHealthStateChanged;
            _myObservedCharacter.Health.Client_OnHealthAdded -= OnHealthAdded;
            _myObservedCharacter.Health.Client_KillConfirmation -= PlayKillIcon;
            _myObservedCharacter.Health.Client_OnDamageDealt -= OnDamageDealt;
        }

        
        private void OnDamageDealt(int currentHealth, int takenDamage, CharacterPart damagedPart, AttackType attackType, byte victimID)
        {
            _hitMarker.PlayAnimation(damagedPart);
        }


        public void OnAmmoStateChanged(string ammo, string supply) 
        {
            _ammo.text = $" {ammo}";
            _ammoSupply.text = $"{supply} ";
        }

        void OnHealthStateChanged(int currentHealth, CharacterPart damagedPart, AttackType attackType, byte attackerID)
        {
            UpdateHealthHUD();

            if (_damageIndicatorAnimation != null) 
            {
                StopCoroutine(_damageIndicatorAnimation);
                _damageIndicatorAnimation = null;
            }

            _damageIndicatorAnimation = StartCoroutine(DamageIndicatorAnimation());

            if (_myObservedCharacter.Health.CurrentHealth <= 0) 
            {
                if (GameManager.Gamemode.LetPlayersSpawnOnTheirOwn) _respawnCooldown.StartUI(RoomSetup.Properties.P_RespawnCooldown);
            }

            IEnumerator DamageIndicatorAnimation() 
            {
                _damageIndicatorImage.color = _damageIndicatorColor;

                Color startColor = _damageIndicatorImage.color;
                float progress = 0;

                while (progress < 1f) 
                {
                    progress += Time.deltaTime * _damageIndicatorVanishTime;
                    _damageIndicatorImage.color = Color.Lerp(startColor, Color.clear, progress);
                    yield return null;
                }
                _damageIndicatorImage.color = Color.clear;
            }
        }
        void OnHealthAdded(int currentHealth, int addedHealth, byte healerID) => UpdateHealthHUD();
        
        

        //update health state in HUD
        void UpdateHealthHUD() 
        {
            _healthText.text = _myObservedCharacter.Health.CurrentHealth.ToString();
            _healthBar.value = (float)_myObservedCharacter.Health.CurrentHealth / _myObservedCharacter.Health.MaxHealth;
        }

        public void ShowCharacterHUD(bool _show)
        {
            Overlay.SetActive(_show);
        }

        public void PlayKillIcon(CharacterPart damagedPart, byte victimID)
        {
            Health victim = GameSync.Singleton.Healths.GetObj(victimID);

            if (!victim) return;

            _killConfirmationMarker.PlayAnimation(damagedPart);

            if (!_showKillConfirmationText) return;

            //feedback fro player
            if (victimID != _myObservedCharacter.netId)
                _killMessage.text = "TERMINATED: " + victim.CharacterName;
            else
                _killMessage.text = "SELFDESTRUCT";

            if (_killConfirmationMessageProcedure != null) 
            {
                StopCoroutine(_killConfirmationMessageProcedure);
                _killConfirmationMessageProcedure = null;
            }
        }

        void GamemodeMessage(string _msg, float _liveTime)
        {
            if (_messageLiveTimeCounter != null)
            {
                StopCoroutine(_messageLiveTimeCounter);
                _messageLiveTimeCounter = null;
            }
            _messageLiveTimeCounter = StartCoroutine(messageLiveTimeCounter());

            IEnumerator messageLiveTimeCounter()
            {
                _msgText.text = _msg;
                _msgBackground.gameObject.SetActive(true);
                _objTeamRoleBot.SetActive(false);
                _objClickBuy.SetActive(false);
                _msgRoundRoleText.gameObject.SetActive(false);
                _msgRoundNumberText.gameObject.SetActive(false);
                //_msgBackground.OnSizeChanged();

                yield return new WaitForSeconds(_liveTime);
                _msgBackground.gameObject.SetActive(false);
                _msgText.text = "";
            }
        }

        void NormalModeMessage(string _msg, int msgRoundNumber, string msgRole, float _liveTime, bool isEnd)
        {
            if (_messageLiveTimeCounter != null)
            {
                StopCoroutine(_messageLiveTimeCounter);
                _messageLiveTimeCounter = null;
            }
            _messageLiveTimeCounter = StartCoroutine(messageLiveTimeCounter());

            IEnumerator messageLiveTimeCounter()
            {
                _msgText.text = _msg;
                _msgRoundRoleText.text = msgRole;
                _msgRoundNumberText.text = "ROUND " + msgRoundNumber;
                _msgBackground.gameObject.SetActive(true);
                _objTeamRoleBot.SetActive(isEnd);
                _objClickBuy.SetActive(!isEnd);
                _msgRoundRoleText.gameObject.SetActive(!isEnd);
                _msgRoundNumberText.gameObject.SetActive(!isEnd);
                _msgRoundRoleBotText.text = msgRole;

                yield return new WaitForSeconds(_liveTime);
                _msgBackground.gameObject.SetActive(false);
                _msgText.text = "";
            }
        }

        public void SetSpriteBtnWeapon(bool isGun)
        {
            buttonShootLeft.gameObject.SetActive(isGun);
            buttonShootRight.gameObject.SetActive(isGun);
            buttonUseKnife.gameObject.SetActive(isGun);
            buttonMeleeLeft.gameObject.SetActive(!isGun);
            buttonMeleeRight.gameObject.SetActive(!isGun);
            buttonKnifeToGun.gameObject.SetActive(!isGun);
        }

        public void SetActiveMinimapSkill(bool isActive)
        {
            if (_minimapSkillQImage == null) return;
            _minimapSkillQImage.gameObject.SetActive(isActive);
        }

        public void SetActiveCountDownSkill_Jett_E(bool isActive, Action<bool> onComplete)
        {
            if (_uiCountDownSkillJett != null)
            {
                _uiCountDownSkillJett.gameObject.SetActive(true);
                _uiCountDownSkillJett.StartUI(5, onComplete);
            }
        }

        public void SetActiveCountDownSkill_Omen_E(int type)
        {
            if (_uiCountDown != null)
            {
                _uiCountDown.gameObject.SetActive(true);
                _uiCountDown.StartUI(type);
            }
        }

        public void StateButtonShop(bool isActive)
        {
           buttonShop.gameObject.SetActive(isActive);
        }    
    }
}
