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
using System.IO;
using System.Collections.Generic;
using JsonFx.Json;

namespace AnimationControl
{
    public class Config
    {
        private string _configPath;
        private Dictionary<String, object> _config;

        public Config(string modFolder)
        {
            _configPath = modFolder + Path.DirectorySeparatorChar + "config.json";
            LoadConfig();
        }

        private void LoadConfig()
        {
            if (File.Exists(_configPath))
            {
                String data = File.ReadAllText(_configPath);
                _config = new JsonReader().Read<Dictionary<String, object>>(data);
            }
            else
            {
                _config = new Dictionary<String, object>();
            }
        }

        public void WriteConfig()
        {
            String data = new JsonWriter().Write(_config);
            File.WriteAllText(_configPath, data);
        }

        public void SetMultiplier(float multiplier)
        {
            _config["multiplier"] = multiplier.ToString();
            WriteConfig();
        }

        public float GetMultiplier()
        {
            return (_config.ContainsKey("multiplier") ? float.Parse((String)_config["multiplier"]) : 1.0f);
        }
    }
}

