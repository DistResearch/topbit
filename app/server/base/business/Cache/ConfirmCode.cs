#region Copyright

//==============================================================================
//  File Name   :   ConfirmCode.cs
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
//   <record date="2011-06-25 16:59:46" author="Zhang Ling" revision="1.00.000">
//		First version of ConfirmCode.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Cache
{
    using System;

    /// <summary>
    ///  Summary of ConfirmCode.
    /// </summary>
    public class ConfirmCode
    {
        private static readonly Random seed = new Random();

        public static string CreateBotConfirmCode()
        {
            return seed.Next(1000, 9999).ToString();
        }
    }
}