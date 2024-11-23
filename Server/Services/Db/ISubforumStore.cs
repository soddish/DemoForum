using DemoForum.Models;

namespace DemoForum.Services.Db;

public interface ISubforumStore
{
    Task<Subforum[]> GetAll();
    Task<Subforum?> GetById(int subforumId);
    Task<bool> Update(int id, Subforum subforum);
    Task<bool> Add(Subforum subforum);
}