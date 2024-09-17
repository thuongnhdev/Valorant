using MultiFPS.Gameplay;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiFPS.UI
{
    public class UISelectMenu : UIMenu
    {
        [SerializeField] private TextMeshProUGUI tmpCooldown;
        [SerializeField] private Image imgFillTime;
        [SerializeField] private Button btnReady;
        [SerializeField] private Button btnBlock;
        [SerializeField] private HeroSelectScroll heroSelectScroll;
        [SerializeField] private Image imgHero;
        [SerializeField] private TextMeshProUGUI heroName;
        [SerializeField] private ToggleGroup groupHeroSkill;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] audioClips;
        [SerializeField] private AudioClip[] clipSelect;
        [SerializeField] private AudioClip clipLockCharacter;
        [SerializeField] private List<SkillHero> ListActionSkill;
        [SerializeField] private HeroCardCell mySlot;
        [SerializeField] private GameObject objHeroCard;
        [SerializeField] private GameObject objHeroInfo;

        public TextMeshProUGUI tmpSkillDesc;

        [HideInInspector]
        public int IdxHeroSelected = 0;

        private const int TIME_COOLDOWN_SELECT_HERO = 50;

        private void Awake()
        {
            btnReady.onClick.AddListener(OnClickReady);
        }

        private void OnClickReady()
        {
            StartCoroutine(LockCharacter());
        }

        private void OnEnable()
        {
            heroSelectScroll.SetScroll();
            /*StartCoroutine(CoolDownSelect());
            List<PlayerInstance> players = GameManager.Players;

            for (int i = 0; i < players.Count; i++)
            {
                PlayerInstance player = players[i];

                GameObject presenter = Instantiate(objHeroCard);
                presenter.SetActive(true);
                presenter.GetComponent<HeroCardCell>().WriteData(player);
            }*/
            objHeroInfo.SetActive(false);
            UserSettings.SelectedCharacterModel = IdxHeroSelected;
        }



        private IEnumerator CoolDownSelect()
        {
            int timecoolDown = TIME_COOLDOWN_SELECT_HERO;
            while (timecoolDown > 0)
            {
                tmpCooldown.text = timecoolDown.ToString();
                imgFillTime.fillAmount = (float)timecoolDown / TIME_COOLDOWN_SELECT_HERO;
                btnBlock.gameObject.SetActive(IdxHeroSelected <= 0);

                yield return new WaitForSeconds(1);
                timecoolDown--;
                if (timecoolDown == 0)
                {
                    List<Client_DataHero.Param> dataHero = ClientDataTable.Instance.GetClientData<Client_DataHero>().param;
                    ClientInterfaceManager.Instance.IdxHeroSelected = Random.Range(dataHero[0].Idx, dataHero.Count);
                    OnClickReady();
                }
            }
        }

        public void SetHeroSlot(CellElement cell)
        {
            StartCoroutine(OnActionSound(cell.idx));

            imgHero.sprite = SpriteManager.Instance.GetImgHeroSelected(cell.HeroData.assetIdx);
            imgHero.gameObject.SetActive(true);
            heroName.text = cell.HeroData.name;
            mySlot.SetMyHero(cell);
            IdxHeroSelected = cell.HeroData.idx - 1;
            UserSettings.SelectedCharacterModel = IdxHeroSelected;
            btnBlock.gameObject.SetActive(false);
            objHeroInfo.SetActive(true);
            cell.isSelected = true;
            for (int i = 0; i < ListActionSkill.Count; i++)
            {
                ListActionSkill[i].UpdateSkillInfo(IdxHeroSelected, i);
                ListActionSkill[i].GetComponent<Toggle>().group = groupHeroSkill;
            }

            if (ListActionSkill.Count > 0)
            {
                ListActionSkill[0].GetComponent<Toggle>().isOn = true;
                ListActionSkill[0].EnableActive(true);
            }

            heroSelectScroll.RefreshScroll();
        }

        private IEnumerator OnActionSound(int idx)
        {
            if (audioSource != null && audioSource.clip != null)
            {
                if (audioSource.isPlaying)
                    audioSource.Stop();

                for (int i = 0; i < MasterCatcher.Instance.HeroDatas.Count; i++)
                {
                    if (i == idx)
                    {
                        audioSource.PlayOneShot(clipSelect[i]);
                        yield return new WaitForSeconds(clipSelect[i].length);
                        audioSource.PlayOneShot(audioClips[i]);
                    }
                }
            }
        }

        private IEnumerator LockCharacter()
        {
            if (clipLockCharacter)
            {
                if (audioSource.isPlaying)
                    audioSource.Stop();

                audioSource.PlayOneShot(clipLockCharacter);
                btnBlock.gameObject.SetActive(true);
                yield return new WaitForSeconds(clipLockCharacter.length);

                this.Hide();
                ClientInterfaceManager.Instance.UILoadingBattle.gameObject.SetActive(true);
            }
        }
    }
}