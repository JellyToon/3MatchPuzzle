using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StageReader
{
    public static StageInfo LoadStage(int stageNum)
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Stage/{GetFileName(stageNum)}");

        if (textAsset != null)
        {
            StageInfo stageInfo = JsonUtility.FromJson<StageInfo>(textAsset.text);

            return stageInfo;
        }

        return null;
    }
    
    private static string GetFileName(int stageNum)
    {
        return string.Format("stage_{0:D4}", stageNum);
    }
}
