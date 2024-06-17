using Microsoft.EntityFrameworkCore;
using TTTN_ViettelStore.Models;

namespace TTTN_ViettelStore.Models
{
    public class MyDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            string strConnectionString = config.GetConnectionString("MyConnectionString").ToString();
            optionsBuilder.UseSqlServer(strConnectionString);
        }

        // Khai báo dòng dưới tương ứng vs table Users trong CSDL
        public DbSet<ItemAdmin> Admin { get; set; }
        public DbSet<ItemAdv> Adv {  get; set; }
        public DbSet<ItemCategories> Categories {  get; set; }
        public DbSet<ItemCategoriesProduct> CategoriesProduct { get; set; }
        public DbSet<ItemUsers> Users { get; set; }
        public DbSet<ItemNews> News { get; set; }
        public DbSet<ItemOrders> Orders { get; set; }
        public DbSet<ItemOrderDetail> OrderDetail { get; set; }
        public DbSet<ItemProducts> Products { get; set; }
        public DbSet<ItemRating> Rating { get; set; }
        public DbSet<ItemSlides> Slides { get; set; }
        public DbSet<ItemTags> Tags { get; set; }
        public DbSet<ItemTagsProducts> TagsProducts {  get; set; }
        public DbSet<ItemQtvWeb> QtvWebs { get; set; }
    }
}
