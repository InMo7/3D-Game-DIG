using UnityEngine;
using UnityEngine.Events;
using TMPro; // Required for TextMeshProUGUI

public class Level1Objective : MonoBehaviour
{
    [Header("Objective Settings")]
    [SerializeField] private string objectiveName = "Reach Location";
    [SerializeField] private string objectiveDescription = "Move to the designated area";
    [SerializeField] private float triggerRadius = .5f;
    [SerializeField] private bool autoCompleteOnEnter = true;

    [Header("Events")]
    public UnityEvent onObjectiveCompleted;

    // Static counters for tracking objectives
    private static int totalObjectives = 0;
    private static int completedObjectives = 0;
    private static TextMeshProUGUI objectiveText;

    private bool isCompleted = false;
    private Transform playerTransform;
    private SphereCollider triggerCollider;

    private void Awake()
    {
        // Get or add SphereCollider
        triggerCollider = GetComponent<SphereCollider>();
        if (triggerCollider == null)
        {
            triggerCollider = gameObject.AddComponent<SphereCollider>();
        }

        // Configure collider
        triggerCollider.isTrigger = true;
        triggerCollider.radius = triggerRadius;

        // Increment total objectives when this instance is created
        totalObjectives++;

        // Find the objective text UI element
        if (objectiveText == null)
        {
            GameObject textObj = GameObject.FindGameObjectWithTag("ObjectiveText");
            if (textObj != null)
            {
                objectiveText = textObj.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogWarning("Level1Objective: No GameObject with 'ObjectiveText' tag found for UI display!");
            }
        }

        // Update UI initially
        UpdateObjectiveUI();
    }

    private void Start()
    {
        // Find player - assuming player has tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Level1Objective: No GameObject with 'Player' tag found!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCompleted || playerTransform == null) return;

        if (other.transform == playerTransform)
        {
            if (autoCompleteOnEnter)
            {
                CompleteObjective();
            }
        }
    }

    public void SetTriggerRadius(float newRadius)
    {
        if (newRadius > 0)
        {
            triggerRadius = newRadius;
            if (triggerCollider != null)
            {
                triggerCollider.radius = newRadius;
            }
            Debug.Log($"Objective '{objectiveName}' trigger radius changed to {newRadius}");
        }
        else
        {
            Debug.LogWarning("Trigger radius must be greater than 0!");
        }
    }

    public void CompleteObjective()
    {
        if (!isCompleted)
        {
            isCompleted = true;
            completedObjectives++;
            onObjectiveCompleted.Invoke();
            Debug.Log($"Objective '{objectiveName}' completed!");
            UpdateObjectiveUI();
        }
    }

    public bool IsObjectiveCompleted()
    {
        return isCompleted;
    }

    public string GetObjectiveName()
    {
        return objectiveName;
    }

    public string GetObjectiveDescription()
    {
        return objectiveDescription;
    }

    private void UpdateObjectiveUI()
    {
        if (objectiveText != null)
        {
            objectiveText.text = $"Objectives: {completedObjectives}/{totalObjectives}";
        }
    }

    // Optional: Visualize the trigger radius in editor
    private void OnDrawGizmos()
    {
        Gizmos.color = isCompleted ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }

    // Reset counters when the scene is unloaded or game is reset
    private void OnDestroy()
    {
        totalObjectives--;
        if (isCompleted)
        {
            completedObjectives--;
        }
        UpdateObjectiveUI();
    }
}