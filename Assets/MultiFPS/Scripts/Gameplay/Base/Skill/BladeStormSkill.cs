using DG.Tweening;
using Mirror;
using MultiFPS;
using MultiFPS.Gameplay;
using MultiFPS.Sound;
using System;
using System.Threading.Tasks;
using UnityEngine;
using static MultiFPS.Gameplay.CharacterInstance;

public class BladeStormSkill : NetworkBehaviour
{
    public CharacterInstance _charInstance;
    public CharacterMotor CharMotor;
    public CharacterAnimator CharAni;
    protected Client_DataSkills.Param CurrentSkill;

    public CharacterController _controller;

    public Transform[] Pos_fly_start;
    public Transform[] Pos_fly_start_server;
    public Transform[] Pos_BattleFx_Hit;
    public Transform[] Pos_AuraFx;
    public GameObject[] Go_AttachFx;

    [SerializeField]
    private GameObject knife_3rd;

    [SerializeField]
    private GameObject[] objKnife;

    [SerializeField]
    private GameObject[] objKnife_server;

    [SerializeField]
    private Transform posHandLeftFX;

    [SerializeField]
    private Transform posHandRightFX;

    [SerializeField]
    private Transform posHandCenterFX;
    public enum StepSkill
    {
        Start = 0,
        EquipState = 1,
        CastState = 2,
        ActivationState = 3,
        End = 4,

    }

    #region fx skill
    public GameObject fx_jett_x_bullet;
    public GameObject fx_jett_x_hit;
    public GameObject fx_jett_x_ready_01;
    public GameObject fx_jett_x_ready_02;
    public GameObject fx_jett_x_ready_fly_01;
    public GameObject fx_jett_x_ready_fly_02;
    public GameObject fx_jett_x_trail_geo04_01;
    public GameObject fx_jett_x_trail_geo04_02;
    public GameObject fx_jett_x_trail_geo04_03;
    public GameObject fx_jett_x_trail_geo04_04;
    public GameObject hitEffect;
    private FxSkillController _fx_jett_x_bullet;
    private FxSkillController _fx_jett_x_hit;
    private FxSkillController _fx_jett_x_ready_01;
    private FxSkillController _fx_jett_x_ready_02;
    private FxSkillController[] _fx_jett_x_ready_fly;
    private FxSkillController _fx_jett_x_ready_fly_02;
    private FxSkillController _fx_jett_x_trail_geo04_01;
    private FxSkillController _fx_jett_x_trail_geo04_02;
    private FxSkillController _fx_jett_x_trail_geo04_03;
    private FxSkillController _fx_jett_x_trail_geo04_04;
    #endregion

    [SerializeField]
    private GameObject fx_jett_knife_prefab;

    public string Jett_Ability_x = "Jett_Ability_x";
    public string IsSkilling = "IsSkilling";
    public string Jett_Ability_x_attack01 = "Jett_Ability_x_attack01";
    public string Jett_Ability_x_attack02 = "Jett_Ability_x_attack02";
    public string Jett_Ability_x_attack03 = "Jett_Ability_x_attack03";
    public string Jett_Ability_x_attack04 = "Jett_Ability_x_attack04";
    public string Jett_Ability_x_attack05 = "Jett_Ability_x_attack05";
    public string Jett_Ability_x_attack_Special = "Jett_Ability_x_attack_Special";
    public string Jett_Ability_x_loop = "Jett_Ability_x_loop";
    public string Jett_Ability_x_start = "Jett_Ability_x_start";

    private Vector3 targetPosition;
    private Vector3 _targetPosition;
    public StepSkill CurrentStepSkill;

    private void Start()
    {
    }

    public void StartSkill(Vector3 targetPosition)
    {
        CmdIsActiveKnife(true);
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

    public async void Action_Start()
    {
        await Task.Delay(500);
        if (_charInstance != null)
            _charInstance.SwitchItemWhenUserSkill();
        CmdSetAnim();
        CmdAnimStart();
        SetCharacterAni(CurrentStepSkill);
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
        CharAni._animator.SetTrigger(Jett_Ability_x_start);
    }

    public void Action_EquipState()
    {
        CmdSpawnObj(0);
        CmdSpawnObj(1);
        CmdSpawnObj(2);

        SetCharacterAni(CurrentStepSkill);
    }

    public void OnEventBladeStormEndAnimtionStart()
    {
        CmdAnimLoop();
    }

    [Command]
    private void CmdAnimLoop()
    {
        RpcAnimLoop();
    }

    [ClientRpc]
    private void RpcAnimLoop()
    {
        CharAni._animator.SetTrigger(Jett_Ability_x_loop);
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
        fxSkill.transform.localPosition = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
    }

    bool isFxSkill = false;
    float hitDistance = 0;
    private void FixedUpdate()
    {
        if (isFxSkill)
        {
        }
    }

    private void SetFxPos(Vector3 targetOffset)
    {
        targetPosition = _targetPosition + targetOffset;
    }

    Action _onCompleteActionSkill;
    FxSkillController[] _fx_jett_x_bullets = new FxSkillController[5];


    FxSkillController[] fx_jett_knife_readys = new FxSkillController[5];
    public void ActionSkill(Action onComplete)
    {
        CmdAnimAttack();
        _onCompleteActionSkill = onComplete;

        if (NetworkServer.active)
        {
            RpcCreateObj();
            RpcActionMoveObj();
        }
        else
            RpcActionMoveObj();
    }

    [Command]
    private void CmdAnimAttack()
    {
        RpcAnimAttack();
    }

    [ClientRpc]
    private void RpcAnimAttack()
    {
        CharAni._animator.SetTrigger(Jett_Ability_x_attack_Special);
    }

    //[ClientRpc]
    private void RpcActionMoveObj()
    {
        PlayFX_Sound_Active_End();
        if (_charInstance == null && Pos_fly_start.Length == 0) return;

        Vector3 targetOffset = _charInstance.FPPLook.transform.forward * 15;
        Vector3 targetPosition = _targetPosition + targetOffset;
        for (int i = 0; i < 5; i++)
        {
            if (Pos_fly_start[i] != null)
            {
                GameObject fx_ready_0 = Instantiate(fx_jett_x_ready_01, transform.position, transform.rotation);
                fx_jett_knife_readys[i] = fx_ready_0.GetComponent<FxSkillController>();
                fx_jett_knife_readys[i].transform.SetParent(_charInstance.isOwned ? Pos_fly_start[i].transform : Pos_fly_start_server[i].transform);
                fx_jett_knife_readys[i].transform.localPosition = Vector3.zero;
                fx_jett_knife_readys[i].transform.localScale = Vector3.one;

                GameObject fx_bullet_0 = Instantiate(fx_jett_x_bullet, transform.position, transform.rotation);
                _fx_jett_x_bullets[i] = fx_bullet_0.GetComponent<FxSkillController>();
                _fx_jett_x_bullets[i].transform.localPosition = _charInstance.isOwned ? Pos_fly_start[i].transform.position : Pos_fly_start_server[i].transform.position;
                _fx_jett_x_bullets[i].transform.localScale = new Vector3(20, 20, 20);
                //_fx_jett_x_bullets[i].GetComponent<SkillDamage>().setHitEffect(_charInstance, hitEffect, 10);
                _fx_jett_x_bullets[i].transform.DOMove(targetPosition, 0.7f).OnComplete(() => {
                    if (_fx_jett_x_bullets[i] != null)
                    {
                        _fx_jett_x_bullets[i].gameObject.SetActive(false);
                        Destroy(_fx_jett_x_bullets[i].gameObject);
                    }
                });
            }
        }

        if (!_charInstance.isOwned)
            knife_3rd.gameObject.SetActive(false);
    }

    [ClientRpc]
    private void RpcCreateObj()
    {
    }

    public void OnEventBladeStormEndAnimtionSpecial()
    {
        _onCompleteActionSkill?.Invoke();
    }

    [Command]
    private void CmdSpawnObj(int index)
    {
        if (isServer)
        {
            var nGo = (index) switch
            {
                0 => fx_jett_x_bullet,
                1 => fx_jett_x_hit,
                2 => fx_jett_x_ready_01,
                3 => fx_jett_x_ready_02,
                4 => fx_jett_x_ready_fly_01,
                5 => fx_jett_x_ready_fly_02,
                6 => fx_jett_x_trail_geo04_01,
                7 => fx_jett_x_trail_geo04_02,
                8 => fx_jett_x_trail_geo04_03,
                _ => fx_jett_x_trail_geo04_04,
            };
            var go = Instantiate(nGo, transform.position, transform.rotation);
            /*NetworkServer.Spawn(go);
            go.GetComponent<NetworkIdentity>().AssignClientAuthority(netIdentity.connectionToClient);*/
            RpcSpawnObj(go, index);
        }
    }

    [ClientRpc]
    private void RpcSpawnObj(GameObject networkIdentity, int index)
    {
        switch (index)
        {
            case 0:
                var go = Instantiate(fx_jett_x_bullet, transform.position, transform.rotation);
                _fx_jett_x_bullet = go.GetComponent<FxSkillController>();
                Action_SetPositionFx(_fx_jett_x_bullet);
                break;
            case 1:
                go = Instantiate(fx_jett_x_hit, transform.position, transform.rotation);
                _fx_jett_x_hit = go.GetComponent<FxSkillController>();
                Action_SetPositionFx(_fx_jett_x_hit);
                break;
            case 2:
                go = Instantiate(fx_jett_x_ready_01, transform.position, transform.rotation);
                _fx_jett_x_ready_01 = go.GetComponent<FxSkillController>();
                Action_SetPositionFx(_fx_jett_x_ready_01);
                break;
            case 3:
                _fx_jett_x_ready_02 = networkIdentity.GetComponent<FxSkillController>();
                break;
            case 4:
                _fx_jett_x_ready_fly[0] = networkIdentity.GetComponent<FxSkillController>();
                break;
            case 5:
                _fx_jett_x_ready_fly_02 = networkIdentity.GetComponent<FxSkillController>();
                break;
            case 6:
                _fx_jett_x_trail_geo04_01 = networkIdentity.GetComponent<FxSkillController>();
                break;
            case 7:
                _fx_jett_x_trail_geo04_02 = networkIdentity.GetComponent<FxSkillController>();
                break;
            case 8:
                _fx_jett_x_trail_geo04_03 = networkIdentity.GetComponent<FxSkillController>();
                break;
            default:
                _fx_jett_x_trail_geo04_04 = networkIdentity.GetComponent<FxSkillController>();
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
                _fx_jett_x_bullet = null;
                break;
            case 1:
                _fx_jett_x_hit = null;
                break;
            case 2:
                _fx_jett_x_ready_01 = null;
                break;
        }
    }

    public async void ActionEnd(Action onComplete = null)
    {
        isFxSkill = false;
        StopFx(false);

        CmdSetAnimEnd();

        RequestReloadItem();
        await Task.Delay(500);
        onComplete?.Invoke();
    }

    public void StopSkill(Action onComplete = null)
    {
        isFxSkill = false;
        StopFx(false);

        CmdSetAnimEnd();
        onComplete?.Invoke();
    
        RequestReloadItem();

    }

    [Command]
    private void CmdSetAnimEnd()
    {
        RpcSetAnimEnd();
    }

    [ClientRpc]
    private void RpcSetAnimEnd()
    {
        CharAni._animator.SetBool(IsSkilling, false);
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

    public void StopFx(bool isfirst = true)
    {
        if (isfirst)
        {
            if(NetworkServer.active)
                RpcIsActiveKnife(false);
        }
        else
            CmdIsActiveKnife(false);
       
        if (NetworkServer.active)
            RpcDesTroyFx();
    }

    [ClientRpc]
    public void RpcDesTroyFx()
    {
        if (_fx_jett_x_bullet != null)
        {
            CmdUnspawnObj(_fx_jett_x_bullet.gameObject, 0);
        }
        if (_fx_jett_x_hit != null)
        {
            CmdUnspawnObj(_fx_jett_x_hit.gameObject, 1);
        }
        if (_fx_jett_x_ready_01 != null)
        {
            CmdUnspawnObj(_fx_jett_x_ready_01.gameObject, 2);
        }
        if (_fx_jett_x_ready_02 != null)
        {
            
        }
        if(_fx_jett_x_ready_fly != null)
        {
            for (int i = 0; i < _fx_jett_x_ready_fly.Length; i++)
            {
                if (_fx_jett_x_ready_fly[i] != null)
                {
                    _fx_jett_x_ready_fly[i].gameObject.SetActive(false);
                    Destroy(_fx_jett_x_ready_fly[i].gameObject);
                }
            }
        }
  
        if (_fx_jett_x_ready_fly_02 != null)
        {
            _fx_jett_x_ready_fly_02.gameObject.SetActive(false);
            Destroy(_fx_jett_x_ready_fly_02.gameObject);
        }
        if (_fx_jett_x_trail_geo04_01 != null)
        {
            _fx_jett_x_trail_geo04_01.gameObject.SetActive(false);
            Destroy(_fx_jett_x_trail_geo04_01.gameObject);
        }
        if (_fx_jett_x_trail_geo04_02 != null)
        {
            _fx_jett_x_trail_geo04_02.gameObject.SetActive(false);
            Destroy(_fx_jett_x_trail_geo04_02.gameObject);
        }
        if (_fx_jett_x_trail_geo04_03 != null)
        {
            _fx_jett_x_trail_geo04_03.gameObject.SetActive(false);
            Destroy(_fx_jett_x_trail_geo04_03.gameObject);
        }
        if (_fx_jett_x_trail_geo04_04 != null)
        {
            _fx_jett_x_trail_geo04_04.gameObject.SetActive(false);
            Destroy(_fx_jett_x_trail_geo04_04.gameObject);
        }
    }

    public void Start_Skill_Server(Vector3 targetPosition)
    {

    }

    [Command]
    private void CmdIsActiveKnife(bool isActive)
    {
        RpcIsActiveKnife(isActive);
    }

    [ClientRpc]
    private void RpcIsActiveKnife(bool isActive)
    {
        for (int i = 0; i < objKnife.Length; i++)
        {
            objKnife[i].SetActive(isActive);
        }
    }

    public void PlayFX_Sound_Start()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.BladeStormSkill, 0);

        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.BladeStormSkill, true);
        bool isLocal = fxSkill[0].IsLocal;
        if (!isLocal)
            Request_PlayFX_Sound_Start();
    }

    public void PlayFX_Sound_Active()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.BladeStormSkill, 1);

        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.BladeStormSkill, true);
        bool isLocal = fxSkill[1].IsLocal;
        if (!isLocal)
            Request_PlayFX_Sound_Active();
    }

    public void PlayFX_Sound_Active_End()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.BladeStormSkill, 0);

        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.BladeStormSkill, true);
        bool isLocal = fxSkill[2].IsLocal;
        if (!isLocal)
            Request_PlayFX_Sound_Active_End();
    }

    [Command]
    private void Request_PlayFX_Sound_Start()
    {
        Response_PlayFX_Sound_Start();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_Start()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.BladeStormSkill, 0);
    }

    [Command]
    private void Request_PlayFX_Sound_Active()
    {
        Response_PlayFX_Sound_Active();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_Active()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.BladeStormSkill, 1);
    }

    [Command]
    private void Request_PlayFX_Sound_Active_End()
    {
        Response_PlayFX_Sound_Active_End();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_Active_End()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.BladeStormSkill, 2);
    }
}
