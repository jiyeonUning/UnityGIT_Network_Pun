using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResetPassPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField Re_EmailInputField;


    public void SendResetEmail()
    {
        string email = Re_EmailInputField.text;

        // SendPasswordResetEmailAsync(email) = 작성된 email을 기반으로, 비밀번호 재설정 요청 메일을 전송하는 작업을 요청한다.
        BackendManager.Auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                return;
            }

            // 성공적으로 비밀번호 재설정 메일을 작성된 이메일로 전송하였을 경우, 정상적으로 전송되었다는 콘솔 메세지를 출력하고,
            Debug.Log("Password reset email sent successfully.");
            // 비밀번호 재설정 화면을 비활성화 한다.
            gameObject.SetActive(false);
        });
    }
}
