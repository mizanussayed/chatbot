using SignarlRChat.Entity;
using SignarlRChat.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace SignarlRChat.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHubContext<ChatHub> hubContext)
            : base(options)
        {
            _hubContext = hubContext;
        }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ChatGroup> ChatGroups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<ConversationFile> ConversationFiles { get; set; }
        public DbSet<TransectionHistory> TransectionHistories { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>(entity =>
                entity.ToTable("Users", "Identity")
            );
            builder.Entity<IdentityRole>(entity =>
               entity.ToTable("Roles", "Identity")
            );
            builder.Entity<IdentityUserRole<string>>(entity =>
               entity.ToTable("UserRoles", "Identity")
            );
            builder.Entity<IdentityUserClaim<string>>(entity =>
               entity.ToTable("UserClaims", "Identity")
            );
            builder.Entity<IdentityUserLogin<string>>(entity =>
               entity.ToTable("UserLogins", "Identity")
            );
            builder.Entity<IdentityUserToken<string>>(entity =>
               entity.ToTable("UserTokens", "Identity")
            );
            builder.Entity<IdentityRoleClaim<string>>(entity =>
               entity.ToTable("RoleClaims", "Identity")
            );
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var changedEntities = ChangeTracker.Entries<TransectionHistory>()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted)
                .Select(x => x.Entity);
            foreach (var entity in changedEntities)
            {
                // Call SignalR hub method to send notifications with the changedEntities
                await _hubContext.Clients.User(entity.NotifyUserId).SendAsync("ReceiveNotifications", entity);

            }
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            var changedEntities = ChangeTracker.Entries<TransectionHistory>()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted)
                .Select(x => x.Entity);
            foreach (var entity in changedEntities)
            {
                // Call SignalR hub method to send notifications with the changedEntities
                _hubContext.Clients.User(entity.NotifyUserId).SendAsync("ReceiveNotifications", entity);

            }
            return base.SaveChanges();
        }

    }
}