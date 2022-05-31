using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    private BlockColor m_color;
    private Image m_colorImage;
    private Text m_count;

    public BlockColor color { get { return m_color; } }


    private void Awake()
    {
        m_colorImage = this.GetComponent<Image>();
        m_count = this.transform.Find("Count").GetComponent<Text>();
    }

    public void InitPrefab(BlockColor blockColor, string text)
    {
        m_color = blockColor;
        Color color;
        switch (blockColor)
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

        m_colorImage.color = color;
        m_count.text = text;
    }

    public void UpdateUI(int count)
    {
        if (count < 0) count = 0;
        m_count.text = count.ToString();
    }
}
