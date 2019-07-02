using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IConfiguration _config;

        public StudentsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Students
        public ActionResult Index() // list method
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        SELECT s.Id,
                                            s.FirstName,
                                            s.LastName,
                                            s.Slack,
                                            s.CohortId
                                        FROM Student s
                                    ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Student> students = new List<Student>();
                    while (reader.Read())
                    {
                        Student student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Slack = reader.GetString(reader.GetOrdinal("Slack")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                        };

                        students.Add(student);
                    }

                    reader.Close();

                    return View(students);
                }
            }
        }

        // GET: Students/Details/5
        public ActionResult Details(int id)
        {
            Student student = GetStudentById(id);
            return View(student);
            //using (SqlConnection conn = Connection)
            //{
            //    conn.Open();
            //    using (SqlCommand cmd = conn.CreateCommand())
            //    {
            //        cmd.CommandText = @"SELECT s.Id,
            //                                s.FirstName,
            //                                s.LastName,
            //                                s.Slack,
            //                                s.CohortId
            //                            FROM Student s
            //                            WHERE s.Id = @id;";

            //        cmd.Parameters.Add(new SqlParameter("@id", id));

            //        SqlDataReader reader = cmd.ExecuteReader();

            //        Student student1 = null;
            //        while (reader.Read())
            //        {
            //            student1 = new Student
            //            {
            //                Id = reader.GetInt32(reader.GetOrdinal("Id")),
            //                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
            //                LastName = reader.GetString(reader.GetOrdinal("LastName")),
            //                Slack = reader.GetString(reader.GetOrdinal("Slack")),
            //                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
            //            };

            //        }

            //        reader.Close();
            //        return View(student1);
            //    }
            //}
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Students/Edit/5 
        // get the existing 
        public ActionResult Edit(int id)
        {
            Student student = GetStudentById(id);
            List<Cohort> cohorts = GetAllCohorts();
            StudentEditViewModel viewModel = new StudentEditViewModel();
            viewModel.Student = student;
            viewModel.AvailableCohorts = cohorts;
            return View(viewModel);
            /*using (SqlConnection conn = Connection)
            //{
            //    conn.Open();
            //    using (SqlCommand cmd = conn.CreateCommand())
            //    {
            //        cmd.CommandText = @"SELECT s.Id,
            //                                s.FirstName,
            //                                s.LastName,
            //                                s.Slack,
            //                                s.CohortId
            //                            FROM Student s
            //                            WHERE s.Id = @id;";

            //        cmd.Parameters.Add(new SqlParameter("@id", id));

            //        SqlDataReader reader = cmd.ExecuteReader();

            //        Student student1 = null;
            //        if (reader.Read())
            //        {
            //            student1 = new Student
            //            {
            //                Id = reader.GetInt32(reader.GetOrdinal("Id")),
            //                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
            //                LastName = reader.GetString(reader.GetOrdinal("LastName")),
            //                Slack = reader.GetString(reader.GetOrdinal("Slack")),
            //                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
            //            };

            //        }
            //        reader.Close();
                   
            //        return View(student1);
            //    }
            }*/

        }

        private List<Cohort> GetAllCohorts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id,
                                               c.CohortName
                                        FROM Cohort c";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Cohort> cohorts = new List<Cohort>();
                    while (reader.Read())
                    {
                        Cohort cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("CohortName"))
                        };

                        cohorts.Add(cohort);
                    }

                    reader.Close();

                    return cohorts;
                }
            }
        }

        // POST: Students/Edit/5
        // 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, StudentEditViewModel viewModel)
        {
            Student student = viewModel.Student;
            try
            {
                using(SqlConnection conn = Connection)
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Student
                                            SET FirstName = @firstname,
                                                LastName = @lastname,
                                                Slack = @slack,
                                                CohortId = @cohortId
                                            WHERE Id=@id;";
                        cmd.Parameters.Add(new SqlParameter("@firstname", student.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastname", student.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slack", student.Slack));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", student.CohortId));
                        cmd.Parameters.Add(new SqlParameter("@id", student.Id));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id,
                                            s.FirstName,
                                            s.LastName,
                                            s.Slack,
                                            s.CohortId
                                        FROM Student s
                                        WHERE s.Id = @id;";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Student student1 = null;
                    if (reader.Read())
                    {
                        student1 = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Slack = reader.GetString(reader.GetOrdinal("Slack")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                        };

                    }
                    reader.Close();
                    return View(student1);
                }
            }
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // TODO: Add delete logic here
                using (SqlConnection conn = Connection)
                {
                    // open the connection 
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        // create the query and run it 
                        cmd.CommandText = @"DELETE FROM StudentExercise WHERE StudentId = @id;";

                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = @"DELETE FROM Student 
                                                   WHERE Id = @id;";


                        cmd.ExecuteNonQuery();
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        private Student GetStudentById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id,
                                            s.FirstName,
                                            s.LastName,
                                            s.Slack,
                                            s.CohortId
                                        FROM Student s
                                        WHERE s.Id = @id;";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Student student1 = null;
                    while (reader.Read())
                    {
                        student1 = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Slack = reader.GetString(reader.GetOrdinal("Slack")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                        };

                    }

                    reader.Close();
                    return student1;
                }
            }
        }
    }
}