using DG.Tweening;
using Mirror;
using MultiFPS;
using MultiFPS.Gameplay;
using MultiFPS.Sound;
using System;
using System.Threading.Tasks;
using UnityEngine;
using static MultiFPS.Gameplay.CharacterInstance;

public class CloudBurstSkill : NetworkBehaviour
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
    public GameObject fx_jett_c_bullet;
    public GameObject fx_jett_c_ready;
    #endregion

    public string IsStart = "Jett_Ability_c";
    public string IsSkilling = "IsSkilling";

    private void Start()
    {
        
    }

    private Vector3 targetPosition;
    private Vector3 _targetPosition;
    public StepSkill CurrentStepSkill;

    bool isFxSkill = false;
    Action _onCompleteSkill = null;
    public void StartSkill(Vector3 targetPosition,Action onComplete)
    {
        _onCompleteSkill = onComplete;
        _targetPosition = targetPosition;
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
        CmdAnimStart();
        SetCharacterAni(CurrentStepSkill);
    }

    [Command]
    private void CmdAnimStart()
    {
        RpcAnimStart();
    }

    [ClientRpc]
    private void RpcAnimStart()
    {
        if (_charInstance != null)
            _charInstance.SwitchItemWhenUserSkill();
        CharAni._animator.SetTrigger(IsStart);
    }

    public void OnEventActionSkillCloudBurst()
    {
        ActionSkill(_onCompleteSkill);
    }

    private FxSkillController _fx_jett_c_bullet;
    private FxSkillController _fx_jett_c_ready;
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
        isFxSkill = true;
        fxSkill.transform.localPosition = posHandFX.transform.position;
        CmdUpdateBulletPos();
    }

    public void ActionSkill(Action onComplete)
    {
        CmdSpawnObj(0);
    }

    [Command]
    private void CmdUpdateBulletPos()
    {
        RpcUpdateBulletPos();
    }

    [ClientRpc]
    private void RpcUpdateBulletPos()
    {
        targetPosition = _targetPosition + _charInstance.transform.forward * 20;
        _fx_jett_c_bullet.transform.DOMove(targetPosition, 0.9f).OnComplete(() =>
        {
            CmdSpawnObj(1);
            ActionEnd(() =>
            {
                isFxSkill = false;
                _onCompleteSkill?.Invoke();
            });

            CmdUnspawnObj(_fx_jett_c_bullet.gameObject, 0);
        });
    }

    [Command]
    private void CmdSpawnObj(int index)
    {
        if (isServer)
        {
            var nGo = (index) switch
            {
                0 => fx_jett_c_bullet,
                _ => fx_jett_c_ready,
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
                _fx_jett_c_bullet = networkIdentity.GetComponent<FxSkillController>();
                Action_SetPositionFx(_fx_jett_c_bullet);
                break;
            case 1:
                _fx_jett_c_ready = networkIdentity.GetComponent<FxSkillController>();
                _fx_jett_c_ready.transform.localPosition = targetPosition;
                _fx_jett_c_ready.StartFx();
                PlayFX_Sound_ExpandFullSize();
                PlayFX_Sound_SmokeFullSize();
                break;
        }
    }

    [Command]
    private async void CmdUnspawnObj(GameObject fx, int index)
    {
        int timeDelay = index == 1 ? 5000 : 0;
        await Task.Delay(timeDelay);
        if (_charInstance == null) return;
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
                _fx_jett_c_bullet = null;
                break;
            case 1:
                _fx_jett_c_ready = null;
                break;
        }
    }

    public async void ActionEnd(Action onComplete = null)
    {
        isFxSkill = false;
        DesTroyFxEnd();

        await Task.Delay(500);
        onComplete?.Invoke();
        RequestReloadItem();
        //CmdResetAnim();
    }

    public void StopSkill(Action onComplete = null)
    {
        isFxSkill = false;
        DesTroyFxStop();

        //CmdResetAnim();
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
    private void CmdResetAnim()
    {
        RpcResetAnim();
    }

    [ClientRpc]
    private void RpcResetAnim()
    {
        CharAni._animator.SetBool(IsSkilling, _charInstance.IsSkilling);
    }

    public void StopFx()
    {
        DesTroyFxStop();
    }

    public async void DesTroyFxEnd()
    {
        if (_fx_jett_c_bullet != null)
        {
            CmdUnspawnObj(_fx_jett_c_bullet.gameObject, 0);
        }

        await Task.Delay(7000);
      
        if (_fx_jett_c_ready != null)
        {
            CmdUnspawnObj(_fx_jett_c_ready.gameObject, 1);
        }
    }

    public void DesTroyFxStop()
    {
        if (_fx_jett_c_bullet != null)
        {
            CmdUnspawnObj(_fx_jett_c_bullet.gameObject, 0);
        }
        if (_fx_jett_c_ready != null)
        {
            CmdUnspawnObj(_fx_jett_c_ready.gameObject, 1);
        }

    }

    public void Start_Skill_Server(Vector3 targetPosition)
    {

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

    public void PlayFX_Sound_Cast()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.CloudBurstSkill, 0);
        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.CloudBurstSkill, true);
        if (fxSkill == null) return;
        bool isLocal = fxSkill[0].IsLocal;
        if (!isLocal)
            Request_PlayFX_Sound_Cast();
    }

    public void PlayFX_Sound_ExpandFullSize()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.CloudBurstSkill, 1);
        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.CloudBurstSkill, true);
        if (fxSkill == null) return;
        bool isLocal = fxSkill[1].IsLocal;
        if (!isLocal)
            Request_PlayFX_Sound_ExpandFullSize();
    }

    public void PlayFX_Sound_SmokeFullSize()
    {
        SoundManager.Instance.Play_Battle_HeroSkill(CharacterInstance.Skill.CloudBurstSkill, 0, true, 1, 2000);
        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.CloudBurstSkill, false);
        if (fxSkill == null) return;
        bool isLocal = fxSkill[0].IsLocal;
        if (!isLocal)
            Request_PlayFX_Sound_SmokeFullSize();   
    }

    [Command]
    private void Request_PlayFX_Sound_Cast()
    {
        Response_PlayFX_Sound_Cast();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_Cast()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.CloudBurstSkill, 0);
    }

    [Command]
    private void Request_PlayFX_Sound_ExpandFullSize()
    {
        Response_PlayFX_Sound_ExpandFullSize();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_ExpandFullSize()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.CloudBurstSkill, 1);
    }

    [Command]
    private void Request_PlayFX_Sound_SmokeFullSize()
    {
        Response_PlayFX_Sound_SmokeFullSize();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_SmokeFullSize()
    {
        SoundManager.Instance.Play_Battle_HeroSkill(CharacterInstance.Skill.CloudBurstSkill, 0, true, 1, 2000);
    }
}
