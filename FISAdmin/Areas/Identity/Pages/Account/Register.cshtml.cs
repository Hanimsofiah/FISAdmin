// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using FISAdmin.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using FISAdmin.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;

namespace FISAdmin.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration config;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            this.config = configuration;
            _roleManager = roleManager;

            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        /* dropdown user list & dropdown roles list */

        [NotMapped]
        public List<SelectListItem> Roles { get; set; }

        [NotMapped]
        public List<SelectListItem> Users { get; set; }

    

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /* dropdown user list & dropdown roles list */

            [NotMapped]
            public List<SelectListItem> Roles { get; set; }

            [NotMapped]
            public List<SelectListItem> Users { get; set; }

            /* dbfis - dbo.AspNetUsers */
          
            [Column("Nopeng")]
            [DisplayName("IC")]
            [DataType(DataType.Text)]
            [Required(ErrorMessage = "IC is required")]
            [StringLength(12, MinimumLength = 12, ErrorMessage = "IC must be 12 characters")]
            public string Nopeng { get; set; } = "";

            [Column("Nama")]
            [DisplayName("Nama")]
            [DataType(DataType.Text)]
            [Required(ErrorMessage = "User is required")]
            public string Nama { get; set; } = "";


            [Column("ActiveStatus")]
            [DisplayName("Active Status")]
            [DataType(DataType.Text)]
            [Required(ErrorMessage = "Active Status is required")]
            public int ActiveStatus { get; set; } = 1;

            [Column("CreatedBy")]
            [DisplayName("Created By")]
            [DataType(DataType.Text)]
            public string CreatedBy { get; set; } = "";

            [Column("CreatedDate")]
            [DisplayName("Created Date")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            [DataType(DataType.DateTime)]
            public DateTime CreatedDate { get; set; } = DateTime.Now;

            [Column("LastModifiedBy")]
            [DisplayName("Last Modified By")]
            [DataType(DataType.Text)]
            public string? LastModifiedBy { get; set; }

            [Column("LastModifiedDate")]
            [DisplayName("Last Modified Date")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            [DataType(DataType.DateTime)]
            public DateTime? LastModifiedDate { get; set; } = DateTime.Now;

            /* dbfis - dbo.AspNetUsersRoles*/

            [NotMapped]
            [DisplayName("Role")]
            [DataType(DataType.Text)]
            [Required(ErrorMessage = "Role is required")]
            public string RoleId { get; set; }


            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        

        public async Task OnGetAsync(string returnUrl = null)
        {
            
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            var RId = "";
            var RName = "";

            /* get default RoleId and RoleName */
            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                string sql = "SELECT Id,Name FROM AspNetRoles WHERE Name=@Name";
                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = "Admin";

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        RId = dr.GetString(0);
                        RName = dr.GetString(1);

                    }
                }
                con.Close();
            }

            /* dropdown list for roles */
            IEnumerable<IdentityRole> roles = _roleManager.Roles;
           
            ViewData["RoleId"] = new SelectList(roles.ToList(), "Id", "Name", new
            {
                Selected = true,
                Text = RName,
                Value = RId
            });

           
            /* dropdown list for users */
            /*List<SelectListItem> UserId = PopulateUsers();
            ViewData["UserId"] = new SelectList(UserId, "Value" , "Text");*/


            Users = PopulateUsers();

            ViewData["type"] = "Access Users";
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Users/Index");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
     
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                user.Nopeng = Input.Nopeng;
                user.Nama = Input.Nama;
                user.ActiveStatus = Input.ActiveStatus;
                user.RoleId = Input.RoleId;
                user.Email = Input.Email;
                user.UserName = Input.Email;
                user.CreatedBy = Input.CreatedBy;
                user.CreatedDate = DateTime.Now;
                user.LastModifiedBy = Input.LastModifiedBy;
                user.LastModifiedDate = DateTime.Now;


                await _userStore.SetUserNameAsync(user, user.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, user.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                var UserId = ""; 
                var Ic = Input.Nopeng;
                var RId = Input.RoleId;

                ViewData["type"] = "Access Users";

                if (result.Succeeded)
                {
                    /* Assign RoleId from UserId */
                    using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
                    {
                        string sql = "SELECT Id FROM AspNetUsers WHERE Nopeng=@Nopeng";
                        SqlCommand cmd = new SqlCommand(sql, con);

                        cmd.Parameters.Add("@Nopeng", System.Data.SqlDbType.NVarChar).Value = Ic;

                        con.Open();

                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                UserId = dr.GetString(0);

                            }
                        }
                        con.Close();
                    }

                    using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
                    {
                        string sql = "INSERT INTO AspNetUserRoles (UserId,RoleId) VALUES (@UserId,@RoleId)";
                        SqlCommand cmd = new SqlCommand(sql, con);

                        cmd.Parameters.Add("@UserId", System.Data.SqlDbType.NVarChar).Value = UserId;
                        cmd.Parameters.Add("@RoleId", System.Data.SqlDbType.NVarChar).Value = RId;

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }

                    ViewData["type"] = "Access Users";

                    return LocalRedirect(returnUrl);

                    /* Below is default code */

                   /* _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = user.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }*/
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            TempData["error"] = "Please Select Dropdown Field";
            ModelState.AddModelError("", "");
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        private List<SelectListItem> PopulateUsers()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            
            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                items.Add(new SelectListItem
                {
                    Disabled = true,
                    Text = "Please Select New User",
                    Value = ""
                });

                string sql = "SELECT nama,nopeng,emailrasmi FROM v_profailstaf WHERE nopeng NOT IN (SELECT Nopeng FROM AspNetUsers)";
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();

                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = sdr["nama"].ToString() + " (" + sdr["nopeng"].ToString() + ") - " + sdr["emailrasmi"].ToString(),
                            Value = sdr["nopeng"].ToString() + "?" + sdr["nama"].ToString() + "?" + sdr["emailrasmi"].ToString()
                        });
                    }
                }
                con.Close();

            }

            return items;
        }
    }
}
