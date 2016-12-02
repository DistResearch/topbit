#region Copyright

//==============================================================================
//  File Name   :   Roles.cs
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
//   <record date="2011-06-22 01:06:23" author="Zhang Ling" revision="1.00.000">
//		First version of Roles.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Data
{
    using System;
    using Castle.ActiveRecord;
    using Castle.ActiveRecord.Framework;

    /// <summary>
    ///  Summary of Roles.
    /// </summary>
    [ActiveRecord("webpages_Roles")]
    public class Roles : ActiveRecordLinqBase<Roles>
    {
        [ValidateIsUnique]
        [PrimaryKey(PrimaryKeyType.Increment, "RoleId")]
        public int Id { get; set; }

        [Property("RoleName", Length = 256, NotNull = true)]
        public string Name { get; set; }
    }
}