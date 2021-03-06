// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tally_Payment_API.DataModel;

namespace Tally_Payment_API.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20211001215756_dafa")]
    partial class dafa
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Tally_Payment_API.DataModel.Transactions", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<string>("CardDetails")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Currency")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PayerEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PayerName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PayerPhone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentFrontEndUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResponseCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResponseMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("appfee")
                        .HasColumnType("float");

                    b.Property<string>("authModelUsed")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("authurl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("chargeResponseCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("chargeResponseMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("charged_amount")
                        .HasColumnType("float");

                    b.Property<string>("flwRef")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("merchantfee")
                        .HasColumnType("float");

                    b.Property<string>("metaname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("metavalue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("narration")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("orderRef")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("paymentLinkUniqueString")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("transactionRef")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Tally_Payment_API.DataModel.UserPaymentModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.Property<DateTime>("Expiry")
                        .HasColumnType("datetime2");

                    b.Property<string>("FeesBearer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Means")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Payer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RandomString")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Sendmail")
                        .HasColumnType("bit");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("inData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("outData")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("userPaymentModels");
                });
#pragma warning restore 612, 618
        }
    }
}
