using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static MultiFPS.Gameplay.CharacterInstance;

namespace MultiFPS.Sound
{
    public class SoundManager : MonoBehaviour
    {
        private static SoundManager instance = null;
        public static SoundManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new SoundManager();
                return instance;
            }
        }
        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        public AudioSource Skill_EffectSource;
        public AudioSource Skill_EffectSource_OneShot;
        public AudioSource UI_EffectSource;

        [Header("File_sound_Hero")]
        public AudioClip[] FX_HERO_OMEN;
        public AudioClip[] FX_HERO_JETT;

        [Header("File_sound_fx")]
        public DataAudioClip[] FX_SKILL_OMEN_C;
        public DataAudioClip[] FX_SKILL_OMEN_Q;
        public DataAudioClip[] FX_SKILL_OMEN_E;
        public DataAudioClip[] FX_SKILL_OMEN_X;

        public DataAudioClip[] FX_SKILL_JETT_C;
        public DataAudioClip[] FX_SKILL_JETT_Q;
        public DataAudioClip[] FX_SKILL_JETT_E;
        public DataAudioClip[] FX_SKILL_JETT_X;


        [Header("File_sound_fx_Loop")]
        public DataAudioClip[] FX_SKILL_OMEN_C_LOOP;
        public DataAudioClip[] FX_SKILL_OMEN_Q_LOOP;
        public DataAudioClip[] FX_SKILL_OMEN_E_LOOP;
        public DataAudioClip[] FX_SKILL_OMEN_X_LOOP;

        public DataAudioClip[] FX_SKILL_JETT_C_LOOP;
        public DataAudioClip[] FX_SKILL_JETT_Q_LOOP;
        public DataAudioClip[] FX_SKILL_JETT_E_LOOP;
        public DataAudioClip[] FX_SKILL_JETT_X_LOOP;

        [Header("File_sound_fx_UI")]
        public DataAudioClip FX_ONCLICK_BUTTON;
        public DataAudioClip FX_SHOW_LOBBY_MENU;

        private bool IS_INIT = false;


        private void Start()
        {
            InitObj();
        }

        private void InitObj()
        {
            if (!IS_INIT)
            {
                IS_INIT = true;
                Skill_EffectSource.loop = false;
                Skill_EffectSource_OneShot.loop = false;
            }
        }

        public void SetTempBgmVolume(float rate)
        {
            Skill_EffectSource.volume = 1 * rate;
        }

        public void ResetTempBgmVolume()
        {
            Skill_EffectSource.volume = 1;
        }

        public void StopPlay_Battle_HeroSkill()
        {
            Skill_EffectSource.Stop();
        }

        public void StopPlayOneShot_Battle_HeroSkill()
        {
            if (Skill_EffectSource_OneShot == null) return;

            Skill_EffectSource_OneShot.volume = 0;
            Skill_EffectSource_OneShot.Stop();
        }

        public void PlayOneShot_Battle_HeroSkill(Skill skill, int index, float volumeScale = 1f)
        {
            if (index < 0) return;
            DataAudioClip[] fxSkill = GetAudioClipBySkill(skill, true);
            if (fxSkill.Length > index)
            {
                if (fxSkill[index] != null)
                {
                    Skill_EffectSource_OneShot.volume = fxSkill[index].VolumeFx;
                    Skill_EffectSource_OneShot.PlayOneShot(fxSkill[index].AudioClipFX, volumeScale * 1);
                }
            }
        }

        public async void Play_Battle_HeroSkill(Skill skill, int index, bool isLoop, float volume,int timeDuration)
        {
            Skill_EffectSource.loop = isLoop;
            DataAudioClip[] fxSkill = GetAudioClipBySkill(skill, false);
            Skill_EffectSource.clip = fxSkill[index].AudioClipFX;
            Skill_EffectSource.volume = fxSkill[index].VolumeFx;
            Skill_EffectSource.Play();

            await Task.Delay(timeDuration);
            Skill_EffectSource.volume = 0;
            Skill_EffectSource.Stop();

        }

        public DataAudioClip[] GetAudioClipBySkill(Skill skill, bool isOneShot)
        {
            DataAudioClip[] audioClip = null;
            switch (skill)
            {
                case Skill.ShroudedStepSkill:
                    audioClip = isOneShot == true ? FX_SKILL_OMEN_C : FX_SKILL_OMEN_C_LOOP;
                    break;
                case Skill.ParanoiaSkill:
                    audioClip = isOneShot == true ? FX_SKILL_OMEN_Q : FX_SKILL_OMEN_Q_LOOP;
                    break;
                case Skill.DarkCoverSkill:
                    audioClip = isOneShot == true ? FX_SKILL_OMEN_E : FX_SKILL_OMEN_E_LOOP;
                    break;
                case Skill.FromTheShadowsSkill:
                    audioClip = isOneShot == true ? FX_SKILL_OMEN_X : FX_SKILL_OMEN_X_LOOP;
                    break;
                case Skill.CloudBurstSkill:
                    audioClip = isOneShot == true ? FX_SKILL_JETT_C : FX_SKILL_JETT_C_LOOP;
                    break;
                case Skill.UpdraftSkill:
                    audioClip = isOneShot == true ? FX_SKILL_JETT_Q : FX_SKILL_JETT_Q_LOOP;
                    break;
                case Skill.TailWindSkill:
                    audioClip = isOneShot == true ? FX_SKILL_JETT_E : FX_SKILL_JETT_E_LOOP;
                    break;
                case Skill.BladeStormSkill:
                    audioClip = isOneShot == true ? FX_SKILL_JETT_X : FX_SKILL_JETT_X_LOOP;
                    break;
            }
            return audioClip;
        }

        public void PlayOneShot_UI(float volumeScale = 1f)
        {
            if (FX_ONCLICK_BUTTON == null) return;

            if (UI_EffectSource)
            {
                if (UI_EffectSource.isPlaying)
                    UI_EffectSource.Stop();

                UI_EffectSource.PlayOneShot(FX_ONCLICK_BUTTON.AudioClipFX, volumeScale);
            }
        }      
        public void OnPlaySoundLobbyMenu(float volumeScale = 1f)
        {
            if (FX_SHOW_LOBBY_MENU == null) return;

            if (UI_EffectSource)
            {
                if (UI_EffectSource.isPlaying)
                    UI_EffectSource.Stop();

                UI_EffectSource.PlayOneShot(FX_SHOW_LOBBY_MENU.AudioClipFX, volumeScale);
            }
        }
    }
    [System.Serializable]
    public class DataAudioClip
    {
        public AudioClip AudioClipFX;
        public float VolumeFx;
        public bool IsLocal;

        public DataAudioClip(AudioClip audioClip,float volume,bool isLocal)
        {
            AudioClipFX = audioClip;
            VolumeFx = volume;
            IsLocal = isLocal;
        }
    }
}
