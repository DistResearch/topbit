#region Copyright

//==============================================================================
//  File Name   :   Share.cs
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
//   <record date="2011-06-12 09:18:54" author="Zhang Ling" revision="1.00.000">
//		First version of Share.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Data
{
    using System;
    using Castle.ActiveRecord;

    /// <summary>
    ///  Summary of Share.
    /// </summary>
    [ActiveRecord("Share")]
    public class Share : NumberIndexedCreationRecord<Share>
    {
        [Property]
        public int BlockId { get; set; }

        [Property]
        public int AccountId { get; set; }

        [Property]
        public int MineId { get; set; }

        [Property]
        public int BotId { get; set; }

        [Property(Length = 32)]
        public string IPAddress { get; set; }

        [Property(Length = 160)]
        public string Solution { get; set; }

        [Property(Length = 64)]
        public string Hash { get; set; }

        [Property]
        public int Result { get; set; }

        [Property]
        public bool LocalResult { get; set; }

        [Property]
        public bool NetworkResult { get; set; }
    }
}