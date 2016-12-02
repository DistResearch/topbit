#region Copyright

//==============================================================================
//  File Name   :   MineProfile.cs
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
//   <record date="2011-06-28 21:33:09" author="Zhang Ling" revision="1.00.000">
//		First version of MineProfile.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Data
{
    using System;
    using Castle.ActiveRecord;
    using NHibernate.Criterion;

    /// <summary>
    ///  Summary of MineProfile.
    /// </summary>
    [ActiveRecord("MineProfile")]
    public class MineProfile : TrackableNumberIndexedRecord<MineProfile>
    {
        #region CRUD Opreations: FindBy(login), FindBy(login, password)

        public static MineProfile[] FindByAccount(int accountId)
        {
            return FindAll(Restrictions.Eq("AccountId", accountId), Restrictions.IsNotNull("MineId"), Restrictions.Or(Restrictions.IsNull("BotId"), Restrictions.Eq("BotId", 0)));
        }

        public static MineProfile FindByAccountMine(int accountId, int mineId)
        {
            return FindFirst(Restrictions.Eq("AccountId", accountId), Restrictions.Eq("MineId", mineId), Restrictions.Or(Restrictions.IsNull("BotId"), Restrictions.Eq("BotId", 0)));
        }

        public static MineProfile FindByBot(int botId)
        {
            return FindFirst(Restrictions.Eq("BotId", botId));
        }

        #endregion

        [Property]
        public int AccountId { get; set; }

        [Property]
        public int BotId { get; set; }

        [Property]
        public int MineId { get; set; }

        [Property]
        public string CustomAccount { get; set; }

        [Property]
        public string CustomPassword { get; set; }
    }
}