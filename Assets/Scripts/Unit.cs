using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Characteristic")]
    public RaceTag myRace; // 인스펙터에서 종족 선택
    public ClassData myClass;

    //유닛 고유 스탯
    [Header("Unit Stats")]
    public float unit_HP;
    public float unit_SPD;
    public float unit_ATK;
    public float unit_DEF;
    public float unit_WIL;
    public float unit_MNT;
    public float unit_FOC;
    public float unit_RNG;
    public float unit_HIA;

    //직업 보정치 적용한 스탯
    [Header("Total Stats")]
    private float total_HP;
    private float total_SPD;
    private float total_ATK;
    private float total_DEF;
    private float total_WIL;
    private float total_MNT;
    private float total_FOC;
    private float total_RNG;
    private float total_HIA;

    //아마 전투 때 쓰게 될..?지 안 될지 모르겠는 현재 스탯
    [Header("Current Stats")]
    private float cur_HP;
    private float cur_SPD;
    private float cur_ATK;
    private float cur_DEF;
    private float cur_WIL;
    private float cur_MNT;
    private float cur_FOC;
    private float cur_RNG;
    private float cur_HIA;

    void Start()
    {
        // 칸으로 표현하게 되면 아마 수치를 더하는 형식일 것 같아서~~
        total_HP = unit_HP + myClass.class_HP;
        total_SPD = unit_SPD + myClass.class_SPD;
        total_ATK = unit_ATK + myClass.class_ATK;
        total_DEF = unit_DEF + myClass.class_DEF;
        total_WIL = unit_WIL + myClass.class_WIL;
        total_MNT = unit_MNT + myClass.class_MNT;
        total_FOC = unit_FOC + myClass.class_FOC;
        total_RNG = unit_RNG + myClass.class_RNG;
        total_HIA = unit_HIA + myClass.class_HIA;

        total_HP = cur_HP;
        total_SPD = cur_SPD;
        total_ATK = cur_ATK;
        total_DEF = cur_DEF;
        total_WIL = cur_WIL;
        total_MNT= cur_MNT;
        total_FOC = cur_FOC;
        total_RNG = cur_RNG;
        total_HIA = cur_HIA;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
