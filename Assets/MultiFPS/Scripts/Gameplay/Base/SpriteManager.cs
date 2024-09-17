using UnityEngine;

namespace MultiFPS
{
    public class SpriteManager : MonoBehaviour
    {
        public static SpriteManager Instance;

        public Sprite[] IconClass;
        public Sprite[] IconHero;
        public Sprite[] ImgHeros;
        public Sprite[] IconSkill1;
        public Sprite[] IconSkill2;
        public Sprite[] IconSkill3;
        public Sprite[] IconSkill4;
        public Sprite[] IconSkill5;
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        public Sprite GetIconHero(int index)
        {
            return IconHero[index];
        }      
        public Sprite GetImgHeroSelected(int index)
        {
            return ImgHeros[index];
        }

        public Sprite GetIconHeroClass(int index)
        {
            return IconClass[index];
        }        

        public Sprite GetIconSkill_Skill_1(int index)
        {
            return IconSkill1[index];
        }    

        public Sprite GetIconSkill_Skill_2(int index)
        {
            return IconSkill2[index];
        }  
        
        public Sprite GetIconSkill_Skill_3(int index)
        {
            return IconSkill3[index];
        }

        public Sprite GetIconSkill_Skill_4(int index)
        {
            return IconSkill4[index];
        }
        public Sprite GetIconSkill_Skill_5(int index)
        {
            return IconSkill5[index];
        }

    }
}