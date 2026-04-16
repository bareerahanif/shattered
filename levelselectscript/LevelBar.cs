using UnityEngine;
using UnityEngine.UI;

public class LevelBar : MonoBehaviour
{
    [Header("Level Info")]
    public int levelNumber;
    public int sceneIndex;

    [Header("Sprites")]
    public Sprite blackSprite;
    public Sprite colorSprite;

    [Header("Color")]
    public Color levelColor;

    [HideInInspector] public bool isUnlocked = false;

    // make these public so you can verify they're assigned in Inspector
    public Image img;
    public Button btn;

    void Awake()
    {
        img = GetComponent<Image>();
        btn = GetComponent<Button>();

        if (img == null) Debug.LogError("No Image found on " + gameObject.name);
        if (blackSprite == null) Debug.LogError("Black sprite missing on " + gameObject.name);
        if (colorSprite == null) Debug.LogError("Color sprite missing on " + gameObject.name);

        SetLocked();
    }

    public void SetLocked()
    {
        if (img == null) img = GetComponent<Image>();
        img.sprite = blackSprite;
        img.color = Color.black;
        btn.interactable = false;
    }

    public void SetUnlocked()
    {
        isUnlocked = true;
        if (img == null) img = GetComponent<Image>();
        img.sprite = colorSprite;
        img.color = levelColor;
        btn.interactable = true;
    }

    public void SetColor(Color c)
    {
        if (img == null) img = GetComponent<Image>();
        img.color = c;
    }

    public void img_SetSprite_Color()
    {
        if (img == null) img = GetComponent<Image>();
        img.sprite = colorSprite;
        img.color = Color.black;
    }
}
