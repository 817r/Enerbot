## 🛠 Unity Version

This project is developed using **Unity 2022.3.20f1** (LTS).  
Please ensure you're using this version (or a compatible one) to avoid compatibility issues.

---

## 🧪 Editor Testing

To perform testing in the **Unity Editor**, make sure the GameObject **`GameManager`** is active.  
This is essential for handling scene transitions and core game logic.

> **Example scenes where `GameManager` is required:**  
> - `Background Selection`  
> - `Enerbot AI`

---

## 🌐 Building for Landscape Mode

To build the application in **landscape orientation**, follow these steps:

1. **Use a scene with the `_Landscape` suffix**, e.g., `MainMenu_Landscape`, `Quiz_Landscape`, etc.
2. **Disable the virtual keyboard** in the UI, if it's not required.
3. **Update the scene switching logic** in the `GameManager_Enerbot` script:
   - Append `_Landscape` to the scene name when calling the `GoToScene...` function:
     ```csharp
     GoToScene("YourSceneName_Landscape");
     ```

These steps ensure the application is correctly displayed and functions as expected in landscape mode.

