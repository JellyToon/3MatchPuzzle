using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class StageInfo
{
    public int maxRow;
    public int maxCol;

    public int[] cells;

    public int clearColorNumCount;
    public int trapColorNumCount;

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }

    public CellType GetCellType(int row, int col)
    {
        int revisedRow = (this.maxRow - 1) - row;

        if (cells.Length > revisedRow * this.maxCol + col)
            return (CellType)cells[revisedRow * this.maxCol + col];

        return CellType.EMPTY;
    }

    public bool DeValidation()
    {
        if (this.cells.Length != this.maxRow * this.maxCol)
            return false;

        return true;
    }
}
