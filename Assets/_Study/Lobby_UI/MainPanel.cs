using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class MainPanel : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject createRoomPanel;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_InputField maxPlayerInputField;


    private void OnEnable()
    {
        createRoomPanel.SetActive(false);
    }

    public void CreateRoomMenu()
    {
        createRoomPanel.SetActive(true);
        roomNameInputField.text = $"Room {Random.Range(1000, 10000)}";
        maxPlayerInputField.text = "8";
    }

    public void CreateRoomConfirm()
    {
        string roomName = roomNameInputField.text;

        if (roomName == "") return;

        int maxPlayer = int.Parse(maxPlayerInputField.text);
        maxPlayer = Mathf.Clamp(maxPlayer, 1, 8);

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = maxPlayer;

        PhotonNetwork.CreateRoom(roomName, options);
    }

    public void CreateRoomCancel()
    {
        createRoomPanel.SetActive(false);
    }

    public void RandomMatching()
    {
        Debug.Log("랜덤 매칭 요청");

        // 비어있는 방이 없을 때, 들어가지 않는 방식
        //PhotonNetwork.JoinRandomRoom(); 

        // 비어있는 방이 없을 때, 스스로 방을 만들어 들어가는 방식
        string name = $"Room {Random.Range(1000, 10000)}";
        RoomOptions options = new RoomOptions() { MaxPlayers = 8 };
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: name, roomOptions: options);
    }

    public void JoinLobby()
    {
        Debug.Log("로비 접속 신청");
        PhotonNetwork.JoinLobby();
    }

    public void Logout()
    {
        Debug.Log("로그아웃 요청");
        PhotonNetwork.Disconnect();
    }

    public void DeleteUser()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;

        // 현재 로그인 된 유저의 정보를 토대로 하여, 해당 유저의 이메일 인증 정보를 삭제를 요청할 수 있다.
        user.DeleteAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("DeleteAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                return;
            }

            // 요청에 성공하였다면, 유저의 인증 정보가 정상적으로 삭제되었다는 콘솔 메세지를 출력한다.
            Debug.Log("User deleted successfully.");
            // 이후 유저가 로그인 화면으로 돌아갈 수 있도록, 현재 유저의 접속을 끊는다.
            PhotonNetwork.Disconnect();
        });
    }
}
