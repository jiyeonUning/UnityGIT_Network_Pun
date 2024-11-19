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
        string uid = BackendManager.Auth.CurrentUser.UserId;
        userDataRef = BackendManager.Database.RootReference.Child("UserData").Child(uid);
        levelRef = userDataRef.Child("level");

        userDataRef.GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogWarning("�� �������� ��ҵ�");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"�� �������� ������ : {task.Exception.Message}");
                    return;
                }

                DataSnapshot snapshot = task.Result;
                if (snapshot.Value == null)
                {
                    UserData userData = new UserData();
                    userData.name = BackendManager.Auth.CurrentUser.DisplayName;
                    userData.email = BackendManager.Auth.CurrentUser.Email;
                    userData.level = 1;

                    userData.list.Add("ù��°");
                    userData.list.Add("�ι�°");
                    userData.list.Add("����°");

                    string json = JsonUtility.ToJson(userData);
                    userDataRef.SetRawJsonValueAsync(json);
                }
                else
                {
                    level = int.Parse(snapshot.Child("level").Value.ToString());
                    Debug.Log(level);
                }
            });

        levelRef.ValueChanged += LevelRef_ValueChanged;
    }

    private void OnDisable()
    {
        levelRef.ValueChanged -= LevelRef_ValueChanged;
    }

    void LevelRef_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        Debug.Log($"�� ���� �̺�Ʈ Ȯ�� : {e.Snapshot.Value.ToString()}");
        level = int.Parse(e.Snapshot.Value.ToString());
    }

    // === === ===

    public void LevelUp()
    {
        level++;
        levelRef.SetValueAsync(level);
    }

    public void LevelDown()
    {
        level--;
        levelRef.SetValueAsync(level);
    }
}

[Serializable]
public class UserData
{
    public string name;
    public string email;
    public int level;

    public List<string> list = new List<string>();
}
