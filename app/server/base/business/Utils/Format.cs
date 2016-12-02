#region Copyright

//==============================================================================
//  File Name   :   Format.cs
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
//   <record date="2011-06-25 20:04:17" author="Zhang Ling" revision="1.00.000">
//		First version of Format.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Utils
{
    using System;

    /// <summary>
    ///  Summary of Format.
    /// </summary>
    public class Format
    {
        public static string AsBitCoin(double value)
        {
            return string.Format("{0} BTC", value);
        }

        public static string AsCurrency(double value)
        {
            return string.Format("{0} RMB", value);
        }

        public static string AsSpeed(decimal value)
        {
            const int scale = 1024;

            var orders = new[] { "Phash/s", "Thash/s", "Ghash/s", "Mhash/s", "Khash/s", "hash/s" };
            var max = (decimal)Math.Pow(scale, orders.Length - 1);

            foreach (var order in orders)
            {
                if (value > max)
                    return string.Format("{0:##.##} {1}", Decimal.Divide(value, max), order);

                max /= scale;
            }
            return "0 hash/s";
        }

        public static string AsSpeedIcon(decimal value)
        {
            var orders = new[] { "online-5", "online-4", "online-3", "online-2", "online-1", "offline", "error" };
            var max = new[] { 1073741824, 629145600, 209715200, 52428800, 1048576, 1, 0 };
            var idx = 0;

            while (idx < max.Length)
            {
                var baseVal = max[idx];
                if (value > baseVal)
                {
                    return string.Format("{0}", orders[idx]);
                }
                idx++;
            }
            return "offline";
        }

        public static int AsSpeedLevel(long value)
        {
            const int scale = 1024;

            var orders = new[] { 6, 5, 4, 3, 2, 1 };
            var max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (var order in orders)
            {
                if (value > max)
                    return order;

                max /= scale;
            }
            return 0;
        }

        public static string AsBytes(long value)
        {
            const int scale = 1024;

            var orders = new[] { "PB", "TB", "GB", "MB", "KB", "Bytes" };
            var max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (var order in orders)
            {
                if (value > max)
                    return string.Format("{0:##.##} {1}", Decimal.Divide(value, max), order);

                max /= scale;
            }
            return "0 Bytes";
        }

        public static string AsAge(DateTime birthday)
        {
            var age = DateTime.Now - birthday;
            var minutes = (Decimal)age.TotalMinutes;

            var orders = new[] { "年", "月", "周", "天", "小时", "分钟" };
            var max = new[] { 525600, 43200, 10080, 1440, 60, 1 };
            var idx = 0;

            while (idx < max.Length)
            {
                var baseVal = max[idx];
                if (minutes > baseVal)
                {
                    var result = Decimal.Divide(minutes, baseVal);
                    return string.Format("{0:#.#} {1}", result, orders[idx]);
                }
                idx++;
            }
            return "小于1分钟";
        }

        public static string AsPercent(int current, int total)
        {
            return string.Format("{0:P}", (current / (double)(total)));
        }

        public static string AsCurrentAndPercent(int current, int total)
        {
            return string.Format("{0} ({1:P})", current, (current / (double)(total)));
        }
    }
}