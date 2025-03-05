using UnityEngine;
using TMPro;
using System.Collections;

public class BlinkingText : MonoBehaviour
{
    public TMP_Text startText;
    public float blinkInterval = 0.5f; 

    void Start()
    {
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            startText.enabled = !startText.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
