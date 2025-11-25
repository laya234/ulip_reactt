using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ULIP_proj.Data;

#nullable disable

namespace ULIP_proj.Migrations
{
    [DbContext(typeof(ULIPDbContext))]
    [Migration("20251103113736_Ione")]
    partial class Ione
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ULIP_proj.Models.Fund", b =>
                {
                    b.Property<int>("FundId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FundId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("CurrentNAV")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("ExpenseRatio")
                        .HasColumnType("decimal(5,2)");

                    b.Property<string>("FundName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("FundType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("RiskLevel")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("FundId");

                    b.ToTable("Funds");
                });

            modelBuilder.Entity("ULIP_proj.Models.Policy", b =>
                {
                    b.Property<int>("PolicyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PolicyId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("CurrentValue")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("PolicyMaturityDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PolicyName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PolicyNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("PolicyStartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PolicyStatus")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<decimal>("PremiumAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("PremiumFrequency")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("SumAssured")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TotalPremiumPaid")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("PolicyId");

                    b.HasIndex("PolicyNumber")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Policies");
                });

            modelBuilder.Entity("ULIP_proj.Models.PolicyFund", b =>
                {
                    b.Property<int>("PolicyFundId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PolicyFundId"));

                    b.Property<decimal>("AllocationPercentage")
                        .HasColumnType("decimal(5,2)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("CurrentValue")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("FundId")
                        .HasColumnType("int");

                    b.Property<int>("PolicyId")
                        .HasColumnType("int");

                    b.Property<decimal>("Units")
                        .HasColumnType("decimal(18,4)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("PolicyFundId");

                    b.HasIndex("FundId");

                    b.HasIndex("PolicyId", "FundId")
                        .IsUnique();

                    b.ToTable("PolicyFunds");
                });

            modelBuilder.Entity("ULIP_proj.Models.Premium", b =>
                {
                    b.Property<int>("PremiumId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PremiumId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("PaidDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("PolicyId")
                        .HasColumnType("int");

                    b.Property<decimal>("PremiumAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("TransactionReference")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("PremiumId");

                    b.HasIndex("PolicyId");

                    b.ToTable("Premiums");
                });

            modelBuilder.Entity("ULIP_proj.Models.Transaction", b =>
                {
                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TransactionId"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("FundId")
                        .HasColumnType("int");

                    b.Property<decimal>("NAV")
                        .HasColumnType("decimal(18,4)");

                    b.Property<int>("PolicyId")
                        .HasColumnType("int");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("TransactionType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<decimal>("Units")
                        .HasColumnType("decimal(18,4)");

                    b.HasKey("TransactionId");

                    b.HasIndex("FundId");

                    b.HasIndex("PolicyId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("ULIP_proj.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PanNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("PanNumber")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ULIP_proj.Models.Policy", b =>
                {
                    b.HasOne("ULIP_proj.Models.User", "User")
                        .WithMany("Policies")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ULIP_proj.Models.PolicyFund", b =>
                {
                    b.HasOne("ULIP_proj.Models.Fund", "Fund")
                        .WithMany("PolicyFunds")
                        .HasForeignKey("FundId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ULIP_proj.Models.Policy", "Policy")
                        .WithMany("PolicyFunds")
                        .HasForeignKey("PolicyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Fund");

                    b.Navigation("Policy");
                });

            modelBuilder.Entity("ULIP_proj.Models.Premium", b =>
                {
                    b.HasOne("ULIP_proj.Models.Policy", "Policy")
                        .WithMany("Premiums")
                        .HasForeignKey("PolicyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Policy");
                });

            modelBuilder.Entity("ULIP_proj.Models.Transaction", b =>
                {
                    b.HasOne("ULIP_proj.Models.Fund", "Fund")
                        .WithMany("Transactions")
                        .HasForeignKey("FundId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ULIP_proj.Models.Policy", "Policy")
                        .WithMany("Transactions")
                        .HasForeignKey("PolicyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Fund");

                    b.Navigation("Policy");
                });

            modelBuilder.Entity("ULIP_proj.Models.Fund", b =>
                {
                    b.Navigation("PolicyFunds");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("ULIP_proj.Models.Policy", b =>
                {
                    b.Navigation("PolicyFunds");

                    b.Navigation("Premiums");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("ULIP_proj.Models.User", b =>
                {
                    b.Navigation("Policies");
                });
#pragma warning restore 612, 618
        }
    }
}
