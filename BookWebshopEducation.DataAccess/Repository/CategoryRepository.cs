using BookWebshopEducation.DataAccess.Data;
using BookWebshopEducation.DataAccess.Repository.IRepository;
using BookWebshopEducation.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookWebshopEducation.DataAccess.Repository
{
    public  class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Category category)
        {
                _context.Update(category);
        }
    }
}
