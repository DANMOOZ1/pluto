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

    // unit 가져오기
    private Unit currentUnit;
    private Unit selectedAlly;
    private Unit selectedEnemy;
    private List<Unit> allUnits;

    // 유닛 턴인가요?
    private bool isPlayerTurn;

    public void Start()
    {
        // 구독
        GameManager.Instance.OnBattleStateChange += BattleStateUI;

        // 모든 유닛 한 리스트에 저장 ** spdSortUnits에 현재 가용 가능한 모든 유닛이 있음(Unit 타입)
    }


    // 상태별 UI 켜기
    public void BattleStateUI()
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

            // 머리 위 UI 띄우기
            allUnits = UnitManager.Instance.spdSortUnits;
            foreach (Unit unit in allUnits)
            {
                HeadUI(unit, isPlayerTurn);
            }

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

        } else
        {
            selectedEnemy = currentUnit;

            // 우하단 UI 띄우기
            RightUI();
        }
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

        foreach (Unit unit in allUnits)
        {
            HeadUI(unit, true);

        }

    }

    public void UINext()
    {

    }

    public void UIInfo()
    {
        // 버튼?
        battleButtons.SetActive(false);

        iUI_Portrait.sprite = currentUnit.portrait;
        iUI_UnitName.text = currentUnit.unitName;
        iUI_Level.text = "Lv." + currentUnit.level;

        DrawBar(bigBar, Color.blue, currentUnit.hp, iUI_HP);
        DrawBar(bigBar, Color.blue, currentUnit.atk, iUI_ATK);
        DrawBar(bigBar, Color.blue, currentUnit.def, iUI_DEF);
        DrawBar(bigBar, Color.blue, currentUnit.foc, iUI_FOC);
        DrawBar(bigBar, Color.blue, currentUnit.spd, iUI_SPD);

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
        DrawBar(smallBar, Color.blue, currentUnit.hp, eUI_allyHP);
        DrawBar(smallBar, Color.blue, currentUnit.atk, eUI_allyATK);
        DrawBar(smallBar, Color.blue, currentUnit.foc, eUI_allyFOC);
        DrawBar(smallBar, Color.blue, currentUnit.spd, eUI_allySPD);
        eUI_enemyPortrait.sprite= selectedEnemy.portrait;
        DrawBar(smallBar, Color.red, selectedEnemy.hp, eUI_enemyHP);
        DrawBar(smallBar, Color.red, selectedEnemy.def, eUI_enemyDEF);
        DrawBar(smallBar, Color.red, selectedEnemy.foc, eUI_enemyFOC);
        DrawBar(smallBar, Color.red, selectedEnemy.spd, eUI_enemySPD);

        enemySelectedUI.SetActive(true);

        // 우하단
        RightUI();
    }

    // stat바 그리는 함수
    public void DrawBar(GameObject barImage, Color color, int num, GameObject stat)
    {
        
        HorizontalLayoutGroup stratch = stat.GetComponent<HorizontalLayoutGroup>();
        
        float parentLength = stat.GetComponent<RectTransform>().rect.width;
        RectTransform childSize = barImage.GetComponent<RectTransform>();

        childSize.sizeDelta = new Vector2(parentLength/5, childSize.rect.height);

        stratch.childControlWidth = false;
        stratch.childForceExpandWidth = false;

        if(num > 5)
        {
            stratch.childControlWidth = true;
            stratch.childForceExpandWidth = true;
        }

        foreach (Transform child in stat.transform)
        {
            Destroy(child.gameObject);
        }

        smallBar.GetComponent<Image>().color = color;

        for (int i = 0; i < num; i++)
        {
            Instantiate(barImage, stat.transform);
        }
    }

    // 좌하단 UI
    public void LeftUI()
    {
        lUI_UnitName.text = selectedAlly.unitName;
        lUI_Level.text = "Lv." + selectedAlly.level;
        lUI_Portrait.sprite = selectedAlly.portrait;
        DrawBar(bigBar, Color.green, selectedAlly.hp, lUI_HP);

        leftUI.SetActive(true);
    }

    public void RightUI()
    {
        rUI_UnitName.text = selectedEnemy.unitName;
        rUI_Level.text = "Lv." + selectedEnemy.level;
        rUI_Portrait.sprite = selectedEnemy.portrait;
        DrawBar(bigBar, Color.green, selectedEnemy.hp, rUI_HP);

        rightUI.SetActive(true);
    }

    // 머리 위 UI
    public void HeadUI(Unit parentUnit, bool isPlayerTurn)
    {

        foreach (Transform child in parentUnit.transform)
        {
            if(child.name == "HeadUI")
            {
                Destroy(child.gameObject);
            }
        }

        GameObject clonedHeadUI = Instantiate(headUI, parentUnit.transform);
        clonedHeadUI.name = "HeadUI";

        GameObject portrait = clonedHeadUI.transform.GetChild(0).gameObject;
        GameObject hp = clonedHeadUI.transform.GetChild(1).gameObject;
        GameObject stat = clonedHeadUI.transform.GetChild(2).gameObject;

        portrait.GetComponent<Image>().sprite = parentUnit.portrait;
        smallBar.GetComponent<Image>().color = Color.green;
        DrawBar(smallBar, Color.green, parentUnit.hp, hp);

        bool isAttacker = (isPlayerTurn && parentUnit.isAlly) || (!isPlayerTurn && !parentUnit.isAlly);

        if (isAttacker)
        {
            // 공격: 빨간색 ATK
            DrawBar(smallBar, Color.red, parentUnit.atk, stat);
        }
        else
        {
            // 방어: 파란색 DEF
            DrawBar(smallBar, Color.blue, parentUnit.def, stat);
        }
    }
}
