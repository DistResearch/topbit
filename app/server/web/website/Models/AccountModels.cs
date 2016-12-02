namespace App.Web.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class RegisterModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "登录帐号或邮箱")]
        public string Email { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "登录帐号或邮箱")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "保持登录")]
        public bool RememberMe { get; set; }
    }

    public class ForgotPasswordModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class PasswordResetModel
    {
        [Required]
        [Display(Name = "Password Reset Token")]
        public string ResetToken { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class DashboardModel
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = "支付宝帐号")]
        public string BankAccount { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "人民币")]
        public double Balance { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "比特币帐号")]
        public string BitCoinAccount { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "比特币")]
        public double BitCoin { get; set; }

        [Display(Name = "挖掘速度")]
        public double MiningSpeed { get; set; }

        [Display(Name = "矿池速度")]
        public double PoolSpeed { get; set; }

        [Display(Name = "已连接的矿工数")]
        public int Workers { get; set; }

        [Display(Name = "矿池")]
        public List<MineModel> Mines;
    }
}
