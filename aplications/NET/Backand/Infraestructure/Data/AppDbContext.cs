
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<LocationShare> LocationShares { get; set; }
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Friendship> Friendships => Set<Friendship>();
        public DbSet<Garage> Garages => Set<Garage>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<UserLocation> UserLocations => Set<UserLocation>();
        public DbSet<Like> Likes => Set<Like>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<UserGroup> UserGroups => Set<UserGroup>();
        public DbSet<PrivateMessage> PrivateMessages => Set<PrivateMessage>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<UserEvent> UserEvents => Set<UserEvent>();
        public DbSet<ErrorLog> ErrorLogs => Set<ErrorLog>();



    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Exemplo de configuração de chave composta para UserGroup e UserEvent
            modelBuilder.Entity<UserGroup>()
                .HasKey(ug => new { ug.UserId, ug.GroupId });

            modelBuilder.Entity<UserEvent>()
                .HasKey(ue => new { ue.UserId, ue.EventId });

            // Adicione outras configurações específicas se necessário


            // Configuração explícita dos relacionamentos de amizade
            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Requester)
                .WithMany(u => u.FriendRequestsSent)
                .HasForeignKey(f => f.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Addressee)
                .WithMany(u => u.FriendRequestsReceived)
                .HasForeignKey(f => f.AddresseeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar a relação para LocationShare
            modelBuilder.Entity<LocationShare>()
                .HasOne(ls => ls.Sharer)
                .WithMany()
                .HasForeignKey(ls => ls.SharerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LocationShare>()
                .HasOne(ls => ls.Observer)
                .WithMany()
                .HasForeignKey(ls => ls.ObserverId)
                .OnDelete(DeleteBehavior.Restrict);

            // ErrorLog indexes for faster queries by time and correlation
            modelBuilder.Entity<ErrorLog>(ConfigureErrorLog);

        }

        private static void ConfigureErrorLog(EntityTypeBuilder<ErrorLog> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.OccurredAtUtc);
            builder.HasIndex(e => e.CorrelationId);
            builder.Property(e => e.Message).IsRequired();
        }

    }
}
