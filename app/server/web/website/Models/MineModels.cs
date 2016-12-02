#region Copyright

//==============================================================================
//  File Name   :   MineModels.cs
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
//   <record date="2011-06-21 22:26:26" author="Zhang Ling" revision="1.00.000">
//		First version of MineModels.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using App.Web.Business.Data;
    using App.Web.Business.Utils;
    using WebMatrix.WebData;

    /// <summary>
    ///  Summary of MineModels.
    /// </summary>
    public class MineModel
    {
        public MineModel()
        {
        }

        public MineModel(Mine mine, MineProfile profile = null)
        {
            this.Id = profile == null ? mine.Id : profile.Id;
            this.Address = mine.Address;
            this.HomePage = mine.HomePage;
            //this.Certificated = mine.Certificated;
            this.DisplayName = mine.DisplayName;
            this.Name = mine.Name; 
            this.Icon = mine.Icon;
            this.IconText = mine.IconName;
            this.Description = mine.Description ?? "暂时没啥好说的";
            //this.Offshore = mine.Offshore;
            //this.Private = mine.Private;
            this.Speed = mine.Speed;
            this.Tax = Format.AsPercent(mine.TaxRate, 100);
            this.Status = mine.IsOnline ? "正常" : "离线"; //满载
            this.UpdatedAt = mine.CheckTime;

            this.Ownership = mine.Account != null && mine.Account.UserId == WebSecurity.CurrentUserId;

            this.Action = "Profile";//profile == null ? "Profile" : "Edit";
            this.ActionText = profile == null ? "自定义" : "修改";
            
        }

        [Display(Name = "矿池编号")]
        public int Id { get; set; }

        [Display(Name = "矿池状态")]
        public string Status { get; set; }

        [Display(Name = "矿池代号")]
        public string Name { get; set; }

        [Display(Name = "矿池名称")]
        public string DisplayName { get; set; }

        [Display(Name = "矿池图标")]
        public string Icon { get; set; }

        [Display(Name = "矿池图标名称")]
        public string IconText { get; set; }

        [Display(Name = "矿池说明")]
        public string Description { get; set; }

        [Display(Name = "矿池地址")]
        public string Address { get; set; }

        [Display(Name = "矿池主页")]
        public string HomePage { get; set; }

        [Display(Name = "矿池速度")]
        public decimal Speed { get; set; }

        [Display(Name = "税率")]
        public string Tax { get; set; }

        [Display(Name = "矿池股份")]
        public int Shares { get; set; }

        [Display(Name = "矿主")]
        public bool Ownership { get; set; }

        [Display(Name = "连接的挖掘机数")]
        public int Workers { get; set; }

        [Display(Name = "挖掘机列表")]
        public IList<BotModel> Bots { get; set; }

        [Display(Name = "更新时间")]
        public DateTime UpdatedAt { get; set; }

        [Required]
        [Display(Name = "可选操作")]
        public string Action { get; set; }

        [Required]
        [Display(Name = "可选操作")]
        public string ActionText { get; set; }
    }
}