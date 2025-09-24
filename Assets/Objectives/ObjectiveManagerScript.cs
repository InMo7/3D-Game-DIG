using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public ChestOneObjective Objective1; // Reference to Forest Chest script
    public ChestTwoObjective Objective2; // Reference to Mountain Chest script
    public ChestThreeObjective Objective3; // Reference to Cave Chest script
    public TextMeshProUGUI Objective1Text; // Forest objective UI text
    public TextMeshProUGUI Objective2Text; // Mountain objective UI text
    public TextMeshProUGUI Objective3Text; // Cave objective UI text
    private bool allObjectivesComplete = false;

    void Update()
    {
        if (allObjectivesComplete) return;

        // Check if all objectives are complete
        if (Objective1.IsComplete && Objective2.IsComplete && Objective3.IsComplete)
        {
            allObjectivesComplete = true;
            // Override all text boxes with completion message
            Objective1Text.text = "All Objectives Complete!";
            Objective2Text.text = "";
            Objective3Text.text = "";
            // Optionally trigger additional events (e.g., level end, reward screen)
        }
    }
}