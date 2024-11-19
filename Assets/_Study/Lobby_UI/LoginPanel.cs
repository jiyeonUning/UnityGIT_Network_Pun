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
        #region ���̾� ���̽� ���� ���� �α��� ���
        //if (idInputField.text == "") return;

        // �������� ��û�� ��, �׻� PhotonNetwork�� �� �� �ִ�.
        //PhotonNetwork.LocalPlayer.NickName = idInputField.text; // �÷��̾��� �г��� ����
        //PhotonNetwork.ConnectUsingSettings();                   // ���� ���������� �ش� �������� ������ ��û
        #endregion

        string email = emailInputField.text;
        string password = passwordlInputField.text;

        // ( ).CreateUserWithEmailAndPasswordAsync = �̸��ϰ� �н����带 ����, ()�� ������ �����Ѵ�.
        BackendManager.Auth.SignInWithEmailAndPasswordAsync(email, password)
    .ContinueWithOnMainThread(task =>
    {
        if (task.IsCanceled)
        {
            // ��û�� ���, �� �α����� �߰��� ��ҵǾ��ٸ�, �Ѿ��.
            Debug.Log("CreateUserWithEmailAndPasswordAsync was canceled.");
            return;
        }
        if (task.IsFaulted)
        {
            // ��û�� ����, �� �α����� �����ߴٸ�, ���� ������ �ܼ� â�� ��� ��, �Ѿ��.
            Debug.Log($"CreateUserWithEmailAndPasswordAsync encountered an error :" + task.Exception);
            return;
        }

        // �� �� ��츦 �ѱ�� ��û�� �����Ͽ��� ���,

        // �ش� ���� ����� �����ϰ�, 
        AuthResult result = task.Result;
        // ��� ���� �г��Ӱ� ���� ���̵� ���� �÷��̾ �α��� �Ǿ����� �ܼ�â�� ����.
        Debug.Log($"Firebase user created successfully : {result.User.DisplayName} ({result.User.UserId})");
        // �α��� ������, �÷��̾��� ������ �ҷ����� �Լ��� �����Ѵ�.
        CheckUserInfo();
        // �α����� �Ϸ�Ǿ����Ƿ�, �α��� ȭ���� ��Ȱ��ȭ �Ͽ� �޴� ȭ������ �Ѿ��.
        //gameObject.SetActive(false);

    });
    }



    void CheckUserInfo()
    {
        // ���� �α����� �Ϸ�� ���� ���ӵ� �����(=Currentuser)�� ������ ������ �����ϰ�,
        FirebaseUser user = BackendManager.Auth.CurrentUser;

        // �ش� ���� null���� �ƴ����� ����, �÷��̾��� �α��� ������ �Ǵ��Ѵ�.
        // user�� null�� ���, ��, �α����� �Ǿ����� ���� ���, �ش� �Լ��� �������� �ʴ´�.
        if (user == null) return;


        // �̸��� ������ ���� �Ǿ����� ���� ��,
        if (user.IsEmailVerified == false)
        {
            // �̸��� ����â�� ���, �̸��� ������ �����Ѵ�.
            VerfiPanel.gameObject.SetActive(true);
        }

        // �Ǵ� ������ �г����� ���� �����Ǿ� ���� �ʾ��� ���,
        else if (user.DisplayName == "")
        {
             // �г��� ���� â�� ���, �г��� ������ �����Ѵ�.
             NickNamePanel.gameObject.SetActive(true);
        }

        // �� ��� ������ �̹� ���� or ������ ������ ���,
        else
        {
            // ���� ���ӵ� ������� �г�����
            // �� ������� �̸����� ���� ���̾� ���̽����� �����͸� �˻��Ͽ�, �ش��ϴ� ��ҿ� ����Ǿ� �ִ� �г������� �����Ѵ�.
            PhotonNetwork.LocalPlayer.NickName = user.DisplayName;
            // ��� ������ ������ �Ǿ����Ƿ�, ��Ʈ��ũ�� ������ �õ��Ѵ�.
            PhotonNetwork.ConnectUsingSettings();
        }
    }    

}
