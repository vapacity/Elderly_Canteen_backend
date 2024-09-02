using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Elderly_Canteen.Data.Entities;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Administrator> Administrators { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<DeliverE> DeliverEs { get; set; }

    public virtual DbSet<DeliverEmployee> DeliverEmployees { get; set; }

    public virtual DbSet<DeliverOrder> DeliverOrders { get; set; }

    public virtual DbSet<DeliverReview> DeliverReviews { get; set; }

    public virtual DbSet<DeliverV> DeliverVs { get; set; }

    public virtual DbSet<Dish> Dishes { get; set; }

    public virtual DbSet<Donation> Donations { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Finance> Finances { get; set; }

    public virtual DbSet<Formula> Formulas { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<OrderInf> OrderInfs { get; set; }

    public virtual DbSet<OrderReview> OrderReviews { get; set; }

    public virtual DbSet<PayWage> PayWages { get; set; }

    public virtual DbSet<Repository> Repositories { get; set; }

    public virtual DbSet<Restock> Restocks { get; set; }

    public virtual DbSet<Senior> Seniors { get; set; }

    public virtual DbSet<SeniorInfo> SeniorInfos { get; set; }

    public virtual DbSet<VolApplication> VolApplications { get; set; }

    public virtual DbSet<VolReview> VolReviews { get; set; }

    public virtual DbSet<Volunteer> Volunteers { get; set; }

    public virtual DbSet<VolunteerInfo> VolunteerInfos { get; set; }

    public virtual DbSet<VwDeliverEmployeeDetail> VwDeliverEmployeeDetails { get; set; }

    public virtual DbSet<VwDeliverOrderDetail> VwDeliverOrderDetails { get; set; }

    public virtual DbSet<Weekmenu> Weekmenus { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("User Id=ELDERLY_CANTEEN;Password=CANTEEN_PASSWORD;Data Source=124.220.16.200:1521/xe");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ELDERLY_CANTEEN");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Accountid).HasName("SYS_C0010000");

            entity.ToTable("ACCOUNT");

            entity.HasIndex(e => e.Accountname, "IDX_ACCOUNTNAME");

            entity.HasIndex(e => e.Idcard, "UNIQUE_IDCARD").IsUnique();

            entity.HasIndex(e => e.Phonenum, "UNIQUE_PHONENUM").IsUnique();

            entity.Property(e => e.Accountid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ACCOUNTID");
            entity.Property(e => e.Accountname)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ACCOUNTNAME");
            entity.Property(e => e.Address)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.Birthdate)
                .HasColumnType("DATE")
                .HasColumnName("BIRTHDATE");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("GENDER");
            entity.Property(e => e.Idcard)
                .HasMaxLength(18)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("IDCARD");
            entity.Property(e => e.Identity)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("IDENTITY");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.Password)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Phonenum)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("PHONENUM");
            entity.Property(e => e.Portrait)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PORTRAIT");
            entity.Property(e => e.Verifycode)
                .HasPrecision(6)
                .HasColumnName("VERIFYCODE");
            entity.Property(e => e.Money)
                .HasColumnType("decimal(10, 2)") 
                .HasDefaultValue(100.00m)         
                .IsRequired(false);               


        });

        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("SYS_C0010078");

            entity.ToTable("ADMINISTRATOR");

            entity.HasIndex(e => e.AccountName, "IDX_ADMINISTRATOR_ACCOUNT_NAME");

            entity.HasIndex(e => e.PhoneNum, "IDX_ADMINISTRATOR_PHONE_NUM");

            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ACCOUNT_ID");
            entity.Property(e => e.AccountName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ACCOUNT_NAME");
            entity.Property(e => e.Address)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.BirthDate)
                .HasColumnType("DATE")
                .HasColumnName("BIRTH_DATE");
            entity.Property(e => e.Email)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("GENDER");
            entity.Property(e => e.IdCard)
                .HasMaxLength(18)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID_CARD");
            entity.Property(e => e.Identity)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("IDENTITY");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.Password)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.PhoneNum)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("PHONE_NUM");
            entity.Property(e => e.Portrait)
                .HasColumnType("BLOB")
                .HasColumnName("PORTRAIT");
            entity.Property(e => e.Position)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("POSITION");
            entity.Property(e => e.VerifyCode)
                .HasPrecision(6)
                .HasColumnName("VERIFY_CODE");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.ToTable("CART");

            entity.Property(e => e.CartId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CART_ID");
            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ACCOUNT_ID");
            entity.Property(e => e.CreatedTime)
                .HasColumnType("DATE")
                .HasColumnName("CREATED_TIME");
            entity.Property(e => e.UpdatedTime)
                .HasColumnType("DATE")
                .HasColumnName("UPDATED_TIME");

            entity.HasOne(d => d.Account).WithMany(p => p.Carts)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACCOUNT");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => new { e.CartId, e.DishId, e.Week });

            entity.ToTable("CART_ITEM");

            entity.Property(e => e.CartId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CART_ID");
            entity.Property(e => e.DishId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DISH_ID");
            entity.Property(e => e.Week)
                .HasColumnType("DATE")
                .HasColumnName("WEEK");
            entity.Property(e => e.Quantity)
                .HasPrecision(10)
                .HasColumnName("QUANTITY");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CART");

            entity.HasOne(d => d.Weekmenu).WithMany(p => p.CartItems)
                .HasForeignKey(d => new { d.DishId, d.Week })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DISH_WEEK");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CateId).HasName("SYS_C0010091");

            entity.ToTable("CATEGORY");

            entity.Property(e => e.CateId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CATE_ID");
            entity.Property(e => e.CateName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CATE_NAME");
        });

        modelBuilder.Entity<DeliverE>(entity =>
        {
            entity.HasKey(e => e.OrderId);

            entity.ToTable("DELIVER_E");

            entity.Property(e => e.OrderId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ORDER_ID");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EMPLOYEE_ID");

            entity.HasOne(d => d.Employee).WithMany(p => p.DeliverEs)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DELIVER_E_EMPLOYEE");

            entity.HasOne(d => d.Order).WithOne(p => p.DeliverE)
                .HasForeignKey<DeliverE>(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DELIVER_E_ORDER");
        });

        modelBuilder.Entity<DeliverEmployee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId);

            entity.ToTable("DELIVER_EMPLOYEE");

            entity.Property(e => e.EmployeeId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EMPLOYEE_ID");
            entity.Property(e => e.VolunteerId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("VOLUNTEER_ID");

            entity.HasOne(d => d.Volunteer).WithMany(p => p.DeliverEmployees)
                .HasForeignKey(d => d.VolunteerId)
                .HasConstraintName("FK_DELIVER_EMPLOYEE_VOLUNTEER");
        });

        modelBuilder.Entity<DeliverOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId);

            entity.ToTable("DELIVER_ORDER");

            entity.HasIndex(e => e.CusAddress, "IDX_DELIVER_ORDER_CUS_ADDRESS");

            entity.Property(e => e.OrderId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ORDER_ID");
            entity.Property(e => e.CartId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .ValueGeneratedOnAdd()
                .HasColumnName("CART_ID");
            entity.Property(e => e.CusAddress)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("CUS_ADDRESS");
            entity.Property(e => e.CustomerPhone)
                .HasMaxLength(11)
                .IsUnicode(false)
                .ValueGeneratedOnAdd()
                .HasColumnName("CUSTOMER_PHONE");
            entity.Property(e => e.DeliverPhone)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("DELIVER_PHONE");
            entity.Property(e => e.DeliverStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("DELIVER_STATUS");

            entity.HasOne(d => d.Cart).WithMany(p => p.DeliverOrders)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("FK_DELIVER_ORDER_CART");

            entity.HasOne(d => d.Order).WithOne(p => p.DeliverOrder)
                .HasForeignKey<DeliverOrder>(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DELIVER_ORDER_ORDER");
        });

        modelBuilder.Entity<DeliverReview>(entity =>
        {
            entity.HasKey(e => e.OrderId);

            entity.ToTable("DELIVER_REVIEW");

            entity.HasIndex(e => e.DStars, "IDX_DELIVER_REVIEW_STARS");

            entity.Property(e => e.OrderId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ORDER_ID");
            entity.Property(e => e.DReviewText)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("D_REVIEW_TEXT");
            entity.Property(e => e.DStars)
                .HasColumnType("NUMBER(3,1)")
                .HasColumnName("D_STARS");

            entity.HasOne(d => d.Order).WithOne(p => p.DeliverReview)
                .HasForeignKey<DeliverReview>(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DELIVER_REVIEW_ORDER");
        });

        modelBuilder.Entity<DeliverV>(entity =>
        {
            entity.HasKey(e => e.OrderId);

            entity.ToTable("DELIVER_V");

            entity.Property(e => e.OrderId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ORDER_ID");
            entity.Property(e => e.VolunteerId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("VOLUNTEER_ID");

            entity.HasOne(d => d.Order).WithOne(p => p.DeliverV)
                .HasForeignKey<DeliverV>(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DELIVER_V_ORDER");

            entity.HasOne(d => d.Volunteer).WithMany(p => p.DeliverVs)
                .HasForeignKey(d => d.VolunteerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DELIVER_V_VOLUNTEER");
        });

        modelBuilder.Entity<Dish>(entity =>
        {
            entity.HasKey(e => e.DishId).HasName("SYS_C0010100");

            entity.ToTable("DISH");

            entity.HasIndex(e => e.CateId, "IDX_CATE_ID");

            entity.HasIndex(e => e.DishName, "IDX_DISH_NAME");

            entity.Property(e => e.DishId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DISH_ID");
            entity.Property(e => e.CateId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CATE_ID");
            entity.Property(e => e.DishName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("DISH_NAME");
            entity.Property(e => e.Picture)
                .HasColumnType("BLOB")
                .HasColumnName("PICTURE");
            entity.Property(e => e.Price)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("PRICE");
            entity.Property(e => e.ImageUrl).HasColumnName("IMAGEURL").HasMaxLength(255);
            entity.HasOne(d => d.Cate).WithMany(p => p.Dishes)
                .HasForeignKey(d => d.CateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CATE_ID");
        });

        modelBuilder.Entity<Donation>(entity =>
        {
            entity.ToTable("DONATION");

            entity.Property(e => e.DonationId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DONATION_ID");
            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ACCOUNT_ID");
            entity.Property(e => e.FinanceId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("FINANCE_ID");
            entity.Property(e => e.Origin)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ORIGIN");

            entity.HasOne(d => d.Account).WithMany(p => p.Donations)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DONATION_ACCOUNT");

            entity.HasOne(d => d.Finance).WithMany(p => p.Donations)
                .HasForeignKey(d => d.FinanceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DONATION_FINANCE");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("EMPLOYEE");

            entity.HasIndex(e => e.PhoneNum, "IDX_EMPLOYEE_PHONE_NUM");

            entity.Property(e => e.EmployeeId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EMPLOYEE_ID");
            entity.Property(e => e.Address)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("EMPLOYEE_NAME");
            entity.Property(e => e.EmployeePosition)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("EMPLOYEE_POSITION");
            entity.Property(e => e.IdCard)
                .HasMaxLength(18)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID_CARD");
            entity.Property(e => e.Ispaidthismonth)
                .HasColumnType("NUMBER(1)")
                .HasColumnName("ISPAIDTHISMONTH");
            entity.Property(e => e.PhoneNum)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("PHONE_NUM");
            entity.Property(e => e.Salary)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("SALARY");
        });

        modelBuilder.Entity<Finance>(entity =>
        {
            entity.ToTable("FINANCE");

            entity.HasIndex(e => e.AccountId, "IDX_FINANCE_ACCOUNT_ID");

            entity.HasIndex(e => e.AdministratorId, "IDX_FINANCE_ADMINISTRATOR_ID");

            entity.HasIndex(e => e.FinanceDate, "IDX_FINANCE_DATE");

            entity.Property(e => e.FinanceId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("FINANCE_ID");
            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ACCOUNT_ID");
            entity.Property(e => e.AdministratorId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ADMINISTRATOR_ID");
            entity.Property(e => e.FinanceDate)
                .HasColumnType("DATE")
                .HasColumnName("FINANCE_DATE");
            entity.Property(e => e.FinanceType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("FINANCE_TYPE");
            entity.Property(e => e.InOrOut)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("IN_OR_OUT");
            entity.Property(e => e.Price)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("PRICE");
            entity.Property(e => e.Proof)
                .HasColumnType("BLOB")
                .HasColumnName("PROOF");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("STATUS");

            entity.HasOne(d => d.Account).WithMany(p => p.Finances)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FINANCE_ACCOUNT");

            entity.HasOne(d => d.Administrator).WithMany(p => p.Finances)
                .HasForeignKey(d => d.AdministratorId)
                .HasConstraintName("FK_FINANCE_ADMINISTRATOR");
        });

        modelBuilder.Entity<Formula>(entity =>
        {
            entity.HasKey(e => new { e.DishId, e.IngredientId });

            entity.ToTable("FORMULA");

            entity.Property(e => e.DishId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DISH_ID");
            entity.Property(e => e.IngredientId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("INGREDIENT_ID");
            entity.Property(e => e.Amount)
                .HasPrecision(5)
                .HasColumnName("AMOUNT");

            entity.HasOne(d => d.Dish).WithMany(p => p.Formulas)
                .HasForeignKey(d => d.DishId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FORMULA_DISH");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.Formulas)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FORMULA_INGREDIENT");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.ToTable("INGREDIENT");

            entity.HasIndex(e => e.IngredientName, "UQ_INGREDIENT_NAME").IsUnique();

            entity.Property(e => e.IngredientId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("INGREDIENT_ID");
            entity.Property(e => e.IngredientName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("INGREDIENT_NAME");
        });

        modelBuilder.Entity<OrderInf>(entity =>
        {
            entity.HasKey(e => e.OrderId);

            entity.ToTable("ORDER_INF");

            entity.HasIndex(e => e.Bonus, "IDX_ORDER_INF_BONUS");

            entity.HasIndex(e => e.Status, "IDX_ORDER_INF_STATUS");

            entity.Property(e => e.OrderId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ORDER_ID");
            entity.Property(e => e.Bonus)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("BONUS");
            entity.Property(e => e.CartId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CART_ID");
            entity.Property(e => e.DeliverOrDining)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("DELIVER_OR_DINING");
            entity.Property(e => e.FinanceId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("FINANCE_ID");
            entity.Property(e => e.Remark)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("REMARK");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STATUS");

            entity.HasOne(d => d.Cart).WithMany(p => p.OrderInfs)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ORDER_INF_CART");

            entity.HasOne(d => d.Finance).WithMany(p => p.OrderInfs)
                .HasForeignKey(d => d.FinanceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ORDER_INF_FINANCE");
        });

        modelBuilder.Entity<OrderReview>(entity =>
        {
            entity.HasKey(e => e.OrderId);

            entity.ToTable("ORDER_REVIEW");

            entity.HasIndex(e => e.CStars, "IDX_ORDER_REVIEW_STARS");

            entity.Property(e => e.OrderId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ORDER_ID");
            entity.Property(e => e.CReviewText)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("C_REVIEW_TEXT");
            entity.Property(e => e.CStars)
                .HasColumnType("NUMBER(3,1)")
                .HasColumnName("C_STARS");

            entity.HasOne(d => d.Order).WithOne(p => p.OrderReview)
                .HasForeignKey<OrderReview>(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ORDER_REVIEW_ORDER");
        });

        modelBuilder.Entity<PayWage>(entity =>
        {
            entity.HasKey(e => new { e.FinanceId, e.EmployeeId });

            entity.ToTable("PAY_WAGE");

            entity.Property(e => e.FinanceId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("FINANCE_ID");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EMPLOYEE_ID");
            entity.Property(e => e.AdministratorId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ADMINISTRATOR_ID");

            entity.HasOne(d => d.Administrator).WithMany(p => p.PayWages)
                .HasForeignKey(d => d.AdministratorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PAY_WAGE_ADMINISTRATOR");

            entity.HasOne(d => d.Employee).WithMany(p => p.PayWages)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PAY_WAGE_EMPLOYEE");

            entity.HasOne(d => d.Finance).WithMany(p => p.PayWages)
                .HasForeignKey(d => d.FinanceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PAY_WAGE_FINANCE");
        });

        modelBuilder.Entity<Repository>(entity =>
        {
            entity.HasKey(e => new { e.IngredientId, e.ExpirationTime });

            entity.ToTable("REPOSITORY");

            entity.HasIndex(e => e.ExpirationTime, "IDX_REPOSITORY_EXPIRATION");

            entity.Property(e => e.IngredientId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("INGREDIENT_ID");
            entity.Property(e => e.ExpirationTime)
                .HasColumnType("DATE")
                .HasColumnName("EXPIRATION_TIME");
            entity.Property(e => e.HighConsumption)
                .HasPrecision(2)
                .HasColumnName("HIGH_CONSUMPTION");
            entity.Property(e => e.RemainAmount)
                .HasPrecision(10)
                .HasColumnName("REMAIN_AMOUNT");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.Repositories)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_REPOSITORY_INGREDIENT");
        });

        modelBuilder.Entity<Restock>(entity =>
        {
            entity.HasKey(e => new { e.FinanceId, e.IngredientId });

            entity.ToTable("RESTOCK");

            entity.HasIndex(e => e.AdministratorId, "IDX_RESTOCK_ADMINISTRATOR_ID");

            entity.Property(e => e.FinanceId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("FINANCE_ID");
            entity.Property(e => e.IngredientId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("INGREDIENT_ID");
            entity.Property(e => e.AdministratorId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ADMINISTRATOR_ID");
            entity.Property(e => e.Price)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("PRICE");
            entity.Property(e => e.Quantity)
                .HasPrecision(10)
                .HasColumnName("QUANTITY");

            entity.HasOne(d => d.Administrator).WithMany(p => p.Restocks)
                .HasForeignKey(d => d.AdministratorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RESTOCK_ADMINISTRATOR");

            entity.HasOne(d => d.Finance).WithMany(p => p.Restocks)
                .HasForeignKey(d => d.FinanceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RESTOCK_FINANCE");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.Restocks)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RESTOCK_INGREDIENT");
        });

        modelBuilder.Entity<Senior>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("SYS_C0010089");

            entity.ToTable("SENIOR");

            entity.HasIndex(e => e.FamilyNum, "IDX_SENIOR_FAMILY_NUM");

            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ACCOUNT_ID");
            entity.Property(e => e.FamilyNum)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("FAMILY_NUM");
            entity.Property(e => e.Subsidy)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("SUBSIDY");

            entity.HasOne(d => d.Account).WithOne(p => p.Senior)
                .HasForeignKey<Senior>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SENIOR_ACCOUNT");
        });

        modelBuilder.Entity<SeniorInfo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("SENIOR_INFO");

            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ACCOUNT_ID");
            entity.Property(e => e.Accountname)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ACCOUNTNAME");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.Birthdate)
                .HasColumnType("DATE")
                .HasColumnName("BIRTHDATE");
            entity.Property(e => e.FamilyNum)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("FAMILY_NUM");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("GENDER");
            entity.Property(e => e.Idcard)
                .HasMaxLength(18)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("IDCARD");
            entity.Property(e => e.Identity)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("IDENTITY");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.Password)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Phonenum)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("PHONENUM");
            entity.Property(e => e.Portrait)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("PORTRAIT");
            entity.Property(e => e.Subsidy)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("SUBSIDY");
            entity.Property(e => e.Verifycode)
                .HasPrecision(6)
                .HasColumnName("VERIFYCODE");
        });

        modelBuilder.Entity<VolApplication>(entity =>
        {
            entity.HasKey(e => e.ApplicationId);

            entity.ToTable("VOL_APPLICATION");

            entity.Property(e => e.ApplicationId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("APPLICATION_ID");
            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ACCOUNT_ID");
            entity.Property(e => e.ApplicationDate)
                .HasColumnType("DATE")
                .HasColumnName("APPLICATION_DATE");
            entity.Property(e => e.SelfStatement)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("SELF_STATEMENT");

            entity.HasOne(d => d.Account).WithMany(p => p.VolApplications)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VOL_APPLICATION_ACCOUNT");
        });

        modelBuilder.Entity<VolReview>(entity =>
        {
            entity.HasKey(e => e.ApplicationId);

            entity.ToTable("VOL_REVIEW");

            entity.Property(e => e.ApplicationId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("APPLICATION_ID");
            entity.Property(e => e.AdministratorId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ADMINISTRATOR_ID");
            entity.Property(e => e.Reason)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("REASON");
            entity.Property(e => e.ReviewDate)
                .HasColumnType("DATE")
                .HasColumnName("REVIEW_DATE");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("STATUS");

            entity.HasOne(d => d.Administrator).WithMany(p => p.VolReviews)
                .HasForeignKey(d => d.AdministratorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VOL_REVIEW_ADMINISTRATOR");

            entity.HasOne(d => d.Application).WithOne(p => p.VolReview)
                .HasForeignKey<VolReview>(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VOL_REVIEW_VOL_APPLICATION");
        });

        modelBuilder.Entity<Volunteer>(entity =>
        {
            entity.HasKey(e => e.AccountId);

            entity.ToTable("VOLUNTEER");

            entity.HasIndex(e => e.Available, "IDX_VOLUNTEER_AVAILABLE");

            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ACCOUNT_ID");
            entity.Property(e => e.Available)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("AVAILABLE");
            entity.Property(e => e.Score)
                .HasColumnType("NUMBER(3,1)")
                .HasColumnName("SCORE");

            entity.HasOne(d => d.Account).WithOne(p => p.Volunteer)
                .HasForeignKey<Volunteer>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VOLUNTEER_ACCOUNT");
        });

        modelBuilder.Entity<VolunteerInfo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VOLUNTEER_INFO");

            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ACCOUNT_ID");
            entity.Property(e => e.Accountname)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ACCOUNTNAME");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.Available)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("AVAILABLE");
            entity.Property(e => e.Birthdate)
                .HasColumnType("DATE")
                .HasColumnName("BIRTHDATE");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("GENDER");
            entity.Property(e => e.Idcard)
                .HasMaxLength(18)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("IDCARD");
            entity.Property(e => e.Identity)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("IDENTITY");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.Password)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Phonenum)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("PHONENUM");
            entity.Property(e => e.Portrait)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("PORTRAIT");
            entity.Property(e => e.Score)
                .HasColumnType("NUMBER(3,1)")
                .HasColumnName("SCORE");
            entity.Property(e => e.Verifycode)
                .HasPrecision(6)
                .HasColumnName("VERIFYCODE");
        });

        modelBuilder.Entity<VwDeliverEmployeeDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_DELIVER_EMPLOYEE_DETAILS");

            entity.Property(e => e.Address)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EMPLOYEE_ID");
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("EMPLOYEE_NAME");
            entity.Property(e => e.EmployeePosition)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("EMPLOYEE_POSITION");
            entity.Property(e => e.IdCard)
                .HasMaxLength(18)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID_CARD");
            entity.Property(e => e.PhoneNum)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("PHONE_NUM");
            entity.Property(e => e.Salary)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("SALARY");
            entity.Property(e => e.VolunteerId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("VOLUNTEER_ID");
        });

        modelBuilder.Entity<VwDeliverOrderDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_DELIVER_ORDER_DETAILS");

            entity.Property(e => e.Bonus)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("BONUS");
            entity.Property(e => e.CartId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CART_ID");
            entity.Property(e => e.CusAddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUS_ADDRESS");
            entity.Property(e => e.CustomerPhone)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("CUSTOMER_PHONE");
            entity.Property(e => e.DeliverOrDining)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("DELIVER_OR_DINING");
            entity.Property(e => e.DeliverPhone)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("DELIVER_PHONE");
            entity.Property(e => e.DeliverStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("DELIVER_STATUS");
            entity.Property(e => e.FinanceId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("FINANCE_ID");
            entity.Property(e => e.OrderId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ORDER_ID");
            entity.Property(e => e.Remark)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("REMARK");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STATUS");
        });

        modelBuilder.Entity<Weekmenu>(entity =>
        {
            entity.HasKey(e => new { e.DishId, e.Week });

            entity.ToTable("WEEKMENU");

            entity.HasIndex(e => e.DishId, "IDX_WEEKMENU_DISH_ID");

            entity.HasIndex(e => e.DisPrice, "IDX_WEEKMENU_DIS_PRICE");

            entity.HasIndex(e => e.Week, "IDX_WEEKMENU_WEEK");

            entity.Property(e => e.DishId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DISH_ID");
            entity.Property(e => e.Week)
                .HasColumnType("DATE")
                .HasColumnName("WEEK");
            entity.Property(e => e.DisPrice)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("DIS_PRICE");
            entity.Property(e => e.Sales)
                .HasPrecision(10)
                .HasColumnName("SALES");
            entity.Property(e => e.Stock)
                .HasPrecision(10)
                .HasColumnName("STOCK");
            entity.Property(e => e.Day)
                .HasMaxLength(10)
                .HasColumnName("DAY");
        });
        modelBuilder.HasSequence("EMPLOYEEID_SEQ");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
