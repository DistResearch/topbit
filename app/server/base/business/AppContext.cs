#region Copyright

//==============================================================================
//  File Name   :   AppContext.cs
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
//   <record date="2011-06-19 15:54:35" author="Zhang Ling" revision="1.00.000">
//		First version of AppContext.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business
{
    using System;
    using System.Collections;
    using System.Configuration;
    using App.Web.Business.Data;
    using App.Web.Business.Utils;
    using Castle.ActiveRecord;
    using Castle.ActiveRecord.Framework;
    using Castle.ActiveRecord.Framework.Config;
    using Castle.Facilities.FactorySupport;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    /// <summary>
    ///  Summary of AppContext.
    /// </summary>
    public class AppContext
    {
        #region Constructors
        static AppContext()
        {
            Core = new WindsorContainer();
            Core.AddFacility<FactorySupportFacility>();
        }
        #endregion

        #region App.Core / App.Properties / App.Log

        public static IWindsorContainer Core { get; private set; }

        public static class Properties
        {
            public static string SIGNIN_PAGE = "/Account/Signin.aspx";
            public static string HOME_PAGE = "/";

            public static bool USE_LOCAL_TEMPLATE = true;
            public static string RESOURCE_ROOT = ConfigurationManager.AppSettings["ResourceRoot"];

            public static bool FREE_MODE = ConfigurationManager.AppSettings["FreeMode"] == "true";

            public static string USER_ROLE_CACHE_PREFIX = "ROLE_"; // cache key for user roles.
        }

        public static class Log
        {
            private static readonly log4net.ILog underlyingLogger = log4net.LogManager.GetLogger(typeof(AppContext));

            public static void Info(string message)
            {
                underlyingLogger.Info(message);
            }

            public static void Warn(string message)
            {
                underlyingLogger.Warn(message);
            }

            public static void Debug(string message)
            {
                underlyingLogger.Debug(message);
            }

            public static void Error(string message)
            {
                underlyingLogger.Error(message);
                System.Diagnostics.Debug.WriteLine("(ERROR)" + message);
            }

            public static void Error(string message, Exception exception)
            {
                underlyingLogger.Error(message, exception);
                System.Diagnostics.Debug.WriteLine("(ERROR)" + message);
            }

            public static void Fatal(string message)
            {
                underlyingLogger.Fatal(message);
            }

            public static void Fatal(string message, Exception exception)
            {
                underlyingLogger.Fatal(message, exception);
            }
        }

        #endregion

        #region Start() and End()
        public static void Start()
        {
            InitializeFacilities();
            BuildApplicationCore();
        }

        public static void End()
        {
            Dispose();
        } 
        #endregion

        #region Initialize() and Dispose()

        private static void InitializeFacilities()
        {
            //init for the log system
            var configForLogging = ConfigurationManager.AppSettings["LoggingConfig"].ToServerPath();
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(configForLogging));

            //init the database
            var configForDatabase = ConfigurationManager.AppSettings["DatabaseConfig"].ToServerPath();
            IConfigurationSource source = new XmlConfigurationSource(configForDatabase);

            ActiveRecordStarter.Initialize(source,
                typeof(Data.Log), //为避免数据库体积过大，将日志记录至 /tmp 目录下
                typeof(Users),
                typeof(Roles),
                typeof(Account),
                typeof(Block),
                typeof(Bot),
                typeof(Mine),
                typeof(MineProfile),
                typeof(Share)
            );
        }

        private static void Dispose()
        {
            Core.Dispose();
            Core = null;
        }

        #endregion

        #region BuildApplicationCore()
        private static void BuildApplicationCore()
        {
            //register database entities
            Core.Register(Component.For<Account>().ImplementedBy(typeof(Account)));
            Core.Register(Component.For<Block>().ImplementedBy(typeof(Block)));
            Core.Register(Component.For<Bot>().ImplementedBy(typeof(Bot)));
            Core.Register(Component.For<Mine>().ImplementedBy(typeof(Mine)));
            Core.Register(Component.For<Share>().ImplementedBy(typeof(Share)));

        } 
        #endregion

        #region Core Operations: ResolveBy() / Resolve()
        public static T ResolveBy<T>(params object[] args) where T : class
        {
            IDictionary parameters = new Hashtable();
            for (var idx = 0; idx < args.Length; idx++)
            {
                parameters.Add(idx, args[idx]);
            }
            return Core.Resolve<T>(parameters);
        }

        public static T Resolve<T>() where T : class
        {
            return Core.Resolve<T>();
        }

        public static T Resolve<T>(string key) where T : class
        {
            return Core.Resolve<T>(key);
        } 
        #endregion

        public static void Update()
        {
            ActiveRecordStarter.UpdateSchema();
        }
    }
}