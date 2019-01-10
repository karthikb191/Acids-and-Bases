using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    public static LevelManager Instance { get; set; }
    
    bool shifting = false;
    Player player = null; Door door = null;

    bool startTimer = false;
    public float timer = 0.0f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        //Subscribe to shift room function
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        StartTimer();
    }

    private void Update()
    {
        if (startTimer)
            UpdateTimer();

        TestClearLevel();
    }


    void TestClearLevel()
    {
        Scene s = SceneManager.GetActiveScene();
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Attempting to save");
            if(s.buildIndex < GameManager.Instance.levelsCleared.Count)
            {
                Debug.Log("This level was already played. Checking high scores");
                if(timer < GameManager.Instance.levelsCleared[s.buildIndex].bestTime)
                {
                    GameManager.Instance.levelsCleared[s.buildIndex].bestTime = timer;
                    GameManager.Instance.levelsCleared[s.buildIndex].stars = 1;
                }
            }
            else
            {
                Debug.Log("NO entry found. Trying to add it");
                GameManager.Instance.levelsCleared.Add(new Level()
                {
                    bestTime = timer,
                    stars = 1
                });
            }
            CheckPointManager.RegisterCheckPoint();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            CheckPointManager.LoadCheckPoint();
        }
    }


    #region Room Shifting Functions
    //This function is called from the door that the player wants to open
    public void PrepareRoomShift(Player player, Door door)
    {
        if(door.connectingTo != null)
        {
            if(door.connectingTo.room != null && !shifting)
            {
                this.player = player;
                this.door = door;
                shifting = true;

                //Halt all the enemies temporarily
                for(int i = 0; i < player.enemiesChasing.Count; i++)
                {
                    player.enemiesChasing[i].GetComponent<Enemy>().Halt();
                    player.enemiesChasing[i].transform.parent = null;
                }
                
                //TODO: change the block duration to a dynamic value that it gets from the game manager
                player.BlockInputs(2.5f, true, true);

                StartCoroutine(GameManager.Instance.FadeIn());
                
                //Activate the room that is connected to the door
                door.connectingTo.room.ActivateRoom();

                //Make the target characters as children to the next room
                for (int i = 0; i < player.enemiesChasing.Count; i++)
                {
                    player.enemiesChasing[i].transform.parent = door.connectingTo.room.transform;
                }

                //Setting the characters to spawn in the destination room once the room has been activated
                door.connectingTo.room.SetCharactersToSpawn(player.enemiesChasing, door.connectingTo.transform.position);

                //Subscribing to game manager's fade out event so that the shift occurs at the appropriate time
                GameManager.Instance.FadeOutStartEvent += ShiftRoom;
                
            }
            Debug.Log("Room shift failed.");
        }
        else
        {
            Debug.Log("Room shift failed.");
        }
    }

    void ShiftRoom()
    {
        if(shifting && player && door)
        {
            //Subsrice to the event that spawns the enemies once the fade out is complete
            GameManager.Instance.FadeOutCompleteEvent += door.connectingTo.room.Spawn;


            Debug.Log("Shifting the room");
            //Disable the current room
            door.room.DeactivateRoom();

            //Shift the player's position to the room and to the transform of the door
            player.transform.position = door.connectingTo.transform.position;
            Vector3 cameraPostion = new Vector3(player.transform.position.x, player.transform.position.y,
                                            Camera.main.transform.position.z);

            if (Camera.main.GetComponent<CameraScript>() != null)
            {
                Camera.main.GetComponent<CameraScript>().SetPosition(cameraPostion);
                if(door.connectingTo.room.cameraBoundaries != null)
                    Camera.main.GetComponent<CameraScript>().boundariesContainer = door.connectingTo.room.cameraBoundaries;
                    
            }
            else
                Camera.main.transform.position = cameraPostion;

        }
        //Subscribe to game manager's fade out complete event
        GameManager.Instance.FadeOutCompleteEvent += ResetRoomShift;
        
    }

    void ResetRoomShift()
    {
        shifting = false;
        GameManager.Instance.FadeOutStartEvent -= ShiftRoom;
        GameManager.Instance.FadeOutCompleteEvent -= ResetRoomShift;

        //Unsubscribe the room spawn once the fade is complete
        GameManager.Instance.FadeOutCompleteEvent -= door.connectingTo.room.Spawn;
    }

    public bool IsShiftingRoom()
    {
        return shifting;
    }

    #endregion

    public void StartTimer()
    {
        startTimer = true;
    }

    void UpdateTimer()
    {
        timer += GameManager.Instance.DeltaTime;
    }

    public void StopTimer()
    {
        startTimer = false;
    }
    public void ResetTimer()
    {
        timer = 0.0f;
    }


}
