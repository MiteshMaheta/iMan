using System;
using System.Collections.Generic;

namespace iMan.Data
{
    public class ConstantData
    {
        public enum Units{
            Kg,
            Gram,
            Gross,
            Piece,
            Dozen
        }
        public static List<String> UnitList = new List<string>() {GetEnumName(Units.Kg), GetEnumName(Units.Gram), GetEnumName(Units.Gross), GetEnumName(Units.Piece), GetEnumName(Units.Dozen) };

        public static string GetEnumName(object UnitIndex)
        {
            return Enum.GetName(typeof(ConstantData.Units), UnitIndex);
        }
        public ConstantData()
        {
        }
    }
}
