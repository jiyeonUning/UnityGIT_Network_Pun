using Photon.Pun;
using UnityEngine;

// MonoBehaviourPun : ���� �信 �޷� �־�, ��ũ��ũ ������Ʈ�� �̿��� �� �ִ� ����� ��� ����.
// IPunObservable : ���� �䰡 �ڵ����� ������ �� �ְԲ� ���� �� �ִ�.
public class PlayerController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] Color color;
    [SerializeField] Material mater;
    private Color OG;

    void Start() { OG = mater.color; }
    void OnApplicationQuit() { mater.color = OG; }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mater.color = Random.ColorHSV();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            /* stream.SendNext(value1);
               stream.SendNext(value2);
               stream.SendNext(value3); */

            stream.SendNext(mater.color.r);
            stream.SendNext(mater.color.g);
            stream.SendNext(mater.color.b);
        }

        else if (stream.IsReading)
        {
            // IsReading���� IsWriting�Է°����� ����� ������� ���� �о�� �����ϹǷ�,
            // ���ϴ� ���� ���ϴ� ���� �Է��Ϸ���, IsWriting���� �ۼ��� ������� �Է��Ͽ��߸� �Ѵ�.
            /* value1 = (int)stream.ReceiveNext();
               value2 = (float)stream.ReceiveNext();
               value3 = (float)stream.ReceiveNext(); */

            color.r = (float)stream.ReceiveNext();
            color.g = (float)stream.ReceiveNext();
            color.b = (float)stream.ReceiveNext();

            mater.color = color;
        }
    }
}
