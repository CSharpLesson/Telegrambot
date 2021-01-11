using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotForShaxrixon.Db;
using TelegramBotForShaxrixon.Model;

namespace TelegramBotForShaxrixon.Service
{
    public class ServicesssDoService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async static Task<List<Servicess>> GetAll()
        {
            try
            {
                return await new DataContext().Services.ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async static Task<Servicess> GetById(int Id)
        {
            try
            {
                return await new DataContext().Services.FirstOrDefaultAsync(f => f.Id == Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public static void AddOrUpdate(Servicess model)
        {
            try
            {
                var context = new DataContext();
                context.Services.Update(model);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
