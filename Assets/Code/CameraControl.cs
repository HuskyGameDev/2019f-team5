using UnityEngine;
using System.Collections;
using System;

// This is a custom camera class that follows the player around but restricts the view to the edge of the dungeon
// This prevents the player from seeing "off the map" and increases the atmospheric feel of the game.
public class CameraControl : MonoBehaviour{

    Transform player;               // OUR TARGET TO TRACK
    float xOffset = 0;              // camera's X offset from the player
    float yOffset = 0;              // camera's Y offset from the player
    float followSpeed = 100000;      // speed that the camera will follow the player
    bool xLocked = false;           // whether or not the camera is locked on the current X axis
    bool yLocked = false;           // whether or not the camera is locked on the current Y axis
    bool playerLocked = false;
    World world;
    RectInt levelBounds;


    // Use this for initialization
    void Start(){
               
        // subscribes the "playerSpawned" event to our method
        // that will set the focus of our camera on the player
        // EventManager.Instance.Subscribe(GameEvent.PlayerSpawned, onPlayerSpawned);

        // locks onto and snaps camera to the player on initialization
        player = GameObject.FindWithTag("Player").transform;
        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);

        // gets the world, and subscribes to the world-spawned event
        world = GameObject.FindWithTag("Manager").GetComponent<World>();
        EventManager.Instance.Subscribe(GameEvent.LevelGenerated, OnWorldLoaded);
        followSpeed = 5;
    }

    // Update is called once per frame
    void Update(){

        if ( ! playerLocked ){
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
            return;
        }

        float playerX = player.position.x + xOffset;
        float playerY = player.position.y + yOffset;
        float newX = transform.position.x;
        float newY = transform.position.y;

        // locks camera for reaching the edge of the left and right of the dungeon
        if ( !xLocked && (transform.position.x + 5 > levelBounds.xMax ||
                          transform.position.x - 5 < levelBounds.xMin)) {
            xLocked = true;
        }

        // locks camera for reaching the edge of the top and bottom of the dungeon
        if (yLocked && (transform.position.y + 5 <= levelBounds.yMax ||
                        transform.position.y - 5 <= levelBounds.yMin)) {
            yLocked = false;
        }
        
        // updates 
        if (!xLocked) {
            newX = Mathf.Lerp(transform.position.x, playerX, Time.deltaTime * followSpeed);
        }
        if (!yLocked) {
            newY = Mathf.Lerp(transform.position.y, playerY, Time.deltaTime * followSpeed);
        }

        // updates camera position
        transform.position = new Vector3(newX, newY, transform.position.z);
    }

    // sets the player to the target to track
    //public void onPlayerSpawned(object playerObject){
    //    targetToTrack = GameObject.FindWithTag("Player").transform;
    //}

    // retrieves the world's level bounds once the procedural generation is complete
    public void OnWorldLoaded(object worldobject) {
        levelBounds = world.GetBounds();
    }
}