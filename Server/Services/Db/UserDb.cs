using DemoForum.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoForum.Services.Db;

class UserDb(DbContextOptions<UserDb> options) : 
    DbContext(options)
{
    public DbSet<User> Users => Set<User>();
}