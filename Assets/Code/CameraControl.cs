﻿using UnityEngine;

// This is a custom camera class that follows the player around but restricts the view to the edge of the dungeon
// This prevents the player from seeing "off the map" and increases the atmospheric feel of the game.
public class CameraControl : MonoBehaviour{

    //private Camera mainCamera;
    private Transform player;           // OUR TARGET TO TRACK
    private float xOffset = 0;          // camera's X offset from the player
    private float yOffset = 0;          // camera's Y offset from the player
    private float followSpeed;          // speed that the camera will follow the player
    private World world;                // world object for retrieving bounds
    private RectInt levelBounds;        // outer bounds of the generated level
    private float cameraOrthoSize;      // distance from center of camera to outer Y bounds
    private float cameraHeight;         // essentially orthoSize
    private float cameraWidth;          // height * adjustment aspect ratio
    
    void Awake()
    {
        // locks onto and snaps camera to the player on initialization
        player = GameObject.FindWithTag("Player").transform;

        // gets the world, and subscribes to the world-spawned and player-spawned event
        world = GameObject.FindWithTag("Manager").GetComponent<World>();

        EventManager.Instance.Subscribe( GameEvent.LevelGenerated, OnWorldLoaded );
        EventManager.Instance.Subscribe( GameEvent.PlayerSpawned, OnPlayerSpawned );
    }

    // Update is called once per frame
    void Update(){
        
        // gets the player's new location, adjusts for follow speed
        float playerX = player.position.x + xOffset;
        float playerY = player.position.y + yOffset;
        float newX = Mathf.Lerp( transform.position.x, playerX, Time.deltaTime * followSpeed );
        float newY = Mathf.Lerp( transform.position.y, playerY, Time.deltaTime * followSpeed );

        // clamps camera to edge of screen when at the outer bounds of the map
        newX = Mathf.Clamp(newX, levelBounds.xMin + cameraWidth - 1, levelBounds.xMax - cameraWidth + 1);
        newY = Mathf.Clamp(newY, levelBounds.yMin + cameraHeight - 1, levelBounds.yMax - cameraHeight + 1);

        // updates camera position
        transform.position = new Vector3(newX, newY, transform.position.z);
    }
    
    // retrieves the world's level bounds once the procedural generation is complete
    private void OnWorldLoaded(object worldObject) {
        levelBounds = world.GetBounds();

        // camera clamp setup
        cameraOrthoSize = 9;                           // hard-coded in for now
        cameraHeight = cameraOrthoSize;
        cameraWidth = (cameraOrthoSize) * (16.0f / 9.0f);
    }

    // notifies camera to find the player
    private void OnPlayerSpawned(object playerObject){
        transform.position = new Vector3( player.position.x, player.position.y, transform.position.z );
        followSpeed = 3;
    }
}