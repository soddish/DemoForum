using DemoForum.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoForum.Services.Db.Implementations.Ef;

public class SubforumDb(DbContextOptions<SubforumDb> options) : 
    DbContext(options), ISubforumStore
{
    public DbSet<Subforum> Subforums => Set<Subforum>();

}