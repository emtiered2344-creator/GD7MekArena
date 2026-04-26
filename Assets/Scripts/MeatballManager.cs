using UnityEngine;
using System.Collections;

public class MeatballManager : MonoBehaviour
{
    public static MeatballManager Instance;

    [Header("Meatball Rain Settings")]
    public GameObject meatballPrefab;
    public float rainDuration = 8f;
    public float spawnInterval = 0.4f;
    public float spawnWidth = 25f;
    public float spawnHeight = 25f;
    public float spawnYPosition = 20f;

    [Header("Warning Settings")]
    public GameObject warningIndicatorPrefab;
    public float warningDuration = 1f;
    public Color warningColor = Color.red;

    [Header("Debug Settings")]
    public bool showDebugMessages = true;

    private bool isRaining = false;
    private int meatballCount = 0;
    private float rainStartTime = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("MeatballManager initialized!");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (isRaining && showDebugMessages)
        {
            float timeRemaining = rainDuration - (Time.time - rainStartTime);
            if (timeRemaining > 0 && Time.frameCount % 60 == 0)
            {
                Debug.Log($"Meatball Rain Active - Time Remaining: {timeRemaining:F1} seconds | Meatballs spawned: {meatballCount}");
            }
        }
    }

    public void StartMeatballRain()
    {
        if (!isRaining)
        {
            Debug.Log("StartMeatballRain() called!");
            StartCoroutine(MeatballRainCoroutine());
        }
        else
        {
            Debug.Log("Meatball rain is already active!");
        }
    }

    public void StopMeatballRain()
    {
        if (isRaining)
        {
            StopAllCoroutines();
            isRaining = false;
            Debug.Log("Meatball rain stopped early!");
        }
    }

    public bool IsRaining()
    {
        return isRaining;
    }

    public float GetRemainingTime()
    {
        if (isRaining)
        {
            return rainDuration - (Time.time - rainStartTime);
        }
        return 0f;
    }

    IEnumerator MeatballRainCoroutine()
    {
        isRaining = true;
        meatballCount = 0;
        rainStartTime = Time.time;

        Debug.Log("========================================");
        Debug.Log(" **MEATBALL RAIN STARTED!**");
        Debug.Log($"⏱Duration: {rainDuration} seconds");
        Debug.Log($"Spawn interval: {spawnInterval} seconds");
        Debug.Log("========================================");

        float endTime = Time.time + rainDuration;

        while (Time.time < endTime)
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(-spawnWidth, spawnWidth),
                spawnYPosition,
                Random.Range(-spawnHeight, spawnHeight)
            );

            Vector3 groundPosition = new Vector3(spawnPosition.x, 0.1f, spawnPosition.z);

            if (warningIndicatorPrefab != null)
            {
                GameObject warning = Instantiate(warningIndicatorPrefab, groundPosition, Quaternion.identity);
                Destroy(warning, warningDuration);
            }

            // DIRECT SPAWN - NO DELAY (for testing)
            if (meatballPrefab != null)
            {
                GameObject meatball = Instantiate(meatballPrefab, spawnPosition, Quaternion.identity);
                float randomScale = Random.Range(0.8f, 1.5f);
                meatball.transform.localScale = Vector3.one * randomScale;

                Meatball meatballScript = meatball.GetComponent<Meatball>();
                if (meatballScript != null)
                {
                    meatballScript.damage = 20f * randomScale;
                }

                meatballCount++;
                Debug.Log($"Meatball #{meatballCount} spawned! Will self-destruct in 10 seconds");
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log($"**MEATBALL RAIN ENDED!** ({meatballCount} total)");
        isRaining = false;
    }
}