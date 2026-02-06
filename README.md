# <span style="color:darkblue">CSV Haibara</span>

A lightweight and powerful tool for reading and writing CSV files.

<br>

---

## Install

#### Package Manage Console

    Install-Package CsvHaibara -Version 1.0.1

#### .NET CLI

    dotnet add package CsvHaibara --version 1.0.1

<br>

---

## How to use
- Import namespace `CsvHaibaraNt.Core`

        using CsvHaibaraNt.Core;

<br>

- Get `ICsvHaibara` instance

        using (ICsvHaibara csvHaibara = CsvHaibaraConfiguration.GetCsvHaibara())
        {
            
        }

<br>

---

## Let's get started with an example
- Assume that we have class `Employee`.

        public class Employee
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public bool Gender { get; set; }
        }
<br>

- **Serialize** list of employees to a CSV file: call `SerializeAsync`. Note that if we wouldn't like to serialize the header row, set the 3<sup>rd</sup> argument to false, or simply do not specify since its default value is false.

        await csvHaibara.SerializeAsync<Employee>(path, employees, true);

<br>

- **Deserialize** list of employees from a CSV file: call `DeserializeAsync`. The 2<sup>nd</sup> argument signifies whether that file has the header row. Setting it likewise the above case when serializing.

        var lst = new List<Employee>();
        await foreach (var emp in csvHaibara.DeserializeAsync<Employee>(path, true))
            lst.Add(emp);

<br>

- In case, there are some columns that we like not to generate when *serializing*, for example: `FirstName` and `Gender`. Call method `Ignore` as follows before doing serialization.

        csvHaibara.Ignore<Employee>(m => m.FirstName, m => m.Gender);

<br>

- Any time later, when we would like include all columns to serialize, just call method `IncludeAll`.

        csvHaibara.IncludeAll<Employee>();

<br>

- By default, `CSV Haibara` uses UTF8 when reading and writing files. If you prefer another encoding standard, simply call method `SetEncoding` and pass the appropriate argument, e.g. ASCII, UTF7...

        csvHaibara.SetEncoding(Encoding.ASCII);

<br>

- Change header title in CSV exported file by prefered name: if we like to use custom name other than property name for header column, use attribute `ColumnHeader` to decorate.

        [ColumnHeader("Given Name")]
        public string FirstName { get; set; }

<br>

- Map boolean values: the default values for boolean type are False/True. In case, the CSV imported file contains different values for this data type (like 0/1, Yes/No,...), or when we prefer to those when exporting, just use attributes `BooleanFalseValue` and `BooleanTrueVaue`.

        [BooleanFalseValue("0")]
        [BooleanTrueValue("1")]
        public bool Gender { get; set; }

<br>

---

## Support
- .NET 8.0
- .NET 9.0