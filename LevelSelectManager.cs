using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Level Bars — assign in order 1 to 4")]
    public LevelBar[] bars;

    [Header("Transition")]
    public Image loadingBar;        // the colored fill bar
    public Image loadingBarBG;      // dark background bar
    public Image fadeOverlay;       // full screen black overlay

    public float totalDelay = 5f;    // total wait before scene loads
    public float fadeStartAt = 3.5f;  // when fade to black begins (within the 5s)

    // Animation timings
    private const float UNLOCK_FLASH_DELAY = 0.15f;
    private const float COLOR_FADE_DURATION = 2f;
    private const float BOUNCE_DURATION = 0.75f;

    private int levelsUnlocked;
    private int currentLevel;

    void Start()
    {

        // TEMP — change this number to test different levels (remove before shipping)
        //PlayerPrefs.SetInt("LevelsUnlocked", 3); // change to 2, 3, or 4 to test

        levelsUnlocked = PlayerPrefs.GetInt("LevelsUnlocked", 1);
        currentLevel = levelsUnlocked;

        // Hide transition elements at start
        if (loadingBar) loadingBar.gameObject.SetActive(false);
        if (loadingBarBG) loadingBarBG.gameObject.SetActive(false);
        if (fadeOverlay) SetFadeAlpha(0f);

        foreach (var bar in bars)
            bar.SetLocked();

        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(1f);  // was 0.3f

        // flash completed levels
        for (int i = 0; i < levelsUnlocked - 1; i++)
        {
            bars[i].SetUnlocked();
            yield return new WaitForSeconds(UNLOCK_FLASH_DELAY);
        }

        yield return new WaitForSeconds(0.5f);  // was 0.2f

        LevelBar current = bars[currentLevel - 1];
        yield return StartCoroutine(FadeToColor(current));
        yield return StartCoroutine(BounceBar(current.transform));
        yield return StartCoroutine(TransitionSequence(current.levelColor));

        SceneManager.LoadScene(current.sceneIndex);
    }

    IEnumerator TransitionSequence(Color barColor)
    {
        // Show loading bar
        loadingBarBG.gameObject.SetActive(true);
        loadingBar.gameObject.SetActive(true);
        loadingBar.color = barColor;

        // Store original loading bar width
        RectTransform rt = loadingBar.rectTransform;
        RectTransform rtBG = loadingBarBG.rectTransform;
        float fullWidth = rtBG.rect.width;

        // Set pivot to left so it grows left → right
        rt.pivot = new Vector2(0f, 0.5f);
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(0f, 0f);

        float elapsed = 0f;
        bool fadingStarted = false;

        while (elapsed < totalDelay)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / totalDelay);

            // Fill loading bar
            rt.sizeDelta = new Vector2(fullWidth * progress, rt.sizeDelta.y);

            // Start fade when we hit fadeStartAt
            if (elapsed >= fadeStartAt)
            {
                if (!fadingStarted)
                {
                    fadingStarted = true;
                    fadeOverlay.gameObject.SetActive(true);
                }

                float fadeProgress = Mathf.Clamp01(
                    (elapsed - fadeStartAt) / (totalDelay - fadeStartAt));
                SetFadeAlpha(fadeProgress);
            }

            yield return null;
        }

        // Make sure we end fully faded and bar full
        rt.sizeDelta = new Vector2(fullWidth, rt.sizeDelta.y);
        SetFadeAlpha(1f);

        yield return new WaitForSeconds(0.1f);
    }

    void SetFadeAlpha(float alpha)
    {
        if (fadeOverlay == null) return;
        Color c = fadeOverlay.color;
        c.a = alpha;
        fadeOverlay.color = c;
    }

    // ── Utilities ─────────────────────────────────────────────────────────

    IEnumerator FadeToColor(LevelBar bar)
    {
        bar.img_SetSprite_Color();
        float elapsed = 0f;

        // Use pure black as start, pure level color as end
        Color startColor = Color.black;
        Color endColor = bar.levelColor;

        while (elapsed < COLOR_FADE_DURATION)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / COLOR_FADE_DURATION);
            // Use smooth step for nicer easing
            float smooth = t * t * (3f - 2f * t);
            bar.SetColor(Color.Lerp(startColor, endColor, smooth));
            yield return null;
        }

        bar.SetColor(endColor);
        bar.isUnlocked = true;
    }

    IEnumerator BounceBar(Transform t)
    {
        Vector3 original = t.localScale;
        Vector3 squish = original * 0.85f;
        float half = BOUNCE_DURATION / 2f;
        float elapsed = 0f;

        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            t.localScale = Vector3.Lerp(original, squish, elapsed / half);
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            t.localScale = Vector3.Lerp(squish, original, elapsed / half);
            yield return null;
        }
        t.localScale = original;
    }

    public void OnBarClicked(int levelNumber)
    {
        StopAllCoroutines();
        StartCoroutine(ClickAndLoad(levelNumber));
    }

    IEnumerator ClickAndLoad(int levelNumber)
    {
        yield return StartCoroutine(BounceBar(bars[levelNumber - 1].transform));
        yield return StartCoroutine(
            TransitionSequence(bars[levelNumber - 1].levelColor));
        SceneManager.LoadScene(bars[levelNumber - 1].sceneIndex);
    }
}