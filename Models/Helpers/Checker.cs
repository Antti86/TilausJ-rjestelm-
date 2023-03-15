using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


public static class Checker
{
    public static bool IsNumeric(string s)
    {
        return Regex.IsMatch(s, @"^\d+$");
    }
    public static bool IsNumeric(string s, int min, int max) //Checks if the number is in range of min-max
    {
        if (IsNumeric(s))
        {
            int check = int.Parse(s);
            if (check >= min && check <= max)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    public static bool IsValidDate(string s)
    {
        return Regex.IsMatch(s, "^([0 - 9]{ 1,2}).([0 - 9]{ 1,2}).([0 - 9]{ 4,4})$");
    }
    public static bool IsDecimal(string s)
    {
        return Regex.IsMatch(s, "^[0-9]+[.,][0-9]+$");
    }
}
