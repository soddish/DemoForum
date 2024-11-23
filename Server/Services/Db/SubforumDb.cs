using DemoForum.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoForum.Services.Db;

public class SubforumDb(DbContextOptions<SubforumDb> options) : 
    DbContext(options)
{
    public DbSet<Subforum> Subforums => Set<Subforum>();
}