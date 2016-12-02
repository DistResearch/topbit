#region Copyright

//==============================================================================
//  File Name   :   WorkModels.cs
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
//   <record date="2011-06-23 23:31:06" author="Zhang Ling" revision="1.00.000">
//		First version of WorkModels.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Newtonsoft.Json;

    public class WorkModel
    {
        [Display(Name = "帐号")]
        public string Account { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "支付宝帐号")]
        public string CreditAccount { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "余额")]
        [DisplayFormat(DataFormatString = "{0:#.##} RMB")]
        public double Credit { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "电子货币帐号")]
        public string BitCoinAccount { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "比特币")]
        [DisplayFormat(DataFormatString = "{0:#.########} BTC")]
        public double BitCoin { get; set; }

        [Display(Name = "挖掘速度")]
        public string MiningSpeed { get; set; }

        [Display(Name = "矿池速度")]
        public string PoolSpeed { get; set; }

        [Display(Name = "运行中的挖掘机数量 (总数)")]
        public string Workers { get; set; }

        [Display(Name = "挖掘机")]
        public IList<BotModel> Bots;

        [Display(Name = "自定义矿池")]
        public IList<MineModel> CustomMines;

        [Display(Name = "未配置矿池")]
        public IList<MineModel> Mines;

        public MvcHtmlString ToJson()
        {
            return new MvcHtmlString(JsonConvert.SerializeObject(this));
        }
    }
}