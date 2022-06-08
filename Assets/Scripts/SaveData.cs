using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="saveData", menuName = "SaveData") ]
public class SaveData : ScriptableObject
{
    public bool[] m_stageData;

    public bool[] GetSaveData()
    {
        return m_stageData;
    }

    public int GetSaveDataLength()
    {
        return m_stageData.Length;
    }

    public void SetSaveDataTrue(int num)
    {
        Debug.Log(num - 1);
        m_stageData[num-1] = true;
    }
}
