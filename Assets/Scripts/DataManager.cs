using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine.InputSystem;

public class DataManager : Singleton<DataManager>
{
    public int waveIndex;
    public StageData StageData = new StageData();
    public string selectedunit;
    public bool isAlly;
    
    private void Awake()
    {
        StageData = LoadStageData("StageData1");
        if (StageData == null)
        {
            Debug.LogWarning("경로에 파일이 존재하지 않습니다.");
            StageData = new StageData();
            return;
        }
        UnitManager.Instance.GenerateUnitsByEntryList(StageData.WavesInStage[0]);
    }
    
    private StageData LoadStageData(string fileName)
    {
        string path = Path.Combine(Application.dataPath, "StageData/"+fileName+".json");
        if (File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            StageData stageData = JsonUtility.FromJson<StageData>(jsonData);
            waveIndex = 0; // waveIndex 초기화
            
            return  stageData;
        }
        
        return null;
    }
    
    public void SaveStageData()
    {
        string path  = Path.Combine(Application.dataPath, "StageData/"+"StageData1.json");
        string data = JsonUtility.ToJson(StageData);
        
        print("저장 경로: " + path);
        File.WriteAllText(path, data);
    }
    
    //입력 받은 유닛을 Stage Data에 기록하고 생성함
   private void AddWaveData(int waveIndex, Vector3Int pos, string unitName, bool isally)
    {
        if (!StageData.WavesInStage.ContainsKey(waveIndex))
            StageData.WavesInStage[waveIndex] = new List<UnitEntry>();

        UnitEntry unitEntry = new UnitEntry(new int[] { pos.x, pos.y, pos.z }, unitName, isally);
        StageData.WavesInStage[waveIndex].Add(unitEntry);
        UnitManager.Instance.GenerateUnit(unitEntry);
        
        Debug.Log(unitName+"을 "+pos+"에 추가함.");
    }

    public void AddUnit(Vector3Int pos)
    {
        if (selectedunit != null)
            AddWaveData(waveIndex,pos,selectedunit,isAlly);
    }

    //게임 오브젝트를 씬과 StageData에서 삭제함
    public void DeleteUnit(GameObject unitobject)
    {
        Unit unit =  unitobject.GetComponent<Unit>();
        int[] pos = new int[] { unit.cellPosition.x, unit.cellPosition.y, unit.cellPosition.z };
        UnitEntry unitEntry = new UnitEntry(pos, unit.unitName, unit.isAlly);
        
        // StageData 삭제
        if (StageData.WavesInStage[waveIndex].Contains(unitEntry)) StageData.WavesInStage[waveIndex].Remove(unitEntry); // Contains를 Equals,GethashCode 함수를 override함으로써 작동하게끔 바꿈
        else Debug.LogWarning(unit.unitName+"를 Stage Data에서 삭제하는데 실패 했습니다.");
        
        //UnitManager에서 삭제
        if(unit.isAlly) UnitManager.Instance.allyUnits.Remove(unit);
        else UnitManager.Instance.enemyUnits.Remove(unit);
        
        Destroy(unitobject);
        print(unit.unitName+"을 삭제함");
    }

    public void ChangeWaveIndex(int delta)
    {
        if (waveIndex + delta < 0)
        {
            print("0 미만의 waveIndex는 생성할 수 없습니다.");
            return;
        }
        
        waveIndex += delta;
        
        print("현재 waveindex : "+waveIndex);
        //기존 wave 유닛 모두 제거
        foreach (Unit u in UnitManager.Instance.allyUnits)
        {
            Destroy(u.gameObject);
        }
        UnitManager.Instance.allyUnits.Clear();
        foreach (Unit u in UnitManager.Instance.enemyUnits)
        {
            Destroy(u.gameObject);
        }
        UnitManager.Instance.enemyUnits.Clear();
        
        //현재 wave 생성
        if (!StageData.WavesInStage.ContainsKey(waveIndex))
        {
            print("선택된 wave의 unitEntryList가 비어 있습니다.");
            StageData.WavesInStage[waveIndex] = new List<UnitEntry>();
            return;
        }
        UnitManager.Instance.GenerateUnitsByEntryList(StageData.WavesInStage[waveIndex]);
    }
}
