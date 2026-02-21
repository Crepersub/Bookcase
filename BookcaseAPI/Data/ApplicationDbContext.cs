using BookcaseAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookcaseAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationExam> ApplicationExams { get; set; }
        public DbSet<MajorExam> MajorExams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationExam>()
                .HasKey(ae => new { ae.ApplicationId, ae.ExamId });

            modelBuilder.Entity<ApplicationExam>()
                .HasOne(ae => ae.Application)
                .WithMany(a => a.ApplicationExams)
                .HasForeignKey(ae => ae.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationExam>()
                .HasOne(ae => ae.Exam)
                .WithMany(e => e.ApplicationExams)
                .HasForeignKey(ae => ae.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MajorExam>()
                .HasKey(me => new { me.MajorId, me.ExamId });

            modelBuilder.Entity<MajorExam>()
                .HasOne(me => me.Major)
                .WithMany(m => m.MajorExams)
                .HasForeignKey(me => me.MajorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MajorExam>()
                .HasOne(me => me.Exam)
                .WithMany(e => e.MajorExams)
                .HasForeignKey(me => me.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.Username)
                .IsUnique();

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Major)
                .WithMany(m => m.Applications)
                .HasForeignKey(a => a.MajorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Student)
                .WithMany(c => c.Applications)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}