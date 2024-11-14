using Photon.Pun;
using UnityEngine;

// MonoBehaviourPun : ���� �信 �޷� �־�, ��ũ��ũ ������Ʈ�� �̿��� �� �ִ� ����� ��� ����.
// IPunObservable : ���� �䰡 �ڵ����� ������ �� �ְԲ� ���� �� �ִ�.
public class TestPlayerController : MonoBehaviourPun, IPunObservable
{
    #region ���� ���� ���� �ڵ�
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
        // ��� Ŭ���̾�Ʈ�� �� ����
        #region ���� ���� ���� �ڵ�
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mater.color = Random.ColorHSV();
        }
        #endregion

        // �������ڸ� �� ����
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

            #region ���� �Է��ϱ�
            stream.SendNext(mater.color.r);
            stream.SendNext(mater.color.g);
            stream.SendNext(mater.color.b);
            #endregion
        }
        else if (stream.IsReading)
        {
            // IsReading���� IsWriting�Է°����� ����� ������� ���� �о�� �����ϹǷ�,
            // ���ϴ� ���� ���ϴ� ���� �Է��Ϸ���, IsWriting���� �ۼ��� ������� �Է��Ͽ��߸� �Ѵ�.
            /* value1 = (int)stream.ReceiveNext();
               value2 = (float)stream.ReceiveNext();
               value3 = (float)stream.ReceiveNext(); */

            #region ���� �о���̱�
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
        // ��� ��󿡰� ���� �����ϴ� �Լ��� ����
        photonView.RPC("FireRPC", RpcTarget.All, muzzlePoint.position, muzzlePoint.rotation); 
    }

    [PunRPC]
    void FIreRPC(Vector3 position, Quaternion rotation)
    {
        Debug.Log("�Ѿ� �߻�");
        Instantiate(bulletPrefab, position, rotation);
    }
}
