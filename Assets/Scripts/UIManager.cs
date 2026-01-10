using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject swBackground;
    public Canvas canvas;
    public Unit unit; // unit¿ª πﬁ±‚
    public 

    void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        manageStatWindow();
    }

    public void DrawStatWindow(Unit unit)
    {
        swBackground.SetActive(true);
        
        GameObject swName = new GameObject("swName");
        swName.transform.SetParent(swBackground.transform, false);
        Text swNametext = swName.AddComponent<Text>();
        swNametext.text = unit.unitName;
        swNametext.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        swNametext.color = Color.black;
        RectTransform rt = swName.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 50);
        swNametext.fontSize = 24;
        swNametext.alignment = TextAnchor.MiddleCenter;
    }

    public void CloseStatWindow()
    {
        foreach (Transform child in swBackground.transform)
        {
            Destroy(child.gameObject);
        }
        swBackground.SetActive(false);
    }

    private void manageStatWindow()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            GameObject clickedUI = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            if (clickedUI != null && clickedUI.CompareTag("StatusWindow")) return;
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit.collider != null)
        {
            Unit clickedUnit = hit.collider.GetComponent<Unit>();
            if (clickedUnit != null)
            {
                CloseStatWindow();
                DrawStatWindow(clickedUnit);
            }
            else
            {
                CloseStatWindow();
            }
        }
        else
        {
            CloseStatWindow();
        }
    }
}
