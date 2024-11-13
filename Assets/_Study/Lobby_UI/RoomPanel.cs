using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : MonoBehaviour
{
    [SerializeField] PlayerEntry[] playerEntries;
    [SerializeField] Button startButton;

    // �濡 ������ ��
    private void OnEnable() // or SetRoomPanel
    {
        UpdatePlayers();

        PlayerNumbering.OnPlayerNumberingChanged += UpdatePlayers;

        // Ȯ�� �޼���� �÷��̾������ �������� ���
        PhotonNetwork.LocalPlayer.SetReady(false);

        // or
        //Player localPlayer = PhotonNetwork.LocalPlayer;
        //PhotonHashtable customProperty = new PhotonHashtable();
        //customProperty.Add("Ready", false);
        //localPlayer.SetCustomProperties(customProperty);
    }

    void OnDisable()
    {
        PlayerNumbering.OnPlayerNumberingChanged -= UpdatePlayers;
    }

    public void UpdatePlayers()
    {
        foreach (PlayerEntry entry in playerEntries)
        {
            entry.SetEmpty();
        }

        // PlayerListOthers = �� ȭ���� �÷��̾ ������ ��� �÷��̾ �ҷ�����
        //       PlayerList = �÷��̾� ���� �ҷ�����
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // ���� ���ڰ� �������� �ʾ��� ��, �Ѿ��.
            if (player.GetPlayerNumber() == -1) continue;

            int number = player.GetPlayerNumber();
            playerEntries[number].SetPlayer(player);
        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startButton.interactable = CheckAllPlayerReady();
        }
        else
        {
            startButton.interactable = false;
        }
    }


    public void EnterPlayer(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} ����");
        UpdatePlayers();
    }

    public void ExitPlayer(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} ����");
        UpdatePlayers();
    }

    public void UpdatePlayerProperty(Player targetPlayer, Hashtable properties) // ������ : Photon�� Hashtable�� ������־�� �Ѵ�. Dictionary�� �ް����� ����.
    {
        Debug.Log($"{targetPlayer.NickName} ���� ����");

        // Ready Ŀ���� ������Ƽ�� ������ ����� ��, READY Ű�� �ִ�.
        if (properties.ContainsKey(CustomProperty.READY))
        {
            UpdatePlayers();
        }
    }

    private bool CheckAllPlayerReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetReady() == false) return false;
        }

        return true;
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("_TestGameScene");
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
