using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace RandomizerMod
{
    public class GUIController : MonoBehaviour
    {
        public static bool loadedSave = false;

        private Dictionary<string, Texture2D> textures;
        public GUIStyle style;

        private string seedString;
        public static int seed = -1;
        public static bool hardMode;
        public static bool randomize;

        private System.Random rnd = new System.Random();

        public GUIController()
        {
            this.seedString = rnd.Next().ToString();
        }

        public void Awake()
        {
            this.textures = new Dictionary<string, Texture2D>();
            Texture2D logo = new Texture2D(1, 1);
            Texture2D off = new Texture2D(1, 1);
            Texture2D easy = new Texture2D(1, 1);
            Texture2D hard = new Texture2D(1, 1);
            logo.LoadImage(Properties.Resources.logo_png);
            off.LoadImage(Properties.Resources.off_png);
            easy.LoadImage(Properties.Resources.easy_png);
            hard.LoadImage(Properties.Resources.hard_png);
            this.textures.Add("logo", logo);
            this.textures.Add("off", off);
            this.textures.Add("easy", easy);
            this.textures.Add("hard", hard);
        }

        public void OnGUI()
        {
            //TODO: Less ugly text box
            if (this.style == null)
            {
                this.style = GUI.skin.textField;
                this.style.fontSize = 64;
            }

            //Save all GUI properties before changing them so we don't mess anything up with other UI elements
            int depth = GUI.depth;
            Matrix4x4 matrix = GUI.matrix;
            Color backgroundColor = GUI.backgroundColor;
            Color contentColor = GUI.contentColor;
            Color color = GUI.color;

            GUI.depth = 1;
            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
            GUI.color = Color.white;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3((float)Screen.width / 1920f, (float)Screen.height / 1080f, 1f));

            if (GameManager.instance.sceneName == "Menu_Title")
            {
                //alpha > 0f is true when the screen is uninitialized, but since it's the first screen that doesn't matter
                if (UIManager.instance.mainMenuScreen.alpha > 0f)
                {
                    //Draw logo in bottom right
                    GUI.color = new Color(color.r, color.g, color.b, UIManager.instance.mainMenuScreen.alpha);
                    GUI.DrawTexture(new Rect(1515f, 1020f, 409f, 66f), this.textures["logo"], ScaleMode.ScaleToFit);

                    //Keeping track of whether a save has been loaded because LoadGame is called when still on the save profiles screen
                    if (loadedSave == true)
                    {
                        RandomizerMod.Log("turning off loadedSave");
                    }
                    loadedSave = false;
                }

                //Thankfully not a CanvasGroup like the others, so we can use isActiveAndEnabled
                //Can't just check MainMenuState because menus are visible outside of their state due to the fade in/out
                if (UIManager.instance.playModeMenuScreen.isActiveAndEnabled)
                {
                    //Set alpha to that of the screen for the fancy fade in/out
                    GUI.color = new Color(color.r, color.g, color.b, UIManager.instance.playModeMenuScreen.screenCanvasGroup.alpha);

                    //Create button for off/easy/hard
                    if (!randomize && this.TextureButton(this.textures["off"], new Rect(590f, 757f, 740f, 80f)))
                    {
                        randomize = true;
                        hardMode = false;
                    }
                    else if (randomize && !hardMode && this.TextureButton(this.textures["easy"], new Rect(590f, 757f, 740f, 80f)))
                    {
                        randomize = true;
                        hardMode = true;
                    }
                    else if (randomize && hardMode && this.TextureButton(this.textures["hard"], new Rect(590f, 757f, 740f, 80f)))
                    {
                        randomize = false;
                        hardMode = false;
                    }

                    //Create text field for seed if randomizer is not off
                    if (randomize)
                    {
                        this.seedString = GUI.TextField(new Rect(200f, 757f, 330f, 82f), this.seedString, 9, this.style);

                        //Running Regex every frame is not efficient, but I figure the potential performance hit doesn't matter too much in the main menu
                        this.seedString = Regex.Replace(this.seedString, "[^0-9]", "");
                        int.TryParse(this.seedString, out seed);

                        if (GUI.Button(new Rect(100f, 365f, 250f, 110f), "Log Randomization"))
                        {
                            bool permadeath = false;
                            Randomizer.Randomize(seed, hardMode, permadeath);
                        }
                        if (GUI.Button(new Rect(100f, 593f, 250f, 110f), "Log Randomization"))
                        {
                            bool permadeath = true;
                            Randomizer.Randomize(seed, hardMode, permadeath);
                        }
                        if (GUI.Button(new Rect(100f, 757f, 75f, 82f), "New")) this.seedString = rnd.Next(1000000000).ToString();
                    }
                }
                else
                {
                    this.seedString = rnd.Next().ToString();
                }

                //Disable randomizer while looking at saves so that the variables don't carry over to places they shouldn't
                if (UIManager.instance.menuState == GlobalEnums.MainMenuState.SAVE_PROFILES && !GUIController.loadedSave)
                {
                    randomize = false;
                    hardMode = false;
                }
            }

            //Restore GUI settings
            GUI.depth = depth;
            GUI.matrix = matrix;
            GUI.backgroundColor = backgroundColor;
            GUI.contentColor = contentColor;
            GUI.color = color;
        }

        //I know, this is awful, But I couldn't figure out how to get a texture to stretch over a button
        private bool TextureButton(Texture2D tex, Rect rect)
        {
            GUI.DrawTexture(rect, tex, ScaleMode.ScaleToFit);
            return GUI.Button(rect, "", GUIStyle.none);
        }
    }
}
