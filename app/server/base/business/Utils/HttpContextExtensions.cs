#region Copyright

//==============================================================================
//  File Name   :   HttpContextExtensions.cs
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
//   <record date="2011-06-19 16:00:12" author="Zhang Ling" revision="1.00.000">
//		First version of HttpContextExtensions.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Utils
{
    using System;
    using System.Web;

    /// <summary>
    ///  Summary of HttpContextExtensions.
    /// </summary>
    public static class HttpContextExtensions
    {
        public static string ToServerPath(this string url)
        {
            return HttpContext.Current.Server.MapPath(url);
        }

        public static string[] ToServerPath(this string[] url)
        {
            for (var idx = 0; idx < url.Length; idx++)
            {
                url[idx] = HttpContext.Current.Server.MapPath(url[idx].Trim());
            }

            return url;
        }
    }
}