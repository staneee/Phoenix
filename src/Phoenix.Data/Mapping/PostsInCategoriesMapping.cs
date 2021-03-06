﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phoenix.Data.Mapping
{

    public class PostsInCategoriesMapping : BaseEntityMapping<PostsInCategories>
    {

        public override void Execute(EntityTypeBuilder<PostsInCategories> builder)
        {
            builder.HasKey(t => new { t.PostsId, t.CategoriesId });

            builder.HasOne(pt => pt.Categories)
                .WithMany(p => p.PostsInCategories)
                .HasForeignKey(pt => pt.CategoriesId);

            builder.HasOne(pt => pt.Posts)
                .WithMany(t => t.PostsInCategories)
                .HasForeignKey(pt => pt.PostsId);
        }
    }


}
