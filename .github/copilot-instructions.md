This is a demo repository to demo a Windows Service using background worker that interacts with New Relic free account by sending Open Telemetry data. It is a learning excerise to understand how to configure Open Telemetry that can be 
exported to New Relic.

The application itself is to find the gender of a list of names.

# Technical Bits

- App will file watch directory to see if a list_of_names.csv exists. The csv format is:

firstname,lastname
Gareth,Bradley
Casey, Bradley

- The app will parse the Name Genderizer RESTful API here https://www.nameapi.org/en/developer/manuals/rest-web-services/53/web-services/name-genderizer/

- The app will create a list_of_names_with_gender.csv that includes the Gender

firstname,lastname, gender
Gareth,Bradley, male
Casey, Bradley, female

# Coding Standards and architecture of application
- Use the latest dotnet sdk.
- The application should be operating system agnostic and we shouldnt build out using specfic windows features
- We should use the custom Backgroundworker template detailed here https://learn.microsoft.com/en-us/dotnet/core/extensions/workers?source=recommendations
- Always build and validate
- Always write unit tests to validate logic
- Tests should be written in Xunit
- The application should feature driven vertical slicing using Meditar. One feature per handler, and chain them where appropriate
