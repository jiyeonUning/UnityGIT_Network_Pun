using Photon.Pun;
using UnityEngine;

// MonoBehaviourPun : 포톤 뷰에 달려 있어, 네크워크 오브젝트를 이용할 수 있는 기능이 들어 있음.
// IPunObservable : 포톤 뷰가 자동으로 참조할 수 있게끔 해줄 수 있다.
public class TestPlayerController : MonoBehaviourPun, IPunObservable
{
    #region 색깔 변경 예제 코드
    [SerializeField] Material mater;
    private Color color;
    private Color OG;
    #endregion

    [SerializeField] float Speed = 5f;
    [SerializeField] Transform muzzlePoint;
    [SerializeField] GameObject bulletPrefab;


    void Start() { OG = mater.color; }
    void OnApplicationQuit() { mater.color = OG; }

    void Update()
    {
        // 모든 클라이언트가 할 내용
        #region 색깔 변경 예제 코드
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mater.color = Random.ColorHSV();
        }
        #endregion

        // 소유권자만 할 내용
        if (photonView.IsMine == false) return;
        Move();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            /* stream.SendNext(value1);
               stream.SendNext(value2);
               stream.SendNext(value3); */

            #region 색깔 입력하기
            stream.SendNext(mater.color.r);
            stream.SendNext(mater.color.g);
            stream.SendNext(mater.color.b);
            #endregion
        }
        else if (stream.IsReading)
        {
            // IsReading에선 IsWriting입력값에서 적용된 순서대로 값을 읽어내려 적용하므로,
            // 원하는 곳에 원하는 값을 입력하려면, IsWriting에서 작성한 순서대로 입력하여야만 한다.
            /* value1 = (int)stream.ReceiveNext();
               value2 = (float)stream.ReceiveNext();
               value3 = (float)stream.ReceiveNext(); */

            #region 색깔 읽어들이기
            color.r = (float)stream.ReceiveNext();
            color.g = (float)stream.ReceiveNext();
            color.b = (float)stream.ReceiveNext();

            mater.color = color;
            #endregion
        }
    }

    // === === ===

    void Move()
    {
        Vector3 moveDir = new(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if (moveDir == Vector3.zero) return;

        transform.Translate(moveDir.normalized * Speed * Time.deltaTime, Space.World);
        transform.forward = moveDir.normalized;
    }

    void Fire()
    {
        // 모든 대상에게 총을 발포하는 함수를 싱행
        photonView.RPC("FireRPC", RpcTarget.All, muzzlePoint.position, muzzlePoint.rotation); 
    }

    [PunRPC]
    void FIreRPC(Vector3 position, Quaternion rotation)
    {
        Debug.Log("총알 발사");
        Instantiate(bulletPrefab, position, rotation);
    }
}
