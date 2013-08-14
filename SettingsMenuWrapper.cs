/*
 * This software is licensed under the terms of the BSD 2-Clause License.
 * 
 * Copyright (c) 2013, jsbannis
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met: 
 * 
 * 1. Redistributions of source code must retain the above copyright notice, this
 * list of conditions and the following disclaimer. 
 * 
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Reflection;
using UnityEngine;

namespace AnimationControl
{
    public class SettingsMenuWrapper
    {
        private GUISkin _settingsSkin;
        private GUISkin _regularUI;
        private mod.Mod _mod;
        private float _multiplier = 1.0f;

        public SettingsMenuWrapper(mod.Mod mod, float initialMultiplier)
        {
            _settingsSkin = (GUISkin)Resources.Load("_GUISkins/Settings");
            _regularUI = (GUISkin)Resources.Load("_GUISkins/RegularUI");
            _mod = mod;
            _multiplier = initialMultiplier;
        }

        public void OnGUI(bool drawHighlight, SettingsMenu menu)
        {
            GUI.depth = 22;
            GUI.skin = _settingsSkin;

            // Background frame.
            Rect frameRect = new Rect(
                (float)Screen.width * 0.5f - (float)Screen.height * 0.52f, 
                (float)Screen.height * 0.2f + (float)Screen.height * 0.42f, 
                (float)Screen.height * 0.5f, 
                (float)Screen.height * 0.2f);
            new ScrollsFrame(frameRect).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.CENTER).Draw();

            // Header
            GUI.skin.label.fontSize = Screen.height / 32;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.skin = _regularUI;
            int fontSize = GUI.skin.label.fontSize;
            GUI.skin.label.fontSize = Screen.height / 32;
            float width = frameRect.width * 0.8f;
            float height = frameRect.height * 0.2f;
            Rect headerRect = new Rect(
                frameRect.x + ((frameRect.width - width) / 2.0f),
                frameRect.y + height / 2.0f,
                width,
                height);
            GUI.Label(headerRect, "Animation Control");

            // Slider
            GUI.skin = _settingsSkin;
            GUI.skin.button.fontSize = (Screen.height / 32);
            GUI.skin.horizontalSlider.fixedHeight = (float)(Screen.height / 30);
            GUI.skin.horizontalSliderThumb.fixedHeight = (float)(Screen.height / 30);

            Rect slideRect = new Rect(
                frameRect.x + ((frameRect.width - width) / 2.0f),
                frameRect.y + height * 2.0f,
                width,
                height);
            GUI.Label(slideRect, "Unit Animation Speed Multiplier: " + _multiplier.ToString("N0") + "x");
            slideRect.y = slideRect.y + (float)Screen.height * 0.05f;
            _multiplier = (float)Math.Round(GUI.HorizontalSlider(slideRect, _multiplier, 1.0f, 25.0f));

            // Draw fade-in
            if (menu != null)
            {
                float fadeIn = (float)typeof(SettingsMenu).GetField("fadeIn", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(menu);
                GUI.color = new Color(1f, 1f, 1f, fadeIn);
                GUI.DrawTexture(frameRect, ResourceManager.LoadTexture("Shared/blackFiller"));
                GUI.color = new Color(1f, 1f, 1f, 1f);
            }

            // Update
            _mod.SetMultiplier(_multiplier);
        }
    }
}

