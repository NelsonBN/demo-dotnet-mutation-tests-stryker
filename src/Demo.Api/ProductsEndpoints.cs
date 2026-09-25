using Demo.Application.DTOs;
using Demo.Application.UseCases;
using Demo.Domain;
using Demo.UseCases;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading;

namespace Demo.Infrastructure;

public static class ProductsEndpoints
{
    public static void MapProductsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/products", async (GetProductsUseCase userCase, CancellationToken cancellationToken) =>
        {
            var products = await userCase.ExecuteAsync(cancellationToken);
            return Results.Ok(products);
        });


        endpoints.MapGet("/products/low-stock", async (GetLowStockProductsUseCase userCase, CancellationToken cancellationToken, uint threshold = 10) =>
        {
            var products = await userCase.ExecuteAsync(threshold, cancellationToken);
            return Results.Ok(products);
        });


        endpoints.MapGet("/products/{id:guid}", async (GetProductUseCase userCase, Guid id, CancellationToken cancellationToken) =>
        {
            try
            {
                var product = await userCase.ExecuteAsync(id, cancellationToken);
                return Results.Ok(product);
            }
            catch (NotFoundException)
            {
                return Results.NotFound();
            }
        }).WithName("GetProduct");


        endpoints.MapPost("/products", async (AddProductUseCase userCase, ProductRequest request, CancellationToken cancellationToken) =>
        {
            try
            {
                var product = await userCase.ExecuteAsync(request, cancellationToken);

                return Results.CreatedAtRoute(
                    "GetProduct",
                    new { product.Id },
                    product);
            }
            catch (DuplicateException)
            {
                return Results.Conflict();
            }
            catch (InvalidException)
            {
                return Results.BadRequest();
            }
        });


        endpoints.MapPut("/products/{id}", async (UpdateProductUseCase userCase, Guid id, ProductRequest request, CancellationToken cancellationToken) =>
        {
            try
            {
                await userCase.ExecuteAsync(id, request, cancellationToken);
                return Results.NoContent();
            }
            catch (DuplicateException)
            {
                return Results.Conflict();
            }
            catch (NotFoundException)
            {
                return Results.NotFound();
            }
            catch (InvalidException)
            {
                return Results.BadRequest();
            }
        });


        endpoints.MapDelete("/products/{id}", async (DeleteProductUseCase userCase, Guid id, CancellationToken cancellationToken) =>
        {
            try
            {
                await userCase.ExecuteAsync(id, cancellationToken);
                return Results.NoContent();
            }
            catch (NotFoundException)
            {
                return Results.NotFound();
            }
        });


        endpoints.MapPost("/products/{id:guid}/restock", async (RestockProductUseCase userCase, Guid id, uint amount, CancellationToken cancellationToken) =>
        {
            try
            {
                var product = await userCase.ExecuteAsync(id, amount, cancellationToken);
                return Results.Ok(product);
            }
            catch (NotFoundException)
            {
                return Results.NotFound();
            }
            catch (InvalidException)
            {
                return Results.BadRequest();
            }
        });
    }
}
