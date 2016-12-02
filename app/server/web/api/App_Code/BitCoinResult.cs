
namespace App.Web.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Threading;
    using System.Text;
    using App.Web.Business.Channel;
    using App.Web.Business.Utils;
    using App.Web.Business.Cache;
    using System.Diagnostics;
    using System.Net;
    using System.IO;
    using Newtonsoft.Json.Linq;
    using log4net;

    public class BitCoinResult : AsyncResultNoResult
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(BitCoinResult));

        // Field set when operation completes
        private Result m_result = null;
        private HttpContext m_context = null;

        public void SetResult(Result result)
        {
            m_result = result;
        }

        new public static void End(IAsyncResult result, object owner, string operationId)
        {
            BitCoinResult asyncResult = result as BitCoinResult;
            if (asyncResult == null)
            {
                throw new ArgumentException(
                    "Result passed represents an operation not supported " +
                    "by this framework.",
                    "result");
            }

            // Wait until operation has completed 
            AsyncResultNoResult.End(result, owner, operationId);

            // Write the result to client (if above didn't throw)
            if (asyncResult.m_result != null && asyncResult.m_context.Response.IsClientConnected)
            {
                try
                {
                    var outgoingResponse = asyncResult.m_context.Response;

                    // write the response to stream
                    outgoingResponse.ContentEncoding = Encoding.UTF8;
                    outgoingResponse.ContentType = asyncResult.m_result.ContentType;
                    outgoingResponse.StatusCode = asyncResult.m_result.HttpStatusCode;

                    if (asyncResult.m_result.HttpStatusCode == 401)
                    {
                        outgoingResponse.AddHeader("WWW-Authenticate",
                                                   "BASIC Realm=TopBit API (http://api.topb.it)");
                    }

                    if (outgoingResponse.StatusCode == 200)
                    {
                        outgoingResponse.Write(asyncResult.m_result.ToJson());
                    }

                    outgoingResponse.End();
                }
                catch (Exception)
                {
                    // socket connection from mining client is interrupted.
                }
            }
        }

        public BitCoinResult(AsyncCallback callback, HttpContext context, object state, object owner, string operationId)
            : base(callback, state, owner, operationId)
        {
            if (context == null) throw new ArgumentNullException("HttpContext");

            m_context = context; 
        }

        public void Process(DevelopMentor.ThreadPool threadPool, Message message)
        {
            if (message != null && message.Arguments != null && message.Arguments.Count > 0)
            {
                string username, password;
                ExtractAuthToken(m_context, out username, out password);

                // move to async handler
                var actionResult = WorkManager.TestWork(message, username, password);

                SetResult(actionResult);
                Complete(null, true);

                //async calll to remote
                threadPool.PostRequest(new DevelopMentor.WorkRequestDelegate(ProcessSubmitWorkRequest), message);
            }
            else
            {
                //async calll to remote
                threadPool.PostRequest(new DevelopMentor.WorkRequestDelegate(ProcessGetWorkRequest), message);
            }
        }


        void ProcessSubmitWorkRequest(object state, DateTime requestTime)
        {
            try
            {
                Message message = state as Message;

                string username, password;
                ExtractAuthToken(m_context, out username, out password);

                // move to async handler
                WorkManager.SubmitWork(message, username, password);
            }
            catch (Exception e)
            {
            }
        }

        void ProcessGetWorkRequest(object state, DateTime requestTime)
        {
            try
            {
                Message message = state as Message;

                string username, password;
                ExtractAuthToken(m_context, out username, out password);

                var result = WorkManager.GetWork(username, password, new AsyncCallback(ProcessGetWorkRequest));
            }
            catch (Exception)
            {
            }
        }

        void ProcessGetWorkRequest(IAsyncResult asyncResult)
        {
            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
            HttpWebResponse incomingResponse;
            try
            {
                incomingResponse = (HttpWebResponse)request.EndGetResponse(asyncResult);
            }
            catch (WebException exc)
            {
                incomingResponse = (HttpWebResponse)exc.Response;
            }
            Result result = null;
            try
            {
                result = Result.Parse(incomingResponse);

                SetResult(result);
                Complete(null, false);
            }
            catch (Exception exception)
            {
                result = new Result() { HttpStatusCode = 500, DataText = exception.Message, Data = false };
                logger.Error("Error when parsing result {0}", exception);

                SetResult(result);
                Complete(null, false);
            }
            finally
            {
                if (result != null)
                {
                    //log get work
                    if (result.Data is JObject)
                    {
                        var json = result.Data as JObject;
                        var jsonstring = (json["data"]).ToString();

                        //todo: 记录发放的工作，用于鉴别是否客户端提交了未发放的工作
                        //Server.Recent.LogBotWork(bot, jsonstring);
                    }
#if DEBUG
                    logger.InfoFormat("get_work: {0}", result.ToJson());
#endif
                }
            }
        }

        void ExtractAuthToken(HttpContext context, out string username, out string password)
        {
            // The header is in the following format
            // "Basic 64BitEncodedUsernameAndPasswordString"
            // userAndPasswordDecoded is in the following
            // format "theusername:thepassword"
            var userAndPassDecoded = new ASCIIEncoding().GetString(
                Convert.FromBase64String(context.Request.Headers["Authorization"].Substring(6)));

            var userAndPasswordArray = userAndPassDecoded.Contains(":") ? userAndPassDecoded.Split(':') : new[] { userAndPassDecoded, "" };

            username = userAndPasswordArray[0];
            password = userAndPasswordArray[1];
        }
    }
}