#region Copyright

//==============================================================================
//  File Name   :   BitServer.cs
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
//   <record date="2011-06-15 10:47:05" author="Zhang Ling" revision="1.00.000">
//		First version of BitServer.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Channel
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;
    using System.Text;
    using log4net;

    /// <summary>
    /// 比特币API接口
    /// </summary>
    public interface IBitCoinOperations
    {
        Result GetWork();
        Result SubmitWork(string solution);
        Result<float> GetDifficulty();
        Result<float> GetHashesPerSec();
        Result<float> GetBalance(string account = null, int minConfirmation = 1);
        Result ValidateAddress(string address);
    }

    /// <summary>
    /// 比特币API接口的实现1，远程调用接口
    /// </summary>
    public class BitServer : IBitCoinOperations
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(BitServer));

        private BitServer() { }

        public static BitServer Create(string url)
        {
            var settings = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("DeepBot/Settings");

            var user = settings["rpcuser"];
            var password = settings["rpcpassword"];

            return new BitServer() { ServerAddress = new Uri(url), Credentials = new NetworkCredential(user, password) };
        }

        public static BitServer Create(string url, string user, string password)
        {
            return new BitServer() { ServerAddress = new Uri(url), Credentials = new NetworkCredential(user, password) };
        }

        public Result GetWork()
        {
            return InvokeMethod(GetWorkOperation);
        }

        public Result GetWork(AsyncCallback callback)
        {
            return InvokeMethod(GetWorkOperation, callback);
        }

        public Result SubmitWork(string solution)
        {
            return InvokeMethod(Message.Create("getwork", solution));
        }

        public Result SubmitWork(Message work)
        {
            return InvokeMethod(work);
        }

        public Result<float> GetDifficulty()
        {
            return InvokeMethod(GetDifficultyOperation).ToFloat();
        }

        public Result<float> GetHashesPerSec()
        {
            return InvokeMethod(GetHashesPerSecOperation).ToFloat();
        }

        public Result<float> GetBalance(string account = null, int minConfirmation = 1)
        {
            if (account == null)
            {
                return InvokeMethod(GetBalanceOperation).ToFloat();
            }
            return InvokeMethod(Message.Create("getbalance", account, minConfirmation)).ToFloat();
        }

        public Result ValidateAddress(string address)
        {
            return InvokeMethod(Message.Create("validateaddress", address));
        }

        #region Request
        private Result InvokeMethod(Message message, AsyncCallback callback)
        {
            // serialize json for the request
            var requestJson = message.ToJson();
            var byteArray = Encoding.UTF8.GetBytes(requestJson);

            var outgoingRequest = (HttpWebRequest)WebRequest.Create(ServerAddress);
            outgoingRequest.Timeout = 30000;
            outgoingRequest.Credentials = Credentials;
            outgoingRequest.PreAuthenticate = true;
            outgoingRequest.UnsafeAuthenticatedConnectionSharing = true;
            outgoingRequest.Method = "POST";
            outgoingRequest.ContentType = message.ContentType;
            outgoingRequest.UserAgent = message.UserAgent;
            outgoingRequest.ContentLength = byteArray.Length;

            //write to request stream));
            using (var dataStream = outgoingRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            HttpWebResponse incomingResponse;
            try
            {
                var result = outgoingRequest.BeginGetResponse(callback, outgoingRequest);
                return new Result() { HttpStatusCode = 100, DataText = "Is async-call completed synchronously? " + result.CompletedSynchronously, Data = false };                
            }
            catch (WebException exc)
            {
                incomingResponse = (HttpWebResponse)exc.Response;
            }

            try
            {
                return Result.Parse(incomingResponse);
            }
            catch (Exception exception)
            {
                logger.Error("Error when parsing result {0}", exception);
                return new Result() { HttpStatusCode = 500, DataText = exception.Message, Data = false };
            }
        }

        private Result InvokeMethod(Message message)
        {
            // serialize json for the request
            var requestJson = message.ToJson();
            var byteArray = Encoding.UTF8.GetBytes(requestJson);

            var outgoingRequest = (HttpWebRequest)WebRequest.Create(ServerAddress);
            outgoingRequest.Timeout = 30000;
            outgoingRequest.Credentials = Credentials;
            outgoingRequest.PreAuthenticate = true;
            outgoingRequest.UnsafeAuthenticatedConnectionSharing = true;
            outgoingRequest.Method = "POST";
            outgoingRequest.ContentType = message.ContentType;
            outgoingRequest.UserAgent = message.UserAgent;
            outgoingRequest.ContentLength = byteArray.Length;

            //write to request stream));
            using (var dataStream = outgoingRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            HttpWebResponse incomingResponse;
            try
            {
                incomingResponse = (HttpWebResponse)outgoingRequest.GetResponse();
            }
            catch (WebException exc)
            {
                incomingResponse = (HttpWebResponse)exc.Response;
            }

            try
            {
                return Result.Parse(incomingResponse);
            }
            catch (Exception exception)
            {
                logger.Error("Error when parsing result {0}", exception);
                return new Result() { HttpStatusCode = 500, DataText = exception.Message, Data = false };
            }
        }
        #endregion

        private Uri ServerAddress { get; set; }
        private ICredentials Credentials { get; set; }

        private static readonly Message GetWorkOperation = new Message() { Method = "getwork", Version = "1.0" };
        private static readonly Message GetDifficultyOperation = new Message() { Method = "getdifficulty", Version = "1.0" };
        private static readonly Message GetHashesPerSecOperation = new Message() { Method = "gethashespersec", Version = "1.0" };
        private static readonly Message GetBalanceOperation = new Message() { Method = "getbalance", Version = "1.0" };
    }
}