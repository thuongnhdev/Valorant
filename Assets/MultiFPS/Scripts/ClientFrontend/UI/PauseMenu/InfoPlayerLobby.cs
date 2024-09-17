using MultiFPS.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPlayerLobby : MonoBehaviour
{
    [SerializeField] private Button btnPlus;
    [SerializeField] private Image imgAva;
    [SerializeField] private TextMeshProUGUI txtPlayerName;
    [SerializeField] private GameObject objLeader;

    private bool isLeader;

    private void GetUserName()
    {
        txtPlayerName.text = GetComponent<PlayerInstance>().PlayerInfo.Username;
    }



}
