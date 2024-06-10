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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private ApplicationDbContext _context;
        public OrderDetailRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(OrderDetail orderDetail)
        {
            _context.Update(orderDetail);
        }
    }
}
