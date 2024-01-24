using BundleCounter.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace BundleCounter;

public partial class AppDBContext : DbContext
{
    public DbSet<Bundle> Bundles { get; set; } = default!;
    public AppDBContext()
    {
    }

    public AppDBContext(DbContextOptions<AppDBContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Bundle>().HasOne(bd => bd.Parent).WithMany(bd => bd.Parts).HasForeignKey(bd => bd.ParentId).OnDelete(DeleteBehavior.SetNull);

        base.OnModelCreating(builder);
    }

}