using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System; // Important: Add this for Action


public class DialogManager : MonoBehaviour
{
    [SerializeField]
    private float typingSpeed = 0.05f;
    [SerializeField]
    private TextMeshPro dialogText;
    [SerializeField]
    private TextMeshPro nameText;
    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private List<string> dialogSentences = new List<string>();
    [SerializeField]
    private GameObject backgroundImage; // Reference to the background sprite renderer

    private int currentSentenceIndex = 0; 
    public static DialogManager Instance { get; private set; }
    public GameObject chatDialogObject; // Store the ChatDialog object

    public Action OnDialogFinished; // The new event/callback
    public bool isAfterYesNoDialog = false;




    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Optional: Keep the DialogManager across scenes
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (nextButton == null)
        {
            Debug.LogError("Next Button not found! Please assign the correct button.");
        }
        if (backgroundImage == null)
        {
            Debug.LogError("Background Sprite Renderer not found! Please assign the correct component.");
        }
        

        nextButton.gameObject.SetActive(false); // Initially hide the button

        // If you have dialog sentences, start the first one
        if (dialogSentences.Count > 0)
        {
            StartCoroutine(TypeDialog(dialogSentences[currentSentenceIndex])); 
        }
    }


    // Function to type out the dialog with speed
    public IEnumerator TypeDialog(string sentence)
    {
        dialogText.text = ""; 
        nextButton.gameObject.SetActive(false); 

        // Extract the name and the dialog
        string name = ExtractNameFromSentence(sentence);
        string dialog = sentence; 

        if (!string.IsNullOrEmpty(name))
        {
            dialog = dialog.Substring(name.Length + 3).TrimStart(); // Adjust for quotes and potential whitespace
        }

        // Display the name 
        if (!string.IsNullOrEmpty(name))
        {
            if (name == "King Bubbles")
            {
                nameText.color = new Color32(255, 100, 0, 200); 
            }
            else
            {
                nameText.color = Color.blue; // Reset to default color (white)
            }
            nameText.SetText(name); 
        }

        // Type out the dialog
        for (int i = 0; i <= dialog.Length; i++)
        {
            dialogText.SetText(dialog.Substring(0, i));
            yield return new WaitForSeconds(typingSpeed);
        }

        nextButton.gameObject.SetActive(true);
    }

    private string ExtractNameFromSentence(string sentence)
    {
        if (sentence.StartsWith("\""))
        {
            int endQuoteIndex = sentence.IndexOf("\"", 1); 
            if (endQuoteIndex != -1)
            {
                return sentence.Substring(1, endQuoteIndex - 1); 
            }
        }
        return string.Empty;
    }

    public void SetDialog(List<string> sentences)
    {
        dialogSentences = sentences;
        currentSentenceIndex = 0;
        chatDialogObject.SetActive(true);
        dialogText.text = ""; // Clear any previous text
        if (dialogSentences.Count > 0)
        {
            StartCoroutine(TypeDialog(dialogSentences[currentSentenceIndex]));
            // gameObject.SetActive(true); // Ensure the dialog is active
        }
        else
        {
            Debug.LogWarning("Trying to set an empty dialog!");
            // gameObject.SetActive(false); // Hide the dialog if no sentences
        }
    }

    // Function to display the next sentence in the list
    public void ShowNextSentence()
    {
        currentSentenceIndex++;

        // Check if there are more sentences in the list
        if (currentSentenceIndex < dialogSentences.Count)
        {
            StartCoroutine(TypeDialog(dialogSentences[currentSentenceIndex]));
        }
        else
        {
            Debug.Log("End of dialog.");
            // TurnManager.Instance.EndFishDialog(); // SHOW THE YES NO BUTTONS
            EndDialog();
   
            // You might want to deactivate the dialog box or trigger other events here.
        }

        // Hide the button after displaying a sentence
        nextButton.gameObject.SetActive(false); 
    }

    public void EndDialog()
    {
 
        if(isAfterYesNoDialog)
        {
            isAfterYesNoDialog = false;
            OnDialogFinished(); // Call the event/callback
        }else
        {
            Debug.Log("Show endFinishDIalog");
            TurnManager.Instance.EndFishDialog(); // SHOW THE YES NO BUTTONS
        }
        Debug.Log("EndDialog signal called");
        
    }

    // Adjust padding as needed
    private float padding = 2.5f; 
}