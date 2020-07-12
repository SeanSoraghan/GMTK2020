using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public enum ButtonSelection : int
	{
        PLAY = 0,
        INFO,
        Placeholder,
        EXIT,
	}

    public static string ButtonSelectionString(ButtonSelection selection)
	{
        switch (selection)
		{
            case ButtonSelection.PLAY: return "PLAY";
            case ButtonSelection.INFO: return "INFO";
            case ButtonSelection.Placeholder: return "";
            case ButtonSelection.EXIT: return "EXIT";
        }
        return "";
	}

    public static CameraPanel.DisplayPosition ButtonSelectionDisplayPosition(ButtonSelection selection)
	{
        switch (selection)
		{
            case ButtonSelection.PLAY: return CameraPanel.DisplayPosition.BottomRight;
            case ButtonSelection.INFO: return CameraPanel.DisplayPosition.TopRight;
            case ButtonSelection.Placeholder: return CameraPanel.DisplayPosition.BottomLeft;
            case ButtonSelection.EXIT: return CameraPanel.DisplayPosition.TopLeft;
        }
        return CameraPanel.DisplayPosition.TopLeft;
	}

    public static ButtonSelection DisplayPositionToButtonSelection(CameraPanel.DisplayPosition pos)
    {
        switch (pos)
        {
            case CameraPanel.DisplayPosition.TopLeft: return ButtonSelection.EXIT;
            case CameraPanel.DisplayPosition.TopRight: return ButtonSelection.INFO;
            case CameraPanel.DisplayPosition.BottomLeft: return ButtonSelection.Placeholder;
            case CameraPanel.DisplayPosition.BottomRight: return ButtonSelection.PLAY;
        }
        return ButtonSelection.PLAY;
    }

 //   public static string GetStringForButtonPosition(CameraPanel.DisplayPosition pos)
	//{
 //       return ButtonSelectionString(ButtonSelectionDisplayPosition())
	//}

    public GUISkin guiSkin;
    public GUIStyle guiStylePLAY;
    public GUIStyle guiStyleEXIT;
    public GUIStyle guiStyleINFO;
    public GUIStyle guiStyleCONF;
    public float buttonHeight = 100.0f;

    UIController2 panelController;
    InputActionAsset inputActions;

    ButtonSelection _currentSelection;
    ButtonSelection currentSelection
	{
        get { return _currentSelection; }
        set
		{
            _currentSelection = value;
            panelController.PositionPanelUI(ButtonSelectionDisplayPosition(_currentSelection));
		}
	}
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(guiSkin);
        Assert.IsNotNull(guiStylePLAY);
        Assert.IsNotNull(guiStyleEXIT);
        Assert.IsNotNull(guiStyleINFO);
        Assert.IsNotNull(guiStyleCONF);
        panelController = GetComponent<UIController2>();
        Assert.IsNotNull(panelController);
        currentSelection = ButtonSelection.EXIT;
        panelController.TeleportPanel();
        PlayerInput input = GetComponent<PlayerInput>();
        Assert.IsNotNull(input);
        if (input != null)
            inputActions = input.actions;
        inputActions.actionMaps[0].actions[0].performed += OnConfirmSelection;
        inputActions.actionMaps[0].actions[1].performed += OnSwitchSelectionVertical;
        inputActions.actionMaps[0].actions[2].performed += OnSwitchSelectionHorizontal;
        inputActions.Enable();
    }

	private void OnDestroy()
	{
        inputActions.actionMaps[0].actions[0].performed -= OnConfirmSelection;
        inputActions.actionMaps[0].actions[1].performed -= OnSwitchSelectionVertical;
        inputActions.actionMaps[0].actions[2].performed -= OnSwitchSelectionHorizontal;
    }

	GUIStyle ButtonSelectionStyle(ButtonSelection selection)
	{
        switch (selection)
        {
            case ButtonSelection.PLAY: return guiStylePLAY;
            case ButtonSelection.INFO: return guiStyleINFO;
            case ButtonSelection.Placeholder: return guiStyleCONF;
            case ButtonSelection.EXIT: return guiStyleEXIT;
        }
        return guiStylePLAY;
    }
    // Update is called once per frame
    void OnGUI()
    {
        if (guiSkin != null)
            GUI.skin = guiSkin;

        float w = Screen.width;
        float h = Screen.height;

        float aspect = 445.0f / 140.0f;
        float buttonWidth = buttonHeight * aspect;
        float bW = buttonWidth * 0.5f;
        float bH = buttonHeight * 0.5f;
        float xLeft = w * 0.25f - bW;
        float xRight = w * 0.75f - bW;
        float yTop = h * 0.25f - bH;
        float yBottom = h * 0.75f - bH;

        float logoSize = w > h ? w * 0.5f * 0.4f : h * 0.5f * 0.4f;
        float logoX = w * 0.25f - logoSize * 0.5f;
        float logoY = h * 0.25f - logoSize * 0.5f;
        GUI.Box(new Rect(logoX, logoY, logoSize, logoSize), "", ButtonSelectionStyle(DisplayPositionToButtonSelection(CameraPanel.DisplayPosition.TopLeft)));
        GUI.Box(new Rect(xRight, yTop, buttonWidth, buttonHeight), "", ButtonSelectionStyle(DisplayPositionToButtonSelection(CameraPanel.DisplayPosition.TopRight)));
        GUI.Box(new Rect(xLeft, yBottom, buttonWidth, buttonHeight), "", ButtonSelectionStyle(DisplayPositionToButtonSelection(CameraPanel.DisplayPosition.BottomLeft)));
        GUI.Box(new Rect(xRight, yBottom, buttonWidth, buttonHeight), "", ButtonSelectionStyle(DisplayPositionToButtonSelection(CameraPanel.DisplayPosition.BottomRight)));

        InputSystem.Update();
    }

    void OnSwitchSelectionVertical(InputAction.CallbackContext context)
	{
        switch (ButtonSelectionDisplayPosition(currentSelection))
        {
            case CameraPanel.DisplayPosition.TopLeft:
                currentSelection = DisplayPositionToButtonSelection(CameraPanel.DisplayPosition.BottomLeft);
                break;
            case CameraPanel.DisplayPosition.TopRight:
                currentSelection = DisplayPositionToButtonSelection(CameraPanel.DisplayPosition.BottomRight);
                break;
            case CameraPanel.DisplayPosition.BottomLeft:
                currentSelection = DisplayPositionToButtonSelection(CameraPanel.DisplayPosition.TopLeft);
                break;
            case CameraPanel.DisplayPosition.BottomRight:
                currentSelection = DisplayPositionToButtonSelection(CameraPanel.DisplayPosition.TopRight);
                break;
        }
    }

    void OnSwitchSelectionHorizontal(InputAction.CallbackContext context)
    {
        switch (ButtonSelectionDisplayPosition(currentSelection))
        {
            case CameraPanel.DisplayPosition.TopLeft:
                currentSelection = DisplayPositionToButtonSelection(CameraPanel.DisplayPosition.TopRight);
                break;
            case CameraPanel.DisplayPosition.TopRight:
                currentSelection = DisplayPositionToButtonSelection(CameraPanel.DisplayPosition.TopLeft);
                break;
            case CameraPanel.DisplayPosition.BottomLeft:
                currentSelection = DisplayPositionToButtonSelection(CameraPanel.DisplayPosition.BottomRight);
                break;
            case CameraPanel.DisplayPosition.BottomRight:
                currentSelection = DisplayPositionToButtonSelection(CameraPanel.DisplayPosition.BottomLeft);
                break;
        }
    }

    void OnConfirmSelection(InputAction.CallbackContext context)
	{
        switch (currentSelection)
		{
            case ButtonSelection.PLAY:
                inputActions.Disable();
                SceneManager.LoadScene("MultipleCameras", LoadSceneMode.Single);
                break;
            case ButtonSelection.EXIT:
                //Application.Quit();
                break;
            case ButtonSelection.Placeholder:
                Debug.Log("Placeholder");
                break;
            case ButtonSelection.INFO:
                Debug.Log("Load info menu");
                break;
        }
	}
}
