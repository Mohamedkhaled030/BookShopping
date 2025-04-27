using BookShopping.Infrustructure.Abstruct;
using BookShopping.Infrustructure.Repositoreis;
using BookShopping.Infrustructure.Service.Abstruct;
using BookShopping.Infrustructure.Service.ServiceRepsitry;

namespace BookShopping.Infrustructure
{
    public static class ModuleInfrustructureDependencies
    {
        public static IServiceCollection moduleInfrustructureDependencies(this IServiceCollection services)
        {
            services.AddTransient<IHomeRepository, HomeRepository>();
            services.AddTransient<ICartRepository, CartRepository>();
            services.AddTransient<IUserOrderRepository, UserOrderRepository>();
            services.AddTransient<IStockRepository, StockRepository>();
            services.AddTransient<IGenreRepository, GenreRepository>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IBookRepository, BookRepository>();
            //services.AddTransient<IEmailSender, FakeEmailSender>();
            return services;
        }
    }
}
