using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private StageControl m_stageControl;

    BlockColor m_blockColor;
    public BlockColor color 
    { 
        get { return m_blockColor; }
        set { m_blockColor = value; }
    }

    float m_fallVelocity = 0.0f;

    public void InitStageControl(StageControl stageControl)
    {
        m_stageControl = stageControl;
    }

    public void InitPosition(Vector3 position)
    {
        this.transform.position = position;
    }

    public void SetColor(BlockColor blockColor)
    {
        m_blockColor = blockColor;

        Color color;

        switch (m_blockColor)
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

    private void Update()
    {
        if (m_stageControl.Pause == true) return;

        m_fallVelocity += Physics.gravity.y * Time.deltaTime * 0.020f;
        this.transform.position = new Vector3
            (this.transform.position.x, this.transform.position.y + m_fallVelocity, this.transform.position.z);
    }
}
