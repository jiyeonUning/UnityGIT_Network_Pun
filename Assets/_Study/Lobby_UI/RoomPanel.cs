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

    // 방에 들어왔을 때
    private void OnEnable() // or SetRoomPanel
    {
        UpdatePlayers();

        PlayerNumbering.OnPlayerNumberingChanged += UpdatePlayers;

        // 확장 메서드로 플레이어에서부터 가져오는 방법
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

        // PlayerListOthers = 내 화면의 플레이어를 제외한 모든 플레이어를 불러오기
        //       PlayerList = 플레이어 포함 불러오기
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // 아직 숫자가 배정되지 않았을 땐, 넘어간다.
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
        Debug.Log($"{newPlayer.NickName} 입장");
        UpdatePlayers();
    }

    public void ExitPlayer(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} 퇴장");
        UpdatePlayers();
    }

    public void UpdatePlayerProperty(Player targetPlayer, Hashtable properties) // 주의점 : Photon의 Hashtable로 사용해주어야 한다. Dictionary과 햇갈리지 말것.
    {
        Debug.Log($"{targetPlayer.NickName} 정보 변경");

        // Ready 커스텀 프로퍼티를 변경한 경우일 때, READY 키가 있다.
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
