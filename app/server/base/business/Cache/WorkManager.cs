#region Copyright

//==============================================================================
//  File Name   :   WorkManager.cs
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
//   <record date="2011-06-13 23:46:05" author="Zhang Ling" revision="1.00.000">
//		First version of WorkManager.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Cache
{
    using System;
    using System.Web;
    using App.Web.Business.Channel;
    using App.Web.Business.Data;
    using App.Web.Business.Utils;
    using log4net;
    using Newtonsoft.Json.Linq;

    /// <summary>
    ///  Summary of WorkManager.
    /// </summary>
    public static class WorkManager
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof (WorkManager));

        private static readonly byte[] EmptyByte = new byte[0];
        private static readonly Result AcceptResult = new Result() { DataText = "{result:'true'}" };
        private static readonly Result RejectResult = new Result() { DataText = "{result:'false'}" };

        public static Result TestWork(Message message, string username, string password)
        {
            byte[] data;
            if (ParseInput(message, out data))
            {
                bool localCheckResult = false;
                bool networkCheckResult = false;
                Work work = new Work(data);

                Bot bot = Bot.FindBy(username, password);
                if (bot == null)
                {
                    if (work.HashScore < 7)
                    {
                        //sorry, you don't meet the minimal requirement.
                        return RejectResult;
                    }
                    bot = Bot.Create(username, password);
                }

                //需要重大改进的地方：
                // 对hash进行简单判断后立刻返回，可以减少对 tcp 连接的占用
                // 目前 mining 客户端连接到服务器之后，要等到服务器提交结果并得到反馈之后才能关闭连接
                // 因此mining 客户端的连接时间肯定比服务器提交时间要长，如果有100个提交，那么应该就有200个连接在服务器上，
                // 有可能造成严重阻塞。
                //
                //当使用人数上升之后需要为 getwork 及 submitwork 增加二级缓存的概念。

                //submit to gateway
                try
                {
                    //
                    var localResult = ComputeManager.Gateway.SubmitWork(message);
                    localCheckResult = (bool)localResult.Data;

                    var server = ComputeManager.ByBot(bot);

                    //log to database
                    if (localCheckResult)
                    {
                        string hash = message.Arguments[0];
                        Server.Recent.LogBotSubmit(bot, hash);
                        Block block;
                        BlockManager.Instance.CreateOrRetrieve(work.PreviousHash, out block);
                        Server.Recent.LogShare(bot, block, server.MineId, hash, localCheckResult, networkCheckResult);
                    }

                    return localResult;
                }
                catch (Exception e)
                {
                    logger.Error("Error on submit to gateway - Id:" + ComputeManager.Gateway.MineId, e);
                }
            }
            return RejectResult;
        }

        public static Result SubmitWork(Message message, string username, string password)
        {
            Bot bot = Bot.FindBy(username, password);
            if (bot == null)
            {
                throw new InvalidOperationException("Bot does not exists.");
            }

            //submit offshore mining pool
            var server = ComputeManager.ByBot(bot);
            var networkResult = server.SubmitWork(message);

            // 无论远程返回的结果如何，均不影响分配

            return networkResult;
        }

        public static Result GetWork(string username, string password, AsyncCallback callback)
        {
            Bot bot = null;
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                bot = Bot.FindBy(username, password);
            }
            if (bot == null)
            {
                bot = Bot.Default;
            }

            var mineMgr = ComputeManager.ByBot(bot);
            var work = mineMgr.GetWork(callback);

            return work;
        }

        #region Helper
        static bool ParseInput(Message message, out byte[] data)
        {
            data = EmptyByte;

            if (message.Arguments == null || message.Arguments.Count <= 0)
                return false;

            var solution = message.Arguments[0];

            if (string.IsNullOrWhiteSpace(solution))
                return false;

            if (solution.Length != 256)
                return false;

            try
            {
                if (BitConverter.IsLittleEndian)
                {
                    data = HexEncoding.Decode(solution);
                    return data.Length == 128;
                }

                //sorry, we do not support big-endian system
                return false;
            }
            catch
            {
                return false;
            }
        } 
        #endregion
    }

    //public enum WorkEstimation : byte
    //{
    //    StaleWork,
    //    UnkonwnBot,
    //    TimeTooOld,
    //    TimeTooNew,
    //    UnknownWork,
    //    NotZero,
    //    OK,
    //    Good
    //}
}