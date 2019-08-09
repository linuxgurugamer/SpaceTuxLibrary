using UnityEngine;
using ToolbarControl_NS;
using ClickThroughFix;


namespace KSPColorPicker
{

    // Based on the following:  https://gist.github.com/boj/1181465

        
    public class KSP_ColorPicker : MonoBehaviour
    {
        // Public fields

        public static KSP_ColorPicker colorPickerInstance = null;
        public static bool showPicker = false;
        public static bool success = false;       

        // the color that has been chosen
        private static Color selectedColor;
        public static Color SelectedColor {  get { return selectedColor; } }


        // Private and internal fields
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

        private bool activePingsNeeded = false;

        private Rect _windowPos;
        private GUIStyle saturationTextureStyle;
        private GUIStyle styleTextureStyle;
        private string texturePath = "GameData/KSP_ColorPicker/PluginData/colorpicker_texture";

        double lastTimePinged = 0;
        public void _PingTime()
        {
            lastTimePinged = Time.realtimeSinceStartup;
        }

        public static KSP_ColorPicker CreateColorPicker(Color initialColor, string texturePath, bool activePingsNeeded = true)
        {
            return CreateColorPicker(initialColor, false, texturePath, 0, 0, activePingsNeeded, true);
        }
        public static KSP_ColorPicker CreateColorPicker(Color initialColor, string texturePath = null, bool activePingsNeeded = true, bool destroyOnClose = true)
        {
            return  CreateColorPicker(initialColor, false, texturePath, 0, 0, activePingsNeeded, destroyOnClose);
        }

        public static KSP_ColorPicker CreateColorPicker(Color initialColor, bool useDefinedPosition = false, string texturePath = null,
            int pLeft = 0, int pTop = 0, bool activePingsNeeded = true, bool destroyOnClose = true)
        {
            GameObject go = new GameObject();
            colorPickerInstance = go.AddComponent<KSP_ColorPicker>();
            colorPickerInstance.positionLeft = pLeft;
            colorPickerInstance.positionTop = pTop;
            if (texturePath != null)
                colorPickerInstance.texturePath = texturePath;

            colorPickerInstance.destroyOnClose = destroyOnClose;
            colorPickerInstance.StartUp();
            colorPickerInstance.lastSetColor = initialColor;
            showPicker = true;
            success = false;
            return colorPickerInstance;
        }

        void StartUp()
        {
            if (!useDefinedPosition)
            {
                positionLeft = (Screen.width / 2) - (textureWidth / 2);
                positionTop = (Screen.height / 2) - (textureHeight / 2);
            }
            colorPicker = new Texture2D(2, 2);
            bool rc = ToolbarControl.LoadImageFromFile(ref colorPicker, texturePath);
            
            if (!rc)
            {
                Log.Error("Unable to load ColorPicker image");
                CloseAndDestroy();

            }
            colorPicker.Apply();
            displayPicker = colorPicker;

            if (!useDefinedSize)
            {
                textureWidth = colorPicker.width;
                textureHeight = colorPicker.height;
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
            _windowPos = new Rect(positionLeft, positionTop, textureWidth + 70, textureHeight + 70);

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


        void OnGUI()
        {
            if (activePingsNeeded && Time.realtimeSinceStartup - lastTimePinged > 0.2f)
                CloseAndDestroy();
            
            if (!showPicker) return;
            _windowPos = ClickThruBlocker.GUIWindow(3456789, _windowPos, DrawWindowContents, "Color Picker");
        }


        const int BOTTOM_RIGHT_MARGIN = 60;
        void DrawWindowContents(int id)
        {
            GUI.Box(new Rect(3, 3, textureWidth + BOTTOM_RIGHT_MARGIN, textureHeight + BOTTOM_RIGHT_MARGIN), "");

            if (GUI.RepeatButton(new Rect(6, 6, textureWidth + 10, textureHeight + 10), displayPicker, GUIStyle.none))
            {
                int a = (int)Input.mousePosition.x - (int)_windowPos.x;
                int b = textureHeight - (int)((Screen.height - Input.mousePosition.y) - (int)_windowPos.y);

                selectedColor = displayPicker.GetPixel(a, b);

                lastSetColor = selectedColor;
            }

            saturationSlider = GUI.VerticalSlider(new Rect(textureWidth + 13, 6, 10, textureHeight), saturationSlider, 1, -1);
            SetSaturationTexture(lastSetColor);
            selectedColor = lastSetColor + new Color(saturationSlider, saturationSlider, saturationSlider);


            GUI.Box(new Rect(textureWidth + 30, 6, 20, textureHeight), saturationTexture, saturationTextureStyle);

            if (GUI.Button(new Rect(textureWidth - 120, textureHeight + 10, 60, 25), "Cancel"))
            {
                // hide picker
                showPicker = false;
                if (destroyOnClose)
                {
                    colorPickerInstance = null;
                    Destroy(this);
                }
            }
            if (GUI.Button(new Rect(textureWidth - 60, textureHeight + 10, 60, 25), "Accept"))
            {
                selectedColor = styleTexture.GetPixel(0, 0);
                success = true;
                CloseAndDestroy();  
            }

            // color display

            styleTexture.SetPixel(0, 0, selectedColor);
            styleTexture.Apply();

            styleTextureStyle.normal.background = styleTexture;
            GUI.Box(new Rect(textureWidth + 10, textureHeight + 10, 30, 30), new GUIContent(""), styleTextureStyle);

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
