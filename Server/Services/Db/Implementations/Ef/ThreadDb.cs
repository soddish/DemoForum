using Thread = DemoForum.Models.Thread;
using Microsoft.EntityFrameworkCore;

namespace DemoForum.Services.Db.Implementations.Ef;

public class ThreadDb(DbContextOptions<ThreadDb> options) :
    DbContext(options)
{
    public DbSet<Thread> Threads => Set<Thread>();
}