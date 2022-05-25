using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    BlockRoot m_blockRoot;
    StageControl m_stagecontrol;
    
    [SerializeField]
    DIR4 m_slideDir4 = DIR4.NONE;
    public DIR4 dir 
    { 
        get { return m_slideDir4; }
        set { m_slideDir4 = value; }
    }

    [SerializeField]
    BlockInfo m_blockInfo;
    public BlockInfo info
    {
        get { return m_blockInfo; }
        set { m_blockInfo = value; }
    }

    [SerializeField]
    BlockState m_nextState = BlockState.NONE;

    private Vector3 m_beforePosition = Vector3.zero;
    private Vector3 m_afterPosition = Vector3.zero;

    [SerializeField]
    private float m_vanishTimer = -1.0f;
    [SerializeField]
    private float m_stepTimer = 0.0f;

    public float vanishTimer 
    { 
        get { return m_vanishTimer; } 
        set { m_vanishTimer = value; }
    }

    private struct StepFall
    {
        public float velocity;
    }
    private StepFall m_fall;


    #region Init
    private void Start()
    {
        m_nextState = BlockState.IDLE;
    }
    public void InitStageControl(StageControl stageControl)
    {
        m_nextState = BlockState.IDLE;
        m_stagecontrol = stageControl;
        m_blockRoot = m_stagecontrol.blockRoot;
    }
    public void InitBlockInfo(BlockInfo blockInfo)
    {
        m_blockInfo = new BlockInfo(blockInfo);
        //m_blockInfo = blockInfo;
    }
    public void InitPosition(float x, float y, float z)
    {
        this.transform.position = new Vector3(x, y, z);
    }
    public void SetColor()
    {
        Color color;
        switch (m_blockInfo.color)
        {
            case BlockColor.RED:
                color = Color.red;
                break;
            case BlockColor.GREEN:
                color = Color.green;
                break;
            case BlockColor.BLUE:
                color = Color.blue;
                break;
            case BlockColor.ORANGE:
                color = new Color(1.0f, 0.46f, 0.0f);
                break;
            case BlockColor.YELLOW:
                color = Color.yellow;
                break;
            case BlockColor.MAGENTA:
                color = Color.magenta;
                break;
            default:
                color = new Color(0, 0, 0, 0);
                break;
        }

        this.GetComponent<Renderer>().material.color = color;
    }
    public void SetColor(BlockColor color)
    {
        m_blockInfo.color = color;

        this.SetColor();
    }
    #endregion

    private void Update()
    {
        if (info.type == BlockType.EMPTY)
        {
            SetColor();
            return;
        }

        Vector3 mousePosition;
        m_blockRoot.unprojectMousePosition(out mousePosition, Input.mousePosition);
        Vector2 mousePositionXY = new Vector2(mousePosition.x, mousePosition.y);

        if(m_vanishTimer >= 0.0f)
        {
            m_vanishTimer -= Time.deltaTime;
            if (m_vanishTimer < 0.0f)
            {
                if (m_blockInfo.state != BlockState.SLIDE)
                {
                    m_nextState = BlockState.VACANT;
                    m_vanishTimer = -1.0f;
                }
                else
                    m_vanishTimer = 0.0f;
            }
        }

        m_stepTimer += Time.deltaTime;
        float slideTime = 0.2f;
        if(m_nextState == BlockState.NONE)
        {
            switch(m_blockInfo.state)
            {
                case BlockState.SLIDE:
                    if(m_stepTimer >= slideTime)
                    {
                        if (m_vanishTimer == 0.0f)
                            m_nextState = BlockState.VACANT;
                        else
                            m_nextState = BlockState.IDLE;
                    }
                    break;
                case BlockState.IDLE:
                    this.GetComponent<Renderer>().enabled = true;
                    break;
                case BlockState.FALL:
                    if(m_afterPosition.y <= 0.0f)
                    {
                        m_nextState = BlockState.IDLE;
                        m_afterPosition.y = 0.0f;
                    }
                    break;
            }
        }

        while(m_nextState != BlockState.NONE)
        {
            m_blockInfo.state = m_nextState;
            m_nextState = BlockState.NONE;
            switch(m_blockInfo.state)
            {
                case BlockState.IDLE:
                    m_afterPosition = Vector3.zero;
                    this.transform.localScale = Vector3.one * 1.0f;
                    break;
                case BlockState.GRABBED:
                    this.transform.localScale = Vector3.one * 1.2f;
                    break;
                case BlockState.RELEASED:
                    m_afterPosition = Vector3.zero;
                    this.transform.localScale = Vector3.one * 1.0f;
                    break;
                case BlockState.VACANT:
                    m_afterPosition = Vector3.zero;
                    this.SetVisible(false);
                    break;
                case BlockState.RESPAWN:
                    m_blockInfo.color = (BlockColor)Random.Range(0, (int)BlockColor.NUM);
                    this.SetColor();
                    m_nextState = BlockState.IDLE;
                    break;
                case BlockState.FALL:
                    SetVisible(true);
                    m_fall.velocity = 0.0f;
                    break;
            }
        }

        switch(m_blockInfo.state)
        {
            case BlockState.GRABBED:
                m_slideDir4 = this.CalcSlideDir(mousePositionXY);
                break;
            case BlockState.SLIDE:
                float rate = m_stepTimer / slideTime;
                rate = Mathf.Min(rate, 1.0f);
                rate = Mathf.Sin(rate * Mathf.PI / 2.0f);
                m_afterPosition = Vector3.Lerp(m_beforePosition, Vector3.zero, rate);
                break;
            case BlockState.FALL:
                m_fall.velocity += Physics.gravity.y * Time.deltaTime * 0.3f;
                m_afterPosition.y += this.m_fall.velocity * Time.deltaTime;
                if (m_afterPosition.y < 0.0f)
                    m_afterPosition.y = 0.0f;
                break;
        }

        Vector3 position = m_blockRoot.CalcBlockPosition(info.position) + m_afterPosition;
        this.transform.position = position;

        this.SetColor();
    }

    public void BeginGrab()
    {
        m_nextState = BlockState.GRABBED;
    }
    public void EndGrab()
    {
        m_nextState = BlockState.IDLE;
    }

    public bool IsGrabbable()
    {
        switch(m_blockInfo.state)
        {
            case BlockState.IDLE:
                return true;
            default:
                return false;
        }
    }

    public bool IsContainedPosition(Vector2 position)
    {
        Vector3 center = this.transform.position;
        float h = BlockInfo.COLLISION_SIZE / 2.0f;
     
        if (position.x < center.x - h || center.x + h < position.x) return false;
        if (position.y < center.y - h || center.y + h < position.y) return false;

        return true;
    }

    public DIR4 CalcSlideDir(Vector2 mousePosition)
    {
        DIR4 dir = DIR4.NONE;
        Vector2 v = mousePosition - new Vector2(this.transform.position.x, this.transform.position.y);

        if(v.magnitude > 0.1f)
        {
            if (v.y > v.x)
            {
                if (v.y > -v.x)
                    dir = DIR4.UP;
                else
                    dir = DIR4.LEFT;
            }
            else
            {
                if (v.y > -v.x)
                    dir = DIR4.RIGHT;
                else
                    dir = DIR4.DOWN;
            }
        }
        return dir;
    }

    public float CalcDirOffset(Vector2 position, DIR4 dir)
    {
        float offset = 0.0f;
        Vector2 v = position - new Vector2(this.transform.position.x, this.transform.position.y);

        switch(dir)
        {
            case DIR4.RIGHT:    offset = v.x; break;
            case DIR4.LEFT:     offset = -v.x; break;
            case DIR4.UP:       offset = v.y; break;
            case DIR4.DOWN:   offset = -v.y; break;
        }

        return offset;
    }

    public void BeginSlide(Vector3 offset)
    {
        m_beforePosition = offset;
        m_afterPosition = this.m_beforePosition;
        m_nextState = BlockState.SLIDE;
    }

    public void ToVanishing()
    {
        m_vanishTimer = BlockInfo.VANISH_TIME;
    }

    public bool IsVanishing()
    {
        return m_vanishTimer > 0.0f;
    }

    public void RewindVanishTimer()
    {
        m_vanishTimer = BlockInfo.VANISH_TIME;
    }

    public bool IsVisible()
    {
        return this.GetComponent<MeshRenderer>().enabled;
    }

    public void SetVisible(bool isVisible)
    {
        this.GetComponent<MeshRenderer>().enabled = isVisible;
    }

    public bool IsIdle()
    {
        if (this.info.state == BlockState.IDLE && m_nextState == BlockState.NONE)
            return true;

        return false;
    }

    public void BeginFall(Block start)
    {
        m_nextState = BlockState.FALL;
        this.m_afterPosition.y = 
            (float)(start.info.position.y - this.info.position.y) * BlockInfo.COLLISION_SIZE;
    }

    public void BeginRespawn(int startPositionY)
    {
        m_afterPosition.y =
            (float)(startPositionY - this.info.position.y) * BlockInfo.COLLISION_SIZE;
        m_nextState = BlockState.FALL;
        int colorIndex = Random.Range((int)BlockColor.FIRST, (int)BlockColor.LAST);
        
        this.SetColor((BlockColor)colorIndex);
    }

    public bool IsVacant()
    {
        if (this.info.state == BlockState.VACANT && m_nextState == BlockState.NONE)
            return true;

        return false;
    }

    public bool IsSliding()
    {
        return m_afterPosition.x != 0.0f;
    }
}
