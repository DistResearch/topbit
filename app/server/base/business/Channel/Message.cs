#region Copyright

//==============================================================================
//  File Name   :   Message.cs
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
//      This file is a part of business project.
//   </summary>
//   <author name="Zhang Ling" mail="tox@e2.org.cn"/>
//   <seealso ref=""/>
// </fileinformation>
//
// <history>
//   <record date="2011-06-15 14:46:57" author="Zhang Ling" revision="1.00.000">
//		First version of Message.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Channel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Web;
    using log4net;
    using Newtonsoft.Json;

    /// <summary>
    ///  Summary of Message.
    /// </summary>
    public class Message
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof (Message));

        public static Message Create(string method, params object[] arguments)
        {
            var action = new Message();
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
        public HttpRequest Request { get; set; }

        [JsonIgnore]
        public string ContentType
        {
            get { return Request != null ? Request.ContentType : "application/json-rpc"; }
        }

        [JsonIgnore]
        public string UserAgent
        {
            get { return Request != null ? Request.UserAgent : "E2bot Beta"; }
        }

        public static Message Parse(string json)
        {
            return JsonConvert.DeserializeObject<Message>(json);
        }

        public static Message Parse(HttpRequest request)
        {
            string messageText;
            using (var sr = new StreamReader(request.InputStream))
            {
                messageText = sr.ReadToEnd();
            }

            try
            {
                var message = JsonConvert.DeserializeObject<Message>(messageText);
                message.Request = request;
                return message;
            }
            catch (Exception e)
            {
                logger.Error(string.Format("Error when DeserializeJson from client {0}", messageText), e);
                return null;
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }


        private static List<string> EmptyList = new List<string>(0);
    }
}