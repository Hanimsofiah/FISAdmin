using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISAdmin.Models
{
    /* dbfis - dbo.billing_category_bill, dbo.billing_customer_category, dbo.billing_doc_type, dbo.billing_fis_destination, dbo.billing_priority, dbo.billing_source_bill & dbo.billing_s tatus_bill */
    /* dbfis -  dbo.payment_doc_type, dbo.payment_source, dbo.payment_status & dbo.payment_type */
    public class BillingPaymentModel
    {
        [Key]
        [DisplayName("Id")]
        [Column("id")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Id is required")]
        [StringLength(3, MinimumLength = 1, ErrorMessage = "Id cannot be more than 3 characters.")]
        public string? Id { get; set; }

        [DisplayName("Description")]
        [Column("description")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Description is required")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Description cannot be longer than 30 characters")]
        public string? Description { get; set; }

        [DisplayName("Created By")]
        [Column("CreatedBy")]
        [DataType(DataType.Text)]
        public string? CreatedBy { get; set; }

        [DisplayName("Created On")]
        [Column("CreatedDate")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedDate { get; set; }

        [DisplayName("Last Modified By")]
        [Column("LastModifiedBy")]
        public string? LastModifiedBy { get; set; }

        [DisplayName("Last Modified On")]
        [Column("LastModifiedDate")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LastModifiedDate { get; set; }

        /* unique id for cshtml */
        public string bp_no { get; set; } = "bp";

    }
}
