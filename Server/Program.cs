using DemoForum.Models;
using DemoForum.Services.Db;
using DemoForum.Services.Db.Implementations.Ef;
using DemoForum.Services.Db.Implementations.Postgres;
using Microsoft.EntityFrameworkCore;
using Thread = DemoForum.Models.Thread;

var builder = WebApplication.CreateBuilder(args);

//register database services
builder.Services.AddDbContext<UserDb>(opt => opt.UseInMemoryDatabase("Users"));
//builder.Services.AddDbContext<ISubforumStore, SubforumDb>(opt => opt.UseInMemoryDatabase("Subforums"));
builder.Services.AddDbContext<ReplyDb>(opt => opt.UseInMemoryDatabase("Replies"));
builder.Services.AddDbContext<ThreadDb>(opt => opt.UseInMemoryDatabase("Threads"));

builder.Services.AddSingleton<ISubforumStore, PostgresDatabase>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// add openAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "DemoForum";
    config.Title = "Demo Forum";
    config.Version = "v1";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "DemoForum";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

app.MapGet("/", () => "Hello World!");

// get a list of available subforums
app.MapGet("/subforums", async (ISubforumStore subforumStore) =>
    await subforumStore.GetAll());

// get a subforum
app.MapGet("/subforums/{id}", async (int id, ISubforumStore subforumStore) =>
{
    var subforum = await subforumStore.GetById(id);

    if(subforum is not null)
        return Results.Ok(subforum);
    else
        return Results.NotFound();
});

// create a subforum
app.MapPost("/subforums", async (Subforum subforum, ISubforumStore subforumStore) =>
{
    await subforumStore.Add(subforum);
    return Results.Created($"/subforums/{subforum.Id}", subforum);
});

// update a subforum
app.MapPut("/subforums/{id}", async (int id, Subforum inputSubforum, ISubforumStore subforumStore) =>
{
    var result = await subforumStore.Update(id, inputSubforum);
    if(result)
        return Results.NoContent();
    else
        return Results.NotFound();
});

// get a list of the threads that have been created in a subforum
app.MapGet("/subforums/{id}/threads", async (int id, ThreadDb threadDb) =>
    await threadDb.Threads
        .Where(thread =>
            thread.SubforumId == id)
        .ToArrayAsync());

// create a thread
app.MapPost("/threads", async (Thread thread, ThreadDb threadDb, ISubforumStore subforumDb) =>
{
    if(string.IsNullOrWhiteSpace(thread.Title))
        return Results.BadRequest("Thread must have a title");

    if(string.IsNullOrWhiteSpace(thread.Content))
        return Results.BadRequest("Thread must have content");

    // var subforum = await subforumDb.Subforums.FindAsync(thread.SubforumId);
    // if(subforum is null)
    //     return Results.BadRequest("Subforum does not exist");

    // if(!subforum.NewThreadsOpen)
    //     return Results.BadRequest("Subforum not accepting new threads");

    // set the creation time before storing the thread
    thread.CreationTime = DateTime.Now;

    // threadDb.Threads.Add(thread);
    // await threadDb.SaveChangesAsync();

    // return Results.Created($"/threads/{thread.Id}", thread);
    return Results.InternalServerError();
});

// get a thread
app.MapGet("/threads/{id}", async (int id, ThreadDb threadDb) =>
{
    var thread = await threadDb.Threads.FindAsync(id);

    if(thread is not null)
        return Results.Ok(thread);
    else
        return Results.NotFound();
});

app.MapPut("/threads/{id}", async (int id, Thread inputThread, ThreadDb threadDb) =>
{
    var thread = await threadDb.Threads.FindAsync(id);

    if(thread is null)
        return Results.NotFound();

    thread.SubforumId = inputThread.SubforumId;
    thread.Title = inputThread.Title;
    thread.RepliesOpen = inputThread.RepliesOpen;

    await threadDb.SaveChangesAsync();

    return Results.NoContent();
});

// get replies for a thread
app.MapGet("/threads/{id}/replies", async (int id, ReplyDb replyDb) =>
    await replyDb.Replies
        .Where(reply =>
            reply.ParentThreadId == id)
        .ToArrayAsync());

// create a reply
app.MapPost("/replies", async (Reply reply, ReplyDb replyDb) =>
{
    replyDb.Replies.Add(reply);
    await replyDb.SaveChangesAsync();

    return Results.Created($"/replies/{reply.Id}", reply);
});

// get users
app.MapGet("/users", async (UserDb userDb) =>
    await userDb.Users.ToArrayAsync());

// get a user
app.MapGet("/users/{id}", async (int id, UserDb userDb) =>
{
    var user = await userDb.Users.FindAsync(id);

    if(user is not null)
        return Results.Ok(user);
    else
        return Results.NotFound();
});

// get threads created by a user
app.MapGet("/users/{id}/threads", async (int id, ThreadDb threadDb) =>
    await threadDb.Threads
        .Where(thread =>
            thread.UserId == id)
        .ToArrayAsync());                                                                

app.Run();
