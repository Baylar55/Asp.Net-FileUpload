using Microsoft.EntityFrameworkCore;
using PurpleBuzz_Backend.Models;

namespace PurpleBuzz_Backend.DAL
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<RecentWorkComponent> RecentWorkComponents { get; set; }
        public DbSet<ContactIntro> ContactIntro { get; set; }
        public DbSet<ContactContextComponent> ContactContextComponent { get; set; }
        public DbSet<ContactCommunicationComponent>ContactCommunicationComponents  { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryComponent> CategoryComponents { get; set; }
        public DbSet<PricingComponent> PricingComponents { get; set; }
        public DbSet<ObjectiveComponent> ObjectiveComponents { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
    }
}
