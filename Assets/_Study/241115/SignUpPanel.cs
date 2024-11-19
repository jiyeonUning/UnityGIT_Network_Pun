using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class SignUpPanel : MonoBehaviour
{
    // �̿��ڰ� �Է��ϴ� ���� �޾� �����ϴ� InputField
    [SerializeField] TMP_InputField emailInputfield;
    [SerializeField] TMP_InputField passwordInputField;
    [SerializeField] TMP_InputField passwordConfirmInputField;

    public void SignUp()
    {
        // InputField�� �Էµ� �� ���� �̸��� �´� string �ڷ����� ���� �����Ͽ� ����Ѵ�.
        string email = emailInputfield.text;
        string pass = passwordInputField.text;
        string confirm = passwordConfirmInputField.text;

        // �̸��� ���� �Է°��� ������ ���,
        if (email.IsNullOrEmpty()) // or == "";
        {
            // �Է��� ��õ� �ϰ� �Ѵ�.
            Debug.LogWarning("�̸��� �Է� �� �ٽ� �������ּ���.");
            return;
        }
        // �н����� ���� �Է°���, �н����� Ȯ�� ���� �Է� ���� �������� ���� ���,
        if (pass != confirm)
        {
            // �Է��� ��õ� �ϰ� �Ѵ�.
            Debug.LogWarning("�н����尡 ��ġ���� �ʽ��ϴ�.");
            return;
        }

        // �� InputField�� �Էµ� ������(�̸���, �н�����)�� ���� ������ ���� �����Ϳ� ����� �� �ֵ��� ��û�� ���� ��,
        BackendManager.Auth.CreateUserWithEmailAndPasswordAsync(email, pass)
        // �ش� ��û�� �Ϸ�Ǹ� �Ʒ� �ۼ��� �ҽ��ڵ带 �����Ѵ�.
            .ContinueWithOnMainThread(task =>
        {
            //  ( ).IsCanceled = ��û ���
            if (task.IsCanceled)
            {
                // ��û�� ��ҵǾ��ٸ�, �Ѿ��.
                Debug.Log("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            // ( ).IsFaulted = ��û ����
            if (task.IsFaulted)
            {
                // ��û�� �����Ͽ��ٸ�, ���� ������ �ܼ�â�� ��� ��, �Ѿ��.
                Debug.Log($"CreateUserWithEmailAndPasswordAsync encountered an error :" + task.Exception);
                return;
            }


            // �� �� ��츦 �ѱ�� ��û�� �����Ͽ��� ���,

            // �ش� ���� ����� �����ϰ�, 
            AuthResult result = task.Result;
            // �ܼ�â�� �Էµ� �̸��ϰ� ��й�ȣ�� ���� ������ ���������� �����Ǿ��ٴ� �޼����� ����.
            Debug.Log($"Firebase user created successfully : {result.User.DisplayName} ({result.User.UserId})");
            // ȸ�������� �Ϸ�Ǿ����Ƿ�, ȸ������ ȭ���� ��Ȱ��ȭ �Ͽ� �α��� ȭ������ �ǵ��ư���.
            gameObject.SetActive(false);

        });
    }
}
