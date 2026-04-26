using UnityEngine;

public class MeatballTester : MonoBehaviour
{
    void Update()
    {
        // Press M to start meatball rain
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("🔴 M KEY PRESSED - Checking MeatballManager... 🔴");

            if (MeatballManager.Instance != null)
            {
                Debug.Log("✅ MeatballManager found! Starting meatball rain...");
                MeatballManager.Instance.StartMeatballRain();
            }
            else
            {
                Debug.LogError("❌ MeatballManager.Instance is NULL! Make sure you have a MeatballManager in the scene! ❌");
            }
        }

        // Press N to stop
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (MeatballManager.Instance != null)
            {
                MeatballManager.Instance.StopMeatballRain();
            }
        }

        // Press B to check status
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (MeatballManager.Instance != null)
            {
                if (MeatballManager.Instance.IsRaining())
                {
                    float remaining = MeatballManager.Instance.GetRemainingTime();
                    Debug.Log($"🌧️ Meatball rain ACTIVE - {remaining:F1} seconds remaining 🌧️");
                }
                else
                {
                    Debug.Log("☀️ No meatball rain active ☀️");
                }
            }
        }
    }
}