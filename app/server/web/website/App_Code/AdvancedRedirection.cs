#region Copyright

//==============================================================================
//  File Name   :   AdvancedRedirection.cs
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
//      This file is a part of website project.
//   </summary>
//   <author name="Zhang Ling" mail="tox@e2.org.cn"/>
//   <seealso ref=""/>
// </fileinformation>
//
// <history>
//   <record date="2011-06-30 07:23:19" author="Zhang Ling" revision="1.00.000">
//		First version of AdvancedRedirection.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Helpers
{
    using System;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RedirectToRouteResultEx : RedirectToRouteResult
    {
        public RedirectToRouteResultEx(RouteValueDictionary values)
            : base(values)
        {
        }

        public RedirectToRouteResultEx(string routeName, RouteValueDictionary values)
            : base(routeName, values)
        {
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var destination = new StringBuilder();

            var helper = new UrlHelper(context.RequestContext);
            destination.Append(helper.RouteUrl(RouteName, RouteValues));

            //Add href fragment if set
            if (!string.IsNullOrEmpty(Fragment))
            {
                destination.AppendFormat("#{0}", Fragment);
            }

            context.HttpContext.Response.Redirect(destination.ToString(), false);
        }

        public string Fragment { get; set; }
    }

    public static class RedirectToRouteResultExtensions
    {
        public static RedirectToRouteResultEx AddFragment(this RedirectToRouteResult result, string fragment)
        {
            return new RedirectToRouteResultEx(result.RouteName, result.RouteValues)
            {
                Fragment = fragment
            };
        }
    }
}