using UnityEngine;
using System.Text;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.SceneManagement;
using UnityEngine.Networking; // For WebGL-compatible networking
using System.Linq;


public class APIHelper : MonoBehaviour
{
    private const string apiKey = "YOUR_API_KEY";
    private const string url = "https://api.arliai.com/v1/chat/completions";

    // Public property to store the word list
    public List<string> wordList { get; private set; } = new List<string>();

    // Reference to FAWLogic
    private FAWLogic FAWLogic;
    //public TMP_InputField inputField;
    public InputField inputField;  // Changed from TMP_InputField to InputField
    public Button generateFAWButton;
    public Button resetButton;
    //public Button backButton;
    private string topic;
    public TextMeshProUGUI Notification;
    public TextMeshProUGUI loadingText;
    public GameObject loadingWheel;
    public bool gameJustLaunched;
    public bool stillLoading;
    public string currentDifficulty;
    public int DiffStringCount;
    public AudioSource backgroundMusic;
    private bool musicOn = true;
    public AudioSource clickSFX;


    [System.Serializable]
    public class APIRequest
    {
        public string model;

        public Message[] messages;
        public float repetition_penalty;
        public float temperature;
        public float top_p;
        public int top_k;
        public int max_tokens;
        public bool stream;  // Set to false to disable streaming
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class APIResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    public class Choice
    {
        public Message delta; // Use delta for the assistant's response
    }

    public void Reset()
    {
        Debug.Log("F.A.W Reset.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMain()
    {
        Debug.Log("Back to Main");
        Destroy(DifficultySelect.Instance.gameObject);
        SceneManager.LoadScene("Main Page");
    }

    public void ToggleBackgroundMusic()
    {
        
        if (musicOn)
        {
            backgroundMusic.Pause();
            musicOn = false;
        }
        else
        {
            backgroundMusic.Play();
            musicOn = true;
        }
    }

    // Modified Method for WebGL-Compatible API Request
    /*public void GetChatCompletion(string userMessage)
    {
        var requestBody = new APIRequest
        {
            model = "Meta-Llama-3.1-8B-Instruct",
            messages = new[]
            {
                new Message { role = "system", content = "" +
                "You output a list of 10 strings. The user gives you a topic." +
                "Then you output a list of 10 strings based on that topic." +
                "For example if the topic is Animals you could output: " +
                "['cat','dog','fish','frog','rabbit','lion','tiger','bird','walrus','starfish']." +
                "Please ensure that the format of the output is always ONLY a list of strings!"+
                "Please ensure that strings are not larger than 20 characters long!!!"+
                "Please ensure that strings are not SMALLER than 3 characters long!!! - for example: it,he,on - are NOT ALLOWED!!!"},
                new Message { role = "user", content = userMessage }
            },
            repetition_penalty = 1.1f,
            temperature = 0.7f,
            top_p = 0.9f,
            top_k = 40,
            max_tokens = 1024,
            stream = false  // Set stream to false
        };

        string json = JsonUtility.ToJson(requestBody);
        Debug.Log("Request JSON: " + json); // Log the JSON body

        StartCoroutine(PostRequestCoroutine(url, json)); // Coroutine for WebGL compatibility
    }*/

    public void GetChatCompletion(string userMessage, string difficulty)
    {
        string systemContent;

        // Modify the system message based on difficulty
        switch (difficulty.ToLower())
        {
            case "hard":
                systemContent = "" +
                    "You output a list of 10 strings. The user gives you a topic. " +
                    "Then you output a list of 10 strings based on that topic. " +
                    "For example, if the topic is Animals you could output: " +
                    "['cat','dog','fish','frog','rabbit','lion','tiger','bird','walrus','starfish']. " +
                    "Please ensure that the format of the output is always ONLY a list of strings! " +
                    "Please ensure that strings are not larger than 20 characters long!!! " +
                    "Please ensure that strings are not SMALLER than 3 characters long!!!";
                break;

            case "medium":
                systemContent = "" +
                    "You output a list of 10 strings. The user gives you a topic. " +
                    "Then you output a list of 10 strings based on that topic. " +
                    "For example, if the topic is Animals you could output: " +
                    "['cat','dog','fish','frog','rabbit','lion','tiger','bird','walrus','starfish']. " +
                    "Please ensure that the format of the output is always ONLY a list of strings! " +
                    "Please ensure that strings are not larger than 15 characters long!!! " +
                    "Please ensure that strings are not SMALLER than 3 characters long!!!";
                break;

            case "easy":
                systemContent = "" +
                    "You output a list of 10 strings. The user gives you a topic. " +
                    "Then you output a list of 10 strings based on that topic. " +
                    "For example, if the topic is Animals you could output: " +
                    "['cat','dog','fish','frog','lion']. " +
                    "Please ensure that the format of the output is always ONLY a list of strings! " +
                    "Please ensure that strings are not larger than 10 characters long!!! " +
                    "Please ensure that strings are not SMALLER than 3 characters long!!!";
                break;

            default:
                Debug.LogWarning("Invalid difficulty level! Defaulting to hard.");
                systemContent = "" +
                    "You output a list of 10 strings. The user gives you a topic. " +
                    "Then you output a list of 10 strings based on that topic. " +
                    "For example, if the topic is Animals you could output: " +
                    "['cat','dog','fish','frog','rabbit','lion','tiger','bird','walrus','starfish']. " +
                    "Please ensure that the format of the output is always ONLY a list of strings! " +
                    "Please ensure that strings are not larger than 20 characters long!!! " +
                    "Please ensure that strings are not SMALLER than 3 characters long!!!";
                break;
        }

        // Create the APIRequest object with the selected system message
        var requestBody = new APIRequest
        {
            model = "Mistral-Nemo-12B-Instruct-2407",
            messages = new[]
            {
            new Message { role = "system", content = systemContent },
            new Message { role = "user", content = userMessage }
        },
            repetition_penalty = 1.1f,
            temperature = 0.7f,
            top_p = 0.9f,
            top_k = 40,
            max_tokens = 1024,
            stream = false  // Set stream to false
        };

        string json = JsonUtility.ToJson(requestBody);
        Debug.Log("Request JSON: " + json); // Log the JSON body

        StartCoroutine(PostRequestCoroutine(url, json)); // Coroutine for WebGL compatibility
    }

    // Coroutine to handle WebGL request
    private IEnumerator<UnityWebRequestAsyncOperation> PostRequestCoroutine(string url, string json)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // Send the request and wait for a response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response JSON: " + request.downloadHandler.text); // Log the response

            // Step 1: Locate the "content" substring
            string startKey = "\"content\":\"";
            int startIndex = request.downloadHandler.text.IndexOf(startKey) + startKey.Length;

            // Step 2: Locate the "tool_calls" substring
            string endKey = "\",\"tool_calls";
            int endIndex = request.downloadHandler.text.IndexOf(endKey, startIndex);

            // Step 3: Extract the content string between these indices
            string output_words = request.downloadHandler.text.Substring(startIndex, endIndex - startIndex);
            Debug.Log("output words: " + output_words);

            // Step 4: Remove the square brackets
            string lean_words = output_words.Replace("[", "").Replace("]", "");
            Debug.Log("removed square brackets: " + lean_words);

            // Step 5: Split the string by commas and store it in a list
            wordList = new List<string>(lean_words.Split(','));

            for (int i = 0; i < wordList.Count; i++)
            {
                Debug.Log("Original word: " + wordList[i]);

                if (wordList[i].Length > DiffStringCount)
                {
                    // Clip the word to the maximum allowable character count
                    wordList[i] = wordList[i].Substring(0, DiffStringCount);
                    Debug.Log("Clipped word: " + wordList[i]);
                }
            }

            // Safeguard: Ensure the list contains no more than 10 words
            if (wordList.Count > 10)
            {
                Debug.Log($"Word list contains {wordList.Count} words. Trimming to 10.");
                wordList.RemoveRange(10, wordList.Count - 10); // Remove excess words starting from the 11th word
            }

            // Add random words from the fallback set if the list is too short
            while (wordList.Count < 10)
            {
                string randomWord = fallbackWords.ElementAt(UnityEngine.Random.Range(0, fallbackWords.Count));
                if (!wordList.Contains(randomWord)) // Avoid duplicates
                {
                    wordList.Add(randomWord);
                }
            }




            Debug.Log("Final word list:");
            foreach (string word in wordList)
            {
                Debug.Log(word);
            }


            Debug.Log("length of word list: "+wordList.Count);



            if (wordList.Count == 10)
            {
                try
                {
                    // Step 6: Trim whitespace and extra quotes from each element
                    for (int i = 0; i < wordList.Count; i++)
                    {
                        wordList[i] = wordList[i].Replace("'", "").Replace(" ", ""); ; // Trimming spaces and single quotes
                    }

                    // Step 7: Convert all characters to uppercase and remove any spaces within the strings
                    for (int i = 0; i < wordList.Count; i++)
                    {
                        // Remove spaces, punctuation, and convert to uppercase
                        wordList[i] = new string(wordList[i].Where(char.IsLetterOrDigit).ToArray()).ToUpper();
                    }
                }
                catch (Exception ex)
                {
                    // Log the error and provide information about where it occurred
                    Debug.LogError("Error while processing word list: " + ex.Message);
                    // Optional: You could assign a fallback list or rethrow the exception if necessary
                    Notification.text = "YOU ENTERED A BANNED TOPIC!!! USING DEFAULT SETTINGS!!!";
                    wordList = new List<string> { "CAT", "DOG", "FISH", "FROG", "RABBIT", "LION", "TIGER", "BIRD", "WALRUS", "STARFISH" };
                }
            }
            else
            {
                try
                {
                    Debug.Log("The length of the word list is 1. A user has probably entered a word the API considers unsafe to discuss...");
                    Notification.text = "YOU ENTERED A BANNED TOPIC!!! USING DEFAULT SETTINGS!!!";
                    wordList = new List<string> { "CAT", "DOG", "FISH", "FROG", "RABBIT", "LION", "TIGER", "BIRD", "WALRUS", "STARFISH" };
                }
                catch
                {
                    Debug.Log("Some other error occurred because of entering an unsafe topic, now we are using the default word list!!!");
                    Notification.text = "YOU ENTERED A BANNED TOPIC!!! USING DEFAULT SETTINGS!!!";
                    wordList = new List<string> { "CAT", "DOG", "FISH", "FROG", "RABBIT", "LION", "TIGER", "BIRD", "WALRUS", "STARFISH" };
                }

            }



            Debug.Log("Final word list: " + wordList);
            FAWLogic.setOffProgram();
            loadingText.gameObject.SetActive(false);
            loadingWheel.SetActive(false);
            FAWLogic.gameOver = false;
            stillLoading = false;
        }
        else
        {
            Debug.Log("Request UNSUCCESSFUL!");

            string errorResponse = request.downloadHandler.text;

            if (string.IsNullOrEmpty(errorResponse))
            {
                Debug.LogError("Error: No content received from API. Status Code: " + request.responseCode);
                Notification.text = "No response from server: "+request.responseCode+ "Please try again later.";
                wordList = new List<string> { "CAT", "DOG", "FISH", "FROG", "RABBIT", "LION", "TIGER", "BIRD", "WALRUS", "STARFISH" };
            }
            else
            {
                Debug.LogError("Error: " + request.responseCode + " Response: " + errorResponse);
                Notification.text = "Error retrieving data: "+errorResponse;
                wordList = new List<string> { "CAT", "DOG", "FISH", "FROG", "RABBIT", "LION", "TIGER", "BIRD", "WALRUS", "STARFISH" };
            }

            Debug.Log("Final word list: " + wordList);
            FAWLogic.setOffProgram();
            loadingText.gameObject.SetActive(false);
            loadingWheel.SetActive(false);
            FAWLogic.gameOver = false;
            stillLoading = false;
        }
    }

    // Example usage
    private void Start()
    {
        FAWLogic = FindObjectOfType<FAWLogic>();
        stillLoading = false;
        resetButton.interactable = false;
        currentDifficulty = DifficultySelect.Instance.difficulty;
        if (currentDifficulty == "Easy" )
        {
            DiffStringCount = 10;
        } else if (currentDifficulty == "Medium")
        {
            DiffStringCount = 15;
        } else if (currentDifficulty == "Hard")
        {
            DiffStringCount = 20;
        } else
        {
            DiffStringCount = 20;
        }
        Debug.Log("Difficulty: " + currentDifficulty);
        Debug.Log("Difficulty Max String Count: " + DiffStringCount);
    }

    private HashSet<string> bannedTopics = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "abduction", "abuse", "alcohol", "ammunition", "anorexia", "arson",
    "assassination", "bestiality", "blasphemy", "bomb", "bribery", "bulimia",
    "bullying", "cannibalism", "child abuse", "cocaine", "corruption", "crack", "crime",
    "cult", "cutting", "decapitation", "dildo","dildoes", "discrimination", "dismemberment",
    "domestic violence", "drug", "ecstasy", "embezzlement", "extremism", "fentanyl",
    "fraud", "genocide", "gore", "guns", "hallucinogens", "heroin", "ketamine",
    "LSD", "magic mushrooms", "marijuana", "meth", "methamphetamine",
    "narcotics", "opioid", "overdose", "oxycodone", "painkillers", "pedophilia",
    "poison", "pornography","porn", "profanity", "prostitution", "racism", "rape",
    "sadism", "satanism", "self-harm", "sex", "slavery", "smoking", "steroids",
    "suicide", "synthetic drugs", "terrorism", "torture", "trafficking", "transphobia",
    "violence", "war crimes","war crime", "weapons", "witchcraft", "xenophobia", "zoophilia",
    "two letter","one letter","two letter words","words with two letters","two letters","one letter",
    "one letter words","words with one letter","sexy",
    // Added common drugs/narcotics
    "adderall", "amphetamine", "angel dust", "benzodiazepine", "cannabis",
    "codeine", "crystal meth", "dmt", "GHB", "hash", "hydrocodone",
    "LSD blotter", "MDMA", "methadone", "morphine", "narcotic", "opioid",
    "oxycodone", "oxymorphone", "promethazine", "purple drank",
    "ritalin", "suboxone", "vicodin","xanax",
    // Common racial slurs (please note: these words are harmful and should be handled with care)
    "chink", "coon", "gook","kike", "nigger", "spic", "wetback",
    // Miscellaneous:
    "tit","boob","penis","vagina","cock","dick","pussy","cunt","breast","ass","butt","gyat","anal","blowjob","drugs",
    "tits","boobs","penises","vaginas","cocks","dicks","pussies","cunts","breasts","asses","butts","gyats","anal","blowjob",
    "killers","maniacs","killer","maniac",
    };

    private HashSet<string> fallbackWords = new HashSet<string>
    {
        "AI", "ROBOT", "SPACE", "STARS", "GALAXY", "PLANET", "PYTHON", "UNITY",
        "JAVA", "LOGIC", "ALGORITHM", "MATRIX", "NEURAL", "PIXEL", "VECTOR", "DRONE",
        "SATURN", "MARS", "JUPITER", "OCEAN", "CORAL", "REEF", "SUNSET", "WAVES",
        "FOREST", "RAVEN", "CANYON", "FOSSIL", "VORTEX", "CIRCUIT", "PRISM", "SPECTRUM",
        "BINARY", "FUSION", "VIRGO", "ORBIT", "ASTEROID", "ECLIPSE", "METEOR", "GRAPH",
        "CODE", "SPHERE", "CIPHER", "QUANTUM", "GAMMA", "VECTOR", "ENERGY", "TURING",
        "GAMING", "CLOUD"
    };

    // Checks for banned topics using HashSet
    private bool containsBannedTopics(string inputString)
    {
        foreach (string word in inputString.ToLower().Split(' ', '.', ',', ';', '!', '?'))
        {
            if (bannedTopics.Contains(word))
            {
                return true;
            }
        }
        return false;
    }

    public void getTextFromInputField()
    {
        Notification.text = "";

        if (string.IsNullOrWhiteSpace(inputField.text))
        {
            Debug.Log("Please enter a topic!!!");
            Notification.text = "PLEASE ENTER A TOPIC!!!";
        }
        else if (containsBannedTopics(inputField.text))
        {
            Debug.Log("You entered a banned topic");
            Notification.text = "YOU ENTERED A BANNED TOPIC!!!";
        }
        else
        {
            topic = inputField.text;
            Debug.Log($"Generating a FIND-A-WORD for the TOPIC: {topic}");
            FAWLogic.lives = 5;
            FAWLogic.livesLeft.text = ("LIVES LEFT: " + FAWLogic.lives);
            GetChatCompletion($"Topic: {topic}", currentDifficulty);
            generateFAWButton.interactable = false;
            inputField.interactable = false;
            stillLoading = true;
            loadingText.gameObject.SetActive(true);
            loadingWheel.SetActive(true);
        }
    }
}


