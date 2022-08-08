using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Models
{
    public partial class hashserverdbContext : DbContext
    {
        public hashserverdbContext()
        {
        }

        public hashserverdbContext(DbContextOptions<hashserverdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Hash> Hashes { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;

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

                entity.HasIndex(e => e.Hash1, "hashes_hash_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Hash1).HasColumnName("hash");
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
