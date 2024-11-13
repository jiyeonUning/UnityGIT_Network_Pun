using Photon.Pun;
using UnityEngine;

// MonoBehaviourPun : 포톤 뷰에 달려 있어, 네크워크 오브젝트를 이용할 수 있는 기능이 들어 있음.
// IPunObservable : 포톤 뷰가 자동으로 참조할 수 있게끔 해줄 수 있다.
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
            // IsReading에선 IsWriting입력값에서 적용된 순서대로 값을 읽어내려 적용하므로,
            // 원하는 곳에 원하는 값을 입력하려면, IsWriting에서 작성한 순서대로 입력하여야만 한다.
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
