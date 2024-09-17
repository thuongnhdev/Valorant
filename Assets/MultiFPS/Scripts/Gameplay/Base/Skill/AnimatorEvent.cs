using MultiFPS.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEvent : MonoBehaviour
{
    [SerializeField]
    private CharacterInstance characterInstance;

    public void OnCompleteAnimationStart()
    {
        characterInstance.OnCompleteAnimationStart();
    }

    public void OnCompleteAnimationEnd()
    {
        characterInstance.OnCompleteAnimationEnd();
    }

    public void OnEventShroudedStepEnd()
    {
        characterInstance.OnEventShroudedStepEnd();
    }

    public void OnEventActionSkillCloudBurst()
    {
        characterInstance.OnEventActionSkillCloudBurst();
    }

    public void OnEventParanoiSkillFX_Start_01()
    {
        characterInstance.OnEventParanoiSkillFX_Start_01();
    }

    public void OnEventParanoiSkillFX_Start_02()
    {
        characterInstance.OnEventParanoiSkillFX_Start_02();
    }

    public void OnEventParanoiSkillLoop()
    {
        characterInstance.OnEventParanoiSkillLoop();
    }

    public void OnEventParanoiaFxEnd()
    {
        characterInstance.OnEventParanoiaFxEnd();
    }

    public void OnEventDarkCoverSkillFX_Start_01()
    {
        characterInstance.OnEventDarkCoverSkillFX_Start_01();
    }

    public void OnEventFromTheShadowsSkillFX_Start_01()
    {
        characterInstance.OnEventFromTheShadowsSkillFX_Start_01();
    }

    public void OnEventFromTheShadowsSkillFX_Loop()
    {
        characterInstance.OnEventFromTheShadowsSkillFX_Loop();
    }

    public void OnEventBladeStormEndAnimtionStart()
    {
        characterInstance.OnEventBladeStormEndAnimtionStart();
    }

    public void OnEventBladeStormEndAnimtionSpecial()
    {
        characterInstance.OnEventBladeStormEndAnimtionSpecial();
    }

    #region Sound skill 
    public void Omen_C_PlayFX_Sound_Start()
    {
        characterInstance.Omen_C_PlayFX_Sound_Start();
    }

    public void Omen_C_PlayFX_Sound_Cast()
    {
        characterInstance.Omen_C_PlayFX_Sound_Cast();
    }

    public void Omen_Q_PlayFX_Sound_Cast()
    {
        characterInstance.Omen_Q_PlayFX_Sound_Cast();
    }

    public void Omen_Q_PlayFX_Sound_Active()
    {
        characterInstance.Omen_Q_PlayFX_Sound_Active();
    }

    public void Omen_E_PlayFX_Sound_Start()
    {
        characterInstance.Omen_E_PlayFX_Sound_Start();
    }

    public void Omen_E_PlayFX_Sound_Cast()
    {
        characterInstance.Omen_E_PlayFX_Sound_Cast();
    }

    public void Omen_E_PlayFX_Sound_DarkSmoke()
    {
        characterInstance.Omen_E_PlayFX_Sound_DarkSmoke();
    }

    public void Omen_E_PlayFX_Sound_DarkSmoke_Disappeared()
    {
        characterInstance.Omen_E_PlayFX_Sound_DarkSmoke_Disappeared();
    }

    public void Omen_X_PlayFX_Sound_Start()
    {
        characterInstance.Omen_X_PlayFX_Sound_Start();
    }

    public void Omen_X_PlayFX_Sound_Active()
    {
        characterInstance.Omen_X_PlayFX_Sound_Active();
    }

    public void Omen_X_PlayFX_Sound_Voice_Line()
    {
        characterInstance.Omen_X_PlayFX_Sound_Voice_Line();
    }

    public void Jett_C_PlayFX_Sound_Cast()
    {
        characterInstance.Jett_C_PlayFX_Sound_Cast();
    }

    public void Jett_C_PlayFX_Sound_ExpandFullSize()
    {
        characterInstance.Jett_C_PlayFX_Sound_ExpandFullSize();
    }

    public void Jett_C_PlayFX_Sound_SmokeFullSize()
    {
        characterInstance.Jett_C_PlayFX_Sound_SmokeFullSize();
    }

    public void Jett_Q_PlayFX_Sound_Active()
    {
        characterInstance.Jett_Q_PlayFX_Sound_Active();
    }

    public void Jett_E_PlayFX_Sound_Cast()
    {
        characterInstance.Jett_E_PlayFX_Sound_Cast();
    }

    public void Jett_E_PlayFX_Sound_Active()
    {
        characterInstance.Jett_E_PlayFX_Sound_Active();
    }

    public void Jett_X_PlayFX_Sound_Active()
    {
        characterInstance.Jett_X_PlayFX_Sound_Active();
    }

    public void Jett_X_PlayFX_Sound_Start()
    {
        characterInstance.Jett_X_PlayFX_Sound_Start();
    }

    public void Jett_X_PlayFX_Sound_Active_End()
    {
        characterInstance.Jett_X_PlayFX_Sound_Active_End();
    }

    #endregion
}
