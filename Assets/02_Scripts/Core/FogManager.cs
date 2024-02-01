using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ParticlePool particle = PoolManager.Instance.Pop("SmokeParticle") as ParticlePool;
        Vector3 newPos = other.transform.position;
        newPos.y = 0;
        particle.transform.position = newPos;
    }
}
