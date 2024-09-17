using Mirror;
using MultiFPS;
using MultiFPS.Gameplay;
using MultiFPS.Sound;
using MultiFPS.UI.HUD;
using System;
using System.Threading.Tasks;
using UnityEngine;
using static MultiFPS.Gameplay.CharacterInstance;

public class DarkCoverSkill : NetworkBehaviour
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
    public GameObject fx_omen_01;
    public GameObject fx_omen_02;
    public GameObject fx_omen_03;
    public GameObject fx_omen_04;
    #endregion

    public string IsStart = "IsStart_E";
    public string IsSkilling = "IsSkilling";
    public string IsLoop = "IsLoop_E";
    public string IsEnd = "IsEnd_E";

    private void Start()
    {
    }

    private Vector3 targetPosition;
    public StepSkill CurrentStepSkill;
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

    public async void Action_Start()
    {
        hitDistance = 10;
        if (_charInstance != null)
            _charInstance.SwitchItemWhenUserSkill();
        CmdSetAnimStart();
        SetCharacterAni(CurrentStepSkill);
        if (isOwned)
            UICharacter._instance.SetActiveCountDownSkill_Omen_E(0);
        await Task.Delay(500);
        OnEventDarkCoverSkillFX_Start_01();

    }

    [Command]
    private void CmdSetAnimStart()
    {
        RpcSetAnimStart();
    }

    [ClientRpc]
    private void RpcSetAnimStart()
    {
        if (_charInstance != null)
            _charInstance.SwitchItemWhenUserSkill();
        CharAni._animator.SetBool(IsSkilling, _charInstance.IsSkilling);
        CharAni._animator.SetTrigger(IsStart);
    }

    private FxSkillController _fx_omen_01;
    public void OnEventDarkCoverSkillFX_Start_01()
    {
        if (this.gameObject == null) return;
        CmdSpawnObj(0);
    }
    private FxSkillController _fx_omen_02;
    public void Action_EquipState()
    {
        if (this == null) return;
        CmdSpawnObj(1);
    }

    [Command]
    private void CmdSpawnObj(int index)
    {
        if (isServer)
        {
            var nGo = (index) switch
            {
                0 => fx_omen_01,
                1 => fx_omen_02,
                2 => fx_omen_03,
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
                _fx_omen_01 = networkIdentity.GetComponent<FxSkillController>();
                Action_SetPositionFx(_fx_omen_01);
                break;
            case 1:
                _fx_omen_02 = networkIdentity.GetComponent<FxSkillController>();
                Action_SetPositionFx(_fx_omen_02);
                SetCharacterAni(CurrentStepSkill); 
                break;
            case 2:
                _fx_omen_03 = networkIdentity.GetComponent<FxSkillController>();
                Action_SetPositionFx(_fx_omen_03);
                break;
            default:
                
                break;
        }
    }

    public void OnCompleteAnimationStart()
    {
      
     
    }

    private FxSkillController _fx_omen_03;
    public void Action_CastState()
    {
  
    }

    public  void Action_ActivationState()
    {
 
    }

    public void IncreaseUsageDistanceSkill()
    {
        hitDistance = 25;
        if (isOwned)
            UICharacter._instance.SetActiveCountDownSkill_Omen_E(1);
    }

    public void DecreaseUsageDistanceSkill()
    {
        hitDistance = 10;
        if (isOwned)
            UICharacter._instance.SetActiveCountDownSkill_Omen_E(0);
    }

    public void Action_SetPositionFx(FxSkillController fxSkill)
    {
        fxSkill.StartFx();
        isFxSkill = true;
        Vector3 targetOffset = _charInstance.transform.forward * hitDistance;
        targetPosition = _charInstance.FPPLook.transform.position + targetOffset;
        float ySub = targetPosition.y / 3;
        fxSkill.transform.localPosition = new Vector3(targetPosition.x, targetPosition.y - ySub, targetPosition.z);
    }

    bool isFxSkill = false;
    float hitDistance = 0;
    private void FixedUpdate()
    {
        if (_charInstance == null)
        {
            StopFx();
            return;
        }
        if (isFxSkill && this._charInstance != null)
        {
            CmdUpdatePos();
        }
    }

    [Command]
    private void CmdUpdatePos()
    {
        RpcUpdatePos();
    }

    [ClientRpc]
    private void RpcUpdatePos()
    {
        Vector3 targetOffset = _charInstance.transform.forward * hitDistance;
        targetPosition = _charInstance.FPPLook.transform.position + targetOffset;
        float ySub = targetPosition.y / 3;
        if (_fx_omen_01 != null)
            _fx_omen_01.transform.localPosition = new Vector3(targetPosition.x, targetPosition.y - ySub, targetPosition.z);
        if (_fx_omen_02 != null)
            _fx_omen_02.transform.localPosition = new Vector3(targetPosition.x, targetPosition.y - ySub, targetPosition.z);
    }

    private Action _onCompleteAnimationEnd = null;
    public async void ActionSkill(Action onComplete)
    {
        if (_onCompleteAnimationEnd != null) return;
        if (UICharacter._instance._uiCountDown != null)
            UICharacter._instance._uiCountDown.gameObject.SetActive(false);

        _onCompleteAnimationEnd = onComplete;
        CurrentStepSkill = StepSkill.End;
        CmdSetAnimEnd();

        Vector3 targetOffset = _charInstance.transform.forward * hitDistance;
        Vector3 targetPosition = _charInstance.FPPLook.transform.position + targetOffset;
        float ySub = targetPosition.y - (targetPosition.y / 4);

        await Task.Delay(200);
        if (_fx_omen_01 != null)
        {
            CmdUnspawnObj(_fx_omen_01.gameObject, 0);
        }
        if (_fx_omen_02 != null)
        {
            CmdUnspawnObj(_fx_omen_02.gameObject, 1);
        }

        CmdSpawnObj(2);
        
        await Task.Delay(1000);
        _onCompleteAnimationEnd?.Invoke();

        await Task.Delay(9000);
        if (_fx_omen_03 != null)
        {
            CmdUnspawnObj(_fx_omen_03.gameObject, 2);
        }
        _onCompleteAnimationEnd?.Invoke();
        _onCompleteAnimationEnd = null;
    }

    [Command]
    private void CmdSetAnimEnd()
    {
        RpcSetAnimEnd();
    }

    [ClientRpc]
    private void RpcSetAnimEnd()
    {
        CharAni._animator.SetTrigger(IsEnd);
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
                _fx_omen_01 = null;
                break;
            case 1:
                _fx_omen_02 = null;
                break;
            case 2:
                _fx_omen_03 = null;
                PlayFX_Sound_DarkSmoke_Disappeared();
                break;
            case 3:
                
                break;
        }
    }

    public void OnCompleteAnimationEnd()
    {
    
    }

    public void ActionEnd(Action onComplete = null)
    {
        isFxSkill = false;
        DesTroyFxEnd();
        onComplete?.Invoke();
        _onCompleteAnimationEnd = null;
        CmdResetAnim();
        RequestReloadItem();
    }

    [Command]
    private void CmdResetAnim()
    {
        RpcResetAnim();
    }

    [ClientRpc]
    private void RpcResetAnim()
    {
        CharAni._animator.SetBool(IsSkilling, _charInstance.IsSkilling);
    }

    public void StopSkill(Action onComplete = null)
    {
        isFxSkill = false;
        DesTroyFxStop();
        CmdResetAnim();
        onComplete?.Invoke();
        _onCompleteAnimationEnd = null;
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

    public void DesTroyFxStop()
    {
        if (UICharacter._instance._uiCountDown != null)
            UICharacter._instance._uiCountDown.gameObject.SetActive(false);

        CurrentStepSkill = StepSkill.Start;
        if (_fx_omen_01 != null)
        {
            CmdUnspawnObj(_fx_omen_01.gameObject, 0);
        }
        if (_fx_omen_02 != null)
        {
            CmdUnspawnObj(_fx_omen_02.gameObject, 1);
        }
        if (_fx_omen_03 != null)
        {
            CmdUnspawnObj(_fx_omen_03.gameObject, 2);
        }
    }

    public async void DesTroyFxEnd()
    {
        if (UICharacter._instance._uiCountDown != null)
            UICharacter._instance._uiCountDown.gameObject.SetActive(false);

        CurrentStepSkill = StepSkill.Start;
        if (_fx_omen_01 != null)
        {
            if (_fx_omen_01.gameObject != null)
                CmdUnspawnObj(_fx_omen_01.gameObject, 0);
        }
        if (_fx_omen_02 != null)
        {
            if (_fx_omen_02.gameObject != null)
                CmdUnspawnObj(_fx_omen_02.gameObject, 1);
        }
        await Task.Delay(9000);
        if (_fx_omen_03 != null)
        {
            if (_fx_omen_03.gameObject != null)
                CmdUnspawnObj(_fx_omen_03.gameObject, 2);
        }
    }

    public void PlayFX_Sound_Start()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.DarkCoverSkill, 0);
        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.DarkCoverSkill, true);
        if (fxSkill == null) return;
        bool isLocal = fxSkill[0].IsLocal;
        if (!isLocal)
            Request_PlayFX_Sound_Start();
    }

    public void PlayFX_Sound_Cast()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.DarkCoverSkill, 1);
        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.DarkCoverSkill, true);
        if (fxSkill == null) return;
        bool isLocal = fxSkill[1].IsLocal;
        if (!isLocal)
            Request_PlayFX_Sound_Cast();
    }

    public void Omen_E_PlayFX_Sound_DarkSmoke()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.DarkCoverSkill, 2);
        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.DarkCoverSkill, true);
        if (fxSkill == null) return;
        bool isLocal = fxSkill[2].IsLocal;
        if (!isLocal)
            Request_Omen_E_PlayFX_Sound_DarkSmoke();
    }

    public void PlayFX_Sound_DarkSmoke_Disappeared()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.DarkCoverSkill, 3);
        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.DarkCoverSkill, true);
        if (fxSkill == null) return;
        bool isLocal = fxSkill[3].IsLocal;
        if (!isLocal)
            Request_PlayFX_Sound_DarkSmoke_Disappeared();
    }

    [Command]
    private void Request_PlayFX_Sound_Start()
    {
        Response_PlayFX_Sound_Start();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_Start()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.DarkCoverSkill, 0);
    }

    [Command]
    private void Request_PlayFX_Sound_Cast()
    {
        Response_PlayFX_Sound_Cast();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_Cast()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.DarkCoverSkill, 1);
    }

    [Command]
    private void Request_Omen_E_PlayFX_Sound_DarkSmoke()
    {
        Response_PlayFX_Sound_DarkSmoke();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_DarkSmoke()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.DarkCoverSkill, 2);
    }

    [Command]
    private void Request_PlayFX_Sound_DarkSmoke_Disappeared()
    {
        Response_PlayFX_Sound_DarkSmoke_Disappeared();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_DarkSmoke_Disappeared()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.DarkCoverSkill, 3);
    }

}
