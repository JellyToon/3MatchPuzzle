using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Clear : MonoBehaviour
{
    [SerializeField] Button m_nextButton;

    private void Awake()
    {
        if(DataSave.instance.nextStage == 0)
        {
            m_nextButton.interactable = false;
        }
    }
    public void ClickNextStageButton()
    {
        string stage = "Stage" + DataSave.instance.nextStage;

        SceneManager.LoadScene(stage);
    }

    public void ClickGoToMain()
    {
        SceneManager.LoadScene("Title");
    }
}
