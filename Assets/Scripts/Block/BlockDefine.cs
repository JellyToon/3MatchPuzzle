using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    NONE    = -1,
    EMPTY   = 0,
    BASIC   = 1
}

public enum BlockColor
{
    NONE    = -1,
    RED     = 0,
    GREEN   = 1,
    BLUE    = 2,
    ORANGE  = 3,
    YELLOW  = 4,
    MAGENTA = 5,
    NUM,
    FIRST = RED,
    LAST = NUM
}

public enum BlockState
{
    NONE      = -1,
    IDLE      = 0, 
    GRABBED, 
    RELEASED, 
    SLIDE, 
    VACANT, 
    RESPAWN, 
    FALL, 
    EMPTY,
    NUM
}


public enum DIR4
{
    NONE    = -1,
    RIGHT,
    LEFT,
    UP,
    DOWN,
    NUM
}

public struct IPosition
{
    public int x;
    public int y;
}

//public static class BlockMethod
//{
//    public static bool IsSafeEqual(this Block block, Block targetBlock)
//    {
//        if (block == null) return false;

//        return block.IsEqual(targetBlock);
//    }
//}