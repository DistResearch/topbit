#region Copyright

//==============================================================================
//  File Name   :   BotModels.cs
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
//      This file is a part of website project.
//   </summary>
//   <author name="Zhang Ling" mail="tox@e2.org.cn"/>
//   <seealso ref=""/>
// </fileinformation>
//
// <history>
//   <record date="2011-06-20 22:36:43" author="Zhang Ling" revision="1.00.000">
//		First version of BotModels.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using App.Web.Business.Cache;
    using App.Web.Business.Data;
    using App.Web.Business.Utils;

    public class BotModel
    {
        public BotModel()
        {
        }

        public BotModel(Bot bot)
        {
            this.Id = bot.Id;
            this.Name = bot.Name ?? bot.IPAddress;
            this.WorkHours = Format.AsAge(bot.CreationDate);
            this.Profit = Format.AsBitCoin(bot.Balance);
            this.Efficiency = Format.AsPercent(bot.Share, bot.TotalShare);
            this.EfficiencyText = string.Format("工作：{0} (总数：{1})", bot.Share, bot.TotalShare);
            this.Shares = Format.AsCurrentAndPercent(bot.Share, Server.Recent.TotalShare);
            this.Action = bot.Account == null ? "Authorize" : "Manage";
            this.ActionText = bot.Account == null ? "认领" : "管理";

            //try get mine information from bot
            var cm = ComputeManager.ByBot(bot);
            if(cm != null)
            {
                var mine = Mine.Find(cm.MineId);
                if (mine != null)
                {
                    this.MineIcon = mine.Icon;
                    this.MineText = mine.IconName;
                }
            }

            //try get bot speed
            var speed = Server.Recent.TestBotSpeed(bot.Id);
            this.StatusIcon = "/Content/status/" + Format.AsSpeedIcon(speed) + ".png";
            this.StatusText = Format.AsSpeed(speed);
        }

        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "状态")]
        public string StatusIcon { get; set; }

        [Required]
        [Display(Name = "状态提示")]
        public string StatusText { get; set; }

        [Display(Name = "矿池图标")]
        public string MineIcon { get; set; }

        [Display(Name = "矿池图标名称")]
        public string MineText { get; set; }

        [Required]
        [Display(Name = "挖掘机名称")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "利润")]
        public string Profit { get; set; }

        [Required]
        [Display(Name = "效率")]
        public string Efficiency { get; set; }

        [Required]
        [Display(Name = "效率详情")]
        public string EfficiencyText { get; set; }

        [Required]
        [Display(Name = "服役时间")]
        public string WorkHours { get; set; }

        [Required]
        [Display(Name = "股份及股份所占比率")]
        public string Shares { get; set; }

        [Required]
        [Display(Name = "可选操作")]
        public string Action { get; set; }

        [Required]
        [Display(Name = "可选操作")]
        public string ActionText { get; set; }
    }

    public class CreateBotModel
    {
        [Required]
        [StringLength(8, MinimumLength = 3)]
        [RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed")]
        [Display(Name = "挖掘机帐号")]
        public string Login { get; set; }

        [Required]
        [StringLength(9, MinimumLength = 2)]
        [Display(Name = "挖掘机密码")]
        public string Password { get; set; }

        [Display(Name = "首选矿池")]
        public int[] SelectedMines { get; set; }

        [Display(Name = "可用矿池")]
        public MultiSelectList Mines { get; set; }
    }
}