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

        // SendPasswordResetEmailAsync(email) = �ۼ��� email�� �������, ��й�ȣ �缳�� ��û ������ �����ϴ� �۾��� ��û�Ѵ�.
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

            // ���������� ��й�ȣ �缳�� ������ �ۼ��� �̸��Ϸ� �����Ͽ��� ���, ���������� ���۵Ǿ��ٴ� �ܼ� �޼����� ����ϰ�,
            Debug.Log("Password reset email sent successfully.");
            // ��й�ȣ �缳�� ȭ���� ��Ȱ��ȭ �Ѵ�.
            gameObject.SetActive(false);
        });
    }
}
