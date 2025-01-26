using System.Collections.Generic;
using UnityEngine;

public class FishScript : MonoBehaviour
{
    public string fishName;
    [TextArea(3, 10)] // Makes the text areas in the Inspector more usable
    public List<string> dialog = new List<string>();

    [TextArea(3, 10)]
    public List<string> yesDialog = new List<string>();

    [TextArea(3, 10)]
    public List<string> noDialog = new List<string>();
    public int moneyEffect;
    public int happinessEffect;
    public int harmonyEffect = 1;

    public float walkSpeed = 2f;
    public string targetTag = "WalkTarget";

    private bool isWalking = true;
    private bool reachTarget = false;
    private Transform targetTransform;

    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float rotationForce = 500f;
    private Rigidbody2D rb;
    private bool isThrown = false;

    private int turnCounter = 0;

    public int rejectedMoneyEffect; // Penalty to money when rejected
    public int rejectedHarmonyEffect; // Penalty to Harmony when rejected
    public int rejectedHappinessEffect; // Penalty to happiness when rejected

    public void IncrementTurnCounter()
    {
        turnCounter++;
    }



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on this GameObject!");
            // enabled = false; // Disable this script if no Rigidbody is found
        }
    }

    void Start()
    {
        // Find the target by tag
        GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);
        if (targetObject != null)
        {
            targetTransform = targetObject.transform;
        }
        else
        {
            Debug.LogError("No GameObject with tag '" + targetTag + "' found in the scene!");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if(reachTarget) return;

        if (!isWalking)
        {
            StartWalking();
        }
        else
        {
            Walk();
        }
    }

    void StartWalking()
    {
        isWalking = true;
    }

    void Walk()
    {
        if (targetTransform != null) // Check if the target still exists
        {
            transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, walkSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetTransform.position) < 0.1f)
            {
                ReachedDestination();
            }
        }
        else
        {
            Debug.LogWarning("Target has been destroyed!");
            isWalking = false; // Stop walking
            enabled = false;    // Optionally disable the script
        }
    }

    void ReachedDestination()
    {
        isWalking = false;
        reachTarget = true;
        SetDefaultDialogForDisplay();
    }

    public void SetDefaultDialogForDisplay(){
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.SetDialog(dialog);
        }
        else
        {
            Debug.LogError("DialogManager Instance is null!");
        }
    }
    public void SetYesDialogForDisplay(){
         if (DialogManager.Instance != null)
        {

            DialogManager.Instance.SetDialog(yesDialog);
        }
        else
        {
            Debug.LogError("DialogManager Instance is null!");
        }
    }
        public void SetNoDialogForDisplay(){
            if (DialogManager.Instance != null)
            {

                DialogManager.Instance.SetDialog(noDialog);
            }
            else
            {
                Debug.LogError("DialogManager Instance is null!");
            }
    }

    public void ThrowBack()
    {
        if (rb == null || isThrown) return;

        isThrown = true;

        Vector2 throwDirection = transform.right; // Use -transform.right for 2D

        rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse); // Use ForceMode2D

        rb.AddTorque(rotationForce, ForceMode2D.Impulse); // Use AddTorque(float torque)

        // Optional: Add a small upward force
        rb.AddForce(Vector2.up * (throwForce / 3f), ForceMode2D.Impulse);

    }

    public void ThrowFront()
    {
        if (rb == null || isThrown) return;

        isThrown = true;

        Vector2 throwDirection = -transform.right; // Use -transform.right for 2D

        rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse); // Use ForceMode2D

        rb.AddTorque(rotationForce, ForceMode2D.Impulse); // Use AddTorque(float torque)

        // Optional: Add a small upward force
        rb.AddForce(Vector2.up * (throwForce / 3f), ForceMode2D.Impulse);

    }

   public string CheckConsequences()
{
    int turnsSinceLastInteraction = turnCounter;

    switch (fishName)
    {
        case "SpecialFish":
            if (turnsSinceLastInteraction >= 3)
            {
                int moneyChange = 50;
                int harmonyChange = 2;
                int happinessChange = 10;

                TurnManager.Instance.AddResources(moneyChange);
                TurnManager.Instance.AddHarmony(harmonyChange);
                TurnManager.Instance.AddHappiness(happinessChange);

                return $"Because you waited {turnsSinceLastInteraction} turns after talking to the Special Fish, you received: {moneyChange} money, {harmonyChange} harmony, and {happinessChange} happiness!";
            }
            break;

        case "Balloonfish":
            if (turnsSinceLastInteraction >= 2)
            {
                int happinessChange = 5;

                TurnManager.Instance.AddResources(happinessChange);

                // Twist effect: -2 resources every few turns
                TurnManager.Instance.ApplyTwistEffect(-2, "resources", "Balloonfish occasionally inflates, causing minor inconveniences.");

                return $"Balloonie has settled in, bringing +{happinessChange} Happiness! But beware of minor inconveniences from its nervous inflations.";
            }
            break;

        case "Clownfish":
            if (turnsSinceLastInteraction >= 2)
            {
                int happinessChange = 10;

                TurnManager.Instance.AddHappiness(happinessChange);

                return $"Clownfish's jokes have brought +{happinessChange} Happiness to the kingdom!";
            }
            break;

        case "Bluestriped Grunt":
            if (turnsSinceLastInteraction >= 3)
            {
                int resourceChange = 10;
                int happinessPenalty = -5;

                TurnManager.Instance.AddResources(resourceChange);
                TurnManager.Instance.AddHappiness(happinessPenalty);

                return $"Grunter's insights improved resources by +{resourceChange}, but their constant grumbling caused -{Mathf.Abs(happinessPenalty)} Happiness.";
            }
            break;

        case "Starfish":
            if (turnsSinceLastInteraction >= 2)
            {
                int happinessChange = 10;

                TurnManager.Instance.AddHappiness(happinessChange);

                // Twist effect: occasional distractions
                TurnManager.Instance.ApplyTwistEffect(-5, "resources", "Stella occasionally causes distractions with impromptu performances.");

                return $"Stella's flair added +{happinessChange} Happiness, but beware of her dramatic distractions!";
            }
            break;

        case "Yellowfish":
            if (turnsSinceLastInteraction >= 2)
            {
                int resourceChange = 10;
                int harmonyPenalty = -5;

                TurnManager.Instance.AddResources(resourceChange);
                TurnManager.Instance.AddHarmony(harmonyPenalty);

                return $"Bigfin's size boosted resources by +{resourceChange}, but their boasting caused -{Mathf.Abs(harmonyPenalty)} Harmony.";
            }
            break;

        case "Swordfish":
            if (turnsSinceLastInteraction >= 4)
            {
                int resourceChange = 10;
                int harmonyChange = 5;

                TurnManager.Instance.AddResources(resourceChange);
                TurnManager.Instance.AddHarmony(harmonyChange);

                return $"Blade's efficiency brought +{resourceChange} Resources and +{harmonyChange} Harmony to the kingdom!";
            }
            break;

        case "Shark Electric Guitar":
            if (turnsSinceLastInteraction >= 1)
            {
                int happinessChange = 10;
                int harmonyPenalty = -5;

                TurnManager.Instance.AddHappiness(happinessChange);
                TurnManager.Instance.AddHarmony(harmonyPenalty);

                return $"Riff's music added +{happinessChange} Happiness, but their loud concerts caused -{Mathf.Abs(harmonyPenalty)} Harmony.";
            }
            break;

        case "Chill Hammerhead Shark":
            if (turnsSinceLastInteraction >= 2)
            {
                int harmonyChange = 10;

                TurnManager.Instance.AddHarmony(harmonyChange);

                return $"Vibe's chill tunes created +{harmonyChange} Harmony, keeping the kingdom relaxed.";
            }
            break;

        default:
            return $"It has been {turnsSinceLastInteraction} turn(s) since you talked to this fish.";
    }

    return $"It has been {turnsSinceLastInteraction} turn(s) since you talked to {fishName}. No consequences yet.";
}
}