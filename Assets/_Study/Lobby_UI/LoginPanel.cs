using Photon.Pun;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using static UnityEditor.ShaderData;
using WebSocketSharp;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField emailInputField;
    [SerializeField] TMP_InputField passwordlInputField;

    [SerializeField] NickNamePanel NickNamePanel;
    [SerializeField] VerfiPanel VerfiPanel;


    public void Login()
    {
        #region 파이어 베이스 도입 이전 로그인 방법
        //if (idInputField.text == "") return;

        // 서버에게 요청할 땐, 항상 PhotonNetwork로 할 수 있다.
        //PhotonNetwork.LocalPlayer.NickName = idInputField.text; // 플레이어의 닉네임 설정
        //PhotonNetwork.ConnectUsingSettings();                   // 포톤 설정파일을 해당 내용으로 접속을 신청
        #endregion

        string email = emailInputField.text;
        string password = passwordlInputField.text;

        // ( ).CreateUserWithEmailAndPasswordAsync = 이메일과 패스워드를 통해, ()의 절차를 진행한다.
        BackendManager.Auth.SignInWithEmailAndPasswordAsync(email, password)
    .ContinueWithOnMainThread(task =>
    {
        if (task.IsCanceled)
        {
            // 요청이 취소, 즉 로그인이 중간에 취소되었다면, 넘어간다.
            Debug.Log("CreateUserWithEmailAndPasswordAsync was canceled.");
            return;
        }
        if (task.IsFaulted)
        {
            // 요청이 실패, 즉 로그인이 실패했다면, 실패 사유를 콘솔 창에 띄운 후, 넘어간다.
            Debug.Log($"CreateUserWithEmailAndPasswordAsync encountered an error :" + task.Exception);
            return;
        }

        // 위 두 경우를 넘기고 요청에 성공하였을 경우,

        // 해당 인증 결과를 저장하고, 
        AuthResult result = task.Result;
        // 어떠한 고유 닉네임과 고유 아이디를 가진 플레이어가 로그인 되었는지 콘솔창에 띄운다.
        Debug.Log($"Firebase user created successfully : {result.User.DisplayName} ({result.User.UserId})");
        // 로그인 직전에, 플레이어의 정보를 불러오는 함수를 실행한다.
        CheckUserInfo();
        // 로그인이 완료되었으므로, 로그인 화면을 비활성화 하여 메뉴 화면으로 넘어간다.
        //gameObject.SetActive(false);

    });
    }



    void CheckUserInfo()
    {
        // 현재 로그인이 완료된 현재 접속된 사용자(=Currentuser)의 정보를 가져와 저장하고,
        FirebaseUser user = BackendManager.Auth.CurrentUser;

        // 해당 값이 null인지 아닌지에 따라, 플레이어의 로그인 유무를 판단한다.
        // user가 null인 경우, 즉, 로그인이 되어있지 않을 경우, 해당 함수는 실행하지 않는다.
        if (user == null) return;


        // 이메일 인증이 아직 되어있지 않을 때,
        if (user.IsEmailVerified == false)
        {
            // 이메일 인증창을 띄워, 이메일 인증을 진행한다.
            VerfiPanel.gameObject.SetActive(true);
        }

        // 또는 유저의 닉네임이 아직 설정되어 있지 않았을 경우,
        else if (user.DisplayName == "")
        {
             // 닉네임 설정 창을 띄워, 닉네임 설정을 진행한다.
             NickNamePanel.gameObject.SetActive(true);
        }

        // 위 모든 과정이 이미 설정 or 설정을 마쳤을 경우,
        else
        {
            // 현재 접속된 사용자의 닉네임은
            // 현 사용자의 이메일을 토대로 파이어 베이스에서 데이터를 검색하여, 해당하는 장소에 저장되어 있던 닉네임으로 설정한다.
            PhotonNetwork.LocalPlayer.NickName = user.DisplayName;
            // 모든 과정이 마무리 되었으므로, 네트워크로 접속을 시도한다.
            PhotonNetwork.ConnectUsingSettings();
        }
    }    

}
