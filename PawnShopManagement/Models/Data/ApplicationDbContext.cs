using Microsoft.EntityFrameworkCore;
using PawnShopManagement.Models;
namespace PawnShopManagement.Data;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options) {
    public DbSet<PawnContract> PawnContracts => Set<PawnContract>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    protected override void OnModelCreating(ModelBuilder b) {
        b.Entity<PawnContract>().ToTable("PawnContracts", t => {
            t.HasCheckConstraint("CK_PawnContracts_Loan", "CAST(LoanAmount AS REAL) BETWEEN 500000 AND 1000000000");
            t.HasCheckConstraint("CK_PawnContracts_Rate", "InterestRate BETWEEN 0 AND 100");
            t.HasCheckConstraint("CK_PawnContracts_Status", "Status IN ('DangCam','DaChuoc','DaThanhLy')");
            t.HasCheckConstraint("CK_PawnContracts_TermDays", "TermDays IS NULL OR TermDays BETWEEN 1 AND 3650");
        });
        b.Entity<AppUser>().ToTable("AppUsers",t=>t.HasCheckConstraint("CK_AppUsers_Role","Role IN ('Admin','Staff')"));
        // Legacy contracts may have no agreed term recorded; MVC still requires
        // a term on new/edited contracts through the validation annotation.
        b.Entity<PawnContract>().Property(x=>x.TermDays).IsRequired(false);
        b.Entity<PawnContract>().Property(x=>x.LoanAmount).HasPrecision(18,2);
        b.Entity<PawnContract>().HasOne(x=>x.Customer).WithMany().HasForeignKey(x=>x.CustomerID).OnDelete(DeleteBehavior.Restrict);
        b.Entity<AppUser>().HasIndex(x=>x.UserName).IsUnique();
        b.Entity<Customer>().HasIndex(x=>x.IdentityCard).IsUnique();
    }
}
