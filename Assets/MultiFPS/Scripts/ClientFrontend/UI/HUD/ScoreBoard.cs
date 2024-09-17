using MultiFPS.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MultiFPS.UI.HUD
{
    public class ScoreBoard : MonoBehaviour
    {
        [SerializeField] GameObject _scoreBoard;
        [SerializeField] GameObject _playerPresenter;
        [SerializeField] Transform _gridTeam;
        [SerializeField] Transform _gridEnemy;

        private List<GameObject> _instantiatedPresenters = new List<GameObject>();
        Coroutine c_refresher;

        float widthScale;
        float heightScale;

        void Start()
        {
            _playerPresenter.SetActive(false);
        }

        IEnumerator RefreshScoreboard()
        {
            while (true)
            {
                foreach (GameObject gm in _instantiatedPresenters)
                {
                    Destroy(gm);
                }
                _instantiatedPresenters.Clear();

                List<PlayerInstance> players = GameManager.Players;

                var sortedPlayers = players.FindAll(x => x.Team >= 0);
                sortedPlayers = sortedPlayers.OrderBy(x => x.Team == ClientFrontend.ThisClientTeam ? 0 : 1)
                                             .ThenByDescending(x => x.Kills).ToList();

                for (int i = 0; i < sortedPlayers.Count; i++)
                {
                    PlayerInstance player = sortedPlayers[i];
                    GameObject presenter;
                    if (player.Team == ClientFrontend.ThisClientTeam)
                    {
                        presenter = Instantiate(_playerPresenter, _gridTeam.position, _gridTeam.rotation);
                        presenter.transform.SetParent(_gridTeam);
                    }
                    else
                    {
                        presenter = Instantiate(_playerPresenter, _gridEnemy.position, _gridEnemy.rotation);
                        presenter.transform.SetParent(_gridEnemy);
                    }

                    presenter.SetActive(true);
                    presenter.GetComponent<UIScoreBoardPlayerElement>().WriteData(player);
                    presenter.transform.localScale = Vector3.one;
                    _instantiatedPresenters.Add(presenter);
                }

                yield return new WaitForSeconds(0.5f);
            }
        }

        void Update()
        {
            _scoreBoard.SetActive(Input.GetKey(KeyCode.Tab) && ClientFrontend.GamePlayInput());

            if (_scoreBoard.activeSelf && c_refresher == null)
            {
                c_refresher = StartCoroutine(RefreshScoreboard());
            }

            if (!_scoreBoard.activeSelf && c_refresher != null)
            {
                StopCoroutine(c_refresher);
                c_refresher = null;
            }
        }
    }
}
