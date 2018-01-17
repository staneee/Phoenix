using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneBlog.Data.Mapping
{
    public abstract class BaseEntityMapping
    {
        public abstract void Execute(ModelBuilder builder);
    }

    public abstract class BaseEntityMapping<T> : BaseEntityMapping where T : class
    {
        public BaseEntityMapping()
        {

        }

        public override void Execute(ModelBuilder builder)
        {
            Execute(builder.Entity<T>());
        }

        public abstract void Execute(EntityTypeBuilder<T> builder);
    }
}
