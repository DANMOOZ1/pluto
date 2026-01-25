using GLTFast.Schema;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;

// 추가해야 할 것
// 1. 무식하게 매번 바를 그리는 방식인데, 다소 비효율적이라 미리 그려놓고 켜고 끄는 걸로 바꿔야 할 것 같아요 지금 당장은 제가 시간이 없어서 + 굴러가긴 해서 월~화 새벽에 바꿀 것 같습니다

public class UIManager : Singleton<UIManager>
{
    [Header("leftUI")]
    public GameObject leftUIP;
    public GameObject leftUI;
    public TextMeshProUGUI lUI_UnitName;
    public TextMeshProUGUI lUI_Level;
    public Image lUI_Portrait;
    public GameObject lUI_HP;

    [Header("infoUI")]
    public GameObject infoUIP;
    public GameObject infoUI;
    public Image iUI_Portrait;
    public TextMeshProUGUI iUI_UnitName;
    public TextMeshProUGUI iUI_Level;
    public GameObject iUI_HP;
    public GameObject iUI_ATK;
    public GameObject iUI_DEF;
    public GameObject iUI_FOC;
    public GameObject iUI_SPD;
    public Image iUI_MOV;
    public Image iUI_RNG;
    public Image iUI_HTA;
    public GameObject iUI_Button;

    [Header("enemySelectedUI")]
    public GameObject enemySelectedUIP;
    public GameObject enemySelectedUI;
    public Image eUI_allyPortrait;
    public GameObject eUI_allyHP;
    public GameObject eUI_allyATK;
    public GameObject eUI_allyFOC;
    public GameObject eUI_allySPD;
    public Image eUI_enemyPortrait;
    public GameObject eUI_enemyHP;
    public GameObject eUI_enemyDEF;
    public GameObject eUI_enemyFOC;
    public GameObject eUI_enemySPD;

    [Header("righttUI")]
    public GameObject rightUIP;
    public GameObject rightUI;
    public TextMeshProUGUI rUI_UnitName;
    public TextMeshProUGUI rUI_Level;
    public Image rUI_Portrait;
    public GameObject rUI_HP;

    [Header("battleButtons")]
    public GameObject battleButtonsP;
    public GameObject battleButtons;
    public GameObject combatButton;
    public GameObject nextButton;
    public GameObject infoButton;

    [Header("StayButton")]
    public GameObject StayButtonP;
    public GameObject StayButton;

    [Header("HeadUI")]
    public GameObject headUI;

    [Header("barImage")]
    public GameObject bigBar;
    public GameObject smallBar;

    [Header("turnUI")]
    public GameObject turnUIP;
    public GameObject turnUI;

    [Header("debugMode")]
    public GameObject IsAllyToggle;

    // 가져올 unit turn 만들기
    private Unit currentUnit;
    private Unit selectedAlly;
    private Unit selectedEnemy;

    // 유닛 턴인가요?
    private bool isPlayerTurn;

    public void Start()
    {
        // 구독
        GameManager.Instance.OnGameStateChange += GameStateUI;
        GameManager.Instance.OnBattleStateChange += BattleStateUI;

    }

    public void GameStateUI()
    {
        switch (GameManager.Instance.gameState)
        {
            case GameState.Menu:
                break;
            case GameState.Battle:
                create();
                break;
            case GameState.PositionSetUp:
                break;
            case GameState.UnitSetUp:
                break;
            case GameState.Victory:
                break;
            case GameState.Defeat:
                break;
            case GameState.Debug:
                UIDebug();
                break;

        }
    }


    // 상태별 UI 켜기
    public void BattleStateUI()
    {
        if (GameManager.Instance.gameState == GameState.Battle)
        {
            // 창 다 지우기
            infoUI.SetActive(false);
            enemySelectedUI.SetActive(false);
            leftUI.SetActive(false);
            rightUI.SetActive(false);
            IsAllyToggle.SetActive(false);

            turnUI.SetActive(true);

            // 현재 차례인 유닛 가져오기
            currentUnit = UnitManager.Instance.selectedUnit;



            // 플레이어 턴
            if (UnitManager.Instance.selectedUnit.isAlly)
            {
                selectedAlly = currentUnit;

                // 플레이어 턴
                isPlayerTurn = true;

                // 좌하단 UI 띄우기
                if (UnitManager.Instance.selectedAlly != null)
                {
                    selectedAlly = UnitManager.Instance.selectedAlly;
                }
                LeftUI();

                // 상태별
                switch (GameManager.Instance.battleState)
                {

                    case BattleState.Move:
                        UIMove();
                        break;
                    case BattleState.Default:
                        UIChoice();
                        break;
                    case BattleState.Combat:
                        UICombat();
                        break;
                    case BattleState.Next:
                        UINext();
                        break;
                    case BattleState.Info:
                        UIInfo();
                        break;
  
                }

            }
            else
            {
                selectedEnemy = currentUnit;

                // 우하단 UI 띄우기
                RightUI();
            }

        }

        // 머리 위 UI 띄우기
        foreach (Unit unit in UnitManager.Instance.spdSortUnits)
        {
            HeadUI(unit, isPlayerTurn);
        }

        // 순서 띄우기
        UIturn(UnitManager.Instance.spdSortUnits);
    }

    public void UIChoice()
    {

        if (UnitManager.Instance.UnitCanAttack())
        {
            combatButton.GetComponent<Button>().interactable = true;
        } else
        {
            combatButton.GetComponent<Button>().interactable = false;
        }

        // 버튼 띄우기
        foreach (Transform child in currentUnit.transform)
            {
                if (child.name == "HeadUI")
                {
                    child.gameObject.SetActive(false);
                }
            }
        battleButtons.transform.SetParent(currentUnit.transform, false);
        battleButtons.SetActive(true);

        // 적 선택 시 비교창
        if (UnitManager.Instance.selectedEnemy != null)
        {
            UIEnemySelected();
        }

        StayButton.SetActive(false);


    }

    public void UIMove()
    {

        // 버튼?
        //Debug.Log($"UIMove 호출! StayButton null? {StayButton == null}");
        battleButtons.SetActive(false);

        //foreach (Unit unit in UnitManager.Instance.spdSortUnits)
        //{
        //    Debug.Log(unit.cellPosition);

        //    Vector3Int diff = unit.cellPosition - selectedAlly.cellPosition;
        //    if (diff.x >= 0 && diff.x <= 1 && diff.y >= 0 && diff.y <= 1 && diff != Vector3Int.zero && unit != selectedEnemy)
        //    {

        //    } else
        //    {

        //    }
        //}

        // 적 선택 시 비교창

        StayButton.SetActive(true);

        if (UnitManager.Instance.selectedEnemy != null)
        {
            UIEnemySelected();
        }

    }

    public void UICombat()
    {
        // 버튼?
        battleButtons.SetActive(false);
        StayButton.SetActive(false);

    }

    public void UINext()
    {
        battleButtons.SetActive(false);
        StayButton.SetActive(false);
    }

    public void UIInfo()
    {
        // 버튼?
        battleButtons.SetActive(false);
        StayButton.SetActive(false);

        // 순서 끄기
        turnUI.SetActive(false);

        iUI_Portrait.sprite = currentUnit.portrait;
        iUI_Portrait.preserveAspect = true;

        iUI_UnitName.text = currentUnit.unitName;
        iUI_Level.text = "Lv." + currentUnit.level;

        DrawBar(iUI_HP, currentUnit, Color.blue);
        DrawBar(iUI_ATK, currentUnit, Color.blue);
        DrawBar(iUI_DEF, currentUnit, Color.blue);
        DrawBar(iUI_FOC, currentUnit, Color.blue);
        DrawBar(iUI_SPD, currentUnit, Color.blue);

        iUI_MOV.sprite = currentUnit.movImage;
        iUI_RNG.sprite = currentUnit.rng;
        iUI_HTA.sprite = currentUnit.hta;

        infoUI.SetActive(true);
    }


    // 적군 선택 시
    public void UIEnemySelected()
    {

        // 비교창
        selectedEnemy = UnitManager.Instance.selectedEnemy.GetComponent<Unit>();
        eUI_allyPortrait.sprite = currentUnit.portrait;
        eUI_allyPortrait.preserveAspect = true;
        DrawBar(eUI_allyHP, currentUnit, Color.blue);
        DrawBar(eUI_allyATK, currentUnit, Color.blue);
        DrawBar(eUI_allyFOC, currentUnit, Color.blue);
        DrawBar(eUI_allySPD, currentUnit, Color.blue);
        eUI_enemyPortrait.sprite= selectedEnemy.portrait;
        eUI_enemyPortrait.preserveAspect = true;
        DrawBar(eUI_enemyHP, selectedEnemy, Color.red);
        DrawBar(eUI_enemyDEF, selectedEnemy, Color.red);
        DrawBar(eUI_enemyFOC, selectedEnemy, Color.red);
        DrawBar(eUI_enemySPD, selectedEnemy, Color.red);

        enemySelectedUI.SetActive(true);

        // 우하단
        RightUI();
    }

    // stat바 그리는 함수

    public void DrawBar(GameObject frame, Unit unit, Color color)
    {
        int maxStat = 0;
        int currentStat = 0;
        GameObject barImage = null;

        switch (frame.GetComponent<StatFrame>().statType)
        {
            case StatType.HP:
                maxStat = unit.unitSO.hp;
                currentStat = unit.hp;
                break;
            case StatType.ATK:
                maxStat = unit.unitSO.atk;
                currentStat = unit.atk;
                break;
            case StatType.DEF:
                maxStat = unit.unitSO.def;
                currentStat = unit.def;
                break;
            case StatType.FOC:
                maxStat = unit.unitSO.foc;
                currentStat = unit.foc;
                break;
            case StatType.SPD:
                maxStat = unit.unitSO.spd;
                currentStat = unit.spd;
                break;
        }

        switch (frame.GetComponent<StatFrame>().barType)
        {
            case BarType.big:
                barImage = bigBar;
                break;
            case BarType.small:
                barImage = smallBar;
                break;

        }

        HorizontalLayoutGroup stratch = frame.GetComponent<HorizontalLayoutGroup>();

        float frameLength = frame.GetComponent<RectTransform>().rect.width;
        RectTransform blockSize = barImage.GetComponent<RectTransform>();

        blockSize.sizeDelta = new Vector2(frameLength / 5, blockSize.rect.height);

        stratch.childControlWidth = false;
        stratch.childForceExpandWidth = false;

        if (Mathf.Max(maxStat, currentStat) > 5)
        {
            stratch.childControlWidth = true;
            stratch.childForceExpandWidth = true;
        }

        foreach (Transform child in frame.transform)
        {
            Destroy(child.gameObject);
        }

        barImage.GetComponent<Image>().color = color;


        int totalBars = Mathf.Max(currentStat, maxStat);

        foreach (Transform child in frame.transform) Destroy(child.gameObject);

        for (int i = 0; i < totalBars; i++)
        {
            GameObject bar = Instantiate(barImage, frame.transform);
            Image img = bar.GetComponent<Image>();

            if (i < maxStat && i < currentStat)
                img.color = color;
            else if (i >= currentStat)
                img.color = Color.gray;
            else
                img.color = Color.white;
        }

    }


// 좌하단 UI
public void LeftUI()
    {
        lUI_UnitName.text = selectedAlly.unitName;
        lUI_Level.text = "Lv." + selectedAlly.level;
        lUI_Portrait.sprite = selectedAlly.portrait;
        iUI_Portrait.preserveAspect = true;
        DrawBar(lUI_HP, selectedAlly, Color.green);

        leftUI.SetActive(true);
    }

    public void RightUI()
    {
        rUI_UnitName.text = selectedEnemy.unitName;
        rUI_Level.text = "Lv." + selectedEnemy.level;
        rUI_Portrait.sprite = selectedEnemy.portrait;
        rUI_Portrait.preserveAspect = true;
        DrawBar(rUI_HP, selectedEnemy, Color.green);

        rightUI.SetActive(true);
    }

    // 머리 위 UI
    public void HeadUI(Unit parentUnit, bool isPlayerTurn)
    {

        GameObject UI = null;
        GameObject portrait = null;
        GameObject hp = null;
        GameObject stat = null;

        foreach (Transform child in parentUnit.transform)
        {
            if (child.name == "BattleButtons" && child.gameObject.activeSelf)
            {
                return;
            }
            else if (child.name == "HeadUI")
            {
                UI = child.gameObject;
            }
        }

        //if (UI == null)
        //{
        //    UI = Instantiate(headUI, parentUnit.transform);
        //    UI.name = "HeadUI";
        //}

        UI.SetActive(true);

        foreach (Transform child in UI.transform)
        {
            if (child.name == "portrait")
            {
                portrait = child.gameObject;
            } else if (child.name == "hp")
            {
                hp = child.gameObject;

            } else if (child.name == "atk/def")
            {
                stat = child.gameObject;
            }
        }

        portrait.GetComponent<Image>().sprite = parentUnit.portrait;
        portrait.GetComponent<Image>().preserveAspect = true;


        if (portrait != null && hp != null && stat != null)
        {
            DrawBar(hp, parentUnit, Color.green);

            bool isAttacker = (isPlayerTurn && parentUnit.isAlly) || (!isPlayerTurn && !parentUnit.isAlly);

            if (isAttacker)
            {
                // 공격: 빨간색 ATK
                stat.GetComponent<StatFrame>().statType = StatType.ATK;
                DrawBar(stat, parentUnit, Color.red);
            }
            else
            {
                // 방어: 파란색 DEF
                stat.GetComponent<StatFrame>().statType = StatType.DEF;
                DrawBar(stat, parentUnit, Color.blue);
            }
        }
    }



    // 순서 UI
    public void UIturn(List<Unit> allUnits)
    {
        
        foreach (Transform child in turnUI.transform)
        {

            Destroy(child.gameObject);

        }

        // 지금 차례인 애 index 가져오기
        int num = UnitManager.Instance.selectedUnitIndex;
 

        for (int i = num; i < allUnits.Count; i++)
        {
            Createportrait(allUnits[i]);

        }

        for (int i = 0; i < num; i++)
        {
            Createportrait(allUnits[i]);
        }
    }

    public void Createportrait(Unit unit)
    {
        if(!unit.isAlly)
        {
            if (unit.GetComponent<EnemyAI>().target == null)
            {
                return;
            }

        }
        
        GameObject portrait = new GameObject("portrait");
        portrait.transform.SetParent(turnUI.transform, false);
        Image image = portrait.AddComponent<Image>();
        image.sprite = unit.portrait;
        image.preserveAspect = true;
        AspectRatioFitter size = portrait.AddComponent<AspectRatioFitter>();
        size.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
    }

    // 디버그 모드
    public void UIDebug()
    {
        // 오른쪽 리스트
        foreach (Transform child in turnUI.transform)
        {

            Destroy(child.gameObject);

        }

        foreach (GameObject obj in UnitManager.Instance.units.Values)
        {


            Sprite portrait = obj.GetComponent<Unit>().unitSO.portrait;
            string unitName = obj.GetComponent<Unit>().unitSO.unitName;

            GameObject Obj = new GameObject("Unit");
            Obj.transform.SetParent(turnUI.transform,false);


            Image Image = Obj.AddComponent<Image>();
            Button Button = Obj.AddComponent<Button>();

            Image.sprite = portrait;
            
            Button.onClick.AddListener(() =>
            {
                DataManager.Instance.selectedunit = unitName;
                Debug.Log(unitName+"이(가) 선택됨");

            });
        }

        // 토글
        IsAllyToggle.SetActive(true);

        ToggleIsAlly(IsAllyToggle.GetComponent<Toggle>().isOn);

    }

    public void ToggleIsAlly(bool isOn)
    {
        if (isOn)
        {
            DataManager.Instance.isAlly = true;
            Debug.Log(DataManager.Instance.isAlly);
        }
        else
        {
            DataManager.Instance.isAlly = false;
            Debug.Log(DataManager.Instance.isAlly);

        }
    }

    public void AccuracyText(Unit unit)
    {

    }

    public void create()
    {
        // 1. Canvas 존재 여부 확인 (가장 빈번한 원인)
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("UIManager: 씬에서 'Canvas'를 찾을 수 없습니다! 이름을 확인하세요.");
            return;
        }

        // 2. 프리팹 연결 여부 확인 (인스펙터 할당 실수 방지)
        if (leftUIP == null || rightUIP == null || infoUIP == null)
        {
            Debug.LogError("UIManager: 인스펙터에 프리팹(P)이 연결되지 않았습니다!");
            return;
        }

        foreach(Transform child in canvas.transform)
        {
            Destroy(child.gameObject);
        }
        leftUI = null; rightUI = null; infoUI = null; battleButtons = null; enemySelectedUI = null; StayButton = null; turnUI = null;


        battleButtons = Instantiate(battleButtonsP);
        combatButton = battleButtons.transform.Find("CombatButton").gameObject;
        if (combatButton.GetComponent<Button>() != null )
        {
            combatButton.GetComponent<Button>().onClick.RemoveAllListeners(); // 중복 연결 방지
            combatButton.GetComponent<Button>().onClick.AddListener(CombatButton); // 함수 연결
        }
        nextButton = battleButtons.transform.Find("NextButton").gameObject;
        if (nextButton.GetComponent<Button>() != null)
        {
            nextButton.GetComponent<Button>().onClick.RemoveAllListeners(); // 중복 연결 방지
            nextButton.GetComponent<Button>().onClick.AddListener(NextButton); // 함수 연결
        }
        infoButton = battleButtons.transform.Find("InfoButton").gameObject;
        if (infoButton.GetComponent<Button>() != null)
        {
            infoButton.GetComponent<Button>().onClick.RemoveAllListeners(); // 중복 연결 방지
            infoButton.GetComponent<Button>().onClick.AddListener(InfoButton); // 함수 연결
        }


        leftUI = Instantiate(leftUIP, canvas.transform);
        lUI_UnitName = leftUI.transform.Find("profile/lUI_UnitName").gameObject.GetComponent<TextMeshProUGUI>();
        lUI_Level = leftUI.transform.Find("profile/lUI_Level").gameObject.GetComponent<TextMeshProUGUI>();
        lUI_Portrait = leftUI.transform.Find("lUI_Portrait").gameObject.GetComponent<Image>();
        lUI_HP = leftUI.transform.Find("lUI_HP").gameObject;

        rightUI = Instantiate(rightUIP, canvas.transform);
        rUI_UnitName = rightUI.transform.Find("profile/rUI_UnitName").gameObject.GetComponent<TextMeshProUGUI>();
        rUI_Level = rightUI.transform.Find("profile/rUI_Level").gameObject.GetComponent<TextMeshProUGUI>();
        rUI_Portrait = rightUI.transform.Find("rUI_Portrait").gameObject.GetComponent<Image>();
        rUI_HP = rightUI.transform.Find("rUI_HP").gameObject;

        turnUI = Instantiate(turnUIP, canvas.transform);

        infoUI = Instantiate(infoUIP, canvas.transform);
        iUI_Portrait = infoUI.transform.Find("Image/profile/iUI_Portrait").gameObject.GetComponent<Image>();
        iUI_UnitName = infoUI.transform.Find("Image/profile/profile_text/iUI_UnitName").gameObject.GetComponent<TextMeshProUGUI>();
        iUI_Level = infoUI.transform.Find("Image/profile/profile_text/iUI_Level").gameObject.GetComponent <TextMeshProUGUI>();
        iUI_HP = infoUI.transform.Find("Image/HP/iUI_HP").gameObject;
        iUI_ATK = infoUI.transform.Find("Image/ATK/iUI_ATK").gameObject;
        iUI_DEF = infoUI.transform.Find("Image/DEF/iUI_DEF").gameObject;
        iUI_FOC = infoUI.transform.Find("Image/FOC/iUI_FOC").gameObject;
        iUI_SPD = infoUI.transform.Find("Image/SPD/iUI_SPD").gameObject;
        iUI_MOV = infoUI.transform.Find("Image/Image/MOV/iUI_A").gameObject.GetComponent<Image>();
        iUI_RNG = infoUI.transform.Find("Image/Image/RNG/iUI_B").gameObject.GetComponent<Image>();
        iUI_HTA = infoUI.transform.Find("Image/Image/HTA/iUI_C").gameObject.GetComponent<Image>();
        iUI_Button = infoUI.transform.Find("Button").gameObject;
        if (iUI_Button.GetComponent<Button>() != null)
        {
            iUI_Button.GetComponent<Button>().onClick.RemoveAllListeners(); // 중복 연결 방지
            iUI_Button.GetComponent<Button>().onClick.AddListener(DefaultButton); // 함수 연결
        }

        enemySelectedUI = Instantiate(enemySelectedUIP, canvas.transform);
        eUI_allyPortrait = enemySelectedUI.transform.Find("Portrait/Ally/eUI_allyPortrait").GetComponent<UnityEngine.UI.Image>();
        eUI_enemyPortrait = enemySelectedUI.transform.Find("Portrait/Enemy/eUI_enemyPortrait").GetComponent<UnityEngine.UI.Image>();
        eUI_allyHP = enemySelectedUI.transform.Find("SPD/Allay/eUI_allyHP").gameObject;
        eUI_allyATK = enemySelectedUI.transform.Find("SPD/Allay/eUI_allyATK").gameObject;
        eUI_allyFOC = enemySelectedUI.transform.Find("SPD/Allay/eUI_allyFOC").gameObject;
        eUI_allySPD = enemySelectedUI.transform.Find("SPD/Allay/eUI_allySPD").gameObject;
        eUI_enemyHP = enemySelectedUI.transform.Find("SPD/Enemy/eUI_enemyHP").gameObject;
        eUI_enemyDEF = enemySelectedUI.transform.Find("SPD/Enemy/eUI_enemyDEF").gameObject;
        eUI_enemyFOC = enemySelectedUI.transform.Find("SPD/Enemy/eUI_enemyFOC").gameObject;
        eUI_enemySPD = enemySelectedUI.transform.Find("SPD/Enemy/eUI_enemySPD").gameObject;

        StayButton = Instantiate(StayButtonP, canvas.transform);
        if (StayButton.GetComponent<Button>() != null)
        {
            StayButton.GetComponent<Button>().onClick.RemoveAllListeners(); // 중복 연결 방지
            StayButton.GetComponent<Button>().onClick.AddListener(DefaultButton); // 함수 연결
        }
    }

    // 버튼에 들어갈 함수
    public void CombatButton()
    {
        GameManager.Instance.UpdateBattleState(BattleState.Combat);
    }

    public void NextButton()
    {
        GameManager.Instance.UpdateBattleState(BattleState.Next);
    }

    public void InfoButton()
    {
        GameManager.Instance.UpdateBattleState(BattleState.Info);
    }

    public void DefaultButton()
    {
        TileMapManager.Instance.ClearTileMap(UnitManager.Instance.selectedUnit.accessibleTiles);
        GameManager.Instance.UpdateBattleState(BattleState.Default);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChange -= GameStateUI;
        GameManager.Instance.OnBattleStateChange -= BattleStateUI;
    }
}


