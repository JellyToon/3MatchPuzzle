using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockInfo
{
    public static float COLLISION_SIZE = 1.0f;
    public static float VANISH_TIME = 0.05f;

    [SerializeField]
    BlockType m_blockType = BlockType.NONE;

    public BlockType type
    {
        get { return m_blockType; }
        set { m_blockType = value; }
    }

    [SerializeField]
    BlockColor m_blockColor = BlockColor.NONE;

    public BlockColor color
    {
        get { return m_blockColor; }
        set { m_blockColor = value; }
    }

    [SerializeField]
    BlockState m_blockState = BlockState.NONE;
    public BlockState state
    {
        get { return m_blockState; }
        set { m_blockState = value; }
    }

    IPosition m_iPosition;
    public IPosition position
    {
        get { return m_iPosition; }
        set { m_iPosition = value; }
    }

    public BlockInfo(BlockInfo blockInfo)
    {
        m_blockType     = blockInfo.type;
        m_blockColor    = blockInfo.color;
        m_blockState    = blockInfo.state;
        m_iPosition     = blockInfo.position;
    }
    public BlockInfo(BlockType blockType)
    {
        m_blockType = blockType;
    }
}
