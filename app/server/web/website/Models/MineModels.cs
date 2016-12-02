#region Copyright

//==============================================================================
//  File Name   :   MineModels.cs
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
//   <record date="2011-06-21 22:26:26" author="Zhang Ling" revision="1.00.000">
//		First version of MineModels.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using App.Web.Business.Data;
    using App.Web.Business.Utils;
    using WebMatrix.WebData;

    /// <summary>
    ///  Summary of MineModels.
    /// </summary>
    public class MineModel
    {
        public MineModel()
        {
        }

        public MineModel(Mine mine, MineProfile profile = null)
        {
            this.Id = profile == null ? mine.Id : profile.Id;
            this.Address = mine.Address;
            this.HomePage = mine.HomePage;
            //this.Certificated = mine.Certificated;
            this.DisplayName = mine.DisplayName;
            this.Name = mine.Name; 
            this.Icon = mine.Icon;
            this.IconText = mine.IconName;
            this.Description = mine.Description ?? "��ʱûɶ��˵��";
            //this.Offshore = mine.Offshore;
            //this.Private = mine.Private;
            this.Speed = mine.Speed;
            this.Tax = Format.AsPercent(mine.TaxRate, 100);
            this.Status = mine.IsOnline ? "����" : "����"; //����
            this.UpdatedAt = mine.CheckTime;

            this.Ownership = mine.Account != null && mine.Account.UserId == WebSecurity.CurrentUserId;

            this.Action = "Profile";//profile == null ? "Profile" : "Edit";
            this.ActionText = profile == null ? "�Զ���" : "�޸�";
            
        }

        [Display(Name = "��ر��")]
        public int Id { get; set; }

        [Display(Name = "���״̬")]
        public string Status { get; set; }

        [Display(Name = "��ش���")]
        public string Name { get; set; }

        [Display(Name = "�������")]
        public string DisplayName { get; set; }

        [Display(Name = "���ͼ��")]
        public string Icon { get; set; }

        [Display(Name = "���ͼ������")]
        public string IconText { get; set; }

        [Display(Name = "���˵��")]
        public string Description { get; set; }

        [Display(Name = "��ص�ַ")]
        public string Address { get; set; }

        [Display(Name = "�����ҳ")]
        public string HomePage { get; set; }

        [Display(Name = "����ٶ�")]
        public decimal Speed { get; set; }

        [Display(Name = "˰��")]
        public string Tax { get; set; }

        [Display(Name = "��عɷ�")]
        public int Shares { get; set; }

        [Display(Name = "����")]
        public bool Ownership { get; set; }

        [Display(Name = "���ӵ��ھ����")]
        public int Workers { get; set; }

        [Display(Name = "�ھ���б�")]
        public IList<BotModel> Bots { get; set; }

        [Display(Name = "����ʱ��")]
        public DateTime UpdatedAt { get; set; }

        [Required]
        [Display(Name = "��ѡ����")]
        public string Action { get; set; }

        [Required]
        [Display(Name = "��ѡ����")]
        public string ActionText { get; set; }
    }
}