
namespace Demo.DataAccess.Data.ModelsConfiguration
{
    class DepartmentConfiguration : BaseEntityConfiguration<Department>,IEntityTypeConfiguration<Department>
    {
        public new void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.Property(D => D.Id).UseIdentityColumn(seed:10,increment:10);
            builder.Property(D => D.Name).HasColumnType("varchar(20)");
            builder.Property(D => D.Code).HasColumnType("varchar(20)");
            builder.HasMany(D => D.Employees)
                   .WithOne(D => D.Department)
                   .HasForeignKey(D => D.DepartmentId)
                   .OnDelete(DeleteBehavior.SetNull);

            base.Configure(builder);
        }
    }
}
