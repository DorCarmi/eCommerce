using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.DataLayer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace eCommerce
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // CreateHostBuilder(args).Build().Run();
            // test_write();
            test_read();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        private static void test_write()
        {
            using (var db = new testContext())
            {
                var student1 = new Student() {StudentId = 1, StudentName = "One"};
                var course1 = new Course() {CourseId = 8, CourseName = "history"};
                var student2 = new Student() {StudentId = 2, StudentName = "Two"};
                var course2 = new Course() {CourseId = 9, CourseName = "Arts"};
                var class1 = new Classroom() {ClassroomId = 43};
                var class2 = new Classroom() {ClassroomId = 45};
                var pair1_1 = new MyPair<Classroom, Course>()
                {
                    StudentId = student1.StudentId, Courses =  new List<Course>(){course1}, ClassroomId = class1.ClassroomId,
                    Classroom = class1
                };
                
                var pair1_2 = new MyPair<Classroom, Course>()
                {
                    StudentId = student1.StudentId, Courses =  new List<Course>(){course2}, ClassroomId = class2.ClassroomId,
                    Classroom = class2
                };
                student1.dict.Add(pair1_1);
                student1.dict.Add(pair1_2);
                var pair2_1 = new MyPair<Classroom, Course>()
                {
                    StudentId = student2.StudentId, Courses =  new List<Course>(){course1}, ClassroomId = class1.ClassroomId,
                    Classroom = class1
                };
                
                var pair2_2 = new MyPair<Classroom, Course>()
                {
                    StudentId = student2.StudentId, Courses =  new List<Course>(){course1,course2}, ClassroomId = class2.ClassroomId,
                    Classroom = class2
                };
                student2.dict.Add(pair2_1);
                student2.dict.Add(pair2_2);
                // Create
                Console.WriteLine("Inserting a new blog");
                db.Add(student1);
                db.Add(student2);
                db.SaveChanges();
                
            }
        }

        private static void test_read()
        {
             using (var db = new testContext())
             {
                 Console.WriteLine("Querying for a blog");
                 var pairs = db.Pairs
                     .Include(p => p.Classroom)
                     .Include(p => p.Courses)
                     .ToList();
                 var students = db.Students
                     .Include(s => s.dict)
                     .ThenInclude(par => par.Courses)
                     .Include(s => s.dict)
                     .ThenInclude(par => par.Classroom)
                     .OrderBy(s => s.StudentId)
                     .ToList();
                 Console.WriteLine(students[0].dict[0].Courses[0] == students[1].dict[0].Courses[0]);
             }
        }

    }
}