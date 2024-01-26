using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockDir { Right = -1, Left = 1 };
public enum BlockType { Normal, Edge, MoveVerticle, MoveHorizontal };

public class BlockManager : MonoBehaviour
{
    public static BlockManager Instance;

    public BlockDir m_curDir { get; private set; } = BlockDir.Right;
    private Queue<BlockMono> m_blocks = new Queue<BlockMono>();


    [Header("Setting")]
    [Tooltip("Distance between blocks")]
    [SerializeField] private float m_minDistance = 6f;

    [Tooltip("Maximum number of blocks to spawn")]
    [SerializeField] private int m_blockCount = 3;


    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    void Update()
    {
        
    }

    public void SpanwBlock(Vector3 pos, BlockType blockType)
    {
        if(m_blocks.Count > 3)
        {
            while(m_blocks.Count > 3)
            {
                BlockMono block = m_blocks.Dequeue();
                block.HideBlock();
            }
        }


    }

    public void SapwnNextBlock()
    {
        //m_blocks.
    }
}
