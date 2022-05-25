using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlockFactory
{
    public static BlockInfo SpawnBlock(BlockType blockType)
    {
        BlockInfo blockInfo = new BlockInfo(blockType);

        if (blockType == BlockType.BASIC)
            blockInfo.color = (BlockColor)Random.Range((int)BlockColor.FIRST, (int)BlockColor.NUM);
        else if (blockType == BlockType.EMPTY)
            blockInfo.color = BlockColor.FIRST;

        return blockInfo;
    }
}

