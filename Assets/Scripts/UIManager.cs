using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

// 추가해야 할 것
// 1. 배틀 default에서 현재 차례인 unit이 currentunit에 들어가게
// 2. HP의 경우 전체 HP랑 남은 HP 구분하기

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
    public List<GameObject> accessibleUnits;
    public Unit selectedEnemy;

    public void Start()
    {
        accessibleUnits = UnitManager.Instance.accessibleUnits;
        currentUnit = accessibleUnits[0].GetComponent<Unit>();        // 구현을 위해 임시로 지정
    }

    // 상태별 UI 켜기
    public void UIBattle()
    {
        // 버튼 띄우기
        battleButtons.transform.SetParent(currentUnit.transform);
        battleButtons.SetActive(true);

        // UI 띄우기
        bUI_UnitName.text = currentUnit.unitName;
        bUI_Level.text = "Lv." + currentUnit.level;
        bUI_Portrait.sprite = currentUnit.portrait;
        DrawBar(bU_barImage, currentUnit.hp, bUI_HP);

        battleUI.SetActive(true);

        // 머리 위 UI 띄우기
        foreach (GameObject accessibleUnit in accessibleUnits)
        {
            if (accessibleUnit != currentUnit.gameObject)
            {
                HeadUI(accessibleUnit, true);
            }
        }
    }

    public void UIMove()
    {
        // 버튼 없애기
        battleButtons.SetActive(false);

        foreach (GameObject accessibleUnit in accessibleUnits)
        {
            HeadUI(accessibleUnit, true);

        }
    }

    public void UICombat()
    {
        // 버튼 없애기
        battleButtons.SetActive(false);

        foreach (GameObject accessibleUnit in accessibleUnits)
        {
            HeadUI(accessibleUnit, true);

        }

    }

    public void UINext()
    {
        // 버튼 없애기
        battleButtons.SetActive(false);

        foreach (GameObject accessibleUnit in accessibleUnits)
        {
            HeadUI(accessibleUnit, true);

        }
    }

    public void UIInfo()
    {
        // 버튼 없애기
        battleButtons.SetActive(false);

        // UI 띄우기
        iUI_Portrait.sprite = currentUnit.portrait;
        iUI_UnitName.text = currentUnit.unitName;
        iUI_Level.text = "Lv." + currentUnit.level;
        DrawBar(bU_barImage, currentUnit.hp, iUI_HP);
        DrawBar(bU_barImage, currentUnit.atk, iUI_ATK);
        DrawBar(bU_barImage, currentUnit.def, iUI_DEF);
        DrawBar(bU_barImage, currentUnit.foc, iUI_FOC);
        DrawBar(bU_barImage, currentUnit.spd, iUI_SPD);

        infoUI.SetActive(true);
    }

    public void UIEnemyTurn()
    {
        foreach (GameObject accessibleUnit in accessibleUnits)
        {
            HeadUI(accessibleUnit, false);
        }
    }

    public void UIEnemySelected()
    {
        // UI 띄우기
        eUI_allyPortrait.sprite = currentUnit.portrait;
        eU_barImage.GetComponent<Image>().color = Color.blue;
        DrawBar(eU_barImage, currentUnit.hp, eUI_allyHP);
        DrawBar(eU_barImage, currentUnit.atk, eUI_allyATK);
        DrawBar(eU_barImage, currentUnit.foc, eUI_allyFOC);
        DrawBar(eU_barImage, currentUnit.spd, eUI_allySPD);
        eUI_enemyPortrait.sprite= selectedEnemy.portrait;
        eU_barImage.GetComponent<Image>().color = Color.red;
        DrawBar(eU_barImage, selectedEnemy.hp, eUI_enemyHP);
        DrawBar(eU_barImage, selectedEnemy.def, eUI_enemyDEF);
        DrawBar(eU_barImage, selectedEnemy.foc, eUI_enemyFOC);
        DrawBar(eU_barImage, selectedEnemy.spd, eUI_enemySPD);

        enemySelectedUI.SetActive(true);

        foreach (GameObject accessibleUnit in accessibleUnits)
        {
            HeadUI(accessibleUnit, true);
        }

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

    // 머리 위 UI
    public void HeadUI(GameObject accessibleUnit, bool isPlayerTurn)
    {
        foreach (Transform child in accessibleUnit.transform)
        {
            if(child.name == "HeadUI")
            {
                Destroy(child.gameObject);
            }
        }

        GameObject clonedHeadUI = Instantiate(headUI, accessibleUnit.transform);
        clonedHeadUI.name = "HeadUI";

        Unit unit = accessibleUnit.GetComponent<Unit>();

        GameObject portrait = clonedHeadUI.transform.GetChild(0).gameObject;
        GameObject hp = clonedHeadUI.transform.GetChild(1).gameObject;
        GameObject stat = clonedHeadUI.transform.GetChild(2).gameObject;

        portrait.GetComponent<Image>().sprite = unit.portrait;
        eU_barImage.GetComponent<Image>().color = Color.green;
        DrawBar(eU_barImage, unit.hp, hp);

        bool isAttacker = (isPlayerTurn && unit.isAlly) || (!isPlayerTurn && !unit.isAlly);

        if (isAttacker)
        {
            // 공격자: 빨간색 ATK
            eU_barImage.GetComponent<Image>().color = Color.red;
            DrawBar(eU_barImage, unit.atk, stat);
        }
        else
        {
            // 방어자: 파란색 DEF
            eU_barImage.GetComponent<Image>().color = Color.blue;
            DrawBar(eU_barImage, unit.def, stat);
        }
    }
}
