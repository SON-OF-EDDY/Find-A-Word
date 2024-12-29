using TMPro; // Make sure to include this if it's not already at the top
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class FAWLogic : MonoBehaviour
{
    public int gridSize;
    //private char[,] grid = new char[gridSize, gridSize];
    private char[,] grid;

    private APIHelper apiHelper;

    private List<string> words = new List<string>();
    // List to track found words
    private List<string> foundWords = new List<string>();
    private List<List<Vector2Int>> allWordPositions = new List<List<Vector2Int>>(); // Array to hold positions of all word letters
                                                                                    // Initialize the list to avoid null reference issues

    // Reference to the Find-A-Word GameBoard Tilemap
    public Tilemap FAWTilemap;

    // The list of words to display to the user as a textmeshpro element:
    public TextMeshProUGUI wordsToDisplay;
    public TextMeshProUGUI livesLeft;
    public bool gameOver;
    public GameObject popUpPanel;
    public GameObject screenBlocker;
    public TextMeshProUGUI popUpText;
    public string currentDifficulty;
    public int DiffStringCount;



    // TileBases for all the letters of the alphabet:
    public TileBase A_Tile;
    public TileBase B_Tile;
    public TileBase C_Tile;
    public TileBase D_Tile;
    public TileBase E_Tile;
    public TileBase F_Tile;
    public TileBase G_Tile;
    public TileBase H_Tile;
    public TileBase I_Tile;
    public TileBase J_Tile;
    public TileBase K_Tile;
    public TileBase L_Tile;
    public TileBase M_Tile;
    public TileBase N_Tile;
    public TileBase O_Tile;
    public TileBase P_Tile;
    public TileBase Q_Tile;
    public TileBase R_Tile;
    public TileBase S_Tile;
    public TileBase T_Tile;
    public TileBase U_Tile;
    public TileBase V_Tile;
    public TileBase W_Tile;
    public TileBase X_Tile;
    public TileBase Y_Tile;
    public TileBase Z_Tile;
    public TileBase Blank_Tile;

    public int lives = 5;
    private int counterFoundWords = 0;

    // For Sound effects
    public AudioSource clickSFX;
    public AudioSource foundWordSFX;
    public AudioSource notAWordSFX;
    public AudioSource victorySFX;
    public AudioSource defeatSFX;

    public void SetGridSize(int size) // Method to set the grid size
    {
        gridSize = size; // Set the grid size
        grid = new char[gridSize, gridSize]; // Initialize the grid with the new size
    }

    public void ScaleTilemap()
    {
        float scaleFactor = 1.0f;  // Default scaling factor

        // Adjust the scale factor based on the grid size
        if (gridSize == 10) // Easy mode
        {
            Debug.Log("Using scale factor for EASY MODE");
            scaleFactor = 1.5f;  // Increase scale for larger grid cells
        }
        else if (gridSize == 15) // Medium mode
        {
            scaleFactor = 1.2f;  // Slightly increase scale for medium grid cells
            Debug.Log("Using scale factor for MEDIUM MODE");
        }
        else
        {
            scaleFactor = 1.0f;  // Default scale for normal mode
            Debug.Log("Using scale factor for HARD MODE");
        }

        // Apply the scaling to the Tilemap
        FAWTilemap.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
    }

    void Start()
    {
        // Find the APIHelper component in the scene
        apiHelper = FindObjectOfType<APIHelper>();
        currentDifficulty = DifficultySelect.Instance.difficulty;


        if (currentDifficulty == "Easy")
        {
            gridSize = 10;
        }
        else if (currentDifficulty == "Medium")
        {
            gridSize = 15;
        }
        else if (currentDifficulty == "Hard")
        {
            gridSize = 20;
        }
        else
        {
            gridSize = 20;
        }
        Debug.Log("Difficulty: " + currentDifficulty);
        Debug.Log("Gridsize: " + gridSize);

        SetGridSize(gridSize);
        FillTilemapWithBlanks();
        ScaleTilemap();
        CenterTilemap();


    }

    // Add coordinates for each letter of a word into 'allWordPositions'
    private void AddWordCoordinatesToPositionList(string word, int startX, int startY, Vector2Int direction)
    {
        List<Vector2Int> wordPositions = new List<Vector2Int>();

        for (int i = 0; i < word.Length; i++)
        {
            wordPositions.Add(new Vector2Int(startX, startY));
            startX += direction.x;
            startY += direction.y;
        }

        allWordPositions.Add(wordPositions);
    }

    public void clickOnATile(Vector3Int tilePosition)
    {
        
        // Check if the clicked position is within the tilemap bounds
        if (FAWTilemap.HasTile(tilePosition) && !gameOver && apiHelper.resetButton.interactable != false) // Only proceed if there's a tile at the clicked position, the game is not over, and the user has searched for at least one topic
        {
            // Check if the clicked tile matches any word's letter positions
            bool foundWord = false;
            string foundWordString = null; // To store the string of the word found
            List<Vector2Int> wordToRemove = null; // To store the word to remove if found

            foreach (var wordPositions in allWordPositions)
            {
                foreach (var letterPosition in wordPositions)
                {
                    // If the clicked position matches a letter position
                    if (tilePosition.x == letterPosition.x && tilePosition.y == letterPosition.y)
                    {
                        foundWord = true;
                        Debug.Log("You found a word!");

                        foundWordSFX.Play();

                        // Replace the word's tiles with blank tiles
                        foreach (var pos in wordPositions)
                        {
                            FAWTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), Blank_Tile);
                        }

                        // Store the word to remove from the list later
                        wordToRemove = wordPositions;
                       

                        // Get the index of the word found to pass to markWordAsFound()
                        int wordIndex = allWordPositions.IndexOf(wordPositions);
                        if (wordIndex != -1)
                        {
                            foundWordString = words[wordIndex];
                            MarkWordAsFound(foundWordString);
                            Debug.Log(foundWordString);
                            // Get the actual word string
                        } else
                        {
                            
                            Debug.Log("couldn't add word to the found words list");
                        }

                        break;
                    }
                }

                if (foundWord) break;
            }

            // If a word was found, remove it from the list and check for win condition
            if (foundWord && wordToRemove != null)
            {
                //allWordPositions.Remove(wordToRemove); // Remove the found word
                counterFoundWords += 1;
                

                checkWin(); // Check if all words are found
            }
            // If no matching tile, reduce the player's lives
            else if (!foundWord)
            {
                lives--;
                
                Debug.Log("Incorrect tile clicked. Lives remaining: " + lives);
                notAWordSFX.Play();
                livesLeft.text = ("LIVES LEFT: " + lives);

                if (lives == 0)
                {
                    Debug.Log("GAME OVER!");
                    defeatSFX.Play();
                    //apiHelper.Notification.text = ("GAME OVER!");
                    gameOver = true;
                    popUpPanel.SetActive(true);
                    screenBlocker.SetActive(true);
                    apiHelper.resetButton.interactable = false;
                    popUpText.text = "YOU HAVE RUN OUT OF LIVES!!!";
                    
                } 
            }
        } else
        {
            clickSFX.Play();
        }
        
    }

    // Check if the player has found all words
    private void checkWin()
    {
        if (counterFoundWords == 10)
        {
            Debug.Log("Congratulations! You found all the words! VICTORY!");
            victorySFX.Play();
            //apiHelper.Notification.text = ("ALL WORDS FOUND! VICTORY!");
            gameOver = true;
            apiHelper.resetButton.interactable = false;
            popUpPanel.SetActive(true);
            screenBlocker.SetActive(true);
            
            popUpText.text = "YOU HAVE FOUND ALL THE WORDS!!!";

        }
        else
        {
            Debug.Log("Keep going! You have almost found all words!!!");
        }
    }

    // Call this function when a word is found to mark it as "found"
    void MarkWordAsFound(string word)
    {
        if (!foundWords.Contains(word))
        {
            foundWords.Add(word); // Add the word to the list of found words
            updateListOfWordsToDisplayToUser(); // Update the UI
        }
    }

    public void updateListOfWordsToDisplayToUser()
    {
        // Clear the current text
        wordsToDisplay.text = "";

        // Iterate through the words list and build the display string
        for (int i = 0; i < words.Count; i++)
        {
            string word = words[i];

            // Check if the word has been found
            if (foundWords.Contains(word))
            {
                // Apply strikethrough using TextMeshPro tags
                wordsToDisplay.text += $"{i + 1}. <s>{word}</s>\n"; // Strikethrough applied here
            }
            else
            {
                // Display normally if the word hasn't been found
                wordsToDisplay.text += $"{i + 1}. {word}\n";
            }
        }
    }

    
    



    public void setOffProgram ()
    {
        PutWordsInList();
        InitializeGrid();
        PopulateGrid();
        FillRandomLetters();
        DisplayGrid();
        PopulateTilemap();
        //CenterTilemap();
        updateListOfWordsToDisplayToUser();
        apiHelper.resetButton.interactable = true;

    }


    void PutWordsInList()
    {
        
        if (apiHelper != null)
        {
            // Access the wordList
            words = apiHelper.wordList;

            
            
        }
        else
        {
            Debug.LogError("APIHelper not found in the scene.");
        }
    }


    private void InitializeGrid()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                grid[i, j] = '-'; // Using '-' to represent empty cells
            }
        }
    }

    private void PopulateGrid()
    {
        foreach (string word in words)
        {
            bool placed = false;
            while (!placed)
            {
                int startX = UnityEngine.Random.Range(0, gridSize);
                int startY = UnityEngine.Random.Range(0, gridSize);
                Vector2Int direction = GetRandomDirection();
                int length = CalculateMaxDistance(startX, startY, direction);

                if (length >= word.Length)
                {
                    if (CanPlaceWord(startX, startY, word, direction))
                    {
                        PlaceWord(startX, startY, word, direction);
                        AddWordCoordinatesToPositionList(word, startX, startY, direction); // Add positions of the word's letters to the array
                        placed = true;
                    }
                }
            }
        }
    }

    private Vector2Int GetRandomDirection()
    {
        Vector2Int[] directions = {
            new Vector2Int(0, 1),   // Right
            new Vector2Int(0, -1),  // Left
            new Vector2Int(1, 0),   // Down
            new Vector2Int(-1, 0),  // Up
            new Vector2Int(1, 1),   // Bottom Right
            new Vector2Int(-1, -1), // Top Left
            new Vector2Int(1, -1),  // Bottom Left
            new Vector2Int(-1, 1)   // Top Right
        };
        return directions[UnityEngine.Random.Range(0, directions.Length)];
    }

    private int CalculateMaxDistance(int startX, int startY, Vector2Int direction)
    {
        int distance = 0;
        int x = startX;
        int y = startY;

        // Calculate distance in the given direction until the boundary
        while (IsWithinBounds(x, y))
        {
            distance++;
            x += direction.x;
            y += direction.y;
        }

        return distance;
    }

    private bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < gridSize && y >= 0 && y < gridSize;
    }

    private bool CanPlaceWord(int startX, int startY, string word, Vector2Int direction)
    {
        int x = startX;
        int y = startY;

        for (int i = 0; i < word.Length; i++)
        {
            if (!IsWithinBounds(x, y) || grid[x, y] != '-')
            {
                return false; // Out of bounds or cell already occupied
            }
            x += direction.x;
            y += direction.y;
        }

        return true;
    }

    private void PlaceWord(int startX, int startY, string word, Vector2Int direction)
    {
        int x = startX;
        int y = startY;

        for (int i = 0; i < word.Length; i++)
        {
            grid[x, y] = word[i];
            x += direction.x;
            y += direction.y;
        }
    }

    private void FillRandomLetters()
    {
        System.Random random = new System.Random();

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (grid[i, j] == '-')
                {
                    grid[i, j] = (char)('A' + random.Next(0, 26)); // Random letter A-Z
                }
            }
        }
    }

    private void DisplayGrid()
    {
        string gridString = "";
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                gridString += grid[i, j] + " ";
            }
            gridString += "\n";
        }
        Debug.Log(gridString);
    }


    private void PopulateTilemap()
    {
        // Clear the Tilemap before populating it
        FAWTilemap.ClearAllTiles();

        // Iterate through the grid and populate the Tilemap
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                char letter = grid[x, y];
                TileBase tileToPlace = GetTileForLetter(letter);

                if (tileToPlace != null)
                {
                    // Calculate the position in the Tilemap
                    Vector3Int tilePosition = new Vector3Int(x, y, 0); // Z coordinate is usually 0 for 2D games
                    FAWTilemap.SetTile(tilePosition, tileToPlace);
                }
            }
        }
    }

    private TileBase GetTileForLetter(char letter)
    {
        switch (char.ToUpper(letter)) // Convert to uppercase to match tile naming
        {
            case 'A': return A_Tile;
            case 'B': return B_Tile;
            case 'C': return C_Tile;
            case 'D': return D_Tile;
            case 'E': return E_Tile;
            case 'F': return F_Tile;
            case 'G': return G_Tile;
            case 'H': return H_Tile;
            case 'I': return I_Tile;
            case 'J': return J_Tile;
            case 'K': return K_Tile;
            case 'L': return L_Tile;
            case 'M': return M_Tile;
            case 'N': return N_Tile;
            case 'O': return O_Tile;
            case 'P': return P_Tile;
            case 'Q': return Q_Tile;
            case 'R': return R_Tile;
            case 'S': return S_Tile;
            case 'T': return T_Tile;
            case 'U': return U_Tile;
            case 'V': return V_Tile;
            case 'W': return W_Tile;
            case 'X': return X_Tile;
            case 'Y': return Y_Tile;
            case 'Z': return Z_Tile;
            default: return E_Tile; // Return null for empty spaces or unrecognized letters
        }
    }

    private void CenterTilemap()
    {
        // Calculate the center position of the Tilemap
        float offset = gridSize/4.0f * FAWTilemap.transform.localScale.x; // The offset to center the Tilemap

        // Set the position of the Tilemap GameObject
        FAWTilemap.transform.position = new Vector3(-offset, -offset, 0);
    }

    void Update()
    {
        // Detect mouse click

        


        if (Input.GetMouseButtonDown(0) && apiHelper.stillLoading == false) // Left mouse button click
        {
            // Get the mouse position in screen space and convert it to world space
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Convert the world position to the cell position in the tilemap
            Vector3Int clickedTilePosition = FAWTilemap.WorldToCell(mouseWorldPos);

            // Call the clickOnATile function with the clicked tile position
            clickOnATile(clickedTilePosition);
        } 
    }

    private void FillTilemapWithBlanks()
    {
        // Clear the Tilemap before populating it
        FAWTilemap.ClearAllTiles();

        // Iterate through the grid and fill it with blank tiles
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0); // Z coordinate is 0 for 2D
                FAWTilemap.SetTile(tilePosition, Blank_Tile); // Set each tile to a blank tile
            }
        }
    }

}

