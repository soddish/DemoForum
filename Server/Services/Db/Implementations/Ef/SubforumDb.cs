using DemoForum.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoForum.Services.Db.Implementations.Ef;

public class SubforumDb(DbContextOptions<SubforumDb> options) : 
    DbContext(options), ISubforumStore
{
    public DbSet<Subforum> Subforums => Set<Subforum>();

    async Task<bool> ISubforumStore.Add(Subforum subforum)
    {
        Subforums.Add(subforum);
        await SaveChangesAsync();
        return true;
    }

    Task<Subforum[]> ISubforumStore.GetAll() =>
        Subforums.ToArrayAsync();

    Task<Subforum?> ISubforumStore.GetById(int subforumId) =>
        Subforums.FindAsync(subforumId).AsTask();

    async Task<bool> ISubforumStore.Update(int id, Subforum inputSubforum)
    {
        var subforum = await Subforums
            .FindAsync(id);

        if(subforum is null)
            return false;

        subforum.Name = inputSubforum.Name;
        subforum.NewThreadsOpen = inputSubforum.NewThreadsOpen;

        await SaveChangesAsync();

        return true;
    }
}