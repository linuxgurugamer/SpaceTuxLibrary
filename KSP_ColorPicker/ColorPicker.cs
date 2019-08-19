using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ToolbarControl_NS;
using ClickThroughFix;
using KSP_Log;

namespace KSPColorPicker
{

    // Based on the following:  https://gist.github.com/boj/1181465


    public class KSP_ColorPicker : MonoBehaviour
    {
        // Public fields

        public static KSP_ColorPicker colorPickerInstance = null;
        public static bool showPicker { get; private set; } = false;
        public static bool success { get; private set; } = false;

        // the color that has been chosen
        private static Color selectedColor;
        public static Color SelectedColor { get { return selectedColor; } }


        // Private and internal fields

        private static Log Log = new Log("ColorPicker");

        internal bool destroyOnClose = false;
        internal bool useDefinedPosition = false;
        internal int positionLeft = 0;
        internal int positionTop = 0;

        // the solid texture which everything is compared against
        internal Texture2D colorPicker;

        // the picker being displayed
        private Texture2D displayPicker;


        private Color lastSetColor;

        internal bool useDefinedSize = false;
        internal int textureWidth = 400;
        internal int textureHeight = 400;

        private float saturationSlider = 0.0F;
        private Texture2D saturationTexture;
        private Texture2D styleTexture;

        private bool activePingsNeeded = true;

        private Rect _windowPos;
        private GUIStyle saturationTextureStyle;
        private GUIStyle styleTextureStyle;
        private string texturePath = "ColorPicker-Texture";
        private const string TEXTURES = "GameData/SpaceTuxLibrary/PluginData/Images";
        double lastTimePinged = 0;

        Settings settings = new Settings();

        public void PingTime()
        {
            lastTimePinged = Time.realtimeSinceStartup;
        }

        public static string[] AvailableTextures()
        {
            string[] png = Directory.GetFiles(TEXTURES, "*.png");

            string[] result = png.OrderBy(x => x).ToArray();
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = result[i].Substring(TEXTURES.Length, result[i].Length - TEXTURES.Length - 4);
            }
            return result;
        }

        public static KSP_ColorPicker CreateColorPicker(Color initialColor, string texturePath, bool activePingsNeeded = true)
        { return CreateColorPicker(initialColor, false, texturePath, 0, 0, activePingsNeeded, true); }

        public static KSP_ColorPicker CreateColorPicker(Color initialColor, string texturePath = null, bool activePingsNeeded = true, bool destroyOnClose = true)
        { return CreateColorPicker(initialColor, false, texturePath, 0, 0, activePingsNeeded, destroyOnClose); }

        public static KSP_ColorPicker CreateColorPicker(Color initialColor, bool useDefinedPosition = false, string texturePath = null,
            int pLeft = 0, int pTop = 0, bool activePingsNeeded = true, bool destroyOnClose = true)
        {
            GameObject go = new GameObject();
            colorPickerInstance = go.AddComponent<KSP_ColorPicker>();
            colorPickerInstance.positionLeft = pLeft;
            colorPickerInstance.positionTop = pTop;
            if (texturePath != null)
                colorPickerInstance.texturePath = texturePath;

            pickerTextures = AvailableTextures();

            // If it's not a rooted path, then assume it's in a subdir of the TEXTURES directory
            if (!System.IO.Path.IsPathRooted(colorPickerInstance.texturePath))
                colorPickerInstance.texturePath = TEXTURES + colorPickerInstance.texturePath;

            colorPickerInstance.destroyOnClose = destroyOnClose;
            colorPickerInstance.FindInitTexture();
            colorPickerInstance.LoadAndInitTexture();
            colorPickerInstance.lastSetColor = initialColor;
            showPicker = true;
            success = false;
            return colorPickerInstance;
        }

        public bool UseKSPskin { private get; set; } = true;
        static string[] pickerTextures;

        int? curTextureNum = null;

        void FindInitTexture()
        {
            if (colorPickerInstance.settings != null)
            {
                curTextureNum = colorPickerInstance.settings.selectedColorPicker;
                if (curTextureNum >= 0 && curTextureNum < pickerTextures.Length)
                {
                    texturePath = TEXTURES + pickerTextures[(int)curTextureNum];
                }
            }
        }

        void LoadAndInitTexture()
        {
            colorPicker = new Texture2D(2, 2);
            bool rc = ToolbarControl.LoadImageFromFile(ref colorPicker, texturePath);
            curTextureNum = null;
            for (int i = 0; i < pickerTextures.Length; i++)
            {
                if (texturePath == TEXTURES + pickerTextures[i])
                {
                    curTextureNum = i;
                    break;
                }
            }
            if (settings != null && curTextureNum != null)
                settings.Save((int)curTextureNum);
            if (!rc)
            {
                Log.Error("Unable to load ColorPicker image");
                CloseAndDestroy();

            }
            colorPicker.Apply();
            displayPicker = colorPicker;
#if false
            if (!useDefinedSize)
            {
                textureWidth = Math.Min(400, colorPicker.width);
                textureHeight = Math.Min(400, colorPicker.height);
            }
#endif
            if (!useDefinedPosition)
            {
                positionLeft = (Screen.width / 2) - (textureWidth / 2);
                positionTop = (Screen.height / 2) - (textureHeight / 2);
            }
            saturationTexture = new Texture2D(20, textureHeight);
            SetSaturationTexture(Color.black);

            saturationTextureStyle = new GUIStyle();
            saturationTextureStyle.stretchHeight = true;
            saturationTextureStyle.stretchWidth = true;

            selectedColor = lastSetColor;
            // small color picker box texture
            styleTexture = new Texture2D(1, 1);
            styleTexture.SetPixel(0, 0, selectedColor);
            _windowPos = new Rect(positionLeft, positionTop, textureWidth + 70, textureHeight + 70+ TITLE_HEIGHT);

            styleTextureStyle = new GUIStyle();
        }


        void SetSaturationTexture(Color c)
        {
            for (int j = 0; j < textureHeight; j++)
            {
                float v = ((float)(j + 1) / (float)textureHeight * 2.0f) - 1f;
                Color newColor = c + new Color(v, v, v, 1);

                for (int i = 0; i < saturationTexture.width; i++)
                    saturationTexture.SetPixel(i, j, newColor);
            }
            saturationTexture.Apply();
        }

        bool reloadTexture = false;
        void OnGUI()
        {
            if (UseKSPskin)
                GUI.skin = HighLogic.Skin;

            if (activePingsNeeded && Time.realtimeSinceStartup - lastTimePinged > 0.25f)
                CloseAndDestroy();

            if (!showPicker) return;
            if (reloadTexture)
            {
                LoadAndInitTexture();
            }
            _windowPos = ClickThruBlocker.GUIWindow(3456789, _windowPos, DrawWindowContents, "Color Picker");
        }


        const int BOTTOM_RIGHT_MARGIN = 60;
        const int BUTTON_HEIGHT = 25;
        const int BUTTON_WIDTH = 60;
        const int TITLE_HEIGHT = 25;
        void DrawWindowContents(int id)
        {
            GUI.Box(new Rect(3, TITLE_HEIGHT +3, textureWidth + BOTTOM_RIGHT_MARGIN, textureHeight + BOTTOM_RIGHT_MARGIN), "");

            if (GUI.RepeatButton(new Rect(6, TITLE_HEIGHT + 6, textureWidth + 10, textureHeight + 10), displayPicker, GUIStyle.none))
            {
                int a = (int)Input.mousePosition.x - (int)_windowPos.x;
                int b = textureHeight - (int)((Screen.height - Input.mousePosition.y) - (int)_windowPos.y);

                selectedColor = displayPicker.GetPixel(a, b);

                lastSetColor = selectedColor;
            }

            saturationSlider = GUI.VerticalSlider(new Rect(textureWidth + 13, TITLE_HEIGHT + 6, 10, textureHeight), saturationSlider, 1, -1);
            SetSaturationTexture(lastSetColor);
            selectedColor = lastSetColor + new Color(saturationSlider, saturationSlider, saturationSlider);


            GUI.Box(new Rect(textureWidth + 30, TITLE_HEIGHT + 6, 20, textureHeight), saturationTexture, saturationTextureStyle);

            if (curTextureNum != null)
            {
                if (GUI.Button(new Rect(10, TITLE_HEIGHT + textureHeight + (BOTTOM_RIGHT_MARGIN - BUTTON_HEIGHT) / 2, 50, BUTTON_HEIGHT), "<"))
                {
                    curTextureNum--;
                    if (curTextureNum < 0)
                        curTextureNum = pickerTextures.Length - 1;
                    colorPickerInstance.texturePath = TEXTURES + pickerTextures[(int)curTextureNum];
                    reloadTexture = true;
                }
                if (GUI.Button(new Rect(50, TITLE_HEIGHT + textureHeight + (BOTTOM_RIGHT_MARGIN - BUTTON_HEIGHT) / 2, 50, BUTTON_HEIGHT), ">"))
                {
                    curTextureNum++;
                    if (curTextureNum >= pickerTextures.Length)
                        curTextureNum = 0;
                    colorPickerInstance.texturePath = TEXTURES + pickerTextures[(int)curTextureNum];
                    reloadTexture = true;
                }
            }

            if (GUI.Button(new Rect(1200, TITLE_HEIGHT + textureHeight + (BOTTOM_RIGHT_MARGIN - BUTTON_HEIGHT) / 2, BUTTON_WIDTH, BUTTON_HEIGHT), "Cancel"))
            {
                // hide picker
                showPicker = false;
                if (destroyOnClose)
                {
                    colorPickerInstance = null;
                    Destroy(this);
                }
            }
            if (GUI.Button(new Rect(10 + textureWidth / 2, TITLE_HEIGHT + textureHeight + (BOTTOM_RIGHT_MARGIN - BUTTON_HEIGHT) / 2, BUTTON_WIDTH, BUTTON_HEIGHT), "Accept"))
            {
                selectedColor = styleTexture.GetPixel(0, 0);
                success = true;
                CloseAndDestroy();
            }


            GUI.Label(new Rect(textureWidth - BUTTON_WIDTH * 2, TITLE_HEIGHT + textureHeight + (BOTTOM_RIGHT_MARGIN - BUTTON_HEIGHT) / 2, BUTTON_WIDTH * 2, BUTTON_HEIGHT), "Selected Color -->");

            // color display

            styleTexture.SetPixel(0, 0, selectedColor);
            styleTexture.Apply();

            styleTextureStyle.normal.background = styleTexture;
            GUI.Box(new Rect(textureWidth + 10, TITLE_HEIGHT + textureHeight + 10, 30, 30), new GUIContent(""), styleTextureStyle);

            GUI.DragWindow();
        }

        void CloseAndDestroy()
        {
            // hide picker
            showPicker = false;

            if (destroyOnClose)
            {
                colorPickerInstance = null;
                Destroy(this);
            }
        }
    }
}
