using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable; // 클래스 임시 이름 변경을 해주는 using구문

public class PlayerEntry : MonoBehaviour
{
    [SerializeField] TMP_Text readyText;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Button readyButton;

    public void SetPlayer(Player player)
    {
        if (player.IsMasterClient)
        {
            nameText.text = $"*{player.NickName}";
        }
        else
        {
            nameText.text = player.NickName;
        }

        readyButton.gameObject.SetActive(true);
        readyButton.interactable = player ==  PhotonNetwork.LocalPlayer;

        if (player.GetReady())
        {

            readyText.text = "Ready";
        }
        else
        {
            readyText.text = "";
        }
    }

    public void SetEmpty()
    {
        readyText.text = "";
        nameText.text = "None";
        readyButton.gameObject.SetActive(false);
    }

    public void Ready()
    {
        //Player localPlayer = PhotonNetwork.LocalPlayer;

        //PhotonHashtable hashTable = localPlayer.CustomProperties;
        //bool ready = (bool)hashTable["Ready"];

        //PhotonHashtable customProperty = new PhotonHashtable();
        //customProperty.Add("Ready", !ready);

        //localPlayer.SetCustomProperties(customProperty);

        // or

        bool ready = PhotonNetwork.LocalPlayer.GetReady();

        if (ready)
        {

            PhotonNetwork.LocalPlayer.SetReady(false);
        }
        else
        {
            PhotonNetwork.LocalPlayer.SetReady(true);
        }
    }
}
