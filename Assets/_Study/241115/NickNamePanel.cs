using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class NickNamePanel : MonoBehaviour
{
    [SerializeField] TMP_InputField NickNameInputField;


    public void Confirm()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;
        string nickName = NickNameInputField.text;

        // 닉네임 설정이 되어있지 않은 채로 Confirm을 눌렀을 경우, 닉네임을 설정할 수 있도록 재시도한다.
        if (nickName == "") return;

        // 유저 프로필을 선언하고,
        UserProfile profile = new UserProfile();
        // 해당 프로필 내의 유저 닉네임을, InputField에 작성된 텍스트 값으로 설정한다.
        profile.DisplayName = nickName;


        // 현재 접속해 있는 유저의 프로필을, 위에서 선언한 프로필 profile의 정보값으로 저장하고,
        BackendManager.Auth.CurrentUser.UpdateUserProfileAsync(profile)
            // ContinueWithOnMainThread 아래로 작성된 코드를 실행한다.
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    // 요청이 취소되었을 경우, 넘어간다.
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    // 요청이 실패하였을 경우, 실패 사유를 콘솔 창에 띄운 후, 넘어간다.
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                // 위 과정을 넘겼을 경우, 프로필이 성공적으로 설정 되었다는 콘솔창을 띄운다.
                Debug.Log("User profile updated successfully.");

                // 성공적으로 설정된 프로필의 정보를 콘솔창에 출력한다.
                Debug.Log($"Display Name : {user.DisplayName}");
                Debug.Log($"Email : {user.Email}");
                Debug.Log($"Email verified : {user.IsEmailVerified}");
                Debug.Log($"User ID : {user.UserId}");

                // 위 닉네임 값을 토대로 포톤에 접속하기 위해, 포톤에 저장된 플레이어 닉네임을 -> 설정된 닉네임으로 변경한다.
                PhotonNetwork.LocalPlayer.NickName = nickName;
                // 네트워크에 접속을 시도한다.
                PhotonNetwork.ConnectUsingSettings();

                // 닉네임이 정상적으로 설정되었으므로, 닉네임 화면을 비활성화 한다.
                gameObject.SetActive(false);
            });
    }
}
