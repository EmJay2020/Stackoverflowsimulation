using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EFManytoMany.data
{
    public class UserContext: DbContext
    {
        private string _connectionsString;
        public UserContext(string cs)
        {
            _connectionsString = cs;
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<Answers> Answers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Likes> Likes { get; set; }
        public DbSet<QuestionsTags> QuestionsTags { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionsString);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            //Taken from here:
            //https://www.learnentityframeworkcore.com/configuration/many-to-many-relationship-configuration

            //set up composite primary key
            modelBuilder.Entity<QuestionsTags>()
                .HasKey(qt => new { qt.QuestionsId, qt.TagId });

            //set up foreign key from QuestionsTags to Questions
            modelBuilder.Entity<QuestionsTags>()
                .HasOne(qt => qt.Questions)
                .WithMany(q => q.QuestionsTags)
                .HasForeignKey(q => q.QuestionsId);

            //set up foreign key from QuestionsTags to Tags
            modelBuilder.Entity<QuestionsTags>()
                .HasOne(qt => qt.Tag)
                .WithMany(t => t.QuestionsTags)
                .HasForeignKey(q => q.TagId);

            modelBuilder.Entity<Likes>()
                .HasKey(qt => new { qt.UserId, qt.QuestionsId });

            modelBuilder.Entity<Likes>()
                .HasOne(qt => qt.User)
                .WithMany(q => q.Likes)
                .HasForeignKey(q => q.UserId);

            modelBuilder.Entity<Likes>()
                .HasOne(qt => qt.Questions)
                .WithMany(q => q.Likes)
                .HasForeignKey(q => q.QuestionsId);

            modelBuilder.Entity<Answers>()
               .HasKey(qt => new { qt.UserId, qt.QuestionsId });

            modelBuilder.Entity<Answers>()
                .HasOne(qt => qt.User)
                .WithMany(q => q.Answers)
                .HasForeignKey(q => q.UserId);

            modelBuilder.Entity<Answers>()
                .HasOne(qt => qt.Questions)
                .WithMany(q => q.Answers)
                .HasForeignKey(q => q.QuestionsId);
        }
    }
}
