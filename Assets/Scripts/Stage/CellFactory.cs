using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellFactory
{
    public static CellInfo SpawnCell(StageInfo stageInfo, int row, int col)
    {
        return SpawnCell(stageInfo.GetCellType(row, col));
    }

    public static CellInfo SpawnCell(CellType cellType)
    {
        return new CellInfo(cellType);
    }
}
