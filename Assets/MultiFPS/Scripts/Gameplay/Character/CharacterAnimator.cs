﻿using MultiFPS.Gameplay;
using MultiFPS.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace MultiFPS
{
    [RequireComponent(typeof(CharacterInstance))]

    /// <summary>
    /// Class responsible for animating character, this means: Animating character on runtime and applying appropriate animations
    /// based on currently used item
    /// </summary>
    public class CharacterAnimator : MonoBehaviour
    {
        CharacterInstance _characterInstance;
        CharacterItemManager _characterItemManager;
        CharacterController _controller;

        [SerializeField] OverrideTransform _chestIK;
        [SerializeField] OverrideTransform _headIK;

        public Transform[] Pos_BattleFx_Hit;

        bool fppPerspective = false;

        [SerializeField] public Animator _animator { get; set; }

        [SerializeField] Animator _tppModelAnimator;
        [SerializeField] Animator _fppModelAnimator;

        public RuntimeAnimatorController _baseAnimatorController;
        public RuntimeAnimatorController BaseAnimatorControllerFPP;

        [SerializeField] RigBuilder _rigBuilder;
        /// <summary>
        /// player model animator
        /// </summary>
        [Header("Character meshes")]
        [SerializeField] SkinnedMeshRenderer[] _characterMeshes;
        [SerializeField] SkinnedMeshRenderer []_fppCharacterMesh;

        public SkinContainer MySkin { private set; get; }

        #region step animation variables

        [SerializeField] float _rotationAngleThreshold = 80;
        [SerializeField] float _rotationAngleThresholdLeftSide = 20;
        [SerializeField] float _adjustingRotationLerpSpeed = 180;
        bool _adjustingRotation = false;
        #endregion

        float _currentBodyRotation;
        float _upperBodyAnimateFactor;
        float _speed;

        float _recoilX;
        float _recoilStabilizationSpeed = 5f;

        Vector3 _takingDamge_IKSpine;
        Vector3 _takingDamge_IKHead;

        [SerializeField] float _stabilizationAfterTakingDamageSpeed = 4f;

        float _fallingTime;

        [Header("Audio")]
        [SerializeField] AudioSource _audioSource;
        [SerializeField] AudioClip _landingSound;
        [SerializeField] AudioClip _movingSingleSound;
        [SerializeField] float _landingSoundThresholdTime = 0.75f;

        private bool isPlayingMoveSound = false;
        private Coroutine moveSoundCoroutine;

        void Awake()
        {
            if (_audioSource)
            {
                _audioSource.spatialBlend = 1f;

                //set other audiosource values to work in 3D space
                _audioSource.spread = 180;
                _audioSource.rolloffMode = AudioRolloffMode.Linear;
                _audioSource.maxDistance = 30;
                _audioSource.minDistance = 10;
            }

            _characterInstance = GetComponent<CharacterInstance>();
            _characterItemManager = GetComponent<CharacterItemManager>();
            _controller = GetComponent<CharacterController>();

            _characterInstance.Client_OnPerspectiveSet += OnPerspectiveChanged;
            _characterItemManager.Client_EquipmentChanged += AssingCharacterAnimationsForCurrentlyUsedItem;
        }

        private void Start()
        {
            fppPerspective = _characterInstance.FPP;
            _characterInstance.Health.Client_OnHealthStateChanged += OnDamaged;

            _characterInstance.Health.Client_Resurrect += OnResurrected;
            _characterInstance.Health.Client_OnHealthDepleted += OnDeath;
        }
        private void FixedUpdate()
        {
            if (_characterInstance.Health.CurrentHealth > 0)
                UpdateAnimationProperties();
        }

        private void Update()
        {
            if (!_characterInstance.FPP)
                UpdateRotationAnimation(Time.deltaTime);

            if (_movingSingleSound && _characterInstance.Input.Movement != Vector2.zero && _characterInstance.isGrounded && !_characterInstance.ReadActionKeyCode(ActionCodes.Sprint))
            {
                if (!isPlayingMoveSound)
                {
                    PlayMoveSound();
                }
            }

            if (_landingSound && _fallingTime > _landingSoundThresholdTime && _characterInstance.isGrounded)
            {
                _audioSource.pitch = 1f - 0.4f * Mathf.Lerp(0, 1, Mathf.Clamp(_fallingTime - _landingSoundThresholdTime, 0, 1));
                _audioSource.PlayOneShot(_landingSound);
            }

            if (!_characterInstance.isGrounded)
            {
                _fallingTime += Time.deltaTime;
            }
            else
            {
                _fallingTime = 0;
            }
        }

        private float standVolume;
        private void PlayMoveSound()
        {
            if (moveSoundCoroutine != null)
            {
                return;
            }
            step = 0;
            standVolume = _audioSource.volume;
            moveSoundCoroutine = StartCoroutine(WaitAndPlaySecondMoveSound());
        }

        private int step;
        private IEnumerator WaitAndPlaySecondMoveSound()
        {
            if (_characterInstance.Input.Movement != Vector2.zero && _characterInstance.isGrounded)
            {
                if (step % 2 == 0)
                    _audioSource.volume = standVolume + 10;
                else _audioSource.volume = standVolume - 10;
                _audioSource.PlayOneShot(_movingSingleSound);
                yield return new WaitForSeconds(0.35f);
            }

            _audioSource.volume = standVolume;
            moveSoundCoroutine = null;
        }


        private void OnDeath(CharacterPart hittedPartID, byte attackerID)
        {
            enabled = false;
        }

        private void OnResurrected(int health)
        {
            _rigBuilder.Clear();
            _rigBuilder.enabled = false;
            _takingDamge_IKHead = Vector3.zero;
            _takingDamge_IKSpine = Vector3.zero;
            _recoilX = 0;
            _currentBodyRotation = _characterInstance.Input.LookY;
            _animator.transform.localEulerAngles = Vector3.zero;

            _chestIK.transform.localEulerAngles = Vector3.zero;
            _headIK.transform.localEulerAngles = Vector3.zero;

            _rigBuilder.enabled = true;
            _rigBuilder.Build();

            _adjustingRotation = false;

            enabled = true;
        }

        public void ApplySkin(int skindID)
        {
            //apply skin for character, materials and meshed for both FPP hands model and TPP character model
            MySkin = ClientInterfaceManager.Instance.characterSkins[skindID];
            if (!MySkin) return;

            /*if (MySkin.Mesh)
                _characterMeshes[0].sharedMesh = MySkin.Mesh;

            if (MySkin.Material)
                _characterMeshes[0].material = MySkin.Material;

            if (MySkin.FPP_Mesh)
                _fppCharacterMesh.sharedMesh = MySkin.FPP_Mesh;

            if (MySkin.FPP_Material)
                _fppCharacterMesh.material = MySkin.FPP_Material;*/
        }

        #region callbacks

        private void OnHealthDepleted(CharacterPart damagedPart, byte attackerID)
        {
            _animator.runtimeAnimatorController = _baseAnimatorController;
        }

        void OnPerspectiveChanged(bool fpp)
        {
            //we cannot just disable character model, because it has hitboxes so host
            //would be immortal to bots
            foreach (SkinnedMeshRenderer mesh in _characterMeshes)
            {
                //if character is dead, then player model should not be reenabled, 
                //because it is replaced by ragdoll prefab
                mesh.enabled = !fpp && _characterInstance.Health.CurrentHealth > 0;
            }

            _fppModelAnimator.gameObject.SetActive(fpp);
            _animator = fpp ? _fppModelAnimator : _tppModelAnimator;
            foreach (var model in _fppCharacterMesh)
            {
                model.gameObject.layer = (int)GameLayers.fppModels;
            }

            //dont set animator controllers twice for same perspective
            if (fppPerspective != fpp)
                AssingCharacterAnimationsForCurrentlyUsedItem(_characterItemManager.CurrentlyUsedSlotID);

            fppPerspective = fpp;
        }
        #endregion

        void AssingCharacterAnimationsForCurrentlyUsedItem(int slotID = -1)
        {
            Item currentlyUsedItem = slotID >= 0 ? _characterItemManager.Slots[slotID].Item : null;

            if (currentlyUsedItem)
                _animator.runtimeAnimatorController = _characterInstance.FPP ? currentlyUsedItem.AnimatorControllerForCharacterFPP : currentlyUsedItem.AnimatorControllerForCharacter;
            else
                _animator.runtimeAnimatorController = _baseAnimatorController;

            if (_animator.runtimeAnimatorController == null)
                _animator.runtimeAnimatorController = _baseAnimatorController;

            SetTrigger(AnimationNames.ITEM_TAKE);
        }

        private bool curGroundState = false;
        void UpdateAnimationProperties()
        {


            if (!_animator.runtimeAnimatorController) return;
            curGroundState = _characterInstance.isGrounded;

            _characterInstance.IsRunning = _characterInstance.ReadActionKeyCode(ActionCodes.Sprint) && !_characterInstance.ReadActionKeyCode(ActionCodes.Crouch)
                && _characterInstance.Input.Movement != Vector2.zero && _characterInstance.isGrounded && !_characterInstance.IsScoping;

            /*if (_characterInstance.IsRunning)
                _characterInstance.IsAbleToUseItem = false;*/

            _speed = (_characterInstance.IsRunning &&
                !_characterInstance.ReadActionKeyCode(ActionCodes.Trigger1) &&
                !_characterInstance.ReadActionKeyCode(ActionCodes.Trigger2) &&
                !_characterInstance.IsUsingItem &&
                !_characterInstance.IsScoping &&
                !_characterInstance.IsCrouching ? 1f : (_characterInstance.Input.Movement != Vector2.zero ? 1f : 0f));

            if (_characterInstance.isOwned && _characterInstance.ReadActionKeyCode(ActionCodes.Trigger2) && _speed == 2)

                //animate character
                if (!_animator.runtimeAnimatorController) return;

            _animator.SetFloat(AnimationNames.ITEM_SPEED, _speed); //universal parameter for fpp and tpp models

            _upperBodyAnimateFactor = System.Convert.ToInt32(!_characterInstance.IsRunning || _characterInstance.IsReloading || _characterInstance.IsScoping || !curGroundState);

            //_upperBodyAnimateFactor = _characterInstance.IsSkilling ? 0 : _upperBodyAnimateFactor;

            if (!_characterInstance.FPP) //parameter for tpp character model only
            {
                _animator.SetFloat(AnimationNames.CHARACTER_LOOK, -_characterInstance.Input.LookX);
                _animator.SetFloat(AnimationNames.CHARACTER_MOVEMENT_HORIZONTAL, _characterInstance.Input.Movement.x);
                _animator.SetFloat(AnimationNames.CHARACTER_MOVEMENT_VERTICAL, _characterInstance.Input.Movement.y);
                if (_animator.GetBool(AnimationNames.CHARACTER_ISGROUNDED) != curGroundState)
                    _animator.SetBool(AnimationNames.CHARACTER_ISGROUNDED, curGroundState);
                _animator.SetBool(AnimationNames.CHARACTER_ISCROUCHING, _characterInstance.IsCrouching);
                _animator.SetFloat("isCrouchingFloat", _characterInstance.IsCrouching ? 1f : 0f);
                _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), _upperBodyAnimateFactor, 12f * Time.deltaTime));
            }
        }

        void UpdateRotationAnimation(float timestep)
        {
            if (_characterInstance.Health.CurrentHealth <= 0) return;
            if (!_animator.runtimeAnimatorController) return;

            float difference = -Mathf.DeltaAngle(_characterInstance.Input.LookY, _currentBodyRotation);
            if (difference > 0 && Mathf.Abs(difference) > _rotationAngleThreshold ||
                difference < 0 && Mathf.Abs(difference) > _rotationAngleThresholdLeftSide)
            {
                if (!_adjustingRotation)
                {
                    if (_speed == 0)
                        _animator.CrossFade("turn", 0.05f);

                    _adjustingRotation = true;
                }
            }
            else if (Mathf.Abs(difference) < 1)
                _adjustingRotation = false;

            if (_characterInstance.Input.Movement != Vector2.zero || !_characterInstance.isGrounded)
                _currentBodyRotation = _characterInstance.Input.LookY;

            if (_adjustingRotation)
                _currentBodyRotation = ClampRotation(
                    Mathf.Lerp(_currentBodyRotation, _characterInstance.Input.LookY, _adjustingRotationLerpSpeed * Time.deltaTime),
                    _characterInstance.Input.LookY, _rotationAngleThreshold);


            _recoilX = Mathf.Lerp(_recoilX, 0f, _recoilStabilizationSpeed * timestep);

            _takingDamge_IKSpine = Vector3.Lerp(_takingDamge_IKSpine, Vector3.zero, _stabilizationAfterTakingDamageSpeed * timestep);
            _takingDamge_IKHead = Vector3.Lerp(_takingDamge_IKHead, Vector3.zero, _stabilizationAfterTakingDamageSpeed * 0.7f * timestep);

            _chestIK.weight = _upperBodyAnimateFactor;
            Vector3 chestRot = new Vector3(-_recoilX + _characterInstance.Input.LookX * 0.15f + _takingDamge_IKSpine.z, _characterInstance.Input.LookY, -_takingDamge_IKSpine.x);
            _chestIK.transform.localEulerAngles = chestRot;

            _headIK.weight = _upperBodyAnimateFactor;
            Vector3 headRot = new Vector3(_characterInstance.Input.LookX * 0.8f + _takingDamge_IKHead.z, _characterInstance.Input.LookY, -_takingDamge_IKHead.x);
            _headIK.transform.localEulerAngles = headRot;

            _animator.transform.eulerAngles = new Vector3(0, _currentBodyRotation, 0);
        }

        private void OnDamaged(int currentHealth, CharacterPart hittedPartID, AttackType attackType, byte attackerID)
        {
            Health attacker = GameSync.Singleton.Healths.GetObj(attackerID);

            if (!attacker) return;

            Vector3 direction = transform.position - attacker.transform.position;
            //Vector3 direction = Vector3.one;

            direction = transform.InverseTransformDirection(direction);
            direction.Normalize();

            if (hittedPartID != CharacterPart.head)
            {
                _takingDamge_IKSpine.x = direction.x * 30f + UnityEngine.Random.Range(-3f, 3f);
                _takingDamge_IKSpine.z = direction.z * 30f;
            }
            else
            {
                _takingDamge_IKSpine.x = direction.x * 20f + UnityEngine.Random.Range(-2f, 2f);
                _takingDamge_IKSpine.z = direction.z * 20f;

                _takingDamge_IKHead.x = direction.x * 58 + UnityEngine.Random.Range(-3f, 3f);
                _takingDamge_IKHead.z = direction.z * 58;
            }
        }

        public void AddRecoil(float recoil)
        {
            _recoilX = recoil;
        }

        float ClampRotation(float currentRot, float targetRot, float threshold)
        {
            float difference = Mathf.DeltaAngle(currentRot, targetRot);

            if (difference > threshold || difference < -threshold)
            {
                float angle = Mathf.Clamp(currentRot, targetRot - threshold, targetRot + threshold);
                return angle;
            }
            else
                return currentRot;
        }

        #region external access
        public void PlayAnimation(string name)
        {
            if (_animator.enabled)
                _animator.Play(name);
        }
        public void PlayAnimation(int hash) => _animator.Play(hash);
        public void SetTrigger(string triggerName)
        {
            if (_animator.gameObject.activeInHierarchy)
                _animator.SetTrigger(triggerName);
        }
        public void ResetTrigger(string triggerName) => _animator.ResetTrigger(triggerName);
        public void SetTrigger(int triggerHash) => _animator.SetTrigger(triggerHash);
        public void ResetTrigger(int triggerHash) => _animator.ResetTrigger(triggerHash);

        public void ResetAnimator()
        {
            _tppModelAnimator.Rebind();
            _tppModelAnimator.WriteDefaultValues();
        }

        public void RenderCharacterModel(bool render)
        {
            _rigBuilder.enabled = render;
            _tppModelAnimator.gameObject.SetActive(render);

            if (_characterInstance.IsObserved)
                _fppModelAnimator.gameObject.SetActive(render);
        }
        #endregion

        private void OnDestroy()
        {
            _characterInstance.Client_OnPerspectiveSet -= OnPerspectiveChanged;
            _characterItemManager.Client_EquipmentChanged -= AssingCharacterAnimationsForCurrentlyUsedItem;
            _characterInstance.Health.Client_OnHealthDepleted += OnHealthDepleted;

            _characterInstance = null;
            _characterItemManager = null;
        }
    }
}