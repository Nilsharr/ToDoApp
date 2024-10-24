using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDo.Domain.Entities;

namespace ToDo.Infrastructure.Data.Config;

public class ToDoItemConfiguration : IEntityTypeConfiguration<ToDoItem>
{
    public void Configure(EntityTypeBuilder<ToDoItem> builder)
    {
        // rest of the properties configured by convention
        builder.Property(x => x.CompletionPercentage).HasDefaultValue(0);
        builder.Property(x => x.CreatedOn).ValueGeneratedOnAdd();
    }
}