using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public CanvasGroup m_selectGroup;
    public CanvasGroup m_manualGroup;

    [SerializeField] SaveData m_saveData;
    [SerializeField] bool[] m_stage;

    [SerializeField] Button[] m_stageButton;

    private void Start()
    {
        Screen.SetResolution(1600, 900, false);

        if (m_saveData == null) return;
        m_stage = new bool[m_saveData.GetSaveDataLength()];
        m_stage = m_saveData.GetSaveData();

        for(int i = 0; i<m_stage.Length;++i)
        {
            if(m_stage[i] == true)
                m_stageButton[i].interactable = true;
            else
                m_stageButton[i].interactable = false;
        }
    }
    public void StageLoad(int i)
    {
        string stage = "Stage" + i.ToString();

        SceneManager.LoadScene(stage);
    }

    public void ClickStartButton()
    {
        m_selectGroup.alpha = 1.0f;
        m_selectGroup.blocksRaycasts = true;
    }

    public void ClickManualButton()
    {
        m_manualGroup.alpha = 1.0f;
        m_manualGroup.blocksRaycasts = true;
    }

    public void ClickManualExitButton()
    {
        m_selectGroup.alpha = 0.0f;
        m_selectGroup.blocksRaycasts = false;
        m_manualGroup.alpha = 0.0f;
        m_manualGroup.blocksRaycasts = false;

    }


    public void ClickExitButton()
    {
        Application.Quit();
    }

    public void ClickReStart()
    {
        SceneManager.LoadScene("Title");
    }
}
