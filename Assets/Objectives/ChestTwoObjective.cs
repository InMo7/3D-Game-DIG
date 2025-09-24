using UnityEngine;
using TMPro;

public class ChestTwoObjective : MonoBehaviour
{
    public Transform player; // Player's transform
    public Transform Level2Chest; //  chest 2 GameObject
    public TextMeshProUGUI distanceText; // UI text for distance
    public float completionRadius = .5f; // Radius to consider objective complete
    private bool isComplete = false;

    void Update()
    {
        if (isComplete) return;

        // Calculate distance to chest
        float distance = Vector3.Distance(player.position, Level2Chest.position);

        // Update UI text
        distanceText.text = $"Distance to Second Chest: {distance:F1}m";

        // Check if player is within completion radius
        if (distance <= completionRadius)
        {
            isComplete = true;
            distanceText.text = "Completed";
            // Optionally trigger chest opening or reward
        }
    }
}