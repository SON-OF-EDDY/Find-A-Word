using System.Collections;
using UnityEngine;
using TMPro;

public class TextPulseEffect : MonoBehaviour
{
    public TextMeshProUGUI textToPulse; // Assign your TextMeshPro element in the inspector
    public float minFontSize = 30f;      // Minimum font size for the pulse
    public float maxFontSize = 40f;      // Maximum font size for the pulse
    public float pulseDuration = 1.0f;   // Duration of one pulse cycle (increase + decrease)

    private bool pulsing = true;

    void Start()
    {
        if (textToPulse != null)
        {
            StartCoroutine(PulseText());
        }
    }

    IEnumerator PulseText()
    {
        while (pulsing)
        {
            // Increase text size
            yield return StartCoroutine(ChangeFontSize(minFontSize, maxFontSize, pulseDuration / 2));

            // Decrease text size
            yield return StartCoroutine(ChangeFontSize(maxFontSize, minFontSize, pulseDuration / 2));
        }
    }

    IEnumerator ChangeFontSize(float startSize, float endSize, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            textToPulse.fontSize = Mathf.Lerp(startSize, endSize, t);
            yield return null;
        }

        textToPulse.fontSize = endSize;
    }
}

