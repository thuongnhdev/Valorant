using MultiFPS;
using MultiFPS.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintShellsSkill : MonoBehaviour
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

    public void InitSkill()
    {

    }

    public void SetCharacterAni()
    {

    }

    public void InitOption()
    {

    }
    private void MoveToPos(Transform posMove, Transform posFace_before, Transform posFace_after, float time, int type)
    {

    }

    private void MoveUpDown(float movePos, float time, int type)
    {

    }

    public void SetSkillInfo(int aniTrigger, Client_DataSkills.Param curSkill)
    {

    }
    public void StartSkill()
    {

    }

    public void Action_Shot(AnimationEvent type, Transform posTrans)
    {

    }

    public void Action_ShotReturn(AnimationEvent type)
    {

    }

    public bool Action_Bounce()
    {
        Debug.Log("Action_Bounce[START]");

        bool retIsFinish = false;

        return retIsFinish;
    }
    public void FinishAction()
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
