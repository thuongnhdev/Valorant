 using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MultiFPS.UI.HUD;
using MultiFPS.Gameplay.Gamemodes;
using MultiFPS.UI;
using System;
using System.Threading.Tasks;
using System.Collections;
using MultiFPS.Sound;

namespace MultiFPS.Gameplay
{

    /// <summary>
    /// This component is responsible for managing and animating character
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Health))]
    public class CharacterInstance : DNNetworkBehaviour
    {
        [Header("Character setup")]
        //determines point to which we will attach player marker with nickname and healthbar
        public Transform CharacterMarkerPosition;

        //Model of character, we need to have access to it in order to lerp it's position beetwen positions received from the server
        public Transform CharacterParent;

        public Transform FPPLook;

        //Transform that player's camera will stick to, since Camera is external object not included in player prefab, must be children of
        //FPPLook (look above) to account for camera recoil produced with shooting from weapons
        public Transform FPPCameraTarget;
        public Transform FPPSkillCamTarget;
        public Transform FPPMinimapTarget;

        public float CameraHeight = 1.7f;

        //weapons recoil is affected by player movement. This variable determines how fast it will transition beetwen states,
        //Given in deegrees for second
        public float RecoilMovementFactorChangeSpeed = 4f;

        CharacterController _controller;

        public delegate void KilledCharacter(Health health);
        public KilledCharacter Server_KilledCharacter { get; set; }

        public delegate void CharacterEvent_SetAsBOT(bool _set);
        public CharacterEvent_SetAsBOT Server_SetAsBOT;

        public Transform characterMind; //this objects indicated direction character is looking at
        public Transform characterFirePoint; //this object is child of characterMind, will be used for recoil of weapons

        /// <summary>
        /// Hitbox prefab to assign to player model
        /// </summary>
        [SerializeField] GameObject _hitBoxContainerPrefab;
        HitboxSetup _hitboxes;

        [HideInInspector] public PlayerRecoil PlayerRecoil { private set; get; }
        [HideInInspector] public ToolMotion ToolMotion;

        [SerializeField] private ShroudedStepSkill shroudedStepSkill;
        [SerializeField] private ParanoiaSkill paranoiaSkill;
        [SerializeField] private DarkCoverSkill darkCoverSkill;
        [SerializeField] private FromTheShadowsSkill fromTheShadowsSkill;

        [SerializeField] private CloudBurstSkill cloudBurstSkill;
        [SerializeField] private UpDraftSkill upDraftSkill;
        [SerializeField] private TailWindSkill tailWindSkill;
        [SerializeField] private BladeStormSkill bladeStormSkill;

        public bool IsReloading; //this will be driven by synchronized events
        public bool IsCrouching = false;
        public bool IsUsingItem { get; set; }
        public bool IsRunning { get; set; }
        public bool IsScoping;
        public bool IsAbleToUseItem = true;
        public bool isGrounded;

        public float RecoilFactor_Movement = 1;
        public float SensitivityItemFactorMultiplier = 1f;

        public bool Block { private set; get; } = false; //determines if character can move and shoot or not, block if it is end of round

        /// <summary>
        /// Only true for character that is controlled by client, so only for player controller
        /// </summary>
        public bool IsObserved { set; get; } = false;

        /// <summary>
        /// Flag that informs us if player is set up to be viewed in 1st or 3rd person
        /// </summary>
        public bool FPP = false;

        /// <summary>
        /// Indicates if character is controlled by server or client
        /// </summary>
        public bool BOT = false;

        Health _killer;

        public CharacterItemManager CharacterItemManager { private set; get; }
        [HideInInspector] public Transform ObjectForDeathCameraToFollow;

        Vector3 _deathCameraDirection;

        #region smooth rotation and position
        [Header("Smooth position lerp")]
        float _lastSyncTime;
        float _previousTickDuration;

        float _rotationSmoothTimer;

        float _currentRotationTargetX;
        float _currentRotationTargetY;

        Vector3 _lastPositionSync;
        Vector3 _currentPositionSync;


        public float PositionLerpSpeed = 10f;
        #endregion

        //information about skins that players selected for his items
        [HideInInspector] public int[] _skinsForItems;

        public delegate void CharacterEvent_OnPerspectiveSet(bool fpp);
        public CharacterEvent_OnPerspectiveSet Client_OnPerspectiveSet { set; get; }


        public delegate void CharacterEvent_OnPickedupObject(string message);
        public CharacterEvent_OnPickedupObject Client_OnPickedupObject { get; set; }


        public delegate void OnDestroyed();
        public OnDestroyed Client_OnDestroyed { set; get; }
        public DNTransform DnTransform { get; internal set; }

        ClientSendSkillMessage clientSendSkillMessage;
        ClientSendInputMessage SendInputMessage;
        CharacterInputMessage InputMessage;
        public CharacterInput Input;

        [SerializeField] private CharacterType characterType;
        public Health Health { private set; get; }

        bool _spawned = false;

        [HideInInspector] public CharacterAnimator CharacterAnimator;

        [Header("Minimap")]
        [SerializeField] private GameObject miniMapCharInf;
        [SerializeField] private GameObject minimapZone;
        [SerializeField] private GameObject minimapPosition;
        [SerializeField] private GameObject minimapCircleZone;

        public enum CharacterType
        {
            Omen = 1,
            Jett = 2,
            Raze = 3,
            Gekko
        }

        public enum Skill
        {
            None = 0,
            ShroudedStepSkill = 1,
            ParanoiaSkill = 2,
            DarkCoverSkill = 3,
            FromTheShadowsSkill = 4,
            CloudBurstSkill = 5,
            UpdraftSkill = 6,
            TailWindSkill = 7,
            BladeStormSkill = 8
        }

        public enum HeroSkillType
        {
            None = 0,
            Skill_C,
            Skill_Q,
            Skill_E,
            Skill_X,
        }

        private int IdItemCurrentWhenUserSkill;

        public Skill CurrentSkill;
        public HeroSkillType RazeCurrentSkill;

        public bool IsSkilling = false;
        private void Awake()
        {
            Health = GetComponent<Health>();
            Health.Client_OnHealthAdded += OnClientHealthAdded;
            Health.Server_OnHealthDepleted += ServerDeath;
            Health.Server_Resurrect = OnServerResurrect;
            Health.Client_OnHealthStateChanged += ClientOnHealthStateChanged;
            Health.Client_Resurrect = OnClientResurrect;

            PlayerRecoil = GetComponent<PlayerRecoil>();
            if (!PlayerRecoil)
                PlayerRecoil = gameObject.AddComponent<PlayerRecoil>();

            PlayerRecoil.Initialize(FPPCameraTarget, this);

            CharacterItemManager = GetComponent<CharacterItemManager>();
            CharacterItemManager.Setup();

            DnTransform = GetComponent<DNTransform>();
            _controller = GetComponent<CharacterController>();

            CharacterAnimator = GetComponent<CharacterAnimator>();

            GameManager.SetLayerRecursively(CharacterParent.gameObject, 8);
            SetFppPerspective(false);
        }

        protected void Start()
        {
            _lastPositionSync = transform.position;
            _currentPositionSync = transform.position;

            if (_hitBoxContainerPrefab)
            {
                _hitboxes = Instantiate(_hitBoxContainerPrefab, transform.position, transform.rotation).GetComponent<HitboxSetup>();
                _hitboxes.SetHiboxes(CharacterParent.gameObject, Health);
                Destroy(_hitboxes.gameObject);//at this point empty object
            }

            GameTicker.Game_Tick += CharacterInstance_Tick;

            Input.LookY = transform.eulerAngles.y; //assigning start look rotation to spawnpoint rotation

            gameObject.layer = 6; //setting apppropriate layer for character collisions

            if (isServer)
            {
                CharacterItemManager.SpawnStartMatchEquipment();
                CharacterItemManager.ServerCommandTakeItem(1);
                GameSync.Singleton.Characters.ServerRegisterDNSyncObj(this);
                _spawned = true;
            }

            if (minimapZone && minimapPosition)
            {
                miniMapCharInf.SetActive(ClientFrontend.ThisClientTeam == Health.Team);
                minimapZone.SetActive(ClientFrontend.ThisClientTeam == Health.Team);
                Color teamColor = minimapPosition.GetComponent<SpriteRenderer>().color;
                minimapPosition.GetComponent<SpriteRenderer>().color = (isOwned) ? Color.white : teamColor;
            }

        }
        void Update()
        {
            #region killcam
            if (Health.CurrentHealth <= 0)
            {
                if (miniMapCharInf != null)
                    miniMapCharInf.SetActive(false);

                if (_killer)
                    FPPLook.rotation = Quaternion.Lerp(FPPLook.rotation, Quaternion.LookRotation(_killer.GetPositionToAttack() - FPPLook.position), 10f * Time.deltaTime);

                if (!ObjectForDeathCameraToFollow) return;

                RaycastHit hit;
                Vector3 castPosition = ObjectForDeathCameraToFollow.position + Vector3.up * 0.1f;

                float length;
                if (Physics.Raycast(castPosition, _deathCameraDirection, out hit, 5f, GameManager.environmentLayer))
                {
                    length = Mathf.Max(0, Vector3.Distance(hit.point, castPosition) - 0.5f);
                }
                else
                    length = 5f;

                FPPLook.transform.position = castPosition + _deathCameraDirection * length + transform.up * 0.2f;
                return;
            }
            #endregion

            if (CharacterItemManager.CurrentlyUsedItem)
            {
                float recoilMultiplier = !isGrounded ? 2.5f : (Input.Movement.x != 0 || Input.Movement.y != 0) ? CharacterItemManager.CurrentlyUsedItem.Recoil_walkMultiplier : 1f;
                RecoilFactor_Movement = Mathf.Lerp(RecoilFactor_Movement, recoilMultiplier, RecoilMovementFactorChangeSpeed * Time.deltaTime);
            }
            else
                RecoilFactor_Movement = 1f;

            if (Block)
                Input.Movement = Vector2.zero;


#if UNITY_EDITOR //rough 3rd person camera for debuging 3rd person animations
            //3rd person camera for testing
            if (UnityEngine.Input.GetKeyDown(KeyCode.I) && isOwned)
            {
                SetFppPerspective(!FPP);
                GetComponent<CharacterMotor>().cameraTargetPosition = new Vector3(0.55f, 2.1f, -3f);
            }


#endif

            if (ReadActionKeyCode(ActionCodes.Trigger1))
                CharacterItemManager.Fire1();
            if (ReadActionKeyCode(ActionCodes.Trigger2))
                CharacterItemManager.Fire2();

            #region observer smooth rotation

            _rotationSmoothTimer += Time.deltaTime;
            float percentage = Mathf.Clamp(_rotationSmoothTimer / _previousTickDuration, 0, 1);

            if (Health.CurrentHealth <= 0) return;

            if (isOwned)
            {
                // Vector3 positionForThisFrame = Vector3.Lerp(_lastPositionSync, _currentPositionSync, percentage);
                //  CharacterParent.position = positionForThisFrame;
                CharacterParent.position = Vector3.Lerp(CharacterParent.position, _currentPositionSync, Time.deltaTime * PositionLerpSpeed);
            }
            else
            {
                CharacterParent.position = Vector3.Lerp(CharacterParent.position, _currentPositionSync, Time.deltaTime * PositionLerpSpeed);
            }

            if (isOwned || isServer && BOT)
            {
                Input.LookX = Mathf.Clamp(Input.LookX, -90f, 90f);

                //rotate character based on player mouse input/bot input
                if (Health.CurrentHealth > 0)
                    transform.rotation = Quaternion.Euler(0, Input.LookY, 0);

                //rotate camera based on player mouse input/bot input
                FPPLook.localRotation = Quaternion.Euler(Input.LookX, 0, 0);
            }
            else
            {
                FPPLook.transform.localRotation = Quaternion.Lerp(FPPLook.transform.localRotation, Quaternion.Euler(_currentRotationTargetX, 0, 0), percentage);
                if (Health.CurrentHealth > 0)
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, _currentRotationTargetY, 0), percentage);
            }
 
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_STANDALONE_WIN //rough 3rd person camera for debuging 3rd person animations
            if (isOwned)
            {
                if (characterType == CharacterType.Omen)
                {
                    // skill shrouded step
                    if (UnityEngine.Input.GetKeyDown(KeyCode.C))
                    {
                        if (CurrentSkill != Skill.ShroudedStepSkill)
                            StopSkill();

                        if (IsSkilling)
                        {
                            return;
                        }
                        else
                        {
                            IsSkilling = true;
                            CurrentSkill = Skill.ShroudedStepSkill;
                            IdItemCurrentWhenUserSkill = CharacterItemManager.CurrentlyUsedSlotID;
                            shroudedStepSkill.StartSkill(FPPLook.transform.position);
                        }
                    }
                    else if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
                    {
                        if (CurrentSkill != Skill.ParanoiaSkill)
                            StopSkill();

                        if (IsSkilling)
                        {
                            return;
                        }
                        else
                        {
                            IsSkilling = true;
                            CurrentSkill = Skill.ParanoiaSkill;
                            IdItemCurrentWhenUserSkill = CharacterItemManager.CurrentlyUsedSlotID;
                            paranoiaSkill.StartSkill(FPPLook.transform.position);
                        }
                    }
                    else if (UnityEngine.Input.GetKeyDown(KeyCode.E))
                    {
                        if (CurrentSkill != Skill.DarkCoverSkill)
                            StopSkill();
                       
                        if (IsSkilling)
                        {
                            darkCoverSkill.ActionSkill(() =>
                            {
                                darkCoverSkill.ActionEnd(() =>
                                {
                                    IsSkilling = false;
                                    CurrentSkill = Skill.None;
                                });
                            });
                        }
                        else
                        {
                            IsSkilling = true;
                            CurrentSkill = Skill.DarkCoverSkill;
                            IdItemCurrentWhenUserSkill = CharacterItemManager.CurrentlyUsedSlotID;
                            darkCoverSkill.StartSkill(FPPLook.transform.position);
                        }
                    }
                    else if (UnityEngine.Input.GetKeyDown(KeyCode.X))
                    {
                        if (CurrentSkill != Skill.FromTheShadowsSkill)
                            StopSkill();

                        if (IsSkilling)
                        {
                            return;
                        }
                        else
                        {
                            IsSkilling = true;
                            CurrentSkill = Skill.FromTheShadowsSkill;
                            IdItemCurrentWhenUserSkill = CharacterItemManager.CurrentlyUsedSlotID;
                            fromTheShadowsSkill.StartSkill(FPPLook.transform.position);
                        }
                    }
                    else if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        if (CurrentSkill != Skill.None)
                        {
                            switch (CurrentSkill)
                            {
                                case Skill.ShroudedStepSkill:
                                    shroudedStepSkill.ActionSkill(() =>
                                    {
                                        shroudedStepSkill.ActionEnd(() =>
                                        {
                                            IsSkilling = false;
                                            CurrentSkill = Skill.None;
                                        });
                                    });
                                    break;
                                case Skill.ParanoiaSkill:
                                    paranoiaSkill.ActionSkill(() =>
                                    {
                                        paranoiaSkill.ActionEnd(() =>
                                        {
                                            IsSkilling = false;
                                            CurrentSkill = Skill.None;
                                        });
                                    });
                                    break;
                                case Skill.DarkCoverSkill:
                                    if (darkCoverSkill.CurrentStepSkill == DarkCoverSkill.StepSkill.ActivationState)
                                        //increase usage distance.
                                        darkCoverSkill.IncreaseUsageDistanceSkill();
                                    break;
                                case Skill.FromTheShadowsSkill:
                                    fromTheShadowsSkill.ActionSkill(() =>
                                    {
                                        fromTheShadowsSkill.ActionEnd(() =>
                                        {
                                            IsSkilling = false;
                                            CurrentSkill = Skill.None;
                                        });
                                    });
                                    break;
                            }
                        }
                        else
                            CharacterItemManager.Fire1();

                    }
                    else if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        if (CurrentSkill != Skill.None)
                        {
                            switch (CurrentSkill)
                            {
                                case Skill.ShroudedStepSkill:
                                case Skill.ParanoiaSkill:
                                case Skill.FromTheShadowsSkill:
                                    if (IsSkilling && CurrentSkill != Skill.None)
                                    {
                                        StopSkill();
                                        CurrentSkill = Skill.None;
                                    }
                                    break;
                                case Skill.DarkCoverSkill:
                                    if (darkCoverSkill.CurrentStepSkill == DarkCoverSkill.StepSkill.ActivationState)
                                        //increase usage distance.
                                        darkCoverSkill.DecreaseUsageDistanceSkill();
                                    break;
                            }

                        }
                        else
                            CharacterItemManager.Fire2();
                    }
                    else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha0) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha1) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)
                        || UnityEngine.Input.GetKeyDown(KeyCode.Alpha3) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha4) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha5) ||
                        UnityEngine.Input.GetKeyDown(KeyCode.Alpha6) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha7) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha8)
                        || UnityEngine.Input.GetKeyDown(KeyCode.Alpha9) || UnityEngine.Input.GetKeyDown(KeyCode.R) ||
                        UnityEngine.Input.GetKeyDown(KeyCode.T) || UnityEngine.Input.GetKeyDown(KeyCode.Y) || UnityEngine.Input.GetKeyDown(KeyCode.U) ||
                        UnityEngine.Input.GetKeyDown(KeyCode.I) || UnityEngine.Input.GetKeyDown(KeyCode.O) || UnityEngine.Input.GetKeyDown(KeyCode.P) ||
                        UnityEngine.Input.GetKeyDown(KeyCode.F) || UnityEngine.Input.GetKeyDown(KeyCode.G) || UnityEngine.Input.GetKeyDown(KeyCode.H) ||
                        UnityEngine.Input.GetKeyDown(KeyCode.J) || UnityEngine.Input.GetKeyDown(KeyCode.K) || UnityEngine.Input.GetKeyDown(KeyCode.L) ||
                        UnityEngine.Input.GetKeyDown(KeyCode.Z) || UnityEngine.Input.GetKeyDown(KeyCode.V) || UnityEngine.Input.GetKeyDown(KeyCode.B) ||
                        UnityEngine.Input.GetKeyDown(KeyCode.N) || UnityEngine.Input.GetKeyDown(KeyCode.M) || UnityEngine.Input.GetKeyDown(KeyCode.Space) ||
                        UnityEngine.Input.GetKeyDown(KeyCode.LeftShift) || UnityEngine.Input.GetKeyDown(KeyCode.RightShift))
                    {
                        if (IsSkilling && CurrentSkill != Skill.None)
                        {
                            StopSkill();
                            CurrentSkill = Skill.None;
                        }

                    }
                }
                else if(characterType == CharacterType.Jett)
                {
                    // skill shrouded step
                    if (UnityEngine.Input.GetKeyDown(KeyCode.C))
                    {
                        if (CurrentSkill != Skill.CloudBurstSkill)
                            StopSkill();

                        if (IsSkilling)
                        {
                            return;
                        }
                        else
                        {
                            IsSkilling = true;
                            CurrentSkill = Skill.CloudBurstSkill;
                            IdItemCurrentWhenUserSkill = CharacterItemManager.CurrentlyUsedSlotID;
                            cloudBurstSkill.StartSkill(FPPLook.transform.position,()=> {
                                IsSkilling = false;
                                CurrentSkill = Skill.None;
                            });
                        }
                    }
                    else if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
                    {
                        if (CurrentSkill != Skill.UpdraftSkill)
                            StopSkill();

                        if (IsSkilling)
                        {
                            return;
                        }
                        else
                        {
                            IsSkilling = true;
                            CurrentSkill = Skill.UpdraftSkill;
                            IdItemCurrentWhenUserSkill = CharacterItemManager.CurrentlyUsedSlotID;
                            upDraftSkill.StartSkill(FPPLook.transform.position, () => {
                              
                                this.gameObject.GetComponent<CharacterMotor>().SetStartFallingPointY(0);
                                IsSkilling = false;
                                CurrentSkill = Skill.None;
                            });
                        }
                    }
                    else if (UnityEngine.Input.GetKeyDown(KeyCode.E))
                    {
                        if (CurrentSkill != Skill.TailWindSkill)
                            StopSkill();
                       
                        if (IsSkilling)
                        {
                            if (tailWindSkill.CurrentStepSkill == TailWindSkill.StepSkill.ActivationState)
                            {
                                IdItemCurrentWhenUserSkill = CharacterItemManager.CurrentlyUsedSlotID;
                                tailWindSkill.ActionSkill(() =>
                                {
                                    tailWindSkill.ActionEnd(() =>
                                    {
                                        IsSkilling = false;
                                        CurrentSkill = Skill.None;
                                    });
                                });
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            IsSkilling = true;
                            CurrentSkill = Skill.TailWindSkill;
                            tailWindSkill.StartSkill(FPPLook.transform.position);
                        }
                    }
                    else if (UnityEngine.Input.GetKeyDown(KeyCode.X))
                    {
                        if (CurrentSkill != Skill.BladeStormSkill)
                            StopSkill();

                        if (IsSkilling)
                        {
                            return;
                        }
                        else
                        {
                            IsSkilling = true;
                            CurrentSkill = Skill.BladeStormSkill;
                            IdItemCurrentWhenUserSkill = CharacterItemManager.CurrentlyUsedSlotID;
                            bladeStormSkill.StartSkill(FPPLook.transform.position);
                        }
                    }
                    else if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        if (CurrentSkill != Skill.None)
                        {
                            switch (CurrentSkill)
                            {
                                case Skill.CloudBurstSkill:
                                case Skill.UpdraftSkill:
                                case Skill.TailWindSkill:
                                    CharacterItemManager.Fire1();
                                    break;
                                case Skill.BladeStormSkill:
                                    bladeStormSkill.ActionSkill(() =>
                                    {
                                        bladeStormSkill.ActionEnd(() =>
                                        {
                                            IsSkilling = false;
                                            CurrentSkill = Skill.None;
                                        });
                                    });
                                    break;
                            }
                        }
                        else
                            CharacterItemManager.Fire1();

                    }
                    else if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        if (CurrentSkill != Skill.None)
                        {
                            switch (CurrentSkill)
                            {
                                case Skill.CloudBurstSkill:
                                case Skill.UpdraftSkill:
                                case Skill.TailWindSkill:
                                    CharacterItemManager.Fire2();
                                    break;
                                case Skill.BladeStormSkill:
                                    bladeStormSkill.ActionSkill(() =>
                                    {
                                        bladeStormSkill.ActionEnd(() =>
                                        {
                                            IsSkilling = false;
                                            CurrentSkill = Skill.None;
                                        });
                                    });
                                    break;
                            }
                        }
                        else
                            CharacterItemManager.Fire2();
                    }
                    else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha0) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha1) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)
                        || UnityEngine.Input.GetKeyDown(KeyCode.Alpha3) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha4) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha5) ||
                        UnityEngine.Input.GetKeyDown(KeyCode.Alpha6) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha7) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha8)
                        || UnityEngine.Input.GetKeyDown(KeyCode.Alpha9) || UnityEngine.Input.GetKeyDown(KeyCode.R) ||
                        UnityEngine.Input.GetKeyDown(KeyCode.T) || UnityEngine.Input.GetKeyDown(KeyCode.Y) || UnityEngine.Input.GetKeyDown(KeyCode.U) ||
                        UnityEngine.Input.GetKeyDown(KeyCode.I) || UnityEngine.Input.GetKeyDown(KeyCode.O) || UnityEngine.Input.GetKeyDown(KeyCode.P) ||
                        UnityEngine.Input.GetKeyDown(KeyCode.F) || UnityEngine.Input.GetKeyDown(KeyCode.G) || UnityEngine.Input.GetKeyDown(KeyCode.H) ||
                        UnityEngine.Input.GetKeyDown(KeyCode.J) || UnityEngine.Input.GetKeyDown(KeyCode.K) || UnityEngine.Input.GetKeyDown(KeyCode.L) ||
                        UnityEngine.Input.GetKeyDown(KeyCode.Z) || UnityEngine.Input.GetKeyDown(KeyCode.V) || UnityEngine.Input.GetKeyDown(KeyCode.B) ||
                        UnityEngine.Input.GetKeyDown(KeyCode.N) || UnityEngine.Input.GetKeyDown(KeyCode.M) || UnityEngine.Input.GetKeyDown(KeyCode.Space) ||
                        UnityEngine.Input.GetKeyDown(KeyCode.LeftShift) || UnityEngine.Input.GetKeyDown(KeyCode.RightShift))
                    {
                        if (IsSkilling && CurrentSkill != Skill.None)
                        {
                            StopSkill();
                            CurrentSkill = Skill.None;
                        }

                    }
                }
                else if(characterType == CharacterType.Raze)
                {
                    if (UnityEngine.Input.GetKeyDown(KeyCode.C))
                    {
                        if (IsSkilling) StopSkill();

                        IsSkilling = true;
                        RazeCurrentSkill = HeroSkillType.Skill_C;
                        // todo
                    }
                    if (UnityEngine.Input.GetKeyDown(KeyCode.E))
                    {
                        if (IsSkilling) StopSkill();

                        IsSkilling = true;
                        RazeCurrentSkill = HeroSkillType.Skill_E;
                        SwitchItemRazeUseSkill();
                        //todo action
                    }
                    if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
                    {
                        if (IsSkilling) StopSkill();

                        IsSkilling = true;
                        RazeCurrentSkill = HeroSkillType.Skill_Q;
                        SwitchItemRazeUseSkill();
                        //todo action
                    }
                    if (UnityEngine.Input.GetKeyDown(KeyCode.X))
                    {
                        if (IsSkilling) StopSkill();

                        IsSkilling = true;
                        RazeCurrentSkill = HeroSkillType.Skill_X;
                        //todo action
                    }
                    else if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        if (IsSkilling)
                        {
                            CharacterItemManager.Fire1();
                            StopSkill();
                        }                        
                    }
                    else if (CheckCancelSkill() && IsSkilling)
                    {
                        StopSkill();
                    }
                }
            }

#endif
            #endregion
            if (minimapCircleZone != null)
            {
                bool isShowZoneSound = Input.Movement != Vector2.zero && isOwned && !ReadActionKeyCode(ActionCodes.Sprint);
                minimapCircleZone.SetActive(isShowZoneSound);
            }
        }

        private bool CheckCancelSkill()
        {
            return (UnityEngine.Input.GetKeyDown(
                KeyCode.Alpha0) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha1) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)
                || UnityEngine.Input.GetKeyDown(KeyCode.Alpha3) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha4) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha5) 
                || UnityEngine.Input.GetKeyDown(KeyCode.Alpha6) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha7) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha8)
                || UnityEngine.Input.GetKeyDown(KeyCode.Alpha9) || UnityEngine.Input.GetKeyDown(KeyCode.R) || UnityEngine.Input.GetKeyDown(KeyCode.T) 
                || UnityEngine.Input.GetKeyDown(KeyCode.Y) || UnityEngine.Input.GetKeyDown(KeyCode.U) || UnityEngine.Input.GetKeyDown(KeyCode.I) 
                || UnityEngine.Input.GetKeyDown(KeyCode.O) || UnityEngine.Input.GetKeyDown(KeyCode.P) || UnityEngine.Input.GetKeyDown(KeyCode.F) 
                || UnityEngine.Input.GetKeyDown(KeyCode.G) || UnityEngine.Input.GetKeyDown(KeyCode.H) || UnityEngine.Input.GetKeyDown(KeyCode.J) 
                || UnityEngine.Input.GetKeyDown(KeyCode.K) || UnityEngine.Input.GetKeyDown(KeyCode.L) || UnityEngine.Input.GetKeyDown(KeyCode.Z) 
                || UnityEngine.Input.GetKeyDown(KeyCode.V) || UnityEngine.Input.GetKeyDown(KeyCode.B) || UnityEngine.Input.GetKeyDown(KeyCode.N) 
                || UnityEngine.Input.GetKeyDown(KeyCode.M) || UnityEngine.Input.GetKeyDown(KeyCode.LeftShift) || UnityEngine.Input.GetKeyDown(KeyCode.RightShift));
        }

        public void Action_Skill_Jett_E_Auto()
        {
            if (CurrentSkill != Skill.TailWindSkill)
                StopSkill();

            if (IsSkilling)
            {
                if (tailWindSkill.CurrentStepSkill == TailWindSkill.StepSkill.ActivationState)
                {
                    IdItemCurrentWhenUserSkill = CharacterItemManager.CurrentlyUsedSlotID;
                    tailWindSkill.ActionSkill(() =>
                    {
                        tailWindSkill.ActionEnd(() =>
                        {
                            IsSkilling = false;
                            CurrentSkill = Skill.None;
                        });
                    });
                }
                else
                {
                    return;
                }
            }
        }

        private void StopSkill()
        {
            if (characterType == CharacterType.Omen)
            {
                SoundManager.Instance.StopPlayOneShot_Battle_HeroSkill();
                paranoiaSkill.StopFx();
                darkCoverSkill.StopFx();
                shroudedStepSkill.StopFx();
                fromTheShadowsSkill.StopFx();
                IsSkilling = false;
                CurrentSkill = Skill.None;
                switch (CurrentSkill)
                {
                    case Skill.ShroudedStepSkill:
                        shroudedStepSkill.StopSkill(() => { });
                        break;
                    case Skill.ParanoiaSkill:
                        paranoiaSkill.StopSkill(() => { });
                        break;
                    case Skill.DarkCoverSkill:
                        darkCoverSkill.StopSkill(() => { });
                        break;
                    case Skill.FromTheShadowsSkill:
                        fromTheShadowsSkill.StopSkill(() => { });
                        break;
                }
                UICharacter._instance.SetActiveMinimapSkill(false);
            }
            else if (characterType == CharacterType.Jett)
            {
                SoundManager.Instance.StopPlayOneShot_Battle_HeroSkill();
                cloudBurstSkill.StopFx();
                upDraftSkill.StopFx();
                tailWindSkill.StopFx();
                bladeStormSkill.StopFx();
                IsSkilling = false;
                CurrentSkill = Skill.None;
                switch (CurrentSkill)
                {
                    case Skill.CloudBurstSkill:
                        cloudBurstSkill.StopSkill(() => { });
                        break;
                    case Skill.UpdraftSkill:
                        upDraftSkill.StopSkill(() => { });
                        break;
                    case Skill.TailWindSkill:
                        tailWindSkill.StopSkill(() => { });
                        break;
                    case Skill.BladeStormSkill:
                        bladeStormSkill.StopSkill(() => { });
                        break;
                }
            }
            else if (characterType == CharacterType.Raze)
            {
                IsSkilling = false;
                switch (RazeCurrentSkill)
                {
                    case HeroSkillType.Skill_C:
                        break;
                    case HeroSkillType.Skill_Q:
                    case HeroSkillType.Skill_E:
                        SetAnimReloadItem_Gaze();
                        break;
                    case HeroSkillType.Skill_X:
                        break;
                }

                RazeCurrentSkill = HeroSkillType.None;
            }
        }
        void SetAnimReloadItem_Gaze()
        {
            if (CharacterItemManager == null) return;
            CharacterItemManager.ClientTakeItem(1);

        }

        public void SetAnimReloadItem()
        {
            SetActiveItem(true);
            if (CharacterItemManager == null || Health.CurrentHealth <= 0) return;
            CharacterItemManager.ReloadAnimItem(CharacterItemManager.CurrentlyUsedSlotID);
        }

        public void SwitchItemRazeUseSkill()
        {
            if (CharacterItemManager == null) return;

            if (CharacterItemManager.CurrentlyUsedSlotID == 2)
            { 
                CharacterItemManager.ClientTakeItem(2); 
            }
            else
            {
                var itemIdx = (RazeCurrentSkill) switch
                {
                    HeroSkillType.Skill_E => 5,
                    HeroSkillType.Skill_Q => 4,
                    _ => 2
                };
                CharacterItemManager.ClientTakeItem(itemIdx);
            }
        }

        public void SwitchItemWhenUserSkill()
        {
            SetActiveItem(false);
        }

        public void SetActiveItem(bool isActive)
        {
            if (CharacterItemManager != null)
                if (CharacterItemManager.CurrentlyUsedItem != null)
                    CharacterItemManager.CurrentlyUsedItem.gameObject.SetActive(isActive);
        }

        public void SetSkillCam(Transform target = null)
        {
            GameplayCamera._instance?.SetTarget(target == null ? FPPCameraTarget : target);
            GameplayCamera._instance.SetFov(target == null);
        }

        public void OnEventActionSkillCloudBurst()
        {
            cloudBurstSkill.OnEventActionSkillCloudBurst();
        }

        public void OnCompleteAnimationStart()
        {
            darkCoverSkill.OnCompleteAnimationStart();
        }

        public void OnCompleteAnimationEnd()
        {
            darkCoverSkill.OnCompleteAnimationEnd();
        }

        public void OnEventShroudedStepEnd()
        {
            shroudedStepSkill.OnEventShroudedStepEnd();
        }

        public void OnEventParanoiSkillFX_Start_01()
        {
            paranoiaSkill.OnEventParanoiSkillFX_Start_01();
        }

        public void OnEventParanoiSkillFX_Start_02()
        {
            paranoiaSkill.OnEventParanoiSkillFX_Start_02();
        }

        public void OnEventParanoiSkillLoop()
        {
            paranoiaSkill.OnEventParanoiSkillLoop();
        }

        public void OnEventParanoiaFxEnd()
        {
            paranoiaSkill.OnEventParanoiaFxEnd();
        }

        public void OnEventDarkCoverSkillFX_Start_01()
        {
            //darkCoverSkill.OnEventDarkCoverSkillFX_Start_01();
        }

        public void OnEventFromTheShadowsSkillFX_Start_01()
        {
            fromTheShadowsSkill.OnEventFromTheShadowsSkillFX_Start_01();
        }

        public void OnEventFromTheShadowsSkillFX_Loop()
        {
            fromTheShadowsSkill.OnEventFromTheShadowsSkillFX_Loop();
        }

        public void OnEventBladeStormEndAnimtionStart()
        {
            bladeStormSkill.OnEventBladeStormEndAnimtionStart();
        }

        public void OnEventBladeStormEndAnimtionSpecial()
        {
            bladeStormSkill.OnEventBladeStormEndAnimtionSpecial();
        }

        public void SetCurrentSkillAllUserTarget(CharacterSkillMessage skill)
        {

        }

        #region Sound skill 
        public void Omen_C_PlayFX_Sound_Start()
        {
            shroudedStepSkill.PlayFX_Sound_Start();
        }

        public void Omen_C_PlayFX_Sound_Cast()
        {
            shroudedStepSkill.PlayFX_Sound_Cast();
        }

        public void Omen_Q_PlayFX_Sound_Cast()
        {
            paranoiaSkill.PlayFX_Sound_Cast();
        }

        public void Omen_Q_PlayFX_Sound_Active()
        {
            paranoiaSkill.PlayFX_Sound_Active();
        }

        public void Omen_E_PlayFX_Sound_Start()
        {
            darkCoverSkill.PlayFX_Sound_Start();
        }

        public void Omen_E_PlayFX_Sound_Cast()
        {
            darkCoverSkill.PlayFX_Sound_Cast();
        }

        public void Omen_E_PlayFX_Sound_DarkSmoke()
        {
            darkCoverSkill.Omen_E_PlayFX_Sound_DarkSmoke();
        }

        public void Omen_E_PlayFX_Sound_DarkSmoke_Disappeared()
        {
            darkCoverSkill.PlayFX_Sound_DarkSmoke_Disappeared();
        }

        public void Omen_X_PlayFX_Sound_Start()
        {
            fromTheShadowsSkill.PlayFX_Sound_Start();
        }

        public void Omen_X_PlayFX_Sound_Active()
        {
            fromTheShadowsSkill.PlayFX_Sound_Active();
        }

        public void Omen_X_PlayFX_Sound_Voice_Line()
        {
            fromTheShadowsSkill.PlayFX_Sound_Voice_Line();
        }

        public void Jett_C_PlayFX_Sound_Cast()
        {
            cloudBurstSkill.PlayFX_Sound_Cast();
        }

        public void Jett_C_PlayFX_Sound_ExpandFullSize()
        {
            cloudBurstSkill.PlayFX_Sound_ExpandFullSize();
        }

        public void Jett_C_PlayFX_Sound_SmokeFullSize()
        {
            cloudBurstSkill.PlayFX_Sound_SmokeFullSize();
        }

        public void Jett_Q_PlayFX_Sound_Active()
        {
            upDraftSkill.PlayFX_Sound_Active();
        }

        public void Jett_E_PlayFX_Sound_Cast()
        {
            tailWindSkill.PlayFX_Sound_Cast();
        }

        public void Jett_E_PlayFX_Sound_Active()
        {
            tailWindSkill.PlayFX_Sound_Active();
        }

        public void Jett_X_PlayFX_Sound_Active()
        {
            bladeStormSkill.PlayFX_Sound_Active();
        }

        public void Jett_X_PlayFX_Sound_Start()
        {
            bladeStormSkill.PlayFX_Sound_Start();
        }

        public void Jett_X_PlayFX_Sound_Active_End()
        {
            bladeStormSkill.PlayFX_Sound_Active_End();
        }

        #endregion
        #region position lerp
        public void PrepareCharacterToLerp() 
        {
            _lastPositionSync = CharacterParent.position;
        }
        public void SetCurrentPositionTargetToLerp(Vector3 target) 
        {
            CharacterParent.position = _lastPositionSync;

            _currentPositionSync = target;


            _previousTickDuration = Time.time - _lastSyncTime;
            _lastSyncTime = Time.time;

            _rotationSmoothTimer = 0f;
        }
        #endregion

        public bool IsClientOrBot() { return isOwned || isServer && BOT; }


        #region input networking
        void CharacterInstance_Tick()
        {
            if (isOwned && !isServer)
                NetworkClient.Send(PrepareInputMessage(), Channels.Unreliable);
        }

        public void CharacterInstance_Skill()
        {
            if (isOwned && !isServer)
                ;
                //NetworkClient.Send(PrepareSkillMessage(), Channels.Unreliable);
        }
        #endregion


        private void ServerDeath(CharacterPart hittedPartID, AttackType attackType, Health killer, int attackForce) 
        {
            if (killer)
            {
                CharacterInstance killerChar = killer.GetComponent<CharacterInstance>();

                if (killerChar)
                    killerChar.Server_KilledCharacter?.Invoke(Health);
            }

            GameManager.Gamemode.Server_OnPlayerKilled(Health, killer);
            _controller.enabled = false;
        }

        private void OnServerResurrect(int health) 
        {
            _lastPositionSync = transform.position;

            _controller.enabled = true;
            GetComponent<CharacterMotor>().enabled = true;

            if (_spawned)
            {
                CharacterItemManager.SpawnStartMatchEquipment();
                CharacterItemManager.ServerCommandTakeItem(1);
            }
        }
        private void OnClientResurrect(int health)
        {
            GetComponent<CharacterMotor>().enabled = true; //disable movement for dead character
            _controller.enabled = true;

            FPPLook.transform.localPosition = new Vector3(0, CameraHeight, 0);

            //if (isOwned)
            //    ClientFrontend.SetObservedCharacter(this);
            //else
                SetFppPerspective(false);
        }
        public void ClientOnHealthStateChanged(int currentHealth, CharacterPart hittedPartID, AttackType attackType, byte attackerID)
        {
            if (currentHealth > 0) return;

            //death
            _controller.enabled = false;

            //Set camera to follow killer
            if (IsObserved)
            {
                GameplayCamera._instance.SetFovToDefault();

                _killer = GameSync.Singleton.Healths.GetObj(attackerID);

                if (_killer)
                {
                    _deathCameraDirection = _killer.netId != netId ?
                    (FPPLook.transform.position - _killer.GetPositionToAttack()).normalized :
                    -FPPLook.forward;

                    if (_killer && (attackerID != DNID)) //dont show this message in case of suicide
                        GameManager.GameEvent_GamemodeEvent_Message?.Invoke("You were killed by " + _killer.CharacterName, RoomSetup.Properties.P_RespawnCooldown);
                }
            }
        }


        public void BlockCharacter(bool block) 
        {
            Block = block;
            RpcBlockCharacter(block);
        }
        [ClientRpc]
        private void RpcBlockCharacter(bool block) 
        {
            Block = block;
        }

        public void SetFppPerspective(bool fpp) 
        {
            FPP = fpp;

            Client_OnPerspectiveSet?.Invoke(fpp);

            if (fpp)
                GameplayCamera._instance.SetTarget(FPPCameraTarget);

            if (isOwned)
                MinimapCamera.Instance.SetTarget(FPPMinimapTarget);
        }


        #region input
        public bool ReadActionKeyCode(ActionCodes actionCode)
        {
            return (Input.ActionCodes & (1 << (int)actionCode)) != 0;
        }
        public void SetActionKeyCode(ActionCodes actionCode, bool _set)
        {
            int a = Input.ActionCodes;
            if (_set)
            {
                a |= 1 << ((byte)actionCode);
            }
            else
            {
                a &= ~(1 << (byte)actionCode);
            }
            Input.ActionCodes = (byte)a;
        }
        #endregion

        public void SetAsBOT(bool _set)
        {
            //if turn bot to player, game will try to teleport him to his spawnpoint because server does not write these values
            //for bots every single tick
            if (BOT)
            {
                _lastPositionSync = transform.position;
                _currentPositionSync = transform.position;
            }

            BOT = _set;

            Server_SetAsBOT?.Invoke(_set);
        }

        //for ui to read this and display message in hud
        void OnClientHealthAdded(int currentHealth, int addedHealth, byte healerID)
        {
            Client_OnPickedupObject?.Invoke($"Health +{addedHealth}");
        }

        public void ServerTeleport(Vector3 pos, float rotationY) 
        {
            Input.LookX = 0;
            Input.LookY = rotationY;

            transform.SetPositionAndRotation(pos, Quaternion.Euler(0, rotationY, 0));
            CharacterParent.position = pos;
            _lastPositionSync = pos;
            _currentPositionSync = pos;
            Physics.SyncTransforms();
            RpcTeleport(pos, rotationY);
        }

        [ClientRpc]
        public void RpcTeleport(Vector3 pos, float rotationY) 
        {
            if (isServer) return;

            Input.LookX = 0;
            Input.LookY = rotationY;

            _lastPositionSync = pos;
            _currentPositionSync = pos;
            transform.SetPositionAndRotation(pos, Quaternion.Euler(0,rotationY,0));

            CharacterParent.position = pos;
            Physics.SyncTransforms();
        }

        public byte IdSkill;
        public byte ActionStepSkill;
        public Vector3 PositionStart;
        public Vector3 PositionEnd;
        public ClientSendSkillMessage PrepareSkillMessage()
        {
            Vector3 targetOffset = this.transform.forward * 30;
            Vector3 targetPosition = FPPLook.transform.position + targetOffset;
            clientSendSkillMessage.IdSkill = (byte)CurrentSkill;
            clientSendSkillMessage.CharacterID = 0;
            clientSendSkillMessage.ActionStepSkill = (byte)CurrentSkill;
            clientSendSkillMessage.PositionStart = FPPLook.transform.position;
            clientSendSkillMessage.PositionEnd = targetPosition;

            return clientSendSkillMessage;
        }

        public ClientSendSkillMessage ServerPrepareSkillMessage()
        {
            Vector3 targetOffset = this.transform.forward * 30;
            Vector3 targetPosition = FPPLook.transform.position + targetOffset;
            clientSendSkillMessage.IdSkill = (byte)CurrentSkill;
            clientSendSkillMessage.CharacterID = 0;
            clientSendSkillMessage.ActionStepSkill = (byte)CurrentSkill;
            clientSendSkillMessage.PositionStart = FPPLook.transform.position;
            clientSendSkillMessage.PositionEnd = targetPosition;

            return clientSendSkillMessage;
        }

        public ClientSendInputMessage PrepareInputMessage()
        {
            SendInputMessage.Movement = FitMovementInputToOneByte(Input.Movement);
            SendInputMessage.LookX = (sbyte)Mathf.FloorToInt(Input.LookX);
            SendInputMessage.LookY = (short)Input.LookY;
            SendInputMessage.ActionCodes = Input.ActionCodes;

            return SendInputMessage;
        }
        public CharacterInputMessage ServerPrepareInputMessage()
        {
            InputMessage.Movement = FitMovementInputToOneByte(Input.Movement);
            InputMessage.LookX = (sbyte)Mathf.FloorToInt(Input.LookX);
            InputMessage.LookY = (short)Input.LookY;
            InputMessage.ActionCodes = Input.ActionCodes;

            return InputMessage;
        }


        public byte FitMovementInputToOneByte(Vector2 movement)
        {
            // Two small signed numbers (values between -8 to 7)
            int mX = Mathf.FloorToInt(movement.x / 0.2f);
            int mY = Mathf.FloorToInt(movement.y / 0.2f);

            // Convert the numbers to 4-bit two's complement representation
            byte first4Bits = (byte)((mX < 0 ? 0x08 : 0x00) | (Math.Abs(mX) & 0x07)); // Check sign bit and keep last 3 bits
            byte second4Bits = (byte)((mY < 0 ? 0x08 : 0x00) | (Math.Abs(mY) & 0x07)); // Check sign bit and keep last 3 bits

            // Combine the two 4-bit representations into a single byte
            return (byte)((first4Bits << 4) | second4Bits);
        }

        public void ReadMovementInputFromByte(byte input)
        {
            Input.Movement.x = ((input & 0x70) >> 4) * ((input & 0x80) == 0x80 ? -1 : 1) * 0.2f;
            Input.Movement.y = (input & 0x07) * ((input & 0x08) == 0x08 ? -1 : 1) * 0.2f;
        }

        public void ReadAndApplyInputFromMessage(ClientSendInputMessage msg)
        {
            ApplyInput(msg.Movement, msg.LookX, msg.LookY, msg.ActionCodes);
        }
        public void ReadAndApplyInputFromServer(CharacterInputMessage msg)
        {
            ApplyInput(msg.Movement, msg.LookX, msg.LookY, msg.ActionCodes);
        }

        void ApplyInput(byte movementInput, float lookInputX, float lookInputY, byte actionCodes) 
        {
            ReadMovementInputFromByte(movementInput);
            Input.LookX = lookInputX;
            Input.LookY = lookInputY;
            Input.ActionCodes = actionCodes;

            _currentRotationTargetX = lookInputX;
            _currentRotationTargetY = lookInputY;
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            Client_OnDestroyed?.Invoke();

            Health.Server_OnHealthDepleted -= ServerDeath;
            Health.Client_OnHealthStateChanged -= ClientOnHealthStateChanged;

            GameTicker.Game_Tick -= CharacterInstance_Tick;
        }

        void OnDestroy()
        {
            if(GameSync.Singleton)
                GameSync.Singleton.Characters.ServerDeregisterDNSyncObj(this);
            StopSkill();
            characterFirePoint = null;
            FPPCameraTarget = null;
            FPPLook = null;
            CharacterParent = null;
            CharacterMarkerPosition = null;
            characterMind = null;
            characterFirePoint = null;

            PlayerRecoil = null;
            CharacterItemManager = null;
            ToolMotion = null;
        }
    }
    public enum ActionCodes 
    {
        Trigger1,
        Trigger2,
        Sprint,
        Crouch,
    }

    [System.Serializable]
    //input that is interpreted by the game
    public struct CharacterInput
    {
        public Vector2 Movement;
        public float LookX;
        public float LookY;
        public byte ActionCodes;
    }

    //compressed version of input that will be sent over network
    public struct CharacterInputMessage
    {
        public byte Movement; //both horizontal and vertical movement input will be stored in one byte
        public sbyte LookX;
        public short LookY;
        public byte ActionCodes; //sprint, fire1, fire2, and 5 free inputs to utilize   
    }

    public struct CharacterSkillMessage
    {
        public byte CharacterID;
        public byte IdSkill;
        public byte ActionStepSkill;
        public Vector3 PositionStart;
        public Vector3 PositionEnd;

        public CharacterSkillMessage(CharacterInstance character)
        {
            CharacterID = character.DNID;
            IdSkill = character.IdSkill;
            ActionStepSkill = character.ActionStepSkill;
            PositionStart = character.PositionStart;
            PositionEnd = character.PositionEnd;
        }
    }
}
