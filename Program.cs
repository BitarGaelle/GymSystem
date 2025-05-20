var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//to redirect root URL to your Home page
app.MapGet("/", context =>
{
    context.Response.Redirect("/HomePage/Home");
    return Task.CompletedTask;
});

app.MapGet("/ViewMembership", context =>
{
    context.Response.Redirect("/Pages/ViewMyMembership");
    return Task.CompletedTask;
});

app.MapRazorPages();

app.Run();