#region Copyright

//==============================================================================
//  File Name   :   GuidIndexedRecord.cs
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
//      This file is a part of Deepbot project.
//   </summary>
//   <author name="Zhang Ling" mail="tox@e2.org.cn"/>
//   <seealso ref=""/>
// </fileinformation>
//
// <history>
//   <record date="2011-06-12 22:09:17" author="Zhang Ling" revision="1.00.000">
//		First version of GuidIndexedRecord.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Data
{
    using System;
    using System.Web;
    using Castle.ActiveRecord;
    using Castle.ActiveRecord.Framework;

    /// <summary>
    ///  Summary of GuidIndexedRecord.
    /// </summary>
    public class GuidIndexedRecord<T> : ActiveRecordLinqBase<T>
    {
        [PrimaryKey(PrimaryKeyType.GuidComb, "Id")]
        public Guid Id { get; set; }
    }

    public abstract class TrackableGuidIndexedRecord<T> : GuidIndexedRecord<T>, ITrackable
    {
        public override void Save()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.CreationLog))
                {
                    this.CreationLog = Guid.NewGuid().ToString();
                    this.CreationDate = DateTime.Now;
                    if (HttpContext.Current != null)
                    {
                        this.CreationLog = HttpContext.Current.Request.UserAgent;
                    }

                }

                this.UpdatedDate = DateTime.Now;
                if (HttpContext.Current != null)
                {
                    this.UpdatedLog = HttpContext.Current.Request.UserAgent;
                }
            }
            catch (HttpException)
            {
                //access HttpContext.Current.Request in Application_Start and Application_End
            }

            base.Save();
        }

        #region Implementation of ITrackable
        [Property]
        public string CreationLog { get; set; }
        [Property]
        public DateTime CreationDate { get; set; }
        [Property]
        public string UpdatedLog { get; set; }
        [Property]
        public DateTime UpdatedDate { get; set; }

        #endregion
    }
}