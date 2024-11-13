using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MonoBehaviourPun : ���� �信 �޷� �־�, ��ũ��ũ ������Ʈ�� �̿��� �� �ִ� ����� ��� ����.
// IPunObservable : ���� �䰡 �ڵ����� ������ �� �ְԲ� ���� �� �ִ�.
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
