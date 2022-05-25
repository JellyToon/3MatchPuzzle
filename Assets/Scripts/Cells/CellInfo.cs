using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellInfo
{
    int m_row;
    int m_col;
    public int row
    {
        get { return m_row; }
        set { m_row = value; }
    }
    public int col
    {
        get { return m_col; }
        set { m_col = value; }
    }

    private CellType m_cellType;
    public CellType type
    {
        get { return m_cellType; }
        set { m_cellType = value; }
    }

    public CellInfo(CellType cellType)
    {
        m_cellType = cellType;
    }
}
