using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SitemapTest.Models;
using SimpleMvcSitemap;

namespace SitemapTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        //home/item
        //home/product

        // GET: api/Sitemap
        public ActionResult Product(int? id)
        {
            var dataSource = CreateProducts(50000).ToList().AsQueryable();
            var productSitemapIndexConfiguration = new ProductSitemapIndexConfiguration(dataSource, id, Url);
            return new DynamicSitemapIndexProvider().CreateSitemapIndex(new SitemapProvider(), productSitemapIndexConfiguration);
        }

        public ActionResult Item(int? id)
        {
            var dataSource = CreateItems(50000).ToList().AsQueryable();
            var itemSitemapIndexConfiguration = new ItemSitemapIndexConfiguration(dataSource, id, Url);
            return new DynamicSitemapIndexProvider().CreateSitemapIndex(new SitemapProvider(), itemSitemapIndexConfiguration);
        }
        private IEnumerable<Product> CreateProducts(int count)
        {
            return Enumerable.Range(1, count).Select(i => new Product { Id = i });
        }
        private IEnumerable<Item> CreateItems(int count)
        {
            return Enumerable.Range(1, count).Select(i => new Item { Id = i });
        }

        public class ProductSitemapIndexConfiguration : SitemapIndexConfiguration<Product>
        {
            private readonly IUrlHelper urlHelper;

            public ProductSitemapIndexConfiguration(IQueryable<Product> dataSource, int? currentPage, IUrlHelper urlHelper)
                : base(dataSource, currentPage)
            {
                this.urlHelper = urlHelper;
                Size = 50000;

            }

            public override SitemapIndexNode CreateSitemapIndexNode(int currentPage)
            {
                return new SitemapIndexNode(urlHelper.Action("Product", "Sitemap", new { id = currentPage }));
            }

            public override SitemapNode CreateNode(Product source)
            {
                return new SitemapNode(urlHelper.Page("/Content", new { id = source.Id }))
                {
                    ChangeFrequency = ChangeFrequency.Daily,
                    LastModificationDate = source.LastUpdate,
                    Priority = 1,
                };
            }
        }


        public class ItemSitemapIndexConfiguration : SitemapIndexConfiguration<Item>
        {
            private readonly IUrlHelper urlHelper;

            public ItemSitemapIndexConfiguration(IQueryable<Item> dataSource, int? currentPage, IUrlHelper urlHelper)
                : base(dataSource, currentPage)
            {
                this.urlHelper = urlHelper;
                Size = 50000;
            }

            public override SitemapIndexNode CreateSitemapIndexNode(int currentPage)
            {
                return new SitemapIndexNode(urlHelper.Action("Item", "Sitemap", new { id = currentPage }));
            }

            public override SitemapNode CreateNode(Item source)
            {
                return new SitemapNode(urlHelper.Page("/Item", new { id = source.Id }))
                {
                    ChangeFrequency = ChangeFrequency.Monthly,
                    LastModificationDate = source.Datetime,
                    Priority = 0.8M,
                };
            }

        
        }

    }

    public class Product
    {
        public int Id { get; set; }
        public DateTime? LastUpdate { get; internal set; }
    }
    public class Item
    {
        public int Id { get; set; }
        public DateTime? Datetime { get; internal set; }
    }
}
