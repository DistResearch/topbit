#region Copyright

//==============================================================================
//  File Name   :   Block.cs
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
//   <record date="2011-06-15 16:22:20" author="Zhang Ling" revision="1.00.000">
//		First version of Block.
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
    ///  Summary of Block.
    /// </summary>
    [ActiveRecord("Block")]
    public class Block : NumberIndexedRecord<Block>
    {
        #region CRUD Opreations: FindBy(hash)

        /// <summary>
        /// 根据hash检索一条记录
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static Block FindBy(string hash)
        {
            return FindOne(Restrictions.Eq("Hash", hash));
        }
        #endregion

        [Property]
        public int Sequence { get; set; }

        [Property(Unique = true)]
        public string Hash { get; set; }

        [Property]
        public float Target { get; set; }

        [Property]
        public float Difficulty { get; set; }

        [Property(NotNull = false)]
        public bool Solved { get; set; }

        [Property(NotNull = false)]
        public DateTime SolvedAt { get; set; }

        [Property(NotNull = false)]
        public DateTime CreateAt { get; set; }

        [Property(NotNull = false)]
        public string Winner { get; set; }

        [Property(NotNull = false)]
        public float Bouns { get; set; }

        [Property(NotNull = false)]
        public string Solution { get; set; }
    }
}