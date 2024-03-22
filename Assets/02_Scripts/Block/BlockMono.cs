using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class BlockMono : PoolableMono
{
    public bool IsTouched { private set; get; } = false;

    private void OnEnable()
    {
        Reset();
    }

    public override void Reset() 
    {
        IsTouched = false;
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
        if(IsTouched) 
            return;

        IsTouched = true;
        Vector3 playerPos = transform.InverseTransformPoint(collision.transform.position);
        Debug.Log(playerPos);

        if(Mathf.Abs(playerPos.x)  > 0.5 || Mathf.Abs(playerPos.z) > 0.5)
        {
            GameObject.Find("Player").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotationY;
        }

        GameObject.Find("Fog").transform.position = transform.position;
        BlockManager.Instance.SapwnNextBlock();
        GameManager.Instance.AddScore();
        GetComponent<AudioPlayer>().SimplePlay("landing");
    }
}
