#region Copyright

//==============================================================================
//  File Name   :   BitCoinService.cs
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
//      This file is a part of api project.
//   </summary>
//   <author name="Zhang Ling" mail="tox@e2.org.cn"/>
//   <seealso ref=""/>
// </fileinformation>
//
// <history>
//   <record date="2011-06-15 00:20:44" author="Zhang Ling" revision="1.00.000">
//		First version of BitCoinService.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Service
{
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using App.Web.Business.Channel;

    /// <summary>
    /// Address: http://grid.e2.to/btc
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class BitCoinService
    {
        [WebGet(UriTemplate = "")]
        public string GetRequest()
        {
            return "nothing but good";
        }

        [WebInvoke(UriTemplate = "", Method = "POST")]
        public string Main(Message cmd)
        {
            if(cmd.Arguments != null && cmd.Arguments.Count > 0)
            {
                

            }
            return "GOD is love you";
        }
    }
}