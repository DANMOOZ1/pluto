using UnityEngine;

[CreateAssetMenu]
public class ClassData : ScriptableObject
{
    public ClassTag classTag;

    [Header("Class Stats")]
    public float class_HP;
    public float class_SPD;
    public float class_ATK;
    public float class_DEF;
    public float class_WIL;
    public float class_MNT;
    public float class_FOC;
    public float class_RNG;
    public float class_HIA;
}
