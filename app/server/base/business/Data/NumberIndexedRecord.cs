#region Copyright

//==============================================================================
//  File Name   :   NumberIndexedRecord.cs
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
//   <record date="2011-06-12 22:10:04" author="Zhang Ling" revision="1.00.000">
//		First version of NumberIndexedRecord.
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
    ///  Summary of NumberIndexedRecord.
    /// </summary>
    public abstract class NumberIndexedRecord<T> : ActiveRecordLinqBase<T>
    {
        [PrimaryKey(PrimaryKeyType.Increment, "Id")]
        public int Id { get; set; }
    }

    public abstract class NumberIndexedCreationRecord<T> : NumberIndexedRecord<T>
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

        #endregion
    }

    public abstract class TrackableNumberIndexedRecord<T> : NumberIndexedRecord<T>, ITrackable
    {
        public override void Save()
        {
            try
            {
                this.UpdatedDate = DateTime.Now;

                if (string.IsNullOrWhiteSpace(this.CreationLog))
                {
                    this.CreationLog = Guid.NewGuid().ToString();
                    this.CreationDate = DateTime.Now;
                    if (HttpContext.Current != null)
                    {
                        this.CreationLog = HttpContext.Current.Request.UserAgent;
                    }
                }

                if (HttpContext.Current.Request != null)
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