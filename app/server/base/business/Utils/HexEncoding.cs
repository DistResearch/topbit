#region Copyright

//==============================================================================
//  File Name   :   HexEncoding.cs
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
//   <record date="2011-06-19 17:24:06" author="Zhang Ling" revision="1.00.000">
//		First version of HexEncoding.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Utils
{
    using System;
    using log4net;

    /// <summary>
    ///  Summary of HexEncoding.
    /// </summary>
    public class HexEncoding
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof (HexEncoding));

        public static byte[] Decode(string hexString)
        {
            //check for null
            if (hexString == null) return null;
            //get length
            int len = hexString.Length;
            if (len % 2 == 1) return null;
            int len_half = len / 2;
            //create a byte array
            var bs = new byte[len_half];
            try
            {
                //convert the hexstring to bytes
                for (int i = 0; i != len_half; i++)
                {
                    bs[i] = (byte)Int32.Parse(hexString.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error when decode hex string: " + hexString, ex);
            }
            //return the byte array
            return bs;
        }
    }
}