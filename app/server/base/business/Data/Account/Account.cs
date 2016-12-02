#region Copyright

//==============================================================================
//  File Name   :   Account.cs
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
//   <record date="2011-06-12 22:05:32" author="Zhang Ling" revision="1.00.000">
//		First version of Account.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Data
{
    using System.Collections.Generic;
    using Castle.ActiveRecord;
    using Castle.Components.Validator;
    using NHibernate.Criterion;

    /// <summary>
    ///  Summary of Account.
    /// </summary>
    [ActiveRecord("Account")]
    public class Account : TrackableNumberIndexedRecord<Account>
    {
        /// <summary>
        /// 使用登录名检索一条用户记录（登录名包括：用户名、电子邮件地址或者手机号码）
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Account FindBy(int userId)
        {
            //登录名不区分大小写
            return FindOne(Restrictions.Eq("UserId", userId));
        }

        /// <summary>
        /// 用户登录email
        /// </summary>
        [ValidateIsUnique]
        [Property]
        public int UserId { get; set; }

        /// <summary>
        /// 比特币账户
        /// </summary>
        [Property]
        public string BitCoinAccount { get; set; }

        /// <summary>
        /// 比特币余额
        /// </summary>
        [Property]
        public float BitCoin { get; set; }

        /// <summary>
        /// 支付宝帐户
        /// </summary>
        [Property]
        public string CreditAccount { get; set; }

        /// <summary>
        /// 人民币余额
        /// </summary>
        [Property]
        public float Credit { get; set; }

        /// <summary>
        /// 获取所有与这个 Account 关联的 Bot 对象。
        /// </summary>
        /// <remarks>
        /// Account 与 Bot 未非级联关系，Account 删除仅会在 Bot 表中将相关的 AccountId 字段置空。
        /// </remarks>
        [HasMany(typeof(Bot), ColumnKey = "AccountId", Cascade = ManyRelationCascadeEnum.SaveUpdate)]
        public IList<Bot> Bots { get; set; }

        /// <summary>
        /// 获取所有与这个 Account 关联的 Mine 对象。
        /// </summary>
        /// <remarks>
        /// Account 与 Mine 未非级联关系，Account 删除仅会在 Mine 表中将相关的 AccountId 字段置空。
        /// </remarks>
        [HasMany(typeof(Mine), ColumnKey = "AccountId", Cascade = ManyRelationCascadeEnum.SaveUpdate)]
        public IList<Mine> Mines { get; set; }
    }
}