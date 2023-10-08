using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float _bulletSpeed;
    [SerializeField] float _lastTime;

    private void Update()
    {
        transform.position += transform.right * Time.deltaTime * _bulletSpeed;
        Destroy(gameObject, _lastTime);
    }

}
