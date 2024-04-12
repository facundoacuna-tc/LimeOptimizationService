using FluentMigrator;
using LOS.Common.Hash;
using System.Collections.Generic;

namespace LOS.Migrations
{
    [Migration(1000000)]
    public class _1000000_CreateTables : Migration
    {
        public override void Up()
        {
            //create table for Users
            Create.Table("User")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("Username").AsString().NotNullable()
                .WithColumn("Password").AsString().NotNullable()

                .WithColumn("Salt").AsString().Nullable()

                .WithColumn("AccessToken").AsString().Nullable()
                .WithColumn("RefreshToken").AsString().Nullable()

                // TODO: change admin logic
                .WithColumn("IsSysAdmin").AsBoolean().NotNullable()

                .WithColumn("LastLogin").AsDateTime().Nullable()
                .WithColumn("ExpiresIn").AsInt32().Nullable()
                .WithColumn("ExpiresOn").AsInt64().Nullable()
                .WithColumn("Deleted").AsBoolean().NotNullable();

            var salt = Hasher.GenerateSalt();
            Insert.IntoTable("User").Row(new 
                { 
                    Username = "admin", 
                    Password = Hasher.HashPassword(salt, "admin"), 
                    Salt = Hasher.GenerateSalt(), 
                    IsSysAdmin = true, 
                    Deleted = false 
                });

            salt = Hasher.GenerateSalt();
            Insert.IntoTable("User").Row(new
            {
                Username = "test1@test.com",
                Password = Hasher.HashPassword(salt, "XElWz9WPwSLK3y0jUP6KhO"),
                Salt = Hasher.GenerateSalt(),
                IsSysAdmin = false,
                Deleted = false
            });

            salt = Hasher.GenerateSalt();
            Insert.IntoTable("User").Row(new
            {
                Username = "test2@test.com",
                Password = Hasher.HashPassword(salt, "XElWz9WPwSLK3y0jUP6KhO"),
                Salt = Hasher.GenerateSalt(),
                IsSysAdmin = false,
                Deleted = false
            });



            //create table Sensors Types
            Create.Table("SensorType")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("Name").AsString().NotNullable();

            //create table Sensors
            Create.Table("Sensor")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("Description").AsString().NotNullable()
                .WithColumn("SensorTypeId").AsInt32().NotNullable();

            Create.ForeignKey("FK_SensorType_Sensor")
                .FromTable("Sensor").ForeignColumn("SensorTypeId")
                .ToTable("SensorType").PrimaryColumn("Id");

            //create table for Sensors Values
            Create.Table("SensorValue")
                .WithColumn("SensorId").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("Value").AsString().NotNullable()
                .WithColumn("Date").AsDateTime().NotNullable();

            Create.ForeignKey("FK_Sensor_SensorValue")
                .FromTable("SensorValue").ForeignColumn("SensorId")
                .ToTable("Sensor").PrimaryColumn("Id");


        }

        public override void Down()
        {
            Delete.Table("User");
        }
    }
}
