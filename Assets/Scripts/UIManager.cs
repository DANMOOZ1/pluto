using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 추가해야 할 것
// 1. 배틀 default에서 현재 차례인 unit이 currentunit에 들어가게
// 2. HP의 경우 전체 HP랑 남은 HP 구분하기
// 3. 무식하게 매번 바를 그리는 방식인데, 다소 비효율적이라 미리 그려놓고 켜고 끄는 걸로 바꿔야 할 것 같아요 지금 당장은 제가 시간이 없어서 + 굴러가긴 해서 월~화 새벽에 바꿀 것 같습니다
// 4. 행마법이랑 공격 확정되면 그려보기
// 5. attack이랑 enemyturn에서는 HP가 바로바로 업데이트되어야 함 

public class UIManager : Singleton<UIManager>
{
    [Header("battleUI")]
    public GameObject battleUI;
    public TextMeshProUGUI bUI_UnitName;
    public TextMeshProUGUI bUI_Level;
    public Image bUI_Portrait;
    public GameObject bUI_HP;
    public GameObject bU_barImage;

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
    public GameObject eU_barImage;

    [Header("battleButtons")]
    public GameObject battleButtons;

    [Header("HeadUI")]
    public GameObject headUI;

    // unit 가져오기
    public Unit currentUnit;
    public List<Unit> allUnits;

    // 유닛 턴인가요?
    public bool isPlayerTurn;

    public void Start()
    {
        // 구독
        GameManager.Instance.OnBattleStateChange += BattleStateUI;

        // 모든 유닛 한 리스트에 저장
        allUnits = UnitManager.Instance.spdSortUnits;
    }


    // 상태별 UI 켜기
    public void BattleStateUI()
    {
        // 현재 차례인 unit 가져오기
        if (UnitManager.Instance.selectedUnit.isAlly)
        {
            currentUnit = UnitManager.Instance.selectedUnit;
        }

        // 창 다 지우기
        battleButtons.SetActive(false);
        infoUI.SetActive(false);
        enemySelectedUI.SetActive(false);

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
            case BattleState.EnemyTurn:
                UIEnemyTurn();
                break;
        }

        // 좌하단 UI 띄우기
        profileUI();

        // 머리 위 UI 띄우기
        foreach (Unit unit in allUnits)
        {
            HeadUI(unit, isPlayerTurn);

        }
    }

    public void UIChoice()
    {
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

        // 플레이어 턴
        isPlayerTurn = true;
    }

    public void UIMove()
    {

        // 적 선택 시 비교창
        if (UnitManager.Instance.selectedEnemy != null)
        {
            UIEnemySelected();
        }

        // 플레이어 턴
        isPlayerTurn = true;
    }

    public void UICombat()
    {

        foreach (Unit unit in allUnits)
        {
            HeadUI(unit, true);

        }

        // 플레이어 턴
        isPlayerTurn = true;

    }

    public void UINext()
    {
        // 플레이어 턴
        isPlayerTurn = true;
    }

    public void UIInfo()
    {
        iUI_Portrait.sprite = currentUnit.portrait;
        iUI_UnitName.text = currentUnit.unitName;
        iUI_Level.text = "Lv." + currentUnit.level;
        DrawBar(bU_barImage, Color.blue, currentUnit.hp, iUI_HP);
        DrawBar(bU_barImage, Color.blue, currentUnit.atk, iUI_ATK);
        DrawBar(bU_barImage, Color.blue, currentUnit.def, iUI_DEF);
        DrawBar(bU_barImage, Color.blue, currentUnit.foc, iUI_FOC);
        DrawBar(bU_barImage, Color.blue, currentUnit.spd, iUI_SPD);

        infoUI.SetActive(true);

        // 플레이어 턴
        isPlayerTurn = true;
    }

    public void UIEnemyTurn()
    {
        // 플레이어 턴
        isPlayerTurn = false;
    }


    // 
    public void UIEnemySelected()
    {   
        Unit selectedEnemy = UnitManager.Instance.selectedEnemy.GetComponent<Unit>();
        eUI_allyPortrait.sprite = currentUnit.portrait;
        DrawBar(eU_barImage, Color.blue, currentUnit.hp, eUI_allyHP);
        DrawBar(eU_barImage, Color.blue, currentUnit.atk, eUI_allyATK);
        DrawBar(eU_barImage, Color.blue, currentUnit.foc, eUI_allyFOC);
        DrawBar(eU_barImage, Color.blue, currentUnit.spd, eUI_allySPD);
        eUI_enemyPortrait.sprite= selectedEnemy.portrait;
        DrawBar(eU_barImage, Color.red, selectedEnemy.hp, eUI_enemyHP);
        DrawBar(eU_barImage, Color.red, selectedEnemy.def, eUI_enemyDEF);
        DrawBar(eU_barImage, Color.red, selectedEnemy.foc, eUI_enemyFOC);
        DrawBar(eU_barImage, Color.red, selectedEnemy.spd, eUI_enemySPD);

        enemySelectedUI.SetActive(true);
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

        eU_barImage.GetComponent<Image>().color = color;

        for (int i = 0; i < num; i++)
        {
            Instantiate(barImage, stat.transform);
        }
    }

    // 좌하단 UI
    public void profileUI()
    {
        bUI_UnitName.text = currentUnit.unitName;
        bUI_Level.text = "Lv." + currentUnit.level;
        bUI_Portrait.sprite = currentUnit.portrait;
        DrawBar(bU_barImage, Color.green, currentUnit.hp, bUI_HP);

        battleUI.SetActive(true);
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
        eU_barImage.GetComponent<Image>().color = Color.green;
        DrawBar(eU_barImage, Color.green, parentUnit.hp, hp);

        bool isAttacker = (isPlayerTurn && parentUnit.isAlly) || (!isPlayerTurn && !parentUnit.isAlly);

        if (isAttacker)
        {
            // 공격: 빨간색 ATK
            DrawBar(eU_barImage, Color.red, parentUnit.atk, stat);
        }
        else
        {
            // 방어: 파란색 DEF
            DrawBar(eU_barImage, Color.blue, parentUnit.def, stat);
        }
    }
}
