using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AADTest1.Models
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [Table("User", Schema = "UserAuthorization")]
    public class User
    {
        #region Model
        /// <summary>
        /// 用户编号
        /// </summary>
        [Required]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column("UserId", Order = 0)]
        public Guid? UserId
        {
            set;
            get;
        }

        /// <summary>
        /// 所属企业编号
        /// </summary>
        [Column("EnterpriseId")]
        public Guid? EnterpriseId
        {
            set;
            get;
        }

        /// <summary>
        /// 用户名（邮箱）
        /// </summary>
        [Required, RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Bad mailbox format.")]
        [Column("UserName")]
        public string UserName
        {
            set;
            get;
        }

        /// <summary>
        /// 账号状态
        /// 禁用 = 0
        /// 启用 = 1
        /// </summary>
        [Column("Enabled")]
        public int Enabled
        {
            set;
            get;
        }

        /// <summary>
        /// 密码
        /// </summary>
        [Column("Password")]
        public string Password
        {
            set;
            get;
        }

        /// <summary>
        /// 别名
        /// </summary>
        [Column("Alias")]
        public string Alias
        {
            set;
            get;
        }

        /// <summary>
        /// 职务
        /// </summary>
        [Column("JobTitle")]
        public string JobTitle
        {
            set;
            get;
        }

        /// <summary>
        /// 部门
        /// </summary>
        [Column("Department")]
        public string Department
        {
            set;
            get;
        }

        /// <summary>
        /// 街道地址
        /// </summary>
        [Column("StreetAddress")]
        public string StreetAddress
        {
            set;
            get;
        }

        /// <summary>
        /// 城市
        /// </summary>
        [Column("City")]
        public string City
        {
            set;
            get;
        }

        /// <summary>
        /// 省/自治区/直辖市
        /// </summary>
        [Column("State")]
        public string State
        {
            set;
            get;
        }

        /// <summary>
        /// 国家或地区
        /// </summary>
        [Column("Country")]
        public string Country
        {
            set;
            get;
        }

        /// <summary>
        /// 办公电话
        /// </summary>
        [Column("TelephoneNumber")]
        public string TelephoneNumber
        {
            set;
            get;
        }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [Column("PostalCode")]
        public string PostalCode
        {
            set;
            get;
        }

        /// <summary>
        /// 头像
        /// </summary>
        [Column("Photo")]
        public string Photo
        {
            set;
            get;
        }

        /// <summary>
        /// 令牌
        /// </summary>
        [Column("Token")]
        public string Token
        {
            set;
            get;
        }

        /// <summary>
        /// 创建人
        /// </summary>
        [Column("CreatedBy")]
        public Guid? CreatedBy
        {
            set;
            get;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("CreatedOn")]
        public DateTime? CreatedOn
        {
            set;
            get;
        }

        /// <summary>
        /// 修改人
        /// </summary>
        [Column("ModifiedBy")]
        public Guid? ModifiedBy
        {
            set;
            get;
        }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Column("ModifiedOn")]
        public DateTime? ModifiedOn
        {
            set;
            get;
        }

        public override string ToString()
        {
            return "UserId=" + UserId + ",EnterpriseId=" + EnterpriseId + ",UserName=" + UserName + ",Enabled=" + Enabled + ",Password=" + Password + ",Alias=" + Alias + ",JobTitle=" + JobTitle + ",Department=" + Department + ",StreetAddress=" + StreetAddress + ",City=" + City + ",State=" + State + ",Country=" + Country + ",TelephoneNumber=" + TelephoneNumber + ",PostalCode=" + PostalCode + ",Photo=" + Photo + ",Token=" + Token + ",CreatedBy=" + CreatedBy + ",CreatedOn=" + CreatedOn + ",ModifiedBy=" + ModifiedBy + ",ModifiedOn=" + ModifiedOn;
        }
        #endregion Model
    }
}
