using System;
using RaspTracer_AN_Modbus.Models;

namespace RaspTracer_AN_Modbus.Logic
{
    public class AvailableParameters
    {
        public static readonly List<EPEverQuery> parametersToRetrieve = new()
        {
            new EPEverQuery() { Name = "LOAD_VOLTAGE", ByteArray = new byte[] { 0x04, 0x31, 0x0C, 0x00, 0x01 }, Factor = 100, HasHighRegister = false },
            new EPEverQuery() { Name = "LOAD_CURRENT", ByteArray = new byte[] { 0x04, 0x31, 0x0D, 0x00, 0x01 }, Factor = 100, HasHighRegister = false },
            new EPEverQuery() { Name = "BATTERY_VOLTAGE", ByteArray = new byte[] { 0x04, 0x33, 0x1A, 0x00, 0x01 }, Factor = 100, HasHighRegister = false },
            new EPEverQuery() { Name = "BATTERY_CURRENT", ByteArray = new byte[] { 0x04, 0x33, 0x1B, 0x00, 0x01 }, Factor = 100, HasHighRegister = true }, // It should have one, but it only returns 0xFFFF
            new EPEverQuery() { Name = "SOLAR_VOLTAGE", ByteArray = new byte[] { 0x04, 0x31, 0x00, 0x00, 0x01 }, Factor = 100, HasHighRegister = false },
            new EPEverQuery() { Name = "SOLAR_CURRENT", ByteArray = new byte[] { 0x04, 0x31, 0x01, 0x00, 0x01 }, Factor = 100, HasHighRegister = false },
            new EPEverQuery() { Name = "BATTERY_TEMPERATURE", ByteArray = new byte[] { 0x04, 0x31, 0x10, 0x00, 0x01 }, Factor = 100, HasHighRegister = false },
            new EPEverQuery() { Name = "DEVICE_TEMPERATURE", ByteArray = new byte[] { 0x04, 0x31, 0x11, 0x00, 0x01 }, Factor = 100, HasHighRegister = false },
            new EPEverQuery() { Name = "BATTERY_SOC", ByteArray = new byte[] { 0x04, 0x31, 0x1A, 0x00, 0x01 }, Factor = 1, HasHighRegister = false },
            new EPEverQuery() { Name = "TOTAL_GENERATED_ENERGY", ByteArray = new byte[] { 0x04, 0x33, 0x12, 0x00, 0x01 }, Factor = 100, HasHighRegister = true },
            new EPEverQuery() { Name = "TOTAL_GENERATED_ENERGY_YEAR", ByteArray = new byte[] { 0x04, 0x33, 0x10, 0x00, 0x01 }, Factor = 100, HasHighRegister = true },
            new EPEverQuery() { Name = "TOTAL_GENERATED_ENERGY_MONTH", ByteArray = new byte[] { 0x04, 0x33, 0x0E, 0x00, 0x01 }, Factor = 100, HasHighRegister = true },
            new EPEverQuery() { Name = "TOTAL_GENERATED_ENERGY_TODAY", ByteArray = new byte[] { 0x04, 0x33, 0x0C, 0x00, 0x01 }, Factor = 100, HasHighRegister = true },
         };

    }



}

