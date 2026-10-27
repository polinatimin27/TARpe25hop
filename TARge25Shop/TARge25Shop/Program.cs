
using Microsoft.EntityFrameworkCore;
using TARge25Shop.ApplicationServices.Services;
using TARge25Shop.Core.ServiceInterface;
using TARge25Shop.Data;

namespace TARge25Shop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<ISpaceshipServices, SpaceshipServices>();
            builder.Services.AddScoped<IFileServices, FileServices>();
            builder.Services.AddScoped<IRealEstateServices, RealEstateServices>();
            builder.Services.AddScoped<IKindergartenServices, KindergartenServices>();

            // Raamatukogu services
            builder.Services.AddScoped<IBookServices, BookServices>();
            builder.Services.AddScoped<IReaderServices, ReaderServices>();
            builder.Services.AddScoped<ILoanServices, LoanServices>();

            // Database connection from appsettings.json
            builder.Services.AddDbContext<TARge25ShopContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
