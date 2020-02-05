using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Alithea3.Models;

namespace Alithea3.Controllers.Respository.CategoryManager
{
    public class CategoryRepository : BaseRepository<Category>
    {
        public List<Category> listPagination(int page, int limit)
        {

            List<Category> list = new List<Category>();
            try
            {
                list = _table.OrderByDescending(o => o.UpdatedAt).Skip(page).Take(limit).ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return list;
        }
    }
}