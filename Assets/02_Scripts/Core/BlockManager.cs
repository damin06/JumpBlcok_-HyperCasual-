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


    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(this);

        SpanwBlock(Vector3.zero, BlockType.Normal);
    }

    void Update()
    {
        
    }

    public void SpanwBlock(Vector3 pos = default, BlockType blockType = BlockType.Normal)
    {
        if(m_blocks.Count > m_blockCount)
        {
            while(m_blocks.Count > m_blockCount)
            {
                BlockMono block = m_blocks.Dequeue();
                block.HideBlock();
            }
        }

        string blockName = "NormalBlock";
        Vector3 newPos = pos;
        Vector3 newSize = new Vector3(3.65f, 8, 3.65f);

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

        if(m_currentBlock != null)
        {
            float maxSize = Mathf.Clamp(3.65f - (GameManager.Instance.GetScore() / 100),1f, 3.65f);
            float minSize = Mathf.Clamp(3.65f - (GameManager.Instance.GetScore() / 10),1f, 3.65f);

            float totalSize = Random.Range(minSize, maxSize);
            newSize = new Vector3(totalSize, 8f, totalSize);
        }

        if(newPos == default && m_currentBlock != null)
        {
            Vector3 curPos = m_currentBlock.transform.position;
            if(m_curDir == BlockDir.Right)
            {
                curPos.x += (m_currentBlock.transform.localScale.x / 2) + (newSize.x / 2) + Random.Range(m_minDistance, m_maxDistance);
            }
            else
            {
                curPos.z += (m_currentBlock.transform.localScale.z / 2) + (newSize.z / 2) + Random.Range(m_minDistance, m_maxDistance);
            }
            newPos = curPos;
        }

        m_currentBlock = PoolManager.Instance.Pop(blockName) as BlockMono;

        m_currentBlock.SetBlockPosAndScale(newPos, newSize);

        m_blocks.Enqueue(m_currentBlock);
    }

    public void SapwnNextBlock()
    {
        //m_curDir = (BlockDir)Random.Range(0, 2);
        SpanwBlock();
    }
}
