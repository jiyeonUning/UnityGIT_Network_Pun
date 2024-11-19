using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using UnityEngine;

public class VerfiPanel : MonoBehaviour
{
    [SerializeField] NickNamePanel nickNamePanel;


    private void OnEnable()
    {
        // ȭ���� Ȱ��ȭ �� ��, ��ϵ� �̸��Ϸ� ���� �̸����� ������.
        SendVerifyMail();
    }

    private void OnDisable()
    {
        // ȭ�� ��Ȱ��ȭ = �̸��� ���� ���� or ���� �̹Ƿ�, ������ Ȯ���ϴ� �ڷ�ƾ�� �����Ѵ�.
        if (checkVerifyRoutine != null)
        {
            StopCoroutine(checkVerifyRoutine);
        }
    }


    void SendVerifyMail()
    {
        // ���� �α��� �� ������ �������� �����ϰ�,
        FirebaseUser user = BackendManager.Auth.CurrentUser;

        // �������� �̸��� ��û�� ������, �Ʒ� �ڵ带 �����Ѵ�.
        user.SendEmailVerificationAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    // �̸��� ���� ��û�� ������ ���� ��ҵǾ��� ���, �ܼ�â�� ���� ��� �޼����� ����,
                    Debug.LogError("SendEmailVerificationAsync was canceled.");
                    // �̸��� ���� ȭ���� ��Ȱ��ȭ �Ѵ�.
                    gameObject.SetActive(false);
                    return;
                }
                if (task.IsFaulted)
                {
                    // �̸��� ���� ��û�� ������ ���� �����Ͽ��� ���, �ܼ�â�� ���� �޼����� �Բ� ���� ������ ����,
                    Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                    // �̸��� ���� ȭ���� ��Ȱ��ȭ �Ѵ�.
                    gameObject.SetActive(false);
                    return;
                }

                // �� ������ �Ѱ��� ���, �̸��� ���� ��û�� �����Ͽ��ٴ� �ܼ� �޼����� ����Ѵ�.
                Debug.Log("Email sent successfully.");

                // �ֱ������� �̸��� ������ �Ǿ����� Ȯ���Ѵ�.
                checkVerifyRoutine = StartCoroutine(CheckVerifyRoutine());
            });
    }

    Coroutine checkVerifyRoutine;
    IEnumerator CheckVerifyRoutine()
    {
        // 3�ʿ� �ѹ��� ����ǵ���, �ڷ�ƾ ���࿡ �����̸� �����Ѵ�.
        WaitForSeconds delay = new WaitForSeconds(3f);

        while (true)
        {      
            // CurrentUser.ReloadAsync = ���� ������ ������ ���ΰ�ħ �Ͽ� �����´�.
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

                // ���� ������ �̸��� ���� ���θ� Ȯ���ϰ�, �ش� ���� true�� ���,
                if (BackendManager.Auth.CurrentUser.IsEmailVerified == true)
                {
                    // �г��� ���� ȭ���� Ȱ��ȭ �ϰ�,
                    nickNamePanel.gameObject.SetActive(true);
                    // �̸��� ���� ���� ȭ���� ��Ȱ��ȭ �Ѵ�.
                    gameObject.SetActive(false);
                }
            });

            // ���� Ȯ���� ���� �Ǿ����� �ʾ��� ���, �ش� �ڷ�ƾ�� delay �ð� �� �ٽ� �����Ѵ�.
            yield return delay;
        }
    }
}
