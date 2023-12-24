using System;
using UnityEngine;

namespace NaughtyAttributes
{
    public enum EButtonEnableMode
    {
        /// <summary>
        /// Button should be active always
        /// </summary>
        Always,
        /// <summary>
        /// Button should be active only in editor
        /// </summary>
        Editor,
        /// <summary>
        /// Button should be active only in playmode
        /// </summary>
        Playmode
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple=true)]
    public class ColorKeyAttribute : Attribute
    {
        public string Key { get; private set; }
        public Color Color { get; private set; }

        public ColorKeyAttribute(float r, float g, float b, string key)
        {
            this.Color = new Color(r, g, b);
            this.Key = key;
        }
        
        public ColorKeyAttribute(byte r, byte g, byte b, string key)
        {
            this.Color = new Color32(r, g, b, 255);
            this.Key = key;
        }
        
    }
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ButtonAttribute : SpecialCaseDrawerAttribute
    {
        public string Text { get; private set; }
        public EButtonEnableMode SelectedEnableMode { get; private set; }
        public FontStyle FontStyle { get; private set; }

        public ButtonAttribute(string text = null, EButtonEnableMode enabledMode = EButtonEnableMode.Always, FontStyle fontStyle = FontStyle.Normal)
        {
            this.Text = text;
            this.SelectedEnableMode = enabledMode;
            this.FontStyle = fontStyle;
        }
    }
}
