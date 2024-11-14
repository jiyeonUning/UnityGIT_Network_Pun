using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviourPunCallbacks
{
    public const string RoomName = "TestRoom01";

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

        // 내가 방장이 아닐 때, 아래 내용은 실행하지 않는다.
        if (PhotonNetwork.IsMasterClient == false) return;

        // 해당 내용은 내가 방장일 경우에만 실행된다.
        spawnRoutine = StartCoroutine(MonsterSpawnRoutine());
    }

    // === === ===

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // 방장이 새롭게 선정될 경우 실행되는 함수 OnMasterClientSwitched

        // 내가 새롭게 선정된 방장일 경우, if문 내의 코드를 실행한다.
        if (newMasterClient.IsLocal)
        {
            spawnRoutine = StartCoroutine(MonsterSpawnRoutine());
        }
    }

    // === === ===

    private void PlayerSpawn()
    {
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 1, Random.Range(-5f, 5f));

        // 플레이어 생성 시 색상을 랜덤하게 선정할 수 있도록 하는 코드
        // Color color = Random.ColorHSV();
        // object[] data = { color.r, color.g, color.b };

        // 경로가 필요할 땐, "폴더/프리팹이름" 형식으로 작성 시 사용이 가능해짐.
        PhotonNetwork.Instantiate("TestPlayer", randomPos, Quaternion.identity /*,data : data*/);
    }


    Coroutine spawnRoutine;

    IEnumerator MonsterSpawnRoutine()
    {
        // 3초에 한 번 씩 몬스터를 생성하는 코루틴
        WaitForSeconds delay = new WaitForSeconds(3f);

        while (true)
        {
            yield return delay;

            Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

            // InstantiateRoomObject : 해당 게임 오브젝트는 방장이 컨트롤 권한을 가지고 있으나, 방장 개인의 소유가 아닌 공공 소유의 오브젝트로 생성하게 된다.
            PhotonNetwork.InstantiateRoomObject("Monster", randomPos, Quaternion.identity);
        }
    }
}
