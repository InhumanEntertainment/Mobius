using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : GameScreen
{
    static public Game Instance;
    public SnakeController PlayerObject;
    public Vector3 PlayerStartPosition;

    // Data //
    public GameData Data;
    public GameText HighScoreText;

    // Gameplay //
    public int Score = 0;
    public GameText ScoreText;
    float SmoothScore = 0;
    public SnakeController Player;

    // Energy //
    public int EnergyCount = 0;
    public int EnergyPerBomb = 100;
    public SpriteRenderer EnergyBar;

    // Difficulty //
    float StartTime;
    public float DifficultyRampDuraction = 60;
    public float DifficultyAcceleration = 1;
    public float Difficulty = 0;
    public float DifficultyVelocityMax = 0.1f;
    public int DifficultyLineLength = 500;
    float DifficultyVelocity = 0;
    float DifficultyMin = 0;
    float DifficultyMax = 1;
    List<float> DifficultyLine = new List<float>();

    // Enemy Tracking //
    public List<GameObject> Enemies = new List<GameObject>();
    public Transform ObjectGroup;

    //============================================================================================================================================================================================//
    public void Awake()
    {
        // Singleton //
        if (Instance == null)
        {
            Instance = this;          
            SetHighScore();
        }
    }

    //============================================================================================================================================================================================//
    float DifficultyTime = 0;
    float DifficultyVariation = 0;
    float DifficultyVariationAmount = 0.3f;
    void Update()
    {
        UpdateScore();

        // Difficulty Ramp //
        DifficultyVelocity += (Random.value * DifficultyAcceleration - (DifficultyAcceleration * 0.5f));
        if (Mathf.Abs(DifficultyVelocity) > DifficultyVelocityMax)
        {
            DifficultyVelocity = Mathf.Sign(DifficultyVelocity) * DifficultyVelocityMax;
        }

        DifficultyVariation += DifficultyVelocity * Time.deltaTime;
        DifficultyVariation = Mathf.Clamp(DifficultyVariation, -DifficultyVariationAmount, DifficultyVariationAmount);
        DifficultyTime += (Time.deltaTime / DifficultyRampDuraction);

        if (Input.GetKey(KeyCode.A))
            DifficultyTime += 0.5f * Time.deltaTime;
        else if(Input.GetKey(KeyCode.Z))
            DifficultyTime -= 0.5f * Time.deltaTime;

        DifficultyTime = Mathf.Clamp01(DifficultyTime);
        Difficulty = DifficultyTime + DifficultyVariation;
        Difficulty = Mathf.Clamp01(Difficulty);
    }

    //============================================================================================================================================================================================//
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // Draw Bounds //
            Gizmos.color = Color.white;
            Vector3 TopLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 101));
            Vector3 BottomRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0));
            Vector3 Minimum = Camera.main.ScreenToWorldPoint(new Vector3(0, DifficultyMin / DifficultyMax * 101, 0));
            Gizmos.DrawLine(new Vector3(TopLeft.x, TopLeft.y, 0), new Vector3(BottomRight.x, TopLeft.y, 0));
            Gizmos.DrawLine(new Vector3(TopLeft.x, BottomRight.y, 0), new Vector3(BottomRight.x, BottomRight.y, 0));
            //Gizmos.DrawLine(new Vector3(TopLeft.x, Minimum.y, 0), new Vector3(BottomRight.x, Minimum.y, 0));

            // Draw Graph //
            Gizmos.color = Color.cyan;
            DifficultyLine.Insert(0, Difficulty);
            if (DifficultyLine.Count > DifficultyLineLength)
                DifficultyLine.RemoveRange(DifficultyLineLength, DifficultyLine.Count - DifficultyLineLength);

            Vector3 To = Vector3.zero;
            for (int i = 0; i < DifficultyLine.Count; i++)
            {
                Vector3 From = new Vector3((DifficultyLineLength - 1 - i) / (float)DifficultyLineLength * Screen.width, DifficultyLine[i] / DifficultyMax * 100, 0);
                From = Camera.main.ScreenToWorldPoint(From);
                From.z = 0;
                if (i == 0)
                    To = From;

                Gizmos.DrawLine(From, To);
                To = From;
            }
        }
    }

    //============================================================================================================================================================================================//
    public void NewGame()
    {
        print("Frontend: Play");

        // Create Player and Level //
        CleanupScene();
        SetScore(0);
        SetEnergy(0);
        SmoothScore = 0;
        Difficulty = 0;
        DifficultyMin = 0;
        DifficultyMax = 1;
        DifficultyVelocity = 0;
        DifficultyVariation = 0;
        DifficultyTime = 0;
        Player = (SnakeController)Game.Spawn(PlayerObject, PlayerStartPosition);
        StartTime = Time.timeSinceLevelLoad;

        Time.timeScale = 1;
        App.Instance.SetScreen("Game");
        Audio.PlayMusic("Next", true);
    }

    //============================================================================================================================================================================================//
    public void Pause()
    {
        print("Frontend: Pause");   
   
        Time.timeScale = 0f;
        Audio.Music.Pause();
        App.Instance.SetScreen("Pause");
        gameObject.SetActive(false);
    }

    //============================================================================================================================================================================================//
    public void GameOver()
    {
        print("GAME OVER!");

        Time.timeScale = 0;
        CleanupScene();
        SetHighScore();
        Audio.PlaySound("Game Over");
        App.Instance.SetScreen("Game Over");
        Game.Instance.gameObject.SetActive(false);              
    }

    //============================================================================================================================================================================================//
    public void AddEnergy(int value)
    {
        SetEnergy(EnergyCount + value);
    }

    //============================================================================================================================================================================================//
    public void SetEnergy(int value)
    {
        EnergyCount = value;

        if (EnergyCount > EnergyPerBomb)
        {
            EnergyCount -= EnergyPerBomb;
            EnergyCount = 0;
        }

        EnergyBar.transform.localScale = new Vector3(EnergyCount / (float)EnergyPerBomb * 76, 1, 1);
    }

    //============================================================================================================================================================================================//
    public void AddScore(int value)
    {
        SetScore(Score + value);
    }

    //============================================================================================================================================================================================//
    public void SetScore(int value)
    {
        Score = value;
        ScoreText.Text = string.Format("{0:n0}", SmoothScore);
    }

    //============================================================================================================================================================================================//
    public void SetHighScore()
    {
        if (Score > Data.HighScore)
        {
            Data.HighScore = Score;
            print("New High Score: " + Score.ToString());

            HighScoreText.gameObject.SetActive(true);
        }
        else
            HighScoreText.gameObject.SetActive(false);
    }

    //============================================================================================================================================================================================//
    public void UpdateScore()
    {
        SmoothScore = SmoothScore * 0.8f + Score * 0.2f;
        ScoreText.Text = string.Format("{0:n0}", SmoothScore);
    }

    //============================================================================================================================================================================================//
    public void Death()
    {      
        Audio.PlaySound("Player Death");
    }

    //============================================================================================================================================================================================//
    public void CleanupScene()
    {
        GameObject objectsGroup = GameObject.Find("Objects");
        if (objectsGroup != null)
        {
            for (int i = 0; i < objectsGroup.transform.childCount; i++)
            {
                Destroy(objectsGroup.transform.GetChild(i).gameObject);
            }
        }
    }

    //============================================================================================================================================================================================//
    // Spawn objects into group so they can be easily cleanup up //
    //============================================================================================================================================================================================//
    static public Object Spawn(Object original)
    {
        return Game.Spawn(original, Vector3.zero, Quaternion.identity, false);
    }

    static public Object Spawn(Object original, Vector3 position)
    {
        return Game.Spawn(original, position, Quaternion.identity, false);
    }

    static public Object Spawn(Object original, Vector3 position, Quaternion rotation)
    {
        return Game.Spawn(original, position, rotation, false);
    }

    static public Object Spawn(Object original, Vector3 position, Quaternion rotation, bool isPermanent)
    {
        Object obj = Instantiate(original, position, rotation);

        if (!isPermanent)
        {
            if (Instance.ObjectGroup != null)
            {
                Transform xform = null;
                if (obj is GameObject)
                    xform = ((GameObject)obj).transform;
                else if (obj is Component)
                    xform = ((Component)obj).transform;

                if (xform != null)
                    xform.parent = Instance.ObjectGroup;
            }
        }

        return obj;
    }
}
