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
            this.EfficiencyText = string.Format("������{0} (������{1})", bot.Share, bot.TotalShare);
            this.Shares = Format.AsCurrentAndPercent(bot.Share, Server.Recent.TotalShare);
            this.Action = bot.Account == null ? "Authorize" : "Manage";
            this.ActionText = bot.Account == null ? "����" : "����";

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
        [Display(Name = "״̬")]
        public string StatusIcon { get; set; }

        [Required]
        [Display(Name = "״̬��ʾ")]
        public string StatusText { get; set; }

        [Display(Name = "���ͼ��")]
        public string MineIcon { get; set; }

        [Display(Name = "���ͼ������")]
        public string MineText { get; set; }

        [Required]
        [Display(Name = "�ھ������")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "����")]
        public string Profit { get; set; }

        [Required]
        [Display(Name = "Ч��")]
        public string Efficiency { get; set; }

        [Required]
        [Display(Name = "Ч������")]
        public string EfficiencyText { get; set; }

        [Required]
        [Display(Name = "����ʱ��")]
        public string WorkHours { get; set; }

        [Required]
        [Display(Name = "�ɷݼ��ɷ���ռ����")]
        public string Shares { get; set; }

        [Required]
        [Display(Name = "��ѡ����")]
        public string Action { get; set; }

        [Required]
        [Display(Name = "��ѡ����")]
        public string ActionText { get; set; }
    }

    public class CreateBotModel
    {
        [Required]
        [StringLength(8, MinimumLength = 3)]
        [RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed")]
        [Display(Name = "�ھ���ʺ�")]
        public string Login { get; set; }

        [Required]
        [StringLength(9, MinimumLength = 2)]
        [Display(Name = "�ھ������")]
        public string Password { get; set; }

        [Display(Name = "��ѡ���")]
        public int[] SelectedMines { get; set; }

        [Display(Name = "���ÿ��")]
        public MultiSelectList Mines { get; set; }
    }
}