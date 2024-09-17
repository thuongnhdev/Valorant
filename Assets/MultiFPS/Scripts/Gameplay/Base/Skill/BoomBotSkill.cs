using MultiFPS;
using MultiFPS.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomBotSkill : MonoBehaviour
{
    public CharacterInstance _charInstance;
    public CharacterMotor CharMotor;
    public CharacterAnimator CharAni;
    protected Client_DataSkills.Param CurrentSkill;

    private HeroSkill.Param _heroSkill;
    public CharacterController _controller;
    public enum StepSkill
    {
        Start = 0,
        EquipState = 1,
        CastState = 2,
        ActivationState = 3,
        End = 4,

    }


    #region fx skill
    public GameObject fx_omen_c_cast_01;
    public GameObject fx_omen_c_ready_01;
    public GameObject fx_omen_c_ready_02;
    public GameObject fx_omen_c_ready_03;

    public GameObject fx_omen_e_cast_01;
    public GameObject fx_omen_e_cast_02;
    public GameObject fx_omen_e_ready_02;

    public GameObject fx_omen_e_trail;

    public GameObject fx_omen_q_bullet;
    public GameObject fx_omen_q_cast_01_1;
    public GameObject fx_omen_q_cast_01_2;
    public GameObject fx_omen_q_cast_02;
    public GameObject fx_omen_q_cast_03;

    public GameObject fx_omen_x_cast_01;
    public GameObject fx_omen_x_ready_01;
    #endregion

    #region
    public Animator ability_c;
    public Animator ability_e;
    public Animator ability_q;
    public Animator ability_x;
    #endregion

    public string Start = "IsStart";
    public string EquipStateAnimation = "IsEquipState";
    public string CastStateAnimation = "IsCastState";
    public string ActivationStateAnimation = "IsActivationState";
    public string End = "IsEnd";

    public StepSkill CurrentStepSkill;
    public void InitSkill()
    {
        _heroSkill = ClientDataTable.Instance._heroSkill.param.Find(t => t.IDSkill == (int)IDSkill.BoomBot);
        CurrentStepSkill = StepSkill.Start;
        SetCharacterAni(CurrentStepSkill);
    }

    public void SetCharacterAni(StepSkill stepSkill)
    {
        switch (stepSkill)
        {
            case StepSkill.Start:
                CharAni.SetTrigger(Start);
                break;
            case StepSkill.EquipState:
                CurrentStepSkill = StepSkill.EquipState;
                CharAni.SetTrigger(EquipStateAnimation);
                break;
            case StepSkill.CastState:
                CurrentStepSkill = StepSkill.CastState;
                CharAni.SetTrigger(CastStateAnimation);
                break;
            case StepSkill.ActivationState:
                CurrentStepSkill = StepSkill.ActivationState;
                CharAni.SetTrigger(ActivationStateAnimation);
                break;
            case StepSkill.End:
                CurrentStepSkill = StepSkill.End;
                CharAni.SetTrigger(End);
                break;
        }
    }

    public void InitOption()
    {

    }

    public void SetSkillInfo(int aniTrigger, Client_DataSkills.Param curSkill)
    {

    }
    public void Action_Start()
    {

    }

    public void Action_EquipState(AnimationEvent type, Transform posTrans)
    {

    }

    public void Action_CastState(AnimationEvent type, Transform posTrans)
    {

    }

    public void Action_ActivationState(AnimationEvent type, Transform posTrans)
    {

    }

    public void Action_End(AnimationEvent type, Transform posTrans)
    {

    }

    private void MoveToPos(Transform posMove, Transform posFace_before, Transform posFace_after, float time, int type)
    {

    }

    private void MoveUpDown(float movePos, float time, int type)
    {

    }

    private void UpdateInfo()
    {

    }


    public void FinishProjectile(GameObject obj)
    {

    }


    public Client_DataSkills.Param GetCurrentAction()
    {
        return CurrentSkill;
    }
}
