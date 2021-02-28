using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoLabelController : MonoBehaviour
{
    public Font font;
    public Color32 labelBackgroundColour;
    public int fontSize;
    public int margin = 5;
    public string labelText;
    public TextAnchor alignment;
    public Vector2Int screenPos;
    GUIStyle guiStyle;
    Vector2 textSize;

    void Awake()
    {
        guiStyle = new GUIStyle();
        Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        texture.SetPixel(0, 0, labelBackgroundColour);
        texture.Apply();
        guiStyle.normal.background = texture;
        guiStyle.font = font;
        guiStyle.fontSize = fontSize;
        guiStyle.alignment = alignment;
        textSize = guiStyle.CalcSize(new GUIContent(labelText));
    }

	private void OnGUI()
	{
        GUI.Label(new Rect(screenPos, textSize + new Vector2(margin, margin)), labelText, guiStyle);
    }
}
