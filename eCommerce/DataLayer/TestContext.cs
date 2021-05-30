using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.DataLayer
{
    public class testContext: DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<MyPair<Classroom,Course>> Pairs { get; set; }

        
       // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    =>optionsBuilder.EnableSensitiveDataLogging().UseSqlite(@"Data Source=.\Persistence\testSchool.db");

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MyPair<Classroom,Course>>()
                .HasKey(p => new {p.StudentId, p.ClassroomId});
        }
    }

    public class MyPair<K,V>
    {
        public int ClassroomId { get; set; }
        public K Classroom { get; set; }
        // public int CourseId { get; set; }
        public virtual List<V> Courses { get; set; }
        public int StudentId { get; set; }
    }
    
    // public class MyPair2<K,V>
    // {
    //     public int ClassroomId { get; set; }
    //     public K Classroom { get; set; }
    //     public int CourseId { get; set; }
    //     public V Course { get; set; }
    //     public int StudentId { get; set; }
    // }

    public class Student
    {
        public Student() 
        {
            this.dict = new List<MyPair<Classroom,Course>>();
        }

        public int StudentId { get; set; }
        [Required]
        public string StudentName { get; set; }

        [Required]
        public virtual List<MyPair<Classroom,Course>> dict { get; set; }
    }
        
    public class Course
    {
        public Course()
        {
            this.Students = new HashSet<Student>();
        }

        public int CourseId { get; set; }
        public string CourseName { get; set; }

        [NotMapped]
        public virtual ICollection<Student> Students { get; set; }
    }

    public class Classroom
    {
        public int ClassroomId { get; set; }
    }
}