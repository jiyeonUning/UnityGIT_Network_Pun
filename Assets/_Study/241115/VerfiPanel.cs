using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using UnityEngine;

public class VerfiPanel : MonoBehaviour
{
    [SerializeField] NickNamePanel nickNamePanel;


    private void OnEnable()
    {
        // 화면이 활성화 될 때, 등록된 이메일로 인증 이메일을 보낸다.
        SendVerifyMail();
    }

    private void OnDisable()
    {
        // 화면 비활성화 = 이메일 인증 성공 or 실패 이므로, 인증을 확인하는 코루틴을 정지한다.
        if (checkVerifyRoutine != null)
        {
            StopCoroutine(checkVerifyRoutine);
        }
    }


    void SendVerifyMail()
    {
        // 현재 로그인 된 유저의 정보값을 저장하고,
        FirebaseUser user = BackendManager.Auth.CurrentUser;

        // 유저에게 이메일 요청을 보내고, 아래 코드를 실행한다.
        user.SendEmailVerificationAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    // 이메일 인증 요청을 보내는 것이 취소되었을 경우, 콘솔창에 인증 취소 메세지를 띄우고,
                    Debug.LogError("SendEmailVerificationAsync was canceled.");
                    // 이메일 인증 화면을 비활성화 한다.
                    gameObject.SetActive(false);
                    return;
                }
                if (task.IsFaulted)
                {
                    // 이메일 인증 요청을 보내는 것이 실패하였을 경우, 콘솔창에 실패 메세지와 함께 실패 요인을 띄우고,
                    Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                    // 이메일 인증 화면을 비활성화 한다.
                    gameObject.SetActive(false);
                    return;
                }

                // 위 과정을 넘겼을 경우, 이메일 인증 요청이 성공하였다는 콘솔 메세지를 출력한다.
                Debug.Log("Email sent successfully.");

                // 주기적으로 이메일 인증이 되었는지 확인한다.
                checkVerifyRoutine = StartCoroutine(CheckVerifyRoutine());
            });
    }

    Coroutine checkVerifyRoutine;
    IEnumerator CheckVerifyRoutine()
    {
        // 3초에 한번씩 실행되도록, 코루틴 실행에 딜레이를 설정한다.
        WaitForSeconds delay = new WaitForSeconds(3f);

        while (true)
        {      
            // CurrentUser.ReloadAsync = 현재 유저의 정보를 새로고침 하여 가져온다.
            BackendManager.Auth.CurrentUser.ReloadAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("ReloadAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"ReloadAsync encountered an error :" + task.Exception.Message);
                    return;
                }

                // 현재 유저의 이메일 인증 여부를 확인하고, 해당 값이 true일 경우,
                if (BackendManager.Auth.CurrentUser.IsEmailVerified == true)
                {
                    // 닉네임 설정 화면을 활성화 하고,
                    nickNamePanel.gameObject.SetActive(true);
                    // 이메일 인증 진행 화면을 비활성화 한다.
                    gameObject.SetActive(false);
                }
            });

            // 인증 확인이 아직 되어있지 않았을 경우, 해당 코루틴을 delay 시간 후 다시 진행한다.
            yield return delay;
        }
    }
}
