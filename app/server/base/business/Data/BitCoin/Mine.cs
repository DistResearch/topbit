#region Copyright

//==============================================================================
//  File Name   :   Mine.cs
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
//   <record date="2011-06-12 09:18:43" author="Zhang Ling" revision="1.00.000">
//		First version of Mine.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Data
{
    using System;
    using System.Collections.Generic;
    using Castle.ActiveRecord;
    using NHibernate.Criterion;

    /// <summary>
    ///  Summary of Mine.
    /// </summary>
    /// <remarks>
    /// 矿池 Score 计算方法
    /// 1. 在线 +100分
    /// </remarks>
    [ActiveRecord("Mine")]
    public class Mine : TrackableNumberIndexedRecord<Mine>
    {
        public static Mine Create(string address, string username, string password)
        {
            return new Mine() {Address = address, DefaultAccount = username, DefaultPassword = password};
        }

        /// <summary>
        /// 矿池代码
        /// </summary>
        [Property]
        public string Name { get; set; }

        /// <summary>
        /// 矿池显示名字
        /// </summary>
        [Property]
        public string DisplayName { get; set; }

        /// <summary>
        /// 矿池图片
        /// </summary>
        [Property]
        public string Icon { get; set; }

        /// <summary>
        /// 矿池图片名称
        /// </summary>
        [Property]
        public string IconName { get; set; }

        /// <summary>
        /// 矿池说明
        /// </summary>
        [Property]
        public string Description { get; set; }

        /// <summary>
        /// 认证的矿
        /// </summary>
        [Property]
        public bool Certificated { get; set; }

        /// <summary>
        /// 专属矿
        /// </summary>
        [Property]
        public bool Private { get; set; }

        /// <summary>
        /// 远程矿
        /// </summary>
        [Property]
        public bool Offshore { get; set; }

        /// <summary>
        /// 远程矿帐号
        /// </summary>
        [Property]
        public string OffshoreAccount { get; set; }

        /// <summary>
        /// 矿池地址
        /// </summary>
        [Property]
        public string Address { get; set; }

        /// <summary>
        /// 矿池主页
        /// </summary>
        [Property]
        public string HomePage { get; set; }

        /// <summary>
        /// 矿池速度
        /// </summary>
        [Property]
        public decimal Speed { get; set; }

        /// <summary>
        /// 税率（百分比）
        /// </summary>
        [Property]
        public int TaxRate { get; set; }

        /// <summary>
        /// 支付总额
        /// </summary>
        [Property]
        public double TotalPay { get; set; }

        /// <summary>
        /// 矿场余额
        /// </summary>
        [Property]
        public double Balance { get; set; }

        /// <summary>
        /// 矿主
        /// </summary>
        [BelongsTo("AccountId", Type = typeof(Mine), Cascade = CascadeEnum.None)]
        public Account Account { get; set; }

        /// <summary>
        /// 系统默认的矿池账户
        /// </summary>
        [Property]
        public string DefaultAccount { get; set; }

        /// <summary>
        /// 系统默认的矿池密码
        /// </summary>
        [Property]
        public string DefaultPassword { get; set; }

        //runtime properties
        [Property]
        public DateTime CheckTime { get; set; }

        [Property]
        public DateTime NextCheckTime { get; set; }

        [Property]
        public string LastException { get; set; }

        [Property]
        public bool IsOnline { get; set; }
    }
}