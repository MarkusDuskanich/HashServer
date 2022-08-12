using Microsoft.EntityFrameworkCore;
using Models;

namespace DAL
{
    public partial class HashServerDbContext : DbContext
    {
        public DbSet<Hash> Hashes { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;


        public HashServerDbContext()
        {
        }

        public HashServerDbContext(DbContextOptions<HashServerDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Database=hashserverdb;Username=postgres;Password=postgres");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hash>(entity =>
            {
                entity.ToTable("hashes");

                entity.HasIndex(e => e.HashValue, "hashes_hash_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.HashValue).HasColumnName("hash");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("messages");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Hashid).HasColumnName("hashid");

                entity.HasOne(d => d.Hash)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.Hashid)
                    .HasConstraintName("fk_messages_hashes");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
