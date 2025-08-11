# Advanced Unity Programming: The Alchemist's Workshop

![Project Banner]()

This repository contains the project files for "The Alchemist's Workshop," a 4-hour course designed to teach advanced programming principles and system architecture in Unity. Students will progressively build a small, interactive scene where a player can move, manage an inventory, interact with objects, and save their progress.

The project is structured into five distinct versions (from V1.0 to V5.0), each corresponding to a scene that focuses on a specific set of advanced topics. This allows students to learn concepts in focused, manageable "bricks" without getting overwhelmed by boilerplate code.

## Core Concepts Covered

This course is designed to go beyond basic scripting and teach students how to think like software architects. Key topics include:

*   **S.O.L.I.D. Principles:** Practical, in-engine examples of Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, and Dependency Inversion.
*   **Unity Attributes:** Using attributes like `[Header]`, `[SerializeField]`, `[Tooltip]`, and `[Range]` to create clean, designer-friendly inspectors.
*   **Advanced C# Features:** Delegates, `Action` events, the Observer Pattern, `switch` expressions with type pattern matching, and coroutines for asynchronous operations.
*   **Architectural Patterns:** Implementing Singletons, data-driven design with Scriptable Objects, and decoupled UI systems.
*   **Robust Save/Load System:** Building a flexible persistence system using interfaces (`ISaveable`), JSON serialization (`Newtonsoft.Json`), and Data Transfer Objects (DTOs).
*   **Editor Tooling:** Using Gizmos (`OnDrawGizmosSelected`) to provide clear visual feedback for designers and developers.

---

## Project Structure & Course Flow

The project is divided into branches or folders, each representing a stage of the course. The learning path is designed to be completed sequentially from V1.0 to V5.0. Each version builds upon the last, with students filling in missing code to complete the exercises.

### **V1.0: The Designer's Item**
*   **Focus:** Attributes, Enums, Scriptable Objects, Inheritance & Liskov Substitution.
*   **Scene:** `1_Attributes_Scene`
*   **Student Objective:** Configure data scripts to be powerful and easy to use for designers.
    1.  **`ItemData.cs`**: Add `[Header]`, `[Tooltip]`, `[Space]`, and other attributes to organize the Inspector.
    2.  **`ItemType.cs`**: Create the `ItemType` enum.
    3.  **`UsableItemData.cs`**: Analyze this script to understand inheritance from `ItemData` (Liskov Principle).
    4.  **`HealthPotionData.cs`**: Uncomment the code and see how it inherits from `UsableItemData`.
    5.  **`PlayerInventoryManager.cs`**: Implement the `UpdateStatus()` method using `switch` and ternary operators.

### **V2.0: The Interactive UI**
*   **Focus:** Delegates, Events (Observer Pattern), and UI scripting.
*   **Scene:** `2_UI_Events_Scene`
*   **Student Objective:** Decouple the game's data layer from its presentation (UI) layer.
    1.  **`PlayerInventoryManager.cs`**: Declare a public `Action` event `OnInventoryChanged` and invoke it in `AddItem()` and `RemoveItem()`.
    2.  **`InventoryUI.cs`**: In `OnEnable()` and `OnDisable()`, subscribe and unsubscribe the `RedrawUI()` method to the manager's event.
    3.  **`InventorySlotUI.cs`**: Implement the `OnSlotClicked()` method to allow players to use or remove items from the inventory.

### **V3.0: Interacting with the World**
*   **Focus:** Interfaces (ISP), Raycasting, Coroutines, and Gizmos.
*   **Scene:** `3_Interaction_Scene`
*   **Student Objective:** Build a "move-then-interact" system for the player.
    1.  **`PlayerInteraction.cs`**: 
        *   Implement the `Update()` method to fire a raycast on mouse click.
        *   Start the `FollowAndInteract` coroutine if an interactable is hit.
        *   In the coroutine, use a `switch` with type pattern matching to check for `ICollectable`, `IActivatable`, etc., and call the correct method.
        *   Add `OnDrawGizmosSelected` to visualize the interaction radius.
    2.  **`WorldItem.cs`**: Uncomment the `ICollectable` interface.
    3.  **`IngredientStation.cs` / `CraftingTable.cs`**: Implement the `IActivatable` interface and its `Activate()` method.

### **V4.0: Making it Persistent**
*   **Focus:** Advanced Delegates, JSON Serialization, Singleton Pattern.
*   **Scene:** `4_Saving_Scene`
*   **Student Objective:** Implement a system to save and load the player's state.
    1.  **`PlayerHealth.cs`**: 
        *   Implement the `ISaveable` interface (`CaptureState` and `RestoreState`).
        *   Create and invoke a delegate/event for health changes.
    2.  **`PlayerHealthBarUI.cs`**: Subscribe to the `PlayerHealth` event to update the UI automatically.
    3.  **`WorldItemManager.cs`**: Implement the Singleton pattern to track all `WorldItem` objects.
    4.  **`WorldItem.cs`**: In `OnEnable()` and `OnDisable()`, register/unregister with the `WorldItemManager`.
    5.  **`GameManager.cs`**: Uncomment the code and fill in the `SaveGame()` and `LoadGame()` methods to handle saving player data and world item locations.

### **V5.0: The Final Polish**
*   **Focus:** Reviewing all concepts and ensuring the final project is robust and complete.
*   **Scene:** `5_Final_Scene`
*   **Student Objective:** This version represents the completed project. It serves as a reference and a demonstration of how all the systems work together in harmony.

---

## How to Use This Repository

1.  **Clone the Repository:**
    ```bash
    git clone [your-repository-url]
    ```
2.  **Open in Unity:** Open the cloned folder as a project in the Unity Hub (ensure you are using a compatible Unity version, e.g., 2022.3.x LTS or later).
3.  **Install Dependencies:** If prompted, install the **Newtonsoft Json.NET** package via the Unity Package Manager (`Window > Package Manager`).
4.  **Navigate the Scenes:** Open the `Scenes` folder in the Project window. You can start with `1_Attributes_Scene` and progress through to `5_Final_Scene`. Each scene is pre-configured for its corresponding lesson.
5.  **Follow the Code:** Open the scripts mentioned in each section's objectives. You will find `// TO-DO:` comments guiding you on what code to write.
6.  **Jump sections** If you are stuck you can use the different tags (1.0 to 5.0) to go to the corrected versions of each chapter. (You can use the .bat/.ps1/.sh scripts to jump with gototag.bat and the version e.g. gototag.bat\ 2.0 -> chapter II

Happy coding

