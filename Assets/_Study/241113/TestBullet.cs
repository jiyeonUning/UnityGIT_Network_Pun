using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBullet : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] float speed = 10f;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();

        rigid.velocity = transform.forward * speed;
        Destroy(gameObject, 5f);
    }
}
