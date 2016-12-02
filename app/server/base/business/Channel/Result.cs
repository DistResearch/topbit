#region Copyright

//==============================================================================
//  File Name   :   Result.cs
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
//   <record date="2011-06-15 14:58:42" author="Zhang Ling" revision="1.00.000">
//		First version of Result.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Channel
{
    using System;
    using System.IO;
    using System.Net;
    using log4net;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    ///  Summary of Result.
    /// </summary>
    public class Result
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Result));

        public Result()
        {
            HttpStatusCode = 200;
        }

        [JsonProperty("result")]
        public object Data { get; set; }

        [JsonProperty("error")]
        public object Error { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonIgnore]
        public string DataText { get; set; }

        [JsonIgnore]
        public int HttpStatusCode { get; set; }

        [JsonIgnore]
        public HttpWebResponse Response { get; private set; }

        [JsonIgnore]
        public string ContentType
        {
            get { return Response != null ? Response.ContentType : "application/json-rpc"; }
        }

        [JsonIgnore]
        public string XLongPulling { get; set; }

        [JsonIgnore]
        public string XTime { get; set; }

        public static Result Parse(HttpWebResponse response)
        {
            var resultText = "";
            var incomingResponse = response;
            try
            {
                var stream = response.GetResponseStream();
                if (stream != null)
                {
                    using (var sw = new StreamReader(stream))
                    {
                        resultText = sw.ReadToEnd();
                        sw.Close();
                    }
                }
            }
            catch (WebException exc)
            {
                incomingResponse = (HttpWebResponse)exc.Response;
                if (incomingResponse != null)
                {
                    var stream = incomingResponse.GetResponseStream();
                    if (stream != null)
                    {
                        using (var sw = new StreamReader(stream))
                        {
                            resultText = sw.ReadToEnd();
                            sw.Close();
                        }
                    }
                }
            }

            Result result;
            try
            {
                result = JsonConvert.DeserializeObject<Result>(resultText);
            }
            catch(Exception e)
            {
                logger.Error(string.Format("Error when DeserializeJson from server {0}", resultText), e);
                result = new Result();
                result.Data = false;
                result.DataText = resultText;
                result.Error = e.Message;
                result.Id = "0";
            }
            result.Response = incomingResponse;

            return result;    
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public Result<float> ToFloat()
        {
            return new Result<float>() { Value = float.Parse(DataText), Response = Response };
        }

        public Result<int> ToInteger()
        {
            return new Result<int>() { Value = int.Parse(DataText), Response = Response };
        }

        public Result<bool> ToBoolean()
        {
            return new Result<bool>() { Value = bool.Parse(DataText), Response = Response };
        }
    }

    public class Result<T> : Result
    {
        [JsonProperty("result")]
        public T Value { get; set; }
    }
}