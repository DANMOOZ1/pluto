using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class DataManager : Singleton<DataManager>
{
    public int waveIndex;
    public StageData StageData = new StageData();
    
    private void Awake()
    {
        SaveStageData();
        waveIndex = 0;
        
        StageData = LoadStageData("StageData1");
        if (StageData == null) Debug.LogWarning("경로에 파일이 존재하지 않습니다.");
    }

    public void NextWave()
    {
        
    }
    
    private StageData LoadStageData(string fileName)
    {
        string path = Path.Combine(Application.dataPath, "StageData/"+fileName+".json");
        if (File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            StageData stageData = JsonUtility.FromJson<StageData>(jsonData);
            return  stageData;
        }
        
        return null;
    }
    
    private void SaveStageData()
    {
        string path  = Path.Combine(Application.dataPath, "StageData/"+"StageData1.json");
        string data = JsonUtility.ToJson(StageData);
        
        print("저장 경로: " + path);
        File.WriteAllText(path, data);
    }
    
    // 안전한 방식으로 추가
    void AddWaveData(int waveIndex, Vector3Int pos, string unitName, bool isAlly)
    {
        if (StageData.WavesInStage[waveIndex] == null)
            StageData.WavesInStage[waveIndex] = new List<UnitEntry>();
    
        
        StageData.WavesInStage[waveIndex].Add(new UnitEntry(new int[]{pos.x,pos.y,pos.z}, unitName, isAlly));
        
        Debug.Log(unitName+"을 "+pos.ToString()+"에 추가함.");
    }
}
