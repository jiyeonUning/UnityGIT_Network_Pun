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
        // ��ũ��ũ �غ� �ʿ��� �ð� �����̸� �ִ� �뵵�� �ڷ�ƾ
        yield return new WaitForSeconds(1f);
        TestGameStart();
    }

    public void TestGameStart()
    {
        Debug.Log("���� ����");

        // �׽�Ʈ�� ���� ���� �κ�
        PlayerSpawn();

        // ���� ������ �ƴ� ��, �Ʒ� ������ �������� �ʴ´�.
        if (PhotonNetwork.IsMasterClient == false) return;

        // �ش� ������ ���� ������ ��쿡�� ����ȴ�.
        spawnRoutine = StartCoroutine(MonsterSpawnRoutine());
    }

    // === === ===

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // ������ ���Ӱ� ������ ��� ����Ǵ� �Լ� OnMasterClientSwitched

        // ���� ���Ӱ� ������ ������ ���, if�� ���� �ڵ带 �����Ѵ�.
        if (newMasterClient.IsLocal)
        {
            spawnRoutine = StartCoroutine(MonsterSpawnRoutine());
        }
    }

    // === === ===

    private void PlayerSpawn()
    {
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 1, Random.Range(-5f, 5f));

        // �÷��̾� ���� �� ������ �����ϰ� ������ �� �ֵ��� �ϴ� �ڵ�
        // Color color = Random.ColorHSV();
        // object[] data = { color.r, color.g, color.b };

        // ��ΰ� �ʿ��� ��, "����/�������̸�" �������� �ۼ� �� ����� ��������.
        PhotonNetwork.Instantiate("TestPlayer", randomPos, Quaternion.identity /*,data : data*/);
    }


    Coroutine spawnRoutine;

    IEnumerator MonsterSpawnRoutine()
    {
        // 3�ʿ� �� �� �� ���͸� �����ϴ� �ڷ�ƾ
        WaitForSeconds delay = new WaitForSeconds(3f);

        while (true)
        {
            yield return delay;

            Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

            // InstantiateRoomObject : �ش� ���� ������Ʈ�� ������ ��Ʈ�� ������ ������ ������, ���� ������ ������ �ƴ� ���� ������ ������Ʈ�� �����ϰ� �ȴ�.
            PhotonNetwork.InstantiateRoomObject("Monster", randomPos, Quaternion.identity);
        }
    }
}
