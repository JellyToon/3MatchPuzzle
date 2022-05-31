using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRoot : MonoBehaviour
{
    private GameObject m_blockPrefab;
    private Transform m_blockArea;
    private StageControl m_stageControl;

    private int m_maxRow;
    private int m_maxCol;
    
    private CellInfo[,] m_cellInfos;
    private Block[,] m_blocks;

    private Camera m_mainCamera;
    [SerializeField]
    private AudioMgr m_audioMgr;


    private Block m_grabbedBlock = null;

    #region Init
    public void InitObject(GameObject blockPrefab, Transform blockArea, StageControl stageControl)
    {
        m_blockPrefab = blockPrefab;
        m_blockArea = blockArea;
        m_stageControl = stageControl;
    }
    public void InitVariable(int maxRow, int maxCol)
    {
        m_maxRow = maxRow;
        m_maxCol = maxCol;
    }
    public void InitInfos(CellInfo[,] cellInfos, Block[,] blocks)
    {
        m_cellInfos = cellInfos;
        m_blocks = blocks;
    }
    #endregion

    void Awake()
    {
        m_mainCamera = Camera.main;
    }

    private void Update()
    {
        if (m_stageControl.Pause == true) return;

        Vector3 mousePosition;

        unprojectMousePosition(out mousePosition, Input.mousePosition);

        Vector2 mousePositionXY = new Vector2(mousePosition.x, mousePosition.y);

        if(m_grabbedBlock == null)
        {
            if(IsHasFallingBlock() == false)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    foreach(Block block in m_blocks)
                    {
                        if (block.info.type == BlockType.EMPTY) continue;
                        if (block == null) continue;
                        if (block.IsGrabbable() == false) continue;
                        if (block.IsContainedPosition(mousePositionXY) == false) continue;

                        m_grabbedBlock = block;
                        m_grabbedBlock.BeginGrab();

                        break;
                    }
                }
            }
        }
        else
        {
            do
            {
                Block swapTarget = this.GetNextBlock(m_grabbedBlock, m_grabbedBlock.dir);
                
                if (swapTarget == null) break;
                if (swapTarget.info.type == BlockType.EMPTY) break;
                if (swapTarget.IsGrabbable() == false) break;

                float offset = m_grabbedBlock.CalcDirOffset(mousePositionXY, m_grabbedBlock.dir);
                if (offset < BlockInfo.COLLISION_SIZE / 2.0f) break;

                this.SwapBlock(m_grabbedBlock, m_grabbedBlock.dir, swapTarget);
                m_grabbedBlock = null;
            }
            while (false);

            if(Input.GetMouseButton(0) == false)
            {
                m_grabbedBlock.EndGrab();
                m_grabbedBlock = null;
            }
        }

        if (IsHasFallingBlock() == true || IsHasSlidingBlock() == true)
        {
            //
        }
        else
        {
            int count = 0;

            foreach (Block block in m_blocks)
            {
                if (block.IsIdle() == false) continue;
                if (this.CheckConnection(block) == true) count += 1;
            }

            if (count > 0)
            {
                int blockCount = 0;

                foreach (Block block in m_blocks)
                {
                    if (block.info.type == BlockType.EMPTY) continue;
                    if (block.IsVanishing() == true)
                        block.RewindVanishTimer();
                }
            }
        }

        bool isVanishing = this.IsHasVanishingBlock();

        do
        {
            if (isVanishing == true) break;
            if (IsHasSlidingBlock() == true) break;
            for (int x = 0; x < m_maxRow; ++x)
            {
                if (this.IsHasSlidingBlockInColumn(x) == true) continue;

                for (int y = 0; y < m_maxCol - 1; ++y)
                {
                    if (m_blocks[x, y].info.type == BlockType.EMPTY) continue;
                    if (m_blocks[x, y].IsVacant() == false) continue;

                    for (int y1 = y + 1; y1 < m_maxCol; ++y1)
                    {
                        if (m_blocks[x, y1].info.type == BlockType.EMPTY) continue;
                        if (m_blocks[x, y1].IsVacant() == true) continue;

                        this.FallBlock(m_blocks[x, y], DIR4.UP, m_blocks[x, y1]);
                        break;
                    }
                }
            }

            for (int x = 0; x < m_maxRow; ++x)
            {
                int fallStartY = m_maxCol;
                for(int y = 0; y< m_maxCol;++y)
                {
                    if (m_blocks[x, y].info.type == BlockType.EMPTY) continue;
                    if (m_blocks[x, y].IsVacant() == false) continue;

                    m_blocks[x, y].BeginRespawn(fallStartY);
                    fallStartY += 1;
                }
            }
            m_audioMgr.PlayEffectSound();
        } while (false);

    }

    public void unprojectMousePosition(out Vector3 worldPosition, Vector3 mousePosition)
    {
        Plane plane = new Plane(Vector3.back, new Vector3(0.0f, 0.0f, -BlockInfo.COLLISION_SIZE / 2.0f));

        Ray ray = m_mainCamera.ScreenPointToRay(mousePosition);
        float depth;

        if (plane.Raycast(ray, out depth))
            worldPosition = ray.origin + ray.direction * depth;
        else
            worldPosition = Vector3.zero;
    }

    private bool IsHasVanishingBlock()
    {
        foreach(Block block in m_blocks)
        {
            //if (block.info.type == BlockType.EMPTY) continue;
            if (block.vanishTimer > 0.0f)
                return true;
        }
        return false;
    }

    private bool IsHasFallingBlock()
    {
        foreach(Block block in m_blocks)
        {
            if (block == null) continue;
            if (block.info.state == BlockState.FALL)
                return true;
        }
        return false;
    }

    private bool IsHasSlidingBlock()
    {
        foreach(Block block in m_blocks)
        {
            if (block.info.type == BlockType.EMPTY) continue;

            if (block.info.state == BlockState.SLIDE)
                return true;
        }
        return false;
    }

    public Block GetNextBlock(Block block, DIR4 dir)
    {
        Block nextBlock = null;
        
        int positionX = block.info.position.x + (int)m_stageControl.blockAreaTrans.position.x;
        int positionY = block.info.position.y + (int)m_stageControl.blockAreaTrans.position.y;

        int arrX = block.info.position.x/* - (int)m_stageControl.blockAreaTrans.position.x*/;
        int arrY = block.info.position.y/* - (int)m_stageControl.blockAreaTrans.position.y*/;

        switch(dir)
        {
            case DIR4.RIGHT:
                if(arrX < m_stageControl.StageInfo.maxRow - 1)
                    nextBlock = m_blocks[arrX + 1, arrY];
                break;
            case DIR4.LEFT:
                if (arrX > 0)
                    nextBlock = m_blocks[arrX - 1, arrY];
                break;
            case DIR4.UP:
                if (arrY < m_stageControl.StageInfo.maxCol - 1)
                    nextBlock = m_blocks[arrX, arrY + 1];
                break;
            case DIR4.DOWN:
                if (arrY > 0)
                    nextBlock = m_blocks[arrX, arrY - 1];
                break;
        }

        if (nextBlock != null && nextBlock.info.type == BlockType.EMPTY) return nextBlock;
        return nextBlock;
    }
    
    public Vector3 CalcBlockPosition(IPosition iposition)
    {
        Vector3 position = new Vector3
            (-(m_stageControl.StageInfo.maxRow / 2.0f - 0.5f),
            -(m_stageControl.StageInfo.maxCol / 2.0f - 0.5f)
            ,0.0f );

        position.x += (float)iposition.x * BlockInfo.COLLISION_SIZE + m_blockArea.position.x;
        position.y += (float)iposition.y * BlockInfo.COLLISION_SIZE + m_blockArea.position.y;

        return position;
    }

    public Vector3 GetDirVector(DIR4 dir)
    {
        Vector3 v = Vector3.zero;
        switch(dir)
        {
            case DIR4.RIGHT:    v = Vector3.right;  break;
            case DIR4.LEFT:     v = Vector3.left;   break;
            case DIR4.UP:       v = Vector3.up;     break;
            case DIR4.DOWN:     v = Vector3.down;   break;
        }

        v *= BlockInfo.COLLISION_SIZE;
        return v;
    }

    public DIR4 GetOppositDir(DIR4 dir)
    {
        DIR4 opposit = dir;
        switch(dir)
        {
            case DIR4.RIGHT:    opposit = DIR4.LEFT;    break;
            case DIR4.LEFT:     opposit = DIR4.RIGHT;   break;
            case DIR4.UP:       opposit = DIR4.DOWN;    break;
            case DIR4.DOWN:     opposit = DIR4.UP;      break;
        }
        return opposit;
    }

    public void SwapBlock(Block block0, DIR4 dir, Block block1)
    {
        BlockColor color0 = block0.info.color;
        BlockColor color1 = block1.info.color;

        Vector3 scale0 = block0.transform.localScale;
        Vector3 scale1 = block1.transform.localScale;

        float vanishTimer0 = block0.vanishTimer;
        float vanishTimer1 = block1.vanishTimer;

        Vector3 offset0 = this.GetDirVector(dir);
        Vector3 offset1 = this.GetDirVector(this.GetOppositDir(dir));

        block0.SetColor(color1);
        block1.SetColor(color0);

        block0.vanishTimer = vanishTimer1;
        block1.vanishTimer = vanishTimer0;

        block0.BeginSlide(offset0);
        block1.BeginSlide(offset1);
    }

    public bool CheckConnection(Block start)
    {
        bool ret = false;

        //if (start.info.type == BlockType.EMPTY) return ret;

        int normalBlockNum = 0;
        if (start.IsVanishing() == false)
            normalBlockNum = 1;

        int rx = start.info.position.x;
        int lx = start.info.position.x;


        Block nextBlock;
        for (int x = lx - 1; x > 0; --x)
        {
            nextBlock = m_blocks[x, start.info.position.y];
            if (nextBlock.info.type == BlockType.EMPTY) break;
            if (nextBlock.info.color != start.info.color) break;
            if (nextBlock.info.state == BlockState.FALL || nextBlock.info.state == BlockState.FALL) break;
            if (nextBlock.info.state == BlockState.SLIDE || nextBlock.info.state == BlockState.SLIDE) break;

            if (nextBlock.IsVanishing() == false) normalBlockNum += 1;
            lx = x;
        }

        for (int x = rx + 1; x < m_maxRow; ++x)
        {
            nextBlock = m_blocks[x, start.info.position.y];

            if (nextBlock.info.type == BlockType.EMPTY) break;
            if (nextBlock.info.color != start.info.color) break;
            if (nextBlock.info.state == BlockState.FALL || nextBlock.info.state == BlockState.FALL) break;
            if (nextBlock.info.state == BlockState.SLIDE || nextBlock.info.state == BlockState.SLIDE) break;

            if (nextBlock.IsVanishing() == false) normalBlockNum += 1;
            rx = x;
        }

        do
        {
            if ((rx - lx + 1) < 3) break;
            if (normalBlockNum == 0) break;
            for (int x = lx; x < rx + 1; ++x)
            {
                m_blocks[x, start.info.position.y].ToVanishing();
                ret = true;
            }
        } while (false);

        normalBlockNum = 0;
        if (start.IsVanishing() == false) normalBlockNum = 1;

        int uy = start.info.position.y;
        int dy = start.info.position.y;

        for (int y = dy - 1; y > 0; --y)
        {
            nextBlock = m_blocks[start.info.position.x, y];

            if (nextBlock.info.type == BlockType.EMPTY) break;
            if (nextBlock.info.color != start.info.color) break;
            if (nextBlock.info.state == BlockState.FALL || nextBlock.info.state == BlockState.FALL) break;
            if (nextBlock.info.state == BlockState.SLIDE || nextBlock.info.state == BlockState.SLIDE) break;

            if (nextBlock.IsVanishing() == false) normalBlockNum += 1;
            dy = y;
        }
        for(int y = uy + 1; y<m_maxCol;++y)
        {
            nextBlock = m_blocks[start.info.position.x, y];

            if (nextBlock.info.type == BlockType.EMPTY) break;
            if (nextBlock.info.color != start.info.color) break;
            if (nextBlock.info.state == BlockState.FALL || nextBlock.info.state == BlockState.FALL) break;
            if (nextBlock.info.state == BlockState.SLIDE || nextBlock.info.state == BlockState.SLIDE) break;

            if (nextBlock.IsVanishing() == false) normalBlockNum += 1;
            uy = y;
        }

        do
        {
            if (uy - dy + 1 < 3) break;
            if (normalBlockNum == 0) break;

            for (int y = dy; y < uy + 1; ++y)
            {
                m_blocks[start.info.position.x, y].ToVanishing();
                ret = true;
            }
        } while (false);

        return ret;
    }


    public void FallBlock(Block upBlock, DIR4 dir, Block dwBlock)
    {
        BlockColor upColor = upBlock.info.color;
        BlockColor dwColor = dwBlock.info.color;

        Vector3 upScale = upBlock.transform.localScale;
        Vector3 dwScale = dwBlock.transform.localScale;

        float upVanishTimer = upBlock.vanishTimer;
        float dwVanishTimer = dwBlock.vanishTimer;

        bool upVisible = upBlock.IsVisible();
        bool dwVisible = dwBlock.IsVisible();

        BlockState upState = upBlock.info.state;
        BlockState dwState = dwBlock.info.state;

        upBlock.SetColor(dwColor);
        dwBlock.SetColor(upColor);
        upBlock.transform.localScale = dwScale;
        dwBlock.transform.localScale = upScale;
        upBlock.vanishTimer = dwVanishTimer;
        dwBlock.vanishTimer = upVanishTimer;
        upBlock.SetVisible(dwVisible);
        dwBlock.SetVisible(upVisible);
        upBlock.info.state = dwState;
        dwBlock.info.state = upState;

        upBlock.BeginFall(dwBlock);

    }

    private bool IsHasSlidingBlockInColumn(int x)
    {
        for(int y = 0; y<m_maxCol;++y)
        {
            if (m_blocks[x, y].info.type == BlockType.EMPTY) continue;
            if (m_blocks[x, y].IsSliding() == true)
                return true;
        }
        return false;
    }
}
