using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public List<GameObject> fishPrefabs;
    public Transform fishSpawnPoint;
    public Button yesButton;
    public Button noButton;
    public GameObject endFishDialogPanel; // The panel to show

    private int currentTurn = 0;
    private List<GameObject> spawnedFish = new List<GameObject>();
    private List<GameObject> acceptedFish = new List<GameObject>(); // List of accepted fish
    private List<GameObject> rejectedFish = new List<GameObject>(); // List of rejected fish


    private int money = 100;
    private int happiness = 40;
    private int harmony = 30;

    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text happinessText;
    [SerializeField] private TMP_Text harmonyText;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        yesButton.onClick.AddListener(OnYesButtonClick); // Call OnYesButtonClick
        noButton.onClick.AddListener(OnNoButtonClick);   // Call OnNoButtonClick

        // Initially hide the panel
        if (endFishDialogPanel != null)
        {
            endFishDialogPanel.SetActive(false);
        }
        UpdateUI();
        StartNewTurn();
    }

    private void ApplyAcceptedFishEffects(FishScript fishScript)
    {

        money += fishScript.moneyEffect; // Using the EFFECT
        happiness += fishScript.happinessEffect; // Using the EFFECT
        harmony += fishScript.harmonyEffect; // Using the EFFECT

        UpdateUI();
    }

    private void ApplyNegativeFishEffects(FishScript fishScript)
    {

        money += fishScript.rejectedMoneyEffect; // Using the EFFECT
        happiness += fishScript.rejectedHarmonyEffect; // Using the EFFECT
        harmony += fishScript.rejectedHappinessEffect; // Using the EFFECT

        UpdateUI();
    }


    // Function to just update the UI with the current values
    private void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "Bubbles: " + money;
        }
        else
        {
            Debug.LogError("Money TextMeshPro object is not assigned!");
        }

        if (happinessText != null)
        {
            happinessText.text = "Happiness: " + happiness + "%";
        }
        else
        {
            Debug.LogError("Happiness TextMeshPro object is not assigned!");
        }

        if (harmonyText != null)
        {
            harmonyText.text = "Harmony: " + harmony;
        }
        else
        {
            Debug.LogError("harmony TextMeshPro object is not assigned!");
        }
    }

    // Functions to add to the values and update the UI
    public void AddResources(int amount)
    {
        money += amount;
        UpdateUI();
    }

    public void AddHappiness(int amount)
    {
        happiness += amount;
        UpdateUI();
    }

    public void AddHarmony(int amount)
    {
        harmony += amount;
        UpdateUI();
    }

    public void StartNewTurn()
    {
        currentTurn++;
        Debug.Log("Starting Turn: " + currentTurn);


        if (fishPrefabs.Count > 0)
        {
            GameObject newFish = Instantiate(fishPrefabs[0], fishSpawnPoint.position, fishSpawnPoint.rotation);
            spawnedFish.Add(newFish);
            // Remove the first prefab from the list
            fishPrefabs.RemoveAt(0);
        }
        else
        {

            return;
        }

    }

    public void ApplyTwistEffect(int effectValue, string effectType, string description)
    {
        switch (effectType.ToLower())
        {
            case "resources":
                money += effectValue;
                break;

            case "happiness":
                happiness += effectValue;
                break;

            case "population":
                harmony += effectValue;
                break;

            default:
                Debug.LogWarning("Unknown effect type.");
                return;
        }

        Debug.Log($"Twist Effect Applied: {description} ({effectValue} {effectType}).");
        UpdateUI();
    }

    public void EndTurn()
    {
        Debug.Log("Ending Turn: " + currentTurn);
        CheckFishConsequencesAccepted();
        StartNewTurn();
    }

    public void EndFishDialog()
    {
        if (endFishDialogPanel != null)
        {
            endFishDialogPanel.SetActive(true);
        }
    }

    // New functions to handle button clicks on the dialog
    public void OnYesButtonClick()
    {
        Debug.Log("Yes button clicked.");
        DialogManager.Instance.OnDialogFinished += HideEndFishDialogYes; // Subscribe to the event
        GameObject lastFish = spawnedFish[spawnedFish.Count - 1]; // Get the last GameObject
        FishScript fishScript = lastFish.GetComponent<FishScript>();
        DialogManager.Instance.isAfterYesNoDialog = true;
        fishScript.SetYesDialogForDisplay(); // SET THE TEXT 
        endFishDialogPanel.SetActive(false);
        acceptedFish.Add(lastFish);
        ApplyAcceptedFishEffects(fishScript);
        
        //set buffs

    }

    public void OnNoButtonClick()
    {
        Debug.Log("No button clicked.");
        DialogManager.Instance.OnDialogFinished += HideEndFishDialogNo; // Subscribe to the event
        GameObject lastFish = spawnedFish[spawnedFish.Count - 1]; // Get the last GameObject
        FishScript fishScript = lastFish.GetComponent<FishScript>();
        DialogManager.Instance.isAfterYesNoDialog = true;
        fishScript.SetNoDialogForDisplay(); // SET THE TEXT 
        endFishDialogPanel.SetActive(false);

        rejectedFish.Add(lastFish);
        ApplyNegativeFishEffects(fishScript);


    }

    private void HideEndFishDialogNo()
    {
        DialogManager.Instance.OnDialogFinished -= HideEndFishDialogNo; // Unsubscribe to prevent memory leaks!
        GameObject lastFish = spawnedFish[spawnedFish.Count - 1]; // Get the last GameObject
        FishScript fishScript = lastFish.GetComponent<FishScript>();

        if (endFishDialogPanel != null)
        {
            DialogManager.Instance.chatDialogObject.SetActive(false);
            fishScript.ThrowBack();
            EndTurn();
            
        }
    }

    private void HideEndFishDialogYes()
    {
        DialogManager.Instance.OnDialogFinished -= HideEndFishDialogYes; // Unsubscribe to prevent memory leaks!
        GameObject lastFish = spawnedFish[spawnedFish.Count - 1]; // Get the last GameObject
        FishScript fishScript = lastFish.GetComponent<FishScript>();

        if (endFishDialogPanel != null)
        {
            DialogManager.Instance.chatDialogObject.SetActive(false);
            fishScript.ThrowFront();
            EndTurn();
            
        }
    }

    public void CheckFishConsequencesAccepted()
    {
        foreach (GameObject fishObject in acceptedFish)
        {
            FishScript fishScript = fishObject.GetComponent<FishScript>();
            if (fishScript != null)
            {
                fishScript.IncrementTurnCounter();
                string consequencesMessage = fishScript.CheckConsequences();
                if (!string.IsNullOrEmpty(consequencesMessage))
                {
                    Debug.Log(consequencesMessage);
                }
            }
            else
            {
                Debug.LogError("FishScript component not found on " + fishObject.name);
            }
        }
    }

    

}

// public class FishScript : MonoBehaviour
// {
//     public void PerformTurnAction()
//     {
//         transform.position += new Vector3(Random.Range(-0.1f, 0.1f), 0, 0);
//         Debug.Log(gameObject.name + " performed its turn action.");
//     }
// }