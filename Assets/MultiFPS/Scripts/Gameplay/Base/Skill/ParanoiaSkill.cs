using DG.Tweening;
using Mirror;
using MultiFPS;
using MultiFPS.Gameplay;
using MultiFPS.Sound;
using System;
using System.Threading.Tasks;
using UnityEngine;
using static MultiFPS.Gameplay.CharacterInstance;

public class ParanoiaSkill : NetworkBehaviour
{
    public CharacterInstance _charInstance;
    public CharacterMotor CharMotor;
    public CharacterAnimator CharAni;
    protected Client_DataSkills.Param CurrentSkill;

    public CharacterController _controller;

    [SerializeField]
    private Transform posHandLeftFX_01_01;

    [SerializeField]
    private Transform posHandLeftFX_01_02;

    [SerializeField]
    private Transform posHandLeftFX_02_01;

    [SerializeField]
    private Transform posHandLeftFX_02_02;
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
    public GameObject fx_omen_bullet;
    #endregion

    public string IsStart = "IsStart_Q";
    public string IsSkilling = "IsSkilling";
    public string IsLoop = "IsLoop_Q";
    public string IsEnd = "IsEnd_Q";

    private void Start()
    {
    }

    private Vector3 targetPosition;
    private Vector3 _targetPosition;
    public StepSkill CurrentStepSkill;
    public void StartSkill(Vector3 targetPosition)
    {
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

    private FxSkillController _fx_omen_01;
    private FxSkillController _fx_omen_02;
    private FxSkillController _fx_omen_03;
    private FxSkillController _fx_omen_04;
    public void OnEventParanoiSkillFX_Start_01()
    {
        CmdSpawnObj(0);
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
                3 => fx_omen_04,
                _ => fx_omen_bullet,
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
                _fx_omen_01.transform.localPosition = new Vector3(posHandLeftFX_01_01.transform.position.x, posHandLeftFX_01_01.transform.position.y, posHandLeftFX_01_01.transform.position.z);
                _fx_omen_01.StartFx();
                break;
            case 1:
                _fx_omen_02 = networkIdentity.GetComponent<FxSkillController>();
                _fx_omen_02.transform.localPosition = new Vector3(posHandLeftFX_01_02.transform.position.x, posHandLeftFX_01_02.transform.position.y, posHandLeftFX_01_02.transform.position.z);
                _fx_omen_02.StartFx();
                break;
            case 2:
                _fx_omen_03 = networkIdentity.GetComponent<FxSkillController>();
                _fx_omen_03.transform.SetParent(posHandLeftFX_02_02.transform);
                _fx_omen_03.transform.localPosition = Vector3.zero;
                _fx_omen_03.StartFx();
                break;
            case 3:
                _fx_omen_04 = networkIdentity.GetComponent<FxSkillController>();
                _fx_omen_04.transform.SetParent(posHandLeftFX_02_02.transform);
                _fx_omen_04.transform.localPosition = Vector3.zero;
                _fx_omen_04.StartFx();
                break;
            default:
                _fx_omen_bullet = networkIdentity.GetComponent<FxSkillController>();
                _fx_omen_bullet.StartFx();
                _fx_omen_bullet.transform.localPosition = new Vector3(posHandLeftFX_02_02.transform.position.x, posHandLeftFX_02_02.transform.position.y, posHandLeftFX_02_02.transform.position.z);
                CmdDomoveBullet();
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

    public void Action_EquipState()
    {
        SetCharacterAni(CurrentStepSkill);
    }

    public void OnEventParanoiSkillFX_Start_02()
    {
        CmdSpawnObj(2);
        CmdSpawnObj(3);
        if (_fx_omen_01 != null)
            CmdUnspawnObj(_fx_omen_01.gameObject, 0);
        if (_fx_omen_02 != null)
            CmdUnspawnObj(_fx_omen_02.gameObject, 1);
        isFxSkill = true;
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
                _fx_omen_01 = null;
                break;
            case 1:
                _fx_omen_02 = null;
                break;
            case 2:
                _fx_omen_03 = null;
                break;
            case 3:
                _fx_omen_04 = null;
                break;
            case 4:
                _fx_omen_bullet = null;
                break;
        }
    }

    public void OnEventParanoiSkillLoop()
    {
        CmdSetAnimLoop();
    }

    [Command]
    private void CmdSetAnimLoop()
    {
        RpcSetAnimLoop();
    }

    [ClientRpc]
    private void RpcSetAnimLoop()
    {
        CharAni._animator.SetTrigger(IsLoop);
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
        fxSkill.transform.localPosition = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
    }

    bool isFxSkill = false;
    float hitDistance = 0;
    private FxSkillController _fx_omen_bullet;
    private Action _onCompleteAnmationEnd = null;
    public void ActionSkill(Action onComplete)
    {
        if (_onCompleteAnmationEnd != null) return;
        isFxSkill = false;
        CmdSetAnimEnd();
        _onCompleteAnmationEnd = onComplete;
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


    public void OnEventParanoiaFxEnd()
    {
        CmdSpawnObj(4);
        
        if (_fx_omen_03 != null)
        {
            CmdUnspawnObj(_fx_omen_03.gameObject, 2);
        }

        if (_fx_omen_04 != null)
        {
            CmdUnspawnObj(_fx_omen_04.gameObject, 3);
        }
    }

    [Command]
    private void CmdDomoveBullet()
    {
        RpcDomoveBullet();
    }

    [ClientRpc]
    private void RpcDomoveBullet()
    {
        Vector3 targetOffset = _charInstance.transform.forward * 30;
        Vector3 targetPosition = _targetPosition + targetOffset;
        Sequence s = DOTween.Sequence();
        s.Append(_fx_omen_bullet.transform.DOMove(targetPosition, 2.0f));
        s.OnComplete(() =>
        {
            if (_fx_omen_bullet != null)
            {
                CmdUnspawnObj(_fx_omen_bullet.gameObject, 4);
            }

            _onCompleteAnmationEnd?.Invoke();
            _onCompleteAnmationEnd = null;
        });
    }

    public async void ActionEnd(Action onComplete = null)
    {
        isFxSkill = false;
        CmdDesTroyFx();

        await Task.Delay(500);
        onComplete?.Invoke();
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
        CmdDesTroyFx();
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
        CmdDesTroyFx();
    }

    public void CmdDesTroyFx()
    {
        if (_fx_omen_bullet != null)
        {
            CmdUnspawnObj(_fx_omen_bullet.gameObject, 4);
        }
        if (_fx_omen_01 != null)
        {
            CmdUnspawnObj(_fx_omen_01.gameObject,0);
        }
        if (_fx_omen_02 != null)
        {
            CmdUnspawnObj(_fx_omen_02.gameObject, 1);
        }
        if (_fx_omen_03 != null)
        {
            CmdUnspawnObj(_fx_omen_03.gameObject, 2);
        }
        if (_fx_omen_04 != null)
        {
            CmdUnspawnObj(_fx_omen_04.gameObject, 3);
        }
    }

    public void PlayFX_Sound_Cast()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.ParanoiaSkill, 0);

        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.ParanoiaSkill, true);
        if (fxSkill == null) return;
        bool isLocal = fxSkill[0].IsLocal;
        if (!isLocal)
            Request_PlayFX_Sound_Cast();
    }

    public void PlayFX_Sound_Active()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.ParanoiaSkill, 1);

        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.ParanoiaSkill, true);
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
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.ParanoiaSkill, 0);
    }

    [Command]
    private void Request_PlayFX_Sound_Active()
    {
        Response_PlayFX_Sound_Active();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_Active()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.ParanoiaSkill, 1);
    }
}
