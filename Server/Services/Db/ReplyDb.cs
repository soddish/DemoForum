using DemoForum.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoForum.Services.Db;

public class ReplyDb(DbContextOptions<ReplyDb> options) :
    DbContext(options)
{
    public DbSet<Reply> Replies => Set<Reply>();
}