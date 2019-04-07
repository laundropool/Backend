using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WasherDAL.Models
{
    public partial class WasherDBContext : DbContext
    {
        public WasherDBContext()
        {
        }

        public WasherDBContext(DbContextOptions<WasherDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AcceptedRequest> AcceptedRequest { get; set; }
        public virtual DbSet<LaundryRequest> LaundryRequest { get; set; }
        public virtual DbSet<MatchedRequest> MatchedRequest { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source =(localdb)\\MSSQLLocalDB;Initial Catalog=WasherDB;Integrated Security=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AcceptedRequest>(entity =>
            {
                entity.Property(e => e.AcceptedRequestId).HasColumnName("AcceptedRequestID");

                entity.Property(e => e.OwnerId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.Property(e => e.WasherId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.MatchedRequest)
                    .WithMany(p => p.AcceptedRequest)
                    .HasPrincipalKey(p => new { p.OwnerId, p.WasherId, p.OwnerRequestId, p.WasherRequestId })
                    .HasForeignKey(d => new { d.OwnerId, d.WasherId, d.OwnerRequestId, d.WasherRequestId })
                    .HasConstraintName("fk_RequestsAccepted");
            });

            modelBuilder.Entity<LaundryRequest>(entity =>
            {
                entity.HasKey(e => e.RequestId);

                entity.Property(e => e.RequestId).HasColumnName("RequestID");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.WashingTime).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.LaundryRequest)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__LaundryRe__UserI__276EDEB3");
            });

            modelBuilder.Entity<MatchedRequest>(entity =>
            {
                entity.HasIndex(e => new { e.OwnerId, e.WasherId, e.OwnerRequestId, e.WasherRequestId })
                    .HasName("uq_RequestsMatched")
                    .IsUnique();

                entity.Property(e => e.Distance).HasColumnType("numeric(25, 2)");

                entity.Property(e => e.OwnerId)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.OwnerRequestId).IsRequired();

                entity.Property(e => e.RequestSentBy)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.WasherId)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.WasherRequestId).IsRequired();

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.MatchedRequestOwner)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MatchedRe__Owner__31EC6D26");

                entity.HasOne(d => d.OwnerRequest)
                    .WithMany(p => p.MatchedRequestOwnerRequest)
                    .HasForeignKey(d => d.OwnerRequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MatchedRe__Owner__33D4B598");

                entity.HasOne(d => d.Washer)
                    .WithMany(p => p.MatchedRequestWasher)
                    .HasForeignKey(d => d.WasherId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MatchedRe__Washe__32E0915F");

                entity.HasOne(d => d.WasherRequest)
                    .WithMany(p => p.MatchedRequestWasherRequest)
                    .HasForeignKey(d => d.WasherRequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MatchedRe__Washe__34C8D9D1");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.Message)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TransactionType)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.Transaction)
                    .HasForeignKey(d => d.RequestId)
                    .HasConstraintName("FK__Transacti__Reque__403A8C7D");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Transaction)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Transacti__UserI__3C69FB99");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.Userid);

                entity.ToTable("USERS");

                entity.Property(e => e.Userid)
                    .HasColumnName("USERID")
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Latitude)
                    .IsRequired()
                    .HasColumnName("LATITUDE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Longitude)
                    .IsRequired()
                    .HasColumnName("LONGITUDE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Useremail)
                    .IsRequired()
                    .HasColumnName("USEREMAIL")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Usermobile)
                    .IsRequired()
                    .HasColumnName("USERMOBILE")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("USERNAME")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Userpassword)
                    .IsRequired()
                    .HasColumnName("USERPASSWORD")
                    .HasMaxLength(64);

                entity.Property(e => e.Washing).HasColumnName("WASHING");
            });
        }
    }
}
