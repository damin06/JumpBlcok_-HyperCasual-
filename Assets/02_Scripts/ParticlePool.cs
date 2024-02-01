using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : PoolableMono
{
    public override void Reset()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Play();
    }

    private void OnParticleSystemStopped()
    {
        Debug.Log("stop");
        PoolManager.Instance.Push(this);
    }
}
