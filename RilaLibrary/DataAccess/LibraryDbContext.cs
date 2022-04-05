using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DataAccess.Models;
using DataAccess.Entities;

namespace DataAccess
{
    public class LibraryDbContext : IdentityDbContext<UserEntity, IdentityRole, string>
    {
        public LibraryDbContext()
        {
        }

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        // DB Sets
        public DbSet<AddressEntity> Addresses { get; set; } = null!;

        public DbSet<AuthorEntity> Authors { get; set; } = null!;

        public DbSet<AuthorsBooks> AuthorsBooks { get; set; } = null!;

        public DbSet<BookEntity> Books { get; set; } = null!;

        public DbSet<BookReservationEntity> BookReservations { get; set; } = null!;

        public DbSet<CommentEntity> Comments { get; set; } = null!;

        public DbSet<GenreEntity> Genres { get; set; } = null!;

        public DbSet<GenresBooks> GenresBooks { get; set; } = null!;

        public DbSet<NotificationEntity> Notifications { get; set; } = null!;

        public DbSet<ProlongingRequestEntity> ProlongingRequests { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customise the default (IdentitytUser & IdentityRoles) table names
            modelBuilder.Entity<UserEntity>(b =>
            {
                b.ToTable("Users");
            });
            modelBuilder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.ToTable("UserClaims");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.ToTable("UserLogins");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(b =>
            {
                b.ToTable("UserTokens");
            });

            modelBuilder.Entity<IdentityRole>(b =>
            {
                b.ToTable("Roles");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.ToTable("RoleClaims");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(b =>
            {
                b.ToTable("UserRoles");
            });

            // Many-to-many relations, composite key with fluent API
            modelBuilder.Entity<AuthorsBooks>(entity =>
           {
               entity.HasKey(ab => new { ab.AuthorEntityId, ab.BookEntityId });
           });

            modelBuilder.Entity<GenresBooks>(entity =>
            {
                entity.HasKey(gb => new { gb.GenreEntityId, gb.BookEntityId });
            });
        }

        public override int SaveChanges()
            => this.SaveChanges(true);

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await this.SaveChangesAsync(true);

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            this.ApplyAuditInfoRules();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess);
        }

        private void ApplyAuditInfoRules()
        {
            var changedEntries = this.ChangeTracker
                .Entries()
                .Where(e =>
                    e.Entity is IAuditableEntity &&
                    (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in changedEntries)
            {
                var entity = (IAuditableEntity)entry.Entity;
                if (entry.State == EntityState.Added && entity.CreatedOn == default)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }
                else
                {
                    entity.ModifiedOn = DateTime.UtcNow;
                }
            }
        }
    }
}