using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MonoBehaviourPun : 포톤 뷰에 달려 있어, 네크워크 오브젝트를 이용할 수 있는 기능이 들어 있음.
// IPunObservable : 포톤 뷰가 자동으로 참조할 수 있게끔 해줄 수 있다.
public class PlayerController : MonoBehaviourPun, IPunObservable
{
    public int value1;
    public float value2;
    public float value3;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(value1);
            stream.SendNext(value2);
            stream.SendNext(value3);
        }
        else if (stream.IsReading)
        {
            value1 = (int)stream.ReceiveNext();
            value2 = (float)stream.ReceiveNext();
            value3 = (float)stream.ReceiveNext();
        }
        
    }
}
