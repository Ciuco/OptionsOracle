/*
 * OptionsOracle DataCenter
 * Copyright 2006-2012 SamoaSky
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 */

using System;
using System.Collections.Generic;
using System.Text;
using OptionsOracle.DataCenter.Data;
using OptionsOracle.DataCenter.Remote;

namespace OptionsOracle.DataCenter
{
    class Config
    {
        private static ConfigSet    local  = null;
        private static RemoteConfig remote = null;
        private static RemoteModule parser = null;

        public static ConfigSet Local
        {
            get 
            { 
                if (local == null) 
                { 
                    local = new ConfigSet(); 
                    local.Load();   // load local config (local file)
                    local.Update(); // load from remote config module
                } 
                
                return local; 
            }
        }

        public static RemoteConfig Remote
        {
            get
            {
                if (remote == null)
                {
                    remote = new RemoteConfig();
                    remote.Load();
                }

                return remote;
            }
        }

        public static RemoteModule Parser
        {
            get
            {
                if (parser == null)
                {
                    parser = new RemoteModule("parser");
                }

                return parser;
            }
        }
    }
}
