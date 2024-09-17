using MultiFPS.UI;
using MultiFPS.UI.HUD;
using UnityEngine;

namespace MultiFPS.Gameplay
{
    [DisallowMultipleComponent]
    /// <summary>
    /// This class is always instantiated on scene once, and will manage player input, so we dont
    /// have to check on every player instance separately if it belongs to us, we can just apply input
    /// read from this script to one spawned player prefab that is ours
    /// </summary>
    public class PlayerGameplayInput : MonoBehaviour
    {
        public static PlayerGameplayInput Instance { private set; get; }
        public InputSystem InputSystem1 { get => _inputSystem; set => _inputSystem = value; }

        private CharacterInstance _myCharIntance;
        private CharacterMotor _motor;

        [SerializeField] InputSystem _inputSystem = InputSystem.NewInputSystem;

        private MultiFPSInputs _input;
        private Vector2 _lookInput;
        private Vector2 _movementInput;

        public enum InputSystem
        {
            Legacy,
            NewInputSystem,
            MobileInput,
        }

        private void Awake()
        {
            Instance = this;
        }

        void Update()
        {
            if (!_myCharIntance) return;

            //apply user input
            if (ClientFrontend.GamePlayInput())
            {
                //read old unity system input system if it's selected to be used
                if (InputSystem1 == InputSystem.Legacy)
                {
                    _lookInput.x = -Input.GetAxis("Mouse Y");
                    _lookInput.y = Input.GetAxis("Mouse X");

                    _movementInput.x = Input.GetAxis("Horizontal");
                    _movementInput.y = Input.GetAxis("Vertical");

                    //character related input
                    if (Input.GetKeyDown(KeyCode.Space)) _motor.Jump();

                    //if (Input.GetKeyDown(KeyCode.E)) _myCharIntance.CharacterItemManager.TryGrabItem();
                    if (Input.GetKeyDown(KeyCode.G)) _myCharIntance.CharacterItemManager.TryDropItem();

                    if (Input.GetKeyDown(KeyCode.Alpha1)) _myCharIntance.CharacterItemManager.ClientTakeItem(0);
                    if (Input.GetKeyDown(KeyCode.Alpha2)) _myCharIntance.CharacterItemManager.ClientTakeItem(1);
                    if (Input.GetKeyDown(KeyCode.Alpha3)) _myCharIntance.CharacterItemManager.ClientTakeItem(2);
                    if (Input.GetKeyDown(KeyCode.Alpha4)) _myCharIntance.CharacterItemManager.ClientTakeItem(3);
                    if (Input.GetKeyDown(KeyCode.X)) _myCharIntance.CharacterItemManager.ClientTakeItem(4);


                    if (Input.GetKeyDown(KeyCode.R)) _myCharIntance.CharacterItemManager.Reload();
                    if (Input.GetKeyDown(KeyCode.Q)) _myCharIntance.CharacterItemManager.TakePreviousItem();
                    if (Input.GetKey(KeyCode.V) && _myCharIntance.CharacterItemManager.CurrentlyUsedItem)
                        _myCharIntance.CharacterItemManager.CurrentlyUsedItem.PushMeele();

                    _myCharIntance.SetActionKeyCode(ActionCodes.Sprint, Input.GetKey(KeyCode.LeftShift));
                    _myCharIntance.SetActionKeyCode(ActionCodes.Crouch, Input.GetKey(KeyCode.C));

                    _myCharIntance.SetActionKeyCode(ActionCodes.Trigger2, Input.GetMouseButton(1));
                    _myCharIntance.SetActionKeyCode(ActionCodes.Trigger1, Input.GetMouseButton(0));
                }
                else if (InputSystem1 == InputSystem.MobileInput)
                {
                    _lookInput.x = -Input.GetAxis("Mouse Y");
                    _lookInput.y = Input.GetAxis("Mouse X");

                    _movementInput.x = UICharacter._instance.joystick.Horizontal();
                    _movementInput.y = UICharacter._instance.joystick.Vertical();

                }
                else
                {
                    _lookInput.x = -_input.Player.Look.ReadValue<Vector2>().y / 10;
                    _lookInput.y = _input.Player.Look.ReadValue<Vector2>().x / 10;

                    _movementInput = _input.Player.Movement.ReadValue<Vector2>();

                    if (_input.Player.Jump.WasPressedThisFrame()) _motor.Jump();

                    if (_input.Player.Interaction.WasPressedThisFrame()) _myCharIntance.CharacterItemManager.TryGrabItem();
                    if (_input.Player.DropItem.WasPressedThisFrame()) _myCharIntance.CharacterItemManager.TryDropItem();

                    if (_input.Player.TakeItem1.WasPressedThisFrame()) _myCharIntance.CharacterItemManager.ClientTakeItem(0);
                    if (_input.Player.TakeItem2.WasPressedThisFrame()) _myCharIntance.CharacterItemManager.ClientTakeItem(1);
                    if (_input.Player.TakeItem3.WasPressedThisFrame()) _myCharIntance.CharacterItemManager.ClientTakeItem(2);
                    if (_input.Player.TakeItem4.WasPressedThisFrame()) _myCharIntance.CharacterItemManager.ClientTakeItem(3);
                    if (_input.Player.TakeItem_Bomb.WasPressedThisFrame()) _myCharIntance.CharacterItemManager.ClientTakeItem(4);


                    if (_input.Player.Reload.WasPressedThisFrame()) _myCharIntance.CharacterItemManager.Reload();
                    //if (_input.Player.TakePreviousItem.WasPressedThisFrame()) _myCharIntance.CharacterItemManager.TakePreviousItem();
                    if (_input.Player.MeleeAttack.WasPressedThisFrame() && _myCharIntance.CharacterItemManager.CurrentlyUsedItem)
                        _myCharIntance.CharacterItemManager.CurrentlyUsedItem.PushMeele();

                    _myCharIntance.SetActionKeyCode(ActionCodes.Sprint, _input.Player.Sprint.ReadValue<float>() > 0);

                    _myCharIntance.SetActionKeyCode(ActionCodes.Crouch, _input.Player.Crouch.ReadValue<float>() > 0);

                    _myCharIntance.SetActionKeyCode(ActionCodes.Trigger2, _input.Player.Fire2Hold.ReadValue<float>() > 0);
                    _myCharIntance.SetActionKeyCode(ActionCodes.Trigger1, _input.Player.Fire1Hold.ReadValue<float>() > 0);

                    _myCharIntance.Input.LookY += _lookInput.y * UserSettings.MouseSensitivity * _myCharIntance.SensitivityItemFactorMultiplier;
                    _myCharIntance.Input.LookX += _lookInput.x * UserSettings.MouseSensitivity * _myCharIntance.SensitivityItemFactorMultiplier;

                }
                _myCharIntance.Input.Movement = _movementInput;
            }
            else
            {
                _myCharIntance.Input.Movement = Vector2.zero;
                _myCharIntance.SetActionKeyCode(ActionCodes.Sprint, false);
            }

            //game managament related input
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (ClientFrontend.GamemodeUI)
                    ClientFrontend.GamemodeUI.Btn_ShowTeamSelector();
            }
        }

        public void AssignCharacterToBeControlledByPlayer(CharacterInstance character)
        {

            _myCharIntance = character;
            _motor = character.GetComponent<CharacterMotor>();
        }
        #region Mobile Version
        public void Jump()
        {
            _motor.Jump();
        }
        public void Reload()
        {
            _myCharIntance.CharacterItemManager.Reload();
        }
        public void Shoot()
        {
            _myCharIntance.CharacterItemManager.Fire1();
        }
        public void Aim()
        {
       
            _myCharIntance.CharacterItemManager.Fire2();
        }

        public void Sprint()
        {
            _myCharIntance.SetActionKeyCode(ActionCodes.Sprint, _input.Player.Sprint.ReadValue<float>() == 1);
        }

        public void Cround()
        {
            _myCharIntance.IsCrouching = !_myCharIntance.IsCrouching;

            if (_myCharIntance.IsCrouching)
            {
                _motor.Crouch();
            }
            else
                _motor.Stand();
        }
        public void TakeItem1()
        {
            _myCharIntance.CharacterItemManager.ClientTakeItem(0);
        }

        public void TakeItem2()
        {
            _myCharIntance.CharacterItemManager.ClientTakeItem(1);
        }
        public void TakeKnife()
        {
            _myCharIntance.CharacterItemManager.ClientTakeItem(2);
        }

        public Vector2 MovementInput()
        {
            return _movementInput;
        }

        public void DropItem()
        {
            _myCharIntance.CharacterItemManager.TryDropItem();
        }
        #endregion

        #region new input system implementation
        private void OnEnable()
        {

            if (InputSystem1 == InputSystem.NewInputSystem)
            {
                if (_input == null)
                    _input = new MultiFPSInputs();

                _input.Enable();
            }

        }
        private void OnDisable()
        {
            if (InputSystem1 == InputSystem.NewInputSystem)
            {
                _input.Disable();
            }
        }
        #endregion
    }
}