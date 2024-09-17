using Mirror;
using MultiFPS;
using MultiFPS.Gameplay;
using MultiFPS.Sound;
using MultiFPS.UI.HUD;
using System;
using System.Threading.Tasks;
using UnityEngine;
using static MultiFPS.Gameplay.CharacterInstance;

public class FromTheShadowsSkill : NetworkBehaviour
{
    public CharacterInstance _charInstance;
    public CharacterMotor CharMotor;
    public CharacterAnimator CharAni;
    protected Client_DataSkills.Param CurrentSkill;

    public CharacterController _controller;

    [SerializeField]
    private Transform posHandFX;
    [SerializeField]
    private Transform posActionFX;
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
    public GameObject fx_omen_bullet;
    #endregion

    public string IsStart = "IsStart_X";
    public string IsSkilling = "IsSkilling";
    public string IsLoop = "IsLoop_X";
    public string IsEnd = "IsEnd_X";

    private void Start()
    {
    }

    private Vector3 targetPosition;
    public StepSkill CurrentStepSkill;
    public void StartSkill(Vector3 targetPosition)
    {
        CurrentStepSkill = StepSkill.Start;
        SetCharacterAni(CurrentStepSkill);

        UICharacter._instance.SetActiveMinimapSkill(_charInstance.IsSkilling);
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
        CharAni._animator.SetBool(IsSkilling, _charInstance.IsSkilling);
        CharAni._animator.SetTrigger(IsStart);
    }

    private FxSkillController _fx_omen_01;
    private FxSkillController _fx_omen_02;
    public void OnEventFromTheShadowsSkillFX_Start_01()
    {
    
    }

    public void Action_EquipState()
    {
        SetCharacterAni(CurrentStepSkill);
    }

    public void Action_CastState()
    {
        CmdSpawnObj(0);
        SetCharacterAni(CurrentStepSkill);
    }

    public void Action_ActivationState()
    {
     
    }

    public void OnEventFromTheShadowsSkillFX_Loop()
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
        CharAni._animator.SetTrigger(IsLoop);
    }

    private void Action_SetPositionFx(FxSkillController fxSkill)
    {
        CmdAction_SetPositionFx(fxSkill.gameObject);
    }

    [Command]
    private void CmdAction_SetPositionFx(GameObject fxSkill)
    {
        RpcAction_SetPositionFx(fxSkill);
    }

    [ClientRpc]
    private void RpcAction_SetPositionFx(GameObject fxSkill)
    {
        fxSkill.GetComponent<FxSkillController>().StartFx();
        isFxSkill = true;
        fxSkill.transform.localPosition = new Vector3(posHandFX.transform.position.x, posHandFX.transform.position.y, posHandFX.transform.position.z);
    }

    bool isFxSkill = false;
    float hitDistance = 0;

    private FxSkillController _fx_omen_bullet;
    public async void ActionSkill(Action onComplete)
    {
        PlayFX_Sound_Voice_Line();
        UICharacter._instance.SetActiveMinimapSkill(false);
        if (_fx_omen_01 != null)
        {
            CmdUnspawnObj(_fx_omen_01.gameObject, 0);
        }

        CmdAnimEnd();
        await Task.Delay(500);

        CmdSpawnObj(1);
        CmdSpawnObj(2);

        await Task.Delay(750);
        if (_fx_omen_bullet != null)
        {
            CmdUnspawnObj(_fx_omen_bullet.gameObject, 2);
        }
        if (_fx_omen_01 != null)
        {
            CmdUnspawnObj(_fx_omen_01.gameObject, 0);
        }
        if (_fx_omen_02 != null)
        {
            CmdUnspawnObj(_fx_omen_02.gameObject, 1);
        }
        _charInstance.PrepareCharacterToLerp();
        
        Vector3 target = _charInstance.transform.forward * UnityEngine.Random.Range(20, 30);
        _controller.Move(target);
        _charInstance.transform.position = target;
        _charInstance.SetCurrentPositionTargetToLerp(_charInstance.transform.position);


        onComplete?.Invoke();
    }

    [Command]
    private void CmdAnimEnd()
    {
        RpcAnimEnd();
    }

    [ClientRpc]
    private void RpcAnimEnd()
    {
        CharAni._animator.SetTrigger(IsEnd);
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
                _ => fx_omen_bullet
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
                _fx_omen_01.transform.SetParent(posHandFX.transform);
                GameManager.SetGameLayerRecursive(_fx_omen_01.gameObject, _charInstance.isOwned ? 13 : 0);
                //Action_SetPositionFx(_fx_omen_01);
                _fx_omen_01.transform.localPosition = Vector3.zero;
                break;
            case 1:
                _fx_omen_02 = networkIdentity.GetComponent<FxSkillController>();
                Action_SetPositionFx(_fx_omen_02);
                SetFxPos();
                break;
            default:
                _fx_omen_bullet = networkIdentity.GetComponent<FxSkillController>();
                Action_SetPositionFx(_fx_omen_bullet);
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
                _fx_omen_01 = null;
                break;
            case 1:
                _fx_omen_02 = null;
                break;
            case 2:
                _fx_omen_bullet = null;
                break;
            case 3:

                break;
        }
    }

    private void SetFxPos()
    {
        hitDistance = 10f;
        Vector3 targetOffset = _charInstance.transform.forward * hitDistance;
        targetPosition = _charInstance.FPPLook.transform.position + targetOffset;
        Vector3 targetOfft = _charInstance.transform.forward * 1;
        Vector3 targetPos = _charInstance.FPPLook.transform.position + targetOfft;
        if(_fx_omen_bullet)
            _fx_omen_bullet.transform.localPosition = new Vector3(targetPos.x, posActionFX.transform.position.y - 0.5f, targetPos.z);
        targetPosition = _charInstance.FPPLook.transform.position + targetOffset;
        if(_fx_omen_02)
            _fx_omen_02.transform.localPosition = new Vector3(targetPosition.x, 3, targetPosition.z);

        CmdUpdatePos();
    }

    [Command]
    private void CmdUpdatePos()
    {
        RpcUpdatePos();
    }

    [ClientRpc]
    private void RpcUpdatePos()
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        RaycastHit hit;
        Vector3 tar_y = new Vector3(_fx_omen_02.transform.position.x, targetPosition.y, _fx_omen_02.transform.position.z);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(tar_y, _fx_omen_02.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
        {
            if (hit.distance < 2)
            {
                if (_fx_omen_02)
                {
                    if (hit.transform.position.y > 1.1f)
                    {
                        _fx_omen_02.transform.localPosition = new Vector3(targetPosition.x, 3, targetPosition.z);
                    }
                    else
                    {
                        _fx_omen_02.transform.localPosition = new Vector3(targetPosition.x, 1.0f, targetPosition.z);
                    }
                }
            }
            else if (hit.distance >= 2 && hit.distance < 3)
            {
                if (_fx_omen_02)
                    _fx_omen_02.transform.localPosition = new Vector3(targetPosition.x, 2.0f, targetPosition.z);
            }
            else if (hit.distance >= 3)
            {
                if (_fx_omen_02)
                    _fx_omen_02.transform.localPosition = new Vector3(targetPosition.x, 1.0f, targetPosition.z);
            }
        }
    }

    public async void ActionEnd(Action onComplete = null)
    {
        isFxSkill = false;
        DesTroyFx();

        await Task.Delay(500);
        onComplete?.Invoke();
        RequestReloadItem();
        CmdResetAnim();
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
        DesTroyFx();
        CmdResetAnim();
        RequestReloadItem();
        onComplete?.Invoke();
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
        DesTroyFx();
    }

    public void DesTroyFx()
    {
        if (_fx_omen_bullet != null)
        {
            CmdUnspawnObj(_fx_omen_bullet.gameObject, 2);
        }

        if (_fx_omen_01 != null)
        {
            CmdUnspawnObj(_fx_omen_01.gameObject, 0);
        }
        if (_fx_omen_02 != null)
        {
            CmdUnspawnObj(_fx_omen_02.gameObject, 1);
        }
        UICharacter._instance.SetActiveMinimapSkill(_charInstance.IsSkilling);
    }

    public void PlayFX_Sound_Start()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.FromTheShadowsSkill, 0);
        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.FromTheShadowsSkill, true);
        if (fxSkill == null) return;
        bool isLocal = fxSkill[0].IsLocal;
        if (!isLocal)
            Request_PlayFX_Sound_Start();
    }

    public void PlayFX_Sound_Active()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.FromTheShadowsSkill, 1);
        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.FromTheShadowsSkill, true);
        if (fxSkill == null) return;
        bool isLocal = fxSkill[1].IsLocal;
        if (!isLocal)
            Request_PlayFX_Sound_Active();
    }

    public void PlayFX_Sound_Voice_Line()
    {
        SoundManager.Instance.Play_Battle_HeroSkill(CharacterInstance.Skill.FromTheShadowsSkill, 0, false, 1, 1000);
        DataAudioClip[] fxSkill = SoundManager.Instance.GetAudioClipBySkill(CharacterInstance.Skill.FromTheShadowsSkill, false);
        if (fxSkill == null) return;
        bool isLocal = fxSkill[0].IsLocal;
        if (!isLocal)
            Request_PlayFX_Sound_Voice_Line();
    }

    [Command]
    private void Request_PlayFX_Sound_Start()
    {
        Response_PlayFX_Sound_Start();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_Start()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.FromTheShadowsSkill, 0);
    }

    [Command]
    private void Request_PlayFX_Sound_Active()
    {
        Response_PlayFX_Sound_Active();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_Active()
    {
        SoundManager.Instance.PlayOneShot_Battle_HeroSkill(CharacterInstance.Skill.FromTheShadowsSkill, 1);
    }

    [Command]
    private void Request_PlayFX_Sound_Voice_Line()
    {
        Response_PlayFX_Sound_Voice_Line();
    }

    [ClientRpc]
    private void Response_PlayFX_Sound_Voice_Line()
    {
        SoundManager.Instance.Play_Battle_HeroSkill(CharacterInstance.Skill.FromTheShadowsSkill, 0, false, 1, 1000);
    }
}
