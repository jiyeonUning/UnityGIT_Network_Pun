using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class BackendManager : MonoBehaviour
{
    public static BackendManager Instance { get; private set; }

    private FirebaseApp app;           // 앱 기능
    private FirebaseAuth auth;         // 인증 기능
    private FirebaseDatabase database; // 저장된 데이터

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
        // 파이어 베이스/포톤 과의 차이점 :
        //                 파이어 베이스 - 함수 실행 후, 아래로 작성된 소스코드를 실행한다.
        //                         포톤 - 함수 실행 후, 괄호에 입력된 함수를 실행한다.

        // CheckAndFixDependenciesAsync = 비동기식(Async)함수. 호환성 체크 후, 버그 발견 시 함수를 통해 수정을 진행한다.
        // ContinueWithOnMainThread = 앞에 붙은 비동기식 함수의 작업이 끝났을 때, 아래 작업을 진행한다.
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            // task.Result = 결과 | Available = 사용 가능
            // 즉, 해당 결과가 사용이 가능하다면,
            if (task.Result == DependencyStatus.Available)
            {
                // ( ) = Firebase( ).DefaultInstance; : 파이어베이스에서 연동을 하기 위한 객체를 각 자료형에게 전달한다.
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                database = FirebaseDatabase.DefaultInstance;

                Debug.Log("Firebase dependencies check success");
            }
            // 사용이 가능하지 않다면,
            else
            {
                // 실패한 이유를 출력하고,
                Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");

                // 각 값의 객체를 비운다.
                app = null;
                auth = null;
                database = null;
            }
        });
    }
}
