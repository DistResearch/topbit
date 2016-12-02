#region Copyright

//==============================================================================
//  File Name   :   Server.cs
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
//   <record date="2011-06-25 20:57:35" author="Zhang Ling" revision="1.00.000">
//		First version of Server.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using App.Web.Business.Data;
    using Castle.Core;
    using log4net;

    

    /// <summary>
    ///  Summary of Server.
    /// </summary>
    public class Server
    {
        //static
        public static readonly Server Recent = new Server();

        private static readonly ILog Log = LogManager.GetLogger(typeof(Server));

        //runtime properties
        private DateTime NextUpdate;
        private int LastUpdateCost = 0;

        private int totalShare { get; set; }
        private Dictionary<int, int> ShareByMine { get; set; }
        private Dictionary<int, int> ShareByBot { get; set; }
        private Dictionary<int, int> ShareByAccount { get; set; }
        private Dictionary<Pair<int, int>, int> ShareByMineBot { get; set; }
        private Dictionary<Pair<int, int>, int> ShareByMineAccount { get; set; }
        private Dictionary<Pair<int, int>, int> ShareByMineBlock { get; set; }
        private Dictionary<Triple, int> ShareByMineBlockAccount { get; set; }

        private Dictionary<int, BotInfo> BotStatistics { get; set; }

        private decimal totalSpeed { get; set; }
        
        /// <summary>
        /// 初始化股份管理器
        /// </summary>
        public Server()
        {
            NextUpdate = DateTime.MinValue;
            BotStatistics = new Dictionary<int, BotInfo>();
        }

        public void LogShare(Bot bot, Block block, int mineId, string data, bool localResult, bool networkResult)
        {
            var item = new Share();

            item.AccountId = bot.Account != null ? bot.Account.Id : 0;
            item.BlockId = block.Id;
            item.BotId = bot.Id;
            item.MineId = mineId;

            item.LocalResult = localResult;
            item.NetworkResult = networkResult;
            item.Solution = data.Substring(0, 160);
            item.Result = 1;

            try
            {
                item.IPAddress = HttpContext.Current.Request.UserHostAddress;
            }
            catch (Exception e)
            {
                Log.Error("Exception in Server.LogShare - Retrieve IPAddress", e);
            }

            item.Save();
        }

        public void LogBotWork(Bot bot, string work)
        {
            BotInfo botInfo;
            if(!BotStatistics.TryGetValue(bot.Id, out botInfo))
            {
                botInfo = new BotInfo(bot);
                lock (BotStatistics)
                {
                    BotStatistics[bot.Id] = botInfo;
                }
            }
            Work w = Work.Parse(work);
            botInfo.RequestWork(work, w.PreviousHash);
        }

        public bool LogBotSubmit(Bot bot, string work)
        {
            BotInfo botInfo;
            if (!BotStatistics.TryGetValue(bot.Id, out botInfo))
            {
                botInfo = new BotInfo(bot);
                lock (BotStatistics)
                {
                    BotStatistics[bot.Id] = botInfo;
                }
            }
            return botInfo.SubmitWork(work);
        }

        /// <summary>
        /// 矿池速度
        /// </summary>
        public decimal TotalSpeed
        {
            get
            {
                this.CheckUpdate();
                return totalSpeed;
            }
        }

        /// <summary>
        /// 股份总数
        /// </summary>
        public int TotalShare
        {
            get
            {
                this.CheckUpdate();
                return totalShare;
            }
        }

        public int GetShareByMine(int mineId)
        {
            this.CheckUpdate();
            int shore;
            return ShareByMine.TryGetValue(mineId, out shore) ? shore : 0;
        }

        public int GetShareByBot(int botId)
        {
            this.CheckUpdate();
            int shore;
            return ShareByBot.TryGetValue(botId, out shore) ? shore : 0;
        }

        public decimal TestBotSpeed(int botId)
        {
            BotInfo botInfo;
            return BotStatistics.TryGetValue(botId, out botInfo) ? botInfo.Speed : 0;
        }

        public int GetShareByAccount(int accountId)
        {
            this.CheckUpdate();
            int shore;
            return ShareByAccount.TryGetValue(accountId, out shore) ? shore : 0;
        }

        public int GetShareByMineBot(int mineId, int botId)
        {
            this.CheckUpdate();
            int shore;
            return ShareByMineBot.TryGetValue(new Pair<int, int>(mineId, botId), out shore) ? shore : 0;
        }

        public int GetShareByMineAccount(int mineId, int accountId)
        {
            this.CheckUpdate();
            int shore;
            return ShareByMineAccount.TryGetValue(new Pair<int, int>(mineId, accountId), out shore) ? shore : 0;
        }

        public int GetShareByMineBlock(int mineId, int blockId)
        {
            this.CheckUpdate();
            int shore;
            return ShareByMineBlock.TryGetValue(new Pair<int, int>(mineId, blockId), out shore) ? shore : 0;
        }

        public int GetShareByMineBlockAccount(int mineId, int blockId, int accountId)
        {
            this.CheckUpdate();
            int shore;
            return ShareByMineBlockAccount.TryGetValue(new Triple(mineId, blockId, accountId), out shore) ? shore : 0;
        }

        public bool CheckUpdate()
        {
            if (DateTime.Now < NextUpdate)
            {
                return false;
            }

            lock(this)
            {
                if (DateTime.Now < NextUpdate)
                {
                    return false;
                }

                RunUpdate();

                //update every 60 minutes
                NextUpdate = DateTime.Now.AddMinutes(30);
            }

            return true;
        }

        private void RunUpdate()
        {
            var start = DateTime.Now;

            //total shore
            totalShare = Bot.Queryable.Sum(b => b.Share);

            //GetShareByMine
            ShareByMine = ( 
                            from s in Share.Queryable
                            group s by s.MineId into sbm
                            select sbm 
                           )
                            .ToDictionary(k => k.Key, v => v.Sum(s => s.Result));

            //ShareByBot
            ShareByBot = (  
                            from share in Share.Queryable
                            group share by share.BotId into sbb
                            select sbb 
                          )
                          .ToDictionary(k => k.Key, v => v.Sum(s => s.Result));

            //ShareByAccount
            ShareByAccount = (  
                                from share in Share.Queryable
                                group share by share.AccountId into sbb
                                select sbb
                             )
                             .ToDictionary(k => k.Key, v => v.Sum(s => s.Result));

            //private static Dictionary<string, int> ShareByMineBot { get; set; }
            ShareByMineBot = (
                                from share in Share.Queryable
                                group share by new Pair<int, int>(share.MineId, share.BotId) into mine
                                select mine
                            )
                            .ToDictionary(k => k.Key, v => v.Sum(s => s.Result));

            //private static Dictionary<string, int> ShareByMineBot { get; set; }
            ShareByMineAccount = (
                                from share in Share.Queryable
                                group share by new Pair<int, int>(share.MineId, share.AccountId) into mine
                                select mine
                            )
                            .ToDictionary(k => k.Key, v => v.Sum(s => s.Result));
            
            //private static Dictionary<string, int> ShareByMineBlock { get; set; }
            ShareByMineBlock = (
                    from share in Share.Queryable
                    group share by new Pair<int, int>(share.MineId, share.BlockId) into mine
                    select mine
                )
                .ToDictionary(k => k.Key, v => v.Sum(s => s.Result));

            //private Dictionary<Triple, int> ShareByMineBlockAccount { get; set; }
            ShareByMineBlockAccount = (
                    from share in Share.Queryable
                    group share by new Triple(share.MineId,  share.BlockId, share.AccountId) into mine
                    select mine
                )
                .ToDictionary(k => k.Key, v => v.Sum(s => s.Result));

            //TotalSpeed
            totalSpeed = Mine.Queryable.Sum(m=>(decimal)m.Speed);

            LastUpdateCost = (int)Math.Ceiling((DateTime.Now - start).TotalMilliseconds);

            Log.InfoFormat("[Perf] Update Server cost {0} ms", LastUpdateCost);
        }
    }

    public struct Triple
    {
        public Triple(int first, int second, int third) : this()
        {
            this.First = first;
            this.Second = second;
            this.Third = third;
        }

        public int First { get; private set; }
        public int Second { get; private set; }
        public int Third { get; private set; }
    }
}