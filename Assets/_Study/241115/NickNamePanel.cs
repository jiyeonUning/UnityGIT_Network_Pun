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

        // �г��� ������ �Ǿ����� ���� ä�� Confirm�� ������ ���, �г����� ������ �� �ֵ��� ��õ��Ѵ�.
        if (nickName == "") return;

        // ���� �������� �����ϰ�,
        UserProfile profile = new UserProfile();
        // �ش� ������ ���� ���� �г�����, InputField�� �ۼ��� �ؽ�Ʈ ������ �����Ѵ�.
        profile.DisplayName = nickName;


        // ���� ������ �ִ� ������ ��������, ������ ������ ������ profile�� ���������� �����ϰ�,
        BackendManager.Auth.CurrentUser.UpdateUserProfileAsync(profile)
            // ContinueWithOnMainThread �Ʒ��� �ۼ��� �ڵ带 �����Ѵ�.
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    // ��û�� ��ҵǾ��� ���, �Ѿ��.
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    // ��û�� �����Ͽ��� ���, ���� ������ �ܼ� â�� ��� ��, �Ѿ��.
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                // �� ������ �Ѱ��� ���, �������� ���������� ���� �Ǿ��ٴ� �ܼ�â�� ����.
                Debug.Log("User profile updated successfully.");

                // ���������� ������ �������� ������ �ܼ�â�� ����Ѵ�.
                Debug.Log($"Display Name : {user.DisplayName}");
                Debug.Log($"Email : {user.Email}");
                Debug.Log($"Email verified : {user.IsEmailVerified}");
                Debug.Log($"User ID : {user.UserId}");

                // �� �г��� ���� ���� ���濡 �����ϱ� ����, ���濡 ����� �÷��̾� �г����� -> ������ �г������� �����Ѵ�.
                PhotonNetwork.LocalPlayer.NickName = nickName;
                // ��Ʈ��ũ�� ������ �õ��Ѵ�.
                PhotonNetwork.ConnectUsingSettings();

                // �г����� ���������� �����Ǿ����Ƿ�, �г��� ȭ���� ��Ȱ��ȭ �Ѵ�.
                gameObject.SetActive(false);
            });
    }
}
