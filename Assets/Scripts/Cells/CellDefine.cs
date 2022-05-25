using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    EMPTY = 0,
    BASIC
    //FIXTURE,
    //JELLY
}

static class CellTypeMethod
{
    public static bool IsBlockAllocatableType(this CellType cellType)
    {
        return !(cellType == CellType.EMPTY);
    }

    public static bool IsBlockMovableType(this CellType cellType)
    {
        return !(cellType == CellType.EMPTY);
    }
}
