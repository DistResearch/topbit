#region Copyright

//==============================================================================
//  File Name   :   BitCoinRapidService.cs
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
//   <record date="2011-06-16 02:30:21" author="Zhang Ling" revision="1.00.000">
//		First version of BitCoinRapidService.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Service
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Web;
    using App.Web.Business.Cache;
    using App.Web.Business.Channel;
    using log4net;
    using System.Threading;
    using System.Diagnostics;

    /// <summary>
    ///  Deepbot IIS Module with Long-pulling request enabled.
    /// </summary>
    /// <remark>
    /// You will need to configure this module in the web.config file of your
    /// web and register it with IIS before being able to use it. For more information
    /// see the following link: http://go.microsoft.com/?linkid=8101007
    /// </remark>
    public class BitCoinRapidService : IHttpModule
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(BitCoinRapidService));
        
        static readonly Result UnknownMethod = new Result() { HttpStatusCode = 400, DataText = "Unknown Method" };
        static readonly Result NeedAuthorizationMethod = new Result() { HttpStatusCode = 401, DataText = "Bot Not Found" };
        static readonly Result InternalError = new Result() { HttpStatusCode = 500, DataText = "Internal Error" };
        
        static readonly byte[] EmptyByte = new byte[0];
        static readonly Result AcceptResult = new Result() { DataText = "{result:'true'}" };
        static readonly Result RejectResult = new Result() { DataText = "{result:'false'}" };

        static DevelopMentor.ThreadPool threadPool;
        static int activeRequests = 0;

        static BitCoinRapidService()
        {
            threadPool = new DevelopMentor.ThreadPool(2, 10, "Bitcoin Push Pool");
            threadPool.RequestQueueLimit = 10000;
            threadPool.PropogateCallContext = true;
            threadPool.PropogateThreadPrincipal = true;
            threadPool.PropogateHttpContext = true;
            threadPool.Start();
        }

        #region Implementation of IHttpModule

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(OnBeginRequest);

            context.AddOnPreRequestHandlerExecuteAsync(new BeginEventHandler(BeginProcessRequest),
                                                       new EndEventHandler(EndProcessRequest)); 
        }

        public void Dispose()
        {
            //do nothing at present
        }

        #endregion

        #region OnBeginRequest

        public void OnBeginRequest(object source, EventArgs e)
        {
            // do simple check on the request before the request go down to IHttpAsyncHandler
            var app = (HttpApplication)source;
            var incomingRequest = app.Request;
#if DEBUG
            logger.InfoFormat("OnBeginRequest - {0} {1} ({2})",
                incomingRequest.HttpMethod,
                incomingRequest.FilePath,
                incomingRequest.InputStream.Length);
#endif
            // Rule: redirect to homepage if the request is not we need
            if (incomingRequest.HttpMethod != "POST" || incomingRequest.FilePath != "/")
            {
                app.Response.Redirect("http://topb.it", true);
                app.CompleteRequest();
                return;
            }

            // Rule: end request if no data received
            if (incomingRequest.InputStream.Length < 1)
            {
                app.CompleteRequest();
                return;
            }
            
            // Rule: 401 error when Username or Password has not been specified
            var authorizationHeader = app.Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(authorizationHeader) || authorizationHeader.Length <= 6)
            {
                app.Response.AddHeader("WWW-Authenticate",
                                       "BASIC Realm=TopBit API (http://api.topb.it)");
                app.Response.StatusCode = 401;
                app.CompleteRequest();
                return;
            }

            // pass all check
            // let IHttpAsyncHandler to handle the request
        }

        #endregion

        #region IHttpAsyncHandler Members

        /// <remarks>
        /// 以下设计将保证 bitcoin 网关处理尽可能多的请求
        /// getwork:
        ///     远程调用，使用异步处理
        /// submitwork:
        ///     当本地验证未通过则直接返回
        ///     当本地验证通过时使用远程调用，由于不需要考虑结果，仍然可以立即返回
        /// </remarks>
        IAsyncResult BeginProcessRequest(object sender, EventArgs e, AsyncCallback cb, object extraData)
        {

#if DEBUG
            int workerAvailable = threadPool.AvailableThreads;
            int completionPortAvailable = threadPool.MaxThreads;
            int activeRequests = completionPortAvailable - workerAvailable;

            Debug.WriteLine(string.Format("BeginProcessRequest: {0} {1} out of {2}/{3} ({4} Requests Active)", Thread.CurrentThread.IsThreadPoolThread, Thread.CurrentThread.ManagedThreadId, workerAvailable, completionPortAvailable, activeRequests));

            //attach the debugger if query string contains 'break=yes' 
            BreakIfRequested();
#endif
            HttpContext context = ((HttpApplication)sender).Context;

            //  prepare async context
            var result = new BitCoinResult(cb, context, extraData, this, "getwork");

            try
            {
                var message = Message.Parse(context.Request);
                if (message != null && message.Method == "getwork")
                {
                    // api.topb.it only deals 'getwork' request
                    result.Process(threadPool, message);
                }
                else
                {
                    result.SetResult(UnknownMethod);
                    result.Complete(null, true);
                }    
            }
            catch (Exception error)
            {
                logger.Fatal("BeginProcessRequest - " + error.Message, error);

#if DEBUG
                Debug.WriteLine(string.Format("Exception in BeginProcessRequest: {0}", error.Message));
#endif
                result.SetResult(InternalError);
                result.Complete(null, true);
            }

            //  ok, return it
            return result;
        }

        void EndProcessRequest(IAsyncResult result)
        {
#if DEBUG
            int workerAvailable = threadPool.AvailableThreads;
            int completionPortAvailable = threadPool.MaxThreads;
            int activeRequests = completionPortAvailable - workerAvailable;

            Debug.WriteLine(string.Format("EndProcessRequest: Pooled:{0} Id:{1} out of {2}/{3} ({4} Requests Active, {5} Requests Pending)", Thread.CurrentThread.IsThreadPoolThread, Thread.CurrentThread.ManagedThreadId, workerAvailable, completionPortAvailable, activeRequests, threadPool.RequestQueueLength));
#endif

            BitCoinResult.End(result, this, "getwork");
        }

        #endregion

        static void BreakIfRequested()
        {
            string breakArg = HttpContext.Current.Request.QueryString["break"];

            if ((breakArg != null) && ((breakArg == "yes") || (breakArg == "true")))
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                else
                {
                    Debugger.Launch();
                }
            }
        }
    }
}