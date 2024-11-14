using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntry : MonoBehaviour
{
    [SerializeField] TMP_Text readyText;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Button readyButton;

    public void SetPlayer(Player player)
    {
        if (player.IsMasterClient)
        {
            nameText.text = $"Master\n{player.NickName}";
        }
        else
        {
            nameText.text = player.NickName;
        }

        readyButton.gameObject.SetActive(true);
        readyButton.interactable = player == PhotonNetwork.LocalPlayer;

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
