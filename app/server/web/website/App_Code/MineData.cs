#region Copyright

//==============================================================================
//  File Name   :   MineData.cs
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
//   <record date="2011-06-24 04:44:19" author="Zhang Ling" revision="1.00.000">
//		First version of MineData.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Helpers
{
    using System;
    using System.Linq;
    using System.Collections.Specialized;
    using System.Net;
    using System.Web;
    using App.Web.Business.Cache;
    using App.Web.Business.Data;
    using App.Web.Business.Utils.CountryLookupProj;
    using NHibernate.Criterion;

    /// <summary>
    ///  Summary of MineData.
    /// </summary>
    public static class MineData
    {
        public static void UpdateDatabase()
        {
            //update 
            var instances = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("DeepBot/Instances");
            
            if (instances == null)
                return;

            var countryLookup = new CountryLookup(HttpContext.Current.Server.MapPath("~/Content/data/GeoIP.dat"));
            //update address from web.config
            foreach (var keyString in instances.Keys.OfType<string>())
            {
                InsertOrUpdateAddress(keyString, instances[keyString], countryLookup);
            }

            //updata mine description
            InsertOrUpdateMineName("TopBit", "TopBit [中文]", "http://topb.it", false);
            InsertOrUpdateMineName("GetCoin", "GetCoin.org [中文]", "http://getcoin.org");
            InsertOrUpdateMineName("slush's pool", "Slush's pool", "http://api.bitcoin.cz");
            InsertOrUpdateMineName("deepbit", "Deepbit", "http://deepbit.net");
            InsertOrUpdateMineName("BTC Guild", "BTC Guild", "http://www.btcguild.com");
            InsertOrUpdateMineName("BitPenny", "BitPenny", "http://bitpenny.com");
            InsertOrUpdateMineName("BitCoin Pool", "BitCoin Pool", "http://bitcoinpool.com");
            InsertOrUpdateMineName("BTC Mine", "BTC Mine", "http://btcmine.com");
            InsertOrUpdateMineName("BitClockers", "BitClockers", "http://bitclockers.com");

            //update mine account or password
            UpdateMineAccount("deepbit", "tox.vip@gmail.com_at4", "xA123456");
            UpdateMineAccount("slush's pool", "e2tox.at1", "xA123456");
            UpdateMineAccount("TopBit", "topbit", "topbit20110629");
        }

        private static void InsertOrUpdateMineName(string name, string displayName, string homePage, bool offshore = true)
        {
            var record = Mine.FindOne(Restrictions.Eq("Name", name)) ?? new Mine { Name = name };
            if (record != null)
            {
                record.DisplayName = displayName;
                record.HomePage = homePage;
                record.Offshore = offshore;
                record.Save();
            }
        }

        private static void UpdateMineAccount(string name, string account, string password)
        {
            var record = Mine.FindOne(Restrictions.Eq("Name", name));
            if(record != null)
            {
                record.DefaultAccount = account;
                record.DefaultPassword = password;
                record.Save();
            }
        }

        private static void InsertOrUpdateAddress(string name, string address, CountryLookup lookup)
        {
            var record = Mine.FindOne(Restrictions.Eq("Name", name)) ?? new Mine { Name = name };
            record.Address = address;
            var uri = new Uri(record.Address);
            var host = Dns.GetHostEntry(uri.DnsSafeHost);
            //update the country flag for the mine pools
            if (host.AddressList.Length > 0)
            {
                var ip = host.AddressList[0];
                var countryCode = lookup.lookupCountryCode(ip);
                record.Icon = "/Content/flags/" + countryCode + ".gif";
                record.IconName = lookup.lookupCountryName(ip);
            }
            record.IsOnline = true;
            record.CheckTime = DateTime.Now;
            record.NextCheckTime = DateTime.Now;
            record.Save();
        }
    }
}