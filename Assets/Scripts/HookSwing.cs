using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HookSwing : MonoBehaviour
{
    // Hook-related variables
    public float rotationSpeed = 50f;
    private float direction = 1f;
    private bool isMoving = false;
    private bool isRetracting = false;
    private float moveSpeed = 5f;
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private Rigidbody2D hook;

    // Game state variables
    private int score = 0;  
    private float timeRemaining = 60f;
    private bool isTimerRunning = false;
    private bool gameStarted = false;
    private bool isItemSlidingDown = false;
    public float slideSpeed = 5f;
    private GameObject collectedObject;
    private GameObject droppedItem;
    private Dictionary<GameObject, Vector3> originalItemPositions = new Dictionary<GameObject, Vector3>();

    // UI elements
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public GameObject gameOverScreen;
    public GameObject titleScreen;
    public GameObject startScreen;

    // Item scores 
    private Dictionary<string, int> itemScores = new Dictionary<string, int>()
    {
        { "Coin", 50 },
        { "GoldSmall", 100 },
        { "GoldBig", 500 },
        { "Diamond", 1000 }
    };

    void Start()
    {
        hook = GetComponent<Rigidbody2D>();
        hook.isKinematic = true;
        startPosition = transform.position;

        // Store original positions of all collectible items
        GameObject[] items = GameObject.FindGameObjectsWithTag("Coin");  // Find all coins
        foreach (GameObject item in items)
        {
            originalItemPositions[item] = item.transform.position;
        }

        items = GameObject.FindGameObjectsWithTag("GoldSmall");  // Find all small gold pieces
        foreach (GameObject item in items)
        {
            originalItemPositions[item] = item.transform.position;
        }

        items = GameObject.FindGameObjectsWithTag("GoldBig");  // Find all big gold pieces
        foreach (GameObject item in items)
        {
            originalItemPositions[item] = item.transform.position;
        }

        items = GameObject.FindGameObjectsWithTag("Diamond");  // Find all diamonds
        foreach (GameObject item in items)
        {
            originalItemPositions[item] = item.transform.position;
        }

        // Ensure StartScreen is active and ScoreText and TimerText are visible
        if (startScreen != null)
        {
            startScreen.SetActive(true);
        }

        // Ensure GameOverScreen is assigned and hidden at the start
        if (gameOverScreen == null)
        {
            gameOverScreen = GameObject.Find("GameOverScreen");
            if (gameOverScreen != null) gameOverScreen.SetActive(false);
        }

        // Initialize score
        score = 0;
        UpdateScoreUI();

        // Check that TimerText exists
        if (timerText == null)
        {
            Debug.LogError("TimerText is not assigned in the HookSwing script!");
        }
        else
        {
            UpdateTimerUI();  // Update timer UI at the start
        }
    }

    void Update()
    {
        // If the game is over and spacebar is pressed, reset the game
        if (gameOverScreen.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            ResetGame();
            return; // Prevents further processing in this frame
        }

        // If the game has not started yet and spacebar is pressed, start the game
        if (!gameStarted && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
            return;
        }

        // Update timer if running
        if (isTimerRunning)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                EndGame();
            }
            UpdateTimerUI();
        }

        // Rotate the hook when not moving
        if (!isMoving)
        {
            transform.Rotate(0, 0, rotationSpeed * direction * Time.deltaTime);
            float currentAngle = transform.eulerAngles.z;
            if (currentAngle > 180f) currentAngle -= 360f;

            if (currentAngle >= 90f) direction = -1f;
            else if (currentAngle <= -90f) direction = 1f;
        }

        // Deploy the hook when spacebar is pressed (only after game starts)
        if (gameStarted && Input.GetKeyDown(KeyCode.Space) && !isMoving && !isRetracting)
        {
            StartMovingHook();
        }

        if (isMoving) MoveHook();
        if (isRetracting) RetractHook();
        if (isItemSlidingDown) SlideItemDown();
    }

    public void StartGame()
    {
        gameStarted = true;
        isTimerRunning = true;

        // Hide the TitleScreen panel when the game starts
        if (titleScreen != null)
        {
            titleScreen.SetActive(false);
        }
    }

    void StartMovingHook()
    {
        rotationSpeed = 0f;

        float angleInRadians = (transform.eulerAngles.z - 90f) * Mathf.Deg2Rad;
        float xDirection = Mathf.Cos(angleInRadians);
        float yDirection = Mathf.Sin(angleInRadians);

        // Determine target position for hook movement
        targetPosition = transform.position + new Vector3(xDirection, yDirection, 0f) * 10f;
        isMoving = true;
    }

    void MoveHook()
    {
        // Move hook towards target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Stop moving and retract hook once target position is reached
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) StopMovingAndRetract();
    }

    void StopMovingAndRetract()
    {
        isMoving = false;
        isRetracting = true;
    }

    void RetractHook()
    {
        // Retract hook back to starting position
        transform.position = Vector3.MoveTowards(transform.position, startPosition, moveSpeed * 3 * Time.deltaTime);
        if (collectedObject != null) collectedObject.transform.position = transform.position;

        // If an item is successfully collected, update score
        if (Vector3.Distance(transform.position, startPosition) < 0.1f)
        {
            isRetracting = false;
            rotationSpeed = 50f;

            if (collectedObject != null)
            {
                string itemTag = collectedObject.tag;
                if (itemScores.ContainsKey(itemTag)) AddScore(itemTag);
                collectedObject.SetActive(false);
                collectedObject = null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If hook collides with collectible item, retract hook to collect item 
        if (isMoving && itemScores.ContainsKey(other.tag))
        {
            isMoving = false;
            collectedObject = other.gameObject;
            StopMovingAndRetract();
        }

        // If hook collides with enemy (ghost or skeleton), retract hook and drop any collected items
        if (other.CompareTag("Ghost") || other.CompareTag("Skeleton"))
        {
            if (collectedObject != null)
            {
                Collider2D collectedCollider = collectedObject.GetComponent<Collider2D>();
                if (collectedCollider != null) collectedCollider.enabled = false;

                droppedItem = collectedObject;
                isItemSlidingDown = true;
                collectedObject = null;
            }

            isMoving = false;
            StopMovingAndRetract();
        }
    }

    private void SlideItemDown()
    {
        // If item is dropped, slide item out of game window and hide it
        if (droppedItem != null)
        {
            droppedItem.transform.Translate(Vector3.down * slideSpeed / 2 * Time.deltaTime);
            Vector3 viewportPosition = Camera.main.WorldToViewportPoint(droppedItem.transform.position);
            if (viewportPosition.y < 0)
            {
                droppedItem.SetActive(false);
                droppedItem = null;
                isItemSlidingDown = false;
            }
        }
    }

    void AddScore(string itemTag)
    {
        if (itemScores.ContainsKey(itemTag))
        {
            score += itemScores[itemTag];
            UpdateScoreUI();
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.Ceil(timeRemaining);
        }
    }

    void EndGame()
    {
        // Show the GameOverScreen
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

        isTimerRunning = false;  // Stop the timer
        gameStarted = false; // Mark the game as not started
    }

    void ResetGame()
    {
        // Reset score and timer
        score = 0;
        timeRemaining = 60f;

        UpdateScoreUI();
        UpdateTimerUI();

        // Reactivate StartScreen
        if (startScreen != null)
        {
            startScreen.SetActive(true);
        }

        // Reactivate TitleScreen
        if (titleScreen != null)
        {
            titleScreen.SetActive(true);
        }

        // Hide GameOverScreen
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }

        // Reset and reactivate collectible items
        foreach (var item in originalItemPositions)
        {
            item.Key.transform.position = item.Value;
            item.Key.SetActive(true);

            Collider2D itemCollider = item.Key.GetComponent<Collider2D>();
            if (itemCollider != null) itemCollider.enabled = true;
        }
    }
}
