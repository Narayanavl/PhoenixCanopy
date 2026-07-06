using System;
using System.Collections.Generic;

namespace StowellCoApp.DTO
{

    public class OverviewData
    {
        public int EmployeeID { get; set; }
        public string Employees { get; set; }
        public string Designation { get; set; }
        public string Mail { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string Trustworthiness { get; set; }
        public int Rating { get; set; }
        public int Software { get; set; }
        public double CurrentSalary { get; set; }
        public string Address { get; set; }
        public string EmployeeImg { get; set; }

        public static List<OverviewData> GetAllRecords(int count)
        {
            var list = new List<OverviewData>();
            var rand = new Random();

            string[] names = { "John", "Jane", "Mike", "Sara", "David" };
            string[] roles = { "Developer", "Manager", "Analyst" };
            string[] locations = { "USA", "UK", "India" };

            for (int i = 1; i <= count; i++)
            {
                list.Add(new OverviewData
                {
                    EmployeeID = i,
                    Employees = names[rand.Next(names.Length)] + " " + i,
                    Designation = roles[rand.Next(roles.Length)],
                    Mail = $"user{i}@mail.com",
                    Location = locations[rand.Next(locations.Length)],
                    Status = rand.Next(2) == 0 ? "Active" : "Inactive",
                    Trustworthiness = "High",
                    Rating = rand.Next(1, 6),
                    Software = rand.Next(10, 100),
                    CurrentSalary = rand.Next(30000, 100000),
                    Address = "Sample Address",
                    EmployeeImg = rand.Next(2) == 0 ? "usermale" : "userfemale"
                });
            }

            return list;
        }
    }
    public class Book
    {
        public string BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public DateTime PublishDate { get; set; }
        public int AvailableCopies { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
    }
    public static class BookData
    {
        public static List<Book> GetAllRecords()
        {
            var list = new List<Book>();
            var rand = new Random();

            string[] categories = { "Fiction", "Science", "Technology", "History" };
            string[] status = { "Available", "Borrowed", "Reserved" };

            for (int i = 1; i <= 20; i++)
            {
                list.Add(new Book
                {
                    BookId = $"BOOK{i:D3}",
                    Title = $"Book {i}",
                    Author = $"Author {i}",
                    Category = categories[rand.Next(categories.Length)],
                    PublishDate = DateTime.Now.AddDays(-rand.Next(1000)),
                    AvailableCopies = rand.Next(1, 20),
                    Location = $"Shelf {rand.Next(1, 10)}",
                    Status = status[rand.Next(status.Length)]
                });
            }

            return list;
        }
    }
}