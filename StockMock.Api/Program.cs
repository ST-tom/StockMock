using StockMock.Api;
using StockMock.Repository;
using StockMock.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. 

builder.AddRepositoryDependency();
builder.AddServiceDependency();
builder.AddApiDependency();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    //设置不限制content-type
    ServeUnknownFileTypes = true
});

app.UseRouting();

app.UseAuthorization();
//授权
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
