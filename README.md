# CSV Haibara

A lightweight and versatile tool for reading and writing CSV files.

<br>

## Install

#### Package Manage Console

    Install-Package CsvHaibara

#### .NET CLI

    dotnet add package CsvHaibara

<br>

## How to use
- Import namespace `CsvHaibaraNt.Core`.

        using CsvHaibaraNt.Core;

<br>

- Get `ICsvHaibara` instance.

        using (ICsvHaibara csvHaibara = CsvHaibaraConfiguration.GetCsvHaibara())
        {
            
        }

<br>

## Let's get started with an example
- Assume that we have class `Employee`.

        public class Employee
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public bool Gender { get; set; }
        }

<br>

- **Serialize** list of employees to a CSV file: call method `SerializeAsync`. Note that if we like not to serialize the header row, set the 3<sup>rd</sup> argument to *false*, or simply do not specify since its default value is *false*. Also, in case list of objects is null, an exception will be thrown;

        await csvHaibara.SerializeAsync<Employee>(path, employees, true);

<br>

- **Deserialize** list of employees from a CSV file: call method `DeserializeAsync`. The 2<sup>nd</sup> argument signifies whether that file has the header row. Setting it likewise the above case when serializing.

        var lst = new List<Employee>();
        await foreach (var emp in csvHaibara.DeserializeAsync<Employee>(path, true))
            lst.Add(emp);

<br>

- In case, there are some columns that we like not to generate when *serializing*, for example: `FirstName` and `Gender`. Call method `Ignore` as follows, before doing serialization.

        csvHaibara.Ignore<Employee>(m => m.FirstName, m => m.Gender);

<br>

- At any time later, when we would like to include all columns to serialize, just call method `IncludeAll`.

        csvHaibara.IncludeAll<Employee>();

<br>

- By default, `CSV Haibara` uses `UTF8` when reading and writing files. If we prefer another encoding standard, simply setting via property `Encoding` to be an appropriate value, e.g. ASCII, UTF7...

        csvHaibara.Encoding = Encoding.ASCII;

<br>

- By default, `CSV Haibara` uses comma as delimiter for data columns. In case we like to use another separator, setting it via property `Delimiter`.

<br>

- By default, `CSV Haibara` uses double quote `"` as the escape character. If we want to choose another one, setting it via property `Escape`.

<br>

- By default, `CSV Haibara` uses `CultureInfo.CurrentCulture` when importing and exporting files. Should we prefer another one, setting it via property `CultureInfo`.

<br>

- **Important**: delimiter, escape character, and decimal point in `CultureInfo` must be three unique values. Otherwise, an exception will be thrown when we try to read/write files.

<br>

- Change header title in CSV exported file by prefered name: if we like to employ custom name other than property name for header column, use attribute `ColumnHeader` to decorate. Also, to use attributes in `CsvHaibara`, let's import namespace `CsvHaibaraNt.CoreModels`.

        [ColumnHeader("Given Name")]
        public string FirstName { get; set; }

<br>

- Map boolean values: the default values for boolean type are True/False. In case, the CSV imported file consists of different values for this data type (like 1/0, Yes/No,...), or when we prefer to employ the custom boolean values when exporting, just use attributes `BooleanFalseValue` and `BooleanTrueVaue`.

        [BooleanFalseValue("0")]
        [BooleanTrueValue("1")]
        public bool Gender { get; set; }

<br>

## Support
- .NET 8.0
- .NET 9.0