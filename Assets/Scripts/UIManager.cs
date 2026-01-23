using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 추가해야 할 것
// 1. HP의 경우 전체 HP랑 남은 HP 구분하기
// 2. 무식하게 매번 바를 그리는 방식인데, 다소 비효율적이라 미리 그려놓고 켜고 끄는 걸로 바꿔야 할 것 같아요 지금 당장은 제가 시간이 없어서 + 굴러가긴 해서 월~화 새벽에 바꿀 것 같습니다

public class UIManager : Singleton<UIManager>
{
    [Header("leftUI")]
    public GameObject leftUI;
    public TextMeshProUGUI lUI_UnitName;
    public TextMeshProUGUI lUI_Level;
    public Image lUI_Portrait;
    public GameObject lUI_HP;

    [Header("infoUI")]
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

    [Header("enemySelectedUI")]
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
    public GameObject rightUI;
    public TextMeshProUGUI rUI_UnitName;
    public TextMeshProUGUI rUI_Level;
    public Image rUI_Portrait;
    public GameObject rUI_HP;

    [Header("battleButtons")]
    public GameObject battleButtons;
    public GameObject combatButton;

    [Header("HeadUI")]
    public GameObject headUI;

    [Header("barImage")]
    public GameObject bigBar;
    public GameObject smallBar;

    [Header("turnUI")]
    public GameObject turnUI;

    // 가져올 unit turn 만들기
    private Unit currentUnit;
    private Unit selectedAlly;
    private Unit selectedEnemy;
    //private List<Unit> allUnits;

    // 유닛 턴인가요?
    private bool isPlayerTurn;

    public void Start()
    {
        // 구독
        GameManager.Instance.OnBattleStateChange += BattleStateUI;
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
        List<Unit> allUnits = UnitManager.Instance.spdSortUnits; // 쓸 때 가져오기

        foreach (Unit unit in allUnits)
        {
            HeadUI(unit, isPlayerTurn);
        }
        turnUI.SetActive(true);

        // 순서 띄우기
        UIturn(allUnits);
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
                Destroy(child.gameObject);
            }
        }
        battleButtons.transform.SetParent(currentUnit.transform, false);
        battleButtons.SetActive(true);

    }

    public void UIMove()
    {

        // 버튼?
        battleButtons.SetActive(false);

        // 적 선택 시 비교창
        if (UnitManager.Instance.selectedEnemy != null)
        {
            UIEnemySelected();
        }

    }

    public void UICombat()
    {
        // 버튼?
        battleButtons.SetActive(false);

    }

    public void UINext()
    {
        battleButtons.SetActive(false);

    }

    public void UIInfo()
    {
        // 버튼?
        battleButtons.SetActive(false);

        // 순서 끄기
        turnUI.SetActive(false);

        iUI_Portrait.sprite = currentUnit.portrait;
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
        DrawBar(eUI_allyHP, currentUnit, Color.blue);
        DrawBar(eUI_allyATK, currentUnit, Color.blue);
        DrawBar(eUI_allyFOC, currentUnit, Color.blue);
        DrawBar(eUI_allySPD, currentUnit, Color.blue);
        eUI_enemyPortrait.sprite= selectedEnemy.portrait;
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
        DrawBar(lUI_HP, selectedAlly, Color.green);

        leftUI.SetActive(true);
    }

    public void RightUI()
    {
        rUI_UnitName.text = selectedEnemy.unitName;
        rUI_Level.text = "Lv." + selectedEnemy.level;
        rUI_Portrait.sprite = selectedEnemy.portrait;
        DrawBar(rUI_HP, selectedEnemy, Color.green);

        rightUI.SetActive(true);
    }

    // 머리 위 UI
    public void HeadUI(Unit parentUnit, bool isPlayerTurn)
    {

        foreach (Transform child in parentUnit.transform)
        {
            if (child.name == "BattleButtons" && child.gameObject.activeSelf)
            {
                return;
            }
            else if (child.name == "HeadUI")
            {
                Destroy(child.gameObject);
            }
        }

        GameObject clonedHeadUI = Instantiate(headUI, parentUnit.transform);
        clonedHeadUI.name = "HeadUI";

        GameObject portrait = null;
        GameObject hp = null;
        GameObject stat = null;


        foreach (Transform child in clonedHeadUI.transform)
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
            Unit target = unit.GetComponent<EnemyAI>().FindNearestTarget();
            if (target == null)
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


}
