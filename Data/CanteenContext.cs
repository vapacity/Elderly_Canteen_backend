using System;
using System.Collections.Generic;
using Elderly_Canteen.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Elderly_Canteen.Data;

public partial class CanteenContext : DbContext
{
    public CanteenContext()
    {
    }

    public CanteenContext(DbContextOptions<CanteenContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Administrator> Administrators { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<DeliverEmployee> DeliverEmployees { get; set; }

    public virtual DbSet<DeliverOrder> DeliverOrders { get; set; }

    public virtual DbSet<DeliverReview> DeliverReviews { get; set; }

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

    public virtual DbSet<VolApplication> VolApplications { get; set; }

    public virtual DbSet<VolReview> VolReviews { get; set; }

    public virtual DbSet<Volunteer> Volunteers { get; set; }

    public virtual DbSet<Weekmenu> Weekmenus { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseOracle("User Id=ELDERLY_CANTEEN;Password=CANTEEN_PASSWORD;Data Source=124.220.16.200:1521/xe");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("SYS_C007682");

            entity.ToTable("ACCOUNT");

            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ACCOUNT_ID");
            entity.Property(e => e.AccountName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ACCOUNT_NAME");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.BirthDate)
                .HasColumnType("DATE")
                .HasColumnName("BIRTH_DATE");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
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
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("PORTRAIT");
            entity.Property(e => e.VerifyCode)
                .HasPrecision(6)
                .HasColumnName("VERIFY_CODE");
        });

        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("SYS_C007683");

            entity.ToTable("ADMINISTRATOR");

            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ACCOUNT_ID");
            entity.Property(e => e.Email)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Position)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("POSITION");

            entity.HasOne(d => d.Account).WithOne(p => p.Administrator)
                .HasForeignKey<Administrator>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SYS_C007684");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("SYS_C007715");

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
                .HasConstraintName("SYS_C007716");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => new { e.CartId, e.DishId, e.Week }).HasName("SYS_C007721");

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
                .HasConstraintName("SYS_C007722");

            entity.HasOne(d => d.Dish).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.DishId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SYS_C007723");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CateId).HasName("SYS_C007688");

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

        modelBuilder.Entity<DeliverEmployee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("SYS_C007741");

            entity.ToTable("DELIVER_EMPLOYEE");

            entity.Property(e => e.EmployeeId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EMPLOYEE_ID");
            entity.Property(e => e.VolunteerId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("VOLUNTEER_ID");

            entity.HasOne(d => d.Employee).WithOne(p => p.DeliverEmployee)
                .HasForeignKey<DeliverEmployee>(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SYS_C007742");

            entity.HasOne(d => d.Volunteer).WithMany(p => p.DeliverEmployees)
                .HasForeignKey(d => d.VolunteerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SYS_C007743");
        });

        modelBuilder.Entity<DeliverOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("SYS_C007731");

            entity.ToTable("DELIVER_ORDER");

            entity.Property(e => e.OrderId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ORDER_ID");
            entity.Property(e => e.CusAddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUS_ADDRESS");
            entity.Property(e => e.CustomerPhone)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("CUSTOMER_PHONE");
            entity.Property(e => e.DeliverPhone)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("DELIVER_PHONE");
            entity.Property(e => e.DeliverStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("DELIVER_STATUS");

            entity.HasOne(d => d.Order).WithOne(p => p.DeliverOrder)
                .HasForeignKey<DeliverOrder>(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SYS_C007732");
        });

        modelBuilder.Entity<DeliverReview>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("SYS_C007760");

            entity.ToTable("DELIVER_REVIEW");

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
                .HasConstraintName("SYS_C007761");
        });

        modelBuilder.Entity<Dish>(entity =>
        {
            entity.HasKey(e => e.DishId).HasName("SYS_C007692");

            entity.ToTable("DISH");

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
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("PICTURE");
            entity.Property(e => e.Price)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("PRICE");

            entity.HasOne(d => d.Cate).WithMany(p => p.Dishes)
                .HasForeignKey(d => d.CateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SYS_C007693");
        });

        modelBuilder.Entity<Donation>(entity =>
        {
            entity.HasKey(e => e.FinanceId).HasName("SYS_C007696");

            entity.ToTable("DONATION");

            entity.Property(e => e.FinanceId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("FINANCE_ID");
            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ACCOUNT_ID");
            entity.Property(e => e.Origin)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ORIGIN");

            entity.HasOne(d => d.Account).WithMany(p => p.Donations)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SYS_C007697");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("SYS_C007739");

            entity.ToTable("EMPLOYEE");

            entity.Property(e => e.EmployeeId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EMPLOYEE_ID");
            entity.Property(e => e.Address)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.IdCard)
                .HasMaxLength(18)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID_CARD");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.PhoneNum)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("PHONE_NUM");
            entity.Property(e => e.Position)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("POSITION");
            entity.Property(e => e.Salary)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("SALARY");
        });

        modelBuilder.Entity<Finance>(entity =>
        {
            entity.HasKey(e => e.FinanceId).HasName("SYS_C007705");

            entity.ToTable("FINANCE");

            entity.Property(e => e.FinanceId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("FINANCE_ID");
            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ACCOUNT_ID");
            entity.Property(e => e.AdministraterId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ADMINISTRATER_ID");
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
                .HasConstraintName("SYS_C007706");

            entity.HasOne(d => d.Administrater).WithMany(p => p.Finances)
                .HasForeignKey(d => d.AdministraterId)
                .HasConstraintName("SYS_C007707");
        });

        modelBuilder.Entity<Formula>(entity =>
        {
            entity.HasKey(e => new { e.DishId, e.IngredientId }).HasName("SYS_C007778");

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
                .HasConstraintName("SYS_C007779");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.Formulas)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SYS_C007780");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.IngredientId).HasName("SYS_C007774");

            entity.ToTable("INGREDIENT");

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
            entity.HasKey(e => e.OrderId).HasName("SYS_C007726");

            entity.ToTable("ORDER_INF");

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
                .HasConstraintName("SYS_C007728");

            entity.HasOne(d => d.Finance).WithMany(p => p.OrderInfs)
                .HasForeignKey(d => d.FinanceId)
                .HasConstraintName("SYS_C007727");

            entity.HasMany(d => d.Employees).WithMany(p => p.Orders)
                .UsingEntity<Dictionary<string, object>>(
                    "DeliverE",
                    r => r.HasOne<Employee>().WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("SYS_C007748"),
                    l => l.HasOne<OrderInf>().WithMany()
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("SYS_C007747"),
                    j =>
                    {
                        j.HasKey("OrderId", "EmployeeId").HasName("SYS_C007746");
                        j.ToTable("DELIVER_E");
                        j.IndexerProperty<string>("OrderId")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("ORDER_ID");
                        j.IndexerProperty<string>("EmployeeId")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("EMPLOYEE_ID");
                    });

            entity.HasMany(d => d.Volunteers).WithMany(p => p.Orders)
                .UsingEntity<Dictionary<string, object>>(
                    "DeliverV",
                    r => r.HasOne<Volunteer>().WithMany()
                        .HasForeignKey("VolunteerId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("SYS_C007753"),
                    l => l.HasOne<OrderInf>().WithMany()
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("SYS_C007752"),
                    j =>
                    {
                        j.HasKey("OrderId", "VolunteerId").HasName("SYS_C007751");
                        j.ToTable("DELIVER_V");
                        j.IndexerProperty<string>("OrderId")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("ORDER_ID");
                        j.IndexerProperty<string>("VolunteerId")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("VOLUNTEER_ID");
                    });
        });

        modelBuilder.Entity<OrderReview>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("SYS_C007756");

            entity.ToTable("ORDER_REVIEW");

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
                .HasConstraintName("SYS_C007757");
        });

        modelBuilder.Entity<PayWage>(entity =>
        {
            entity.HasKey(e => e.FinanceId).HasName("SYS_C007764");

            entity.ToTable("PAY_WAGE");

            entity.Property(e => e.FinanceId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("FINANCE_ID");
            entity.Property(e => e.AdministraterId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ADMINISTRATER_ID");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EMPLOYEE_ID");

            entity.HasOne(d => d.Administrater).WithMany(p => p.PayWages)
                .HasForeignKey(d => d.AdministraterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SYS_C007767");

            entity.HasOne(d => d.Employee).WithMany(p => p.PayWages)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SYS_C007766");

            entity.HasOne(d => d.Finance).WithOne(p => p.PayWage)
                .HasForeignKey<PayWage>(d => d.FinanceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SYS_C007765");
        });

        modelBuilder.Entity<Repository>(entity =>
        {
            entity.HasKey(e => e.IngredientId).HasName("SYS_C007772");

            entity.ToTable("REPOSITORY");

            entity.Property(e => e.IngredientId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("INGREDIENT_ID");
            entity.Property(e => e.ExpirationTime)
                .HasColumnType("DATE")
                .HasColumnName("EXPIRATION_TIME");
            entity.Property(e => e.HighConsumption)
                .HasColumnType("NUMBER(1)")
                .HasColumnName("HIGH_CONSUMPTION");
            entity.Property(e => e.IngredientName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("INGREDIENT_NAME");
            entity.Property(e => e.RemainAmount)
                .HasPrecision(10)
                .HasColumnName("REMAIN_AMOUNT");
        });

        modelBuilder.Entity<Restock>(entity =>
        {
            entity.HasKey(e => e.FinanceId).HasName("SYS_C007785");

            entity.ToTable("RESTOCK");

            entity.Property(e => e.FinanceId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("FINANCE_ID");
            entity.Property(e => e.AdministraterId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ADMINISTRATER_ID");
            entity.Property(e => e.IngredientId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("INGREDIENT_ID");
            entity.Property(e => e.Price)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("PRICE");
            entity.Property(e => e.Quantity)
                .HasPrecision(10)
                .HasColumnName("QUANTITY");

            entity.HasOne(d => d.Administrater).WithMany(p => p.Restocks)
                .HasForeignKey(d => d.AdministraterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SYS_C007788");

            entity.HasOne(d => d.Finance).WithOne(p => p.Restock)
                .HasForeignKey<Restock>(d => d.FinanceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SYS_C007786");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.Restocks)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SYS_C007787");
        });

        modelBuilder.Entity<Senior>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("SYS_C007685");

            entity.ToTable("SENIOR");

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
                .HasConstraintName("SYS_C007686");
        });

        modelBuilder.Entity<VolApplication>(entity =>
        {
            entity.HasKey(e => e.ApplicationId).HasName("SYS_C007792");

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
                .HasConstraintName("SYS_C007793");
        });

        modelBuilder.Entity<VolReview>(entity =>
        {
            entity.HasKey(e => new { e.ApplicationId, e.AdministratorId }).HasName("SYS_C007798");

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
                .HasConstraintName("SYS_C007800");

            entity.HasOne(d => d.Application).WithMany(p => p.VolReviews)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SYS_C007799");
        });

        modelBuilder.Entity<Volunteer>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("SYS_C007698");

            entity.ToTable("VOLUNTEER");

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
                .HasConstraintName("SYS_C007699");
        });

        modelBuilder.Entity<Weekmenu>(entity =>
        {
            entity.HasKey(e => new { e.DishId, e.Week }).HasName("SYS_C007710");

            entity.ToTable("WEEKMENU");

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

            entity.HasOne(d => d.Dish).WithMany(p => p.Weekmenus)
                .HasForeignKey(d => d.DishId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SYS_C007711");
        });
        modelBuilder.HasSequence("LOGMNR_DIDS$");
        modelBuilder.HasSequence("LOGMNR_EVOLVE_SEQ$");
        modelBuilder.HasSequence("LOGMNR_SEQ$");
        modelBuilder.HasSequence("LOGMNR_UIDS$").IsCyclic();
        modelBuilder.HasSequence("MVIEW$_ADVSEQ_GENERIC");
        modelBuilder.HasSequence("MVIEW$_ADVSEQ_ID");
        modelBuilder.HasSequence("ROLLING_EVENT_SEQ$");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
