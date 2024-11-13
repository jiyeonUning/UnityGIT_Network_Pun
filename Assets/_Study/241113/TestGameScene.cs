using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameScene : MonoBehaviourPunCallbacks
{
    public const string RoomName = "TestRoom";

    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;
        options.IsVisible = false;

        PhotonNetwork.JoinOrCreateRoom(RoomName, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(StartDelayRoutine());
    }

    IEnumerator StartDelayRoutine()
    {
        // 네크워크 준비에 필요한 시간 딜레이를 주는 용도의 코루틴
        yield return new WaitForSeconds(1f);
        TestGameStart();
    }

    public void TestGameStart()
    {
        Debug.Log("게임 시작");

        // 테스트용 게임 시작 부분
        PlayerSpawn();
    }

    // === === ===

    private void PlayerSpawn()
    {
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

        // 경로가 필요할 땐, "폴더/프리팹이름" 형식으로 작성 시 사용이 가능해짐.
        PhotonNetwork.Instantiate("TestPlayer", randomPos, Quaternion.identity);
    }
}
