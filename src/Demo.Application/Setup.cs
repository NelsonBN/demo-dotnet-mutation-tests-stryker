using Demo.Application.UseCases;
using Demo.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Application;

public static class Setup
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        => services.AddTransient<AddProductUseCase>()
                   .AddTransient<DeleteProductUseCase>()
                   .AddTransient<GetProductsUseCase>()
                   .AddTransient<GetProductUseCase>()
                   .AddTransient<UpdateProductUseCase>();
}
