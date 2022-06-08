using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSave : MonoBehaviour
{
    public static DataSave instance;

    public SaveData saveData;

    public bool[] stage;

    public int nextStage;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        stage = new bool[saveData.GetSaveDataLength()];
        stage = saveData.GetSaveData();
        nextStage = 0;
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetSaveDataTrue(int stageNum)
    {
        if (stageNum < stage.Length)
        {
            nextStage = stageNum + 1;
            saveData.SetSaveDataTrue(stageNum + 1);
        }
        else
            nextStage = 0;
    }
}
