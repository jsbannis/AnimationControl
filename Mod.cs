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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using ScrollsModLoader.Interfaces;
using UnityEngine;
using Mono.Cecil;

namespace AnimationControl.mod
{
	public class Mod : BaseMod
	{
        private SettingsMenuWrapper _settingsMenu = null;
        private Config _configManager = null;

        private float _multiplier = 1.0f;
        private Hashtable _oldSpeed = new Hashtable();
        private string _lobbyMenu = "";

		//initialize everything here, Game is loaded at this point
		public Mod ()
		{
            _configManager = new Config(this.OwnFolder());
            _multiplier = _configManager.GetMultiplier();
		}

		public static string GetName ()
		{
			return "AnimationControl";
		}

		public static int GetVersion ()
		{
			return 1;
		}

        public void SetMultiplier(float multiplier)
        {
            if (!Mathf.Approximately(_multiplier, multiplier))
            {
                _multiplier = multiplier;
            }
            _configManager.SetMultiplier(multiplier);
        }

		//only return MethodDefinitions you obtained through the scrollsTypes object
		//safety first! surround with try/catch and return an empty array in case it fails
		public static MethodDefinition[] GetHooks (TypeDefinitionCollection scrollsTypes, int version)
		{
			return new MethodDefinition[]
            {
                // unit speedup logic
                scrollsTypes["iTween"].Methods.GetMethod("Launch")[0],
                scrollsTypes["AnimPlayer"].Methods.GetMethod("UpdateOnly")[0],
                // configuration
                scrollsTypes["SettingsMenu"].Methods.GetMethod("OnGUI")[0],
                scrollsTypes["SceneLoader"].Methods.GetMethod("loadScene")[0],
            };
		}

		public override void BeforeInvoke (InvocationInfo info)
		{
            // Handle settings Menu
            if (info.targetMethod.Equals("loadScene"))
            {
                _lobbyMenu = ((String)info.arguments[0]);
                return;
            }
            
            Boolean tweenLaunch = info.targetMethod.Equals("Launch");
            Boolean animUpdate = info.target != null && info.target.GetType() == typeof(AnimPlayer) && info.targetMethod.Equals("UpdateOnly");

            if (!(tweenLaunch || animUpdate))
            {
                return;
            }

            // Make sure that this is a unit animation / tween
            if (info.stackTrace == null || info.stackTrace.GetFrames() == null)
            {
                return;
            }
            Boolean unitAnimation = false;
            StackFrame[] frames = info.stackTrace.GetFrames();
            foreach (StackFrame frame in frames)
            {
                if (frame.GetMethod().DeclaringType == typeof(Unit))
                {
                    unitAnimation = true;
                }
            }
            if (!unitAnimation)
            {
                return;
            }

            // Handle tweening
            if (tweenLaunch && info.parameterTypes.Length > 1 && info.parameterTypes[1] == typeof(Hashtable))
            {
                Hashtable args = (Hashtable)info.arguments[1];
                if (args.ContainsKey("time"))
                {
                    args["time"] = ((float)args["time"]) / _multiplier;
                }
            }

            // Handle frame animation
            if (animUpdate)
            {
                // (only if comes from attack)
                if (((AnimPlayer)info.target).lastAnim.ToLower().Contains("attack") || 
                    ((AnimPlayer)info.target).lastAnim.ToLower().Contains("gethit") ||
                    ((AnimPlayer)info.target).lastAnim.ToLower().Contains("charge"))
                {
                    float speed = (float)typeof(AnimPlayer).GetField("_speed", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(info.target);
                    ((AnimPlayer)info.target).setSpeed(speed * _multiplier);
                    _oldSpeed.Add(info.target, speed);
                }
            }
		}

		public override void AfterInvoke (InvocationInfo info, ref object returnValue)
		{
            // Handle settings Menu
            if (info.target != null && info.target.GetType() == typeof(SettingsMenu) && info.targetMethod.Equals("OnGUI"))
            {
                if (!"_Settings".Equals(_lobbyMenu))
                    return;
                if (_settingsMenu == null)
                {
                    _settingsMenu = new SettingsMenuWrapper(this, _multiplier);
                }
                _settingsMenu.OnGUI(true, (SettingsMenu)info.target);
                return;
            }

            // Restore speed in frame animation
            Boolean animUpdate = info.target != null && info.target.GetType() == typeof(AnimPlayer) && info.targetMethod.Equals("UpdateOnly");
            if (!animUpdate)
            {
                return;
            }
            if (_oldSpeed.ContainsKey(info.target))
            {
                ((AnimPlayer)info.target).setSpeed((float)_oldSpeed[info.target]);
                _oldSpeed.Remove(info.target);
            }
		}
	}
}

