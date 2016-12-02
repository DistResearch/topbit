#region Copyright

//==============================================================================
//  File Name   :   Users.cs
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
//   <record date="2011-06-22 00:55:17" author="Zhang Ling" revision="1.00.000">
//		First version of Users.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Data
{
    using System;
    using Castle.ActiveRecord;
    using Castle.Components.Validator;

    /// <summary>
    ///  Summary of Users.
    /// </summary>
    [ActiveRecord("Users")]
    public class Users : NumberIndexedRecord<Users>
    {
        [ValidateEmail]
        [Property(Length = 56, NotNull = true)]
        public string Email { get; set; }
    }
}