﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Alithea3.Controllers.Respository;
using Alithea3.Models;

namespace Alithea3.Controllers.Service
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        protected BaseRepository<T> _baseRepo;

        public BaseService(BaseRepository<T> baseRepo)
        {
            _baseRepo = baseRepo;
        }

        public BaseService()
        {
            _baseRepo = new BaseRepository<T>();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _baseRepo.GetAll();
        }

        public T SelectById(object id)
        {
            try
            {
                return _baseRepo.SelectById(id);
            }
            catch
            {
                return null;
            }
        }

        public virtual void Insert(T obj)
        {
            _baseRepo.Insert(obj);
        }

        public virtual void Update(T obj)
        {
            _baseRepo.Update(obj);
        }

        public virtual void Delete(object id)
        {
            _baseRepo.Delete(id);
        }

        public void Save()
        {
            _baseRepo.Save();
        }

    }
}