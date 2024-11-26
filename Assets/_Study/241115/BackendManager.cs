using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using UnityEngine;

public class BackendManager : MonoBehaviour
{
    public static BackendManager Instance { get; private set; }

    private FirebaseApp app;           // �� ���
    private FirebaseAuth auth;         // ���� ���
    private FirebaseDatabase database; // ����� ������

    public static FirebaseApp App { get { return Instance.app; } }
    public static FirebaseAuth Auth { get { return Instance.auth; } }
    public static FirebaseDatabase Database { get { return Instance.database; } }



    private void Awake()
    {
        /* �� ��ü�� �ʿ��� �����͸� ���� �����ϴ� ����� ���� 01
        JsonTest test = new JsonTest();
        test.name = "test";
        test.level = 30;
        test.rate = 2.3f;
        
        test.userData = new UserData();
        test.userData.phoneNumber = "1234567890";
        test.userData.address = "���� ������ ���� ���� ��ī���� ��"; 
        */

        SetSingleton();
    }

    private void Start()
    {
        CheckDependency();
    }

    // === === === ===

    void SetSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CheckDependency()
    {
        // ���̾� ���̽�/���� ���� ������ :
        //                 ���̾� ���̽� - �Լ� ���� ��, �Ʒ��� �ۼ��� �ҽ��ڵ带 �����Ѵ�.
        //                         ���� - �Լ� ���� ��, ��ȣ�� �Էµ� �Լ��� �����Ѵ�.

        // CheckAndFixDependenciesAsync = �񵿱��(Async)�Լ�. ȣȯ�� üũ ��, ���� �߰� �� �Լ��� ���� ������ �����Ѵ�.
        // ContinueWithOnMainThread = �տ� ���� �񵿱�� �Լ��� �۾��� ������ ��, �Ʒ� �۾��� �����Ѵ�.
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            // task.Result = ��� | Available = ��� ����
            // ��, �ش� ����� ����� �����ϴٸ�,
            if (task.Result == DependencyStatus.Available)
            {
                // ( ) = Firebase( ).DefaultInstance; : ���̾�̽����� ������ �ϱ� ���� ��ü�� �� �ڷ������� �����Ѵ�.
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                database = FirebaseDatabase.DefaultInstance;

                Debug.Log("Firebase dependencies check success");


                // �߰��� ������ ���̽��� ����� Ư�� ���� �������� ����, ���� �ֻ��� ��ġ�� �����ϴ� ������ ���� ������ �����Ѵ�.
                // (= https://fir-study-32514-default-rtdb.asia-southeast1.firebasedatabase.app/ <- �ش� ����� �ٷ� �Ʒ� �����ϴ� ��)
                DatabaseReference root = BackendManager.Database.RootReference;

                // �ֻ��� ������ ���� �ҷ�����, �ش� ������ �Ʒ��� ����� ���� �����͸� Ÿ�� ������, ���ϴ� ���� �̸����� �ҷ��� �� �ִ�.
                // ex) ���� '����'�� ���� ������ ������ = �ֻ��� �������� �ڽ� UserData�� �ڽ� Jiyeon�� �ڽ� level�� ����Ǿ� �ִ�.
                DatabaseReference JiyeonLevel = root.Child("UserData").Child("Jiyoen").Child("level");

                // + (������ ��).OrderByChild = �ش� ������ �� �Ʒ� ����� ���� �����͵��� ��� ������ �� �ִ�.
                //   (������ ��).Parent = �ش� ������ ���� ������ ��ġ�� ������ ���� ������ �� �ִ�.


                // ~ ������ ���̽��� ����� ���� �����ϴ� ��� ~
                // �ش� ���۷��� ��ο� ����� ���� ( )�� ������ �������� �� �ִ�.
                JiyeonLevel.SetValueAsync(10);
            }
            // ����� �������� �ʴٸ�,
            else
            {
                // ������ ������ ����ϰ�,
                Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");

                // �� ���� ��ü�� ����.
                app = null;
                auth = null;
                database = null;
            }
        });
    }

    /* �� ��ü�� �ʿ��� �����͸� ���� �����ϴ� ����� ���� 02

    [Serializable]
    public class JsonTest
    {
        public string mane;
        public int level;
        public float rate;

        public UserData userData;
    }

    [Serializable]
    public class UserData
    {
        public string phoneNumber;
        public string address;
    }
    */
}
