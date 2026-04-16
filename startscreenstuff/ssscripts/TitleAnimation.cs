using UnityEngine;
using TMPro;

public class TitleAnimation : MonoBehaviour
{
    [Header("Wave")]
    public float waveAmplitude = 12f;      // how high each letter bobs
    public float waveSpeed = 2f;       // how fast the wave rolls
    public float waveSpread = 0.4f;     // phase gap between letters

    [Header("Shimmer")]
    public Color baseColor = new Color(0.96f, 0.77f, 0.09f);  // gold
    public Color shimmerColor = new Color(1f, 1f, 0.75f);   // bright gold
    public float shimmerSpeed = 1.8f;
    public float shimmerSpread = 0.5f;

    [Header("Glitch")]
    public float glitchChance = 0.004f;   // probability per letter per frame
    public float glitchAmount = 18f;      // max pixel jump during glitch

    // ── internals ──────────────────────────────────────────────────────────
    private TextMeshProUGUI tmp;

    // per-letter glitch state
    private float[] glitchTimer;
    private Vector3[] glitchOffset;
    private const float GLITCH_DURATION = 0.05f;   // how long a glitch lasts

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        tmp.ForceMeshUpdate();
        InitGlitch();
    }

    void InitGlitch()
    {
        int count = tmp.textInfo.characterCount;
        glitchTimer = new float[count];
        glitchOffset = new Vector3[count];
    }

    void Update()
    {
        tmp.ForceMeshUpdate();
        TMP_TextInfo textInfo = tmp.textInfo;

        // reinit arrays if character count changed
        if (glitchTimer == null || glitchTimer.Length != textInfo.characterCount)
            InitGlitch();

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int matIndex = charInfo.materialReferenceIndex;
            int vertIndex = charInfo.vertexIndex;

            Vector3[] verts = textInfo.meshInfo[matIndex].vertices;
            Color32[] colors = textInfo.meshInfo[matIndex].colors32;

            // ── Wave ────────────────────────────────────────────────────
            float wave = Mathf.Sin(Time.time * waveSpeed - i * waveSpread) * waveAmplitude;
            Vector3 waveOffset = new Vector3(0, wave, 0);

            // ── Glitch ──────────────────────────────────────────────────
            if (glitchTimer[i] > 0)
            {
                glitchTimer[i] -= Time.deltaTime;
                if (glitchTimer[i] <= 0)
                    glitchOffset[i] = Vector3.zero;   // snap back
            }
            else if (Random.value < glitchChance)
            {
                // trigger a new glitch
                glitchTimer[i] = GLITCH_DURATION;
                glitchOffset[i] = new Vector3(
                    Random.Range(-glitchAmount, glitchAmount),
                    Random.Range(-glitchAmount, glitchAmount),
                    0f);
            }

            Vector3 totalOffset = waveOffset + glitchOffset[i];

            verts[vertIndex + 0] += totalOffset;
            verts[vertIndex + 1] += totalOffset;
            verts[vertIndex + 2] += totalOffset;
            verts[vertIndex + 3] += totalOffset;

            // ── Shimmer ─────────────────────────────────────────────────
            float phase = Mathf.Sin(Time.time * shimmerSpeed - i * shimmerSpread);
            float t = (phase + 1f) / 2f;
            Color c = Color.Lerp(baseColor, shimmerColor, t);

            // Extra: briefly flash white during a glitch
            if (glitchTimer[i] > 0)
                c = Color.Lerp(c, Color.white, 0.6f);

            colors[vertIndex + 0] = c;
            colors[vertIndex + 1] = c;
            colors[vertIndex + 2] = c;
            colors[vertIndex + 3] = c;
        }

        // Push changes to mesh
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
            tmp.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
