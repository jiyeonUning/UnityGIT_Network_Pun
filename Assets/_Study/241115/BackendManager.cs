using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
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
        /* 한 객체에 필요한 데이터를 직접 지정하는 방식의 예시 01
        JsonTest test = new JsonTest();
        test.name = "test";
        test.level = 30;
        test.rate = 2.3f;
        
        test.userData = new UserData();
        test.userData.phoneNumber = "1234567890";
        test.userData.address = "서울 강동구 경일 게임 아카데미 숍"; 
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


                // 추가로 데이터 베이스에 저장된 특정 값을 가져오기 위해, 가장 최상위 위치에 존재하는 데이터 값을 가져와 저장한다.
                // (= https://fir-study-32514-default-rtdb.asia-southeast1.firebasedatabase.app/ <- 해당 경로의 바로 아래 존재하는 값)
                DatabaseReference root = BackendManager.Database.RootReference;

                // 최상위 데이터 값을 불러오고, 해당 데이터 아래로 저장된 하위 데이터를 타고 내려가, 원하는 값을 이름으로 불러올 수 있다.
                // ex) 유저 '지연'의 레벨 데이터 정보는 = 최상위 데이터의 자식 UserData의 자식 Jiyeon의 자식 level에 저장되어 있다.
                DatabaseReference JiyeonLevel = root.Child("UserData").Child("Jiyoen").Child("level");

                // + (데이터 명).OrderByChild = 해당 데이터 값 아래 저장된 하위 데이터들을 모두 가져올 수 있다.
                //   (데이터 명).Parent = 해당 데이터 값의 상위에 위치한 데이터 값을 가져올 수 있다.


                // ~ 데이터 베이스에 저장된 값을 변경하는 방법 ~
                // 해당 레퍼런스 경로에 저장된 값을 ( )의 값으로 변경해줄 수 있다.
                JiyeonLevel.SetValueAsync(10);
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

    /* 한 객체에 필요한 데이터를 직접 지정하는 방식의 예시 02

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
