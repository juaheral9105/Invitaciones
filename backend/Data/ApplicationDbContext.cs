using Microsoft.EntityFrameworkCore;
using InvitacionesAPI.Models;

namespace InvitacionesAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Confirmation> Confirmations { get; set; }
        public DbSet<GuestListItem> GuestList { get; set; }
        public DbSet<StoredFile> StoredFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Invitation
            modelBuilder.Entity<Invitation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
                entity.HasIndex(e => e.CreatedAt);
            });

            // Configure Confirmation
            modelBuilder.Entity<Confirmation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

                entity.HasOne(e => e.Invitation)
                    .WithMany(i => i.Confirmations)
                    .HasForeignKey(e => e.InvitationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.InvitationId);
                entity.HasIndex(e => e.CreatedAt);
            });

            // Configure GuestListItem
            modelBuilder.Entity<GuestListItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Invitation)
                    .WithMany()
                    .HasForeignKey(e => e.InvitationId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Phone must be unique per invitation
                entity.HasIndex(e => new { e.InvitationId, e.Phone }).IsUnique();
                entity.HasIndex(e => e.Phone);
                entity.HasIndex(e => e.InvitationId);
            });

            // Configure StoredFile
            modelBuilder.Entity<StoredFile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
                entity.HasIndex(e => e.UploadedAt);
                entity.HasIndex(e => e.FileType);
            });
        }
    }
}
