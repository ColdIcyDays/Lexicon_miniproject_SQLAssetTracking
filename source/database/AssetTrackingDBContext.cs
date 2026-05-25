using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Lexicon_Miniproject_SQLAssetTracking;

public class AssetTrackingDBContext : DbContext
{
    const string connectionString = "Server=localhost\\sqlexpress; Database=LexiconAssetTracking; Trusted_Connection=True; Encrypt=Optional;";
    
    private DbSet<DBDevice> DBDevices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlServer(connectionString, builder =>
        {
            builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        });

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DBDevice>()
            .OwnsOne(d => d.PurchasePrice);

        modelBuilder.Entity<DBDevice>()
            .HasDiscriminator<string>("DeviceType")
            .HasValue<DBDevice>("Base")
            .HasValue<DBComputerDevice>("Computer")
            .HasValue<DBSmartphoneDevice>("Smartphone");
    }

    internal List<Device> GetAllDevices()
    {
        List<Device> resultDevices = new List<Device>();


        foreach (var dbDevice in DBDevices)
        {
            Device? dev = dbDevice.CreateDevice();
            if (dev != null)
            {
                dev.DBRef = dbDevice;
                resultDevices.Add(dev);
            }
        }
        

        return resultDevices;
    }

    internal bool IsSerialNumberUnique(string aSerialNumber)
    {
        return DBDevices.FirstOrDefault(x => x.SerialNumber == aSerialNumber) == null;
    }
    
    internal List<Device> GetDevices(Func<DBDevice, bool> aWhere)
    {
        var devices = DBDevices.Where(aWhere).OrderBy(device => device.SerialNumber);
        List<Device> resultDevices = new List<Device>();
        foreach (var dbDevice in devices)
        {
            Device? dev = dbDevice.CreateDevice();
            if (dev != null)
            {
                dev.DBRef = dbDevice;
                resultDevices.Add(dev);
            }
        }
        
        return resultDevices;
    }

    internal void AddDevice(Device aDevice)
    {
        //DBDevice? foundDevice = DBDevices.FirstOrDefault(x => x.SerialNumber == aDevice.SerialNumber);

        if (!IsSerialNumberUnique(aDevice.SerialNumber))
        {
            return;
        }
        
        // TODO: A little rough way of doing this, but maybe put this in the ComputerDevice class itself, sort of like DBDevice.CreateDevice?
        if (aDevice.GetType() == typeof(ComputerDevice))
        {
            DBDevices.Add(new DBComputerDevice(aDevice));
        }
        else if (aDevice.GetType() == typeof(SmartphoneDevice))
        {
            DBDevices.Add(new DBSmartphoneDevice(aDevice));
        }
        
        SaveChanges();
    }

    internal void UpdateDevice(Device aDevice)
    {
        if (aDevice.DBRef == null)
        {
            return;
        }
        
        aDevice.DBRef.SetupDBDevice(aDevice);
        SaveChanges();
    }

    internal void DeleteDevice(Device aDevice)
    {
        if (aDevice.DBRef == null)
        {
            return;
        }

        DBDevices.Remove(aDevice.DBRef);
        SaveChanges();
    }
}


internal class DBDevice
{
    public DBDevice()
    {
        
    }
    public DBDevice(Device aDevice)
    {
        SetupDBDevice(aDevice);
    }

    public void SetupDBDevice(Device aDevice)
    {
        PurchasePrice = new DBPrice(aDevice.PurchasePrice.GetLocalValue(), aDevice.PurchasePrice.CurrencyCode);
        PurchaseDate = aDevice.PurchaseDate;
        DeviceBrand = aDevice.DeviceBrand;
        ModelName = aDevice.ModelName;
        OfficeLocation = aDevice.OfficeLocation.ToUpper();
        SerialNumber = aDevice.SerialNumber;

        // TODO: This feels a little sketch, but I belive this is fine?
        aDevice.DBRef = this;
    }
    
    [Key]
    public int DBDevice_Id { get; set; }
    
    public string DeviceBrand { get; set; }
    public string ModelName { get; set; }
    public string OfficeLocation { get; set; }
    public string SerialNumber { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public DBPrice PurchasePrice { get; set; }

    public virtual Device? CreateDevice()
    {
        return null; 
    }
}

internal class DBComputerDevice : DBDevice
{
    //public bool BoolToForceDiscirminator { get; set; }
    
    public DBComputerDevice(){}
    public DBComputerDevice(Device aDevice) : base(aDevice)
    {
    }

    public override Device? CreateDevice()
    {
        return new ComputerDevice(PurchasePrice.GetPrice(), PurchaseDate, DeviceBrand, ModelName, OfficeLocation, SerialNumber);
    }
}

internal class DBSmartphoneDevice : DBDevice
{
    DBSmartphoneDevice(){}
    //public bool BoolToForceDiscirminator2 { get; set; }
    public DBSmartphoneDevice(Device aDevice) : base(aDevice)
    {
    }

    public override Device? CreateDevice()
    {
        return new SmartphoneDevice(PurchasePrice.GetPrice(), PurchaseDate, DeviceBrand, ModelName, OfficeLocation, SerialNumber);
    }
}

internal class DBPrice
{
    public Price GetPrice()
    {
        return new Price(PriceConverter.ConvertFromEuro(PriceConverter.ConvertToEuro(Value, CurrencyCode), "USD"), CurrencyCode);
    }
    
    public DBPrice()
    {
        Value = 0;
        CurrencyCode = "USD";
    }

    public DBPrice(decimal aValue)
    {
        Value = aValue;
        CurrencyCode = "USD";
    }

    public DBPrice(decimal aValue, string aCurrencyCode)
    {
        Value = aValue;
        CurrencyCode = aCurrencyCode;
    }

    public decimal Value { get; set; }
    public string CurrencyCode { get; set; }
}