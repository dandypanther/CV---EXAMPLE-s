using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Donaldson_Mottors.Models
{
    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    //DBContext the main database for this application
    public class DonaldsonMottorsDbContext : IdentityDbContext<User>
    {
        //db tables
        
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobType> JobTypes { get; set; }
        public DbSet<StockUsed> StockUsed { get; set; }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DonaldsonMottorsDbContext()
            : base("DonaldsonMottorsDBConnection", throwIfV1Schema: false)
        {
            //DBInitializer
            Database.SetInitializer(new DatabaseInitializer());
        }

        public static DonaldsonMottorsDbContext Create()
        {
            return new DonaldsonMottorsDbContext();
        }

        public System.Data.Entity.DbSet<Donaldson_Mottors.Models.Payment> Payments { get; set; }

        //this acts as a cascade delete
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //cascade delete for booking and its jobs
            modelBuilder.Entity<Job>()
                        .HasRequired(j => j.Booking)
                        .WithMany(b => b.Jobs)
                        .HasForeignKey(j => j.BookingId)
                        .WillCascadeOnDelete(true);
        }

    }
    //DBContext the main database for this application
    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    //Database initializer containing the seeding for the DB
    public class DatabaseInitializer : DropCreateDatabaseAlways<DonaldsonMottorsDbContext>
    {
        protected override void Seed(DonaldsonMottorsDbContext context)
        {
            //db seeding
            if (!context.Users.Any())
            {

            
                //create a few roles and stored them in aspnetroles tables

                //create a rolemanager object that will allow us to create roles and store them in the db
                RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                //theses are all the relevant roles in the application
                //................................
                //if the Admin role doesn't exist
                if (!roleManager.RoleExists("Admin"))
                {
                    //create an admin role
                    roleManager.Create(new IdentityRole("Admin"));
                }

                //if the Customer role doesn't exist
                if (!roleManager.RoleExists("Customer"))
                {
                    //create an Customer role
                    roleManager.Create(new IdentityRole("Customer"));
                }

                //if the Manager role doesn't exist
                if (!roleManager.RoleExists("Manager"))
                {
                    //create a Manager role
                    roleManager.Create(new IdentityRole("Manager"));
                }

                //if the mechanic role doesn't exist
                if (!roleManager.RoleExists("Mechanic"))
                {
                    //create a Mechanic role
                    roleManager.Create(new IdentityRole("Mechanic"));
                }
                //if the Accounts Clerk role doesn't exist
                if (!roleManager.RoleExists("Accounts-Clerk"))
                {
                    //create a Mechanic role
                    roleManager.Create(new IdentityRole("Accounts-Clerk"));
                }

                //if the Stock Controller role doesn't exist
                if (!roleManager.RoleExists("Stock-Controller"))
                {
                    //create a Stock Controller
                    roleManager.Create(new IdentityRole("Stock-Controller"));
                }

                //save the new roles to the database

                context.SaveChanges();
                //................................

                // Create a few Users and assign them to the different roles

                //The userManager object allows creating users and store them in the DB
                UserManager<User> userManager = new UserManager<User>(new UserStore<User>(context));

                //If users with the admin@DonaldsonMottors.com username does not exist then
                if (userManager.FindByName("admin@Don.com") == null)
                {
                    //relaxed password validator
                    userManager.PasswordValidator = new PasswordValidator()
                    {
                        RequireDigit = false,
                        RequiredLength = 1,
                        RequireLowercase = false,
                        RequireUppercase = false,
                        RequireNonLetterOrDigit = false
                    };

                    //*****************************************
                    //Seeding the User table with Customers and Staff
                    //Staff:
                    //Admin
                    var admin = new Staff()
                    {
                        UserName = "admin@Don.com",
                        Email = "admin@DonaldsonMottors.com",
                        Name = "Grant Dunnington",
                        Position = "Administrator",
                        Address = "123 abc lane",
                        EmailConfirmed = true,
                        DateOfBirth = new DateTime(2007, 11, 22),
                        IsActive = true,
                    };

                    //add the hashed password to user
                    userManager.Create(admin, "admin123");

                    //add the admin to the admin role
                    userManager.AddToRole(admin.Id, "Admin");

                    //****************************************
                    //Manager
                    var Manager = new Staff()
                    {
                        UserName = "manager@DonaldsonMottors.com",
                        Email = "manager@DonaldsonMottors.com",
                        Name = "David Donaldson",
                        Position = "Manager",
                        Address = "G22 E54",
                        EmailConfirmed = true,
                        DateOfBirth = new DateTime(2004, 8, 25),
                        IsActive = true,
                    };

                    //add the hashed password to user
                    userManager.Create(Manager, "manager123");

                    //add the Manager to the Manager role
                    userManager.AddToRole(Manager.Id, "Manager");

                    //****************************************
                    //Mechanics

                    var Mechanic1 = new Staff()
                    {
                        UserName = "mechanic1@DonaldsonMottors.com",
                        Email = "mechanic1@DonaldsonMottors.com",
                        Name = "John Smith",
                        Position = "Mechanic",
                        Address = "G23 JI4",
                        EmailConfirmed = true,
                        DateOfBirth = new DateTime(2002, 4, 3),
                        IsActive = true,
                    };

                    var Mechanic2 = new Staff()
                    {
                        UserName = "mechanic2@DonaldsonMottors.com",
                        Email = "mechanic2@DonaldsonMottors.com",
                        Name = "Jane Doe",
                        Position = "Mechanic",
                        Address = "G23 PO0",
                        EmailConfirmed = true,
                        DateOfBirth = new DateTime(2004, 3, 7),
                        IsActive = true,
                    };

                    var Mechanic3 = new Staff()
                    {
                        UserName = "mechanic3@DonaldsonMottors.com",
                        Email = "mechanic3@DonaldsonMottors.com",
                        Name = "David South",
                        Position = "Mechanic",
                        Address = "G4 0NN",
                        EmailConfirmed = true,
                        DateOfBirth = new DateTime(2006, 9, 14),
                        IsActive = true,
                    };

                    //add the hashed password to user
                    userManager.Create(Mechanic1, "mechanic123");

                    userManager.Create(Mechanic2, "mechanic123");

                    userManager.Create(Mechanic3, "mechanic123");

                    //add the Mechanics to the Mechanic role
                    userManager.AddToRole(Mechanic1.Id, "Mechanic");

                    userManager.AddToRole(Mechanic2.Id, "Mechanic");

                    userManager.AddToRole(Mechanic3.Id, "Mechanic");

                    //****************************************
                    //Accounts-Clerks

                    var AccountsClerk1 = new Staff()
                    {
                        UserName = "accountsclerk1@DonaldsonMottors.com",
                        Email = "accountsclerk1@DonaldsonMottors.com",
                        Name = "Mark Iplier",
                        Position = "Accounts Clerk",
                        Address = "G22 TV3",
                        EmailConfirmed = true,
                        DateOfBirth = new DateTime(2005, 12, 21),
                        IsActive = true,
                    };

                    var AccountsClerk2 = new Staff()
                    {
                        UserName = "accountsclerk2@DonaldsonMottors.com",
                        Email = "accountsclerk2@DonaldsonMottors.com",
                        Name = "Even Fong",
                        Position = "Accounts Clerk",
                        Address = "E22 B33",
                        EmailConfirmed = true,
                        DateOfBirth = new DateTime(1998, 6, 30),
                        IsActive = true,
                    };

                    //add the hashed password to user
                    userManager.Create(AccountsClerk1, "account123");

                    userManager.Create(AccountsClerk2, "account123");

                    //add the Account clerks to the account clerk role
                    userManager.AddToRole(AccountsClerk1.Id, "Accounts-Clerk");

                    userManager.AddToRole(AccountsClerk2.Id, "Accounts-Clerk");

                    //****************************************
                    //Stock-Controller

                    var StockController1 = new Staff()
                    {
                        UserName = "stockController1@DonaldsonMottors.com",
                        Email = "stockController1@DonaldsonMottors.com",
                        Name = "Alexander Hart",
                        Position = "Stock Controller",
                        Address = "G22 JI7",
                        EmailConfirmed = true,
                        DateOfBirth = new DateTime(2003, 1, 16),
                        IsActive = true,
                    };

                    var StockController2 = new Staff()
                    {
                        UserName = "stockController2@DonaldsonMottors.com",
                        Email = "stockController2@DonaldsonMottors.com",
                        Name = "Louis Micheals",
                        Position = "Stock Controller",
                        Address = "G23 P88",
                        EmailConfirmed = true,
                        DateOfBirth = new DateTime(2001, 2, 27),
                        IsActive = true,
                    };

                    //add the hashed password to user
                    userManager.Create(StockController1, "stock123");

                    userManager.Create(StockController2, "stock123");

                    //add the Stock controllers to the Stock controller role
                    userManager.AddToRole(StockController1.Id, "Stock-Controller");

                    userManager.AddToRole(StockController2.Id, "Stock-Controller");

                    //||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
                    //Customers:

                    var Customer1 = new Customer()
                    {
                        UserName = "McLovin@gmail.com",
                        Email = "McLovin@gmail.com",
                        Name = "Mohammed McLovin",
                        Street = "10 love street",
                        Town = "Glasgow",
                        Postcode = "G22 5EE",
                        TelephoneNo = "07770562055",
                        IsActive = true,
                    };

                    var Customer2 = new Customer()
                    {
                        UserName = "Baker@gmail.com",
                        Email = "Baker@gmail.com",
                        Name = "Tom Baker",
                        Street = "4 Doctor street",
                        Town = "Glasgow",
                        Postcode = "G20 7EA",
                        TelephoneNo = "07780962015",
                        IsActive = true,
                    };

                    var Customer3 = new Customer()
                    {
                        UserName = "WalterMcDowall@gmail.com",
                        Email = "WalterMcDowall@gmail.com",
                        Name = "Walter McDowall",
                        Street = "98 Cathedral Street",
                        Town = "Glasgow",
                        Postcode = "G4 0ND",
                        TelephoneNo = "01415523941",
                        IsActive = true,
                    };

                    //add the hashed password to user
                    userManager.Create(Customer1, "customer123");

                    userManager.Create(Customer2, "customer123");

                    userManager.Create(Customer3, "customer123");

                    //add the Customers to the Customer role
                    userManager.AddToRole(Customer1.Id, "Customer");

                    userManager.AddToRole(Customer2.Id, "Customer");

                    userManager.AddToRole(Customer3.Id, "Customer");

                    //save users to the db
                    context.SaveChanges();//Save them to the db

                    //*****************************************************************
                    //Seeding the JobType table
                    // JobTypes 1 - 5
                    var JobType1 = new JobType() { JobTypeId = "JOB-2026-0001", JobDescription = "Oil and Filter Service", LabourRate = 60, NoOfHours = 1 };
                    var JobType2 = new JobType() { JobTypeId = "JOB-2026-0002", JobDescription = "Front Brake Discs and Pads", LabourRate = 60, NoOfHours = 2 };
                    var JobType3 = new JobType() { JobTypeId = "JOB-2026-0003", JobDescription = "Inspection and Headlight Bulb Replacement", LabourRate = 60, NoOfHours = 1.5 };
                    var JobType4 = new JobType() { JobTypeId = "JOB-2026-0004", JobDescription = "Diagnose Misfire and Replace Spark Plugs", LabourRate = 60, NoOfHours = 2.5 };
                    var JobType5 = new JobType() { JobTypeId = "JOB-2026-0005", JobDescription = "Fit Two All‑Season Tyres and Balance", LabourRate = 60, NoOfHours = 1.2 };
                    // JobTypes 6 - 10
                    var JobType6 = new JobType() { JobTypeId = "JOB-2026-0006", JobDescription = "Replace all four tires and Wheel alignment", LabourRate = 60, NoOfHours = 3 };
                    var JobType7 = new JobType() { JobTypeId = "JOB-2026-0007", JobDescription = "Replace Battery and Electrical system check", LabourRate = 60, NoOfHours = 1.8 };
                    var JobType8 = new JobType() { JobTypeId = "JOB-2026-0008", JobDescription = "Air conditioning re-gas and leak inspection", LabourRate = 60, NoOfHours = 1.1 };
                    var JobType9 = new JobType() { JobTypeId = "JOB-2026-0009", JobDescription = "Rear Brake Discs and Pads", LabourRate = 60, NoOfHours = 1.8 };
                    var JobType10 = new JobType() { JobTypeId = "JOB-2026-0010", JobDescription = "Timing belt and water pump Replacement", LabourRate = 60, NoOfHours = 4.5 };
                
                    //Add each JobType to the JobType table
                    context.JobTypes.Add(JobType1);
                    context.JobTypes.Add(JobType2);
                    context.JobTypes.Add(JobType3);
                    context.JobTypes.Add(JobType4);
                    context.JobTypes.Add(JobType5);
                    context.JobTypes.Add(JobType6);
                    context.JobTypes.Add(JobType7);
                    context.JobTypes.Add(JobType8);
                    context.JobTypes.Add(JobType9);
                    context.JobTypes.Add(JobType10);
                    context.SaveChanges();//Save them to the db

                    //*****************************************************************
                    //Seeding the Supplier table

                    var Supplier1 = new Supplier()
                    {
                        SupplierName = "Eli Oil",
                        Address1 = "145 Clydebank Road",
                        Postcode = "G81 2RX",
                        TelephoneNO = "07741082011",
                        Email = "EliOil@ContactUsEliOil.com"
                    };

                    var Supplier2 = new Supplier()
                    {
                        SupplierName = "Alan Wake's Brake Supplies",
                        Address1 = "88 Forge Industrial Estate",
                        Postcode = "G21 4RT",
                        TelephoneNO = "01415529081",
                        Email = "AlanWake@ContactUsAlanWakesBrakeSupplies.com"
                    };

                    var Supplier3 = new Supplier()
                    {
                        SupplierName = "Bob's bits and bobs | Vehicle Maintence Supplies",
                        Address1 = "12 Duke Street",
                        Address2 = "East End Industrial Units",
                        Postcode = "G31 1JD",
                        TelephoneNO = "01417632209",
                        Email = "contactUs@BobsBitsAndBobs.com"
                    };

                    var Supplier4 = new Supplier()
                    {
                        SupplierName = "VoltLine Electrics",
                        Address1 = "55 Kelvin Industrial Park",
                        Address2 = "Unit 9",
                        Postcode = "G20 8PL",
                        TelephoneNO = "01413317742",
                        Email = "contact@VoltLineElectrics.com"
                    };

                    var Supplier5 = new Supplier()
                    {
                        SupplierName = "Frosty’s Auto Climate Parts",
                        Address1 = "101 Springburn Road",
                        Address2 = "Floor 2, Room 5",
                        Postcode = "G21 1TP",
                        TelephoneNO = "08884249963",
                        Email = "Frosty@ContactFrostyAutoParts.com"
                    };

                    var Supplier6 = new Supplier()
                    {
                        SupplierName = "Michelin",
                        Address1 = "200 Suchiehall st",
                        Postcode = "G22 2RX",
                        TelephoneNO = "07741112011",
                        Email = "contactUs@Michelin.co.uk"
                    };

                    var Supplier7 = new Supplier()
                    {
                        SupplierName = "Spanner & Spin Engine Parts",
                        Address1 = "140 Clydebank Road",
                        Postcode = "G81 2RB",
                        TelephoneNO = "07741082003",
                        Email = "ContactRep@SpannerSpinEngineParts.co.uk"
                    };

                    //add the suppliers to the supplier table

                    context.Suppliers.Add(Supplier1);
                    context.Suppliers.Add(Supplier2);
                    context.Suppliers.Add(Supplier3);
                    context.Suppliers.Add(Supplier4);
                    context.Suppliers.Add(Supplier5);
                    context.Suppliers.Add(Supplier6);
                    context.Suppliers.Add(Supplier7);
                    context.SaveChanges();//Save them to the db

                    //*****************************************************************
                    //Seeding the Stock table
                    //All Stock/Products are organised here by Supplier

                    //Alan Wake's Brake Supplies
                    var Stock1 = new Stock()
                    {
                        ProductName = "Front Brake Discs(pair)",
                        Price = 110,
                        StockLevel = 12,
                        Supplier = Supplier2 //assign this stock/product a supplier
                    };

                    var Stock2 = new Stock()
                    {
                        ProductName = "Front Break Pads (set)",
                        Price = 45,
                        StockLevel = 10,
                        Supplier = Supplier2
                    };

                    var Stock3 = new Stock()
                    {
                        ProductName = "Rear Brake Discs(pair)",
                        Price = 110,
                        StockLevel = 15,
                        Supplier = Supplier2
                    };

                    var Stock4 = new Stock()
                    {
                        ProductName = "Rear Break Pads (set)",
                        Price = 45,
                        StockLevel = 13,
                        Supplier = Supplier2
                    };

                    var Stock5 = new Stock()
                    {
                        ProductName = "Brake Cleaner",
                        Price = 4,
                        StockLevel = 5,
                        Supplier = Supplier2
                    };

                    //Bob's bits and bobs | Vehicle Maintenece Supplies
                    var Stock6 = new Stock()
                    {
                        ProductName = "Sump Washer",
                        Price = 0.80,
                        StockLevel = 22,
                        Supplier = Supplier3
                    };

                    var Stock7 = new Stock()
                    {
                        ProductName = "Spark Plugs (set of 4)",
                        Price = 40,
                        StockLevel = 9,
                        Supplier = Supplier3
                    };

                    var Stock8 = new Stock()
                    {
                        ProductName = "Intake Cleaner",
                        Price = 6,
                        StockLevel = 7,
                        Supplier = Supplier3
                    };

                    //Eli Oil
                    var Stock9 = new Stock()
                    {
                        ProductName = "Oil Filter",
                        Price = 8.50,
                        StockLevel = 7,
                        Supplier = Supplier1
                    };

                    var Stock10 = new Stock()
                    {
                        ProductName = "Fully Synthetic Oil(5L)",
                        Price = 34,
                        StockLevel = 9,
                        Supplier = Supplier1
                    };

                    //Frosty's Auto Climate Parts
                    var Stock11 = new Stock()
                    {
                        ProductName = "R134a Refrigerant",
                        Price = 34.99,
                        StockLevel = 18,
                        Supplier = Supplier5
                    };

                    var Stock12 = new Stock()
                    {
                        ProductName = "UV Leak Detection Dye",
                        Price = 25,
                        StockLevel = 5,
                        Supplier = Supplier5
                    };

                    var Stock13 = new Stock()
                    {
                        ProductName = "A/C System Oil",
                        Price = 30,
                        StockLevel = 9,
                        Supplier = Supplier5
                    };

                    //Michelin
                    var Stock14 = new Stock()
                    {
                        ProductName = " All-Season Typres 205/55 R16",
                        Price = 75,
                        StockLevel = 10,
                        Supplier = Supplier6
                    };

                    var Stock15 = new Stock()
                    {
                        ProductName = "Valves & Balance Weights",
                        Price = 6,
                        StockLevel = 8,
                        Supplier = Supplier6
                    };

                    var Stock16 = new Stock()
                    {
                        ProductName = "Wheel ALignment Service Kit",
                        Price = 22.50,
                        StockLevel = 6,
                        Supplier = Supplier6
                    };

                    //VoltLine Electrics
                    var Stock17 = new Stock()
                    {
                        ProductName = "12V Car Battery (096 Type)",
                        Price = 75,
                        StockLevel = 13,
                        Supplier = Supplier4
                    };

                    var Stock18 = new Stock()
                    {
                        ProductName = "Battery Terminal Grease",
                        Price = 2.26,
                        StockLevel = 20,
                        Supplier = Supplier4
                    };

                    var Stock19 = new Stock()
                    {
                        ProductName = "H7 Headlight Bulb",
                        Price = 9,
                        StockLevel = 8,
                        Supplier = Supplier4
                    };

                    var Stock20 = new Stock()
                    {
                        ProductName = "Fuse Assortment Pack",
                        Price = 6.99,
                        StockLevel = 6,
                        Supplier = Supplier4
                    };

                    //Spanner & Spin Engine Parts 
                    var Stock21 = new Stock()
                    {
                        ProductName = "Timing Belt Kit",
                        Price = 56.98,
                        StockLevel = 3,
                        Supplier = Supplier7
                    };

                    var Stock22 = new Stock()
                    {
                        ProductName = "Water Pump",
                        Price = 47,
                        StockLevel = 11,
                        Supplier = Supplier7
                    };

                    var Stock23 = new Stock()
                    {
                        ProductName = "Coolant (5L)",
                        Price = 19.99,
                        StockLevel = 14,
                        Supplier = Supplier7
                    };

                    var Stock24 = new Stock()
                    {
                        ProductName = "Auxililary Belt",
                        Price = 4.23,
                        StockLevel = 9,
                        Supplier = Supplier7
                    };

                    //save to db
                    context.Stock.Add(Stock1);
                    context.Stock.Add(Stock2);
                    context.Stock.Add(Stock3);
                    context.Stock.Add(Stock4);
                    context.Stock.Add(Stock5);
                    context.Stock.Add(Stock6);
                    context.Stock.Add(Stock7);
                    context.Stock.Add(Stock8);
                    context.Stock.Add(Stock9);
                    context.Stock.Add(Stock10);
                    context.Stock.Add(Stock11);
                    context.Stock.Add(Stock12);
                    context.Stock.Add(Stock13);
                    context.Stock.Add(Stock14);
                    context.Stock.Add(Stock15);
                    context.Stock.Add(Stock16);
                    context.Stock.Add(Stock17);
                    context.Stock.Add(Stock18);
                    context.Stock.Add(Stock19);
                    context.Stock.Add(Stock20);
                    context.Stock.Add(Stock21);
                    context.Stock.Add(Stock22);
                    context.Stock.Add(Stock23);
                    context.Stock.Add(Stock24);
                    context.SaveChanges();



                    //*****************************************************************
                    //Seeding the StockUsed table

                    // JOB-2026-0001 — Oil and Filter Service
                    var StockUsed1 = new StockUsed()
                    {
                        JobType = JobType1,
                        Stock = Stock9, // Oil Filter
                        Quantity = 1,
                    };

                    var StockUsed2 = new StockUsed()
                    {
                        JobType = JobType1,
                        Stock = Stock10, // Fully Synthetic Oil
                        Quantity = 1,
                    };

                    var StockUsed3 = new StockUsed()
                    {
                        JobType = JobType1,
                        Stock = Stock6, // Sump Washer
                        Quantity = 1,
                    };

                    // JOB-2026-0002 — Front Brake Discs and Pads
                    var StockUsed4 = new StockUsed()
                    {
                        JobType = JobType2,
                        Stock = Stock1, // Front Brake Discs
                        Quantity = 1,
                    };

                    var StockUsed5 = new StockUsed()
                    {
                        JobType = JobType2,
                        Stock = Stock2, // Front Brake Pads
                        Quantity = 1,
                    };

                    var StockUsed6 = new StockUsed()
                    {
                        JobType = JobType2,
                        Stock = Stock5, // Brake Cleaner
                        Quantity = 1,
                    };

                    // JOB-2026-0003 — Inspection and Headlight Bulb
                    var StockUsed7 = new StockUsed()
                    {
                        JobType = JobType3,
                        Stock = Stock19, // H7 Headlight Bulb
                        Quantity = 1,
                    };

                    // JOB-2026-0004 — Misfire and Spark Plugs
                    var StockUsed8 = new StockUsed()
                    {
                        JobType = JobType4,
                        Stock = Stock7, // Spark Plugs
                        Quantity = 1,
                    };

                    var StockUsed9 = new StockUsed()
                    {
                        JobType = JobType4,
                        Stock = Stock8, // Intake Cleaner
                        Quantity = 1,
                    };

                    // JOB-2026-0005 — Two Tyres and Balance
                    var StockUsed10 = new StockUsed()
                    {
                        JobType = JobType5,
                        Stock = Stock14, // Tyres
                        Quantity = 2,
                    };

                    var StockUsed11 = new StockUsed()
                    {
                        JobType = JobType5,
                        Stock = Stock15, // Valves & Balance Weights
                        Quantity = 1,
                    };

                    // JOB-2026-0006 — Four Tyres and Alignment
                    var StockUsed12 = new StockUsed()
                    {
                        JobType = JobType6,
                        Stock = Stock14,
                        Quantity = 4,
                    };

                    var StockUsed13 = new StockUsed()
                    {
                        JobType = JobType6,
                        Stock = Stock15,
                        Quantity = 1,
                    };

                    var StockUsed14 = new StockUsed()
                    {
                        JobType = JobType6,
                        Stock = Stock16, // Alignment Kit
                        Quantity = 1,
                    };

                    // JOB-2026-0007 — Battery and Electrical Check
                    var StockUsed15 = new StockUsed()
                    {
                        JobType = JobType7,
                        Stock = Stock17, // Battery
                        Quantity = 1,
                    };

                    var StockUsed16 = new StockUsed()
                    {
                        JobType = JobType7,
                        Stock = Stock18, // Battery Grease
                        Quantity = 1,
                    };

                    var StockUsed17 = new StockUsed()
                    {
                        JobType = JobType7,
                        Stock = Stock20, // Fuse Pack
                        Quantity = 1,
                    };

                    // JOB-2026-0008 — Air Conditioning Service
                    var StockUsed18 = new StockUsed()
                    {
                        JobType = JobType8,
                        Stock = Stock11, // Refrigerant
                        Quantity = 1,
                    };

                    var StockUsed19 = new StockUsed()
                    {
                        JobType = JobType8,
                        Stock = Stock12, // Leak Dye
                        Quantity = 1,
                    };

                    var StockUsed20 = new StockUsed()
                    {
                        JobType = JobType8,
                        Stock = Stock13, // AC Oil
                        Quantity = 1,
                    };

                    // JOB-2026-0009 — Rear Brake Discs and Pads
                    var StockUsed21 = new StockUsed()
                    {
                        JobType = JobType9,
                        Stock = Stock3, // Rear Brake Discs
                        Quantity = 1,
                    };

                    var StockUsed22 = new StockUsed()
                    {
                        JobType = JobType9,
                        Stock = Stock4, // Rear Brake Pads
                        Quantity = 1,
                    };

                    var StockUsed23 = new StockUsed()
                    {
                        JobType = JobType9,
                        Stock = Stock5, // Brake Cleaner
                        Quantity = 1,
                    };

                    // JOB-2026-0010 — Timing Belt and Water Pump
                    var StockUsed24 = new StockUsed()
                    {
                        JobType = JobType10,
                        Stock = Stock21, // Timing Belt
                        Quantity = 1,
                    };

                    var StockUsed25 = new StockUsed()
                    {
                        JobType = JobType10,
                        Stock = Stock22, // Water Pump
                        Quantity = 1,
                    };

                    var StockUsed26 = new StockUsed()
                    {
                        JobType = JobType10,
                        Stock = Stock23, // Coolant
                        Quantity = 1,
                    };

                    var StockUsed27 = new StockUsed()
                    {
                        JobType = JobType10,
                        Stock = Stock24, // Auxiliary Belt
                        Quantity = 1,
                    };

                    //save to db
                    context.StockUsed.Add(StockUsed1);
                    context.StockUsed.Add(StockUsed2);
                    context.StockUsed.Add(StockUsed3);
                    context.StockUsed.Add(StockUsed4);
                    context.StockUsed.Add(StockUsed5);
                    context.StockUsed.Add(StockUsed6);
                    context.StockUsed.Add(StockUsed7);
                    context.StockUsed.Add(StockUsed8);
                    context.StockUsed.Add(StockUsed9);
                    context.StockUsed.Add(StockUsed10);
                    context.StockUsed.Add(StockUsed11);
                    context.StockUsed.Add(StockUsed12);
                    context.StockUsed.Add(StockUsed13);
                    context.StockUsed.Add(StockUsed14);
                    context.StockUsed.Add(StockUsed15);
                    context.StockUsed.Add(StockUsed16);
                    context.StockUsed.Add(StockUsed17);
                    context.StockUsed.Add(StockUsed18);
                    context.StockUsed.Add(StockUsed19);
                    context.StockUsed.Add(StockUsed20);
                    context.StockUsed.Add(StockUsed21);
                    context.StockUsed.Add(StockUsed22);
                    context.StockUsed.Add(StockUsed23);
                    context.StockUsed.Add(StockUsed24);
                    context.StockUsed.Add(StockUsed25);
                    context.StockUsed.Add(StockUsed26);
                    context.StockUsed.Add(StockUsed27);
                    context.SaveChanges();

                    //*****************************************************************
                    //Seeding the Vehicle table

                    var Vehicle1 = new Vehicle() 
                    { 
                
                        VehicleRegNO = "ABC123",
                        Make = "PEUGEOT",
                        Model = "407",
                        Year = 2008,
                        EngineSize = 1800,
                        Miles = 2125,
                        Customer = Customer3,
                        IsValid = true,
                
                    };

                    var Vehicle2 = new Vehicle()
                    {

                        VehicleRegNO = "FX18KLM",
                        Make = "Ford",
                        Model = "Focus",
                        Year = 2018,
                        EngineSize = 1500,
                        Miles = 7201,
                        Customer = Customer2,
                        IsValid = true,

                    };

                    var Vehicle3 = new Vehicle()
                    {

                        VehicleRegNO = "VA65TGH",
                        Make = "Vauxhall",
                        Model = "Crossland",
                        Year = 2015,
                        EngineSize = 1600,
                        Miles = 5420,
                        Customer = Customer1,
                        IsValid = true,

                    };

                    context.Vehicles.Add(Vehicle1);
                    context.Vehicles.Add(Vehicle2);
                    context.Vehicles.Add(Vehicle3);
                    context.SaveChanges();

                    

                    //*****************************************************************
                    //Seeding the Booking table

                    //includes JobType 1 and 10
                    var Booking1 = new Booking()
                    {
                        DateOfBooking = DateTime.Now.AddMonths(-5),
                        CompletionStatus = true,
                        TotalCost = 501.5,
                        AmountPaid = 501.5,
                        Customer = Customer3,
                        Vehicle = Vehicle1,
                        JobDescription = "Oil and Filter Service, Timing belt and water pump Replacement.",
                        Status = "Paid",

                    };

                    var Booking2 = new Booking()
                    {
                        DateOfBooking = DateTime.Now.AddMonths(-4),
                        CompletionStatus = true,
                        TotalCost = 616.5,
                        AmountPaid = 616.5,
                        Customer = Customer2,
                        Vehicle = Vehicle2,
                        JobDescription = "Replace all four tires and Wheel alignment.",
                        Status = "Paid",

                    };

                    var Booking3 = new Booking()
                    {
                        DateOfBooking = DateTime.Now.AddMonths(-3),
                        CompletionStatus = true,
                        TotalCost = 99,
                        AmountPaid = 99,
                        Customer = Customer1,
                        Vehicle = Vehicle3,
                        JobDescription = "Inspection and Headlight Bulb Replacement.",
                        Status = "Paid",

                    };

                    //uncompleted job
                    var Booking4 = new Booking()
                    {
                        DateOfBooking = DateTime.Now.AddDays(-1),
                        CompletionStatus = true,
                        TotalCost = 419.25,
                        AmountPaid = 105.06,
                        Customer = Customer1,
                        Vehicle = Vehicle3,
                        JobDescription = "Fit Two All-Season Tyres and Balance, Replace Battery and Electrical system check.",
                        Status = "Paid",
                    };
                
                    //save to db
                    context.Bookings.Add(Booking1);
                    context.Bookings.Add(Booking2);
                    context.Bookings.Add(Booking3);
                    context.Bookings.Add(Booking4);
                    context.SaveChanges();

                    //*****************************************************************
                    //Seeding the Invoice table

                    var Invoice1 = new Invoice()
                    {
                        CompanyAddress = "145 Clydebank Road. Glasgow United Kingdom, G81 2RX",
                        CompanyInfo = "Contact us at: ContactUs@DonaldsonMottors.com\nOr Contact us at : +44 1410 4852 08",
                        CustomerName = Customer3.Name,
                        CustomerAddress = Customer3.Street + ", " + Customer3.Town + ". " + Customer3.Postcode,
                        CustomerContactInfo = "Contact Customer at: " + Customer3.Email + "\nOr at: " + Customer3.TelephoneNo,
                        LabourCost = 60 * 5.5, //labourRate times NoHours
                        ProductCost = 171.50,
                        TotalCost = (60 * 5.5) + 171.50,
                        Booking = Booking1,
                        NoHrs = 5.5

                    };

                    var Invoice2 = new Invoice()
                    {
                        CompanyAddress = "145 Clydebank Road. Glasgow United Kingdom, G81 2RX",
                        CompanyInfo = "Contact us at: ContactUs@DonaldsonMottors.com\nOr Contact us at : +44 1410 4852 08",
                        CustomerName = Customer2.Name,
                        CustomerAddress = Customer2.Street + ", " + Customer2.Town + ". " + Customer2.Postcode,
                        CustomerContactInfo = "Contact Customer at: " + Customer2.Email + "\nOr at: " + Customer2.TelephoneNo,
                        LabourCost = 60 * 3,
                        ProductCost = (75 * 4) + 6 + 22.50,
                        TotalCost = (60 * 3) + ((75 * 4) + 6 + 22.50),
                        Booking = Booking2,
                        NoHrs = 3

                    };

                    var Invoice3 = new Invoice()
                    {
                        CompanyAddress = "145 Clydebank Road. Glasgow United Kingdom, G81 2RX",
                        CompanyInfo = "Contact us at: ContactUs@DonaldsonMottors.com\nOr Contact us at : +44 1410 4852 08",
                        CustomerName = Customer1.Name,
                        CustomerAddress = Customer1.Street + ", " + Customer1.Town + ". " + Customer1.Postcode,
                        CustomerContactInfo = "Contact Customer at: " + Customer1.Email + "\nOr at: " + Customer1.TelephoneNo,
                        LabourCost = 60 * 1.5,
                        ProductCost = 9,
                        TotalCost = 99,
                        Booking = Booking3,
                        NoHrs = 1.5

                    };

                    //add invoices to db
                    context.Invoices.Add(Invoice1);
                    context.Invoices.Add(Invoice2);
                    context.Invoices.Add(Invoice3);
                    context.SaveChanges();

                    //*****************************************************************
                    //Seeding the Job table

                    //booking1
                    var Job1 = new Job()
                    {
                        JobType = JobType1,
                        CompletionDate = DateTime.Now.AddMonths(-5).AddDays(1),
                        StaffAssigned = Mechanic1,
                        Booking = Booking1,
                    };

                    var Job2 = new Job()
                    {
                        JobType = JobType10,
                        CompletionDate = DateTime.Now.AddMonths(-5).AddDays(1),
                        StaffAssigned = Mechanic2,
                        Booking = Booking2,
                    };

                    //booking2
                    var Job3 = new Job()
                    {
                        JobType = JobType6,
                        CompletionDate = DateTime.Now.AddMonths(-4).AddDays(1),
                        StaffAssigned = Mechanic1,
                        Booking = Booking2,
                    };

                    //booking3

                    var Job4 = new Job()
                    {
                        JobType = JobType3,
                        CompletionDate = DateTime.Now.AddMonths(-3).AddDays(1),
                        StaffAssigned = Mechanic2,
                        Booking = Booking3,
                    };

                    //booking4

                    var Job5 = new Job()
                    {
                        JobType = JobType5,
                        StaffAssigned = Mechanic1,
                        Booking = Booking4,
                        CompletionDate = DateTime.Now
                    };

                    var Job6 = new Job()
                    {
                        JobType = JobType7,
                        StaffAssigned = Mechanic1,
                        Booking = Booking4,
                        CompletionDate = DateTime.Now
                    };

                    //save to db
                    context.Jobs.Add(Job1);
                    context.Jobs.Add(Job2);
                    context.Jobs.Add(Job3);
                    context.Jobs.Add(Job4);
                    context.Jobs.Add(Job5);
                    context.Jobs.Add(Job6);
                    context.SaveChanges();

                    //*****************************************************************
                    //Seeding the Payment table

                    //booking 1 and Customer 3
                    var Payment1 = new Payment()
                    {
                        PaymentMethod = "PayPal",
                        Amount = 501.50,
                        Currency = "Pounds Sterling",
                        PaymentDateTime = DateTime.Now.AddMonths(-5),
                        Status = "Success",
                        PaymentGatewayTransactionReference = null,
                        Booking = Booking1,
                        Customer = Customer3

                    };

                    //Booking2 and Customer2
                    var Payment2 = new Payment()
                    {
                        PaymentMethod = "PayPal",
                        Amount = 616.50,
                        Currency = "Pounds Sterling",
                        PaymentDateTime = DateTime.Now.AddMonths(-4),
                        Status = "Success",
                        PaymentGatewayTransactionReference = null,
                        Booking = Booking2,
                        Customer = Customer2

                    };

                    //Booking3 and Customer1
                    var Payment3 = new Payment()
                    {
                        PaymentMethod = "PayPal",
                        Amount = 99,
                        Currency = "Pounds Sterling",
                        PaymentDateTime = DateTime.Now.AddMonths(-3),
                        Status = "Success",
                        PaymentGatewayTransactionReference = null,
                        Booking = Booking3,
                        Customer = Customer1

                    };

                    //
                    //Booking4 and Customer1
                    var Payment4 = new Payment()
                    {
                        PaymentMethod = "Debbit Card",
                        Amount = 105.06,
                        Currency = "Pounds Sterling",
                        PaymentDateTime = DateTime.Now.AddDays(-1),
                        Status = "Success",
                        PaymentGatewayTransactionReference = null,
                        Booking = Booking4,
                        Customer = Customer1

                    };

                    //save changes made to db
                    context.SaveChanges();

                }//End if

            }//End if
        }//End of Seed method
        
        //Database initializer containing the seeding for the DB
        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    }//End of class
}//End of Namespace