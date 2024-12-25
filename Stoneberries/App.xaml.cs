using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Stoneberries
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlite("Data Source=StoneBerries.db"));

            services.AddSingleton<MainWindow>();
            
            services.AddSingleton<CurrentUserService>();
            
            services.AddTransient<CartService>();

            ServiceProvider = services.BuildServiceProvider();


            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
