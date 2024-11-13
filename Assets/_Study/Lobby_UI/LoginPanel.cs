using Photon.Pun;
using TMPro;
using UnityEngine;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField idInputField;

    private void Start()
    {
        idInputField.text = $"Player {Random.Range(1000, 10000)}";
    }

    public void Login()
    {
        if (idInputField.text == "") return;

        // �������� ��û�� ��, �׻� PhotonNetwork�� �� �� �ִ�.
        PhotonNetwork.LocalPlayer.NickName = idInputField.text; // �÷��̾��� �г��� ����
        PhotonNetwork.ConnectUsingSettings();                   // ���� ���������� �ش� �������� ������ ��û
    }
}
