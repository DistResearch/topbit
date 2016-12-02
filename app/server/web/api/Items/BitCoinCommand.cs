#region Copyright

//==============================================================================
//  File Name   :   BitCoinCommand.cs
//
//  Copyright (C) 2011 E2 Technologies. All rights reserved.
//
//  Distributable under e2 technologies code license.
//  See terms of license at www.e2.org.cn
//
//==============================================================================

//==============================================================================
// <fileinformation>
//   <summary>
//      This file is a part of api project.
//   </summary>
//   <author name="Zhang Ling" mail="tox@e2.org.cn"/>
//   <seealso ref=""/>
// </fileinformation>
//
// <history>
//   <record date="2011-06-15 00:30:45" author="Zhang Ling" revision="1.00.000">
//		First version of BitCoinCommand.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.API.Items
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using Newtonsoft.Json;

    /// <summary>
    ///  Summary of BitCoinCommand.
    /// </summary>
    public class BitCoinCommand
    {
        public static BitCoinCommand Create(string method, params object[] arguments)
        {
            var action = new BitCoinCommand();
            action.Id = "1";
            action.Method = method;
            action.Version = "1.0";
            if (arguments != null && arguments.Length > 0)
            {
                action.Arguments = new List<string>();
                foreach (var argument in arguments)
                {
                    action.Arguments.Add(argument.ToString());
                }
            }
            else
            {
                action.Arguments = EmptyList;
            }
            return action;
        }


        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("jsonrpc")]
        public string Version { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("params")]
        public List<string> Arguments { get; set; }

        [JsonIgnore]
        public HttpWebRequest Request { get; set; }

        [JsonIgnore]
        public string ContentType
        {
            get { return Request != null ? Request.ContentType : "application/json-rpc"; }
        }

        [JsonIgnore]
        public string UserAgent
        {
            get { return Request != null ? Request.UserAgent : "Deepbot 1.0"; }
        }

        public static BitCoinCommand Parse(string json)
        {
            return JsonConvert.DeserializeObject<BitCoinCommand>(json);
        }

        public static BitCoinCommand Parse(Stream stream)
        {
            string actionScript;
            using (var sr = new StreamReader(stream))
            {
                actionScript = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<BitCoinCommand>(actionScript);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }


        private static List<string> EmptyList = new List<string>(0);
    }
}