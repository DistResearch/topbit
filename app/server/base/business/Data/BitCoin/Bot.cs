#region Copyright

//==============================================================================
//  File Name   :   Bot.cs
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
//      This file is a part of Deepbot project.
//   </summary>
//   <author name="Zhang Ling" mail="tox@e2.org.cn"/>
//   <seealso ref=""/>
// </fileinformation>
//
// <history>
//   <record date="2011-06-12 22:28:38" author="Zhang Ling" revision="1.00.000">
//		First version of Bot.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Data
{
    using System;
    using System.Web;
    using Castle.ActiveRecord;
    using log4net;
    using NHibernate.Criterion;

    /// <summary>
    ///  Summary of Bot.
    /// </summary>
    [ActiveRecord("Bot")]
    public class Bot : TrackableNumberIndexedRecord<Bot>
    {
        //public
        public static readonly Bot Default = new Bot { Id = 0, Name = "Topbit" };

        //private
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Bot));
        private static readonly object _createLock = new object();

        #region CRUD Opreations: FindBy(login), FindBy(login, password)

        public static Bot Create(string login, string password)
        {
            lock (_createLock)
            {
                var bot = FindOne(Restrictions.And(Restrictions.Eq("Login", login.ToLowerInvariant()), Restrictions.Eq("Password", password)));
                if (bot == null)
                {
                    var tmp = new Bot
                    {
                        Login = login.ToLowerInvariant(),
                        Password = password,
                        ConfirmCode = App.Web.Business.Cache.ConfirmCode.CreateBotConfirmCode()
                    };

                    try
                    {
                        tmp.IPAddress = HttpContext.Current.Request.UserHostAddress;
                    }
                    catch (Exception e)
                    {
                        _logger.ErrorFormat("Error when CreateOrRetrieve(Retrieve IPAddress) the bot with user:{0} and password:{1}", login, password);
                        _logger.Error("Exception in BotManager.CreateOrRetrieve(login, password) - Retrieve IPAddress", e);
                    }

                    tmp.Save();
                    bot = tmp;
                }
                return bot;
            }
        }

        public static Bot[] FindBy(string login)
        {
            //登录名不区分大小写
            return FindAll(Restrictions.Eq("Login", login.ToLowerInvariant()));
        }

        public static Bot FindBy(string login, string password)
        {
            //登录名不区分大小写
            return FindOne(Restrictions.And(Restrictions.Eq("Login", login.ToLowerInvariant()), Restrictions.Eq("Password", password)));
        }

        #endregion

        /// <summary>
        /// Bot 的认领 email 地址
        /// </summary>
        [Property(NotNull = true)]
        public string Login { get; set; }

        /// <summary>
        /// Bot 认领密码
        /// </summary>
        [Property(NotNull = false)]
        public string Password { get; set; }

        /// <summary>
        /// Bot 认领代码
        /// </summary>
        [Property(NotNull = false)]
        public string ConfirmCode { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        /// <remarks>
        /// Balance increased after block solve
        /// </remarks>
        [Property(NotNull = false)]
        public double Balance { get; set; }

        /// <summary>
        /// Bot 的名称 (认领后可修改)
        /// </summary>
        [Property(NotNull = false)]
        public string Name { get; set; }

        /// <summary>
        /// 创建 Bot 时的 IP 地址
        /// </summary>
        [Property(NotNull = false)]
        public string IPAddress { get; set; }

        /// <summary>
        /// 这个 Bot 所属帐号 （可后期绑定）
        /// </summary>
        [BelongsTo("AccountId", Type = typeof(Account), Cascade = CascadeEnum.None, NotNull = false)]
        public Account Account { get; set; }

        /// <summary>
        /// 工作
        /// </summary>
        [Property(NotNull = false)]
        public int Share { get; set; }

        /// <summary>
        /// 工作总数
        /// </summary>
        [Property(NotNull = false)]
        public int TotalShare { get; set; }

        /// <summary>
        /// 是否允许执行指令
        /// </summary>
        [Property(NotNull = false)]
        public bool AcceptCommand { get; set; }

        /// <summary>
        /// 等待执行的挖掘命令
        /// </summary>
        [Property(NotNull = false)]
        public string Command { get; set; }

        /// <summary>
        /// 挖掘命令执行的结果
        /// </summary>
        [Property(NotNull = false)]
        public string CommandResult { get; set; }

        /// <summary>
        /// 挖掘速度
        /// </summary>
        public int Nonce { get; set; }

        /// <summary>
        /// 两次挖掘之间的延迟
        /// </summary>
        public int NonceDelay { get; set; }
    }
}