﻿using MoreNote.Logic.DB;
using MoreNote.Logic.Entity;

using System;
using System.Linq;

namespace MoreNote.Logic.Service
{
    public class RandomImageService
    {
        public static async System.Threading.Tasks.Task InsertImageAsync(RandomImage randomImage)
        {
            using (DataContext db = new DataContext())
            {
                db.RandomImage.Add(randomImage);
                await db.SaveChangesAsync();
            }
        }
        public static RandomImage GetRandomImage(string type)
        {
            using (DataContext db = new DataContext())
            {
                int count = db.RandomImage.Where(b => b.TypeName.Equals(type) && b.Sex == false && b.Delete == false && b.Block == false).Count();
                if (count < 1)
                {
                    return null;
                }
                Random random = new Random();
                int num = random.Next(0, count);
                RandomImage result = db.RandomImage.Where(b => b.TypeName.Equals(type) && b.Sex == false && b.Delete == false && b.Block == false).Skip(num).FirstOrDefault();
                return result;
            }
        }
        public static bool Exist(string type, string fileSHA1)
        {
            using (DataContext db = new DataContext())
            {
                int count = db.RandomImage.Where(b => b.TypeName.Equals(type) && b.FileSHA1.Equals(fileSHA1)).Count();
                return count > 0;
            }
        }
        public static bool Exist(string fileSHA1)
        {
            using (DataContext db = new DataContext())
            {
                int count = db.RandomImage.Where(b => b.FileSHA1.Equals(fileSHA1)).Count();
                return count > 0;
            }
        }
    }
}
