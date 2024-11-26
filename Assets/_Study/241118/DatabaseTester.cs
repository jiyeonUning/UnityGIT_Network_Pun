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
        // ���� ������ �̸��Ϸ� �α��� �� ������ ���̵� ������ ����
        string uid = BackendManager.Auth.CurrentUser.UserId;

        // ������ ������ ���� �������, UserData ������ ��ġ�� ���� ���� ������ ��θ� ����
        userDataRef = BackendManager.Database.RootReference.Child("UserData").Child(uid);

        // ������ ������ ��� �Ʒ��� level �̸��� ���� ���� ������
        levelRef = userDataRef.Child("level");


        // ~ ������ �б� : GetValueAsync ~
        // �о���̴� �������� ������ ��ġ�ϴ� ��� ������ ������ �� ���� �ҷ��� �� �ִ�.
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

                // ����� �����̾��� ���, �ش� ���� datasnapshot�� �����Ͽ� ������� �� �ִ�.
                DataSnapshot snapshot = task.Result;

                // ���� �ش� ������ �Ʒ��� �������� ������ ���,
                if (snapshot.Value == null)
                {
                    // �ʱⰪ���� ������ ������ �����Ѵ�.
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
                // �ƴ� ���,
                else
                {
                    // ������ ������ ������ �о���� �� �ִ� string �ڷ���
                    string json = snapshot.GetRawJsonValue();
                    // FromJson�� ���� ���� ������ userData�� ��������� ���� �ִ�.
                    UserData userData = JsonUtility.FromJson<UserData>(json);

                    // ���� �÷��̾��� ���� ��������, �ش� ��� ������ ���� ���� ������ ��ο��� ������ ����Ѵ�.
                    level = int.Parse(snapshot.Child("level").Value.ToString());
                    Debug.Log(level);


                    // ����� �������� ���� �����Ͱ��� �ѹ��� ������ �� �ִ� foreach��
                    foreach (DataSnapshot child in snapshot.Child("list").Children)
                    {
                        Debug.Log($"{child.Key} : {child.Value}");
                    }
                }
            });

        // level �����Ͱ��� ������ �־��� ��, Ư�� �̺�Ʈ�� �����Ͽ� ������� �� �ִ�.
        levelRef.ValueChanged += LevelRef_ValueChanged;
    }

    private void OnDisable()
    {
        // ������Ʈ�� ��Ȱ��ȭ �Ǿ��� ��, Ư�� �̺�Ʈ ������ �����Ѵ�.
        levelRef.ValueChanged -= LevelRef_ValueChanged;
    }

    void LevelRef_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        Debug.Log($"�� ���� �̺�Ʈ Ȯ�� : {e.Snapshot.Value.ToString()}");

        // �̺�Ʈ ����� ����, level���� �ٲ� ����� -> level ���� ������ ���� ������ �����Ų��.
        level = int.Parse(e.Snapshot.Value.ToString());
    }

    // === === ===

    public void LevelUp()
    {
        /* ���� : OrderByValue
         * Ư�� �����͸� ��������, �ش� �������� ���� �����͸� Value(=������)���� ��� ���Ľ��� �ҷ����� ���
         
        userDataRef.Child("list").OrderByValue().GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled) return;
            if (task.IsFaulted) return;

            // �ҷ����� ���� �����Ͽ��� ���, �ش� �ҷ��� ���� �����Ѵ�.
            DataSnapshot snapshot = task.Result;

            // ������ �����ص� ���� �������, �����Ͱ��� ������� �ܼ�â�� ȣ���Ѵ�.
            foreach (DataSnapshot child in snapshot.Children)
            {
                Debug.Log($"{child.Key} : {child.Value}");
                // ��� : Value�� ��ϵ� ���� ù���� ������ ��������, ������ ������ ���ĵǾ� ���� �ҷ��Դ�.
            }
        }); */

        /* ���� : OrderByChild
         * UserData�� ��������, �ش� �� �Ʒ��� ��ϵ� ������ �������� Ư�� �������� ���� ���� ��� ���Ľ��� �ҷ����� ���
         
        BackendManager.Database.RootReference.Child("UserData")
            .OrderByChild("level")
            // �߰��� �Ʒ��� ��Ÿ ���� �Լ��� ���̴� ������ ���͸��� ��ĥ �� �ִ�.
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
              if (task.IsCanceled) return;
              if (task.IsFaulted) return;
              DataSnapshot snapshot = task.Result;

              foreach (DataSnapshot child in snapshot.Children)
              {
                  Debug.Log($"{child.Key} : {child.Child("level").Value}");
                  // ��� : Value�� ��ϵ� level�� ���� ������� �ܼ�â�� ȣ�� �Ͽ���.
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
        // �⺻ �ڷ����� ���� ���� ���� ���
        // ~.Child(�����͸�).SetValueAsync(������(=�����Ұ�));
        userDataRef.Child("string").SetValueAsync("�ؽ�Ʈ");
        userDataRef.Child("number").SetValueAsync(10);
        userDataRef.Child("double").SetValueAsync(3.14);
        userDataRef.Child("bool").SetValueAsync(true);
    }

    public void SaveList()
    {
        // List �ڷᱸ���� ���� ���� �������� ���� ���
        List<string> list = new List<string>() { "ù��°", "�ι�°", "����°" };
        userDataRef.Child("List").SetValueAsync(list);
    }

    public void SaveDic()
    {
        // Dictionary �ڷᱸ���� ���� Ű&�� ���� ���
        // Dictionary<�����͸�, ������(=�����Ұ�)> ~
        Dictionary<string, object> dictionary = new Dictionary<string, object>()

        {   // ������ ��(=Ű) �Է�, �� �Է����� ���� �־��� �� �ִ�.
            { "stringKey", "�ؽ�Ʈ" },
            { "longKey", 10 },
            { "doubleKey", 3.14 },
            { "boolKey", true }
        };

        userDataRef.Child("Dictionary").SetValueAsync(dictionary);
    }

    public void SaveSetAll()
    {
        // ������ ��� ���� : SetRawJsonValueAsync()
        // ���ÿ� ���� �ڷ����� �� ���� ����ϰ� ���� �� ����ϴ� ���

        // ����&�߰��� ���� �����ص� Ŭ������ ������ ���� ��,
        UserData userData = new UserData();

        //�ش� ���� ������ ���ϴ� �������� �Է����ش�.
        userData.name = "������";
        userData.email = "yeon14735@gmail.com";
        userData.level = 100;

        // �Ŀ�, �ش� ���� ���¸� Json������ ������ �����Ѵ�.
        string json = JsonUtility.ToJson(userData);

        // Json���� ����� userData���� �Ѳ����� �Ű��ش�.
        userDataRef.SetRawJsonValueAsync(json);
    }

    // === === ===
}


[Serializable] // = ����ȭ�� ���� Serializable
public class UserData
{
    public string name;
    public string email;
    public int level;

    public List<string> list = new List<string>();
}
