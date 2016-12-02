#region Copyright

//==============================================================================
//  File Name   :   ComputeManager.cs
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
//   <record date="2011-06-22 00:31:17" author="Zhang Ling" revision="1.00.000">
//		First version of ComputeManager.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using App.Web.Business.Channel;
    using App.Web.Business.Data;
    using log4net;

    /// <summary>
    ///  Summary of ComputeManager.
    /// </summary>
    public class ComputeManager
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof (ComputeManager));
        private static Dictionary<int, ComputeManager> instances = new Dictionary<int, ComputeManager>();

        private static Queue<Mine> systemPreferred;
        private static Mine systemGateway;

        static ComputeManager()
        {
            logger.Info("Initialize ComputeManager");
            InitializeQueue();
            logger.Info("Initialize ComputeManager Complete.");
        }

        public static void InitializeQueue()
        {
            systemGateway = new Mine 
                {
                    Address = "http://localhost:8332",
                    DefaultAccount = "topbit",
                    DefaultPassword = "topbit20110629"
                };

            var list = Mine.Queryable.ToList()
                .Select(m => new
                {
                    Id = m.Id,
                    Priority = GetMinePriority(m)
                })
                .Where(m => m.Priority >= 0)
                .OrderBy(m => m.Priority).ToList();

            var sp = new Queue<Mine>();
            foreach (var m in list)
            {
                var mine = Mine.Find(m.Id);
                if(mine != null)
                {
                    sp.Enqueue(mine);
                }
            }
            systemPreferred = sp;
        }

        private static int GetMinePriority(Mine m)
        {
            var score = m.Id;
            score += m.IsOnline ? 750 : 0;
            score += (int)(DateTime.Now - m.NextCheckTime).TotalHours;

            if (string.IsNullOrWhiteSpace(m.DefaultAccount) || string.IsNullOrWhiteSpace(m.DefaultPassword))
                score = -1;

            if (!m.Offshore)
                score = -1;

            return score;
        }

        public static void Clear()
        {
            lock (instances)
            {
                instances = new Dictionary<int, ComputeManager>();
            }
        }

        public static void Reset(Bot bot)
        {
            lock (instances)
            {
                instances.Remove(bot.Id);
            }
        }

        public static Mine[] GetQueueSnapshot()
        {
            return systemPreferred.ToArray();
        }

        public static ComputeManager ByBot(Bot bot)
        {
            ComputeManager computeMgr;
            if (!instances.TryGetValue(bot.Id, out computeMgr))
            {
                lock(instances)
                {
                    if (!instances.TryGetValue(bot.Id, out computeMgr))
                    {
                        computeMgr = new ComputeManager(bot);
                        computeMgr.UpdateServerWithPreferredMineAndCredential();
                        instances[bot.Id] = computeMgr;
                    }
                }
            }
            return computeMgr;
        }

        public static ComputeManager Gateway
        {
            get
            {
                ComputeManager computeMgr;
                if (!instances.TryGetValue(0, out computeMgr))
                {
                    lock (instances)
                    {
                        if (!instances.TryGetValue(0, out computeMgr))
                        {
                            instances[0] = computeMgr = new ComputeManager();
                        }
                    }
                }
                return computeMgr;
            }
        }

       
        //readonly
        private readonly bool isGateway;
        private readonly Bot bot;

        //runtime changes
        private BitServer server;
        private Mine mine;

        public ComputeManager()
        {
            this.isGateway = true;
            this.server = BitServer.Create(systemGateway.Address, systemGateway.DefaultAccount, systemGateway.DefaultPassword);
            logger.InfoFormat("Create ComputeManager Gateway {0} {1} {2}", systemGateway.Address, systemGateway.DefaultAccount, systemGateway.DefaultPassword);
        }

        public ComputeManager(Bot bot)
        {
            this.isGateway = false;
            this.bot = bot;
        }

        public int MineId
        {
            get { return mine.Id; }
        }

        public Result GetWork(AsyncCallback callback)
        {
            var count = 3;
            var checkAgain = false;

            do
            {
                try
                {
                    //get work
                    return server.GetWork(callback);
                }
                catch (WebException webException)
                {
                    checkAgain = RequireWorkFromAnotherMine(webException);
                    if (checkAgain)
                    {
                        //pick up another server and try again
                        UpdateServerWithPreferredMineAndCredential();
                    }
                    else
                    {
                        logger.Error("Abort GetWork on WebException for bot " + bot.Id + " at " + bot.IPAddress, webException);
                    }
                }
                catch (Exception exception)
                {
                    logger.Error("Abort GetWork on Exception for bot " + bot.Id + " at " + bot.IPAddress, exception);
                    return new Result() { HttpStatusCode = 404, DataText = "Server is offline.", Data = false };
                }

            } while (checkAgain && --count > 0);

            return new Result() { HttpStatusCode = 404, DataText = "All Servers are offline. (after 3 times check)", Data = false };
        }

        public Result SubmitWork(Message message)
        {
            try
            {
                //get work
                return server.SubmitWork(message);
            }
            catch (WebException webException)
            {
                if (RequireWorkFromAnotherMine(webException))
                {
                    //pick up another server and try again
                    UpdateServerWithPreferredMineAndCredential();
                }
                logger.Error("Abort GetWork on WebException for bot " + bot.Id + " at " + bot.IPAddress, webException);
            }
            catch (Exception exception)
            {
                logger.Error("Abort GetWork on Exception for bot " + bot.Id + " at " + bot.IPAddress, exception);
                return new Result() { HttpStatusCode = 404, DataText = "Server is offline.", Data = false };
            }

            return new Result() { HttpStatusCode = 404, DataText = "All Servers are offline. (after 1 times check)", Data = false };
        }

        private bool RequireWorkFromAnotherMine(WebException exception)
        {
            if (exception.Status == WebExceptionStatus.RequestCanceled
                || exception.Status == WebExceptionStatus.ConnectFailure
                || exception.Status == WebExceptionStatus.ConnectionClosed
                || exception.Status == WebExceptionStatus.Timeout)
            {
                lock (systemPreferred)
                {
                    mine.Refresh();

                    //update mine
                    mine.IsOnline = false;
                    mine.CheckTime = DateTime.Now; //the time server down
                    mine.NextCheckTime = mine.CheckTime.AddMinutes(45);
                    mine.LastException = exception.Message;
                    mine.SaveAndFlush();

                    //update SystemPreferred queue
                    var m = systemPreferred.Peek();

                    if (mine.Id == m.Id)
                    {
                        //put the current mine config to the tail of the queue
                        systemPreferred.Dequeue();
                        systemPreferred.Enqueue(m);
                    }
                }
                return true;
            }
            return false; //needn't request for another mining pool config.
        }

        public void UpdateServerWithPreferredMineAndCredential()
        {
            if (isGateway)
                return;

            try
            {
                bot.Refresh();

                //1. 优先查看 MineProfile 的配置
                var cma = MineProfile.FindByBot(bot.Id);
                if (cma != null && cma.MineId > 0 && !string.IsNullOrWhiteSpace(cma.CustomAccount) && !string.IsNullOrWhiteSpace(cma.CustomPassword))
                {
                    var m = Mine.Find(cma.MineId);
                    if (m != null)
                    {
                        server = BitServer.Create(m.Address, cma.CustomAccount, cma.CustomPassword);
                        mine = m;
                        return;
                    }
                }

                //2. 其次查看 SystemPreferred
                var preferred = systemPreferred.Peek();
                preferred.Refresh();

                //2.5 如果有绑定信息的话
                if (bot.Account != null)
                {
                    var pma = MineProfile.FindByAccountMine(bot.Account.Id, preferred.Id);
                    if (pma != null && !string.IsNullOrWhiteSpace(pma.CustomAccount) && !string.IsNullOrWhiteSpace(pma.CustomPassword))
                    {
                        var m = Mine.Find(pma.MineId);
                        if (m != null)
                        {
                            server = BitServer.Create(preferred.Address, pma.CustomAccount, pma.CustomPassword);
                            mine = m;
                            return;
                        }
                    }
                }

                //3. 最后使用系统自带的
                server = BitServer.Create(preferred.Address, preferred.DefaultAccount, preferred.DefaultPassword);
                mine = preferred;
            }
            catch(Exception error)
            {
                logger.Error("UpdateServerWithPreferredMineAndCredential", error);
            }
        } 
    }
}