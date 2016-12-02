#region Copyright

//==============================================================================
//  File Name   :   Log.cs
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
//   <record date="2011-06-12 09:19:04" author="Zhang Ling" revision="1.00.000">
//		First version of Log.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Data
{
    using System;
    using Castle.ActiveRecord;

    /// <summary>
    ///  Summary of Log.
    /// </summary>
    [ActiveRecord]
    public class Log : ActiveRecordBase<Log>
    {
        [PrimaryKey(PrimaryKeyType.Identity, "Id")]
        public int Id { get; set; }

        [Property(NotNull = true)]
        public DateTime Date { get; set; }

        [Property(SqlType = "varchar (50)", NotNull = true)]
        public string Thread { get; set; }

        [Property(SqlType = "varchar (50)", NotNull = true)]
        public string Level { get; set; }

        [Property(SqlType = "varchar (255)", NotNull = true)]
        public string Logger { get; set; }

        [Property(SqlType = "varchar (4000)", NotNull = true)]
        public string Message { get; set; }

        [Property(SqlType = "varchar (2000)", NotNull = true)]
        public string Exception { get; set; }
    }
}