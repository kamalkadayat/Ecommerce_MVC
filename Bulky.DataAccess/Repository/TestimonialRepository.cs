﻿using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class TestimonialRepository : Repository<Testimonial>, ITestimonialRepository
    {
        private ApplicationDbContext _db;
        public TestimonialRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Testimonial obj)
        {
            _db.Testimonials.Update(obj);
        }
    }
}
