#region Copyright

//==============================================================================
//  File Name   :   UnixTime.cs
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
//   <record date="2011-06-19 21:47:49" author="Zhang Ling" revision="1.00.000">
//		First version of UnixTime.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Utils
{
    using System;

    /// <summary>
    ///  Summary of UnixTime.
    /// </summary>
    public class UnixTime
    {
        public static DateTime ConvertFromUnixTimestamp(int seconds)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(seconds);
        }

        public static int ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return (int)Math.Floor(diff.TotalSeconds);
        }
    }
}