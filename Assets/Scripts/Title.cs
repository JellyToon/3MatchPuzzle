using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public CanvasGroup canvasGroup;


    public void StageLoad(int i)
    {
        string stage = "Stage" + i.ToString();

        SceneManager.LoadScene(stage);
    }

    public void ClickStartButton()
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
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
