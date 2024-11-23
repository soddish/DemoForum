using Thead = DemoForum.Models.Thread;
using Microsoft.EntityFrameworkCore;

namespace DemoForum.Services.Db;

public class ThreadDb(DbContextOptions<ThreadDb> options) :
    DbContext(options)
{
    public DbSet<Thread> Threads => Set<Thread>();
}