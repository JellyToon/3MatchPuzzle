using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageControl : MonoBehaviour
{

    [SerializeField] Transform m_blockArea;
    public Transform blockAreaTrans { get { return m_blockArea; } }
    [SerializeField] GameObject m_blockPrefab;

    [SerializeField] Transform m_cellArea;
    [SerializeField] GameObject m_cellPrefab;

    [SerializeField] Transform m_itemArea;
    [SerializeField] GameObject m_itemPrefab;

    [SerializeField] Transform m_ItemParent;
    [SerializeField] GameObject m_boxPrefab;
    [SerializeField] BOX m_box;

    [SerializeField] Text m_text;
    [SerializeField] Text m_timer;
    float time = 60f;
    //[SerializeField] AudioMgr m_audioMgr;

    public int m_stageNum;

    private BlockRoot m_blockRoot;
    public BlockRoot blockRoot { get { return m_blockRoot; } }

    [SerializeField]
    private StageInfo m_stageInfo;

    public StageInfo StageInfo { get { return m_stageInfo; } }

    private CellInfo[,] m_cellInfos;
    private Block[,] m_blocks;

    public List<int> m_clearColor;
    public List<int> m_trapColor;
    public int[] m_clearCondition = new int[(int)BlockColor.NUM]; 
    //public int[] m_clearCount = new int[(int)BlockColor.NUM];

    private void Awake()
    {
        m_blockRoot = this.GetComponent<BlockRoot>();
        for(int i = 0; i<m_clearCondition.Length;++i)
        {
            m_clearCondition[i] = -1;
        }
    }

    private void Start()
    {
        InitStage();
    }

    private void Update()
    {
        time -= Time.deltaTime;

        m_timer.text = "Timer : " + ((int)time).ToString();
        if (time <= 0)
            SceneManager.LoadScene("GameOver");
    }


    public void InitStage()
    {
        m_stageInfo = StageReader.LoadStage(m_stageNum);

        m_cellInfos = new CellInfo[m_stageInfo.maxRow, m_stageInfo.maxCol];
        m_blocks = new Block[m_stageInfo.maxRow, m_stageInfo.maxCol];
        BlockInfo blockInfo;

        float initX = CalcInitX(0.5f);
        float initY = CalcInitY(0.5f);

        for (int row = 0; row < m_stageInfo.maxRow; ++row)
        {
            for (int col = 0; col < m_stageInfo.maxCol; ++col)
            {
                m_cellInfos[row, col] = SetCellInfo(row, col);
                blockInfo = SetBlockInfo(row, col);
                //m_blocks[row, col].info = SetBlockInfo(row, col);

                SpawnCell(m_cellInfos[row, col], row, col);
                SpawnBlock(blockInfo, row, col);
            }
        }

        InitBlockRoot();
        InitClearCondition();
    }

    private void InitBlockRoot()
    {
        m_blockRoot.InitObject(m_blockPrefab, m_blockArea, this);
        m_blockRoot.InitVariable(m_stageInfo.maxRow, m_stageInfo.maxCol);
        m_blockRoot.InitInfos(m_cellInfos, m_blocks);
    }

    public float CalcInitX(float offset = 0)
    {
        return -m_stageInfo.maxCol / 2.0f + offset;
    }
    public float CalcInitY(float offset = 0)
    {
        return -m_stageInfo.maxRow / 2.0f + offset;
    }

    private CellInfo SetCellInfo(int row, int col)
    {
        return CellFactory.SpawnCell(m_stageInfo, row, col);
    }
    private BlockInfo SetBlockInfo(int row, int col)
    {
        if (m_cellInfos[row, col].type == CellType.EMPTY)
            return BlockFactory.SpawnBlock(BlockType.EMPTY);

        return BlockFactory.SpawnBlock(BlockType.BASIC);
    }

    private void SpawnCell(CellInfo cellInfo, int row, int col)
    {
        if (cellInfo.type == CellType.EMPTY) return;

        float initX = CalcInitX(0.5f);
        float initY = CalcInitY(0.5f);

        GameObject cellObject = null;
        Cell cell = null;

        cellObject = Instantiate(m_cellPrefab, m_cellArea);
        cellObject.name = "cells[" + row.ToString() + "," + col.ToString() + "]";
        cell = cellObject.GetComponent<Cell>();

        cell.InitPosition
            (initX + row + m_cellArea.transform.position.x,
            initY + col + m_cellArea.transform.position.y,
            m_cellArea.transform.position.z);
    }
    private void SpawnBlock(BlockInfo blockInfo, int row, int col)
    {
        bool emptyBlock = false;
        if (blockInfo.type == BlockType.EMPTY)
        {
            emptyBlock = true;
        }

        float initX = CalcInitX(0.5f);
        float initY = CalcInitY(0.5f);

        GameObject blockObject = null;
        Block block = null;

        IPosition iPosition;
        iPosition.x = row;
        iPosition.y = col;
        blockInfo.position = iPosition;

        blockObject = Instantiate(m_blockPrefab, m_blockArea);
        blockObject.name = "blocks[" + row.ToString() + "," + col.ToString() + "]";
        block = blockObject.GetComponent<Block>();

        block.InitStageControl(this);

        block.InitBlockInfo(blockInfo);
        block.InitPosition
            (initX + row + m_blockArea.transform.position.x,
            initY + col + m_blockArea.transform.position.y,
            m_blockArea.transform.position.z);
        block.SetColor();

        if (emptyBlock == true)
            blockObject.GetComponent<MeshRenderer>().enabled = false;

        m_blocks[row, col] = block;
    }

    void InitClearCondition()
    {
        int random;

        int i = 0;
        while(i<m_stageInfo.clearColorNumCount)
        {
            random = Random.Range(0, (int)BlockColor.NUM);
            if(m_clearCondition[random] == -1)
            {
                m_clearCondition[random] = Random.Range(15,20);
                m_clearColor.Add(random);
                ++i;
                continue;
            }
        }
        i = 0;
        while(i<m_stageInfo.trapColorNumCount)
        {
            random = Random.Range(0, (int)BlockColor.NUM);
            if(m_clearCondition[random] == -1)
            {
                m_clearCondition[random] = -2;
                m_trapColor.Add(random);
                ++i;
                continue;
            }
        }

        m_text.text = "";
        for(i = 0; i<m_stageInfo.clearColorNumCount + m_stageInfo.trapColorNumCount;++i)
        {
            if (i < m_stageInfo.clearColorNumCount)
                m_text.text +=
                    ((BlockColor)(m_clearColor[i])).ToString() + " : " + m_clearCondition[m_clearColor[i]] + "\n";
            else
                m_text.text += "X : " + ((BlockColor)(m_trapColor[i-m_stageInfo.clearColorNumCount])).ToString();
        }
        
    }

    public void AddItem(BlockColor color, Vector3 initPosition)
    {
        GameObject go = Instantiate(m_itemPrefab, m_ItemParent);
        Item item = go.GetComponent<Item>();

        Vector3 position = new Vector3(initPosition.x, initPosition.y, -5f);

        item.InitStageControl(this);
        item.InitPosition(position);
        item.SetColor(color);

    }
    public void EatItem(GameObject itemObject)
    {
        Item item = itemObject.GetComponent<Item>();

        BlockColor color = item.color;

        foreach(int i in m_clearColor)
        {
            if(color == (BlockColor)i)
            {
                if (m_clearCondition[i] != 0)
                {
                    m_clearCondition[i] -= 1;
                    UpdateUI();
                    CheckGameClear();
                }
                Destroy(itemObject);
                return;
            }
        }

        foreach(int i in m_trapColor)
        {
            if(color == (BlockColor)i)
            {
                m_box.GetDebuffs();
                Destroy(itemObject);
                return;
            }
        }
    }

    public void UpdateUI()
    {
        m_text.text = "";
        for (int i = 0; i < m_stageInfo.clearColorNumCount + m_stageInfo.trapColorNumCount; ++i)
        {
            if (i < m_stageInfo.clearColorNumCount)
                m_text.text +=
                    ((BlockColor)(m_clearColor[i])).ToString() + " : " + m_clearCondition[m_clearColor[i]] + "\n";
            else
                m_text.text += "X : " + ((BlockColor)(m_trapColor[i - m_stageInfo.clearColorNumCount])).ToString();
        }
    }

    public void CheckGameClear()
    {
        int length = m_clearCondition.Length;
        foreach(int i in m_clearCondition)
        {
            if (i > 0) continue;
            
            length -= 1;
        }

        Debug.Log(length);
        if(length == 0)
        {
            SceneManager.LoadScene("Clear");
        }
    }
}
