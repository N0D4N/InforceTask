using InforceTask;
using InforceTask.DataAccess.DbMigrator;

var builder = WebApplication.CreateBuilder(args);
Startup.ConfigureServices(builder);

var app = builder.Build();
Startup.Configure(app);

await DbMigrator.MigrateDbAsync(app.Services);

app.Run();