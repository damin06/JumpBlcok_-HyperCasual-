using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlockMono : PoolableMono
{
    private void OnEnable()
    {
        Reset();
    }

    public override void Reset() 
    {
        Vector3 pos = transform.position;
        pos.y = -5;
        transform.position = pos;
    }

    public void SetBlockPos(Vector3 pos)
    {
        transform.position = pos;
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
        transform.DOMoveY(-5, 1.2f).SetEase(Ease.InCubic).onComplete = GotoPool;
    }

    public void GotoPool()
    {
        PoolManager.Instance.Push(this);
    }
}
