using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
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
}
