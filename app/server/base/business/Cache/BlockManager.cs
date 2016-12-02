#region Copyright

//==============================================================================
//  File Name   :   BlockManager.cs
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
//   <record date="2011-06-26 07:43:55" author="Zhang Ling" revision="1.00.000">
//		First version of BlockManager.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Cache
{
    using System;
    using App.Web.Business.Data;
    using log4net;

    /// <summary>
    ///  Summary of BlockManager.
    /// </summary>
    public class BlockManager
    {
        //static
        public static readonly BlockManager Instance = new BlockManager();
        private static readonly ILog Log = LogManager.GetLogger(typeof(BlockManager));

        public bool CreateOrRetrieve(string blockhash, out Block block)
        {
            try
            {
                block = Block.FindBy(blockhash);
                if (block == null)
                {
                    lock (Instance)
                    {
                         block = Block.FindBy(blockhash);
                        if (block == null)
                        {
                            var tmp = new Block
                            {
                                Hash = blockhash,
                                SolvedAt = new DateTime(2000, 1, 1),
                                CreateAt = DateTime.Now
                            };
                            tmp.Save();
                            block = tmp;
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Log.ErrorFormat("Error when CreateOrRetrieve the block with hash:{0} and password:{1}", blockhash);
                Log.Error("Exception in BlockManager.CreateOrRetrieve(blockhash)", e);
                block = null;
                return false;
            }
        }
    }
}