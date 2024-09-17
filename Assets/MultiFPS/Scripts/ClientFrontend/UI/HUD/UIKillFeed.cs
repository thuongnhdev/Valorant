using System.Collections.Generic;
using UnityEngine;

using MultiFPS.Gameplay;
using MultiFPS.Gameplay.Gamemodes;
using UnityEngine.UI;

namespace MultiFPS.UI.HUD {
    /*

    public class UIKillFeed : MonoBehaviour
    {
        [SerializeField] GameObject _killFeedPrefab;
        [SerializeField] Transform _grid;
        [SerializeField] Transform _scaler;
        [SerializeField] public float Size = 0.75f;
        [SerializeField] byte _maxKillFeedElementsAtOnce = 5;
        byte _currentElementIDtoUse;
        private List<UIKillFeedElement> _elements = new List<UIKillFeedElement>();

        [SerializeField] VerticalLayoutGroup _verticalLayoutGroup;

        private void Start()
        {
            //spawn all killfeed tiles at the start of scene to use them later without need to instantiate them on runtime
            for (int i = 0; i < _maxKillFeedElementsAtOnce - 1; i++)
            {
                GameObject element = Instantiate(_killFeedPrefab, _grid.position, _grid.rotation);
                element.transform.SetParent(_grid);
                _elements.Add(element.GetComponent<UIKillFeedElement>());

                element.SetActive(false);
            }

            _elements.Add(_killFeedPrefab.GetComponent<UIKillFeedElement>());
            _killFeedPrefab.SetActive(false);

            _scaler.localScale = new Vector3(Size, Size, Size);
        }

        void OnGamemodeSet(Gamemode gamamemode)
        {
            GameManager.Gamemode.Client_PlayerKilledByPlayer += Killfeed;
        }
        void Killfeed(uint victimID, CharacterPart hittedPart, AttackType attackType, uint killerID)
        {
            _verticalLayoutGroup.enabled = false;

            if (_currentElementIDtoUse >= _elements.Count) _currentElementIDtoUse = 0;

            _elements[_currentElementIDtoUse].Write(victimID, hittedPart, attackType, killerID);
            _currentElementIDtoUse++;

        }
        public void SetTiles() 
        {
            _verticalLayoutGroup.enabled = true;
            _verticalLayoutGroup.CalculateLayoutInputVertical();
        }

        private void OnEnable()
        {
            GameManager.GameEvent_OnGamemodeSet += OnGamemodeSet;
        }
        private void OnDisable()
        {
            GameManager.GameEvent_OnGamemodeSet -= OnGamemodeSet;
        }
    }
    */
}
