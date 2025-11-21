using System.Collections;
using UnityEngine;

public class csDestroyObject : MonoBehaviour
{
    [SerializeField] float destroyTime = 0.5f;
    [SerializeField] private Light pointLight;

    private void Start()
    {
        if (pointLight == null)
        {
            pointLight = GetComponent<Light>();
        }

        if (pointLight != null)
        {
            StartCoroutine(DimLightCoroutine());
        }

        StartCoroutine(DestroyCoroutine());
    }

    private IEnumerator DimLightCoroutine()
    {
        float startIntensity = pointLight.intensity;
        float elapsedTime = 0f;

        while (elapsedTime < destroyTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / destroyTime;

            // Плавно уменьшаем интенсивность от начального значения до 0
            pointLight.intensity = Mathf.Lerp(startIntensity, 0f, progress);

            yield return null;
        }

        pointLight.intensity = 0f;
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}