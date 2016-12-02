#region Copyright

//==============================================================================
//  File Name   :   BotInfo.cs
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
//   <record date="2011-06-26 17:05:19" author="Zhang Ling" revision="1.00.000">
//		First version of BotInfo.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Cache
{
    using System;
    using System.Collections.Generic;
    using App.Web.Business.Data;
    using log4net;
    using log4net.Repository.Hierarchy;

    /// <summary>
    /// 数据量太大，更新过于频繁，因此不存数据库
    /// </summary>
    /// <remarks>
    /// 需要记录这个 Bot 申请的每一份工作及反馈，每当 Block solved 之后，重置所有数据。
    /// </remarks>
    public class BotInfo
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BotInfo));

        public readonly int Id;
        private readonly Dictionary<string, bool> workSubmitResult;
        private readonly Dictionary<string, bool> workSubmitNettworkResult;

        private decimal speed;
        private string blockhash;
        private string lastsubmit;
        private string lastsubmitdata;
        private Dictionary<string, DateTime> workRequest;
        private Dictionary<string, DateTime> workSubmit;

        public BotInfo(Bot bot)
        {
            this.Id = bot.Id;
            this.speed = 0;
            this.blockhash = string.Empty;
            this.workRequest = new Dictionary<string, DateTime>();
            this.workSubmit = new Dictionary<string, DateTime>();
            this.workSubmitResult = new Dictionary<string, bool>();
            this.workSubmitNettworkResult = new Dictionary<string, bool>();
        }

        public decimal Speed
        {
            get
            {
                if (!string.IsNullOrEmpty(lastsubmit) && workRequest.ContainsKey(lastsubmit) && workSubmit.ContainsKey(lastsubmit))
                {
                    var timeDiff = workSubmit[lastsubmit] - workRequest[lastsubmit];
                    var work = Work.Parse(lastsubmitdata);
                    speed =  decimal.Divide(work.Nonce, (decimal)timeDiff.TotalMilliseconds) * 1000;
                }
                return speed;
            }
        }

        public void RequestWork(string work, string hash)
        {
            if (hash != this.blockhash)
            {
                this.blockhash = hash;
            }

            var identity = work.Substring(0, 152);
            try
            {
                workRequest.Add(identity, DateTime.Now);
            }
            catch (Exception e)
            {
                Logger.Error(Id + " request a duplicated work " + work.Substring(0, 160), e);
            }
        }

        public bool SubmitWork(string work)
        {
            var identity = work.Substring(0, 152);
            lastsubmit = identity;
            lastsubmitdata = work;

            try
            {
                workSubmit.Add(lastsubmit, DateTime.Now);
            }
            catch(Exception e)
            {
                Logger.Error(Id + " submit a duplicated work " + work.Substring(0, 160), e);
            }
            
            return workRequest.ContainsKey(lastsubmit);
        }
    }
}