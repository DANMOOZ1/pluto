using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Text.RegularExpressions;
using UnityEditor;

// 추가해야 할 것
// 1. 배틀 default에서 현재 차례인 unit 자동으로 unit에 넣기
// 2. OnBattleStateChange에 현재 차례인 unit을 currentunits[0]에 넣는 함수 더하기
// 3. 현재 차례인 unit 머리 위로 버튼 위치 옮기기
// 4. HP의 경우 전체 HP랑 남은 HP 구분하기

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


    [Header("combatButton")]
    public GameObject combatButton;

    // unit 가져오기
    public Unit unit;

    public List<Unit> currentunits = new List<Unit>();


    public void UIBattle()
    {
        // 버튼 띄우기
        combatButton.SetActive(true);

        if (unit == null) return;

        // 출력할 ally 저장
        if (unit.isAlly)
        {
            currentunits.Clear();
            currentunits.Add(unit);
        }

        // UI 띄우기
        bUI_UnitName.text = currentunits[0].unitName;
        bUI_Level.text = "Lv." + currentunits[0].level;
        bUI_Portrait.sprite = currentunits[0].portrait;
        DrawBar(bU_barImage, currentunits[0].hp, bUI_HP);

        battleUI.SetActive(true);
    }

    public void UIMove()
    {
        // 버튼 없애기
        combatButton.SetActive(false);
    }

    public void UICombat()
    {
        // 버튼 없애기
        combatButton.SetActive(false);

    }

    public void UINext()
    {
        // 버튼 없애기
        combatButton.SetActive(false);
    }

    public void UIInfo()
    {
        // 버튼 없애기
        combatButton.SetActive(false);

        // UI 띄우기
        iUI_Portrait.sprite = currentunits[0].sprite;
        iUI_UnitName.text = currentunits[0].unitName;
        iUI_Level.text = "Lv." + currentunits[0].level;
        DrawBar(bU_barImage, currentunits[0].hp, iUI_HP);
        DrawBar(bU_barImage, currentunits[0].atk, iUI_ATK);
        DrawBar(bU_barImage, currentunits[0].def, iUI_DEF);
        DrawBar(bU_barImage, currentunits[0].foc, iUI_FOC);
        DrawBar(bU_barImage, currentunits[0].spd, iUI_SPD);

        infoUI.SetActive(true);
    }

    public void UIEnemySelected()
    {
        // 출력할 enemy 저장
        if(!unit.isAlly)
        {
            if (currentunits.Count > 1)
            {
                currentunits[1] = unit;
            }
            else
            {
                currentunits.Add(unit);
            }
        }

        // UI 띄우기
        eUI_allyPortrait.sprite = currentunits[0].sprite;
        DrawBar(eU_barImage, currentunits[0].hp, eUI_allyHP);
        DrawBar(eU_barImage, currentunits[0].atk, eUI_allyATK);
        DrawBar(eU_barImage, currentunits[0].foc, eUI_allyFOC);
        DrawBar(eU_barImage, currentunits[0].spd, eUI_allySPD);
        eUI_enemyPortrait.sprite= currentunits[1].sprite;
        DrawBar(eU_barImage, currentunits[1].hp, eUI_enemyHP);
        DrawBar(eU_barImage, currentunits[1].def, eUI_enemyDEF);
        DrawBar(eU_barImage, currentunits[1].foc, eUI_enemyFOC);
        DrawBar(eU_barImage, currentunits[1].spd, eUI_enemySPD);

        enemySelectedUI.SetActive(true);

    }
    

    // 버튼에 들어갈 함수
    public void MoveButton()
    {
        GameManager.Instance.UpdateBattleState(BattleState.Move);
    }

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


    // stat바 그리는 함수
    public void DrawBar(GameObject barImage, int num, GameObject stat)
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

        for (int i = 0; i < num; i++)
        {
            Instantiate(barImage, stat.transform);
        }
    }
    
}
