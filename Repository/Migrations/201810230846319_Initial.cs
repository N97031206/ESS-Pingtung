namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 50),
                        Tel = c.String(maxLength: 100),
                        Email = c.String(maxLength: 250),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        Disabled = c.Boolean(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        LastLoginDate = c.DateTime(),
                        LastLogoutDate = c.DateTime(),
                        PasswordFailureCount = c.Int(nullable: false),
                        LastPasswordFailureDate = c.DateTime(),
                        LastLockedDate = c.DateTime(),
                        LastPasswordChangedDate = c.DateTime(),
                        RoleId = c.Guid(nullable: false),
                        OrginId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orgins", t => t.OrginId, cascadeDelete: true)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.OrginId);
            
            CreateTable(
                "dbo.Orgins",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        OrginCode = c.Int(nullable: false, identity: true),
                        OrginName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Alarts",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        StationID = c.Guid(nullable: false),
                        AlartTypeID = c.Guid(nullable: false),
                        AlartContext = c.String(),
                        StartTimet = c.DateTime(nullable: false),
                        EndTimet = c.DateTime(),
                        Disabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AlartTypes", t => t.AlartTypeID, cascadeDelete: true)
                .ForeignKey("dbo.Stations", t => t.StationID, cascadeDelete: true)
                .Index(t => t.StationID)
                .Index(t => t.AlartTypeID);
            
            CreateTable(
                "dbo.AlartTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        AlartTypeCode = c.Int(nullable: false, identity: true),
                        AlartTypeName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Stations",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        StationCode = c.Int(nullable: false, identity: true),
                        StationName = c.String(nullable: false),
                        UUID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Batteries",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        version = c.Single(nullable: false),
                        index = c.Single(nullable: false),
                        modelSerial = c.String(),
                        serialNO = c.String(),
                        name = c.String(),
                        connected = c.Boolean(nullable: false),
                        updateTime = c.DateTime(nullable: false),
                        voltage = c.Single(nullable: false),
                        charging_current = c.Single(nullable: false),
                        discharging_current = c.Single(nullable: false),
                        charging_watt = c.Single(nullable: false),
                        discharging_watt = c.Single(nullable: false),
                        SOC = c.Single(nullable: false),
                        Cycle = c.Single(nullable: false),
                        charge_direction = c.Single(nullable: false),
                        temperature = c.Single(nullable: false),
                        cells_index = c.String(),
                        cells_voltage = c.String(),
                        OV_DIS = c.Boolean(nullable: false),
                        UV_DIS = c.Boolean(nullable: false),
                        OC_DIS = c.Boolean(nullable: false),
                        SC_DIS = c.Boolean(nullable: false),
                        OT_DIS = c.Boolean(nullable: false),
                        UT_DIS = c.Boolean(nullable: false),
                        RV_DIS = c.Boolean(nullable: false),
                        OC0_DIS = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Bulletins",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        title = c.String(nullable: false),
                        context = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        Disabled = c.Boolean(nullable: false),
                        OrginID = c.Guid(nullable: false),
                        AccountID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orgins", t => t.OrginID, cascadeDelete: true)
                .Index(t => t.OrginID);
            
            CreateTable(
                "dbo.ESSObjects",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UpdateDate = c.DateTime(nullable: false),
                        stationUUID = c.String(),
                        stationName = c.String(),
                        GridPowerIDs = c.String(),
                        LoadPowerIDs = c.String(),
                        GeneratorIDs = c.String(),
                        InvertersIDs = c.String(),
                        BatteryIDs = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Generators",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        version = c.Int(nullable: false),
                        index = c.Int(nullable: false),
                        modelSerial = c.String(),
                        serialNO = c.String(),
                        name = c.String(),
                        connected = c.Boolean(nullable: false),
                        UpdateTime = c.DateTime(nullable: false),
                        OilPressure = c.Single(nullable: false),
                        CoolantTemperature = c.Single(nullable: false),
                        OilTemperature = c.Single(nullable: false),
                        FuleLevel = c.Single(nullable: false),
                        InternalFlexibleSenderAnalogueInputType = c.Single(nullable: false),
                        ChargeAlternatorVoltage = c.Single(nullable: false),
                        EngineBatteryVoltage = c.Single(nullable: false),
                        EngineSpeed = c.Single(nullable: false),
                        EngineRunTime = c.Int(nullable: false),
                        NumberOfStarts = c.Int(nullable: false),
                        frequency = c.Single(nullable: false),
                        L1Nvoltage = c.Single(nullable: false),
                        L2Nvoltage = c.Single(nullable: false),
                        L3Nvoltage = c.Single(nullable: false),
                        L1L2voltage = c.Single(nullable: false),
                        L2L3voltage = c.Single(nullable: false),
                        L3L1voltage = c.Single(nullable: false),
                        L1current = c.Single(nullable: false),
                        L2current = c.Single(nullable: false),
                        L3current = c.Single(nullable: false),
                        earthcurrent = c.Single(nullable: false),
                        L1watts = c.Single(nullable: false),
                        L2watts = c.Single(nullable: false),
                        L3watts = c.Single(nullable: false),
                        currentlaglead = c.Single(nullable: false),
                        totalwatts = c.Single(nullable: false),
                        L1VA = c.Single(nullable: false),
                        L2VA = c.Single(nullable: false),
                        L3VA = c.Single(nullable: false),
                        totalVA = c.Single(nullable: false),
                        L1Var = c.Single(nullable: false),
                        L2Var = c.Single(nullable: false),
                        L3Var = c.Single(nullable: false),
                        totalVar = c.Single(nullable: false),
                        powerfactorL1 = c.Single(nullable: false),
                        powerfactorL2 = c.Single(nullable: false),
                        powerfactorL3 = c.Single(nullable: false),
                        averagepowerfactor = c.Single(nullable: false),
                        percentageoffullpower = c.Single(nullable: false),
                        percentageoffullVar = c.Single(nullable: false),
                        positiveKWhours = c.Single(nullable: false),
                        negativeKWhours = c.Single(nullable: false),
                        KVAhours = c.Single(nullable: false),
                        KVArhours = c.Single(nullable: false),
                        ControlStatus = c.String(),
                        NumberOfNamedAlarms = c.Single(nullable: false),
                        EmergencyStop = c.Single(nullable: false),
                        LowOilPressure = c.Single(nullable: false),
                        HighCoolantTemperature = c.Single(nullable: false),
                        LowCoolantTemperature = c.Single(nullable: false),
                        UnderSpeed = c.Single(nullable: false),
                        OverSpeed = c.Single(nullable: false),
                        GeneratorUnderFrequency = c.Single(nullable: false),
                        GeneratorOverFrequency = c.Single(nullable: false),
                        GeneratorLowVoltage = c.Single(nullable: false),
                        GeneratorHighVoltage = c.Single(nullable: false),
                        BatteryLowVoltage = c.Single(nullable: false),
                        BatteryHighVoltage = c.Single(nullable: false),
                        ChargeAlternatorFailure = c.Single(nullable: false),
                        FailToStart = c.Single(nullable: false),
                        FailToStop = c.Single(nullable: false),
                        GeneratorFailToClose = c.Single(nullable: false),
                        MainsFailToClose = c.Single(nullable: false),
                        OilPressureSenderFault = c.Single(nullable: false),
                        LossOfMagneticPickUp = c.Single(nullable: false),
                        MagneticPickUpOpenCircuit = c.Single(nullable: false),
                        GeneratorHighCurrent = c.Single(nullable: false),
                        NoneA = c.Single(nullable: false),
                        LowFuelLevel = c.Single(nullable: false),
                        CANECUWarning = c.Single(nullable: false),
                        CANECUShutdown = c.Single(nullable: false),
                        CANECUDataFail = c.Single(nullable: false),
                        LowOillevelSwitch = c.Single(nullable: false),
                        HighTemperatureSwitch = c.Single(nullable: false),
                        LowFuelLevelSwitch = c.Single(nullable: false),
                        ExpansionUnitWatchdogAlarm = c.Single(nullable: false),
                        kWOverloadAlarm = c.Single(nullable: false),
                        NegativePhaseSequenceCurrentAlarm = c.Single(nullable: false),
                        EarthFaultTripAlarm = c.Single(nullable: false),
                        GeneratorPhaseRotationAlarm = c.Single(nullable: false),
                        AutoVoltageSenseFail = c.Single(nullable: false),
                        MaintenanceAlarm = c.Single(nullable: false),
                        LoadingFrequencyAlarm = c.Single(nullable: false),
                        LoadingVoltageAlarm = c.Single(nullable: false),
                        NoneB = c.Single(nullable: false),
                        NoneC = c.Single(nullable: false),
                        NoneD = c.Single(nullable: false),
                        NoneE = c.Single(nullable: false),
                        GeneratorShortCircuit = c.Single(nullable: false),
                        MainsHighCurrent = c.Single(nullable: false),
                        MainsEarthFault = c.Single(nullable: false),
                        MainsShortCircuit = c.Single(nullable: false),
                        ECUProtect = c.Single(nullable: false),
                        NoneF = c.Single(nullable: false),
                        Message = c.String(),
                        AvailabilityEnergy = c.Single(nullable: false),
                        AvailabilityHour = c.Single(nullable: false),
                        FuelRelay = c.Boolean(nullable: false),
                        StartRelay = c.Boolean(nullable: false),
                        DigitalOutC = c.Boolean(nullable: false),
                        DigitalOutD = c.Boolean(nullable: false),
                        DigitalOutE = c.Boolean(nullable: false),
                        DigitalOutF = c.Boolean(nullable: false),
                        DigitalOutG = c.Boolean(nullable: false),
                        DigitalOutH = c.Boolean(nullable: false),
                        STOPLEDstatus = c.Boolean(nullable: false),
                        MANUALLEDstatus = c.Boolean(nullable: false),
                        TESTLEDstatus = c.Boolean(nullable: false),
                        AUTOLEDstatus = c.Boolean(nullable: false),
                        GENLEDstatus = c.Boolean(nullable: false),
                        GENBREAKERLEDstatus = c.Boolean(nullable: false),
                        MAINSLEDstatus = c.Boolean(nullable: false),
                        MAINSBREAKERLEDstatus = c.Boolean(nullable: false),
                        USERLED1status = c.Boolean(nullable: false),
                        USERLED2statu = c.Boolean(nullable: false),
                        USERLED3status = c.Boolean(nullable: false),
                        USERLED4status = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GridPowers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        version = c.Int(nullable: false),
                        index = c.Int(nullable: false),
                        modelSerial = c.String(),
                        serialNO = c.String(),
                        name = c.String(),
                        date_time = c.DateTime(nullable: false),
                        VA = c.Single(nullable: false),
                        VB = c.Single(nullable: false),
                        VC = c.Single(nullable: false),
                        Vavg = c.Single(nullable: false),
                        Ia = c.Single(nullable: false),
                        Ib = c.Single(nullable: false),
                        Ic = c.Single(nullable: false),
                        In = c.Single(nullable: false),
                        Isum = c.Single(nullable: false),
                        Watt_a = c.Single(nullable: false),
                        Watt_b = c.Single(nullable: false),
                        Watt_c = c.Single(nullable: false),
                        Watt_t = c.Single(nullable: false),
                        Var_a = c.Single(nullable: false),
                        Var_b = c.Single(nullable: false),
                        Var_c = c.Single(nullable: false),
                        Var_t = c.Single(nullable: false),
                        VA_a = c.Single(nullable: false),
                        VA_b = c.Single(nullable: false),
                        VA_c = c.Single(nullable: false),
                        VA_t = c.Single(nullable: false),
                        PF_a = c.Single(nullable: false),
                        PF_b = c.Single(nullable: false),
                        PF_c = c.Single(nullable: false),
                        PF_t = c.Single(nullable: false),
                        Angle_Va = c.Single(nullable: false),
                        Angle_Vb = c.Single(nullable: false),
                        Angle_Vc = c.Single(nullable: false),
                        Angle_Ia = c.Single(nullable: false),
                        Angle_Ib = c.Single(nullable: false),
                        Angle_Ic = c.Single(nullable: false),
                        Frequency = c.Single(nullable: false),
                        Vab = c.Single(nullable: false),
                        Vbc = c.Single(nullable: false),
                        Vca = c.Single(nullable: false),
                        VIIavg = c.Single(nullable: false),
                        kWHt = c.Single(nullable: false),
                        kWHa = c.Single(nullable: false),
                        kWHb = c.Single(nullable: false),
                        kWHc = c.Single(nullable: false),
                        kVarHt = c.Single(nullable: false),
                        kVarHa = c.Single(nullable: false),
                        kVarHb = c.Single(nullable: false),
                        kVarHc = c.Single(nullable: false),
                        kVAHt = c.Single(nullable: false),
                        kVAHa = c.Single(nullable: false),
                        kVAHb = c.Single(nullable: false),
                        kVAHc = c.Single(nullable: false),
                        Demand = c.Single(nullable: false),
                        Prev_Demand = c.Single(nullable: false),
                        Prev_Demand2 = c.Single(nullable: false),
                        Prev_Demand3 = c.Single(nullable: false),
                        Max_Demand_CurrnetMonth = c.Single(nullable: false),
                        Max_Demand_LastMonth = c.Single(nullable: false),
                        Remain_Time = c.Int(nullable: false),
                        IsCurrent = c.Boolean(nullable: false),
                        ErrorMessage = c.String(),
                        event_info = c.Int(nullable: false),
                        event_date_time = c.DateTime(nullable: false),
                        Alarm = c.Int(nullable: false),
                        ELeve = c.Int(nullable: false),
                        EType = c.Int(nullable: false),
                        ELoop = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Inverters",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        version = c.Int(nullable: false),
                        index = c.Int(nullable: false),
                        modelSerial = c.String(),
                        serialNO = c.String(),
                        name = c.String(),
                        connected = c.Boolean(nullable: false),
                        UpdateTime = c.DateTime(nullable: false),
                        DeviceMode = c.String(),
                        InverterFault = c.Boolean(nullable: false),
                        BusOver = c.Boolean(nullable: false),
                        BusUnder = c.Boolean(nullable: false),
                        BusSoftFail = c.Boolean(nullable: false),
                        LINE_FAIL = c.Boolean(nullable: false),
                        OPVShort = c.Boolean(nullable: false),
                        InverterVoltageTooLow = c.Boolean(nullable: false),
                        InverterVoltageTooHigh = c.Boolean(nullable: false),
                        OverTemperature = c.Boolean(nullable: false),
                        FanLocked = c.Boolean(nullable: false),
                        BatteryVoltageHigh = c.Boolean(nullable: false),
                        BatteryLowAlarm = c.Boolean(nullable: false),
                        BatteryUnderShutdown = c.Boolean(nullable: false),
                        OverLoad = c.Boolean(nullable: false),
                        EepromFault = c.Boolean(nullable: false),
                        InverterOverCurrent = c.Boolean(nullable: false),
                        InverterSoftFail = c.Boolean(nullable: false),
                        SelfTestFail = c.Boolean(nullable: false),
                        OP_DC_VoltageOver = c.Boolean(nullable: false),
                        BatOpen = c.Boolean(nullable: false),
                        CurrentSensorFail = c.Boolean(nullable: false),
                        BatteryShort = c.Boolean(nullable: false),
                        PowerLimit = c.Boolean(nullable: false),
                        PV_VoltageHigh = c.Boolean(nullable: false),
                        MPPT_OverloadFault = c.Boolean(nullable: false),
                        MPPT_OverloadWarning = c.Boolean(nullable: false),
                        BatteryTooLowToCharge = c.Boolean(nullable: false),
                        Message = c.String(),
                        ParallelInformation_IsExist = c.Boolean(nullable: false),
                        ParallelInformation_SerialNumber = c.String(),
                        ParallelInformation_WorkMode = c.String(),
                        ParallelInformation_FaultCode = c.String(),
                        ParallelInformation_GridVoltage = c.Single(nullable: false),
                        ParallelInformation_GridFrequency = c.Single(nullable: false),
                        ParallelInformation_ACOutputVoltage = c.Single(nullable: false),
                        ParallelInformation_ACOutputFrequency = c.Single(nullable: false),
                        ParallelInformation_ACOutputApparentPower = c.Single(nullable: false),
                        ParallelInformation_ACOutputActivePower = c.Single(nullable: false),
                        ParallelInformation_LoadPercentage = c.Single(nullable: false),
                        ParallelInformation_BatteryVoltage = c.Single(nullable: false),
                        ParallelInformation_BatteryChargingCurrent = c.Single(nullable: false),
                        ParallelInformation_BatteryCapacity = c.Single(nullable: false),
                        ParallelInformation_PV_InputVoltage = c.Single(nullable: false),
                        ParallelInformation_TotalChargingCurrent = c.Single(nullable: false),
                        ParallelInformation_Total_AC_OutputApparentPower = c.Single(nullable: false),
                        ParallelInformation_TotalOutputActivePower = c.Single(nullable: false),
                        ParallelInformation_Total_AC_OutputPercentage = c.Single(nullable: false),
                        SCC_OK = c.Boolean(nullable: false),
                        AC_Charging = c.Boolean(nullable: false),
                        SCC_Charging = c.Boolean(nullable: false),
                        Battery = c.String(),
                        Line_OK = c.Boolean(nullable: false),
                        loadOn = c.Boolean(nullable: false),
                        ConfigurationChange = c.Boolean(nullable: false),
                        ParallelInformation_OutputMode = c.String(),
                        ParallelInformation_ChargerSourcePriority = c.String(),
                        ParallelInformation_MaxChargerCurrent = c.Single(nullable: false),
                        ParallelInformation_MaxChargerRange = c.Single(nullable: false),
                        ParallelInformation_Max_AC_ChargerCurrent = c.Single(nullable: false),
                        ParallelInformation_PV_InputCurrentForBattery = c.Single(nullable: false),
                        ParallelInformation_BatteryDischargeCurrent = c.Single(nullable: false),
                        GridVoltage = c.Single(nullable: false),
                        GridFrequency = c.Single(nullable: false),
                        AC_OutputVoltage = c.Single(nullable: false),
                        AC_OutputFrequency = c.Single(nullable: false),
                        AC_OutputApparentPower = c.Single(nullable: false),
                        AC_OutputActivePower = c.Single(nullable: false),
                        OutputLoadPercent = c.Single(nullable: false),
                        BUSVoltage = c.Single(nullable: false),
                        BatteryVoltage = c.Single(nullable: false),
                        BatteryChargingCurrent = c.Single(nullable: false),
                        BatteryCapacity = c.Single(nullable: false),
                        InverterHeatSinkTemperature = c.Single(nullable: false),
                        PV_InputCurrentForBattery = c.Single(nullable: false),
                        PV_InputVoltage = c.Single(nullable: false),
                        BatteryVoltageFrom_SCC = c.Single(nullable: false),
                        BatteryDischargeCurrent = c.Single(nullable: false),
                        Has_SBU_PriorityVersion = c.Boolean(nullable: false),
                        ConfigurationStatus_Change = c.Boolean(nullable: false),
                        SCC_FirmwareVersion_Updated = c.Boolean(nullable: false),
                        LoadStatus_On = c.Boolean(nullable: false),
                        BatteryVoltageTOSteadyWhileCharging = c.Boolean(nullable: false),
                        ChargingStatus_On = c.Boolean(nullable: false),
                        ChargingSstatus_SCC_Charging_On = c.Boolean(nullable: false),
                        ChargingStatus_AC_Charging_On = c.Boolean(nullable: false),
                        ChargingStatusCharging = c.String(),
                        ChargingToFloatingMode = c.Boolean(nullable: false),
                        SwitchOn = c.Boolean(nullable: false),
                        BatteryVoltageOffsetForFansOn = c.Single(nullable: false),
                        EEPROM_Version = c.Single(nullable: false),
                        PV_ChargingPower = c.Single(nullable: false),
                        SPM90Voltage = c.Single(nullable: false),
                        SPM90Current = c.Single(nullable: false),
                        SPM90ActivePower = c.Single(nullable: false),
                        SPM90ActiveEnergy = c.Single(nullable: false),
                        SPM90VoltageDirection = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LoadPowers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        version = c.Int(nullable: false),
                        index = c.Int(nullable: false),
                        modelSerial = c.String(),
                        serialNO = c.String(),
                        name = c.String(),
                        connected = c.Boolean(nullable: false),
                        updateTime = c.DateTime(nullable: false),
                        date_Time = c.DateTime(nullable: false),
                        VA = c.Single(nullable: false),
                        VB = c.Single(nullable: false),
                        VC = c.Single(nullable: false),
                        Vavg = c.Single(nullable: false),
                        Ia = c.Single(nullable: false),
                        Ib = c.Single(nullable: false),
                        Ic = c.Single(nullable: false),
                        In = c.Single(nullable: false),
                        Isum = c.Single(nullable: false),
                        Watt_a = c.Single(nullable: false),
                        Watt_b = c.Single(nullable: false),
                        Watt_c = c.Single(nullable: false),
                        Watt_t = c.Single(nullable: false),
                        Var_a = c.Single(nullable: false),
                        Var_b = c.Single(nullable: false),
                        Var_c = c.Single(nullable: false),
                        Var_t = c.Single(nullable: false),
                        VA_a = c.Single(nullable: false),
                        VA_b = c.Single(nullable: false),
                        VA_c = c.Single(nullable: false),
                        VA_t = c.Single(nullable: false),
                        PF_a = c.Single(nullable: false),
                        PF_b = c.Single(nullable: false),
                        PF_c = c.Single(nullable: false),
                        PF_t = c.Single(nullable: false),
                        Angle_Va = c.Single(nullable: false),
                        Angle_Vb = c.Single(nullable: false),
                        Angle_Vc = c.Single(nullable: false),
                        Angle_Ia = c.Single(nullable: false),
                        Angle_Ib = c.Single(nullable: false),
                        Angle_Ic = c.Single(nullable: false),
                        Frequency = c.Single(nullable: false),
                        Vab = c.Single(nullable: false),
                        Vbc = c.Single(nullable: false),
                        Vca = c.Single(nullable: false),
                        VIIavg = c.Single(nullable: false),
                        kWHt = c.Single(nullable: false),
                        kWHa = c.Single(nullable: false),
                        kWHb = c.Single(nullable: false),
                        kWHc = c.Single(nullable: false),
                        kVarHt = c.Single(nullable: false),
                        kVarHa = c.Single(nullable: false),
                        kVarHb = c.Single(nullable: false),
                        kVarHc = c.Single(nullable: false),
                        kVAHt = c.Single(nullable: false),
                        kVAHa = c.Single(nullable: false),
                        kVAHb = c.Single(nullable: false),
                        kVAHc = c.Single(nullable: false),
                        Demand = c.Single(nullable: false),
                        prev_demand = c.Single(nullable: false),
                        prev_demand2 = c.Single(nullable: false),
                        prev_demand3 = c.Single(nullable: false),
                        maxdemand_currnetmonth = c.Single(nullable: false),
                        maxdemand_lastmonth = c.Single(nullable: false),
                        remain_time = c.Single(nullable: false),
                        IsCurrent = c.Boolean(nullable: false),
                        ErrorMessage = c.String(),
                        event_info = c.Int(nullable: false),
                        event_date_time = c.DateTime(nullable: false),
                        Alarm = c.Int(nullable: false),
                        ELeve = c.Int(nullable: false),
                        EType = c.Int(nullable: false),
                        ELoop = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Email = c.String(maxLength: 200),
                        Content = c.String(nullable: false),
                        IsHandled = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bulletins", "OrginID", "dbo.Orgins");
            DropForeignKey("dbo.Alarts", "StationID", "dbo.Stations");
            DropForeignKey("dbo.Alarts", "AlartTypeID", "dbo.AlartTypes");
            DropForeignKey("dbo.Accounts", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Accounts", "OrginId", "dbo.Orgins");
            DropIndex("dbo.Bulletins", new[] { "OrginID" });
            DropIndex("dbo.Alarts", new[] { "AlartTypeID" });
            DropIndex("dbo.Alarts", new[] { "StationID" });
            DropIndex("dbo.Accounts", new[] { "OrginId" });
            DropIndex("dbo.Accounts", new[] { "RoleId" });
            DropTable("dbo.Messages");
            DropTable("dbo.LoadPowers");
            DropTable("dbo.Inverters");
            DropTable("dbo.GridPowers");
            DropTable("dbo.Generators");
            DropTable("dbo.ESSObjects");
            DropTable("dbo.Bulletins");
            DropTable("dbo.Batteries");
            DropTable("dbo.Stations");
            DropTable("dbo.AlartTypes");
            DropTable("dbo.Alarts");
            DropTable("dbo.Roles");
            DropTable("dbo.Orgins");
            DropTable("dbo.Accounts");
        }
    }
}
