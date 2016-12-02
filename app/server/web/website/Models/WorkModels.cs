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
        [Display(Name = "�ʺ�")]
        public string Account { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "֧�����ʺ�")]
        public string CreditAccount { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "���")]
        [DisplayFormat(DataFormatString = "{0:#.##} RMB")]
        public double Credit { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "���ӻ����ʺ�")]
        public string BitCoinAccount { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "���ر�")]
        [DisplayFormat(DataFormatString = "{0:#.########} BTC")]
        public double BitCoin { get; set; }

        [Display(Name = "�ھ��ٶ�")]
        public string MiningSpeed { get; set; }

        [Display(Name = "����ٶ�")]
        public string PoolSpeed { get; set; }

        [Display(Name = "�����е��ھ������ (����)")]
        public string Workers { get; set; }

        [Display(Name = "�ھ��")]
        public IList<BotModel> Bots;

        [Display(Name = "�Զ�����")]
        public IList<MineModel> CustomMines;

        [Display(Name = "δ���ÿ��")]
        public IList<MineModel> Mines;

        public MvcHtmlString ToJson()
        {
            return new MvcHtmlString(JsonConvert.SerializeObject(this));
        }
    }
}