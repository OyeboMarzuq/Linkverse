using Linkverse.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Linkverse.Persistence.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<ServiceProviders> ServiceProviders { get; set; }
        public DbSet<BankDetails> BankDetails { get; set; }
        public DbSet<HousingListing> HousingListings { get; set; }
        public DbSet<HousingImage> HousingImages { get; set; }
        public DbSet<StudyPDF> StudyPDFs { get; set; }
        public DbSet<PDFPurchase> PDFPurchases { get; set; }
        public DbSet<MatchProfile> MatchProfiles { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchResult> MatchResults { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Agreement> Agreements { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<CampusLocation> CampusLocations { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.UserName).IsUnique();
                entity.Property(u => u.UserName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Role).HasMaxLength(50);
            });
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasOne(p => p.User)
                      .WithOne()
                      .HasForeignKey<UserProfile>(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(p => p.Bio).HasMaxLength(300);
                entity.Property(p => p.Department).HasMaxLength(100);
                entity.Property(p => p.Faculty).HasMaxLength(100);
                entity.Property(p => p.Location).HasMaxLength(200);
            });

            modelBuilder.Entity<ServiceProviders>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasOne(p => p.User)
                      .WithMany()
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(p => p.BusinessName).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Description).HasMaxLength(500);
                entity.Property(p => p.Location).HasMaxLength(150);
                entity.Property(p => p.LicenseNumber).HasMaxLength(100);
                entity.Property(p => p.Rating).HasColumnType("float");
                entity.Property(p => p.Type).HasConversion<string>();
                entity.HasQueryFilter(p => !p.IsDeleted);
            });

            modelBuilder.Entity<BankDetails>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.HasOne(b => b.Provider)
                      .WithOne(p => p.BankDetails)
                      .HasForeignKey<BankDetails>(b => b.ProviderId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(b => b.BankName).IsRequired().HasMaxLength(100);
                entity.Property(b => b.AccountNumber).IsRequired().HasMaxLength(10);
                entity.Property(b => b.AccountName).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<HousingListing>(entity =>
            {
                entity.HasKey(h => h.Id);
                entity.HasOne(h => h.Provider)
                      .WithMany(p => p.HousingListings)
                      .HasForeignKey(h => h.ProviderId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(h => h.Title).IsRequired().HasMaxLength(150);
                entity.Property(h => h.Location).IsRequired().HasMaxLength(200);
                entity.Property(h => h.PricePerYear).HasColumnType("decimal(18,2)");
                entity.Property(h => h.Description).HasMaxLength(1000);
                entity.Property(h => h.Tag).HasMaxLength(50);
                entity.Property(h => h.Type).HasConversion<string>();
                entity.Property(h => h.Apartment).HasConversion<string>();
                entity.HasQueryFilter(h => !h.IsDeleted);
            });

            modelBuilder.Entity<HousingImage>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.HasOne(i => i.Listing)
                      .WithMany(h => h.Images)
                      .HasForeignKey(i => i.ListingId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(i => i.ImageUrl).IsRequired().HasMaxLength(500);
            });

            modelBuilder.Entity<StudyPDF>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.HasOne(s => s.Provider)
                      .WithMany(p => p.StudyNotes)
                      .HasForeignKey(s => s.ProviderId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(s => s.CourseCode).IsRequired().HasMaxLength(20);
                entity.Property(s => s.Department).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Price).HasColumnType("decimal(18,2)");
                entity.Property(s => s.FileUrl).IsRequired().HasMaxLength(500);
                entity.HasQueryFilter(s => !s.IsDeleted);
            });

            modelBuilder.Entity<PDFPurchase>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasOne(p => p.PDF)
                      .WithMany(s => s.Purchases)
                      .HasForeignKey(p => p.NoteId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(p => p.User)
                      .WithMany()
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(p => new { p.NoteId, p.UserId }).IsUnique();
            });

            modelBuilder.Entity<MatchProfile>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.HasOne(m => m.User)
                      .WithMany()
                      .HasForeignKey(m => m.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(m => m.ReligionPreference).HasMaxLength(50);
                entity.Property(m => m.HeightPreference).HasMaxLength(30);
                entity.Property(m => m.Department).HasMaxLength(100);
                entity.Property(m => m.Bio).HasMaxLength(300);
                entity.Property(m => m.LookingFor).HasConversion<string>();
            });

            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.HasOne(m => m.Requester)
                      .WithMany(mp => mp.MatchesAsRequester)
                      .HasForeignKey(m => m.RequesterId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(m => m.Status).HasConversion<string>();
                entity.Property(m => m.CompatibilityScore).HasColumnType("float");
                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.HasOne(b => b.User)
                      .WithMany()
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(b => b.Status).HasMaxLength(20);
                entity.Property(b => b.Notes).HasMaxLength(500);
                entity.HasQueryFilter(b => !b.IsDeleted);
            });

            modelBuilder.Entity<Agreement>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.HasIndex(a => a.ReferenceNumber).IsUnique();
                entity.HasOne(a => a.PartyA)
                      .WithMany()
                      .HasForeignKey(a => a.PartyAId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(a => a.Booking)
                      .WithOne(b => b.Agreement)
                      .HasForeignKey<Agreement>(a => a.BookingId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.Property(a => a.ReferenceNumber).IsRequired().HasMaxLength(20);
                entity.Property(a => a.Title).IsRequired().HasMaxLength(200);
                entity.Property(a => a.Content).IsRequired().HasMaxLength(10000);
                entity.Property(a => a.Status).HasMaxLength(20);
                entity.Property(a => a.DigitalStampHash).HasMaxLength(64);
                entity.Property(a => a.DocumentUrl).HasMaxLength(500);
                entity.HasQueryFilter(a => !a.IsDeleted);
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.HasOne(s => s.User)
                      .WithMany()
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(s => s.PricePerMonth).HasColumnType("decimal(18,2)");
                entity.Property(s => s.Plan).HasConversion<string>();
            });

            modelBuilder.Entity<CampusLocation>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(150);
                entity.Property(c => c.Type).IsRequired().HasMaxLength(30);
                entity.Property(c => c.Description).HasMaxLength(500);
                entity.Property(c => c.ImageUrl).HasMaxLength(500);
                entity.Property(c => c.Campus).HasConversion<string>();
                entity.HasIndex(c => c.Campus);
            });

            // ─── MatchResult ──────────────────────────────────────────────────────
            modelBuilder.Entity<MatchResult>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.HasIndex(r => new { r.SeekerId, r.UnlockedProfileId }).IsUnique();
                entity.HasIndex(r => r.UnlockToken).IsUnique();

                entity.HasOne(r => r.Seeker)
                      .WithMany()
                      .HasForeignKey(r => r.SeekerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.UnlockedProfile)
                      .WithMany()
                      .HasForeignKey(r => r.UnlockedProfileId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(r => r.UnlockToken).IsRequired().HasMaxLength(100);
                entity.HasQueryFilter(r => !r.IsDeleted);
            });


            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasOne(c => c.ParticipantA)
                      .WithMany()
                      .HasForeignKey(c => c.ParticipantAId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(c => c.ParticipantB)
                      .WithMany()
                      .HasForeignKey(c => c.ParticipantBId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.HasOne(m => m.Conversation)
                      .WithMany(c => c.Messages)
                      .HasForeignKey(m => m.ConversationId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(m => m.Sender)
                      .WithMany()
                      .HasForeignKey(m => m.SenderId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(m => m.Content).IsRequired().HasMaxLength(2000);
            });
        }
    }
}
