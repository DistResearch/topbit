#region Copyright

//==============================================================================
//  File Name   :   ITrackable.cs
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
//   <record date="2011-06-12 22:10:39" author="Zhang Ling" revision="1.00.000">
//		First version of ITrackable.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Data
{
    using System;

    /// <summary>
    ///  Summary of ITrackable.
    /// </summary>
    public interface ITrackable
    {
        string CreationLog { get; }

        DateTime CreationDate { get; }

        string UpdatedLog { get; }

        DateTime UpdatedDate { get; }
    }
}