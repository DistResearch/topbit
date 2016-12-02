#region Copyright

//==============================================================================
//  File Name   :   ProfileModels.cs
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
//   <record date="2011-06-30 06:28:28" author="Zhang Ling" revision="1.00.000">
//		First version of ProfileModels.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using App.Web.Business.Data;
    using App.Web.Business.Utils;

    /// <summary>
    ///  Summary of ProfileModels.
    /// </summary>
    public class CreateEditProfileModel
    {
        public static CreateEditProfileModel Build(string mineOrProfileId)
        {
            var mineId = int.Parse(mineOrProfileId);
            var mp = MineProfile.TryFind(mineId);

            if (mp != null)
            {
                mineId = mp.MineId;
            }

            var mine = App.Web.Business.Data.Mine.Find(mineId);
            var model = new CreateEditProfileModel
            {
                Id = mp != null ? mp.Id : 0,
                Mine = mine.DisplayName,
                MineId = mine.Id,
                MineIcon = mine.Icon,
                MineIconText = mine.IconName,
                MineDescription = mine.Description,
                MineTaxRate = Format.AsPercent(mine.TaxRate, 100),
                CustomAccount = mp != null ? mp.CustomAccount : string.Empty,
                CustomPassword = mp != null ? mp.CustomPassword : string.Empty
            };
            return model;
        }

        [Display(Name = "ÅäÖÃÐòºÅ")]
        public int Id { get; set; }

        [Display(Name = "¿ó³ØÐòºÅ")]
        public int MineId { get; set; }

        [Display(Name = "¿ó³ØÃû³Æ")]
        public string Mine { get; set; }

        [Display(Name = "¿ó³ØÍ¼±ê")]
        public string MineIcon { get; set; }

        [Display(Name = "¿ó³ØÍ¼±ê")]
        public string MineIconText { get; set; }

        [Display(Name = "¿ó³Ø½éÉÜ")]
        public string MineDescription { get; set; }

        [Display(Name = "¿ó³ØË°ÂÊ")]
        public string MineTaxRate { get; set; }

        [Display(Name = "ÍÚ¾ò»úÓÃ»§Ãû")]
        public string CustomAccount { get; set; }

        [Display(Name = "ÍÚ¾ò»úÃÜÂë")]
        [DataType(DataType.Password)]
        public string CustomPassword { get; set; }
    }
}