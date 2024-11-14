using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] Bullet BulletPrefab;
    [SerializeField] Transform muzzlePoint;
    [SerializeField] Material material;
    private Color color;
    private Color OG;

    [Header("Player")]
    [SerializeField] int hp;
    [SerializeField] float speed;
    [SerializeField] float percent;


    void Awake()
    { OG = material.color; }

    void Start()
    {
        #region ���� ��ȣ�� �÷��̾��� ������ �����ϴ� �ڵ�
        // �� �÷��̾��� ���� �ѹ��� �����Ͽ� ������� ���� �ִ�.

        //int number = photonView.Owner.GetPlayerNumber();
        //color = colors[number];
        //material.color = color;
        #endregion

        #region ���� ��ũ��ũ���� �÷��̾� ���� ��� �����ߴ� ������ ���� ������ �����ϴ� �ڵ�

        // ���� ��Ʈ��ũ���� ���� ��� �����ߴ� ������ ������ ������� ���� �ִ�.
        //object[] data = photonView.InstantiationData;

        //color.r = (float)data[0];
        //color.g = (float)data[1];
        //color.b = (float)data[2];

        //material.color = color;
        #endregion
    }

    void OnApplicationQuit() { material.color = OG; }

    void Update()
    {
        // �������� ���� ��쿡���� ������ �� �ִ�.


        // if�� ������ ����, �������� �ڽſ��� �ִ� ��쿡�� ������ �� �ִ�.
        if (photonView.Owner.IsLocal == false) return;

        // �����̽� �ٸ� ���� ��, �÷��̾��� ������ �������� �ٲ��.
        if (Input.GetKeyDown(KeyCode.Space)) material.color = Random.ColorHSV();

        Move();
        if (Input.GetMouseButtonDown(1)) // RpcTarget.MasterClient = �ش� �Լ��� ��û�� ���忡�� �Ǵ��� �ñ⵵�� �� �� �ִ�.
        photonView.RPC(nameof(Fire), RpcTarget.All, muzzlePoint.position, muzzlePoint.rotation);
    }

    // === === ===

    void Move()
    {
        Vector3 moveDir = Vector3.zero;
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if (moveDir == Vector3.zero) return;

        transform.Translate(moveDir.normalized * speed * Time.deltaTime, Space.World);
        transform.forward = moveDir.normalized;
    }

    [PunRPC]
    void Fire(Vector3 position, Quaternion rotation, PhotonMessageInfo info)
    {
        // ����(����) ���忡�� ������ ����
        //if (�¾��� ��) photonView.RPC(nameof(Succecss), RpcTarget.All);
        //else photonView.RPC(nameof(Fail), RpcTarget.All);


        // ���� ���� �ð��� ������ �ð��� ����, �� ���� �󸶳� �������� Ȯ���� �� �ִ�.
        float lag = Mathf.Abs((float)PhotonNetwork.Time - (float)info.SentServerTime);
        Debug.Log($"���� �ð� : {lag}");

        // ���� ����� ��ŭ �̸� �ռ� ��ġ�� �����ϵ��� �Ͽ�, ������ ���� �߻��ϴ� ������ ���� �� �ִ�.
        // ��, �ش� ���� ���� ����ϴ� �� ����, Pun�� Rigidbody�� ��ü������ ���� ������ ����� ����ϴ� �ش� ������Ʈ ����� �����Ѵ�.
        position += BulletPrefab.Speed * lag * (rotation * Vector3.forward);

        Instantiate(BulletPrefab.gameObject, position, rotation);
    }

    // === === ===

    //[PunRPC]
    //void Succecss()
    //{ }

    //[PunRPC]
    //void Fail()
    //{ }

    // === === ===

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ���� ����
            stream.SendNext(hp);
            stream.SendNext(speed);
            stream.SendNext(percent);

            // ���� ����
            stream.SendNext(color.r);
            stream.SendNext(color.g);
            stream.SendNext(color.b);
        }
        else if (stream.IsReading)
        {
            // ���� �о���̱�
            hp = (int)stream.ReceiveNext();
            speed = (float)stream.ReceiveNext();
            percent = (float)stream.ReceiveNext();

            // ���� �о���̱�
            color.r = (float)stream.ReceiveNext();
            color.g = (float)stream.ReceiveNext();
            color.b = (float)stream.ReceiveNext();
        }
    }
}
