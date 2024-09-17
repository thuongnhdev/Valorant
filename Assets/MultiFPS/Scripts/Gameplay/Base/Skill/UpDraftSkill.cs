using Mirror;
using MultiFPS;
using MultiFPS.Gameplay;
using MultiFPS.Sound;
using System;
using System.Threading.Tasks;
using UnityEngine;
using static MultiFPS.Gameplay.CharacterInstance;

public class UpDraftSkill : NetworkBehaviour
{
    public CharacterInstance _charInstance;
    public CharacterMotor CharMotor;
    public CharacterAnimator CharAni;
    protected Client_DataSkills.Param CurrentSkill;

    public CharacterController _controller;

    [SerializeField]
    private Transform posHandFX;

    public enum StepSkill
    {
        Start = 0,
        EquipState = 1,
        CastState = 2,
        ActivationState = 3,
        End = 4,

    }

    #region fx skill
    public GameObject fx_jett_q_trail;
    public GameObject fx_jett_q_ready;
    #endregion

    public string IsStart = "Jett_Ability_q";
    public string IsSkilling = "IsSkilling";

    private void Start()
    {
    }

    private Vector3 targetPosition;
    private Vector3 _targetPosition;
    public StepSkill CurrentStepSkill;

    bool isFxSkill = false;
    Action _onCompleteSkill;
    public void StartSkill(Vector3 targetPosition, Action onComplete)
    {
        isFxSkill = true;
        Vector3 targetOffset = _charInstance.transform.up;
        positionCharacterFly = targetOffset;
  
        _onCompleteSkill = onComplete;
        
        CurrentStepSkill = StepSkill.Start;
        SetCharacterAni(CurrentStepSkill);
        CharMotor.Gravity = 5;
    }

    public void SetCharacterAni(StepSkill stepSkill)
    {
        switch (stepSkill)
        {
            case StepSkill.Start:
                CurrentStepSkill = StepSkill.EquipState;
                Action_Start();
                break;
            case StepSkill.EquipState:
                CurrentStepSkill = StepSkill.CastState;
                Action_EquipState();
                break;
            case StepSkill.CastState:
                CurrentStepSkill = StepSkill.ActivationState;
                Action_CastState();
                break;
            case StepSkill.ActivationState:
                Action_ActivationState();
                break;
        }
    }

    public void Action_Start()
    {
        CmdResetAnim();

        SetCharacterAni(CurrentStepSkill);
        ActionSkill(_onCompleteSkill);
    }

    [Command]
    private void CmdResetAnim()
    {
        if (_charInstance != null)
            _charInstance.SwitchItemWhenUserSkill();
        RpcResetAnim();
    }

    [ClientRpc]
    private void RpcResetAnim()
    {
        if (_charInstance != null)
            _charInstance.SwitchItemWhenUserSkill();
        CharAni._animator.SetBool(IsSkilling, _charInstance.IsSkilling);
    }

    private FxSkillController _fx_jett_q_trail = null;
    private FxSkillController _fx_jett_q_ready;
    public void Action_EquipState()
    {
        SetCharacterAni(CurrentStepSkill);
    }

    public void Action_CastState()
    {
        SetCharacterAni(CurrentStepSkill);
    }

    public void Action_ActivationState()
    {

    }

    public void Action_SetPositionFx(FxSkillController fxSkill)
    {
        fxSkill.StartFx();
        fxSkill.transform.localPosition = new Vector3(posHandFX.transform.position.x, 3.7f, posHandFX.transform.position.z);

        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        Vector3 tar_y = new Vector3(fxSkill.transform.position.x, 3.7f, fxSkill.transform.position.z);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(tar_y, fxSkill.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
        {
            fxSkill.transform.position = hit.point;
        }

    }

    private void FixedUpdate()
    {
        if (isFxSkill)
        {
            if (hitDistance < 40)
            {
                if (hitDistance == 0 && _fx_jett_q_trail == null)
                {
                    durationY = (float)(hitDistance + 130) / 500;
                    ActionFx();
                }

                hitDistance += 1;
                durationY += 0.00025f;
                Vector3 target = new Vector3(positionCharacterFly.x, durationY, positionCharacterFly.z);
                _charInstance.PrepareCharacterToLerp();
                //finally move character
                //this additional isGrounded check is for avoiding character controller to think that it is not grounded while player is going down stairs fast
                _controller.Move(target);
                _charInstance.transform.position = target;
                _charInstance.SetCurrentPositionTargetToLerp(_charInstance.transform.position);
                positionCharacterFly = _charInstance.transform.position;


            }
            else if (hitDistance == 40)
            {
                hitDistance += 1;
                ActionEnd(() =>
                {
                    isFxSkill = false;
                    hitDistance = 0;
                    _onCompleteSkill?.Invoke();
                });
            }
        }
    }

    float durationY = 0;
    int hitDistance = 0;
    Vector3 positionCharacterFly;
    public void ActionSkill(Action onComplete)
    {
        isFxSkill = true;
        CmdSpawnObj(1);
    }

    void ActionFx()
    {
        CmdAnimStart();
        CmdSpawnObj(0);
    }

    [Command]
    private void CmdAnimStart()
    {
        RpcAnimStart();
    }

    [ClientRpc]
    private void RpcAnimStart()
    {
        CharAni._animator.SetTrigger(IsStart);
    }

    [Command]
    private void CmdSpawnObj(int index)
    {
        if (isServer)
        {
            var nGo = (index) switch
            {
                0 => fx_jett_q_trail,
                _ => fx_jett_q_ready,
            };
            var go = Instantiate(nGo, transform.position, transform.rotation);
            NetworkServer.Spawn(go);
            go.GetComponent<NetworkIdentity>().AssignClientAuthority(netIdentity.connectionToClient);
            RpcSpawnObj(go, index);
        }
    }

    [ClientRpc]
    private void RpcSpawnObj(GameObject networkIdentity, int index)
    {
        switch (index)
        {
            case 0:
                _fx_jett_q_trail = networkIdentity.GetComponent<FxSkillController>();
                Action_SetPositionFx(_fx_jett_q_trail);
                break;
            case 1:
                _fx_jett_q_ready = networkIdentity.GetComponent<FxSkillController>();
                if (isOwned)
                {
                    posHandFX.gameObject.SetActive(true);
                    _fx_jett_q_ready.transform.SetParent(posHandFX.transform);
                    _fx_jett_q_ready.transform.localPosition = Vector3.zero;
                }
                else
                {
                    Action_SetPositionFx(_fx_jett_q_ready);
                }
                break;
        }
    }

    [Command]
    private void CmdUnspawnObj(GameObject fx, int index)
    {
        if (isServer && fx != null)
        {
            var ident = fx.GetComponent<NetworkIdentity>();
            if (ident != null) ident.RemoveClientAuthority();
        }
        RpcUnspawnObj(fx, index);
    }

    [ClientRpc]
    private void RpcUnspawnObj(GameObject fx, int index)
    {
        NetworkServer.UnSpawn(fx);
        Destroy(fx);
        switch (index)
        {
            case 0:
                _fx_jett_q_trail = null;
                break;
            case 1:
                _fx_jett_q_ready = null;
                break;
        }
    }

    public async void ActionEnd(Action onComplete = null)
    {
        isFxSkill = false;
        DesTroyFxEnd();

        await Task.Delay(500);
        onComplete?.Invoke();
        CmdResetAnim();
        RequestReloadItem();
    }

    public void StopSkill(Action onComplete = null)
    {
        isFxSkill = false;
        DesTroyFxStop();

        CmdResetAnim();
        onComplete?.Invoke();
        RequestReloadItem();
    }

    [Command]
    private void RequestReloadItem()
    {
        if (_charInstance != null)
            _charInstance.SetAnimReloadItem();
        ResponseReloadItem();
    }

    [ClientRpc]
    private void ResponseReloadItem()
    {
        if (_charInstance != null)
            _charInstance.SetAnimReloadItem();
    }


    public void StopFx()
    {
        DesTroyFxStop();
    }

    public void DesTroyFxEnd()
    {
        if (_fx_jett_q_ready != null)
        {
            CmdUnspawnObj(_fx_jett_q_ready.gameObject, 1);
        }

        if (_fx_jett_q_trail != null)
        {
            CmdUnspawnObj(_fx_jett_q_trail.gameObject, 0);
        }
    }

    public void DesTroyFxStop()
    {
        if (_fx_jett_q_ready != null)
        {
            CmdUnspawnObj(_fx_jett_q_ready.gameObject, 1);
        }

        if (_fx_jett_q_trail != null)
        {
            CmdUnspawnObj(_fx_jett_q_trail.gameObject, 0);
        }
    }

    public void PlayFX_Sound_Active()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.UpdraftSkill, 0);
        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.UpdraftSkill, true);
        if (fxSkill == null) return;
        bool isLocal = fxSkill[0].IsLocal;
        if (!isLocal)
            Request_PlayFX_Sound_Active();
    }

    [Command]
    private void Request_PlayFX_Sound_Active()
    {
        Response_PlayFX_Sound_Active();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_Active()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.UpdraftSkill, 0);
    }
}
