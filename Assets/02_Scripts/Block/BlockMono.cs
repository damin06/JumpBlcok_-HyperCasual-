using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class BlockMono : PoolableMono
{
    private bool m_IsTouched = false;

    private void OnEnable()
    {
        Reset();
    }

    public override void Reset() 
    {
        m_IsTouched = false;
    }

    public void SetBlockPosAndScale(Vector3 pos, Vector3 scale)
    {
        pos.y = -5;
        transform.position = pos;
        transform.localScale = scale;
        Showblock();
    }

    [ContextMenu("ShowBlock")]
    public void Showblock()
    {
        transform.DOMoveY(0, 1.2f).SetEase(Ease.OutCubic);
    }

    [ContextMenu("HideBlock")]
    public void HideBlock()
    {
        transform.DOMoveY(-16, 1.2f).SetEase(Ease.InCubic).onComplete = GotoPool;
    }

    public void GotoPool()
    {
        PoolManager.Instance.Push(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(m_IsTouched) 
            return;

        Debug.Log(transform.InverseTransformPoint(collision.transform.position));

        BlockManager.Instance.SapwnNextBlock();

    }
}
