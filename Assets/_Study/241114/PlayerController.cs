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
        #region 고유 번호로 플레이어의 색상을 변경하는 코드
        // 각 플레이어의 고유 넘버를 설정하여 사용해줄 수도 있다.

        //int number = photonView.Owner.GetPlayerNumber();
        //color = colors[number];
        //material.color = color;
        #endregion

        #region 포톤 네크워크에서 플레이어 생성 당시 저장했던 정보를 통해 색상을 변경하는 코드

        // 포톤 네트워크에서 만들 당시 저장했던 정보를 가져와 사용해줄 수도 있다.
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
        // 소유권이 없는 경우에서도 동작할 수 있다.


        // if문 조건을 통해, 소유권이 자신에게 있는 경우에만 동작할 수 있다.
        if (photonView.Owner.IsLocal == false) return;

        // 스페이스 바를 누를 때, 플레이어의 색상이 랜덤으로 바뀐다.
        if (Input.GetKeyDown(KeyCode.Space)) material.color = Random.ColorHSV();

        Move();
        if (Input.GetMouseButtonDown(1)) // RpcTarget.MasterClient = 해당 함수의 요청을 방장에게 판단을 맡기도록 할 수 있다.
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
        // 방장(서버) 입장에서 판정을 진행
        //if (맞았을 때) photonView.RPC(nameof(Succecss), RpcTarget.All);
        //else photonView.RPC(nameof(Fail), RpcTarget.All);


        // 값을 보낸 시간과 현재의 시간의 차이, 즉 렉이 얼마나 나는지를 확인할 수 있다.
        float lag = Mathf.Abs((float)PhotonNetwork.Time - (float)info.SentServerTime);
        Debug.Log($"지연 시간 : {lag}");

        // 렉이 생기는 만큼 미리 앞선 위치에 도착하도록 하여, 렉으로 인해 발생하는 격차를 줄일 수 있다.
        // 단, 해당 값을 직접 계산하는 것 보단, Pun의 Rigidbody는 자체적으로 지연 보상을 계산해 사용하니 해당 컴포넌트 사용을 지향한다.
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
            // 변수 쓰기
            stream.SendNext(hp);
            stream.SendNext(speed);
            stream.SendNext(percent);

            // 색깔 쓰기
            stream.SendNext(color.r);
            stream.SendNext(color.g);
            stream.SendNext(color.b);
        }
        else if (stream.IsReading)
        {
            // 변수 읽어들이기
            hp = (int)stream.ReceiveNext();
            speed = (float)stream.ReceiveNext();
            percent = (float)stream.ReceiveNext();

            // 색깔 읽어들이기
            color.r = (float)stream.ReceiveNext();
            color.g = (float)stream.ReceiveNext();
            color.b = (float)stream.ReceiveNext();
        }
    }
}
