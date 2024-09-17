using Mirror;
using MultiFPS;
using MultiFPS.Gameplay;
using MultiFPS.Sound;
using System;
using System.Threading.Tasks;
using UnityEngine;
using static MultiFPS.Gameplay.CharacterInstance;

public class ShroudedStepSkill : NetworkBehaviour
{
    public CharacterInstance _charInstance;
    public CharacterMotor CharMotor;
    public CharacterAnimator CharAni;
    protected Client_DataSkills.Param CurrentSkill;

    public CharacterController _controller;
    public Transform[] Pos_BattleFx;
    public Transform[] Pos_BattleFx_Hit;
    public Transform[] Pos_AuraFx;
    public GameObject[] Go_AttachFx;

    [SerializeField]
    private Transform posHandLeftFX;

    [SerializeField]
    private Transform posHandRightFX;
    public enum StepSkill
    {
        Start = 0,
        EquipState = 1,
        CastState = 2,
        ActivationState = 3,
        End = 4,
    }
  
    #region fx skill
    public GameObject fx_omen_01L;
    public GameObject fx_omen_01R;
    public GameObject fx_omen_02;
    public GameObject fx_omen_03;
    public GameObject fx_omen_04;
    #endregion

    public string IsStart = "IsStart_C";
    public string IsSkilling = "IsSkilling";
    public string IsLoop = "IsLoop_C";
    public string IsEnd = "IsEnd_C";

    private Vector3 targetPosition;
    public StepSkill CurrentStepSkill;
    int layerMask = 1 << 8;

    private void Start()
    {
        layerMask = ~layerMask;
    }

    public void StartSkill(Vector3 targetPosition)
    {
        CurrentStepSkill = StepSkill.Start;
        SetCharacterAni(CurrentStepSkill);
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
        if (_charInstance != null)
            _charInstance.SwitchItemWhenUserSkill();
        CmdSetAnimStart();
        SetCharacterAni(CurrentStepSkill);
    }

    [Command]
    private void CmdSetAnimStart()
    {
        SetAnimStart();
    }

    [ClientRpc]
    private void SetAnimStart()
    {
        if (_charInstance != null)
            _charInstance.SwitchItemWhenUserSkill();
    
        CharAni._animator.SetTrigger(IsStart);
    }

    private FxSkillController _fx_omen_01_left;
    private FxSkillController _fx_omen_01_right;
    public void Action_EquipState()
    {
        CmdSpawnObj(0);
        CmdSpawnObj(1);
        CmdSpawnObj(2);
        CmdSpawnObj(3);
        SetCharacterAni(CurrentStepSkill);
    }

    [Command]
    private void CmdSpawnObj(int index)
    {
        if (isServer)
        {
            var nGo = (index) switch
            {
                0 => fx_omen_01L,
                1 => fx_omen_01R,
                2 => fx_omen_02,
                3 => fx_omen_03,
                _ => fx_omen_04,
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
                _fx_omen_01_left = networkIdentity.GetComponent<FxSkillController>();
                _fx_omen_01_left.transform.SetParent(posHandLeftFX.transform);
                _fx_omen_01_left.transform.localPosition = Vector3.zero;
                break; 
            case 1:
                _fx_omen_01_right = networkIdentity.GetComponent<FxSkillController>();
                _fx_omen_01_right.transform.SetParent(posHandRightFX.transform);
                _fx_omen_01_right.transform.localPosition = Vector3.zero;
                break; 
            case 2:
                _fx_omen_02 = networkIdentity.GetComponent<FxSkillController>();
                Action_SetPositionFx(_fx_omen_02);
                break;
            case 3:
                _fx_omen_03 = networkIdentity.GetComponent<FxSkillController>();
                Action_SetPositionFx(_fx_omen_03);
                break;
            default:
                _fx_omen_04 = networkIdentity.GetComponent<FxSkillController>();
                _fx_omen_04.StartFx();
                break;
        }
    }

    private FxSkillController _fx_omen_02;
    public void Action_CastState()
    {
        SetCharacterAni(CurrentStepSkill);
    }

    private FxSkillController _fx_omen_03;
    public void Action_ActivationState()
    {
        CmdSetAnimLoop();
    }

    [Command]
    private void CmdSetAnimLoop()
    {
        SetAnimLoop();
    }

    [ClientRpc]
    private void SetAnimLoop()
    {
        CharAni._animator.SetTrigger(IsLoop);
    }

    public void Action_SetPositionFx(FxSkillController fxSkill)
    {
        fxSkill.StartFx();
        isFxSkill = true;
        fxSkill.transform.localPosition = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
    }

    bool isFxSkill = false;
    float hitDistance = 0;
    private void Update()
    {
        if (_charInstance == null)
        {
            CmdDesTroyFx();
            return;
        }
        if (isFxSkill && _charInstance != null)
        {
            Vector3 targetOffset = Vector3.zero;
            var originPos = _charInstance.transform.position + Vector3.up * 2f;
            if (Physics.Raycast(originPos, _charInstance.transform.forward, out RaycastHit hit, 15, layerMask))
            {

                if (hit.collider != null)
                {
                    targetOffset = hit.point - _charInstance.transform.forward * 3;
                }
            }
            targetOffset = targetOffset == Vector3.zero ? _charInstance.transform.position + _charInstance.transform.forward * 15 : targetOffset;
            CmdSetFxPos(targetOffset);
        }
    }

    [Command]
    private void CmdSetFxPos(Vector3 targetOffset)
    {
        RpcSetFxPos(targetOffset);
    }

    [ClientRpc]
    private void RpcSetFxPos(Vector3 targetOffset)
    {
        if (!_fx_omen_02 || !_fx_omen_03 || targetOffset == Vector3.zero) return;

        targetPosition = targetOffset;
        targetPosition.y += 5;
        _fx_omen_02.transform.position = targetPosition;
        if (Physics.Raycast(targetPosition, _fx_omen_02.transform.TransformDirection(Vector3.down), out RaycastHit hit, 20, layerMask))
        {
            if (hit.collider != null)
            {
                _fx_omen_02.transform.position = hit.point;
                _fx_omen_03.transform.localPosition = hit.point;
            }
        }
        else
        {
            _fx_omen_02.transform.position = transform.position;
            _fx_omen_03.transform.localPosition = transform.position;
        }

        targetPosition = _fx_omen_02.transform.position;
    }

    Action onEventShroudedStepEnd = null;
    public void ActionSkill(Action onComplete)
    {
        if (onEventShroudedStepEnd != null) return;
        CmdSetanimEnd();
        onEventShroudedStepEnd = onComplete;
    }

    [Command]
    private void CmdSetanimEnd()
    {
        SetanimEnd();
    }

    [ClientRpc]
    private void SetanimEnd()
    {
        CharAni._animator.SetTrigger(IsEnd);
    }

    public void OnEventShroudedStepEnd()
    {
        _charInstance.PrepareCharacterToLerp();
        var dist = targetPosition - _charInstance.transform.position;
        _controller.Move(dist);
        _charInstance.transform.position = dist;
        _charInstance.SetCurrentPositionTargetToLerp(_charInstance.transform.position);
        onEventShroudedStepEnd?.Invoke();
        onEventShroudedStepEnd = null;
    }

    FxSkillController _fx_omen_04;
    public async void ActionEnd(Action onComplete = null)
    {
        isFxSkill = false;
        {
            CmdDesTroyFx();
        }

        if (fx_omen_04 != null)
        {
            CmdSpawnObj(4);
            await Task.Delay(200);
            if (_fx_omen_04 != null)
                CmdUnspawnObj(_fx_omen_04.gameObject, 4);
        }
    
        await Task.Delay(500);
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

    [Command]
    private void CmdEndAnim()
    {
        EndAnim();
    }

    [ClientRpc]
    private void EndAnim()
    {
        CharAni._animator.SetBool(IsSkilling, _charInstance.IsSkilling);
    }

    public void StopSkill(Action onComplete = null)
    {
        isFxSkill = false;
        if (isServer)
        { 
            CmdDesTroyFx();
        }

        //CmdEndAnim();
        onComplete?.Invoke();
        RequestReloadItem();

    }

    public void StopFx()
    {
        CmdDesTroyFx();
    }

    public void CmdDesTroyFx()
    {
        if(_fx_omen_01_left != null)
        {
            CmdUnspawnObj(_fx_omen_01_left.gameObject, 0);
        }
        if (_fx_omen_01_right != null)
        {
            CmdUnspawnObj(_fx_omen_01_right.gameObject, 1);
        }
        if (_fx_omen_02 != null)
        {
            CmdUnspawnObj(_fx_omen_02.gameObject, 2);
        }
        if (_fx_omen_03 != null)
        {
            CmdUnspawnObj(_fx_omen_03.gameObject, 3);
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
        if (_charInstance == null)
            return;
        NetworkServer.UnSpawn(fx);
        Destroy(fx);
        switch (index)
        {
            case 0:
                _fx_omen_01_left = null;
                break;
            case 1:
                _fx_omen_01_right = null;
                break;
            case 2:
                _fx_omen_02 = null;
                break;
            case 3:
                _fx_omen_03 = null;
                break;
            case 4:
                _fx_omen_04 = null;
                break;
        }
    }

    public void SetCharacterAni_Server(StepSkill stepSkill)
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

    public void PlayFX_Sound_Start()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.ShroudedStepSkill, 0);

        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.ShroudedStepSkill, true);
        if (fxSkill == null) return;
        bool isLocal = fxSkill[0].IsLocal;
        if (!isLocal)
            Request_PlayFX_Sound_Start();
    }

    public void PlayFX_Sound_Cast()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.ShroudedStepSkill, 1);

        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.ShroudedStepSkill, true);
        if (fxSkill == null) return;
        bool isLocal = fxSkill[1].IsLocal;
        if (!isLocal)
            Request_PlayFX_Sound_Cast();

    }

    [Command]
    private void Request_PlayFX_Sound_Start()
    {
        Response_PlayFX_Sound_Start();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_Start()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.ShroudedStepSkill, 0);
    }

    [Command]
    private void Request_PlayFX_Sound_Cast()
    {
        Response_PlayFX_Sound_Cast();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_Cast()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.ShroudedStepSkill, 1);
    }

}
