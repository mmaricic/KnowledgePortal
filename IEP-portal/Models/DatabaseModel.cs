namespace IEP_portal.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DatabaseModel : DbContext
    {
        public DatabaseModel()
            : base("name=DatabaseModel")
        {
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<Answer> Answer { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Channel> Channel { get; set; }
        public virtual DbSet<Distributed_Answer> Distributed_Answer { get; set; }
        public virtual DbSet<Distributed_Question> Distributed_Question { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<Parameters> Parameters { get; set; }
        public virtual DbSet<Question> Question { get; set; }
        public virtual DbSet<Response> Response { get; set; }
        public virtual DbSet<Subscribed> Subscribed { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answer>()
                .Property(e => e.Mark)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<AspNetRoles>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.Order)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.Subscribed)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.StudentId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.Channel)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.TeacherId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.Question)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.AuthorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Channel>()
                .HasMany(e => e.Subscribed)
                .WithRequired(e => e.Channel)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Channel>()
                .HasMany(e => e.Distributed_Question)
                .WithRequired(e => e.Channel)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Distributed_Answer>()
                .Property(e => e.Mark)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Distributed_Answer>()
                .HasMany(e => e.Response)
                .WithRequired(e => e.Distributed_Answer)
                .HasForeignKey(e => e.AnswerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Distributed_Question>()
                .HasMany(e => e.Distributed_Answer)
                .WithRequired(e => e.Distributed_Question)
                .HasForeignKey(e => e.QuestionId);

            modelBuilder.Entity<Distributed_Question>()
                .HasMany(e => e.Response)
                .WithRequired(e => e.Distributed_Question)
                .HasForeignKey(e => e.QuestionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.Price)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Question>()
                .HasMany(e => e.Answer)
                .WithRequired(e => e.Question)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Question>()
                .HasMany(e => e.Distributed_Question)
                .WithOptional(e => e.Question)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<Subscribed>()
                .HasMany(e => e.Response)
                .WithRequired(e => e.Subscribed)
                .HasForeignKey(e => new { e.StudentId, e.Channelid })
                .WillCascadeOnDelete(false);
        }
    }
}
