using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseTester : MonoBehaviour
{
    [SerializeField] int level;

    DatabaseReference userDataRef;
    DatabaseReference levelRef;


    private void OnEnable()
    {
        // 현재 인증된 이메일로 로그인 된 유저의 아이디 정보를 저장
        string uid = BackendManager.Auth.CurrentUser.UserId;

        // 위에서 저장한 값을 기반으로, UserData 하위에 위치한 현재 유저 데이터 경로를 저장
        userDataRef = BackendManager.Database.RootReference.Child("UserData").Child(uid);

        // 위에서 저장한 경로 아래에 level 이름을 가진 값을 저장함
        levelRef = userDataRef.Child("level");


        // ~ 데이터 읽기 : GetValueAsync ~
        // 읽어들이는 데이터의 하위에 위치하는 모든 데이터 정보를 한 번에 불러올 수 있다.
        userDataRef.GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogWarning("값 가져오기 취소됨");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"값 가져오기 실패함 : {task.Exception.Message}");
                    return;
                }

                // 결과가 성공이었을 경우, 해당 값을 datasnapshot에 저장하여 사용해줄 수 있다.
                DataSnapshot snapshot = task.Result;

                // 만약 해당 데이터 아래에 정보값이 없었을 경우,
                if (snapshot.Value == null)
                {
                    // 초기값으로 데이터 구성을 세팅한다.
                    UserData userData = new UserData();
                    userData.name = BackendManager.Auth.CurrentUser.DisplayName;
                    userData.email = BackendManager.Auth.CurrentUser.Email;
                    userData.level = 1;

                    userData.list.Add("첫번째");
                    userData.list.Add("두번째");
                    userData.list.Add("세번째");

                    string json = JsonUtility.ToJson(userData);
                    userDataRef.SetRawJsonValueAsync(json);
                }
                // 아닐 경우,
                else
                {
                    // 저장한 데이터 정보를 읽어들일 수 있는 string 자료형
                    string json = snapshot.GetRawJsonValue();
                    // FromJson을 통해 값을 가져와 userData에 적용시켜줄 수도 있다.
                    UserData userData = JsonUtility.FromJson<UserData>(json);

                    // 현재 플레이어의 레벨 정보값을, 해당 결과 데이터 값의 하위 데이터 경로에서 가져와 사용한다.
                    level = int.Parse(snapshot.Child("level").Value.ToString());
                    Debug.Log(level);


                    // 저장된 데이터의 하위 데이터값을 한번에 가져올 수 있는 foreach문
                    foreach (DataSnapshot child in snapshot.Child("list").Children)
                    {
                        Debug.Log($"{child.Key} : {child.Value}");
                    }
                }
            });

        // level 데이터값에 변동이 있었을 때, 특정 이벤트를 구독하여 사용해줄 수 있다.
        levelRef.ValueChanged += LevelRef_ValueChanged;
    }

    private void OnDisable()
    {
        // 오브젝트가 비활성화 되었을 때, 특정 이벤트 구독을 해제한다.
        levelRef.ValueChanged -= LevelRef_ValueChanged;
    }

    void LevelRef_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        Debug.Log($"값 변경 이벤트 확인 : {e.Snapshot.Value.ToString()}");

        // 이벤트 사용을 통해, level값이 바뀐 결과를 -> level 원본 데이터 값에 실제로 적용시킨다.
        level = int.Parse(e.Snapshot.Value.ToString());
    }

    // === === ===

    public void LevelUp()
    {
        /* 정렬 : OrderByValue
         * 특정 데이터를 기준으로, 해당 데이터의 하위 데이터를 Value(=정보값)기준 삼아 정렬시켜 불러오는 방식
         
        userDataRef.Child("list").OrderByValue().GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled) return;
            if (task.IsFaulted) return;

            // 불러오는 데에 성공하였을 경우, 해당 불러온 값을 저장한다.
            DataSnapshot snapshot = task.Result;

            // 위에서 저장해둔 값을 기반으로, 데이터값을 순서대로 콘솔창에 호출한다.
            foreach (DataSnapshot child in snapshot.Children)
            {
                Debug.Log($"{child.Key} : {child.Value}");
                // 결과 : Value에 등록된 값의 첫글자 자음을 기준으로, 가나다 순으로 정렬되어 값을 불러왔다.
            }
        }); */

        /* 정렬 : OrderByChild
         * UserData를 기준으로, 해당 값 아래로 등록된 데이터 내에서도 특정 데이터의 값을 기준 삼아 정렬시켜 불러오는 방식
         
        BackendManager.Database.RootReference.Child("UserData")
            .OrderByChild("level")
            // 추가로 아래에 기타 조건 함수를 붙이는 것으로 필터링을 거칠 수 있다.
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
              if (task.IsCanceled) return;
              if (task.IsFaulted) return;
              DataSnapshot snapshot = task.Result;

              foreach (DataSnapshot child in snapshot.Children)
              {
                  Debug.Log($"{child.Key} : {child.Child("level").Value}");
                  // 결과 : Value에 등록된 level을 작은 순서대로 콘솔창에 호출 하였다.
              }
            }); */

        levelRef.SetValueAsync(level++);
    }

    public void LevelDown()
    {
        levelRef.SetValueAsync(level--);
    }

    // === === ===

    public void SaveDefult()
    {
        // 기본 자료형을 통한 값의 저장 방식
        // ~.Child(데이터명).SetValueAsync(실제값(=적용할값));
        userDataRef.Child("string").SetValueAsync("텍스트");
        userDataRef.Child("number").SetValueAsync(10);
        userDataRef.Child("double").SetValueAsync(3.14);
        userDataRef.Child("bool").SetValueAsync(true);
    }

    public void SaveList()
    {
        // List 자료구조를 통한 값의 순차적인 저장 방식
        List<string> list = new List<string>() { "첫번째", "두번째", "세번째" };
        userDataRef.Child("List").SetValueAsync(list);
    }

    public void SaveDic()
    {
        // Dictionary 자료구조를 통한 키&값 저장 방식
        // Dictionary<데이터명, 실제값(=적용할값)> ~
        Dictionary<string, object> dictionary = new Dictionary<string, object>()

        {   // 데이터 명(=키) 입력, 값 입력으로 값을 넣어줄 수 있다.
            { "stringKey", "텍스트" },
            { "longKey", 10 },
            { "doubleKey", 3.14 },
            { "boolKey", true }
        };

        userDataRef.Child("Dictionary").SetValueAsync(dictionary);
    }

    public void SaveSetAll()
    {
        // 데이터 모두 쓰기 : SetRawJsonValueAsync()
        // 동시에 여러 자료형을 한 번에 사용하고 싶을 때 사용하는 방식

        // 변경&추가할 값을 저장해둔 클래스를 가져와 선언 후,
        UserData userData = new UserData();

        //해당 값을 가져와 원하는 정보값을 입력해준다.
        userData.name = "김전사";
        userData.email = "yeon14735@gmail.com";
        userData.level = 100;

        // 후에, 해당 값의 형태를 Json으로의 변경을 진행한다.
        string json = JsonUtility.ToJson(userData);

        // Json으로 변경된 userData값을 한꺼번에 옮겨준다.
        userDataRef.SetRawJsonValueAsync(json);
    }

    // === === ===
}


[Serializable] // = 직렬화를 위한 Serializable
public class UserData
{
    public string name;
    public string email;
    public int level;

    public List<string> list = new List<string>();
}
