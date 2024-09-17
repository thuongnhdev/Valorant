using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiFPS.UI
{
    public class SkillHero : MonoBehaviour
    {
        public UISelectMenu parentMenu;
        [SerializeField] private GameObject GoChoiceEffect;
        [SerializeField] private Image ImgIcon;
        [SerializeField] private Toggle toggleSkill;

        private int skillType;

        public int SkillType { get => skillType; set => skillType = value; }

        private void Awake()
        {
            ImgIcon.gameObject.SetActive(false);
            toggleSkill.onValueChanged.AddListener(OnToggleSkill);
            OnToggleSkill(false);
        }

        public void UpdateSkillInfo(int typeHero, int typeSkill)
        {
            GoChoiceEffect.SetActive(false);
            ImgIcon.gameObject.SetActive(true);
            SkillType = typeSkill;
            switch (typeSkill)
            {
                case 0:
                    ImgIcon.sprite = SpriteManager.Instance.GetIconSkill_Skill_1(typeHero);
                    break;
                case 1:
                    ImgIcon.sprite = SpriteManager.Instance.GetIconSkill_Skill_2(typeHero);
                    break;
                case 2:
                    ImgIcon.sprite = SpriteManager.Instance.GetIconSkill_Skill_3(typeHero);
                    break;
                case 3:
                    ImgIcon.sprite = SpriteManager.Instance.GetIconSkill_Skill_4(typeHero);
                    break;
                case 4:
                    ImgIcon.sprite = SpriteManager.Instance.GetIconSkill_Skill_5(typeHero);
                    break;
            }
        }

        public void EnableActive(bool enable)
        {
            SkillDecs();
            var colorSet = ClientInterfaceManager.Instance.UIColorSet;
            Color colorPressed = colorSet.colorPressed;

            GoChoiceEffect.SetActive(enable);

            if (SkillType == 3)
                colorPressed = colorSet.colorSkill4;

            ImgIcon.color = enable ? colorPressed : colorSet.colorNormal;
        }

        private void OnToggleSkill(bool isOn)
        {
            if (isOn)
            {
                EnableActive(true);
            }
            else
            {
                EnableActive(false);
            }
        }

        public void SkillDecs()
        {
            var skillDescriptions = MasterCatcher.Instance.SkillDescriptionDatas;
            var Description = skillDescriptions.Find(x => x.SkillType == skillType && x.HeroIdx == parentMenu.IdxHeroSelected).Description;

            if (Description != null)
            {
                parentMenu.tmpSkillDesc.text = Description;
            }
            else
            {
                parentMenu.tmpSkillDesc.text = "Description not available";
            }
        }

    }
}
