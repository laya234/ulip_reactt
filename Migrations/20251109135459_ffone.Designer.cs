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
    [Migration("20251109135459_ffone")]
    partial class ffone
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ULIP_proj.Models.Approval", b =>
                {
                    b.Property<int>("ApprovalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ApprovalId"));

                    b.Property<decimal?>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ApprovalComments")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime?>("ApprovedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ApprovedBy")
                        .HasColumnType("int");

                    b.Property<int>("RequestId")
                        .HasColumnType("int");

                    b.Property<string>("RequestReason")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("RequestType")
                        .HasColumnType("int");

                    b.Property<DateTime>("RequestedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("RequestedBy")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("ApprovalId");

                    b.HasIndex("ApprovedBy");

                    b.HasIndex("RequestedBy");

                    b.ToTable("Approvals");
                });

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

                    b.Property<int>("FundType")
                        .HasColumnType("int");

                    b.Property<int>("RiskLevel")
                        .HasColumnType("int");

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

                    b.Property<DateTime?>("AcceptedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("AgentId")
                        .HasColumnType("int");

                    b.Property<decimal>("CommissionPaid")
                        .HasColumnType("decimal(18,2)");

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

                    b.Property<int>("PolicyStatus")
                        .HasColumnType("int");

                    b.Property<decimal>("PremiumAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("PremiumFrequency")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ProposedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RejectedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("RejectionReason")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<decimal>("SumAssured")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("SurrenderRequestDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("SurrenderRequested")
                        .HasColumnType("bit");

                    b.Property<int?>("SurrenderStatus")
                        .HasColumnType("int");

                    b.Property<decimal?>("SurrenderValue")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TotalPremiumPaid")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("PolicyId");

                    b.HasIndex("PolicyNumber")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Policies");
                });

            modelBuilder.Entity("ULIP_proj.Models.Premium", b =>
                {
                    b.Property<int>("PremiumId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PremiumId"));

                    b.Property<decimal>("CommissionAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("CommissionPaid")
                        .HasColumnType("bit");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("PaidDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("PaymentMethod")
                        .HasColumnType("int");

                    b.Property<int>("PolicyId")
                        .HasColumnType("int");

                    b.Property<decimal>("PremiumAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("TransactionReference")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("PremiumId");

                    b.HasIndex("PolicyId");

                    b.ToTable("Premiums");
                });

            modelBuilder.Entity("ULIP_proj.Models.Sale", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AgentId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("CustomerPhone")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int?>("PolicyId")
                        .HasColumnType("int");

                    b.Property<decimal>("QuotedAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AgentId");

                    b.HasIndex("PolicyId");

                    b.ToTable("Sales");
                });

            modelBuilder.Entity("ULIP_proj.Models.Transaction", b =>
                {
                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TransactionId"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("ApprovedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ApprovedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("FundId")
                        .HasColumnType("int");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("bit");

                    b.Property<decimal>("NAV")
                        .HasColumnType("decimal(18,4)");

                    b.Property<int>("PolicyId")
                        .HasColumnType("int");

                    b.Property<bool>("RequiresApproval")
                        .HasColumnType("bit");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

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

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PanNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<int>("PoliciesSold")
                        .HasColumnType("int");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalCommissionEarned")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("PanNumber")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ULIP_proj.Models.Approval", b =>
                {
                    b.HasOne("ULIP_proj.Models.User", "ApprovedByUser")
                        .WithMany("ApprovedApprovals")
                        .HasForeignKey("ApprovedBy")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("ULIP_proj.Models.User", "RequestedByUser")
                        .WithMany("RequestedApprovals")
                        .HasForeignKey("RequestedBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ApprovedByUser");

                    b.Navigation("RequestedByUser");
                });

            modelBuilder.Entity("ULIP_proj.Models.Policy", b =>
                {
                    b.HasOne("ULIP_proj.Models.User", "User")
                        .WithMany("Policies")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ULIP_proj.Models.Premium", b =>
                {
                    b.HasOne("ULIP_proj.Models.Policy", "Policy")
                        .WithMany("Premiums")
                        .HasForeignKey("PolicyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Policy");
                });

            modelBuilder.Entity("ULIP_proj.Models.Sale", b =>
                {
                    b.HasOne("ULIP_proj.Models.User", "Agent")
                        .WithMany("Sales")
                        .HasForeignKey("AgentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ULIP_proj.Models.Policy", "Policy")
                        .WithMany()
                        .HasForeignKey("PolicyId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Agent");

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
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Fund");

                    b.Navigation("Policy");
                });

            modelBuilder.Entity("ULIP_proj.Models.Fund", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("ULIP_proj.Models.Policy", b =>
                {
                    b.Navigation("Premiums");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("ULIP_proj.Models.User", b =>
                {
                    b.Navigation("ApprovedApprovals");

                    b.Navigation("Policies");

                    b.Navigation("RequestedApprovals");

                    b.Navigation("Sales");
                });
#pragma warning restore 612, 618
        }
    }
}
