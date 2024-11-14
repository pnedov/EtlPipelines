How to run the project:

1. Clone the repository to your local machine
2. Open solution file PNedov.IsiMarkets.EtlPipeline.sln in Visual Studio
3. Set Multiple start up projects for PNedov.IsiMarkets.EtlPipeline & PNedov.IsiMarkets.EtlPipeline.FakeApi
4. Make sure you have installed MSSQL Server
5. For PNedov.IsiMarkets.EtlPipelineIn appsettings.json instead of 'localhost' set your server name and db name instead of 'localdb' in connection string.
6. Use EF Migration to initialize new DB. Open Developer Command Prompt and run commands: dotnet ef database update

Structure:

1. The main project utilize ASP.NET Core Web API and is designed with a focus on modularity, maintainability, and scalability. 
2. The PNedov.IsiMarkets.EtlPipeline project presents main logic of ETL operations
   1. Managing and Data access APIs:
      - CustomersController - provides API endpoints for managing customer data and their transactions using CRUD operations
      - SystemController - managing system configurations using CRUD operations
   3. Extractors:
      - ApiExtractor - extracts data from an API endpoint (fake entire API)
      - CsvExtractor - extract data from a source CSV file stored in project
      - SqliteExtractor - extract data from a source SQLite DB (not implemented)
   3. Models:
      - RawDataRecord class - represents a raw data record for customer transactions in the ETL pipeline
      - Customers, CustomerTransactions, Products classes - represents main entities
   4. Pipelines:
      - EltPipeline - represents an ETL (Extract, Transform, Load) pipeline that orchestrates the extraction data
      - EtlPiplineService - manages the execution of ETL pipelines at regular intervals
   5. Transformers:
      - AmountFilterTransformer, ConvertTimestampTransformer, RemoveDuplicatesTransformer - classes responsible for remove duplicate records, format string 	 properties, and validate records.
   7. Repositories:
      - CustomersRepository, ProductsRepository, SystemRepository, TransactionsDbContext classes - manages data, including retrieval and upsert operations
      - TransactionsDbContext - custom DbContext for interacting with the TransactionsDB database
3. The PNedov.IsiMarkets.EtlPipeline.FakeApi is a fake API data source returns sample JSON
4. The PNedov.IsiMarkets.EtlPipeline.UnitTests contains unit tests for controllers and transformers

Design considerations:

1. Separation of concerns:
   - The project follows the principle of separation of concerns by dividing responsibilities among different layers:
     - Controllers: Handle HTTP requests and responses
     - Repositories: Handle data access and manipulation
     - Models: Represent the data structure
     - Extractors: Retrieving data from various sources
     - Transformers: Removing duplicates, formatting and validating records
     - Pipeline Services: configure extractors and transformers execution in order optimal data processing  
2. Dependency Injection:
    - The project uses dependency injection to manage dependencies, promoting loose coupling and easier testing. 
3. Asynchronous Programming:
    - The methods in the controller and repository are asynchronous, which helps improve the scalability and responsiveness of the application.
4. Entity Framework Core:
    - The project uses Entity Framework Core for data access and simplifying database operations.
5. ETL pipelines and Managing Data access APIs are implementes in same project because this is evaluation task and it's done to simplify things. 
   Otherwise, for better design decisions ETL pipeline and Controllers should be separated in different projects.
6. The data using for source is structured to provide a comprehensive record of each customer transaction, including details about the customer, product, 
   transaction specifics, and the context of the transaction (e.g., location, payment method, status). 
   This format is useful for importing data into a database or for analysis in data processing tools.
   The source data contains duplicates transactions, multiple repeat transactions with different kind of products with same customer,
   same product with different customers - all raw data processes by transformers following data normalization and stores in MSSQL db tables.
7. In case of this solution should to handle a larger volume of data or add additional data sources the following steps will be taken:
    - Scale the etl vertically (more resources)
    - Shard the input data (eg. by hour it is created) and process it in parallel with multiple instances of the same pipeline (same extractors and transformers)
    - Shard the customer transaction database to reduce the db locking and increase the db throuput
    - Scale the etl horizontally 
    - Optimize extractors and transformers for specific load e.g. specific transformers for specific extractors 
    - Combination from previous points
