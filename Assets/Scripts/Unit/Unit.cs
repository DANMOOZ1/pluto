using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(BoxCollider2D))]
public class Unit : MonoBehaviour
{
    public string unitName;
    public string race;
    public int hp;
    public int atk;
    public int def;
    public int wil;
    public int mnt;
    public int spd;
    public int foc;
    public int rng;
    public int hta;
    public Sprite sprite;

    private void OnMouseDown()
    {
        UIManager.Instance.unit = this;
    }
}

//웹훅되는지 확인용 아무말