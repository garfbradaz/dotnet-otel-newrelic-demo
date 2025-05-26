# dotnet-otel-newrelic-demo
Demo on using Open Telemetry with dotnet and New Relic

## Issue Templates

When creating a new issue, you can use one of the following templates:

- **Feature Request**: Use the Feature template when proposing new functionality. This template includes sections for user story and acceptance criteria.

## About the Project

This application is a demo that illustrates how to use Open Telemetry with dotnet by creating a background service that processes names and assigns genders to them. Data is sent to New Relic for monitoring and telemetry.

The application functionality:
- Watches a directory for new CSV files containing lists of names
- Processes the names using the Name Genderizer API
- Outputs a new CSV file with gender information added
- Sends telemetry data to New Relic

## Project Structure

The repository consists of:

- **NameGenderizer.Service**: A .NET background worker service that monitors a directory for CSV files containing names and processes them to determine genders
- **NameGenderizer.Tests**: Unit tests for the service components

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- A New Relic account (free tier works for demo purposes)

### Building the Project

```bash
dotnet build
```

### Running the Tests

```bash
dotnet test
```

### Running the Service

```bash
cd NameGenderizer.Service
dotnet run
```

## CSV Format

### Input (list_of_names.csv)
```
firstname,lastname
Gareth,Bradley
Casey,Bradley
```

### Output (list_of_names_with_gender.csv)
```
firstname,lastname,gender
Gareth,Bradley,male
Casey,Bradley,female
```
