
namespace Demo.DataAccess.Data.ModelsConfiguration
{
    public class EmployeeConfiguration :BaseEntityConfiguration<Employee>, IEntityTypeConfiguration<Employee>
    {
        public new void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.Property(E => E.Name).HasColumnType("varchar(50)");
            builder.Property(E => E.Address).HasColumnType("varchar(150)");
            builder.Property(E => E.Salary).HasColumnType("decimal(10,2)");
            builder.Property(E => E.Gender)
                .HasConversion((EmpGender) => EmpGender.ToString(),
                    (gender) => (Gender)Enum.Parse(typeof(Gender), gender));

            builder.Property(E => E.EmployeeType)
                .HasConversion((EmpType) => EmpType.ToString(),
                    (type) => (EmployeeType)Enum.Parse(typeof(EmployeeType), type));
            base.Configure(builder);
        }
    }
}
