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
        LOGO = 0,
        INFO,
        Placeholder,
        PLAY,
	}

    public static string ButtonSelectionString(ButtonSelection selection)
	{
        switch (selection)
		{
            case ButtonSelection.PLAY: return "PLAY";
            case ButtonSelection.INFO: return "INFO";
            case ButtonSelection.Placeholder: return "";
            case ButtonSelection.LOGO: return "LOGO";
        }
        return "";
	}

    public GUISkin guiSkin;
    public GUIStyle guiStylePLAY;
    public GUIStyle guiStyleLOGO;
    public GUIStyle guiStyleINFO;
    public GUIStyle guiStyleCONF;
    public float buttonHeight = 100.0f;

    UIPanel panelController;
    InputActionAsset inputActions;


    ButtonSelection _currentSelection;
    ButtonSelection currentSelection
	{
        get { return _currentSelection; }
        set
		{
            _currentSelection = value;
            panelController.PositionPanel((CameraPanel.DisplayPosition)_currentSelection, UIPanel.MovementDirection.Up);
		}
	}

	private void Awake()
	{
		panelController = GetComponent<UIPanel>();
		Assert.IsNotNull(panelController);
		panelController.TeleportPanel();
		PlayerInput input = GetComponent<PlayerInput>();
		Assert.IsNotNull(input);
		if (input != null)
			inputActions = input.actions;
	}

	// Start is called before the first frame update
	void Start()
    {
        Assert.IsNotNull(guiSkin);
        Assert.IsNotNull(guiStylePLAY);
        Assert.IsNotNull(guiStyleLOGO);
        Assert.IsNotNull(guiStyleINFO);
        Assert.IsNotNull(guiStyleCONF);
        currentSelection = ButtonSelection.LOGO;
        inputActions.actionMaps[0].actions[0].performed += OnUp;
        inputActions.actionMaps[0].actions[1].performed += OnDown;
		inputActions.actionMaps[0].actions[2].performed += OnLeft;
		inputActions.actionMaps[0].actions[3].performed += OnRight;
        inputActions.actionMaps[0].actions[4].performed += OnConfirmSelection;
		inputActions.Enable();
    }

	private void OnDestroy()
	{
		inputActions.actionMaps[0].actions[0].performed -= OnUp;
		inputActions.actionMaps[0].actions[1].performed -= OnDown;
		inputActions.actionMaps[0].actions[2].performed -= OnLeft;
		inputActions.actionMaps[0].actions[3].performed -= OnRight;
		inputActions.actionMaps[0].actions[4].performed -= OnConfirmSelection;
	}

	GUIStyle ButtonSelectionStyle(ButtonSelection selection)
	{
        switch (selection)
        {
            case ButtonSelection.PLAY: return guiStylePLAY;
            case ButtonSelection.INFO: return guiStyleINFO;
            case ButtonSelection.Placeholder: return guiStyleCONF;
            case ButtonSelection.LOGO: return guiStyleLOGO;
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
        GUI.Box(new Rect(xLeft, yTop, buttonWidth, buttonHeight), "", ButtonSelectionStyle((ButtonSelection)CameraPanel.DisplayPosition.TopLeft));
        GUI.Box(new Rect(xRight, yTop, buttonWidth, buttonHeight), "", ButtonSelectionStyle((ButtonSelection)CameraPanel.DisplayPosition.TopRight));
        GUI.Box(new Rect(xLeft, yBottom, buttonWidth, buttonHeight), "", ButtonSelectionStyle((ButtonSelection)CameraPanel.DisplayPosition.BottomLeft));
        GUI.Box(new Rect(xRight, yBottom, buttonWidth, buttonHeight), "", ButtonSelectionStyle((ButtonSelection)CameraPanel.DisplayPosition.BottomRight));

        InputSystem.Update();
    }

	public void OnUp(InputAction.CallbackContext context)
	{
		SwitchPanelVertical();
	}

	public void OnDown(InputAction.CallbackContext context)
	{
		SwitchPanelHorizontal();
	}

	public void OnLeft(InputAction.CallbackContext context)
	{
		SwitchPanelVertical();
	}

	public void OnRight(InputAction.CallbackContext context)
	{
		SwitchPanelHorizontal();
	}

	void SwitchPanelVertical()
	{
		currentSelection = (ButtonSelection)CameraPanel.SwitchPositionVertical((CameraPanel.DisplayPosition)currentSelection);
    }

    void SwitchPanelHorizontal()
    {
		currentSelection = (ButtonSelection)CameraPanel.SwitchPositionHorizontal((CameraPanel.DisplayPosition)currentSelection);
    }

    void OnConfirmSelection(InputAction.CallbackContext context)
	{
        switch (currentSelection)
		{
            case ButtonSelection.PLAY:
                inputActions.Disable();
                SceneManager.LoadScene("MultipleCameras", LoadSceneMode.Single);
                break;
            case ButtonSelection.LOGO:
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
