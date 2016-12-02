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
        /// ʹ�õ�¼������һ���û���¼����¼���������û����������ʼ���ַ�����ֻ����룩
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Account FindBy(int userId)
        {
            //��¼�������ִ�Сд
            return FindOne(Restrictions.Eq("UserId", userId));
        }

        /// <summary>
        /// �û���¼email
        /// </summary>
        [ValidateIsUnique]
        [Property]
        public int UserId { get; set; }

        /// <summary>
        /// ���ر��˻�
        /// </summary>
        [Property]
        public string BitCoinAccount { get; set; }

        /// <summary>
        /// ���ر����
        /// </summary>
        [Property]
        public float BitCoin { get; set; }

        /// <summary>
        /// ֧�����ʻ�
        /// </summary>
        [Property]
        public string CreditAccount { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        [Property]
        public float Credit { get; set; }

        /// <summary>
        /// ��ȡ��������� Account ������ Bot ����
        /// </summary>
        /// <remarks>
        /// Account �� Bot δ�Ǽ�����ϵ��Account ɾ�������� Bot ���н���ص� AccountId �ֶ��ÿա�
        /// </remarks>
        [HasMany(typeof(Bot), ColumnKey = "AccountId", Cascade = ManyRelationCascadeEnum.SaveUpdate)]
        public IList<Bot> Bots { get; set; }

        /// <summary>
        /// ��ȡ��������� Account ������ Mine ����
        /// </summary>
        /// <remarks>
        /// Account �� Mine δ�Ǽ�����ϵ��Account ɾ�������� Mine ���н���ص� AccountId �ֶ��ÿա�
        /// </remarks>
        [HasMany(typeof(Mine), ColumnKey = "AccountId", Cascade = ManyRelationCascadeEnum.SaveUpdate)]
        public IList<Mine> Mines { get; set; }
    }
}