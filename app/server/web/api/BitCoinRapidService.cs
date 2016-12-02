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
    using System.Collections.Specialized;
    using System.Net;
    using System.Text;
    using System.Web;
    using App.Web.Business;
    using App.Web.Business.Cache;
    using App.Web.Business.Channel;
    using log4net;

    /// <summary>
    ///  Deepbot IIS Module
    /// </summary>
    /// <remark>
    /// You will need to configure this module in the web.config file of your
    /// web and register it with IIS before being able to use it. For more information
    /// see the following link: http://go.microsoft.com/?linkid=8101007
    /// </remark>
    public class BitCoinRapidService : IHttpModule
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof (BitCoinRapidService));

        #region Implementation of IHttpModule

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(OnBeginRequest);
        }

        public void Dispose()
        {
            //do nothing at present
        }

        #endregion

        #region OnBeginRequest
        public void OnBeginRequest(object source, EventArgs e)
        {
            var app = (HttpApplication)source;
            var incomingRequest = app.Request;

            //copy incoming request body to outgoing request
            if (incomingRequest.HttpMethod != "POST" || incomingRequest.FilePath != "/")
                return;

            var requestStream = incomingRequest.InputStream;
            var length = requestStream.Length;

            if (length > 0)
            {
                var login = HttpContext.Current.Session["login"] as string;
                var password = HttpContext.Current.Session["password"] as string;
                var actionResult = BitChannel.Process(app, requestStream, new NetworkCredential(login, password));

                try
                {
                    var outgoingResponse = app.Response;
                    outgoingResponse.ContentEncoding = Encoding.UTF8;
                    outgoingResponse.ContentType = actionResult.ContentType;
                    outgoingResponse.Write(actionResult.ToJson());
                }
                catch (Exception err)
                {
                    logger.Fatal("[" + DateTime.Now.ToShortTimeString() + "] [Exception] " + err.Message, err);
                }
            }

            // complete
            app.CompleteRequest();
        }
        #endregion
    }
}