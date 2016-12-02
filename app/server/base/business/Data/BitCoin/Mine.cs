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
    /// ��� Score ���㷽��
    /// 1. ���� +100��
    /// </remarks>
    [ActiveRecord("Mine")]
    public class Mine : TrackableNumberIndexedRecord<Mine>
    {
        public static Mine Create(string address, string username, string password)
        {
            return new Mine() {Address = address, DefaultAccount = username, DefaultPassword = password};
        }

        /// <summary>
        /// ��ش���
        /// </summary>
        [Property]
        public string Name { get; set; }

        /// <summary>
        /// �����ʾ����
        /// </summary>
        [Property]
        public string DisplayName { get; set; }

        /// <summary>
        /// ���ͼƬ
        /// </summary>
        [Property]
        public string Icon { get; set; }

        /// <summary>
        /// ���ͼƬ����
        /// </summary>
        [Property]
        public string IconName { get; set; }

        /// <summary>
        /// ���˵��
        /// </summary>
        [Property]
        public string Description { get; set; }

        /// <summary>
        /// ��֤�Ŀ�
        /// </summary>
        [Property]
        public bool Certificated { get; set; }

        /// <summary>
        /// ר����
        /// </summary>
        [Property]
        public bool Private { get; set; }

        /// <summary>
        /// Զ�̿�
        /// </summary>
        [Property]
        public bool Offshore { get; set; }

        /// <summary>
        /// Զ�̿��ʺ�
        /// </summary>
        [Property]
        public string OffshoreAccount { get; set; }

        /// <summary>
        /// ��ص�ַ
        /// </summary>
        [Property]
        public string Address { get; set; }

        /// <summary>
        /// �����ҳ
        /// </summary>
        [Property]
        public string HomePage { get; set; }

        /// <summary>
        /// ����ٶ�
        /// </summary>
        [Property]
        public decimal Speed { get; set; }

        /// <summary>
        /// ˰�ʣ��ٷֱȣ�
        /// </summary>
        [Property]
        public int TaxRate { get; set; }

        /// <summary>
        /// ֧���ܶ�
        /// </summary>
        [Property]
        public double TotalPay { get; set; }

        /// <summary>
        /// �����
        /// </summary>
        [Property]
        public double Balance { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        [BelongsTo("AccountId", Type = typeof(Mine), Cascade = CascadeEnum.None)]
        public Account Account { get; set; }

        /// <summary>
        /// ϵͳĬ�ϵĿ���˻�
        /// </summary>
        [Property]
        public string DefaultAccount { get; set; }

        /// <summary>
        /// ϵͳĬ�ϵĿ������
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