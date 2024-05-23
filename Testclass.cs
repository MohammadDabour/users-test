using Xunit;
using System;
using System.Collections.Generic;
using System.IO;

public class TestClass
{
    public TestClass()
    {
        Utils.CheckUsersTableExists();
    }

    [Fact]
    public void ReturnsEncryptedPasswordTest()
    {
        string password = "password";
        string encryptedPassword = Utils.Encrypt(password);
        Assert.NotEqual(password, encryptedPassword);
    }

    [Fact]
    public void CreatesUsersTest()
    {
        if (!File.Exists("json/mock-users.json"))
        {
            throw new FileNotFoundException("mock-users.json file not found");
        }

        var users = Utils.CreateMockUsers();
        Assert.NotEmpty(users);
    }

    [Fact]
    public void RemoveMockUsers_RemovesUsersTest()
    {
        if (!File.Exists("json/mock-users.json"))
        {
            throw new FileNotFoundException("mock-users.json file not found");
        }

        Utils.CreateMockUsers();
        var removedUsers = Utils.RemoveMockUsers();
        Assert.NotEmpty(removedUsers);
    }

    [Fact]
    public void ReturnsDomainCountsTest()
    {
        if (!File.Exists("json/mock-users.json"))
        {
            throw new FileNotFoundException("mock-users.json file not found");
        }

        Utils.CreateMockUsers();
        dynamic domainCounts = Utils.CountDomainsFromUserEmails();
        Assert.NotEmpty((IDictionary<string, object>)domainCounts);
    }
}
