using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BlockDir { Right = 0, Left = 1 };
public enum BlockType { Normal, Edge, MoveVerticle, MoveHorizontal };

public class BlockManager : MonoBehaviour
{
    public static BlockManager Instance;

    public BlockDir m_curDir { get; private set; } = BlockDir.Right;
    private Queue<BlockMono> m_blocks = new Queue<BlockMono>();
    private BlockMono m_currentBlock;


    [Header("Setting")]
    [Tooltip("Distance between blocks")]
    [SerializeField] private float m_minDistance = 1f;
    [SerializeField] private float m_maxDistance = 7f;

    [Tooltip("Maximum number of blocks to spawn")]
    [SerializeField] private int m_blockCount = 3;

    [Space]

    [Header("Reference")]
    [SerializeField] private Transform m_cam;


    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(this);

        SpanwBlockWithPos(Vector3.zero, BlockType.Normal);
    }

    private void SpanwBlock(Vector3 pos = default, BlockType blockType = BlockType.Normal)
    {
        if(m_blocks.Count > m_blockCount)
        {
                BlockMono block = m_blocks.Dequeue();
                block.HideBlock();
        }

        string blockName = "NormalBlock";
        Vector3 newPos = pos;
        Vector3 newSize = new Vector3(3.65f, 16, 3.65f);
        //Vector3 curPos = Vector3.zero;

        switch (blockType)
        {
            case BlockType.Normal:
                blockName = "NormalBlock";
                break;
            case BlockType.Edge:
                break;
            case BlockType.MoveVerticle:
                break;
            case BlockType.MoveHorizontal:
                break;
        }


#region Size
        if (m_currentBlock != null)
        {
            float maxSize = Mathf.Clamp(3.65f - (GameManager.Instance.GetScore() / 120),1f, 3.65f);
            float minSize = Mathf.Clamp(3.65f - (GameManager.Instance.GetScore() / 70),1f, 3.65f);

            float totalSize = Random.Range(minSize, maxSize);
            newSize = new Vector3(totalSize, 16f, totalSize);
        }
#endregion


 #region Position
        if (newPos == default && m_currentBlock != null)
        {
            Vector3 curPos = m_currentBlock.transform.position;
            if(m_curDir == BlockDir.Right)
            {
                curPos.x += (m_currentBlock.transform.localScale.x / 2) + (newSize.x / 2) + Random.Range(m_minDistance, m_maxDistance);
                curPos.z = GameObject.Find("Player").GetComponent<AgentController>().transform.position.z;
            }
            else
            {
                curPos.z += (m_currentBlock.transform.localScale.z / 2) + (newSize.z / 2) + Random.Range(m_minDistance, m_maxDistance);
                curPos.x = GameObject.Find("Player").GetComponent<AgentController>().transform.position.x;
            }
            newPos = curPos;
        }
#endregion

#region Camera

        if (m_currentBlock != null)
        {
            Vector3 curPos = m_currentBlock.transform.position;
            Vector3 campPos = Vector3.Lerp(curPos, newPos, 0.5f);
            campPos.y = 6;
            m_cam.transform.position = campPos;

            if (m_curDir == BlockDir.Right)
                PlayerPrefs.SetFloat("distance", newPos.x - curPos.x); //Dev Mode
            else
                PlayerPrefs.SetFloat("distance", newPos.z - curPos.z);
        }
        else
        {
            m_cam.transform.position = new Vector3(newPos.x, 6, newPos.z);

            if (m_curDir == BlockDir.Right)
                PlayerPrefs.SetFloat("distance", newPos.x);
            else
                PlayerPrefs.SetFloat("distance", newPos.z);
        }

#endregion

        m_currentBlock = PoolManager.Instance.Pop(blockName) as BlockMono;

        m_currentBlock.SetBlockPosAndScale(newPos, newSize);

        m_blocks.Enqueue(m_currentBlock);
    }

    public void SapwnNextBlock(BlockType blockType = BlockType.Normal)
    {
        m_curDir = (BlockDir)Random.Range(0, 2);
        SpanwBlock();
    }

    public void SpanwBlockWithPos(Vector3 pos, BlockType blockType = BlockType.Normal)
    {
        SpanwBlock(pos, blockType);
    }

    public void ClearBlock()
    {
        while(m_blocks.Count > 0)
        {
            BlockMono block = m_blocks.Dequeue();
            block.HideBlock();
        }

        m_curDir = BlockDir.Right;
        m_currentBlock = null;
    }
}
