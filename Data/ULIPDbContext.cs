using Microsoft.EntityFrameworkCore;
using ULIP_proj.Models;

namespace ULIP_proj.Data
{
    public class ULIPDbContext : DbContext
    {
        public ULIPDbContext(DbContextOptions<ULIPDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<Fund> Funds { get; set; }
        public DbSet<Premium> Premiums { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Approval> Approvals { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<PolicyDocument> PolicyDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.PanNumber).IsUnique();
            });

            modelBuilder.Entity<Policy>(entity =>
            {
                entity.HasIndex(e => e.PolicyNumber).IsUnique();
                entity.HasOne(p => p.User)
                      .WithMany(u => u.Policies)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Premium>(entity =>
            {
                entity.HasOne(pr => pr.Policy)
                      .WithMany(p => p.Premiums)
                      .HasForeignKey(pr => pr.PolicyId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasOne(t => t.Policy)
                      .WithMany(p => p.Transactions)
                      .HasForeignKey(t => t.PolicyId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(t => t.Fund)
                      .WithMany(f => f.Transactions)
                      .HasForeignKey(t => t.FundId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Approval>(entity =>
            {
                entity.HasOne(a => a.RequestedByUser)
                      .WithMany(u => u.RequestedApprovals)
                      .HasForeignKey(a => a.RequestedBy)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(a => a.ApprovedByUser)
                      .WithMany(u => u.ApprovedApprovals)
                      .HasForeignKey(a => a.ApprovedBy)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasOne(s => s.Agent)
                      .WithMany(u => u.Sales)
                      .HasForeignKey(s => s.AgentId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(s => s.Policy)
                      .WithMany()
                      .HasForeignKey(s => s.PolicyId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<PolicyDocument>(entity =>
            {
                entity.HasOne(pd => pd.Policy)
                      .WithMany(p => p.Documents)
                      .HasForeignKey(pd => pd.PolicyId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(pd => pd.VerifiedByUser)
                      .WithMany()
                      .HasForeignKey(pd => pd.VerifiedBy)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
