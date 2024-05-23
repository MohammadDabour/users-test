using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Dynamic;
using System.IO;
using System.Text.Json;

public static class Utils
{
    public static string Encrypt(string password)
    {
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }

    public static void CheckUsersTableExists()
    {
        using (var connection = new SQLiteConnection("Data Source=users.db"))
        {
            connection.Open();
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Email TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL
                )";
            using (var cmd = new SQLiteCommand(createTableQuery, connection))
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine("Users table ensured.");
            }
        }
    }

    public static List<object> CreateMockUsers()
    {
        CheckUsersTableExists();

        string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "json", "mock-users.json");
        if (!File.Exists(jsonFilePath))
        {
            Console.WriteLine("Mock users file not found.");
            return new List<object>();
        }

        string jsonString = File.ReadAllText(jsonFilePath);
        var users = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(jsonString);

        List<object> createdUsers = new List<object>();

        using (var connection = new SQLiteConnection("Data Source=users.db"))
        {
            connection.Open();

            foreach (var user in users)
            {
                string email = user["email"];
                string password = char.ToUpper(email[0]) + email.Substring(1);
                string encryptedPassword = Encrypt(password);

                string checkUserQuery = $"SELECT COUNT(*) FROM Users WHERE Email = @Email";
                using (var checkCmd = new SQLiteCommand(checkUserQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@Email", email);
                    long userCount = (long)checkCmd.ExecuteScalar();

                    if (userCount == 0)
                    {
                        string insertUserQuery = $"INSERT INTO Users (Email, Password) VALUES (@Email, @Password)";
                        using (var insertCmd = new SQLiteCommand(insertUserQuery, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@Email", email);
                            insertCmd.Parameters.AddWithValue("@Password", encryptedPassword);
                            insertCmd.ExecuteNonQuery();
                            Console.WriteLine($"User created: {email}");
                            createdUsers.Add(new { Email = email });
                        }
                    }
                    else
                    {
                        Console.WriteLine($"User already exists: {email}");
                    }
                }
            }
        }

        return createdUsers;
    }

    public static List<object> RemoveMockUsers()
    {
        CheckUsersTableExists();

        string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "json", "mock-users.json");
        if (!File.Exists(jsonFilePath))
        {
            Console.WriteLine("Mock users file not found.");
            return new List<object>();
        }

        string jsonString = File.ReadAllText(jsonFilePath);
        var users = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(jsonString);

        List<object> removedUsers = new List<object>();

        using (var connection = new SQLiteConnection("Data Source=users.db"))
        {
            connection.Open();

            foreach (var user in users)
            {
                string email = user["email"];

                string checkUserQuery = $"SELECT COUNT(*) FROM Users WHERE Email = @Email";
                using (var checkCmd = new SQLiteCommand(checkUserQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@Email", email);
                    long userCount = (long)checkCmd.ExecuteScalar();

                    if (userCount > 0)
                    {
                        string deleteUserQuery = $"DELETE FROM Users WHERE Email = @Email";
                        using (var deleteCmd = new SQLiteCommand(deleteUserQuery, connection))
                        {
                            deleteCmd.Parameters.AddWithValue("@Email", email);
                            deleteCmd.ExecuteNonQuery();
                            Console.WriteLine($"User removed: {email}");
                            removedUsers.Add(new { Email = email });
                        }
                    }
                }
            }
        }

        return removedUsers;
    }

    public static dynamic CountDomainsFromUserEmails()
    {
        CheckUsersTableExists();

        var domainCounts = new Dictionary<string, int>();

        using (var connection = new SQLiteConnection("Data Source=users.db"))
        {
            connection.Open();
            string query = "SELECT Email FROM Users";

            using (var cmd = new SQLiteCommand(query, connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string email = reader["Email"].ToString();
                    string domain = email.Split('@')[1];

                    if (domainCounts.ContainsKey(domain))
                    {
                        domainCounts[domain]++;
                    }
                    else
                    {
                        domainCounts[domain] = 1;
                    }
                }
            }
        }

        dynamic result = new ExpandoObject();
        var resultDict = (IDictionary<string, object>)result;

        foreach (var domainCount in domainCounts)
        {
            resultDict[domainCount.Key] = domainCount.Value;
        }

        return result;
    }
}
