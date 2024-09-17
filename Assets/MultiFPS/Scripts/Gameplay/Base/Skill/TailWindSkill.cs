using Mirror;
using MultiFPS;
using MultiFPS.Gameplay;
using MultiFPS.Sound;
using MultiFPS.UI.HUD;
using System;
using System.Threading.Tasks;
using UnityEngine;
using static MultiFPS.Gameplay.CharacterInstance;

public class TailWindSkill : NetworkBehaviour
{
    public CharacterInstance _charInstance;
    public CharacterMotor CharMotor;
    public CharacterAnimator CharAni;
    protected Client_DataSkills.Param CurrentSkill;

    public CharacterController _controller;

    [SerializeField]
    private Transform posLootLeftFX;

    [SerializeField]
    private Transform posLootRightFX;

    [SerializeField]
    private Transform posHandLeftFX;

    [SerializeField]
    private Transform posHandRightFX;

    [SerializeField]
    private Transform posHandReadyFX;
    public enum StepSkill
    {
        Start = 0,
        EquipState = 1,
        CastState = 2,
        ActivationState = 3,
        End = 4,

    }

    #region fx skill
    public GameObject fx_jett_e_buf_L;
    public GameObject fx_jett_e_buf_R;
    public GameObject fx_jett_e_ready;
    public GameObject fx_jett_e_ready_speed;
    public GameObject fx_jett_e_trail_speed;
    #endregion

    public string IsAction = "Jett_Ability_e";
    public string IsSkilling = "IsSkilling";

    private void Start()
    {
    }

    public StepSkill CurrentStepSkill;


    public void StartSkill(Vector3 targetPosition)
    {
        PlayFX_Sound_Cast();
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
        SetCharacterAni(CurrentStepSkill);
    }

    private FxSkillController _fx_jett_e_buff_L;
    private FxSkillController _fx_jett_e_buff_R;
    private FxSkillController _fx_jett_e_ready;
    private FxSkillController _fx_jett_e_ready_speed;
    private FxSkillController _fx_jett_e_trail_speed;
    public void Action_EquipState()
    {
        posHandReadyFX.gameObject.SetActive(true);
        CmdSpawnObj(0);
        CmdSpawnObj(1);
        CmdSpawnObj(2);
        if (isOwned)
        {
            UICharacter._instance.SetActiveCountDownSkill_Jett_E(_charInstance.IsSkilling, (isComplete) => {
                _charInstance.Action_Skill_Jett_E_Auto();
            });
        }

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
        fxSkill.transform.localPosition = new Vector3(posHandReadyFX.transform.position.x, 3.5f, posHandReadyFX.transform.position.z);
    }

    bool isFxSkill = false; 
    public void ActionSkill(Action onComplete)
    {
        if (UICharacter._instance._uiCountDownSkillJett != null)
            UICharacter._instance._uiCountDownSkillJett.gameObject.SetActive(false);

        CmdSetAnim();
        if (_charInstance != null)
            _charInstance.SwitchItemWhenUserSkill();
        isFxSkill = true;
        CmdAnimStart();

        CmdSpawnObj(3);
        CmdSpawnObj(4);

        _charInstance.PrepareCharacterToLerp();
        Vector3 target = _charInstance.transform.forward * 1000;
        _controller.SimpleMove(target);
        _charInstance.SetCurrentPositionTargetToLerp(_charInstance.transform.position);
     
        onComplete?.Invoke();
    }

    [Command]
    private void CmdSetAnim()
    {
        RpcSetAnim();
    }

    [ClientRpc]
    private void RpcSetAnim()
    {
        CharAni._animator.SetBool(IsSkilling, _charInstance.IsSkilling);
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
        CharAni._animator.SetTrigger(IsAction);
    }

    [Command]
    private void CmdSpawnObj(int index)
    {
        if (isServer)
        {
            var nGo = (index) switch
            {
                0 => fx_jett_e_buf_L,
                1 => fx_jett_e_buf_R,
                2 => fx_jett_e_ready,
                3 => fx_jett_e_ready_speed,
                _ => fx_jett_e_trail_speed,
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
        if (_charInstance == null)
        {
            StopFx();
            return;
        }
        switch (index)
        {
            case 0:
                _fx_jett_e_buff_L = networkIdentity.GetComponent<FxSkillController>();
                _fx_jett_e_buff_L.transform.SetParent(posLootLeftFX.transform);
                _fx_jett_e_buff_L.transform.localPosition = Vector3.zero;
                break;
            case 1:
                _fx_jett_e_buff_R = networkIdentity.GetComponent<FxSkillController>();
                _fx_jett_e_buff_R.transform.SetParent(posLootRightFX.transform);
                _fx_jett_e_buff_R.transform.localPosition = Vector3.zero;
                break;
            case 2:
                _fx_jett_e_ready = networkIdentity.GetComponent<FxSkillController>();
                _fx_jett_e_ready.transform.SetParent(posHandReadyFX.transform);
                _fx_jett_e_ready.transform.localPosition = Vector3.zero;
                SetCharacterAni(CurrentStepSkill);
                break;
            case 3:
                _fx_jett_e_ready_speed = networkIdentity.GetComponent<FxSkillController>();
                _fx_jett_e_ready_speed.transform.SetParent(posHandReadyFX.transform);
                _fx_jett_e_ready_speed.transform.localPosition = Vector3.zero;
                break;
            default:
                _fx_jett_e_trail_speed = networkIdentity.GetComponent<FxSkillController>();
                _fx_jett_e_trail_speed.transform.SetParent(posHandReadyFX.transform);
                _fx_jett_e_trail_speed.transform.localPosition = Vector3.zero;
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
                _fx_jett_e_buff_L = null;
                break;
            case 1:
                _fx_jett_e_buff_R = null;
                break;
            case 2:
                _fx_jett_e_ready = null;
                break;
            case 3:
                _fx_jett_e_ready_speed = null;
                break;
            case 4:
                _fx_jett_e_trail_speed = null;
                break;
        }
    }

    public async void ActionEnd(Action onComplete = null)
    {
        isFxSkill = false;
        DesTroyFx(0);

        await Task.Delay(500);
        onComplete?.Invoke();
        CmdSetAnim();
        RequestReloadItem();
    }    

    public void StopSkill(Action onComplete = null)
    {
        isFxSkill = false;
        DesTroyFx(0);

        CmdSetAnim();
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
        if (_charInstance == null) return;
        DesTroyFx(0);
    }

    public async void DesTroyFx(int timeDelay)
    {
        if (UICharacter._instance._uiCountDownSkillJett != null)
            UICharacter._instance._uiCountDownSkillJett.gameObject.SetActive(false);

        posHandReadyFX.gameObject.SetActive(false);
        posHandLeftFX.gameObject.SetActive(false);
        posHandRightFX.gameObject.SetActive(false);
        if (_fx_jett_e_buff_L != null)
        {
            CmdUnspawnObj(_fx_jett_e_buff_L.gameObject, 0);
        }
        if (_fx_jett_e_buff_R != null)
        {
            CmdUnspawnObj(_fx_jett_e_buff_R.gameObject, 1);
        }
        await Task.Delay(timeDelay);
        if (_fx_jett_e_ready != null)
        {
            CmdUnspawnObj(_fx_jett_e_ready.gameObject, 2);
        }
        if (_fx_jett_e_ready_speed != null)
        {
            CmdUnspawnObj(_fx_jett_e_ready_speed.gameObject, 3);
        }
        if (_fx_jett_e_trail_speed != null)
        {
            CmdUnspawnObj(_fx_jett_e_trail_speed.gameObject, 4);
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

    public void PlayFX_Sound_Cast()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.TailWindSkill, 0);
        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.TailWindSkill, true);
        if (fxSkill == null) return;
        bool isLocal = fxSkill[0].IsLocal;
        if (!isLocal)
            Request_PlayFX_Sound_Cast();
    }

    public void PlayFX_Sound_Active()
    {
        SoundManager.Instance.StopPlayOneShot_Battle_HeroSkill();
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.TailWindSkill, 1);
        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.TailWindSkill, true);
        if (fxSkill == null) return;
        bool isLocal = fxSkill[1].IsLocal;
        if (!isLocal)
            Request_PlayFX_Sound_Active();
    }

    [Command]
    private void Request_PlayFX_Sound_Cast()
    {
        Response_PlayFX_Sound_Cast();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_Cast()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.TailWindSkill, 0);
    }

    [Command]
    private void Request_PlayFX_Sound_Active()
    {
        Response_PlayFX_Sound_Active();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_Active()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.TailWindSkill, 1);
    }
}
