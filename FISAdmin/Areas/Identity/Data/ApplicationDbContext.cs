using FISAdmin.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FISAdmin.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        //throw new NotImplementedException();
        builder.Property(u => u.Nopeng).HasMaxLength(12);
        builder.Property(u => u.Nama).HasMaxLength(250);
        builder.Property(u => u.ActiveStatus).HasDefaultValue(1);
        builder.Property(u => u.Email);
        builder.Property(u => u.CreatedBy);
        builder.Property(u => u.CreatedDate).HasDefaultValue(DateTime.Now);
        builder.Property(u => u.LastModifiedBy);
        builder.Property(u => u.LastModifiedDate);

    }
}