Alien Creep - simple prototype game example

This is a simple example of a prototype game, similar to Space Invaders. It's in the form of a Unity project - download the .zip and extract to a new folder, then open in Unity. Created in Unity 2021.3.13f1.

In Unity, load the 'MainGame' scene from the 'Scenes' folder.

The game is designed to run in 2:3 screen ratio - for simplicty there are no checks made for screen size/ratio.

Scripts:

    BulletContoller.cs : Component for bullet prefabs. Handles bullet movement and destruction
    EnemyGroupContoller.cs : Creates and Moves the group of enemies. Does the win/lose checking
    EnemyShipCollison.cs : Component for enemy prefabs. Informs GameManager when destroyed for score and any callbacks
    GameManager.cs : Controls game states, tracks score and provides events/callback invocation.
    GunController.cs : Component for gun object. Controls mouse/touch input for moving the gun
    Shoot.cs : Component for gun object. Contols shooting bullets at regular intervals
    UIManager.cs : Controls showing/hiding of UI elements, updating texts and score objects

Scene Hierarchy:

    Main Camera : Orthographic camera, black background, Sized and positioned to cover 10 units of world space when 2:3 ratio
    GameManager : Empty GameObject for the GameManager component
    Enemies : Empty GameObject for the EnemyGroupContoller component - enemies are instantiated as children of this object.
    Gun : Gun prefab, ready to go.
    TopCollider : GameObject with Box Collider that acts as a mop-up for the bullets if they miss
    UICanvas : Screen Space - Overlay Canvas, with UIManager component
        BTN_Play : UI Button, for starting the game
        TXT_Messages : Text holder for the Title/Win/Lose messages
        TXT_Score : Text holder for the Score
    EventSystem : Auto-included by Unity, provides input handling

Alan Tinsley